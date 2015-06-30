// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectBaseLayoutOptions.cs" company="">
//   
// </copyright>
// <summary>
//   The select base layout options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Sitecore.BaseLayouts.Commands
{
    using Sitecore.Data;
    using Sitecore.Diagnostics;
    using Sitecore.Shell.Applications.Dialogs.ItemLister;
    using Sitecore.Text;
    using Sitecore.Web;

    /// <summary>
    ///     The select base layout options.
    /// </summary>
    public class SelectBaseLayoutOptions : SelectAcceptableItemOptions
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SelectBaseLayoutOptions" /> class.
        /// </summary>
        public SelectBaseLayoutOptions()
        {
            this.Icon = "ApplicationsV2/32x32/window_gear.png";
            this.Title = "Select a Base Layout";
            this.Text = "Select the base layout that will determine the design of the page.";
            this.ButtonText = "Select";
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
        /// The to url string.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <returns>
        /// The <see cref="UrlString"/>.
        /// </returns>
        public override UrlString ToUrlString(Database database)
        {
            Assert.ArgumentNotNull(database, "database");

            this.AddNullSelectionItem(database);
            var urlString = base.ToUrlString(database);
            if (!ID.IsNullOrEmpty(this.CurrentBaseLayoutId))
            {
                var handle = urlString["hdl"];
                WebUtil.SetSessionValue(this.GetSessionKey(handle), this.CurrentBaseLayoutId);
            }

            return urlString;
        }

        /// <summary>
        ///     The parse options.
        /// </summary>
        protected override void ParseOptions()
        {
            base.ParseOptions();
            var handle = WebUtil.GetQueryString("hdl");
            if (string.IsNullOrEmpty(handle))
            {
                return;
            }

            var key = this.GetSessionKey(handle);
            this.CurrentBaseLayoutId = WebUtil.GetSessionValue(key) as ID;
            WebUtil.RemoveSessionValue(key);
        }

        /// <summary>
        /// The get session key.
        /// </summary>
        /// <param name="handle">
        /// The handle.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetSessionKey(string handle)
        {
            return handle + "_CurrentBaseLayoutId";
        }

        private void AddNullSelectionItem(Database database)
        {
            var nullItem = database.GetItem(BaseLayoutSettings.NullSelectionItemId);
            if (nullItem != null)
            {
                this.Items.Insert(0, nullItem);

                if (ID.IsNullOrEmpty(this.CurrentBaseLayoutId))
                {
                    this.CurrentBaseLayoutId = BaseLayoutSettings.NullSelectionItemId;
                }
            }
        }
    }
}