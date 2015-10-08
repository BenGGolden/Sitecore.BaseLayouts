using Sitecore.Diagnostics;
using Sitecore.Pipelines;

namespace Sitecore.BaseLayouts.Pipelines
{
    public class PipelineRunner
    {
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