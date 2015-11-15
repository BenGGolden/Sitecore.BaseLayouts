using Sitecore.Web.UI.HtmlControls;

namespace Sitecore.BaseLayouts.Abstractions
{
    /// <summary>
    ///     Abstraction of the parts of SheerResponse that are needed for the Base Layouts dialog
    /// </summary>
    public interface ISheerResponse
    {
        /// <summary>
        ///     Show an alert
        /// </summary>
        /// <param name="text">alert text</param>
        /// <param name="parameters">paramaters to format the text with</param>
        /// <returns>a client command object</returns>
        ClientCommand Alert(string text, params string[] parameters);

        /// <summary>
        ///     Check if the item has been modified
        /// </summary>
        /// <returns></returns>
        bool CheckModified();

        /// <summary>
        ///     Show a modal dialog to select an item
        /// </summary>
        /// <param name="url">the dialog url</param>
        /// <returns>a client command object</returns>
        ClientCommand ShowModalDialog(string url);
    }
}