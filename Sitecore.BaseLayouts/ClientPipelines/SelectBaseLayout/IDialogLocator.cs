using Sitecore.Data.Items;

namespace Sitecore.BaseLayouts.ClientPipelines.SelectBaseLayout
{
    /// <summary>
    /// Gets a dialog URL
    /// </summary>
    public interface IDialogLocator
    {
        /// <summary>
        /// Gets the dialog URL with parameters for the given item
        /// </summary>
        /// <param name="item">the item</param>
        /// <returns>the dialog url</returns>
        string GetDialogUrl(Item item);
    }
}