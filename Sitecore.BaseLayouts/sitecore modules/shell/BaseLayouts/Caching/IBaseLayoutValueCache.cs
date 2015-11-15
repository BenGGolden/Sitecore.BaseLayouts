using Sitecore.Data.Items;

namespace Sitecore.BaseLayouts.Caching
{
    /// <summary>
    ///     Provides caching for layout values.
    /// </summary>
    public interface IBaseLayoutValueCache
    {
        /// <summary>
        ///     Adds a layout value to the cache
        /// </summary>
        /// <param name="item">the item</param>
        /// <param name="value">the layout value</param>
        void AddLayoutValue(Item item, string value);


        /// <summary>
        ///     Gets a layout value from the cache
        /// </summary>
        /// <param name="item">the item</param>
        /// <returns>The cached value, if it exists, or null if there is no value for the field.</returns>
        string GetLayoutValue(Item item);

        /// <summary>
        ///     Clears the cache
        /// </summary>
        void Clear();
    }
}