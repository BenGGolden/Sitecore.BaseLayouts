using Sitecore.Diagnostics;
using Sitecore.Pipelines;

namespace Sitecore.BaseLayouts.Pipelines
{
    /// <summary>
    ///     Runs pipelines as specified by RunnablePipelineArgs
    /// </summary>
    public class PipelineRunner : IPipelineRunner
    {
        /// <summary>
        ///     Runs the pipeline specified by the pipeline args
        /// </summary>
        /// <typeparam name="TArgs">A type that extends RunnablePipelineArgs</typeparam>
        /// <param name="args">the runnable pipeline args</param>
        public virtual void Run<TArgs>(TArgs args) where TArgs : RunnablePipelineArgs
        {
            Assert.ArgumentNotNull(args, "args");

            using (new LongRunningOperationWatcher(1000, args.WatcherMessage))
            {
                CorePipeline.Run(args.PipelineName, args);
            }
        }
    }
}