using Sitecore.Data.Items;

namespace Sitecore.BaseLayouts.Abstractions
{
    /// <summary>
    ///     Determines what page modes are supported
    /// </summary>
    public interface IPageModeAccess
    {
        /// <summary>
        ///     Determines if Web Edit mode is available
        /// </summary>
        /// <returns></returns>
        bool CanWebEdit();

        /// <summary>
        ///     Determines if Design mode is available
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool CanDesignItem(Item item);
    }
}