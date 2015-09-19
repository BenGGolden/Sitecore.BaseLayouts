using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using Sitecore.BaseLayouts.Commands;
using Sitecore.Data.Items;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Tasks;
using Xunit;

namespace Sitecore.BaseLayouts.Tests.Commands
{
    public class SelectBaseLayoutTests : FakeDbTestClass
    {
        [Fact]
        public void QueryState_WithCanEditFalse_ReturnsHidden()
        {
            // Arrange
            var context = new CommandContext(MasterFakesFactory.CreateFakeItem());
            var command = Substitute.ForPartsOf<SelectBaseLayout>();
            command.When(c => c.CanEdit()).DoNotCallBase();
            command.CanEdit().Returns(false);
            command.GetBaseLayoutItems(Arg.Any<Item>()).Returns(CreateBaseLayoutItems());

            // Act
            var result = command.QueryState(context);

            // Assert
            Assert.Equal(CommandState.Hidden, result);
        }

        [Fact]
        public void QueryState_WithNoItemInContext_ReturnsHidden()
        {
            // Arrange
            var context = new CommandContext();
            var command = Substitute.ForPartsOf<SelectBaseLayout>();
            command.When(c => c.CanEdit()).DoNotCallBase();
            command.CanEdit().Returns(true);
            command.GetBaseLayoutItems(Arg.Any<Item>()).Returns(CreateBaseLayoutItems());

            // Act
            var result = command.QueryState(context);

            // Assert
            Assert.Equal(CommandState.Hidden, result);
        }

        [Fact]
        public void QueryState_WithItemWithoutBaseLayoutField_ReturnsHidden()
        {
            // Arrange
            var context = new CommandContext(MasterFakesFactory.CreateFakeItem(null, null, null, null, false));
            var command = Substitute.ForPartsOf<SelectBaseLayout>();
            command.When(c => c.CanEdit()).DoNotCallBase();
            command.CanEdit().Returns(true);
            command.GetBaseLayoutItems(Arg.Any<Item>()).Returns(CreateBaseLayoutItems());

            // Act
            var result = command.QueryState(context);

            // Assert
            Assert.Equal(CommandState.Hidden, result);
        }

        [Fact]
        public void QueryState_WhenGetBaseLayoutsReturnsNull_ReturnsHidden()
        {
            // Arrange
            var context = new CommandContext(MasterFakesFactory.CreateFakeItem());
            var command = Substitute.ForPartsOf<SelectBaseLayout>();
            command.When(c => c.CanEdit()).DoNotCallBase();
            command.CanEdit().Returns(true);
            command.GetBaseLayoutItems(Arg.Any<Item>()).Returns((List<Item>)null);

            // Act
            var result = command.QueryState(context);

            // Assert
            Assert.Equal(CommandState.Hidden, result);
        }

        [Fact]
        public void QueryState_WhenGetBaseLayoutsReturnsEmpty_ReturnsHidden()
        {
            // Arrange
            var context = new CommandContext(MasterFakesFactory.CreateFakeItem());
            var command = Substitute.ForPartsOf<SelectBaseLayout>();
            command.When(c => c.CanEdit()).DoNotCallBase();
            command.CanEdit().Returns(true);
            command.GetBaseLayoutItems(Arg.Any<Item>()).Returns(new List<Item>());

            // Act
            var result = command.QueryState(context);

            // Assert
            Assert.Equal(CommandState.Hidden, result);
        }

        [Fact]
        public void QueryState_WhenCanDesignReturnsFalse_ReturnsDisabled()
        {
            // Arrange
            var context = new CommandContext(MasterFakesFactory.CreateFakeItem());
            var command = Substitute.ForPartsOf<SelectBaseLayout>();
            command.When(c => c.CanEdit()).DoNotCallBase();
            command.CanEdit().Returns(true);
            command.GetBaseLayoutItems(Arg.Any<Item>()).Returns(CreateBaseLayoutItems());
            command.When(c => c.CanDesign(Arg.Any<Item>())).DoNotCallBase();
            command.CanDesign(Arg.Any<Item>()).Returns(false);

            // Act
            var result = command.QueryState(context);

            // Assert
            Assert.Equal(CommandState.Disabled, result);
        }

        [Fact]
        public void QueryState_WhenThingsAreNormal_ReturnsEnabled()
        {
            // Arrange
            var context = new CommandContext(MasterFakesFactory.CreateFakeItem());
            var command = Substitute.ForPartsOf<SelectBaseLayout>();
            command.When(c => c.CanEdit()).DoNotCallBase();
            command.CanEdit().Returns(true);
            command.GetBaseLayoutItems(Arg.Any<Item>()).Returns(CreateBaseLayoutItems());
            command.When(c => c.CanDesign(Arg.Any<Item>())).DoNotCallBase();
            command.CanDesign(Arg.Any<Item>()).Returns(true);

            // Act
            var result = command.QueryState(context);

            // Assert
            Assert.Equal(CommandState.Enabled, result);
        }

        private List<Item> CreateBaseLayoutItems()
        {
            return Enumerable.Range(0, 4).Select(x => MasterFakesFactory.CreateFakeItem()).ToList();
        }
    }
}