using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.BaseLayouts.Commands
{
    using System.Collections.Specialized;

    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Shell.Applications.Dialogs.ItemLister;
    using Sitecore.Shell.Applications.WebEdit.Commands;
    using Sitecore.Shell.Framework.Commands;
    using Sitecore.Text;
    using Sitecore.Web;
    using Sitecore.Web.UI.Sheer;

    public class SelectBaseLayout : WebEditCommand
    {
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            if (context.Items.Length != 1)
            {
                return;
            }

            var parameters = new NameValueCollection();
            parameters["items"] = this.SerializeItems(context.Items);
            Context.ClientPage.Start(this, "Run", parameters);
        }

        public override CommandState QueryState(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            if (!WebEditCommand.CanWebEdit() || WebUtil.GetQueryString("mode") != "edit" || context.Items.Length == 0)
            {
                return CommandState.Hidden;
            }

            var currentItem = context.Items[0];
            var field = currentItem.Fields[BaseLayoutSettings.FieldId];
            if (field == null)
            {
                return CommandState.Hidden;
            }

            var items = this.GetBaseLayoutItems(currentItem);
            if (items == null || items.Count == 0)
            {
                return CommandState.Hidden;
            }

            if (!WebEditCommand.CanDesignItem(currentItem))
            {
                return CommandState.Disabled;
            }

            return base.QueryState(context);
        }

        protected virtual List<Item> GetBaseLayoutItems(Item item)
        {
            var folder = item.Database.GetItem("{34A65165-AD43-4DB9-B804-EFA50BEA1F0C}");
            if (folder != null)
            {
                return folder.Children.ToList();
            }

            return null;
        }

        protected void Run(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (!SheerResponse.CheckModified())
            {
                return;
            }

            var currentItem = this.DeserializeItems(args.Parameters["items"])[0];
            if (!args.IsPostBack)
            {
                var items = this.GetBaseLayoutItems(currentItem);
                if (items.Count > 0)
                {
                    var options = new SelectBaseLayoutOptions { Items = items };
                    SheerResponse.ShowModalDialog(options.ToUrlString().ToString(), true);
                    args.WaitForPostBack();
                }
                else
                {
                    SheerResponse.Alert("The base layouts were not found.");
                }
            }
            else
            {
                if (!args.HasResult)
                {
                    return;
                }

                var field = currentItem.Fields[BaseLayoutSettings.FieldId];
                if (field == null)
                {
                    SheerResponse.Alert("Base Layout field not found.");
                    return;
                }

                ShortID sid;
                if (!ShortID.TryParse(args.Result, out sid))
                {
                    SheerResponse.Alert("The selection is invalid.");
                    return;
                }

                ID itemId = sid.ToID();
                if (itemId == BaseLayoutSettings.NullSelectionItemId && field.HasValue)
                {
                    using (new EditContext(currentItem))
                    {
                        field.Reset();
                    }

                    return;
                }

                var baseLayoutItem = Client.ContentDatabase.GetItem(itemId);
                if (baseLayoutItem == null)
                {
                    SheerResponse.Alert("The base layout was not found.");
                    return;
                }

                if (this.HasCircularReference(currentItem, baseLayoutItem))
                {
                    SheerResponse.Alert("Circular reference detected in the base layout chain.");
                    return;
                }

                var idString = itemId.ToString();
                if (!field.Value.Equals(idString, StringComparison.OrdinalIgnoreCase))
                {
                    using (new EditContext(currentItem))
                    {
                        field.Value = idString;
                    }

                    WebEditCommand.Reload();
                }
            }
        }

        private bool HasCircularReference(Item currentItem, Item selectedItem)
        {
            var chain = new List<ID>();
            chain.Add(currentItem.ID);
            var item = selectedItem;
            do
            {
                if (chain.Contains(item.ID))
                {
                    return true;
                }

                chain.Add(item.ID);
                ReferenceField field = item.Fields[BaseLayoutSettings.FieldId];
                item = field == null ? null : field.TargetItem;
            }
            while (item != null);

            return false;
        }
    }
}