using System;
using System.Linq;
using Sitecore.BaseLayouts.Diagnostics;
using Sitecore.Data.Fields;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;

namespace Sitecore.BaseLayouts
{
    /// <summary>
    /// Gets the value of the layout field from the base layout if one is selected.
    /// </summary>
    public class BaseLayoutValueProvider : ILayoutValueProvider
    {
        private readonly string[] _databases;
        private readonly IBaseLayoutValidator _baseLayoutValidator;
        private readonly ILog _log;

        /// <summary>
        /// Initializes the BaseLayoutProvider
        /// </summary>
        /// <param name="baseLayoutValidator">An IBaseLayoutValidator</param>
        /// <param name="log">a log service</param>
        public BaseLayoutValueProvider(IBaseLayoutValidator baseLayoutValidator, ILog log) : this(BaseLayoutSettings.SupportedDatabases, baseLayoutValidator, log)
        {
        }

        /// <summary>
        /// Initializes the BaseLayoutProvider
        /// </summary>
        /// <param name="databases">pipe separated list of database names to support</param>
        /// <param name="baseLayoutValidator">An IBaseLayoutValidator</param>
        /// <param name="log">a log service</param>
        public BaseLayoutValueProvider(string[] databases, IBaseLayoutValidator baseLayoutValidator, ILog log)
        {
            Assert.ArgumentNotNull(baseLayoutValidator, "baseLayoutValidator");
            Assert.ArgumentNotNull(log, "log");

            _databases = databases;
            _baseLayoutValidator = baseLayoutValidator;
            _log = log;
        }

        /// <summary>
        /// Gets the value of the layout field from the base layout if one is selected.
        /// </summary>
        /// <param name="field">The layout field.</param>
        /// <returns>The merged layout field value of the base layout chain.</returns>
        public virtual string GetLayoutValue(Field field)
        {
            // Sanity check.  Make sure the context is appropriate for attempting to find a base layout.
            if (!IsLayoutField(field)
                || !_databases.Contains(field.Item.Database.Name, StringComparer.OrdinalIgnoreCase)
                || !field.Item.Paths.IsContentItem || !TemplateManager.IsFieldPartOfTemplate(BaseLayoutSettings.FieldId, field.Item))
            {
                return null;
            }

            // Get the item selected in the Base Layout field.  Otherwise, exit.
            var baseLayoutItem = new BaseLayoutItem(field.Item).BaseLayout;
            if (baseLayoutItem == null)
            {
                return null;
            }

            // Prevent an infinite loop
            if (_baseLayoutValidator.HasCircularBaseLayoutReference(field.Item))
            {
                _log.Warn(
                    string.Format(
                        "Circular base layout reference detected on item {0}.  Aborting resolution of base layouts.",
                        field.Item.ID));
                return null;
            }

            // Get the value of the layout field on the base layout.
            // If the selected item also has a base layout selected, this will cause implicit recursion.
            return new LayoutField(baseLayoutItem.InnerItem).Value;
        }

        /// <summary>
        /// Determines whether the field is a layout field
        /// </summary>
        /// <param name="field">the field in question</param>
        /// <returns>True if it is a layout field, otherwise false.</returns>
        protected virtual bool IsLayoutField(Field field)
        {
            return field.ID == FieldIDs.LayoutField;
        }
    }
}