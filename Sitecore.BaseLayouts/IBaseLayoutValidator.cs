namespace Sitecore.BaseLayouts
{
    using Sitecore.Data.Items;

    /// <summary>
    /// Interface for a base layout validator
    /// </summary>
    public interface IBaseLayoutValidator
    {
        /// <summary>
        /// Determines if there is a circular reference in the base layout chain.
        /// </summary>
        /// <param name="item">the item</param>
        /// <returns>True if the item's base layout chain contains a circular reference.  Otherwise, false.</returns>
        bool HasCircularBaseLayoutReference(BaseLayoutItem item);

        /// <summary>
        /// Determines if a circular referece would be created if baseLayoutItem were set
        /// as the base layout for item
        /// </summary>
        /// <param name="item">the item</param>
        /// <param name="baseLayoutItem">the candidate base layout item</param>
        /// <returns>True if a circular reference would be created. Otherwise, false.</returns>
        bool CreatesCircularBaseLayoutReference(BaseLayoutItem item, Item baseLayoutItem);
    }
}