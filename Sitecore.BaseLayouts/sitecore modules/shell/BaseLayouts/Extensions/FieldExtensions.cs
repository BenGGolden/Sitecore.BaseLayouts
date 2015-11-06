using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Fields;

namespace Sitecore.BaseLayouts.sitecore_modules.shell.BaseLayouts.Extensions
{
    public static class FieldExtensions
    {
        public static bool IsLayoutField(this Field field)
        {
            return field.ID == FieldIDs.LayoutField;
        }
    }
}