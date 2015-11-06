// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The base layouts standard values provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Specialized;
using Sitecore.BaseLayouts.Abstractions;
using Sitecore.BaseLayouts.Extensions;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Sitecore.BaseLayouts.Data
{
    /// <summary>
    ///     The base layout standard values provider.
    /// </summary>
    public class BaseLayoutStandardValuesProvider : StandardValuesProvider
    {
        /// <summary>
        ///     The inner provider.
        /// </summary>
        private readonly StandardValuesProvider _innerProvider;

        private readonly IBaseLayoutValueProvider _baseLayoutValueProvider;

        private readonly ILog _log;

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseLayoutStandardValuesProvider" /> class.
        /// </summary>
        /// <param name="innerProvider">
        ///     The inner provider.
        /// </param>
        /// <param name="baseLayoutValueProvider">
        ///     The layout value provider
        /// </param>
        /// <param name="log"></param>
        public BaseLayoutStandardValuesProvider(
            StandardValuesProvider innerProvider,
            IBaseLayoutValueProvider baseLayoutValueProvider,
            ILog log)
        {
            Assert.ArgumentNotNull(innerProvider, "innerProvider");
            Assert.ArgumentNotNull(baseLayoutValueProvider, "layoutValueProvider");
            Assert.ArgumentNotNull(log, "log");

            _innerProvider = innerProvider;
            _baseLayoutValueProvider = baseLayoutValueProvider;
            _log = log;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Get the standard value for the field.  If the field is the Layout field (__renderings), it attempts to use base
        ///     layouts.
        ///     Otherwise, it passes the call on to the inner provider, which is usually the built-in standard values provider.
        /// </summary>
        /// <param name="field">
        ///     The field.
        /// </param>
        /// <returns>
        ///     The standard value
        /// </returns>
        public override string GetStandardValue(Field field)
        {
            try
            {
                if (field.IsLayoutField())
                {
                    var layoutValue = _baseLayoutValueProvider.GetBaseLayoutValue(field);
                    if (!string.IsNullOrEmpty(layoutValue))
                    {
                        return layoutValue;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error getting layout value.");
            }

            return _innerProvider.GetStandardValue(field);
        }

        /// <summary>
        ///     Initialize the provider.
        /// </summary>
        /// <param name="name">
        ///     The provider name.
        /// </param>
        /// <param name="config">
        ///     The configuration properties.
        /// </param>
        public override void Initialize(string name, NameValueCollection config)
        {
            _innerProvider.Initialize(name, config);
        }

        /// <summary>
        ///     Determines whether the item is a standard values item.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public override bool IsStandardValuesHolder(Item item)
        {
            return _innerProvider.IsStandardValuesHolder(item);
        }

        /// <summary>
        ///     Gets the name of the provider.
        /// </summary>
        public override string Name
        {
            get { return _innerProvider.Name; }
        }

        /// <summary>
        ///     Gets the description of the provider.
        /// </summary>
        public override string Description
        {
            get { return _innerProvider.Description; }
        }

        #endregion
    }
}