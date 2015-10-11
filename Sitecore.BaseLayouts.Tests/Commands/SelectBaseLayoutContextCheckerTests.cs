using NSubstitute;
using Sitecore.BaseLayouts.Abstractions;
using Sitecore.BaseLayouts.Commands;
using Sitecore.BaseLayouts.Pipelines;
using Sitecore.BaseLayouts.Pipelines.GetBaseLayoutItems;
using Sitecore.Data.Items;
using Xunit;

namespace Sitecore.BaseLayouts.Tests.Commands
{
    public class SelectBaseLayoutContextCheckerTests : FakeDbTestClass
    {
        [Fact]
        public void CanExecute_WhenCanWebEditReturnsFalse_ReturnsFalse()
        {
            // Arrange
            var item = MasterFakesFactory.CreateFakeItem();
            var access = Substitute.For<IPageModeAccess>();
            access.CanWebEdit().Returns(false);
            var runner = Substitute.For<IPipelineRunner>();
            var checker = new SelectBaseLayoutContextChecker(access, runner);

            // Act
            var result = checker.CanExecute(item);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanExecute_WhenCanDesignItemReturnsFalse_ReturnsFalse()
        {
            // Arrange
            var item = MasterFakesFactory.CreateFakeItem();
            var access = Substitute.For<IPageModeAccess>();
            access.CanWebEdit().Returns(true);
            access.CanDesignItem(Arg.Any<Item>()).Returns(false);
            var runner = Substitute.For<IPipelineRunner>();
            var checker = new SelectBaseLayoutContextChecker(access, runner);

            // Act
            var result = checker.CanExecute(item);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanExecute_WhenInitialChecksPassAndPipelineReturnsWithNoBaseLayoutItems_ReturnsFalse()
        {
            // Arrange
            var item = MasterFakesFactory.CreateFakeItem();
            var access = Substitute.For<IPageModeAccess>();
            access.CanWebEdit().Returns(true);
            access.CanDesignItem(Arg.Any<Item>()).Returns(true);
            var runner = Substitute.For<IPipelineRunner>();
            var checker = new SelectBaseLayoutContextChecker(access, runner);

            // Act
            var result = checker.CanExecute(item);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanExecute_WhenInitialChecksPassAndPipelineReturnsWithBaseLayoutItems_ReturnsTrue()
        {
            // Arrange
            var item = MasterFakesFactory.CreateFakeItem();
            var item2 = MasterFakesFactory.CreateFakeItem();
            var access = Substitute.For<IPageModeAccess>();
            access.CanWebEdit().Returns(true);
            access.CanDesignItem(Arg.Any<Item>()).Returns(true);
            var runner = Substitute.For<IPipelineRunner>();
            runner.When(r => r.Run(Arg.Any<GetBaseLayoutItemsArgs>()))
                .Do(x => x.Arg<GetBaseLayoutItemsArgs>().BaseLayoutItems.Add(item2));
            var checker = new SelectBaseLayoutContextChecker(access, runner);

            // Act
            var result = checker.CanExecute(item);

            // Assert
            Assert.True(result);
        }
    }
}