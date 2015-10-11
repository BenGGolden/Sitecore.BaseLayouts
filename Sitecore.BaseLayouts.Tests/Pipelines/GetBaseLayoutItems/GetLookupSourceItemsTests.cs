using NSubstitute;
using Sitecore.BaseLayouts.Pipelines.GetBaseLayoutItems;
using Sitecore.Data.Items;
using Xunit;

namespace Sitecore.BaseLayouts.Tests.Pipelines.GetBaseLayoutItems
{
    public class GetLookupSourceItemsTests : FakeDbTestClass
    {
        [Fact]
        public void Process_WithItemWithoutBaseLayoutField_DoesNotCallGetItems()
        {
            // Arrange
            var args = new GetBaseLayoutItemsArgs(MasterFakesFactory.CreateFakeItem(null, null, null, null, false));
            var processor = Substitute.ForPartsOf<GetLookupSourceItems>();
            processor.When(p => p.GetItems(Arg.Any<Item>(), Arg.Any<string>())).DoNotCallBase();

            // Act
            processor.Process(args);

            // Assert
            processor.DidNotReceive().GetItems(Arg.Any<Item>(), Arg.Any<string>());
        }

        [Fact]
        public void Process_WhenGetItemsReturnsItems_AddsItemsToArgs()
        {
            // Arrange
            var item1 = MasterFakesFactory.CreateFakeItem();
            var item2 = MasterFakesFactory.CreateFakeItem();
            var args = new GetBaseLayoutItemsArgs(MasterFakesFactory.CreateFakeItem());
            var processor = Substitute.ForPartsOf<GetLookupSourceItems>();
            processor.When(p => p.GetItems(Arg.Any<Item>(), Arg.Any<string>())).DoNotCallBase();
            processor.GetItems(Arg.Any<Item>(), Arg.Any<string>()).Returns(new[] {item1, item2});

            // Act
            processor.Process(args);

            // Assert
            Assert.Equal(2, args.BaseLayoutItems.Count);
        }
    }
}