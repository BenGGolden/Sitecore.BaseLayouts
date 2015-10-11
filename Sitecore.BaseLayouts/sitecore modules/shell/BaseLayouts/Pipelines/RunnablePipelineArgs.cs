using Sitecore.Pipelines;

namespace Sitecore.BaseLayouts.Pipelines
{
    public abstract class RunnablePipelineArgs : PipelineArgs
    {
        /// <summary>
        /// Gets the name of the pipeline for which this args class is used
        /// </summary>
        public abstract string PipelineName { get; }

        /// <summary>
        /// Gets a debug message describing the pipeline and its args
        /// </summary>
        public abstract string  WatcherMessage { get; }
    }
}