namespace Sitecore.BaseLayouts
{
    using Sitecore.Data.Fields;

    public interface IBaseLayoutValueProvider
    {
        string GetBaseLayoutValue(Field field);
    }
}