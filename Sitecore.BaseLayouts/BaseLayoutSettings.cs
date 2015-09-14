namespace Sitecore.BaseLayouts
{
    using Sitecore.Configuration;
    using Sitecore.Data;

    public static class BaseLayoutSettings
    {
        public static ID FieldId
        {
            get
            {
                return GetIdSetting("BaseLayouts.FieldId", "{FBC10515-95D6-4559-BAD4-C235148DDECE}");
            }
        }

        public static ID NullSelectionItemId
        {
            get
            {
                return GetIdSetting("BaseLayouts.NullSelectionItemId", "{BE3E0E2D-9E3A-4757-A354-7EF9E3EE856B}");
            }
        }

        private static ID GetIdSetting(string name, string defaultValue)
        {
            return ID.Parse(Settings.GetSetting(name, defaultValue));
        }
    }
}