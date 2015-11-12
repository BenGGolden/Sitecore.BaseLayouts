using System;
using System.Collections.Generic;
using Sitecore.BaseLayouts.Extensions;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Xml.Patch;
using System.Linq;

namespace Sitecore.BaseLayouts.Data
{
    public class BaseLayoutValidator : IBaseLayoutValidator
    {
        private readonly string[] _databases;

        public BaseLayoutValidator() : this(BaseLayoutSettings.SupportedDatabases)
        {
        }

        public BaseLayoutValidator(string[] databases)
        {
            Assert.ArgumentNotNull(databases, "databases");
            _databases = databases;
        }

        /// <summary>
        /// Determines if the field is a layout field on an item that supports base layouts.
        /// </summary>
        /// <param name="item">The item</param>
        /// <returns>A bool indicating whether the field is a layout field on an item that supports base layouts.</returns>
        public virtual bool ItemSupportsBaseLayouts(Item item)
        {
            return _databases.Contains(item.Database.Name, StringComparer.OrdinalIgnoreCase)
                   && item.Paths.IsContentItem && item.HasField(BaseLayoutSettings.FieldId);
        }

        /// <summary>
        /// Determines if there is a circular reference in the base layout chain.
        /// </summary>
        /// <param name="item">the item</param>
        /// <returns>True if the item's base layout chain contains a circular reference.  Otherwise, false.</returns>
        public virtual bool HasCircularBaseLayoutReference(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            return HasDuplicateBaseLayout(item, new HashSet<ID>());
        }

        /// <summary>
        /// Determines if a circular referece would be created if baseLayoutItem were set
        /// as the base layout for item
        /// </summary>
        /// <param name="item">the item </param>
        /// <param name="baseLayoutItem">the candidate base layout item</param>
        /// <returns>True if a circular reference would be created. Otherwise, false.</returns>
        public virtual bool CreatesCircularBaseLayoutReference(Item item, Item baseLayoutItem)
        {
            Assert.ArgumentNotNull(item, "item");
            Assert.ArgumentNotNull(baseLayoutItem, "baseLayoutItem");
            Assert.ArgumentCondition(item.HasField(BaseLayoutSettings.FieldId), "item",
                "item does not have a Base Layout field");

            return HasDuplicateBaseLayout(baseLayoutItem, new HashSet<ID> {item.ID});
        }

#if FINAL_LAYOUT
        public bool CreatesVersioningConflict(Item item, Item baseLayoutItem)
        {
            var itemLayout = item.Fields[FieldIDs.LayoutField].GetValue(false, false);
            return !string.IsNullOrWhiteSpace(itemLayout) && XmlPatchUtils.IsXmlPatch(itemLayout) &&
                   !string.IsNullOrWhiteSpace(baseLayoutItem.Fields[FieldIDs.FinalLayoutField].GetValue(false, false));
        }
#endif

        /// <summary>
        /// Determines if there are duplicat IDs in the baseLayouts ID set and the chain of base layout IDs originating at item.
        /// </summary>
        /// <param name="item">the item</param>
        /// <param name="baseLayouts">the starting set of IDs</param>
        /// <returns>True if there are duplicate IDs.  False if not.</returns>
        protected virtual bool HasDuplicateBaseLayout(BaseLayoutItem item, HashSet<ID> baseLayouts)
        {
            var item2 = item;
            do
            {
                if (!baseLayouts.Add(item2.ID))
                {
                    return true;
                }
                
                item2 = item2.BaseLayout;
            }
            while (item2 != null);

            return false;
        }
    }
}