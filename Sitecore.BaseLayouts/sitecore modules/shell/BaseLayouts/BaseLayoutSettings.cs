using System;
using Sitecore.Configuration;
using Sitecore.Data;

namespace Sitecore.BaseLayouts
{
    /// <summary>
    ///     Provides config settings related to Base Layouts.
    /// </summary>
    public class BaseLayoutSettings : IBaseLayoutSettings
    {
        private static readonly long _layoutValueCacheSize =
            StringUtil.ParseSizeString(Settings.GetSetting("BaseLayouts.LayoutValueCacheSize", "10MB"));

        private static readonly string[] _supportedDatabases = Settings.GetSetting("BaseLayouts.SupportedDatabases",
            "master|web")
            .Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);

        private static readonly bool _alwaysCheckForCircularReference =
            Settings.GetBoolSetting("BaseLayouts.AlwaysCheckForCircularReference", false);

        /// <summary>
        ///     The size of the layout value cache
        /// </summary>
        public long LayoutValueCacheSize
        {
            get { return _layoutValueCacheSize; }
        }

        /// <summary>
        ///     The names of databases to provide base layout support for
        /// </summary>
        public string[] SupportedDatabases
        {
            get { return _supportedDatabases; }
        }

        /// <summary>
        ///     Should the BaseLayoutStandardValuesProvider always check for a circular reference?
        ///     Default is false since the BaseLayoutValueCache does this anyway and throws an exception
        ///     if a circular reference is detected.  If disabling the cache or adding an IBaseLayoutValueProvider
        ///     decorator, strongly consider setting this to true.
        /// </summary>
        public bool AlwaysCheckForCircularReference
        {
            get { return _alwaysCheckForCircularReference; }
        }
    }
}