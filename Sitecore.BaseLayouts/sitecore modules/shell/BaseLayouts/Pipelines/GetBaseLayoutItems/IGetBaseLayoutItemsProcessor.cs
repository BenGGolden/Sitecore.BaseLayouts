namespace Sitecore.BaseLayouts.Pipelines.GetBaseLayoutItems
{
    /// <summary>
    ///     Interface for processors of the getBaseLayoutItems pipeline
    /// </summary>
    public interface IGetBaseLayoutItemsProcessor
    {
        /// <summary>
        ///     Runs the processor
        /// </summary>
        /// <param name="args">the pipeline args</param>
        void Process(GetBaseLayoutItemsArgs args);
    }
}