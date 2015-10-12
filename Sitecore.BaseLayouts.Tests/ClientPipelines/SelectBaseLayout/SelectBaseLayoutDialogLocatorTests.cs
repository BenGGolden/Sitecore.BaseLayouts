using NSubstitute;
using Sitecore.BaseLayouts.ClientPipelines.SelectBaseLayout;
using Sitecore.BaseLayouts.Pipelines;
using Sitecore.BaseLayouts.Pipelines.GetBaseLayoutItems;
using Sitecore.Data;
using Sitecore.Data.Items;
using Xunit;

namespace Sitecore.BaseLayouts.Tests.ClientPipelines.SelectBaseLayout
{
    public class SelectBaseLayoutDialogLocatorTests : FakeDbTestClass
    {
        [Fact]
        public void GetDialogUrl_WithNoBaseLayoutItems_ReturnsNull()
        {
            // Arrange
            var item = MasterFakesFactory.CreateFakeItem();
            var runner = Substitute.For<IPipelineRunner>();
            var locator = new SelectBaseLayoutDialogLocator(runner);

            // Act
            var result = locator.GetDialogUrl(item);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetDialogUrl_WithBaseLayoutItems_CallsGetUrlWithOptionsWithItems()
        {
            // Arrange
            var item = MasterFakesFactory.CreateFakeItem();
            var item2 = MasterFakesFactory.CreateFakeItem();
            var runner = Substitute.For<IPipelineRunner>();
            runner.When(r => r.Run(Arg.Any<GetBaseLayoutItemsArgs>()))
                .Do(x => x.Arg<GetBaseLayoutItemsArgs>().BaseLayoutItems.Add(item2));
            var locator = Substitute.ForPartsOf<SelectBaseLayoutDialogLocator>(runner);
            locator.When(l => l.GetUrl(Arg.Any<SelectBaseLayoutOptions>(), Arg.Any<Database>())).DoNotCallBase();
            
            // Act
            var result = locator.GetDialogUrl(item);

            // Assert
            locator.Received()
                .GetUrl(Arg.Is<SelectBaseLayoutOptions>(o => o.Items.Count == 1 && o.Items[0].ID == item2.ID),
                    Arg.Is<Database>(db => db.Name == item.Database.Name));
        }

        [Fact]
        public void GetDialogUrl_WithSelectedBaseLayout_CallsGetUrlWithOptionsWithCurrentBaseLayoutIdSet()
        {
            // Arrange
            var item = MasterFakesFactory.CreateFakeItem();
            var item2 = MasterFakesFactory.CreateFakeItem(null, null, null, item.ID);
            var runner = Substitute.For<IPipelineRunner>();
            runner.When(r => r.Run(Arg.Any<GetBaseLayoutItemsArgs>()))
                .Do(x => x.Arg<GetBaseLayoutItemsArgs>().BaseLayoutItems.Add(item));
            var locator = Substitute.ForPartsOf<SelectBaseLayoutDialogLocator>(runner);
            locator.When(l => l.GetUrl(Arg.Any<SelectBaseLayoutOptions>(), Arg.Any<Database>())).DoNotCallBase();

            // Act
            var result = locator.GetDialogUrl(item2);

            // Assert
            locator.Received()
                .GetUrl(Arg.Is<SelectBaseLayoutOptions>(o => o.CurrentBaseLayoutId == item.ID),
                    Arg.Is<Database>(db => db.Name == item.Database.Name));
        }
    }
}