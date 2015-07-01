// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The base layouts standard values provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Sitecore.BaseLayouts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    using Sitecore;
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;

    /// <summary>
    /// The base layout standard values provider.
    /// </summary>
    public class BaseLayoutStandardValuesProvider : StandardValuesProvider
    {
        /// <summary>
        /// The inner provider.
        /// </summary>
        private readonly StandardValuesProvider innerProvider;

        #region Fields
        
        /// <summary>
        /// The names of databases that support base layouts
        /// </summary>
        private string[] databases;
        
        #endregion

        #region Constructors and Destructors
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseLayoutStandardValuesProvider"/> class.
        /// </summary>
        /// <param name="innerProvider">
        /// The inner provider.
        /// </param>
        /// <param name="databases">
        /// A pipe delimited list of database names that support base layouts.
        /// </param>
        public BaseLayoutStandardValuesProvider(
            StandardValuesProvider innerProvider, 
            string databases)
        {
            Assert.ArgumentNotNull(innerProvider, "innerProvider");

            this.innerProvider = innerProvider;
            this.databases = databases.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Get the standard value for the field.  If the field is the Layout field (__renderings), it attempts to use base layouts.
        /// Otherwise, it passes the call on to the inner provider, which is usually the built-in standard values provider.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <returns>
        /// The standard value
        /// </returns>
        public override string GetStandardValue(Field field)
        {
            try
            {
                if (field.ID == FieldIDs.LayoutField)
                {
                    var layoutValue = this.GetLayoutValue(field);
                    if (!string.IsNullOrEmpty(layoutValue))
                    {
                        return layoutValue;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error getting layout value.", ex, this);
            }

            return this.innerProvider.GetStandardValue(field);
        }

        /// <summary>
        /// Initialize the provider.
        /// </summary>
        /// <param name="name">
        /// The provider name.
        /// </param>
        /// <param name="config">
        /// The configuration properties.
        /// </param>
        public override void Initialize(string name, NameValueCollection config)
        {
            this.innerProvider.Initialize(name, config);
        }

        /// <summary>
        /// Determines whether the item is a standard values item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool IsStandardValuesHolder(Item item)
        {
            return this.innerProvider.IsStandardValuesHolder(item);
        }

        /// <summary>
        /// Gets the name of the provider.
        /// </summary>
        public override string Name
        {
            get
            {
                return this.innerProvider.Name;
            }
        }

        /// <summary>
        /// Gets the description of the provider.
        /// </summary>
        public override string Description
        {
            get
            {
                return this.innerProvider.Description;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the value of the Layout field using a delta against the base layout.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <returns>
        /// The value of the layout field.
        /// </returns>
        protected virtual string GetLayoutValue(Field field)
        {
            // Sanity check.  Make sure the context is appropriate for attempting to find a base layout.
            if (field.Item == null
                || !this.databases.Contains(field.Item.Database.Name, StringComparer.OrdinalIgnoreCase)
                || !field.Item.Paths.IsContentItem || !field.Item.IsDerived(BaseLayoutSettings.TemplateId))
            {
                return null;
            }

            // Prevent an infinite loop
            if (this.HasCircularBaseLayoutReference(field.Item))
            {
                Log.Warn(string.Format("Circular base layout reference detected on item {0}.  Aborting resolution of base layouts.", field.Item.ID), this);
                return null;
            }

            // Get the item selected in the Base Layout field.  Otherwise, exit.
            ReferenceField baseLayoutField = field.Item.Fields[BaseLayoutSettings.FieldId];
            var baseLayoutItem = baseLayoutField.TargetItem;
            if (baseLayoutItem == null)
            {
                return null;
            }

            // Get the value of the layout field on the base layout.
            // If the selected item also has a base layout selected, this will cause implicit recursion.
            var layoutField = baseLayoutItem.Fields[FieldIDs.LayoutField];
            return layoutField == null ? null : LayoutField.GetFieldValue(layoutField);
        }

        /// <summary>
        /// Determines if there is a circular reference in the chain of base layouts.
        /// </summary>
        /// <param name="startItem">
        /// The start item.
        /// </param>
        /// <returns>
        /// True if there is a circular reference.  False if there is not.
        /// </returns>
        protected virtual bool HasCircularBaseLayoutReference(Item startItem)
        {
            Assert.ArgumentNotNull(startItem, "startItem");

            var item = startItem;
            var chain = new List<ID>();
            do
            {
                if (chain.Contains(item.ID))
                {
                    return true;
                }

                chain.Add(item.ID);
                ReferenceField baseLayoutField = item.Fields[BaseLayoutSettings.FieldId];
                item = baseLayoutField == null ? null : baseLayoutField.TargetItem;
            }
            while (item != null);

            return false;
        }

        #endregion
    }
}