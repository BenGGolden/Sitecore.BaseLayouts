using System;
using System.Collections.Specialized;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Sitecore.BaseLayouts.Abstractions;
using Sitecore.BaseLayouts.Data;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Xunit;

namespace Sitecore.BaseLayouts.Tests.Data
{
    public class BaseLayoutStandardValueProviderTests : FakeDbTestClass
    {
        [Fact]
        public void GetStandardValue_WithNonLayoutField_DoesNotCallLayoutValueProvider()
        {
            // Arrange
            var innerProvider = Substitute.For<StandardValuesProvider>();
            var settings = TestUtil.CreateFakeSettings();
            var layoutProvider = Substitute.For<IBaseLayoutValueProvider>();
            var validator = Substitute.For<IBaseLayoutValidator>();
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutStandardValuesProvider(innerProvider, settings, layoutProvider, validator, log);
            var field = MasterFakesFactory.CreateFakeEmptyField();

            // Act
            var result = provider.GetStandardValue(field);

            // Assert
            layoutProvider.DidNotReceive().GetBaseLayoutValue(Arg.Any<Item>());
        }

        [Fact]
        public void GetStandardValue_WithNonLayoutField_ReturnsValueFromInnerProvider()
        {
            // Arrange
            var layoutProvider = Substitute.For<IBaseLayoutValueProvider>();

            var innerProviderValue = "Standard value from inner provider";
            var innerProvider = Substitute.For<StandardValuesProvider>();
            var settings = TestUtil.CreateFakeSettings();
            var validator = Substitute.For<IBaseLayoutValidator>();
            var log = Substitute.For<ILog>();
            innerProvider.GetStandardValue(Arg.Any<Field>()).Returns(innerProviderValue);

            var provider = new BaseLayoutStandardValuesProvider(innerProvider, settings, layoutProvider, validator, log);
            var field = MasterFakesFactory.CreateFakeEmptyField();

            // Act
            var result = provider.GetStandardValue(field);

            // Assert
            Assert.Equal(innerProviderValue, result);
        }

        [Fact]
        public void GetStandardValue_WithLayoutFieldFromUnsupportedItem_DoesNotCallLayoutValueProvider()
        {
            // Arrange
            var layoutProvider = Substitute.For<IBaseLayoutValueProvider>();
            var innerProvider = Substitute.For<StandardValuesProvider>();
            var settings = TestUtil.CreateFakeSettings();
            var validator = Substitute.For<IBaseLayoutValidator>();
            validator.ItemSupportsBaseLayouts(Arg.Any<Item>()).Returns(false);
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutStandardValuesProvider(innerProvider, settings, layoutProvider, validator, log);
            var field = MasterFakesFactory.CreateFakeLayoutField();

            // Act
            var result = provider.GetStandardValue(field);

            // Assert
            layoutProvider.DidNotReceive().GetBaseLayoutValue(Arg.Any<Item>());
        }

#if FINAL_LAYOUT
        [Fact]
        public void GetStandardValue_WithFinalLayoutFieldFromUnsupportedItem_DoesNotCallLayoutValueProvider()
        {
            // Arrange
            var layoutProvider = Substitute.For<IBaseLayoutValueProvider>();
            var innerProvider = Substitute.For<StandardValuesProvider>();
            var settings = TestUtil.CreateFakeSettings();
            var validator = Substitute.For<IBaseLayoutValidator>();
            validator.ItemSupportsBaseLayouts(Arg.Any<Item>()).Returns(false);
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutStandardValuesProvider(innerProvider, settings, layoutProvider, validator, log);
            var field = MasterFakesFactory.CreateFakeFinalLayoutField();

            // Act
            var result = provider.GetStandardValue(field);

            // Assert
            layoutProvider.DidNotReceive().GetBaseLayoutValue(Arg.Any<Item>());
        }
#endif

        [Fact]
        public void GetStandardValue_WithAlwaysCheckForCircularReferenceTrueAndLayoutFieldFromItemWithCircularReference_DoesNotCallLayoutValueProvider()
        {
            // Arrange
            var layoutProvider = Substitute.For<IBaseLayoutValueProvider>();
            var innerProvider = Substitute.For<StandardValuesProvider>();
            var settings = TestUtil.CreateFakeSettings(null, true);
            var validator = Substitute.For<IBaseLayoutValidator>();
            validator.HasCircularBaseLayoutReference(Arg.Any<Item>()).Returns(true);
            validator.ItemSupportsBaseLayouts(Arg.Any<Item>()).Returns(true);
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutStandardValuesProvider(innerProvider, settings, layoutProvider, validator, log);
            var field = MasterFakesFactory.CreateFakeLayoutField();

            // Act
            var result = provider.GetStandardValue(field);

            // Assert
            layoutProvider.DidNotReceive().GetBaseLayoutValue(Arg.Any<Item>());
        }

#if FINAL_LAYOUT
        [Fact]
        public void GetStandardValue_WithAlwaysCheckForCircularReferenceTrueAndFinalLayoutFieldFromItemWithCircularReference_DoesNotCallLayoutValueProvider()
        {
            // Arrange
            var layoutProvider = Substitute.For<IBaseLayoutValueProvider>();
            var innerProvider = Substitute.For<StandardValuesProvider>();
            var settings = TestUtil.CreateFakeSettings(null, true);
            var validator = Substitute.For<IBaseLayoutValidator>();
            validator.HasCircularBaseLayoutReference(Arg.Any<Item>()).Returns(true);
            validator.ItemSupportsBaseLayouts(Arg.Any<Item>()).Returns(true);
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutStandardValuesProvider(innerProvider, settings, layoutProvider, validator, log);
            var field = MasterFakesFactory.CreateFakeFinalLayoutField();

            // Act
            var result = provider.GetStandardValue(field);

            // Assert
            layoutProvider.DidNotReceive().GetBaseLayoutValue(Arg.Any<Item>());
        }
#endif

        [Fact]
        public void GetStandardValue_WithAlwaysCheckForCircularReferenceTrueAndLayoutFieldFromItemWithCircularReference_CallsLogWarn()
        {
            // Arrange
            var layoutProvider = Substitute.For<IBaseLayoutValueProvider>();
            var innerProvider = Substitute.For<StandardValuesProvider>();
            var settings = TestUtil.CreateFakeSettings(null, true);
            var validator = Substitute.For<IBaseLayoutValidator>();
            validator.HasCircularBaseLayoutReference(Arg.Any<Item>()).Returns(true);
            validator.ItemSupportsBaseLayouts(Arg.Any<Item>()).Returns(true);
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutStandardValuesProvider(innerProvider, settings, layoutProvider, validator, log);
            var field = MasterFakesFactory.CreateFakeLayoutField();

            // Act
            var result = provider.GetStandardValue(field);

            // Assert
            log.ReceivedWithAnyArgs().Warn(Arg.Any<string>());
        }

#if FINAL_LAYOUT
        [Fact]
        public void GetStandardValue_WithAlwaysCheckForCircularReferenceTrueAndFinalLayoutFieldFromItemWithCircularReference_CallsLogWarn()
        {
            // Arrange
            var layoutProvider = Substitute.For<IBaseLayoutValueProvider>();
            var innerProvider = Substitute.For<StandardValuesProvider>();
            var settings = TestUtil.CreateFakeSettings(null, true);
            var validator = Substitute.For<IBaseLayoutValidator>();
            validator.HasCircularBaseLayoutReference(Arg.Any<Item>()).Returns(true);
            validator.ItemSupportsBaseLayouts(Arg.Any<Item>()).Returns(true);
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutStandardValuesProvider(innerProvider, settings, layoutProvider, validator, log);
            var field = MasterFakesFactory.CreateFakeFinalLayoutField();

            // Act
            var result = provider.GetStandardValue(field);

            // Assert
            log.ReceivedWithAnyArgs().Warn(Arg.Any<string>());
        }
#endif

        [Fact]
        public void GetStandardValue_WithLayoutFieldAndNonEmptyLayoutValue_ReturnsLayoutValue()
        {
            // Arrange
            var layoutProviderValue = "This is the layout value!";
            var layoutProvider = Substitute.For<IBaseLayoutValueProvider>();
            layoutProvider.GetBaseLayoutValue(Arg.Any<Item>()).Returns(layoutProviderValue);

            var validator = Substitute.For<IBaseLayoutValidator>();
            validator.ItemSupportsBaseLayouts(Arg.Any<Item>()).Returns(true);
            validator.HasCircularBaseLayoutReference(Arg.Any<Item>()).Returns(false);

            var innerProvider = Substitute.For<StandardValuesProvider>();
            var settings = TestUtil.CreateFakeSettings();
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutStandardValuesProvider(innerProvider, settings, layoutProvider, validator, log);
            var field = MasterFakesFactory.CreateFakeLayoutField();

            // Act
            var result = provider.GetStandardValue(field);

            // Assert
            Assert.Equal(layoutProviderValue, result);
        }

#if FINAL_LAYOUT
        [Fact]
        public void GetStandardValue_WithFinalLayoutFieldAndNonEmptyLayoutValue_ReturnsLayoutValue()
        {
            // Arrange
            var layoutProviderValue = "This is the layout value!";
            var layoutProvider = Substitute.For<IBaseLayoutValueProvider>();
            layoutProvider.GetBaseLayoutValue(Arg.Any<Item>()).Returns(layoutProviderValue);

            var validator = Substitute.For<IBaseLayoutValidator>();
            validator.ItemSupportsBaseLayouts(Arg.Any<Item>()).Returns(true);
            validator.HasCircularBaseLayoutReference(Arg.Any<Item>()).Returns(false);

            var innerProvider = Substitute.For<StandardValuesProvider>();
            var settings = TestUtil.CreateFakeSettings();
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutStandardValuesProvider(innerProvider, settings, layoutProvider, validator, log);
            var field = MasterFakesFactory.CreateFakeFinalLayoutField();

            // Act
            var result = provider.GetStandardValue(field);

            // Assert
            Assert.Equal(layoutProviderValue, result);
        }
#endif

        [Fact]
        public void GetStandardValue_WithLayoutFieldAndEmptyLayoutValue_ReturnsValueFromInnerProvider()
        {
            // Arrange
            var layoutProvider = Substitute.For<IBaseLayoutValueProvider>();
            layoutProvider.GetBaseLayoutValue(Arg.Any<Item>()).Returns(string.Empty);

            var innerProviderValue = "Standard value from inner provider";
            var innerProvider = Substitute.For<StandardValuesProvider>();
            innerProvider.GetStandardValue(Arg.Any<Field>()).Returns(innerProviderValue);

            var settings = TestUtil.CreateFakeSettings();
            var validator = Substitute.For<IBaseLayoutValidator>();
            validator.ItemSupportsBaseLayouts(Arg.Any<Item>()).Returns(true);
            validator.HasCircularBaseLayoutReference(Arg.Any<Item>()).Returns(false);
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutStandardValuesProvider(innerProvider, settings, layoutProvider, validator, log);
            var field = MasterFakesFactory.CreateFakeLayoutField();

            // Act
            var result = provider.GetStandardValue(field);

            // Assert
            Assert.Equal(innerProviderValue, result);
        }

#if FINAL_LAYOUT
        [Fact]
        public void GetStandardValue_WithFinalLayoutFieldAndEmptyLayoutValue_ReturnsValueFromInnerProvider()
        {
            // Arrange
            var layoutProvider = Substitute.For<IBaseLayoutValueProvider>();
            layoutProvider.GetBaseLayoutValue(Arg.Any<Item>()).Returns(string.Empty);

            var innerProviderValue = "Standard value from inner provider";
            var innerProvider = Substitute.For<StandardValuesProvider>();
            innerProvider.GetStandardValue(Arg.Any<Field>()).Returns(innerProviderValue);

            var settings = TestUtil.CreateFakeSettings();
            var validator = Substitute.For<IBaseLayoutValidator>();
            validator.ItemSupportsBaseLayouts(Arg.Any<Item>()).Returns(true);
            validator.HasCircularBaseLayoutReference(Arg.Any<Item>()).Returns(false);
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutStandardValuesProvider(innerProvider, settings, layoutProvider, validator, log);
            var field = MasterFakesFactory.CreateFakeFinalLayoutField();

            // Act
            var result = provider.GetStandardValue(field);

            // Assert
            Assert.Equal(innerProviderValue, result);
        }
#endif

        [Fact]
        public void GetStandardValue_WhenLayoutProviderThrowsException_LogsErrorAndReturnsValueFromInnerProvider()
        {
            // Arrange
            var ex = new Exception("Something bad happened.");
            var layoutProvider = Substitute.For<IBaseLayoutValueProvider>();
            layoutProvider.GetBaseLayoutValue(Arg.Any<Item>()).Throws(ex);

            var innerProviderValue = "Standard value from inner provider";
            var innerProvider = Substitute.For<StandardValuesProvider>();
            innerProvider.GetStandardValue(Arg.Any<Field>()).Returns(innerProviderValue);

            var validator = Substitute.For<IBaseLayoutValidator>();
            validator.ItemSupportsBaseLayouts(Arg.Any<Item>()).Returns(true);
            validator.HasCircularBaseLayoutReference(Arg.Any<Item>()).Returns(false);

            var settings = TestUtil.CreateFakeSettings();
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutStandardValuesProvider(innerProvider, settings, layoutProvider, validator, log);
            var field = MasterFakesFactory.CreateFakeLayoutField();

            // Act
            var result = provider.GetStandardValue(field);

            // Assert
            log.Received().Error(ex, Arg.Any<string>());
            Assert.Equal(innerProviderValue, result);
        }

        [Fact]
        public void Initialize_Always_CallsInnerProviderInitialize()
        {
            // Arrange
            var layoutProvider = Substitute.For<IBaseLayoutValueProvider>();
            var innerProvider = Substitute.For<StandardValuesProvider>();
            var settings = TestUtil.CreateFakeSettings();
            var validator = Substitute.For<IBaseLayoutValidator>();
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutStandardValuesProvider(innerProvider, settings, layoutProvider, validator, log);

            var name = "baseLayouts";
            var config = new NameValueCollection();

            // Act
            provider.Initialize(name, config);

            // Assert
            innerProvider.Received().Initialize(name, config);
        }

        [Fact]
        public void Name_Always_ReturnsInnerProviderName()
        {
            // Arrange
            var name = "baseLayouts";
            var layoutProvider = Substitute.For<IBaseLayoutValueProvider>();
            var innerProvider = Substitute.For<StandardValuesProvider>();
            innerProvider.Name.Returns(name);
            var settings = TestUtil.CreateFakeSettings();
            var validator = Substitute.For<IBaseLayoutValidator>();
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutStandardValuesProvider(innerProvider, settings, layoutProvider, validator, log);

            // Act
            var result = provider.Name;

            // Assert
            Assert.Equal(name, result);
        }

        [Fact]
        public void Description_Always_ReturnsInnerProviderDescription()
        {
            // Arrange
            var description = "I don't think this is even used";
            var layoutProvider = Substitute.For<IBaseLayoutValueProvider>();
            var innerProvider = Substitute.For<StandardValuesProvider>();
            var settings = TestUtil.CreateFakeSettings();
            innerProvider.Description.Returns(description);
            var validator = Substitute.For<IBaseLayoutValidator>();
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutStandardValuesProvider(innerProvider, settings, layoutProvider, validator, log);

            // Act
            var result = provider.Description;

            // Assert
            Assert.Equal(description, result);
        }

        [Fact]
        public void IsStandardValuesHolder_Always_ReturnsInnerProviderIsStandardValuesHolder()
        {
            // Arrange
            var layoutProvider = Substitute.For<IBaseLayoutValueProvider>();
            var innerProvider = Substitute.For<StandardValuesProvider>();
            innerProvider.IsStandardValuesHolder(Arg.Any<Item>()).Returns(true);
            var validator = Substitute.For<IBaseLayoutValidator>();
            var settings = TestUtil.CreateFakeSettings();
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutStandardValuesProvider(innerProvider, settings, layoutProvider, validator, log);

            var item = MasterFakesFactory.CreateFakeEmptyField().Item;

            // Act
            var result = provider.IsStandardValuesHolder(item);

            // Assert
            Assert.True(result);
        }
    }
}