using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Web.UI.HtmlControls.Data;

namespace Sitecore.BaseLayouts.Pipelines
{
    /// <summary>
    /// Processor to get base layout items from LookupSources
    /// </summary>
    public class GetLookupSourceItems : IGetBaseLayoutItemsProcessor
    {
        /// <summary>
        /// Get base layout items from the soure of the base layout field
        /// </summary>
        /// <param name="args">The getBaseLayoutItems pipeline args</param>
        public void Process(GetBaseLayoutItemsArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.IsNotNull(args.Item, "Item cannot be null.");

            if (!TemplateManager.IsFieldPartOfTemplate(BaseLayoutSettings.FieldId, args.Item)) return;

            var field = args.Item.Fields[BaseLayoutSettings.FieldId];
            var items = LookupSources.GetItems(args.Item, field.Source);
            if (items != null)
            {
                args.BaseLayoutItems.AddRange(items);
            }
        }
    }
}