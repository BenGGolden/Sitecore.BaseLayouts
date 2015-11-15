using System.Collections.Generic;
using Sitecore.BaseLayouts.Extensions;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Web.UI.HtmlControls.Data;

namespace Sitecore.BaseLayouts.Pipelines.GetBaseLayoutItems
{
    /// <summary>
    ///     Processor to get base layout items from LookupSources
    /// </summary>
    public class GetLookupSourceItems : IGetBaseLayoutItemsProcessor
    {
        /// <summary>
        ///     Get base layout items from the soure of the base layout field
        /// </summary>
        /// <param name="args">The getBaseLayoutItems pipeline args</param>
        public void Process(GetBaseLayoutItemsArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.IsNotNull(args.Item, "Item cannot be null.");

            if (!args.Item.HasField(BaseLayoutSettings.FieldId)) return;

            var field = args.Item.Fields[BaseLayoutSettings.FieldId];
            args.BaseLayoutItems.AddRange(GetItems(args.Item, field.Source));
        }

        internal virtual IEnumerable<Item> GetItems(Item item, string source)
        {
            return LookupSources.GetItems(item, source);
        }
    }
}