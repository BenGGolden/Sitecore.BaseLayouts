using Sitecore.BaseLayouts.Data;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;

namespace Sitecore.BaseLayouts.Extensions
{
    /// <summary>
    ///     Extension methods for Sitecore.Data.Items.Item
    /// </summary>
    public static class ItemExtensions
    {
        /// <summary>
        ///     Determines if the item's template contains a field with the given field ID
        /// </summary>
        /// <param name="item">the item</param>
        /// <param name="fieldId">the field ID</param>
        /// <returns></returns>
        public static bool HasField(this Item item, ID fieldId)
        {
            return TemplateManager.IsFieldPartOfTemplate(fieldId, item);
        }

        /// <summary>
        ///     Determines if the item's template contains a field with the given field ID
        /// </summary>
        /// <param name="item">the item</param>
        /// <param name="fieldId">the field ID</param>
        /// <returns></returns>
        public static bool HasField(this BaseLayoutItem item, ID fieldId)
        {
            return item.InnerItem.HasField(fieldId);
        }
    }
}