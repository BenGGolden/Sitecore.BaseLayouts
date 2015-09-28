namespace Sitecore.BaseLayouts.Pipelines.SaveBaseLayout
{
    /// <summary>
    /// Processor for the saveBaseLayout pipeline
    /// </summary>
    public interface ISaveBaseLayoutProcessor
    {
        /// <summary>
        /// runs the processor
        /// </summary>
        /// <param name="args">the pipeline arguments</param>
        void Process(SaveBaseLayoutArgs args);
    }
}