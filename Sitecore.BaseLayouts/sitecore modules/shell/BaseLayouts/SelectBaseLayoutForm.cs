// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The select base layout form.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using Sitecore.BaseLayouts.Commands;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.Dialogs.ItemLister;
using Sitecore.Shell.Applications.Dialogs.SelectItemWithThumbnail;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.BaseLayouts
{
    /// <summary>
    ///     The select base layout form.
    /// </summary>
    public class SelectBaseLayoutForm : SelectItemWithThumbnailForm
    {
        /// <summary>
        ///     The previews.
        /// </summary>
        protected Scrollbox Previews;

        /// <summary>
        ///     The is item selectable.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        protected override bool IsItemSelectable(Item item)
        {
            return true;
        }

        /// <summary>
        ///     The on load.
        /// </summary>
        /// <param name="e">
        ///     The e.
        /// </param>
        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            base.OnLoad(e);
            if (Context.ClientPage.IsEvent)
            {
                return;
            }

            var options = SelectItemOptions.Parse<SelectBaseLayoutOptions>();
            Previews.InnerHtml = RenderPreviews(options.Items, options.CurrentBaseLayoutId);
            if (!ID.IsNullOrEmpty(options.CurrentBaseLayoutId))
            {
                SelectedItemId = options.CurrentBaseLayoutId.ToShortID().ToString();
            }
        }

        /// <summary>
        ///     The on ok.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        protected override void OnOK(object sender, EventArgs args)
        {
            SheerResponse.SetDialogValue(SelectedItemId);
            SheerResponse.CloseWindow();
        }

        /// <summary>
        ///     The render previews.
        /// </summary>
        /// <param name="items">
        ///     The items.
        /// </param>
        /// <param name="selectedItemId">
        ///     The selected item id.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        internal virtual string RenderPreviews(IEnumerable<Item> items, ID selectedItemId)
        {
            Assert.ArgumentNotNull(items, "items");
            var output = new HtmlTextWriter(new StringWriter());

            foreach (var item in items)
            {
                if (item.ID == selectedItemId)
                {
                    RenderSelectedItemPreview(item, output);
                }
                else
                {
                    RenderItemPreview(item, output);
                }
            }

            return output.InnerWriter.ToString();
        }

        /// <summary>
        ///     Render the selected item preview.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <param name="output">
        ///     The output.
        /// </param>
        internal virtual void RenderSelectedItemPreview(Item item, HtmlTextWriter output)
        {
            var writer = new HtmlTextWriter(new StringWriter());
            RenderItemPreview(item, writer);

            output.Write(writer.InnerWriter.ToString().Replace("scItemThumbnail", "scItemThumbnailSelected"));
        }

        /// <summary>
        ///     Render the item preview.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <param name="output">
        ///     The output.
        /// </param>
        new internal virtual void RenderItemPreview(Item item, HtmlTextWriter output)
        {
            base.RenderItemPreview(item, output);
        }
    }
}