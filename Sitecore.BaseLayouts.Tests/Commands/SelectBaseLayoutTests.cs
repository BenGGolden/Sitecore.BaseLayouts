using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using NSubstitute;
using Sitecore.BaseLayouts.Abstractions;
using Sitecore.BaseLayouts.Commands;
using Sitecore.BaseLayouts.Pipelines;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using Xunit;

namespace Sitecore.BaseLayouts.Tests.Commands
{
    public class SelectBaseLayoutTests : FakeDbTestClass
    {
        #region QueryState tests
        
        [Fact]
        public void QueryState_WithNoItemInContext_ReturnsDisabled()
        {
            // Arrange
            var sheer = Substitute.For<ISheerResponse>();
            var runner = Substitute.For<IPipelineRunner>();
            var contextChecker = Substitute.For<ICommandContextChecker>();
            var command = Substitute.ForPartsOf<SelectBaseLayout>(sheer, runner, contextChecker);
            command.GetBaseLayoutItems(Arg.Any<Item>()).Returns(CreateBaseLayoutItems());
            var context = new CommandContext();

            // Act
            var result = command.QueryState(context);

            // Assert
            Assert.Equal(CommandState.Disabled, result);
        }

        [Fact]
        public void QueryState_WhenContextCheckerReturnsFals_ReturnsDisabled()
        {
            // Arrange
            var sheer = Substitute.For<ISheerResponse>();
            var runner = Substitute.For<IPipelineRunner>();
            var contextChecker = Substitute.For<ICommandContextChecker>();
            contextChecker.CanExecute(Arg.Any<Item>()).Returns(false);
            var command = Substitute.ForPartsOf<SelectBaseLayout>(sheer, runner, contextChecker);
            var context = new CommandContext(MasterFakesFactory.CreateFakeItem(null, null, null, null, false));

            // Act
            var result = command.QueryState(context);

            // Assert
            Assert.Equal(CommandState.Disabled, result);
        }

        [Fact]
        public void QueryState_WhenContextCheckerReturnsTrue_ReturnsEnabled()
        {
            // Arrange
            var sheer = Substitute.For<ISheerResponse>();
            var runner = Substitute.For<IPipelineRunner>();
            var contextChecker = Substitute.For<ICommandContextChecker>();
            contextChecker.CanExecute(Arg.Any<Item>()).Returns(true);
            var command = Substitute.ForPartsOf<SelectBaseLayout>(sheer, runner, contextChecker);
            var context = new CommandContext(MasterFakesFactory.CreateFakeItem());

            // Act
            var result = command.QueryState(context);

            // Assert
            Assert.Equal(CommandState.Enabled, result);
        }

        #endregion

        #region Run tests

        [Fact]
        public void Run_WhenCheckModifiedReturnsFalse_DoesNotDoAnything()
        {
            // Arrange
            var args = Substitute.For<ClientPipelineArgs>();

            var sheer = Substitute.For<ISheerResponse>();
            sheer.CheckModified().Returns(false);
            var runner = Substitute.For<IPipelineRunner>();
            var contextChecker = Substitute.For<ICommandContextChecker>();
            var command = Substitute.ForPartsOf<SelectBaseLayout>(sheer, runner, contextChecker);
            command.When(c => c.ProcessResult(Arg.Any<BaseLayoutItem>(), Arg.Any<string>())).DoNotCallBase();

            // Act
            command.Run(args);

            // Assert
            sheer.DidNotReceive().Alert(Arg.Any<string>());
            sheer.DidNotReceive().ShowModalDialog(Arg.Any<SelectBaseLayoutOptions>());
            args.DidNotReceive().WaitForPostBack();
            command.DidNotReceive().ProcessResult(Arg.Any<BaseLayoutItem>(), Arg.Any<string>());
        }

        [Fact]
        public void Run_WithItemWithoutBaseLayoutField_CallsAlertButNotShowModalDialogOrProcessResult()
        {
            // Arrange
            var item = MasterFakesFactory.CreateFakeItem(null, null, null, null, false);
            var args = Substitute.For<ClientPipelineArgs>();
            args.Parameters = new NameValueCollection {{"items", item.Uri.ToString()}};

            var sheer = Substitute.For<ISheerResponse>();
            sheer.CheckModified().Returns(true);
            var runner = Substitute.For<IPipelineRunner>();
            var contextChecker = Substitute.For<ICommandContextChecker>();
            var command = Substitute.ForPartsOf<SelectBaseLayout>(sheer, runner, contextChecker);
            command.When(c => c.ProcessResult(Arg.Any<BaseLayoutItem>(), Arg.Any<string>())).DoNotCallBase();
            
            // Act
            command.Run(args);

            // Assert
            sheer.Received().Alert(Arg.Any<string>());
            sheer.DidNotReceive().ShowModalDialog(Arg.Any<SelectBaseLayoutOptions>());
            args.DidNotReceive().WaitForPostBack();
            command.DidNotReceive().ProcessResult(Arg.Any<BaseLayoutItem>(), Arg.Any<string>());
        }

        [Fact]
        public void Run_WithIsPostBackFalseAndGetBaseLayoutItemsReturnsNull_CallsAlertButNotShowModalDialogOrProcessResult()
        {
            // Arrange
            var item = MasterFakesFactory.CreateFakeItem();
            var args = Substitute.For<ClientPipelineArgs>();
            args.Parameters = new NameValueCollection { { "items", item.Uri.ToString() } };
            args.IsPostBack = false;

            var sheer = Substitute.For<ISheerResponse>();
            sheer.CheckModified().Returns(true);
            var runner = Substitute.For<IPipelineRunner>();
            var contextChecker = Substitute.For<ICommandContextChecker>();
            var command = Substitute.ForPartsOf<SelectBaseLayout>(sheer, runner, contextChecker);
            command.When(c => c.ProcessResult(Arg.Any<BaseLayoutItem>(), Arg.Any<string>())).DoNotCallBase();
            command.When(c => c.GetBaseLayoutItems(Arg.Any<Item>())).DoNotCallBase();
            command.GetBaseLayoutItems(Arg.Any<Item>()).Returns((List<Item>)null);
            
            // Act
            command.Run(args);

            // Assert
            sheer.Received().Alert(Arg.Any<string>());
            sheer.DidNotReceive().ShowModalDialog(Arg.Any<SelectBaseLayoutOptions>());
            args.DidNotReceive().WaitForPostBack();
            command.DidNotReceive().ProcessResult(Arg.Any<BaseLayoutItem>(), Arg.Any<string>());
        }

        [Fact]
        public void Run_WithIsPostBackFalseAndGetBaseLayoutItemsReturnsEmpty_CallsAlertButNotShowModalDialogOrProcessResult()
        {
            // Arrange
            var item = MasterFakesFactory.CreateFakeItem();
            var args = Substitute.For<ClientPipelineArgs>();
            args.Parameters = new NameValueCollection { { "items", item.Uri.ToString() } };
            args.IsPostBack = false;

            var sheer = Substitute.For<ISheerResponse>();
            sheer.CheckModified().Returns(true);
            var runner = Substitute.For<IPipelineRunner>();
            var contextChecker = Substitute.For<ICommandContextChecker>();
            var command = Substitute.ForPartsOf<SelectBaseLayout>(sheer, runner, contextChecker);
            command.When(c => c.ProcessResult(Arg.Any<BaseLayoutItem>(), Arg.Any<string>())).DoNotCallBase();
            command.When(c => c.GetBaseLayoutItems(Arg.Any<Item>())).DoNotCallBase();
            command.GetBaseLayoutItems(Arg.Any<Item>()).Returns(new List<Item>());
            
            // Act
            command.Run(args);

            // Assert
            sheer.Received().Alert(Arg.Any<string>());
            sheer.DidNotReceive().ShowModalDialog(Arg.Any<SelectBaseLayoutOptions>());
            args.DidNotReceive().WaitForPostBack();
            command.DidNotReceive().ProcessResult(Arg.Any<BaseLayoutItem>(), Arg.Any<string>());
        }

        [Fact]
        public void Run_WithItemWithBaseLayoutAndIsPostbackFalse_CallsShoModalDialogWithCurrentBaseLayoutIdEqualToBaseLayoutId()
        {
            // Arrange
            var item = MasterFakesFactory.CreateFakeItem();
            var item2 = MasterFakesFactory.CreateFakeItem(null, null, null, item.ID);
            var args = Substitute.For<ClientPipelineArgs>();
            args.Parameters = new NameValueCollection {{"items", item2.Uri.ToString()}};
            args.IsPostBack = false;

            var sheer = Substitute.For<ISheerResponse>();
            sheer.CheckModified().Returns(true);
            var runner = Substitute.For<IPipelineRunner>();
            var contextChecker = Substitute.For<ICommandContextChecker>();
            var command = Substitute.ForPartsOf<SelectBaseLayout>(sheer, runner, contextChecker);
            command.When(c => c.ProcessResult(Arg.Any<BaseLayoutItem>(), Arg.Any<string>())).DoNotCallBase();
            command.When(c => c.GetBaseLayoutItems(Arg.Any<Item>())).DoNotCallBase();
            command.GetBaseLayoutItems(Arg.Any<Item>()).Returns(new List<Item> {item});
            
            // Act
            command.Run(args);

            // Assert
            sheer.DidNotReceive().Alert(Arg.Any<string>());
            sheer.Received().ShowModalDialog(Arg.Is<SelectBaseLayoutOptions>(o => o.CurrentBaseLayoutId == item.ID));
            args.Received().WaitForPostBack();
            command.DidNotReceive().ProcessResult(Arg.Any<BaseLayoutItem>(), Arg.Any<string>());
        }

        [Fact]
        public void Run_WithIsPostBackTrueAndHasResultTrue_CallsProcessResult()
        {
            // Arrange
            var result = new ID().ToString();
            var item = MasterFakesFactory.CreateFakeItem();
            var args = Substitute.For<ClientPipelineArgs>();
            args.Parameters = new NameValueCollection { { "items", item.Uri.ToString() } };
            args.IsPostBack = true;
            args.Result = result;

            var sheer = Substitute.For<ISheerResponse>();
            sheer.CheckModified().Returns(true);
            var runner = Substitute.For<IPipelineRunner>();
            var contextChecker = Substitute.For<ICommandContextChecker>();
            var command = Substitute.ForPartsOf<SelectBaseLayout>(sheer, runner, contextChecker);
            command.When(c => c.ProcessResult(Arg.Any<BaseLayoutItem>(), Arg.Any<string>())).DoNotCallBase();

            // Act
            command.Run(args);

            // Assert
            command.Received().ProcessResult(Arg.Is<BaseLayoutItem>(i => i.ID == item.ID), result);
            sheer.DidNotReceive().ShowModalDialog(Arg.Any<SelectBaseLayoutOptions>());
            args.DidNotReceive().WaitForPostBack();
            sheer.DidNotReceive().Alert(Arg.Any<string>());
        }

        #endregion

        #region ProcessResult tests

        [Fact]
        public void ProcessResult_WithInvalidResult_CallsAlertAndDoesNotCallSaveBaseLayout()
        {
            // Arrange
            string message;
            var item = MasterFakesFactory.CreateFakeItem();

            var sheer = Substitute.For<ISheerResponse>();
            var runner = Substitute.For<IPipelineRunner>();
            var contextChecker = Substitute.For<ICommandContextChecker>();
            var command = Substitute.ForPartsOf<SelectBaseLayout>(sheer, runner, contextChecker);
            command.When(c => c.SaveBaseLayout(Arg.Any<BaseLayoutItem>(), Arg.Any<Item>(), out message)).DoNotCallBase();

            // Act
            command.ProcessResult(item, "this is not a short id");

            // Assert
            sheer.Received().Alert(Arg.Any<string>());
            command.DidNotReceive().SaveBaseLayout(Arg.Any<BaseLayoutItem>(), Arg.Any<Item>(), out message);
        }

        [Fact]
        public void ProcessResult_WithResultEqualToIdOfItemThatDoesNotExist_CallsAlertAndDoesNotCallSaveBaseLayout()
        {
            // Arrange
            string message;
            var item = MasterFakesFactory.CreateFakeItem();

            var sheer = Substitute.For<ISheerResponse>();
            var runner = Substitute.For<IPipelineRunner>();
            var contextChecker = Substitute.For<ICommandContextChecker>();
            var command = Substitute.ForPartsOf<SelectBaseLayout>(sheer, runner, contextChecker);
            command.When(c => c.SaveBaseLayout(Arg.Any<BaseLayoutItem>(), Arg.Any<Item>(), out message)).DoNotCallBase();

            // Act
            command.ProcessResult(item, new ID().ToShortID().ToString());

            // Assert
            sheer.Received().Alert(Arg.Any<string>());
            command.DidNotReceive().SaveBaseLayout(Arg.Any<BaseLayoutItem>(), Arg.Any<Item>(), out message);
        }

        [Fact]
        public void ProcessResult_WithValidSelection_CallsSaveBaseLayout()
        {
            // Arrange
            string message;
            var item = MasterFakesFactory.CreateFakeItem();
            var item2 = MasterFakesFactory.CreateFakeItem();

            var sheer = Substitute.For<ISheerResponse>();
            var runner = Substitute.For<IPipelineRunner>();
            var contextChecker = Substitute.For<ICommandContextChecker>();
            var command = Substitute.ForPartsOf<SelectBaseLayout>(sheer, runner, contextChecker);
            command.When(c => c.SaveBaseLayout(Arg.Any<BaseLayoutItem>(), Arg.Any<Item>(), out message)).DoNotCallBase();

            // Act
            command.ProcessResult(item, item2.ID.ToShortID().ToString());

            // Assert
            command.Received().SaveBaseLayout(Arg.Any<BaseLayoutItem>(), Arg.Any<Item>(), out message);
        }

        [Fact]
        public void ProcessResult_WithValidSelectionAndWhenSaveBaseLayoutReturnsTrue_CallsRefresh()
        {
            // Arrange
            string message;
            var item = MasterFakesFactory.CreateFakeItem();
            var item2 = MasterFakesFactory.CreateFakeItem();

            var sheer = Substitute.For<ISheerResponse>();
            var runner = Substitute.For<IPipelineRunner>();
            var contextChecker = Substitute.For<ICommandContextChecker>();
            var command = Substitute.ForPartsOf<SelectBaseLayout>(sheer, runner, contextChecker);
            command.When(c => c.SaveBaseLayout(Arg.Any<BaseLayoutItem>(), Arg.Any<Item>(), out message)).DoNotCallBase();
            command.SaveBaseLayout(Arg.Any<BaseLayoutItem>(), Arg.Any<Item>(), out message).Returns(true);
            command.When(c => c.Refresh()).DoNotCallBase();

            // Act
            command.ProcessResult(item, item2.ID.ToShortID().ToString());

            // Assert
            command.Received().Refresh();
        }

        [Fact]
        public void ProcessResult_WithValidSelectionAndWhenSaveBaseLayoutReturnsFalse_CallsAlert()
        {
            // Arrange
            string message;
            var item = MasterFakesFactory.CreateFakeItem();
            var item2 = MasterFakesFactory.CreateFakeItem();

            var sheer = Substitute.For<ISheerResponse>();
            var runner = Substitute.For<IPipelineRunner>();
            var contextChecker = Substitute.For<ICommandContextChecker>();
            var command = Substitute.ForPartsOf<SelectBaseLayout>(sheer, runner, contextChecker);
            command.When(c => c.SaveBaseLayout(Arg.Any<BaseLayoutItem>(), Arg.Any<Item>(), out message)).DoNotCallBase();
            command.SaveBaseLayout(Arg.Any<BaseLayoutItem>(), Arg.Any<Item>(), out message).Returns(false);
            command.When(c => c.Refresh()).DoNotCallBase();

            // Act
            command.ProcessResult(item, item2.ID.ToShortID().ToString());

            // Assert
            sheer.Received().Alert(Arg.Any<string>());
        } 
        #endregion

        private List<Item> CreateBaseLayoutItems()
        {
            return Enumerable.Range(0, 4).Select(x => MasterFakesFactory.CreateFakeItem()).ToList();
        }
    }
}