using Sitecore.Data.Items;

namespace Sitecore.BaseLayouts.Data
{
    public interface IBaseLayoutValueProvider
    {
        string GetBaseLayoutValue(Item item);
    }
}