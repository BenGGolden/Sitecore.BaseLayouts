using Sitecore.Data.Items;

namespace Sitecore.BaseLayouts.Commands
{
    public interface ICommandContextChecker
    {
        /// <summary>
        /// Determines whether the command is supported for the given item
        /// </summary>
        /// <param name="item">the context item</param>
        /// <returns>a value indicating if the command is supported</returns>
        bool CanExecute(Item item);
    }
}