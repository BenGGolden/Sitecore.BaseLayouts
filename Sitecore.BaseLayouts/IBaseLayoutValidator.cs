namespace Sitecore.BaseLayouts
{
    using Sitecore.Data.Items;

    public interface IBaseLayoutValidator
    {
        bool HasCircularBaseLayoutReference(Item item);
    }
}