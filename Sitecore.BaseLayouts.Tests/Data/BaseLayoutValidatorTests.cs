﻿using System;
using NSubstitute;
using Sitecore.BaseLayouts.Data;
using Sitecore.Data;
using Xunit;

namespace Sitecore.BaseLayouts.Tests.Data
{
    public class BaseLayoutValidatorTests : FakeDbTestClass
    {
        [Fact]
        public void ItemSupportsBaseLayouts_WithContentItemWithBaseLayoutFieldFromSupportedDatabase_ReturnsTrue()
        {
            // Arrange
            var validator = new BaseLayoutValidator(TestUtil.CreateFakeSettings());
            var item = MasterFakesFactory.CreateFakeItem();

            // Act
            var result = validator.ItemSupportsBaseLayouts(item);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ItemSupportsBaseLayouts_WithNonContentItemWithBaseLayoutFieldFromSupportedDatabase_ReturnsFalse()
        {
            // Arrange
            var validator = new BaseLayoutValidator(TestUtil.CreateFakeSettings());
            var item = MasterFakesFactory.CreateFakeItem(null, ItemIDs.SystemRoot);

            // Act
            var result = validator.ItemSupportsBaseLayouts(item);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ItemSupportsBaseLayouts_WithContentItemWithoutBaseLayoutFieldFromSupportedDatabase_ReturnsFalse()
        {
            // Arrange
            var validator = new BaseLayoutValidator(TestUtil.CreateFakeSettings());
            var item = MasterFakesFactory.CreateFakeItem(null, null, null, null, null, false);

            // Act
            var result = validator.ItemSupportsBaseLayouts(item);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ItemSupportsBaseLayouts_WithContentItemWithBaseLayoutFieldFromUnsupportedDatabase_ReturnsFalse()
        {
            // Arrange
            var validator = new BaseLayoutValidator(TestUtil.CreateFakeSettings(new[] {"web"}));
            var item = MasterFakesFactory.CreateFakeItem();

            // Act
            var result = validator.ItemSupportsBaseLayouts(item);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HasCircularBaseLayoutReference_WithNullItem_ThrowsArgumentNullException()
        {
            // Arrange
            var validator = new BaseLayoutValidator(TestUtil.CreateFakeSettings());

            // Act => Assert
            Assert.Throws<ArgumentNullException>(() => validator.HasCircularBaseLayoutReference(null));
        }

        [Fact]
        public void HasCircularBaseLayoutReference_WithItemWithoutBaseLayoutField_ReturnsFalse()
        {
            // Arrange
            var validator = new BaseLayoutValidator(TestUtil.CreateFakeSettings());
            var item = MasterFakesFactory.CreateFakeItem(null, null, null, null, null, false);

            // Act
            var result = validator.HasCircularBaseLayoutReference(item);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HasCircularBaseLayoutReference_WithItemWithNoBaseLayout_ReturnsFalse()
        {
            // Arrange
            var validator = new BaseLayoutValidator(TestUtil.CreateFakeSettings());
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
            var validator = new BaseLayoutValidator(TestUtil.CreateFakeSettings());
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
            var validator = new BaseLayoutValidator(TestUtil.CreateFakeSettings());
            var id = new ID();
            var item = MasterFakesFactory.CreateFakeItem(id, null, null, null, id);

            // Act
            var result = validator.HasCircularBaseLayoutReference(item);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void HasCircularBaseLayoutReference_WithMultiLevelCircularReference_ReturnsTrue()
        {
            // Arrange
            var validator = new BaseLayoutValidator(TestUtil.CreateFakeSettings());
            var id = new ID();
            var baseId = new ID();
            var baseItem = MasterFakesFactory.CreateFakeItem(baseId, null, null, null, id);
            var item = MasterFakesFactory.CreateFakeItem(id, null, null, null, baseId);

            // Act
            var result = validator.HasCircularBaseLayoutReference(item);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CreatesCircularBaseLayoutReference_WithNullItem_ThrowsArgumentNullException()
        {
            // Arrange
            var validator = new BaseLayoutValidator(TestUtil.CreateFakeSettings());

            // Act => Assert
            Assert.Throws<ArgumentNullException>(
                () => validator.CreatesCircularBaseLayoutReference(null, MasterFakesFactory.CreateFakeItem()));
        }

        [Fact]
        public void CreatesCircularBaseLayoutReference_WithNullBaseLayoutItem_ThrowsArgumentNullException()
        {
            // Arrange
            var validator = new BaseLayoutValidator(TestUtil.CreateFakeSettings());

            // Act => Assert
            Assert.Throws<ArgumentNullException>(
                () => validator.CreatesCircularBaseLayoutReference(MasterFakesFactory.CreateFakeItem(), null));
        }

        [Fact]
        public void CreatesCircularBaseLayoutReference_WithItemWithoutBaseLayoutField_ThrowsArgumentException()
        {
            // Arrange
            var validator = new BaseLayoutValidator(TestUtil.CreateFakeSettings());

            // Act => Assert
            Assert.Throws<ArgumentException>(
                () =>
                    validator.CreatesCircularBaseLayoutReference(
                        MasterFakesFactory.CreateFakeItem(null, null, null, null, null, false),
                        MasterFakesFactory.CreateFakeItem()));
        }

        [Fact]
        public void CreatesCircularBaseLayoutReference_WithItemSameAsBaseLayoutItem_ReturnsTrue()
        {
            // Arrange
            var validator = new BaseLayoutValidator(TestUtil.CreateFakeSettings());
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
            var validator = new BaseLayoutValidator(TestUtil.CreateFakeSettings());
            var item = MasterFakesFactory.CreateFakeItem();
            var item2 = MasterFakesFactory.CreateFakeItem(null, null, null, null, item.ID);

            // Act
            var result = validator.CreatesCircularBaseLayoutReference(item, item2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CreatesCircularBaseLayoutReference_WithThreeNodeCycle_ReturnsTrue()
        {
            // Arrange
            var validator = new BaseLayoutValidator(TestUtil.CreateFakeSettings());
            var item = MasterFakesFactory.CreateFakeItem();
            var item2 = MasterFakesFactory.CreateFakeItem(null, null, null, null, item.ID);
            var item3 = MasterFakesFactory.CreateFakeItem(null, null, null, null, item2.ID);

            // Act
            var result = validator.CreatesCircularBaseLayoutReference(item, item3);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CreatesCircularBaseLayoutReference_WithLinearChain_ReturnsFalse()
        {
            // Arrange
            var validator = new BaseLayoutValidator(TestUtil.CreateFakeSettings());
            var item = MasterFakesFactory.CreateFakeItem();
            var item2 = MasterFakesFactory.CreateFakeItem();
            var item3 = MasterFakesFactory.CreateFakeItem(null, null, null, null, item2.ID);

            // Act
            var result = validator.CreatesCircularBaseLayoutReference(item, item3);

            // Assert
            Assert.False(result);
        }

#if FINAL_LAYOUT
        [Fact]
        public void CreatesVersioningConflict_WhenItemHasOnlyFinalRenderings_ReturnsFalse()
        {
            // Arrange
            var validator = new BaseLayoutValidator(TestUtil.CreateFakeSettings());
            var item = MasterFakesFactory.CreateFakeItem(null, null, string.Empty);
            var item2 = MasterFakesFactory.CreateFakeItem();

            // Act
            var result = validator.CreatesVersioningConflict(item, item2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CreatesVersioningConflict_WhenBaseLayoutItemHasOnlySharedRenderings_ReturnsFalse()
        {
            // Arrange
            var validator = new BaseLayoutValidator(TestUtil.CreateFakeSettings());
            var item = MasterFakesFactory.CreateFakeItem();
            var item2 = MasterFakesFactory.CreateFakeItem(null, null, null, string.Empty);

            // Act
            var result = validator.CreatesVersioningConflict(item, item2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CreatesVersioningConflict_WhenItemHasRenderingsSeedAndBaseLayoutItemHasFinalRenderings_ReturnsFalse()
        {
            // Arrange
            var validator = new BaseLayoutValidator(TestUtil.CreateFakeSettings());
            var item = MasterFakesFactory.CreateFakeItem();
            var item2 = MasterFakesFactory.CreateFakeItem();

            // Act
            var result = validator.CreatesVersioningConflict(item, item2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CreatesVersioningConflict_WhenItemHasRenderingsDeltaAndBaseLayoutItemHasFinalRenderings_ReturnsTrue()
        {
            // Arrange
            var validator = new BaseLayoutValidator(TestUtil.CreateFakeSettings());
            var item = MasterFakesFactory.CreateFakeItem(null, null, "<r xmlns:p=\"p\" xmlns:s=\"s\" p:p=\"1\"></r>");
            var item2 = MasterFakesFactory.CreateFakeItem();

            // Act
            var result = validator.CreatesVersioningConflict(item, item2);

            // Assert
            Assert.True(result);
        }
#endif
    }
}