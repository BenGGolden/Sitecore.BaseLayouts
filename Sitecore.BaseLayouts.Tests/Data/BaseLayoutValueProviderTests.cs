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
            var field = MasterFakesFactory.CreateFakeEmptyField();
            var provider = new BaseLayoutValueProvider();

            // Act
            var result = provider.GetBaseLayoutValue(field);

            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public void GetLayoutValue_WithLayoutFieldFromItemWithoutBaseLayoutField_ReturnsNull()
        {
            // Arrange
            var provider = new BaseLayoutValueProvider();
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
            var provider = new BaseLayoutValueProvider();
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
            var provider = new BaseLayoutValueProvider();
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
            var provider = new BaseLayoutValueProvider();
            var field = MasterFakesFactory.CreateFakeFinalLayoutField();

            // Act
            var result = provider.GetBaseLayoutValue(field);

            // Assert
            Assert.Null(result);
        }
#endif

        [Fact]
        public void GetLayoutValue_WithLayoutFieldFromItemWithValidBaseLayout_ReturnsLayoutFieldValueFromBaseLayoutItem()
        {
            // Arrange
            var baseLayoutValue = "<r><d>This is my layout value!</d></r>";
            var provider = new BaseLayoutValueProvider();
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
            var provider = new BaseLayoutValueProvider();
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