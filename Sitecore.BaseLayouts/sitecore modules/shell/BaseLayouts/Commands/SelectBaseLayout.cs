using System.Collections.Specialized;
using Sitecore.BaseLayouts.Abstractions;
using Sitecore.BaseLayouts.ClientPipelines.SelectBaseLayout;
using Sitecore.BaseLayouts.Data;
using Sitecore.BaseLayouts.Pipelines;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.WebEdit.Commands;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.BaseLayouts.Commands
{
    /// <summary>
    /// Command that opens the base layout selection dialog
    /// </summary>
    public class SelectBaseLayout : WebEditCommand
    {
        private readonly ISheerResponse _sheerResponse;
        private readonly ICommandContextChecker _contextChecker;
        private readonly IDialogLocator _dialogLocator;
        private readonly IDialogResultProcessor _dialogResultProcessor;

        public SelectBaseLayout()
        {
            _sheerResponse = new SheerResponseWrapper();
            var runner = new PipelineRunner();
            _contextChecker = new SelectBaseLayoutContextChecker(new PageModeAccess(), runner);
            _dialogLocator = new SelectBaseLayoutDialogLocator(runner);
            _dialogResultProcessor = new SelectBaseLayoutDialogResultProcessor(runner);
        }

        public SelectBaseLayout(ISheerResponse sheerResponse, ICommandContextChecker contextChecker,
            IDialogLocator dialogLocator, IDialogResultProcessor dialogResultProcessor)
        {
            _sheerResponse = sheerResponse;
            _contextChecker = contextChecker;
            _dialogLocator = dialogLocator;
            _dialogResultProcessor = dialogResultProcessor;
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
            if (context.Items.Length != 1)
            {
                return CommandState.Disabled;
            }
            
            return _contextChecker.CanExecute(context.Items[0]) ? CommandState.Enabled : CommandState.Disabled;
        }

        internal virtual void Run(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (!_sheerResponse.CheckModified())
            {
                return;
            }

            BaseLayoutItem currentItem = DeserializeItems(args.Parameters["items"])[0];

            if (!args.IsPostBack)
            {
                var url = _dialogLocator.GetDialogUrl(currentItem);
                if (!string.IsNullOrEmpty(url))
                {
                    _sheerResponse.ShowModalDialog(url);
                    args.WaitForPostBack();
                }
                else
                {
                    _sheerResponse.Alert("Could not get the dialog URL.");
                }
            }
            else if (args.HasResult)
            {
                string message;
                if (_dialogResultProcessor.ProcessResult(currentItem, args.Result, out message))
                {
                    Refresh();
                }
                else
                {
                    _sheerResponse.Alert(string.IsNullOrEmpty(message)
                        ? "An unknown error occured while processing your selection."
                        : message);
                }
            }
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