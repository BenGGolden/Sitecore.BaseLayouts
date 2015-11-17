using Sitecore.Data;

namespace Sitecore.BaseLayouts
{
    public interface IBaseLayoutSettings
    {
        /// <summary>
        ///     The size of the layout value cache
        /// </summary>
        long LayoutValueCacheSize { get; }

        /// <summary>
        ///     The names of databases to provide base layout support for
        /// </summary>
        string[] SupportedDatabases { get; }

        /// <summary>
        ///     Should the BaseLayoutStandardValuesProvider always check for a circular reference?
        /// </summary>
        bool AlwaysCheckForCircularReference { get; }
    }
}