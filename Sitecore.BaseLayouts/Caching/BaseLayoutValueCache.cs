using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Caching;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Engines;
using Sitecore.Data.Engines.DataCommands;
using Sitecore.Data.Events;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Sitecore.BaseLayouts.Caching
{
    /// <summary>
    /// Provides caching for layout values.
    /// </summary>
    public class BaseLayoutValueCache : CustomCache, IBaseLayoutValueCache
    {
        /// <summary>
        /// Initializes the cache using a configuration setting for the supported databases
        /// </summary>
        public BaseLayoutValueCache() : this(BaseLayoutSettings.SupportedDatabases)
        {
        }

        /// <summary>
        /// Initializes the cache
        /// </summary>
        /// <param name="databases">the names of databases to suppport</param>
        public BaseLayoutValueCache(string[] databases) : base("BaseLayouts.LayoutValueCache", BaseLayoutSettings.LayoutValueCacheSize)
        {
            foreach (var database in databases)
            {
                var db = Factory.GetDatabase(database, false);
                if (db != null)
                {
                    InitializeEventHandlers(db);
                }
            }
        }

        /// <summary>
        /// Gets a layout value from the cache
        /// </summary>
        /// <param name="field">the field</param>
        /// <returns>The cached value, if it exists, or null if there is no value for the field.</returns>
        public virtual string GetLayoutValue(Field field)
        {
            Assert.ArgumentNotNull(field, "field");
            return GetString(GetCacheKey(field.Item));
        }

        /// <summary>
        /// Adds a layout value to the cache
        /// </summary>
        /// <param name="field">the field</param>
        /// <param name="value">the layout value</param>
        public virtual void AddLayoutValue(Field field, string value)
        {
            Assert.ArgumentNotNull(field, "field");
            Assert.ArgumentNotNullOrEmpty(value, "value");
            SetString(GetCacheKey(field.Item), value);
        }

        /// <summary>
        /// Gets the cache key for the item
        /// </summary>
        /// <param name="item">the item</param>
        /// <returns>the cache key</returns>
        protected virtual string GetCacheKey(Item item)
        {
            Assert.ArgumentNotNull(item, "item");

            BaseLayoutItem baseLayoutItem = item;
            var sb = new StringBuilder(baseLayoutItem.Database.Name + ":");
            var baseLayouts = new List<BaseLayoutItem> {baseLayoutItem};
            while (baseLayoutItem.BaseLayout != null)
            {
                baseLayoutItem = baseLayoutItem.BaseLayout;
                baseLayouts.Add(baseLayoutItem);
            }
            baseLayouts.Reverse();

            foreach (var baseLayout in baseLayouts)
            {
                sb.Append(baseLayout.ID);
                sb.Append("|");
            }

            return sb.ToString();
        }

        internal virtual void ProcessItemUpdate(Item item)
        {
            RemovePrefix(StandardValuesManager.IsStandardValuesHolder(item) ? item.Database.Name : GetCacheKey(item));
        }

        private void InitializeEventHandlers(Database database)
        {
            DataEngine dataEngine = database.Engines.DataEngine;
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