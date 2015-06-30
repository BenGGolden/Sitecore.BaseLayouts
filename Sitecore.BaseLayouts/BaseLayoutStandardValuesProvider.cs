// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The layout preset standard values provider.
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
    /// The layout preset standard values provider.
    /// </summary>
    public class BaseLayoutStandardValuesProvider : StandardValuesProvider
    {
        private static readonly ID LayoutPresetTemplateId = ID.Parse("{8CA74595-41A2-4077-9911-D386687E77BD}");

        private static readonly ID LayoutPresetFieldId = ID.Parse("{FBC10515-95D6-4559-BAD4-C235148DDECE}");

        /// <summary>
        /// The inner provider.
        /// </summary>
        private readonly StandardValuesProvider innerProvider;

        #region Fields
        
        /// <summary>
        /// The names of databases that support layout presets
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
        /// A pipe delimited list of database names that support layout presets.
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
        /// Get the standard value for the field.  If the field is the Layout field (__renderings), it attempts to use layout presets.
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
                    var layoutValue = this.GetLayoutValueWithPreset(field);
                    if (!string.IsNullOrEmpty(layoutValue))
                    {
                        return layoutValue;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error getting layout values with preset.", ex, this);
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
        /// Get the value of the Layout field using a delta against the layout preset.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <returns>
        /// The value of the layout field.
        /// </returns>
        protected virtual string GetLayoutValueWithPreset(Field field)
        {
            // Sanity check.  Make sure the context is appropriate for attempting to resolve the layout value with presets.
            if (field.Item == null
                || !this.databases.Contains(field.Item.Database.Name, StringComparer.OrdinalIgnoreCase)
                || !field.Item.Paths.IsContentItem || !field.Item.IsDerived(LayoutPresetTemplateId))
            {
                return null;
            }

            // Prevent an infinite loop
            if (this.HasCircularPresetReference(field.Item))
            {
                Log.Warn(string.Format("Circular layout preset reference detected on item {0}.  Aborting preset resolution.", field.Item.ID), this);
                return null;
            }

            // Get the item selected in the Layout Preset field.  Otherwise, exit.
            ReferenceField presetField = field.Item.Fields[LayoutPresetFieldId];
            var presetItem = presetField.TargetItem;
            if (presetItem == null)
            {
                return null;
            }

            // Get the value of the layout field on the preset.  If the selected item also has a preset selected, this will cause implicit recursion.
            var layoutField = presetItem.Fields[FieldIDs.LayoutField];
            return layoutField == null ? null : LayoutField.GetFieldValue(layoutField);
        }

        /// <summary>
        /// Determines if there is a circular reference in the chain of layout presets.
        /// </summary>
        /// <param name="startItem">
        /// The start item.
        /// </param>
        /// <returns>
        /// True if there is a circular reference.  False if there is not.
        /// </returns>
        protected virtual bool HasCircularPresetReference(Item startItem)
        {
            Assert.ArgumentNotNull(startItem, "startItem");

            var item = startItem;
            var presetChain = new List<ID>();
            do
            {
                if (presetChain.Contains(item.ID))
                {
                    return true;
                }

                presetChain.Add(item.ID);
                ReferenceField presetField = item.Fields[LayoutPresetFieldId];
                item = presetField == null ? null : presetField.TargetItem;
            }
            while (item != null);

            return false;
        }

        #endregion
    }
}