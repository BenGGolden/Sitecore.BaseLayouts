using System;
using NSubstitute;
using Sitecore.BaseLayouts.Abstractions;
using Sitecore.BaseLayouts.Data;
using Sitecore.Data.Items;
using Sitecore.FakeDb;

namespace Sitecore.BaseLayouts.Tests
{
    using Xunit;

    public class BaseLayoutValueProviderTests : FakeDbTestClass
    {
        [Fact]
        public void GetLayoutValue_WithNonLayoutField_ReturnsNull()
        {
            // Arrange
            var validator = Substitute.For<IBaseLayoutValidator>();
            var log = Substitute.For<ILog>();
            var field = MasterFakesFactory.CreateFakeEmptyField();
            var provider = new BaseLayoutValueProvider(new[] {"master", "web"}, validator, log);

            // Act
            var result = provider.GetBaseLayoutValue(field);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetLayoutValue_WithLayoutFieldFromUnsupportedDatabase_ReturnsNull()
        {
            // Arrange
            var validator = Substitute.For<IBaseLayoutValidator>();
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutValueProvider(new[] {"web"}, validator, log);
            var field = MasterFakesFactory.CreateFakeLayoutField();

            // Act
            var result = provider.GetBaseLayoutValue(field);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetLayoutValue_WithLayoutFieldFromNonContentItem_ReturnsNull()
        {
            // Arrange
            var validator = Substitute.For<IBaseLayoutValidator>();
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutValueProvider(new[] {"master", "web"}, validator, log);
            var field = MasterFakesFactory.CreateFakeLayoutField(null, ItemIDs.SystemRoot);

            // Act
            var result = provider.GetBaseLayoutValue(field);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetLayoutValue_WithLayoutFieldFromItemWithoutBaseLayoutField_ReturnsNull()
        {
            // Arrange
            var validator = Substitute.For<IBaseLayoutValidator>();
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutValueProvider(new[] {"master", "web"}, validator, log);
            var field = MasterFakesFactory.CreateFakeLayoutField(null, null, null, null, false);

            // Act
            var result = provider.GetBaseLayoutValue(field);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetLayoutValue_WithLayoutFieldFromItemWithNullBaseLayoutField_ReturnsNull()
        {
            // Arrange
            var validator = Substitute.For<IBaseLayoutValidator>();
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutValueProvider(new[] {"master", "web"}, validator, log);
            var field = MasterFakesFactory.CreateFakeLayoutField();

            // Act
            var result = provider.GetBaseLayoutValue(field);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetLayoutValue_WithLayoutFieldFromItemWithCircularBaseLayoutReference_LogsWarningAndReturnsNull()
        {
            // Arrange
            var validator = Substitute.For<IBaseLayoutValidator>();
            validator.HasCircularBaseLayoutReference(Arg.Any<BaseLayoutItem>()).Returns(true);
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutValueProvider(new[] {"master", "web"}, validator, log);
            var baseLayoutId = MasterFakesFactory.CreateFakeLayoutField().Item.ID;
            var field = MasterFakesFactory.CreateFakeLayoutField(null, null, null, baseLayoutId);

            // Act
            var result = provider.GetBaseLayoutValue(field);

            // Assert
            log.Received().Warn(Arg.Any<string>());
            Assert.Null(result);
        }

        [Fact]
        public void GetLayoutValue_WithLayoutFieldFromItemWithValidBaseLayout_ReturnsLayoutFieldValueFromBaseLayoutItem()
        {
            // Arrange
            var baseLayoutValue = "Hey, this is the base layout value!";
            var validator = Substitute.For<IBaseLayoutValidator>();
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutValueProvider(new[] {"master", "web"}, validator, log);
            var baseLayoutId = MasterFakesFactory.CreateFakeLayoutField(null, null, baseLayoutValue).Item.ID;
            var field = MasterFakesFactory.CreateFakeLayoutField(null, null, null, baseLayoutId);

            // Act
            var result = provider.GetBaseLayoutValue(field);

            // Assert
            Assert.Equal(baseLayoutValue, result);
        }
    }
}