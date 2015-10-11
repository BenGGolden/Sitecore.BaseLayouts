using System;
using NSubstitute;
using Sitecore.BaseLayouts.Caching;
using Sitecore.BaseLayouts.Data;
using Sitecore.Data.Fields;
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
            cache.GetLayoutValue(Arg.Any<Field>()).Returns(layoutValue);
            var provider = new CachedBaseLayoutValueProvider(innerProvider, cache);
            var field = MasterFakesFactory.CreateFakeLayoutField();

            // Act
            var result = provider.GetBaseLayoutValue(field);

            // Assert
            Assert.Equal(layoutValue, result);
            innerProvider.DidNotReceive().GetBaseLayoutValue(Arg.Any<Field>());
        }

        [Fact]
        public void GetLayoutValue_WhenCacheReturnsNull_CallsInnerProviderAndAddsResultToCache()
        {
            // Arrange
            var layoutValue = "this is my layout value";
            var innerProvider = Substitute.For<IBaseLayoutValueProvider>();
            innerProvider.GetBaseLayoutValue(Arg.Any<Field>()).Returns(layoutValue);
            var cache = Substitute.For<IBaseLayoutValueCache>();
            cache.GetLayoutValue(Arg.Any<Field>()).Returns((string)null);
            var provider = new CachedBaseLayoutValueProvider(innerProvider, cache);
            var field = MasterFakesFactory.CreateFakeLayoutField();

            // Act
            var result = provider.GetBaseLayoutValue(field);

            // Assert
            Assert.Equal(layoutValue, result);
            innerProvider.Received().GetBaseLayoutValue(field);
            cache.Received().AddLayoutValue(field, layoutValue);
        }

        [Fact]
        public void GetLayoutValue_WhenCacheReturnsNullAndInnerProviderReturnsValue_AddsResultToCacheAndReturnsIt()
        {
            // Arrange
            var layoutValue = "this is my layout value";
            var innerProvider = Substitute.For<IBaseLayoutValueProvider>();
            innerProvider.GetBaseLayoutValue(Arg.Any<Field>()).Returns(layoutValue);
            var cache = Substitute.For<IBaseLayoutValueCache>();
            cache.GetLayoutValue(Arg.Any<Field>()).Returns((string)null);
            var provider = new CachedBaseLayoutValueProvider(innerProvider, cache);
            var field = MasterFakesFactory.CreateFakeLayoutField();

            // Act
            var result = provider.GetBaseLayoutValue(field);

            // Assert
            Assert.Equal(layoutValue, result);
            cache.Received().AddLayoutValue(field, layoutValue);
        }

        [Fact]
        public void GetLayoutValue_WhenCacheReturnsNullAndInnerProviderReturnsNull_DoesNotAddToCacheAndReturnsNull()
        {
            // Arrange
            var innerProvider = Substitute.For<IBaseLayoutValueProvider>();
            innerProvider.GetBaseLayoutValue(Arg.Any<Field>()).Returns((string)null);
            var cache = Substitute.For<IBaseLayoutValueCache>();
            cache.GetLayoutValue(Arg.Any<Field>()).Returns((string)null);
            var provider = new CachedBaseLayoutValueProvider(innerProvider, cache);
            var field = MasterFakesFactory.CreateFakeLayoutField();

            // Act
            var result = provider.GetBaseLayoutValue(field);

            // Assert
            Assert.Null(result);
            cache.DidNotReceive().AddLayoutValue(Arg.Any<Field>(), Arg.Any<string>());
        }
    }
}