using Sitecore.Shell.Applications.Dialogs.ItemLister;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.BaseLayouts.Abstractions
{
    public class SheerResponseWrapper : ISheerResponse
    {
        /// <summary>
        /// Show an alert
        /// </summary>
        /// <param name="text">alert text</param>
        /// <param name="parameters">paramaters to format the text with</param>
        /// <returns>a client command object</returns>
        public ClientCommand Alert(string text, params string[] parameters)
        {
            return SheerResponse.Alert(text, parameters);
        }

        /// <summary>
        /// Check if the item has been modified
        /// </summary>
        /// <returns></returns>
        public bool CheckModified()
        {
            return SheerResponse.CheckModified();
        }

        /// <summary>
        /// Show a modal dialog to select an item
        /// </summary>
        /// <param name="options">options for the select item dialog</param>
        /// <returns>a client command object</returns>
        public ClientCommand ShowModalDialog(SelectItemOptions options)
        {
            return SheerResponse.ShowModalDialog(options.ToUrlString().ToString(), true);
        }
    }
}