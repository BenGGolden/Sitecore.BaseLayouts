// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Item extensions methods
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Sitecore.BaseLayouts
{
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Data.Managers;
    using Sitecore.Data.Templates;

    /// <summary>
    /// The item extensions.
    /// </summary>
    public static class ItemExtensions
    {
        /// <summary>
        /// Determines if the item is based on the template with the given ID.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="templateId">
        /// The template id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsDerived(this Item item, ID templateId)
        {
            if (item == null || templateId.IsNull)
            {
                return false;
            }

            if (item.TemplateID == templateId)
            {
                return true;
            }

            var template = TemplateManager.GetTemplate(item);
            return template != null && template.DescendsFromOrEquals(templateId);
        }
    }
}