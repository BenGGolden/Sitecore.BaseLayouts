namespace Sitecore.BaseLayouts
{
    using System.Collections.Generic;

    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;

    public class BaseLayoutValidator : IBaseLayoutValidator
    {
        /// <summary>
        /// Determines if there is a circular reference in the chain of base layouts.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// True if there is a circular reference.  False if there is not.
        /// </returns>
        public virtual bool HasCircularBaseLayoutReference(Item item)
        {
            Assert.ArgumentNotNull(item, "item");

            var item2 = item;
            var chain = new List<ID>();
            do
            {
                if (chain.Contains(item2.ID))
                {
                    return true;
                }

                chain.Add(item2.ID);
                ReferenceField baseLayoutField = item2.Fields[BaseLayoutSettings.FieldId];
                item2 = baseLayoutField.TargetItem;
            }
            while (item2 != null);

            return false;
        }
    }
}