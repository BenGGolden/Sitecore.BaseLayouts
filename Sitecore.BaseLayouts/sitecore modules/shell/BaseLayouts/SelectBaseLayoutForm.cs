// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The select base layout form.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Shell.Applications.Dialogs.ItemLister;
using Sitecore.Shell.Applications.Dialogs.SelectItemWithThumbnail;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Version = Sitecore.Data.Version;

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
            options.Items.Insert(0, CreateNullSelectionItem());
            Previews.InnerHtml = RenderPreviews(options.Items, options.CurrentBaseLayoutId);
            if (!ID.IsNullOrEmpty(options.CurrentBaseLayoutId))
            {
                SelectedItemId = options.CurrentBaseLayoutId.ToShortID().ToString();
            }
        }

        protected override void SelectableItemPreview_Click(string id, string language, string version)
        {
            if (id == ID.Null.ToString())
            {
                if (!string.IsNullOrEmpty(SelectedItemId))
                {
                    SheerResponse.SetAttribute(string.Format("I{0}", SelectedItemId), "class", "scItemThumbnail");
                }
                SelectedItemId = ID.Null.ToShortID().ToString();
                SheerResponse.SetAttribute(string.Format("I{0}", SelectedItemId), "class", "scItemThumbnailSelected");
            }
            else
            {
                base.SelectableItemPreview_Click(id, language, version);
            }
        }

        protected override void SelectableItemPreview_DblClick(string id, string language, string version)
        {
            if (id == ID.Null.ToString())
            {
                SheerResponse.SetDialogValue(id);
            }

            base.SelectableItemPreview_DblClick(id, language, version);
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
        internal new virtual void RenderItemPreview(Item item, HtmlTextWriter output)
        {
            base.RenderItemPreview(item, output);
        }

        private Item CreateNullSelectionItem()
        {
            var definition = new ItemDefinition(ID.Null, "None", TemplateIDs.StandardTemplate, ID.Null);
            var fields = new FieldList {{FieldIDs.Icon, "Applications/32x32/selection_delete.png"}};
            var data = new ItemData(definition, Language.Current, Version.First, fields);
            var item = new Item(ID.Null, data, Context.ContentDatabase);
            item.RuntimeSettings.Temporary = true;
            return item;
        }
    }
}