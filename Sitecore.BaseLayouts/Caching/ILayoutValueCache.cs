using Sitecore.Data.Fields;

namespace Sitecore.BaseLayouts.Caching
{
    /// <summary>
    /// Provides caching for layout values.
    /// </summary>
    public interface ILayoutValueCache
    {

        /// <summary>
        /// Adds a layout value to the cache
        /// </summary>
        /// <param name="field">the field</param>
        /// <param name="value">the layout value</param>
        void AddLayoutValue(Field field, string value);


        /// <summary>
        /// Gets a layout value from the cache
        /// </summary>
        /// <param name="field">the field</param>
        /// <returns>The cached value, if it exists, or null if there is no value for the field.</returns>
        string GetLayoutValue(Field field);

        /// <summary>
        /// Clears the cache
        /// </summary>
        void Clear();
    }
}