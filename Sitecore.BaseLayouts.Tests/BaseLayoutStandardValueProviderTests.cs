using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.BaseLayouts.Abstractions;
using Sitecore.BaseLayouts.Data;

namespace Sitecore.BaseLayouts.Tests
{
    using System.Collections.Specialized;

    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.FakeDb;

    using Xunit;

    public class BaseLayoutStandardValueProviderTests : FakeDbTestClass
    {
        [Fact]
        public void GetStandardValue_WithLayoutFieldAndNonEmptyLayoutValue_ReturnsLayoutValue()
        {
            // Arrange
            var layoutProviderValue = "This is the layout value!";
            var layoutProvider = Substitute.For<IBaseLayoutValueProvider>();
            layoutProvider.GetBaseLayoutValue(Arg.Any<Field>()).Returns(layoutProviderValue);

            var innerProvider = Substitute.For<StandardValuesProvider>();
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutStandardValuesProvider(innerProvider, layoutProvider, log);
            var field = MasterFakesFactory.CreateFakeLayoutField();

            // Act
            var result = provider.GetStandardValue(field);

            // Assert
            Assert.Equal(layoutProviderValue, result);
        }

        [Fact]
        public void GetStandardValue_WithLayoutFieldAndEmptyLayoutValue_ReturnsValueFromInnerProvider()
        {
            // Arrange
            var layoutProvider = Substitute.For<IBaseLayoutValueProvider>();
            layoutProvider.GetBaseLayoutValue(Arg.Any<Field>()).Returns(string.Empty);

            var innerProviderValue = "Standard value from inner provider";
            var innerProvider = Substitute.For<StandardValuesProvider>();
            innerProvider.GetStandardValue(Arg.Any<Field>()).Returns(innerProviderValue);

            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutStandardValuesProvider(innerProvider, layoutProvider, log);
            var field = MasterFakesFactory.CreateFakeLayoutField();

            // Act
            var result = provider.GetStandardValue(field);

            // Assert
            Assert.Equal(innerProviderValue, result);
        }

        [Fact]
        public void GetStandardValue_WithNonLayoutField_DoesNotCallLayoutValueProvider()
        {
            // Arrange
            var layoutProvider = Substitute.For<IBaseLayoutValueProvider>();
            var innerProvider = Substitute.For<StandardValuesProvider>();
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutStandardValuesProvider(innerProvider, layoutProvider, log);
            var field = MasterFakesFactory.CreateFakeEmptyField();

            // Act
            var result = provider.GetStandardValue(field);

            // Assert
            layoutProvider.DidNotReceive().GetBaseLayoutValue(Arg.Any<Field>());
        }

        [Fact]
        public void GetStandardValue_WithNonLayoutField_ReturnsValueFromInnerProvider()
        {
            // Arrange
            var layoutProvider = Substitute.For<IBaseLayoutValueProvider>();

            var innerProviderValue = "Standard value from inner provider";
            var innerProvider = Substitute.For<StandardValuesProvider>();
            var log = Substitute.For<ILog>();
            innerProvider.GetStandardValue(Arg.Any<Field>()).Returns(innerProviderValue);

            var provider = new BaseLayoutStandardValuesProvider(innerProvider, layoutProvider, log);
            var field = MasterFakesFactory.CreateFakeEmptyField();

            // Act
            var result = provider.GetStandardValue(field);

            // Assert
            Assert.Equal(innerProviderValue, result);
        }

        [Fact]
        public void GetStandardValue_WhenLayoutProviderThrowsException_LogsErrorAndReturnsValueFromInnerProvider()
        {
            // Arrange
            var ex = new Exception("Something bad happened.");
            var layoutProvider = Substitute.For<IBaseLayoutValueProvider>();
            layoutProvider.GetBaseLayoutValue(Arg.Any<Field>()).Throws(ex);

            var innerProviderValue = "Standard value from inner provider";
            var innerProvider = Substitute.For<StandardValuesProvider>();
            innerProvider.GetStandardValue(Arg.Any<Field>()).Returns(innerProviderValue);

            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutStandardValuesProvider(innerProvider, layoutProvider, log);
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
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutStandardValuesProvider(innerProvider, layoutProvider, log);

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
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutStandardValuesProvider(innerProvider, layoutProvider, log);
            
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
            innerProvider.Description.Returns(description);
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutStandardValuesProvider(innerProvider, layoutProvider, log);

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
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutStandardValuesProvider(innerProvider, layoutProvider, log);

            var item = MasterFakesFactory.CreateFakeEmptyField().Item;
            
            // Act
            var result = provider.IsStandardValuesHolder(item);

            // Assert
            Assert.True(result);
        }
    }
}
