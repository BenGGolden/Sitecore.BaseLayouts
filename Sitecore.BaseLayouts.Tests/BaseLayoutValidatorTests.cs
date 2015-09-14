using System;
using System.Linq.Expressions;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using Xunit;

namespace Sitecore.BaseLayouts.Tests
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
            var item = CreateFakeItem(Db, null, null, false);

            // Act
            var result = validator.HasCircularBaseLayoutReference(item);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HasCircularBaseLayoutReference_WithNoBaseLayout_ReturnsFalse()
        {
            // Arrange
            var validator = new BaseLayoutValidator();
            var item = CreateFakeItem(Db);

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
            var item = CreateFakeItem(Db);

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
            var item = CreateFakeItem(Db, id, id);

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
            var baseItem = CreateFakeItem(Db, baseId, id);
            var item = CreateFakeItem(Db, id, baseId);

            // Act
            var result = validator.HasCircularBaseLayoutReference(item);

            // Assert
            Assert.True(result);
        }

        private Item CreateFakeItem(Db db, ID id = null, ID baseLayoutId = null, bool addBaseLayoutField = true)
        {
            if (ID.IsNullOrEmpty(id))
            {
                id = new ID();
            }

            var item = new DbItem(id.ToShortID().ToString(), id);
            if (addBaseLayoutField)
            {
                item.Fields.Add(BaseLayoutSettings.FieldId,
                    ID.IsNullOrEmpty(baseLayoutId) ? null : baseLayoutId.ToString());
            }

            db.Add(item);
            return db.GetItem(id);
        }
    }
}