using Sitecore.Data.Items;

namespace Sitecore.BaseLayouts.Data
{
    /// <summary>
    /// Interface for a base layout validator
    /// </summary>
    public interface IBaseLayoutValidator
    {
        /// <summary>
        /// Determines if the field is a layout field on an item that supports base layouts.
        /// </summary>
        /// <param name="item">The item</param>
        /// <returns>A bool indicating whether the field is a layout field on an item that supports base layouts.</returns>
        bool ItemSupportsBaseLayouts(Item item);

        /// <summary>
        /// Determines if there is a circular reference in the base layout chain.
        /// </summary>
        /// <param name="item">the item</param>
        /// <returns>True if the item's base layout chain contains a circular reference.  Otherwise, false.</returns>
        bool HasCircularBaseLayoutReference(Item item);

        /// <summary>
        /// Determines if a circular referece would be created if baseLayoutItem were set
        /// as the base layout for item
        /// </summary>
        /// <param name="item">the item</param>
        /// <param name="baseLayoutItem">the candidate base layout item</param>
        /// <returns>True if a circular reference would be created. Otherwise, false.</returns>
        bool CreatesCircularBaseLayoutReference(Item item, Item baseLayoutItem);

#if FINAL_LAYOUT
        /// <summary>
        /// Determines if there is a versioning conflict created if baseLayoutItem were set
        /// as the base layout for item.  A versioning conflict occurs when a shared layout delta
        /// is applied over versioned layout value.  That is, if and item has a value for the renderings
        /// field and the item set as its base layout has a value for the final renderings field.
        /// </summary>
        /// <param name="item">the item</param>
        /// <param name="baseLayoutItem">the base layout item</param>
        /// <returns></returns>
        bool CreatesVersioningConflict(Item item, Item baseLayoutItem);
#endif
    }
}