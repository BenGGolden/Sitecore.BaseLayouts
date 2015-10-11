using System;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;

namespace Sitecore.BaseLayouts.Pipelines.SaveBaseLayout
{
    public class SaveNewBaseLayout : ISaveBaseLayoutProcessor
    {
        public void Process(SaveBaseLayoutArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.IsNotNull(args.Item, "The item cannot be null.");

            var newValue = args.NewBaseLayoutItem == null ? string.Empty : args.NewBaseLayoutItem.ID.ToString();
            var field = args.Item.InnerItem.Fields[BaseLayoutSettings.FieldId];
            if (!field.Value.Equals(newValue, StringComparison.OrdinalIgnoreCase))
            {
                using (new EditContext(args.Item.InnerItem))
                {
                    field.Value = newValue;
                }
            }

            args.Successful = true;
        }
    }
}