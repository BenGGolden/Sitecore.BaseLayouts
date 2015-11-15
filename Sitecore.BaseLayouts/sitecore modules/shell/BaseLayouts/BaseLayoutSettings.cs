using System;
using Sitecore.Configuration;
using Sitecore.Data;

namespace Sitecore.BaseLayouts
{
    /// <summary>
    ///     Provides config settings related to Base Layouts.
    /// </summary>
    public static class BaseLayoutSettings
    {
        /// <summary>
        ///     The ID of the base layout field
        /// </summary>
        public static readonly ID FieldId = GetIdSetting("BaseLayouts.FieldId", "{FBC10515-95D6-4559-BAD4-C235148DDECE}");

        /// <summary>
        ///     The ID of the item that represents no selection ("None") in the Select Base Layout dialog
        /// </summary>
        public static readonly ID NullSelectionItemId = GetIdSetting("BaseLayouts.NullSelectionItemId",
            "{BE3E0E2D-9E3A-4757-A354-7EF9E3EE856B}");

        /// <summary>
        ///     The size of the layout value cache
        /// </summary>
        public static readonly long LayoutValueCacheSize =
            StringUtil.ParseSizeString(Settings.GetSetting("BaseLayouts.LayoutValueCacheSize", "10MB"));

        /// <summary>
        ///     The names of databases to provide base layout support for
        /// </summary>
        public static readonly string[] SupportedDatabases = Settings.GetSetting("BaseLayouts.SupportedDatabases",
            "master|web")
            .Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);


        private static ID GetIdSetting(string name, string defaultValue)
        {
            return ID.Parse(Settings.GetSetting(name, defaultValue));
        }
    }
}