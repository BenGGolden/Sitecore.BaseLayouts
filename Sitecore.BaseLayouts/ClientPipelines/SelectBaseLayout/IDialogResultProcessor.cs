using Sitecore.Data.Items;

namespace Sitecore.BaseLayouts.ClientPipelines.SelectBaseLayout
{
    public interface IDialogResultProcessor
    {
        bool ProcessResult(Item item, string result, out string errorMessage);
    }
}