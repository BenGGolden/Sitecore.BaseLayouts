using System;
using System.Linq;
using Sitecore.BaseLayouts.Abstractions;
using Sitecore.BaseLayouts.Extensions;
using Sitecore.Data.Fields;
using Sitecore.Diagnostics;

namespace Sitecore.BaseLayouts.Data
{
    /// <summary>
    /// Gets the value of the layout field from the base layout if one is selected.
    /// </summary>
    public class BaseLayoutValueProvider : IBaseLayoutValueProvider
    {
        /// <summary>
        /// Gets the value of the layout field from the base layout if one is selected.
        /// </summary>
        /// <param name="field">The layout field.</param>
        /// <returns>The merged layout field value of the base layout chain.</returns>
        public virtual string GetBaseLayoutValue(Field field)
        {
            // Get the item selected in the Base Layout field.  Otherwise, exit.
            var baseLayoutItem = new BaseLayoutItem(field.Item).BaseLayout;
            if (baseLayoutItem == null)
            {
                return null;
            }
            
            // Get the value of the layout field on the base layout.
            // If the selected item also has a base layout selected, this will cause implicit recursion.
            return new LayoutField(baseLayoutItem).Value;
        }
    }
}