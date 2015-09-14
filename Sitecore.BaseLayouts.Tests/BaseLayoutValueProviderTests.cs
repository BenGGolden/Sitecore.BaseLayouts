using System;
using NSubstitute;
using Sitecore.BaseLayouts.Diagnostics;
using Sitecore.Data.Items;
using Sitecore.FakeDb;

namespace Sitecore.BaseLayouts.Tests
{
    using Xunit;

    public class BaseLayoutValueProviderTests : FakeDbTestClass
    {
        private FakeFieldFactory _fakeFieldFactory;

        public BaseLayoutValueProviderTests()
        {
            _fakeFieldFactory = new FakeFieldFactory();
        }

        [Fact]
        public void GetLayoutValue_WithNonLayoutField_ReturnsNull()
        {
            // Arrange
            var validator = Substitute.For<IBaseLayoutValidator>();
            var log = Substitute.For<ILog>();
            var field = _fakeFieldFactory.CreateFakeEmptyField(Db);
            var provider = new BaseLayoutValueProvider("master|web", validator, log);

            // Act
            var result = provider.GetLayoutValue(field);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetLayoutValue_WithLayoutFieldFromUnsupportedDatabase_ReturnsNull()
        {
            // Arrange
            var validator = Substitute.For<IBaseLayoutValidator>();
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutValueProvider("web", validator, log);
            var field = _fakeFieldFactory.CreateFakeLayoutField(Db);

            // Act
            var result = provider.GetLayoutValue(field);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetLayoutValue_WithLayoutFieldFromNonContentItem_ReturnsNull()
        {
            // Arrange
            var validator = Substitute.For<IBaseLayoutValidator>();
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutValueProvider("master|web", validator, log);
            var field = _fakeFieldFactory.CreateFakeLayoutField(Db, null, ItemIDs.SystemRoot);

            // Act
            var result = provider.GetLayoutValue(field);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetLayoutValue_WithLayoutFieldFromItemWithoutBaseLayoutField_ReturnsNull()
        {
            // Arrange
            var validator = Substitute.For<IBaseLayoutValidator>();
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutValueProvider("master|web", validator, log);
            var field = _fakeFieldFactory.CreateFakeLayoutField(Db, null, null, null, null, false);

            // Act
            var result = provider.GetLayoutValue(field);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetLayoutValue_WithLayoutFieldFromItemWithNullBaseLayoutField_ReturnsNull()
        {
            // Arrange
            var validator = Substitute.For<IBaseLayoutValidator>();
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutValueProvider("master|web", validator, log);
            var field = _fakeFieldFactory.CreateFakeLayoutField(Db);

            // Act
            var result = provider.GetLayoutValue(field);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetLayoutValue_WithLayoutFieldFromItemWithCircularBaseLayoutReference_LogsWarningAndReturnsNull()
        {
            // Arrange
            var validator = Substitute.For<IBaseLayoutValidator>();
            validator.HasCircularBaseLayoutReference(Arg.Any<Item>()).Returns(true);
            var log = Substitute.For<ILog>();
            var provider = new BaseLayoutValueProvider("master|web", validator, log);
            var baseLayoutId = _fakeFieldFactory.CreateFakeLayoutField(Db).Item.ID;
            var field = _fakeFieldFactory.CreateFakeLayoutField(Db, null, null, null, baseLayoutId);

            // Act
            var result = provider.GetLayoutValue(field);

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
            var provider = new BaseLayoutValueProvider("master|web", validator, log);
            var baseLayoutId = _fakeFieldFactory.CreateFakeLayoutField(Db, null, null, baseLayoutValue).Item.ID;
            var field = _fakeFieldFactory.CreateFakeLayoutField(Db, null, null, null, baseLayoutId);

            // Act
            var result = provider.GetLayoutValue(field);

            // Assert
            Assert.Equal(baseLayoutValue, result);
        }
    }
}