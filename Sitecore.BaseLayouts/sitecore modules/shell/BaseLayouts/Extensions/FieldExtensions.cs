using Sitecore.Data.Fields;

namespace Sitecore.BaseLayouts.Extensions
{
    public static class FieldExtensions
    {
        public static bool IsLayoutField(this Field field)
        {
            return field.ID == FieldIDs.LayoutField;
        }
    }
}