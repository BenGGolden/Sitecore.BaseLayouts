using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;

namespace Sitecore.BaseLayouts.Pipelines.SaveBaseLayout
{
    /// <summary>
    /// Arguments for the saveBaseLayout pipeline
    /// </summary>
    public class SaveBaseLayoutArgs : PipelineArgs
    {
        /// <summary>
        /// Initializes an instance of SaveBaseLayoutArgs
        /// </summary>
        /// <param name="item">the item to be saved</param>
        public SaveBaseLayoutArgs(BaseLayoutItem item)
        {
            Assert.ArgumentNotNull(item, "item");
            Item = item;
            OldBaseLayoutItem = item.BaseLayout;
        }

        /// <summary>
        /// The item being saved with a new base layout
        /// </summary>
        public BaseLayoutItem Item { get; private set; }

        /// <summary>
        /// The ID of the old base layout
        /// </summary>
        public BaseLayoutItem OldBaseLayoutItem { get; private set; }

        /// <summary>
        /// The ID of the new base layout
        /// </summary>
        public BaseLayoutItem NewBaseLayoutItem { get; set; }

        /// <summary>
        /// Whether the save operation was successful
        /// </summary>
        public bool Successful;
    }
}