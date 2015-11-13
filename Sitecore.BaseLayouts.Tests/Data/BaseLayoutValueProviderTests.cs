using NSubstitute;
using Sitecore.BaseLayouts.Abstractions;
using Sitecore.BaseLayouts.Data;
using Xunit;

namespace Sitecore.BaseLayouts.Tests.Data
{
    public class BaseLayoutValueProviderTests : FakeDbTestClass
    {
        [Fact]
        public void GetLayoutValue_WithItemWithoutBaseLayoutField_ReturnsNull()
        {
            // Arrange
            var provider = new BaseLayoutValueProvider();
            var item = MasterFakesFactory.CreateFakeItem(null, null, null, null, null, false);

            // Act
            var result = provider.GetBaseLayoutValue(item);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetLayoutValue_WithItemWithNullBaseLayoutField_ReturnsNull()
        {
            // Arrange
            var provider = new BaseLayoutValueProvider();
            var item = MasterFakesFactory.CreateFakeItem();

            // Act
            var result = provider.GetBaseLayoutValue(item);

            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public void GetLayoutValue_WithItemWithValidBaseLayout_ReturnsLayoutFieldValueFromBaseLayoutItem()
        {
            // Arrange
            var baseLayoutValue = "<r><d>This is my layout value!</d></r>";
            var provider = new BaseLayoutValueProvider();
            var baseLayoutId = MasterFakesFactory.CreateFakeItem(null, null, baseLayoutValue, string.Empty).ID;
            var item = MasterFakesFactory.CreateFakeItem(null, null, null, null, baseLayoutId);

            // Act
            var result = provider.GetBaseLayoutValue(item);

            // Assert
            Assert.Equal(baseLayoutValue, result);
        }
    }
}