using System;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using Sitecore.Shell.Applications.WebEdit.Commands;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web;

namespace Sitecore.BaseLayouts.Abstractions
{
    /// <summary>
    ///     Determines what page modes are supported
    /// </summary>
    public class PageModeAccess : IPageModeAccess
    {
        /// <summary>
        ///     Determines if Web Edit mode is available
        /// </summary>
        /// <returns></returns>
        public bool CanWebEdit()
        {
            return CommandHack.CanWebEdit();
        }

        /// <summary>
        ///     Determines if Design mode is available
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CanDesignItem(Item item)
        {
            return Policy.IsAllowed("Page Editor/Can Design") && WebEditUtil.CanDesignItem(item);
        }

        private class CommandHack : WebEditCommand
        {
            public new static bool CanWebEdit()
            {
                return WebEditCommand.CanWebEdit();
            }

            public override void Execute(CommandContext context)
            {
                throw new NotImplementedException();
            }
        }
    }
}