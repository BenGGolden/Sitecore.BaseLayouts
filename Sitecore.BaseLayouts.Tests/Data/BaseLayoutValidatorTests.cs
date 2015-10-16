using System;
using Sitecore.BaseLayouts.Data;
using Sitecore.Data;
using Xunit;

namespace Sitecore.BaseLayouts.Tests.Data
{
    public class BaseLayoutValidatorTests : FakeDbTestClass
    {
        [Fact]
        public void HasCircularBaseLayoutReference_WithNullItem_ThrowsArgumentNullException()
        {
            // Arrange
            var validator = new BaseLayoutValidator();

            // Act => Assert
            Assert.Throws<ArgumentNullException>(() => validator.HasCircularBaseLayoutReference(null));
        }

        [Fact]
        public void HasCircularBaseLayoutReference_WithItemWithoutBaseLayoutField_ReturnsFalse()
        {
            // Arrange
            var validator = new BaseLayoutValidator();
            var item = MasterFakesFactory.CreateFakeItem(null, null, null, null, false);

            // Act
            var result = validator.HasCircularBaseLayoutReference(item);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HasCircularBaseLayoutReference_WithItemWithNoBaseLayout_ReturnsFalse()
        {
            // Arrange
            var validator = new BaseLayoutValidator();
            var item = MasterFakesFactory.CreateFakeItem();

            // Act
            var result = validator.HasCircularBaseLayoutReference(item);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HasCircularBaseLayoutReference_WithoutCircularReference_ReturnsFalse()
        {
            // Arrange
            var validator = new BaseLayoutValidator();
            var item = MasterFakesFactory.CreateFakeItem();

            // Act
            var result = validator.HasCircularBaseLayoutReference(item);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HasCircularBaseLayoutReference_WithSelfReference_ReturnsTrue()
        {
            // Arrange
            var validator = new BaseLayoutValidator();
            var id = new ID();
            var item = MasterFakesFactory.CreateFakeItem(id, null, null, id);

            // Act
            var result = validator.HasCircularBaseLayoutReference(item);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void HasCircularBaseLayoutReference_WithMultiLevelCircularReference_ReturnsTrue()
        {
            // Arrange
            var validator = new BaseLayoutValidator();
            var id = new ID();
            var baseId = new ID();
            var baseItem = MasterFakesFactory.CreateFakeItem(baseId, null, null, id);
            var item = MasterFakesFactory.CreateFakeItem(id, null, null, baseId);

            // Act
            var result = validator.HasCircularBaseLayoutReference(item);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CreatesCircularBaseLayoutReference_WithNullItem_ThrowsArgumentNullException()
        {
            // Arrange
            var validator = new BaseLayoutValidator();

            // Act => Assert
            Assert.Throws<ArgumentNullException>(() => validator.CreatesCircularBaseLayoutReference(null, MasterFakesFactory.CreateFakeItem()));
        }

        [Fact]
        public void CreatesCircularBaseLayoutReference_WithNullBaseLayoutItem_ThrowsArgumentNullException()
        {
            // Arrange
            var validator = new BaseLayoutValidator();

            // Act => Assert
            Assert.Throws<ArgumentNullException>(() => validator.CreatesCircularBaseLayoutReference(MasterFakesFactory.CreateFakeItem(), null));
        }

        [Fact]
        public void CreatesCircularBaseLayoutReference_WithItemWithoutBaseLayoutField_ThrowsArgumentException()
        {
            // Arrange
            var validator = new BaseLayoutValidator();

            // Act => Assert
            Assert.Throws<ArgumentException>(() => validator.CreatesCircularBaseLayoutReference(MasterFakesFactory.CreateFakeItem(null, null, null, null, false), MasterFakesFactory.CreateFakeItem()));
        }

        [Fact]
        public void CreatesCircularBaseLayoutReference_WithItemSameAsBaseLayoutItem_ReturnsTrue()
        {
            // Arrange
            var validator = new BaseLayoutValidator();
            var item = MasterFakesFactory.CreateFakeItem();

            // Act
            var result = validator.CreatesCircularBaseLayoutReference(item, item);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CreatesCircularBaseLayoutReference_WhenBaseLayoutItemHasItemSelectedAsBaseLayout_ReturnsTrue()
        {
            // Arrange
            var validator = new BaseLayoutValidator();
            var item = MasterFakesFactory.CreateFakeItem();
            var item2 = MasterFakesFactory.CreateFakeItem(null, null, null, item.ID);

            // Act
            var result = validator.CreatesCircularBaseLayoutReference(item, item2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CreatesCircularBaseLayoutReference_WithThreeNodeCycle_ReturnsTrue()
        {
            // Arrange
            var validator = new BaseLayoutValidator();
            var item = MasterFakesFactory.CreateFakeItem();
            var item2 = MasterFakesFactory.CreateFakeItem(null, null, null, item.ID);
            var item3 = MasterFakesFactory.CreateFakeItem(null, null, null, item2.ID);

            // Act
            var result = validator.CreatesCircularBaseLayoutReference(item, item3);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CreatesCircularBaseLayoutReference_WithLinearChain_ReturnsFalse()
        {
            // Arrange
            var validator = new BaseLayoutValidator();
            var item = MasterFakesFactory.CreateFakeItem();
            var item2 = MasterFakesFactory.CreateFakeItem();
            var item3 = MasterFakesFactory.CreateFakeItem(null, null, null, item2.ID);

            // Act
            var result = validator.CreatesCircularBaseLayoutReference(item, item3);

            // Assert
            Assert.False(result);
        }
    }
}