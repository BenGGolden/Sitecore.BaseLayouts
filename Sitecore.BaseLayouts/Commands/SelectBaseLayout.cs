using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Sitecore.BaseLayouts.Pipelines;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore.SecurityModel;
using Sitecore.Shell.Applications.WebEdit.Commands;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.BaseLayouts.Commands
{
    /// <summary>
    /// Command that opens the base layout selection dialog
    /// </summary>
    public class SelectBaseLayout : WebEditCommand
    {
        private readonly IBaseLayoutValidator _validator;

        /// <summary>
        /// Initializes the SelectBaseLayout command
        /// </summary>
        public SelectBaseLayout() : this(new BaseLayoutValidator())
        {
        }


        /// <summary>
        /// Initializes the SelectBaseLayout command with the provided BaseLayoutValidator
        /// </summary>
        /// <param name="validator"></param>
        public SelectBaseLayout(IBaseLayoutValidator validator)
        {
            Assert.ArgumentNotNull(validator, "validator");
            _validator = validator;
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="context"></param>
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            if (context.Items.Length != 1)
            {
                return;
            }

            var parameters = new NameValueCollection();
            parameters["items"] = SerializeItems(context.Items);
            Context.ClientPage.Start(this, "Run", parameters);
        }

        /// <summary>
        /// Determines the display state of the command button.
        /// </summary>
        /// <param name="context">The command context</param>
        /// <returns>The state that the button should display in</returns>
        public override CommandState QueryState(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            if (!CanEdit() || context.Items.Length == 0)
            {
                return CommandState.Hidden;
            }

            var currentItem = context.Items[0];
            if (!TemplateManager.IsFieldPartOfTemplate(BaseLayoutSettings.FieldId, currentItem))
            {
                return CommandState.Hidden;
            }

            var items = GetBaseLayoutItems(currentItem);
            if (items == null || items.Count == 0)
            {
                return CommandState.Hidden;
            }

            if (!CanDesign(currentItem))
            {
                return CommandState.Disabled;
            }

            return base.QueryState(context);
        }

        internal virtual List<Item> GetBaseLayoutItems(Item item)
        {
            using (new LongRunningOperationWatcher(1000, "getBaseLayoutItems pipeline[item={0}]", item.Paths.Path))
            {
                var args = new GetBaseLayoutItemsArgs(item);
                CorePipeline.Run("getBaseLayoutItems", args);
                return args.BaseLayoutItems;
            }
        }

        protected void Run(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (!SheerResponse.CheckModified())
            {
                return;
            }

            var currentItem = DeserializeItems(args.Parameters["items"])[0];
            if (!args.IsPostBack)
            {
                var items = GetBaseLayoutItems(currentItem);
                if (items.Count > 0)
                {
                    var options = new SelectBaseLayoutOptions {Items = items};
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

                var itemId = sid.ToID();
                if (itemId == ID.Null && field.HasValue)
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

                if (_validator.CreatesCircularBaseLayoutReference(currentItem, baseLayoutItem))
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

                    Reload();
                }
            }
        }

        internal virtual bool CanEdit()
        {
            return CanWebEdit() && WebUtil.GetQueryString("mode") == "edit";
        }

        internal virtual bool CanDesign(Item item)
        {
            return Policy.IsAllowed("Page Editor/Can Design") && CanDesignItem(item);
        }
    }
}