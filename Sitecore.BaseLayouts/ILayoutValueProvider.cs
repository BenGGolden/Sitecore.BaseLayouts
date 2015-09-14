namespace Sitecore.BaseLayouts
{
    using Sitecore.Data.Fields;

    public interface ILayoutValueProvider
    {
        string GetLayoutValue(Field field);
    }
}