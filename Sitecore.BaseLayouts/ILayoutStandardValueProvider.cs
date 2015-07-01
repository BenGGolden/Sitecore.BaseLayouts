// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The LayoutValueProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.BaseLayouts
{
    using Sitecore.Data.Fields;

    /// <summary>
    /// The LayoutValueProvider interface.
    /// </summary>
    public interface ILayoutStandardValueProvider
    {
        /// <summary>
        /// The get layout value.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string GetLayoutValue(Field field);
    }
}