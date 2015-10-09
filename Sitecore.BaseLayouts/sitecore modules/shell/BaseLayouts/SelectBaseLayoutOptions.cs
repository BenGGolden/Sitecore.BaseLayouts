// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectBaseLayoutOptions.cs" company="">
//   
// </copyright>
// <summary>
//   The select base layout options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Sitecore.Data;
using Sitecore.Shell.Applications.Dialogs.ItemLister;
using Sitecore.Text;
using Sitecore.Web;

namespace Sitecore.BaseLayouts
{
    /// <summary>
    ///     The select base layout options.
    /// </summary>
    public class SelectBaseLayoutOptions : SelectAcceptableItemOptions
    {
        internal const string CurrentBaseLayoutQueryKey = "currentBaseLayoutId";

        /// <summary>
        ///     Initializes a new instance of the <see cref="SelectBaseLayoutOptions" /> class.
        /// </summary>
        public SelectBaseLayoutOptions()
        {
            Icon = "ApplicationsV2/32x32/window_gear.png";
            Title = "Select a Base Layout";
            Text = "Select the base layout that will determine the design of the page.";
            ButtonText = "Select";
        }

        /// <summary>
        ///     Gets or sets the current base layout id.
        /// </summary>
        public ID CurrentBaseLayoutId { get; set; }

        /// <summary>
        ///     The get xml control.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        protected override string GetXmlControl()
        {
            return "Sitecore.Shell.Applications.Dialogs.SelectBaseLayout";
        }

        /// <summary>
        ///     The to url string.
        /// </summary>
        /// <param name="database">
        ///     The database.
        /// </param>
        /// <returns>
        ///     The <see cref="UrlString" />.
        /// </returns>
        public override UrlString ToUrlString(Database database)
        {
            return AddCurrentBaseLayout(base.ToUrlString(database));
        }

        /// <summary>
        ///     The parse options.
        /// </summary>
        protected override void ParseOptions()
        {
            base.ParseOptions();
            ParseCurrentBaseLayout();
        }

        internal virtual UrlString AddCurrentBaseLayout(UrlString urlString)
        {
            if (!ID.IsNullOrEmpty(CurrentBaseLayoutId))
            {
                urlString[CurrentBaseLayoutQueryKey] = CurrentBaseLayoutId.ToShortID().ToString();
            }

            return urlString;
        }

        internal virtual void ParseCurrentBaseLayout()
        {
            ShortID id;
            var value = GetQueryString(CurrentBaseLayoutQueryKey);
            if (!string.IsNullOrEmpty(value) && ShortID.TryParse(value, out id))
            {
                CurrentBaseLayoutId = id.ToID();
            }
        }

        internal virtual string GetQueryString(string key)
        {
            return WebUtil.GetQueryString(CurrentBaseLayoutQueryKey);
        }
    }
}