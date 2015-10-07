using Lucene.Net.Search;
using Sitecore.BaseLayouts.Caching;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using Xunit;

namespace Sitecore.BaseLayouts.Tests.Caching
{
    public class LayoutValueCacheTests : FakeDbTestClass
    {
        [Fact]
        public void AddLayoutValue_WithFieldNotInCache_AddsEntry()
        {
            // Arrange
            var cache = new BaseLayoutValueCache(new string[0]) { Enabled = true };
            cache.Clear();
            var field = MasterFakesFactory.CreateFakeLayoutField();
            
            // Act
            cache.AddLayoutValue(field, field.Value);

            // Assert
            Assert.Equal(1, cache.InnerCache.Count);
        }

        [Fact]
        public void AddLayoutValue_WithFieldInCache_UpdatesEntry()
        {
            // Arrange
            var newValue = "This is the new layout value";
            var cache = new BaseLayoutValueCache(new string[0]) { Enabled = true };
            cache.Clear();
            var field = MasterFakesFactory.CreateFakeLayoutField();
            cache.AddLayoutValue(field, field.Value);
            var initalCount = cache.InnerCache.Count;

            // Act
            cache.AddLayoutValue(field, newValue);
            var result = cache.GetLayoutValue(field);

            // Assert
            Assert.Equal(newValue, result);
            Assert.Equal(initalCount, cache.InnerCache.Count);
        }

        [Fact]
        public void AddLayoutValue_WithFieldsFromDifferentItems_AddsEntriesForBoth()
        {
            // Arrange
            var cache = new BaseLayoutValueCache(new string[0]) { Enabled = true };
            cache.Clear();
            var field1 = MasterFakesFactory.CreateFakeLayoutField();
            var field2 = MasterFakesFactory.CreateFakeLayoutField();

            // Act
            cache.AddLayoutValue(field1, field1.Value);
            cache.AddLayoutValue(field2, field2.Value);

            // Assert
            Assert.Equal(2, cache.InnerCache.Count);
        }

        [Fact]
        public void AddLayoutValue_WithSameFieldInDifferentDatabases_AddsEntriesForBoth()
        {
            // Arrange
            var cache = new BaseLayoutValueCache(new string[0]) { Enabled = true };
            cache.Clear();
            var id = new ID();
            var masterField = MasterFakesFactory.CreateFakeLayoutField(id);
            Field webField;
            using (var webDb = new Db("web"))
            {
                var webFakesFactory = new FakesFactory(webDb);
                webField = webFakesFactory.CreateFakeLayoutField(id);
            }
            
            // Act
            cache.AddLayoutValue(masterField, masterField.Value);
            cache.AddLayoutValue(webField, webField.Value);

            // Assert
            Assert.Equal(2, cache.InnerCache.Count);
        }

        [Fact]
        public void GetLayoutValue_WithEmptyCache_ReturnsNull()
        {
            // Arrange
            var field = MasterFakesFactory.CreateFakeLayoutField();
            var cache = new BaseLayoutValueCache(new string[0]) {Enabled = true};
            cache.Clear();

            // Act
            var result = cache.GetLayoutValue(field);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetLayoutValue_AfterAddLayoutValue_ReturnsAddedValue()
        {
            // Arrange
            var value = "Here be ye olde layout value.";
            var field = MasterFakesFactory.CreateFakeLayoutField(null, null, value);
            var cache = new BaseLayoutValueCache(new string[0]) {Enabled = true};
            cache.Clear();

            // Act
            cache.AddLayoutValue(field, value);
            var result = cache.GetLayoutValue(field);

            // Assert
            Assert.Equal(value, result);
        }

        [Fact]
        public void ProcessItemUpdate_WithUnrelatedItem_DoesNotRemoveEntries()
        {
            // Arrange
            var cache = new BaseLayoutValueCache(new string[0]) { Enabled = true };
            cache.Clear();
            var count = 3;
            for (int i = 0; i < count; i++)
            {
                var field = MasterFakesFactory.CreateFakeLayoutField();
                cache.AddLayoutValue(field, field.Value);
            }

            // Act
            cache.ProcessItemUpdate(MasterFakesFactory.CreateFakeItem());

            // Assert
            Assert.Equal(count, cache.InnerCache.Count);
        }

        [Fact]
        public void ProcessItemUpdate_WithItemMatchingEntryItem_RemovesEntries()
        {
            // Arrange
            var cache = new BaseLayoutValueCache(new string[0]) { Enabled = true };
            cache.Clear();
            Field field = null;
            var count = 3;
            for (int i = 0; i < count; i++)
            {
                field = MasterFakesFactory.CreateFakeLayoutField();
                cache.AddLayoutValue(field, field.Value);
            }

            // Act
            cache.ProcessItemUpdate(field.Item);

            // Assert
            Assert.True(count > cache.InnerCache.Count);
        }

        [Fact]
        public void ProcessItemUpdate_WithBaseLayoutOfItemWithEntry_RemovesEntry()
        {
            // Arrange
            var cache = new BaseLayoutValueCache(new string[0]) { Enabled = true };
            cache.Clear();
            var baseLayoutItem = MasterFakesFactory.CreateFakeItem();
            var field = MasterFakesFactory.CreateFakeLayoutField(null, null, null, baseLayoutItem.ID);
            cache.AddLayoutValue(field, field.Value);

            // Act
            cache.ProcessItemUpdate(baseLayoutItem);

            // Assert
            Assert.Equal(0, cache.InnerCache.Count);
        }

        [Fact]
        public void ProcessItemUpdate_WithEntriesInDifferentDatabases_OnlyRemovesEntryForMatchingDatabase()
        {
            // Arrange
            var cache = new BaseLayoutValueCache(new string[0]) { Enabled = true };
            cache.Clear();
            var id = new ID();
            var masterField = MasterFakesFactory.CreateFakeLayoutField(id);
            Field webField;
            using (var webDb = new Db("web"))
            {
                var webFakesFactory = new FakesFactory(webDb);
                webField = webFakesFactory.CreateFakeLayoutField(id);
            }

            cache.AddLayoutValue(masterField, masterField.Value);
            cache.AddLayoutValue(webField, webField.Value);

            // Act
            cache.ProcessItemUpdate(masterField.Item);

            // Assert
            Assert.Null(cache.GetLayoutValue(masterField));
            Assert.Equal(1, cache.InnerCache.Count);
        }

        [Fact]
        public void ProcessItemUpdate_WithEntriesForBaseLayoutChain_OnlyRemovesEntriesForDependentItems()
        {
            // Arrange
            var cache = new BaseLayoutValueCache(new string[0]) { Enabled = true };
            cache.Clear();

            // create 2 base layout chains of 5 items each
            // save the 3rd item in the first chain as the item to pass to ProcessItemUpdate
            ID id = null;
            Item updatedItem = null;
            for (int i = 0; i < 10; i++)
            {
                var field = MasterFakesFactory.CreateFakeLayoutField(null, null, null, id);
                cache.AddLayoutValue(field, field.Value);
                id = (i == 4) ? null : field.Item.ID;
                if (i == 2) updatedItem = field.Item;
            }

            // Act
            cache.ProcessItemUpdate(updatedItem);

            // Assert
            Assert.Equal(7, cache.InnerCache.Count);
        }

        [Fact]
        public void ProcessItemUpdate_WithStandardValuesItem_RemovesAllEntriesForMatchingDatabase()
        {
            // Arrange
            var cache = new BaseLayoutValueCache(new string[0]) { Enabled = true };
            cache.Clear();
            for (int i = 0; i < 3; i++)
            {
                var masterField = MasterFakesFactory.CreateFakeLayoutField();
                cache.AddLayoutValue(masterField, masterField.Value);
            }

            using (var webDb = new Db("web"))
            {
                var webFakesFactory = new FakesFactory(webDb);
                for (int i = 0; i < 3; i++)
                {
                    var webField = webFakesFactory.CreateFakeLayoutField();
                    cache.AddLayoutValue(webField, webField.Value);
                }
            }

            var tid = new ID();
            MasterDb.Add(new DbTemplate("Test", tid)
            {
                Fields = {{"Title", "$name"}},
                Children = { new DbItem("__Standard Values", new ID(), tid) }
            });

            var standardValues = MasterDb.GetItem("/sitecore/templates/Test/__Standard Values");

            // Act
            cache.ProcessItemUpdate(standardValues);

            // Assert
            Assert.Equal(3, cache.InnerCache.Count);
            Assert.Equal(0, cache.InnerCache.GetCacheKeys("master:").Count);
        }
    }
}