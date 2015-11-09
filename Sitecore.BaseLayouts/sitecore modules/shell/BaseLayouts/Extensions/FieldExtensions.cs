using Sitecore.Data.Fields;

namespace Sitecore.BaseLayouts.Extensions
{
    public static class FieldExtensions
    {
        public static bool IsLayoutField(this Field field)
        {
#if SC80
            return field.ID == FieldIDs.LayoutField || field.ID == FieldIDs.FinalLayoutField;
#else
            return field.ID == FieldIDs.LayoutField;
#endif
        }
    }
}