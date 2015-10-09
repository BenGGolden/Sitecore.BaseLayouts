using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using NSubstitute;
using Sitecore.BaseLayouts.Abstractions;
using Sitecore.BaseLayouts.ClientPipelines.SelectBaseLayout;
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
            var locator = Substitute.For<IDialogLocator>();
            var command = new SelectBaseLayout(sheer, runner, contextChecker, locator);

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
            var locator = Substitute.For<IDialogLocator>();
            var command = new SelectBaseLayout(sheer, runner, contextChecker, locator);

            var context = new CommandContext(MasterFakesFactory.CreateFakeItem());

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
            var locator = Substitute.For<IDialogLocator>();
            var command = new SelectBaseLayout(sheer, runner, contextChecker, locator);

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
            var locator = Substitute.For<IDialogLocator>();
            var command = Substitute.ForPartsOf<SelectBaseLayout>(sheer, runner, contextChecker, locator);
            command.When(c => c.ProcessResult(Arg.Any<BaseLayoutItem>(), Arg.Any<string>())).DoNotCallBase();

            // Act
            command.Run(args);

            // Assert
            sheer.DidNotReceive().Alert(Arg.Any<string>());
            sheer.DidNotReceive().ShowModalDialog(Arg.Any<string>());
            args.DidNotReceive().WaitForPostBack();
            command.DidNotReceive().ProcessResult(Arg.Any<BaseLayoutItem>(), Arg.Any<string>());
        }

        [Fact]
        public void Run_WithIsPostBackFalse_CallsDialogLocatorWithItemFromPipelineArgs()
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
            var locator = Substitute.For<IDialogLocator>();
            var command = Substitute.ForPartsOf<SelectBaseLayout>(sheer, runner, contextChecker, locator);
            command.When(c => c.ProcessResult(Arg.Any<BaseLayoutItem>(), Arg.Any<string>())).DoNotCallBase();

            // Act
            command.Run(args);

            // Assert
            locator.Received().GetDialogUrl(Arg.Is<Item>(i => i.ID == item.ID));
        }

        [Fact]
        public void Run_WithIsPostBackFalseAndDialogLocatorReturnsNull_CallsAlertButNotShowModalDialog()
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
            var locator = Substitute.For<IDialogLocator>();
            locator.GetDialogUrl(Arg.Any<Item>()).Returns((string) null);
            var command = Substitute.ForPartsOf<SelectBaseLayout>(sheer, runner, contextChecker, locator);
            command.When(c => c.ProcessResult(Arg.Any<BaseLayoutItem>(), Arg.Any<string>())).DoNotCallBase();
            
            // Act
            command.Run(args);

            // Assert
            sheer.Received().Alert(Arg.Any<string>());
            sheer.DidNotReceive().ShowModalDialog(Arg.Any<string>());
        }

        [Fact]
        public void Run_WithIsPostBackFalseAndDialogLocatorReturnsValue_CallsShowModalDialogWithReturnedValueAndCallsWaitForPostBack()
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
            var url = "/This/is/my/dialog/url";
            var locator = Substitute.For<IDialogLocator>();
            locator.GetDialogUrl(Arg.Any<Item>()).Returns(url);
            var command = new SelectBaseLayout(sheer, runner, contextChecker, locator);

            // Act
            command.Run(args);

            // Assert
            sheer.Received().ShowModalDialog(url);
            args.Received().WaitForPostBack();
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
            var locator = Substitute.For<IDialogLocator>();
            var command = Substitute.ForPartsOf<SelectBaseLayout>(sheer, runner, contextChecker, locator);
            command.When(c => c.ProcessResult(Arg.Any<BaseLayoutItem>(), Arg.Any<string>())).DoNotCallBase();

            // Act
            command.Run(args);

            // Assert
            command.Received().ProcessResult(Arg.Is<BaseLayoutItem>(i => i.ID == item.ID), result);
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
            var locator = Substitute.For<IDialogLocator>();
            var command = Substitute.ForPartsOf<SelectBaseLayout>(sheer, runner, contextChecker, locator);
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
            var locator = Substitute.For<IDialogLocator>();
            var command = Substitute.ForPartsOf<SelectBaseLayout>(sheer, runner, contextChecker, locator);
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
            var locator = Substitute.For<IDialogLocator>();
            var command = Substitute.ForPartsOf<SelectBaseLayout>(sheer, runner, contextChecker, locator);
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
            var locator = Substitute.For<IDialogLocator>();
            var command = Substitute.ForPartsOf<SelectBaseLayout>(sheer, runner, contextChecker, locator);
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
            var locator = Substitute.For<IDialogLocator>();
            var command = Substitute.ForPartsOf<SelectBaseLayout>(sheer, runner, contextChecker, locator);
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