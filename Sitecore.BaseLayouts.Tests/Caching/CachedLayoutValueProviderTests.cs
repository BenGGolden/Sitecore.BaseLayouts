using NSubstitute;
using Sitecore.BaseLayouts.Caching;
using Sitecore.BaseLayouts.Data;
using Sitecore.Data.Items;
using Xunit;

namespace Sitecore.BaseLayouts.Tests.Caching
{
    public class CachedLayoutValueProviderTests : FakeDbTestClass
    {
        [Fact]
        public void GetLayoutValue_WhenCacheReturnsVaue_ReturnsThatValueAndeDoesNotCallInnerProvider()
        {
            // Arrange
            var layoutValue = "this is my layout value";
            var innerProvider = Substitute.For<IBaseLayoutValueProvider>();
            var cache = Substitute.For<IBaseLayoutValueCache>();
            cache.GetLayoutValue(Arg.Any<Item>()).Returns(layoutValue);
            var provider = new CachedBaseLayoutValueProvider(innerProvider, cache);
            var item = MasterFakesFactory.CreateFakeItem();

            // Act
            var result = provider.GetBaseLayoutValue(item);

            // Assert
            Assert.Equal(layoutValue, result);
            innerProvider.DidNotReceive().GetBaseLayoutValue(Arg.Any<Item>());
        }

        [Fact]
        public void GetLayoutValue_WhenCacheReturnsNull_CallsInnerProviderAndAddsResultToCache()
        {
            // Arrange
            var layoutValue = "this is my layout value";
            var innerProvider = Substitute.For<IBaseLayoutValueProvider>();
            innerProvider.GetBaseLayoutValue(Arg.Any<Item>()).Returns(layoutValue);
            var cache = Substitute.For<IBaseLayoutValueCache>();
            cache.GetLayoutValue(Arg.Any<Item>()).Returns((string) null);
            var provider = new CachedBaseLayoutValueProvider(innerProvider, cache);
            var item = MasterFakesFactory.CreateFakeItem();

            // Act
            var result = provider.GetBaseLayoutValue(item);

            // Assert
            Assert.Equal(layoutValue, result);
            innerProvider.Received().GetBaseLayoutValue(item);
            cache.Received().AddLayoutValue(item, layoutValue);
        }

        [Fact]
        public void GetLayoutValue_WhenCacheReturnsNullAndInnerProviderReturnsValue_AddsResultToCacheAndReturnsIt()
        {
            // Arrange
            var layoutValue = "this is my layout value";
            var innerProvider = Substitute.For<IBaseLayoutValueProvider>();
            innerProvider.GetBaseLayoutValue(Arg.Any<Item>()).Returns(layoutValue);
            var cache = Substitute.For<IBaseLayoutValueCache>();
            cache.GetLayoutValue(Arg.Any<Item>()).Returns((string) null);
            var provider = new CachedBaseLayoutValueProvider(innerProvider, cache);
            var item = MasterFakesFactory.CreateFakeItem();

            // Act
            var result = provider.GetBaseLayoutValue(item);

            // Assert
            Assert.Equal(layoutValue, result);
            cache.Received().AddLayoutValue(item, layoutValue);
        }

        [Fact]
        public void GetLayoutValue_WhenCacheReturnsNullAndInnerProviderReturnsNull_DoesNotAddToCacheAndReturnsNull()
        {
            // Arrange
            var innerProvider = Substitute.For<IBaseLayoutValueProvider>();
            innerProvider.GetBaseLayoutValue(Arg.Any<Item>()).Returns((string) null);
            var cache = Substitute.For<IBaseLayoutValueCache>();
            cache.GetLayoutValue(Arg.Any<Item>()).Returns((string) null);
            var provider = new CachedBaseLayoutValueProvider(innerProvider, cache);
            var item = MasterFakesFactory.CreateFakeItem();

            // Act
            var result = provider.GetBaseLayoutValue(item);

            // Assert
            Assert.Null(result);
            cache.DidNotReceive().AddLayoutValue(Arg.Any<Item>(), Arg.Any<string>());
        }
    }
}