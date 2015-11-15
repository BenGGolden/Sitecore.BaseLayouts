using Sitecore.BaseLayouts.Abstractions;
using Sitecore.BaseLayouts.Extensions;
using Sitecore.BaseLayouts.Pipelines;
using Sitecore.BaseLayouts.Pipelines.GetBaseLayoutItems;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Sitecore.BaseLayouts.Commands
{
    public class SelectBaseLayoutContextChecker : ICommandContextChecker
    {
        private readonly IPageModeAccess _pageModeAccess;
        private readonly IPipelineRunner _pipelineRunner;

        /// <summary>
        ///     Initializes the context checker
        /// </summary>
        /// <param name="pageModeAccess">an IPageModeAccess implementation</param>
        /// <param name="pipelineRunner">a pipeline runner</param>
        public SelectBaseLayoutContextChecker(IPageModeAccess pageModeAccess, IPipelineRunner pipelineRunner)
        {
            Assert.ArgumentNotNull(pageModeAccess, "pageModeAccess");
            Assert.ArgumentNotNull(pipelineRunner, "pipelineRunner");

            _pageModeAccess = pageModeAccess;
            _pipelineRunner = pipelineRunner;
        }

        /// <summary>
        ///     Determines whether the command is supported for the given item
        /// </summary>
        /// <param name="item">the context item</param>
        /// <returns>a value indicating if the command is supported</returns>
        public bool CanExecute(Item item)
        {
            if (!_pageModeAccess.CanWebEdit() || !_pageModeAccess.CanDesignItem(item) ||
                !item.HasField(BaseLayoutSettings.FieldId))
            {
                return false;
            }

            var args = new GetBaseLayoutItemsArgs(item);
            _pipelineRunner.Run(args);

            return args.BaseLayoutItems.Count > 0;
        }
    }
}