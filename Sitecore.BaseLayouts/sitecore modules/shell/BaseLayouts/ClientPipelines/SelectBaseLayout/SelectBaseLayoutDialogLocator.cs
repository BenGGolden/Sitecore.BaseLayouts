using Sitecore.BaseLayouts.Data;
using Sitecore.BaseLayouts.Pipelines;
using Sitecore.BaseLayouts.Pipelines.GetBaseLayoutItems;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Text;

namespace Sitecore.BaseLayouts.ClientPipelines.SelectBaseLayout
{
    /// <summary>
    /// Gets the URL for the Select Base Layout dialog
    /// </summary>
    public class SelectBaseLayoutDialogLocator : IDialogLocator
    {
        private readonly IPipelineRunner _pipelineRunner;

        public SelectBaseLayoutDialogLocator(IPipelineRunner pipelineRunner)
        {
            Assert.ArgumentNotNull(pipelineRunner, "pipelineRunner");
            _pipelineRunner = pipelineRunner;
        }

        /// <summary>
        /// Gets the select base layout dialog URL with parameters for the given item
        /// </summary>
        /// <param name="item">the item</param>
        /// <returns>the dialog url</returns>
        public string GetDialogUrl(Item item)
        {
            var args = new GetBaseLayoutItemsArgs(item);
            _pipelineRunner.Run(args);

            if (args.BaseLayoutItems.Count == 0) return null;

            BaseLayoutItem baseLayoutItem = item;
            var options = new SelectBaseLayoutOptions {Items = args.BaseLayoutItems};
            if (baseLayoutItem.BaseLayout != null)
            {
                options.CurrentBaseLayoutId = baseLayoutItem.BaseLayout.ID;
            }

            return GetUrl(options, item.Database);
        }

        internal virtual string GetUrl(SelectBaseLayoutOptions options, Database db)
        {
            return options.ToUrlString(db).ToString();
        }
    }
}