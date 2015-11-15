using Sitecore.BaseLayouts.Pipelines.SaveBaseLayout;
using Xunit;

namespace Sitecore.BaseLayouts.Tests.Pipelines.SaveBaseLayout
{
    public class SaveNewBaseLayoutTests : FakeDbTestClass
    {
        [Fact]
        public void Process_WithNewBaseLayoutItemNull_SetsFieldValueToEmptyString()
        {
            // Arrange
            var args = new SaveBaseLayoutArgs(MasterFakesFactory.CreateFakeItem());
            var processor = new SaveNewBaseLayout();

            // Act
            processor.Process(args);

            // Assert
            Assert.Equal(string.Empty, args.Item.InnerItem.Fields[BaseLayoutSettings.FieldId].Value);
        }

        [Fact]
        public void Process_WithNewBaseLayoutItemNull_SetsSuccessfulTrue()
        {
            // Arrange
            var args = new SaveBaseLayoutArgs(MasterFakesFactory.CreateFakeItem());
            var processor = new SaveNewBaseLayout();

            // Act
            processor.Process(args);

            // Assert
            Assert.True(args.Successful);
        }

        [Fact]
        public void Process_WithNewBaseLayoutItemNotNull_SetsFieldValueToIdString()
        {
            // Arrange
            var item = MasterFakesFactory.CreateFakeItem();
            var item2 = MasterFakesFactory.CreateFakeItem();
            var args = new SaveBaseLayoutArgs(item) {NewBaseLayoutItem = item2};
            var processor = new SaveNewBaseLayout();

            // Act
            processor.Process(args);

            // Assert
            Assert.Equal(item2.ID.ToString(), args.Item.InnerItem.Fields[BaseLayoutSettings.FieldId].Value);
        }

        [Fact]
        public void Process_WithNewBaseLayoutItemNotNull_SetsSuccessfulTrue()
        {
            // Arrange
            var item = MasterFakesFactory.CreateFakeItem();
            var item2 = MasterFakesFactory.CreateFakeItem();
            var args = new SaveBaseLayoutArgs(item) {NewBaseLayoutItem = item2};
            var processor = new SaveNewBaseLayout();

            // Act
            processor.Process(args);

            // Assert
            Assert.True(args.Successful);
        }
    }
}