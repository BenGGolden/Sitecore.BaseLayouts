﻿using System;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Sitecore.BaseLayouts.Abstractions;
using Sitecore.BaseLayouts.Caching;
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
        private readonly IBaseLayoutValidator _baseLayoutValidator;
        private readonly IBaseLayoutValueProvider _baseLayoutValueProvider;
        private readonly StandardValuesProvider _innerProvider;
        private readonly IBaseLayoutSettings _settings;
        private readonly ILog _log;

        [ExcludeFromCodeCoverage]
        public BaseLayoutStandardValuesProvider() : this(new StandardValuesProvider())
        {
        }

        [ExcludeFromCodeCoverage]
        public BaseLayoutStandardValuesProvider(StandardValuesProvider innerProvider) : this(innerProvider, new BaseLayoutSettings())
        {
        }

        [ExcludeFromCodeCoverage]
        public BaseLayoutStandardValuesProvider(StandardValuesProvider innerProvider, IBaseLayoutSettings settings)
            : this(innerProvider, settings,
                new CachedBaseLayoutValueProvider(new BaseLayoutValueProvider(), new BaseLayoutValueCache(settings)),
                new BaseLayoutValidator(settings), new LogWrapper())
        {
        }

        public BaseLayoutStandardValuesProvider(
            StandardValuesProvider innerProvider,
            IBaseLayoutSettings settings,
            IBaseLayoutValueProvider baseLayoutValueProvider,
            IBaseLayoutValidator baseLayoutValidator,
            ILog log)
        {
            Assert.ArgumentNotNull(innerProvider, "innerProvider");
            Assert.ArgumentNotNull(settings, "settings");
            Assert.ArgumentNotNull(baseLayoutValueProvider, "layoutValueProvider");
            Assert.ArgumentNotNull(baseLayoutValidator, "baseLayoutValidator");
            Assert.ArgumentNotNull(log, "log");

            _innerProvider = innerProvider;
            _settings = settings;
            _baseLayoutValueProvider = baseLayoutValueProvider;
            _baseLayoutValidator = baseLayoutValidator;
            _log = log;
        }

        public override string Name
        {
            get { return _innerProvider.Name; }
        }

        public override string Description
        {
            get { return _innerProvider.Description; }
        }

        /// <summary>
        ///     Get the standard value for the field.  If the field is the Layout field (__renderings), it attempts to use base
        ///     layouts. Otherwise, it passes the call on to the inner provider, which is usually the built-in standard values
        ///     provider.
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
                if (field.IsLayoutField() && _baseLayoutValidator.ItemSupportsBaseLayouts(field.Item))
                {
                    if (_settings.AlwaysCheckForCircularReference &&
                        _baseLayoutValidator.HasCircularBaseLayoutReference(field.Item))
                    {
                        _log.Warn(
                            "Circular Base Layout reference detected on item {0}. Aborting base layout composition.",
                            field.Item.Paths.Path);
                    }
                    else
                    {
                        var layoutValue = _baseLayoutValueProvider.GetBaseLayoutValue(field.Item);
                        if (!string.IsNullOrEmpty(layoutValue))
                        {
                            return layoutValue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error getting layout value.");
            }

            return _innerProvider.GetStandardValue(field);
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            _innerProvider.Initialize(name, config);
        }

        public override bool IsStandardValuesHolder(Item item)
        {
            return _innerProvider.IsStandardValuesHolder(item);
        }
    }
}