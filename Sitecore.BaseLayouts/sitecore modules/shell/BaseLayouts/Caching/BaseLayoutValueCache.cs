using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.BaseLayouts.Data;
using Sitecore.Caching;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Engines.DataCommands;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Exceptions;

namespace Sitecore.BaseLayouts.Caching
{
    /// <summary>
    ///     Provides caching for layout values.
    /// </summary>
    public class BaseLayoutValueCache : CustomCache, IBaseLayoutValueCache
    {
        public BaseLayoutValueCache(IBaseLayoutSettings settings)
            : base("BaseLayouts.LayoutValueCache", settings.LayoutValueCacheSize)
        {
            foreach (var db in settings.SupportedDatabases.Select(db => Factory.GetDatabase(db, false)).Where(db => db != null))
            {
                InitializeEventHandlers(db);
            }
        }

        /// <summary>
        ///     Gets a layout value from the cache
        /// </summary>
        /// <param name="item">the item</param>
        /// <returns>The cached value, if it exists, or null if there is no value for the field.</returns>
        public virtual string GetLayoutValue(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            return GetString(GetCacheKey(item));
        }

        /// <summary>
        ///     Adds a layout value to the cache
        /// </summary>
        /// <param name="item">the item</param>
        /// <param name="value">the layout value</param>
        public virtual void AddLayoutValue(Item item, string value)
        {
            Assert.ArgumentNotNull(item, "field");
            Assert.ArgumentNotNullOrEmpty(value, "value");
            SetString(GetCacheKey(item), value);
        }

        /// <summary>
        ///     Gets the cache key for the item
        /// </summary>
        /// <param name="item">the item</param>
        /// <returns>the cache key</returns>
        public virtual string GetCacheKey(Item item)
        {
            Assert.ArgumentNotNull(item, "item");

            var dbName = item.Database.Name;
            BaseLayoutItem baseLayoutItem = item;
            var idList = new List<ID>();
            var idSet = new HashSet<ID>();
            while (baseLayoutItem != null)
            {
                if (idSet.Add(baseLayoutItem.ID))
                {
                    idList.Add(baseLayoutItem.ID);
                    baseLayoutItem = baseLayoutItem.BaseLayout;
                }
                else
                {
                    throw new CircularReferenceException("A circular reference was detected in the base layout chain.");
                }
            }
            idList.Reverse();

            return dbName + ":" + string.Join("|", idList);
        }

        internal virtual void ProcessItemUpdate(Item item)
        {
            RemovePrefix(StandardValuesManager.IsStandardValuesHolder(item) ? item.Database.Name : GetCacheKey(item));
        }

        private void InitializeEventHandlers(Database database)
        {
            var dataEngine = database.Engines.DataEngine;
            dataEngine.DeletedItem += DataEngineDeletedItem;
            dataEngine.DeletedItemRemote += DataEngineDeletedItemRemote;
            dataEngine.SavedItem += DataEngineSavedItem;
            dataEngine.SavedItemRemote += DataEngineSavedItemRemote;
        }

        private void DataEngineSavedItemRemote(object sender, ItemSavedRemoteEventArgs e)
        {
            ProcessItemUpdate(e.Item);
        }

        private void DataEngineSavedItem(object sender, ExecutedEventArgs<SaveItemCommand> e)
        {
            ProcessItemUpdate(e.Command.Item);
        }

        private void DataEngineDeletedItemRemote(object sender, ItemDeletedRemoteEventArgs e)
        {
            ProcessItemUpdate(e.Item);
        }

        private void DataEngineDeletedItem(object sender, ExecutedEventArgs<DeleteItemCommand> e)
        {
            ProcessItemUpdate(e.Command.Item);
        }
    }
}