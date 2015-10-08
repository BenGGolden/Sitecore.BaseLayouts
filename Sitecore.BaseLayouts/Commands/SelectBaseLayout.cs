using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Sitecore.BaseLayouts.Pipelines;
using Sitecore.BaseLayouts.Pipelines.GetBaseLayoutItems;
using Sitecore.BaseLayouts.Pipelines.SaveBaseLayout;
using Sitecore.BaseLayouts.UI;
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
        private readonly ISheerResponse _sheerResponse;
        private readonly PipelineRunner _pipelineRunner;

        public SelectBaseLayout() : this(new SheerResponseWrapper(), new PipelineRunner())
        {
        }

        public SelectBaseLayout(ISheerResponse sheerResponse, PipelineRunner pipelineRunner)
        {
            _sheerResponse = sheerResponse;
            _pipelineRunner = pipelineRunner;
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
            RunClientPipeline(parameters);
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

        internal virtual void Run(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (!_sheerResponse.CheckModified())
            {
                return;
            }

            BaseLayoutItem currentItem = DeserializeItems(args.Parameters["items"])[0];
            if (!TemplateManager.IsFieldPartOfTemplate(BaseLayoutSettings.FieldId, currentItem))
            {
                _sheerResponse.Alert("This item does not support base layouts.");
                return;
            }

            if (!args.IsPostBack)
            {
                var items = GetBaseLayoutItems(currentItem);
                if (items != null && items.Count > 0)
                {
                    var options = new SelectBaseLayoutOptions {Items = items};
                    if (currentItem.BaseLayout != null)
                    {
                        options.CurrentBaseLayoutId = currentItem.BaseLayout.ID;
                    }

                    _sheerResponse.ShowModalDialog(options);
                    args.WaitForPostBack();
                }
                else
                {
                    _sheerResponse.Alert("The base layouts were not found.");
                }
            }
            else if (args.HasResult)
            {
                ProcessResult(currentItem, args.Result);
            }
        }

        internal virtual void ProcessResult(BaseLayoutItem currentItem, string result)
        {
            ShortID sid;
            if (!ShortID.TryParse(result, out sid))
            {
                _sheerResponse.Alert("Could not get the ID of the selected base layout.");
                return;
            }

            var itemId = sid.ToID();
            Item baseLayoutItem = null;
            if (itemId != ID.Null)
            {
                baseLayoutItem = currentItem.Database.GetItem(itemId);
                if (baseLayoutItem == null)
                {
                    _sheerResponse.Alert("The selected base layout item was not found.");
                    return;
                }
            }

            string message;
            if (SaveBaseLayout(currentItem, baseLayoutItem, out message))
            {
                Refresh();
            }
            else
            {
                _sheerResponse.Alert(message);
            }
        }

        internal virtual List<Item> GetBaseLayoutItems(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            var args = new GetBaseLayoutItemsArgs(item);
            _pipelineRunner.Run(args);
            return args.BaseLayoutItems;
        }

        internal virtual bool SaveBaseLayout(BaseLayoutItem item, Item baseLayoutItem, out string message)
        {
            Assert.ArgumentNotNull(item, "item");
            message = string.Empty;
            var args = new SaveBaseLayoutArgs(item) {NewBaseLayoutItem = baseLayoutItem};
            _pipelineRunner.Run(args);
            if (!string.IsNullOrEmpty(args.Message))
            {
                message = args.Message;
            }

            return args.Successful;
        }

        internal virtual bool CanEdit()
        {
            return CanWebEdit() && WebUtil.GetQueryString("mode") == "edit";
        }

        internal virtual bool CanDesign(Item item)
        {
            return Policy.IsAllowed("Page Editor/Can Design") && CanDesignItem(item);
        }

        internal virtual void Refresh()
        {
            Reload();
        }

        internal virtual void RunClientPipeline(NameValueCollection parameters)
        {
            Context.ClientPage.Start(this, "Run", parameters);
        }
    }
}