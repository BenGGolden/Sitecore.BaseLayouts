using NSubstitute;
using Sitecore.BaseLayouts.Abstractions;
using Sitecore.BaseLayouts.Data;
using Xunit;

namespace Sitecore.BaseLayouts.Tests.Data
{
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

#if FINAL_LAYOUT
        [Fact]
        public void GetLayoutValue_WithFinalLayoutFieldFromUnsupportedDatabase_ReturnsNull()
        {
            // Arrange
            var validator = Substitute.For<IBaseLayoutValidator>();
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutValueProvider(new[] { "web" }, validator, log);
            var field = MasterFakesFactory.CreateFakeFinalLayoutField();

            // Act
            var result = provider.GetBaseLayoutValue(field);

            // Assert
            Assert.Null(result);
        }
#endif

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

#if FINAL_LAYOUT
        [Fact]
        public void GetLayoutValue_WithFinalLayoutFieldFromNonContentItem_ReturnsNull()
        {
            // Arrange
            var validator = Substitute.For<IBaseLayoutValidator>();
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutValueProvider(new[] { "master", "web" }, validator, log);
            var field = MasterFakesFactory.CreateFakeFinalLayoutField(null, ItemIDs.SystemRoot);

            // Act
            var result = provider.GetBaseLayoutValue(field);

            // Assert
            Assert.Null(result);
        }
#endif

        [Fact]
        public void GetLayoutValue_WithLayoutFieldFromItemWithoutBaseLayoutField_ReturnsNull()
        {
            // Arrange
            var validator = Substitute.For<IBaseLayoutValidator>();
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutValueProvider(new[] {"master", "web"}, validator, log);
            var field = MasterFakesFactory.CreateFakeLayoutField(null, null, null, null, null, false);

            // Act
            var result = provider.GetBaseLayoutValue(field);

            // Assert
            Assert.Null(result);
        }

#if FINAL_LAYOUT
        [Fact]
        public void GetLayoutValue_WithFinalLayoutFieldFromItemWithoutBaseLayoutField_ReturnsNull()
        {
            // Arrange
            var validator = Substitute.For<IBaseLayoutValidator>();
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutValueProvider(new[] { "master", "web" }, validator, log);
            var field = MasterFakesFactory.CreateFakeFinalLayoutField(null, null, null, null, null, false);

            // Act
            var result = provider.GetBaseLayoutValue(field);

            // Assert
            Assert.Null(result);
        }
#endif

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

#if FINAL_LAYOUT
        [Fact]
        public void GetLayoutValue_WithFinalLayoutFieldFromItemWithNullBaseLayoutField_ReturnsNull()
        {
            // Arrange
            var validator = Substitute.For<IBaseLayoutValidator>();
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutValueProvider(new[] { "master", "web" }, validator, log);
            var field = MasterFakesFactory.CreateFakeFinalLayoutField();

            // Act
            var result = provider.GetBaseLayoutValue(field);

            // Assert
            Assert.Null(result);
        }
#endif

        [Fact]
        public void GetLayoutValue_WithLayoutFieldFromItemWithCircularBaseLayoutReference_LogsWarningAndReturnsNull()
        {
            // Arrange
            var validator = Substitute.For<IBaseLayoutValidator>();
            validator.HasCircularBaseLayoutReference(Arg.Any<BaseLayoutItem>()).Returns(true);
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutValueProvider(new[] {"master", "web"}, validator, log);
            var baseLayoutId = MasterFakesFactory.CreateFakeItem().ID;
            var field = MasterFakesFactory.CreateFakeLayoutField(null, null, null, null, baseLayoutId);

            // Act
            var result = provider.GetBaseLayoutValue(field);

            // Assert
            log.Received().Warn(Arg.Any<string>());
            Assert.Null(result);
        }

#if FINAL_LAYOUT
        [Fact]
        public void GetLayoutValue_WithFinalLayoutFieldFromItemWithCircularBaseLayoutReference_LogsWarningAndReturnsNull()
        {
            // Arrange
            var validator = Substitute.For<IBaseLayoutValidator>();
            validator.HasCircularBaseLayoutReference(Arg.Any<BaseLayoutItem>()).Returns(true);
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutValueProvider(new[] { "master", "web" }, validator, log);
            var baseLayoutId = MasterFakesFactory.CreateFakeItem().ID;
            var field = MasterFakesFactory.CreateFakeFinalLayoutField(null, null, null, null, baseLayoutId);

            // Act
            var result = provider.GetBaseLayoutValue(field);

            // Assert
            log.Received().Warn(Arg.Any<string>());
            Assert.Null(result);
        }
#endif

        [Fact]
        public void GetLayoutValue_WithLayoutFieldFromItemWithValidBaseLayout_ReturnsLayoutFieldValueFromBaseLayoutItem()
        {
            // Arrange
            var baseLayoutValue = "<r><d>This is my layout value!</d></r>";
            var validator = Substitute.For<IBaseLayoutValidator>();
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutValueProvider(new[] {"master", "web"}, validator, log);
            var baseLayoutId = MasterFakesFactory.CreateFakeItem(null, null, baseLayoutValue, string.Empty).ID;
            var field = MasterFakesFactory.CreateFakeLayoutField(null, null, null, null, baseLayoutId);

            // Act
            var result = provider.GetBaseLayoutValue(field);

            // Assert
            Assert.Equal(baseLayoutValue, result);
        }

#if FINAL_LAYOUT
        [Fact]
        public void GetLayoutValue_WithFinalLayoutFieldFromItemWithValidBaseLayout_ReturnsLayoutFieldValueFromBaseLayoutItem()
        {
            // Arrange
            var baseLayoutValue = "<r><d>This is my layout value!</d></r>";
            var validator = Substitute.For<IBaseLayoutValidator>();
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutValueProvider(new[] { "master", "web" }, validator, log);
            var baseLayoutId = MasterFakesFactory.CreateFakeItem(null, null, null, baseLayoutValue).ID;
            var field = MasterFakesFactory.CreateFakeFinalLayoutField(null, null, null, null, baseLayoutId);

            // Act
            var result = provider.GetBaseLayoutValue(field);

            // Assert
            Assert.Equal(baseLayoutValue, result);
        }
#endif
    }
}