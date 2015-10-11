using Sitecore.Data.Fields;

namespace Sitecore.BaseLayouts.Data
{
    public interface IBaseLayoutValueProvider
    {
        string GetBaseLayoutValue(Field field);
    }
}