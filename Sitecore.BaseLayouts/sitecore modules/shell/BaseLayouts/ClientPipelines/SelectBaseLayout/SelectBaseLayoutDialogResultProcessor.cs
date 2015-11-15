using Sitecore.BaseLayouts.Pipelines;
using Sitecore.BaseLayouts.Pipelines.SaveBaseLayout;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Sitecore.BaseLayouts.ClientPipelines.SelectBaseLayout
{
    internal class SelectBaseLayoutDialogResultProcessor : IDialogResultProcessor
    {
        private readonly IPipelineRunner _pipelineRunner;

        public SelectBaseLayoutDialogResultProcessor(IPipelineRunner pipelineRunner)
        {
            Assert.ArgumentNotNull(pipelineRunner, "pipelineRunner");
            _pipelineRunner = pipelineRunner;
        }

        public bool ProcessResult(Item item, string result, out string errorMessage)
        {
            errorMessage = string.Empty;

            ShortID sid;
            if (!ShortID.TryParse(result, out sid))
            {
                errorMessage = "Could not get the ID of the selected base layout.";
                return false;
            }

            var itemId = sid.ToID();
            Item baseLayoutItem = null;
            if (itemId != ID.Null)
            {
                baseLayoutItem = item.Database.GetItem(itemId);
                if (baseLayoutItem == null)
                {
                    errorMessage = "The selected base layout item was not found.";
                    return false;
                }
            }

            var args = new SaveBaseLayoutArgs(item) {NewBaseLayoutItem = baseLayoutItem};
            _pipelineRunner.Run(args);
            if (!string.IsNullOrEmpty(args.Message))
            {
                errorMessage = args.Message;
            }

            return args.Successful;
        }
    }
}