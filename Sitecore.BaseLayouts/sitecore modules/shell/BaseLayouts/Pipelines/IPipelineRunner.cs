namespace Sitecore.BaseLayouts.Pipelines
{
    /// <summary>
    ///     Runs pipelines as specified by RunnablePipelineArgs
    /// </summary>
    public interface IPipelineRunner
    {
        /// <summary>
        ///     Runs the pipeline specified by the pipeline args
        /// </summary>
        /// <typeparam name="TArgs">A type that extends RunnablePipelineArgs</typeparam>
        /// <param name="args">the runnable pipeline args</param>
        void Run<TArgs>(TArgs args) where TArgs : RunnablePipelineArgs;
    }
}