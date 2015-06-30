using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.BaseLayouts
{
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

        public static ID TemplateId
        {
            get
            {
                return GetIdSetting("BaseLayouts.TemplateId", "{8CA74595-41A2-4077-9911-D386687E77BD}");
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
            return ID.Parse(Configuration.Settings.GetSetting(name, defaultValue));
        }
    }
}