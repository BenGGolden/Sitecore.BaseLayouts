using System.Collections.Generic;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Sitecore.BaseLayouts.Pipelines.GetBaseLayoutItems
{
    /// <summary>
    ///     Pipeline args for the getBaseLayoutItems pipeline
    /// </summary>
    public class GetBaseLayoutItemsArgs : RunnablePipelineArgs
    {
        /// <summary>
        ///     Initializes a GetBaseLayoutItemArgs
        /// </summary>
        /// <param name="item"></param>
        public GetBaseLayoutItemsArgs(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            Item = item;
            BaseLayoutItems = new List<Item>();
        }

        /// <summary>
        ///     The Item to get base layouts for.
        /// </summary>
        public Item Item { get; set; }


        /// <summary>
        ///     The resulting list of base layout items
        /// </summary>
        public List<Item> BaseLayoutItems { get; private set; }

        public override string PipelineName
        {
            get { return "getBaseLayoutItems"; }
        }

        public override string WatcherMessage
        {
            get { return string.Format("{0} pipeline[item={1}]", PipelineName, Item.Paths.Path); }
        }
    }
}