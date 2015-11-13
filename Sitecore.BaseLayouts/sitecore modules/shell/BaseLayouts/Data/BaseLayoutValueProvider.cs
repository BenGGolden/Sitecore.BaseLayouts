using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Sitecore.BaseLayouts.Data
{
    /// <summary>
    ///     Gets the value of the layout field from the base layout if one is selected.
    /// </summary>
    public class BaseLayoutValueProvider : IBaseLayoutValueProvider
    {
        /// <summary>
        ///     Gets the value of the layout field from the base layout if one is selected.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The merged layout field value of the base layout chain.</returns>
        public virtual string GetBaseLayoutValue(Item item)
        {
            // Get the item selected in the Base Layout field.  Otherwise, exit.
            var baseLayout = new BaseLayoutItem(item).BaseLayout;
            if (baseLayout == null)
            {
                return null;
            }

            // Get the value of the layout field on the base layout.
            // If the selected item also has a base layout selected, this will cause implicit recursion.
            return new LayoutField(baseLayout).Value;
        }
    }
}