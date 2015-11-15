using Sitecore.BaseLayouts.Data;
using Sitecore.Diagnostics;

namespace Sitecore.BaseLayouts.Pipelines.SaveBaseLayout
{
    /// <summary>
    ///     Arguments for the saveBaseLayout pipeline
    /// </summary>
    public class SaveBaseLayoutArgs : RunnablePipelineArgs
    {
        /// <summary>
        ///     Whether the save operation was successful
        /// </summary>
        public bool Successful;

        /// <summary>
        ///     Initializes an instance of SaveBaseLayoutArgs
        /// </summary>
        /// <param name="item">the item to be saved</param>
        public SaveBaseLayoutArgs(BaseLayoutItem item)
        {
            Assert.ArgumentNotNull(item, "item");
            Item = item;
            OldBaseLayoutItem = item.BaseLayout;
        }

        /// <summary>
        ///     The item being saved with a new base layout
        /// </summary>
        public BaseLayoutItem Item { get; }

        /// <summary>
        ///     The ID of the old base layout
        /// </summary>
        public BaseLayoutItem OldBaseLayoutItem { get; private set; }

        /// <summary>
        ///     The ID of the new base layout
        /// </summary>
        public BaseLayoutItem NewBaseLayoutItem { get; set; }

        public override string PipelineName
        {
            get { return "saveBaseLayout"; }
        }

        public override string WatcherMessage
        {
            get
            {
                return string.Format("{0} pipeline[item={1}, baseLayout={2}]", PipelineName, Item.InnerItem.Paths.Path,
                    NewBaseLayoutItem == null ? "null" : NewBaseLayoutItem.InnerItem.Paths.Path);
            }
        }
    }
}