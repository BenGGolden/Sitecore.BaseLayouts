using System.Collections.Generic;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;

namespace Sitecore.BaseLayouts.Pipelines
{
    /// <summary>
    /// Pipeline args for the getBaseLayoutItems pipeline
    /// </summary>
    public class GetBaseLayoutItemsArgs : PipelineArgs
    {
        /// <summary>
        /// Initializes a GetBaseLayoutItemArgs
        /// </summary>
        /// <param name="item"></param>
        public GetBaseLayoutItemsArgs(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            Item = item;
            BaseLayoutItems = new List<Item>();
        }

        /// <summary>
        /// The Item to get base layouts for.
        /// </summary>
        public Item Item { get; set; }


        /// <summary>
        /// The resulting list of base layout items
        /// </summary>
        public List<Item> BaseLayoutItems { get; private set; }
    }
}