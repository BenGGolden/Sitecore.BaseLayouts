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
        #region Execute Tests

        [Fact]
        public void Execute_WithNoItemsInContext_DoesNotRunPipeline()
        {
            // Arrange
            var context = new CommandContext();
            var sheer = Substitute.For<ISheerResponse>();
            var contextChecker = Substitute.For<ICommandContextChecker>();
            var locator = Substitute.For<IDialogLocator>();
            var processor = Substitute.For<IDialogResultProcessor>();
            var command = Substitute.ForPartsOf<SelectBaseLayout>(sheer, contextChecker, locator, processor);
            command.When(c => c.RunClientPipeline(Arg.Any<NameValueCollection>())).DoNotCallBase();

            // Act
            command.Execute(context);

            // Assert
            command.DidNotReceive().RunClientPipeline(Arg.Any<NameValueCollection>());
        }

        [Fact]
        public void Execute_WithMoreThanOneItemInContext_DoesNotRunPipeline()
        {
            // Arrange
            var item = MasterFakesFactory.CreateFakeItem();
            var item2 = MasterFakesFactory.CreateFakeItem();
            var context = new CommandContext(new[] { item, item2 });
            var sheer = Substitute.For<ISheerResponse>();
            var contextChecker = Substitute.For<ICommandContextChecker>();
            var locator = Substitute.For<IDialogLocator>();
            var processor = Substitute.For<IDialogResultProcessor>();
            var command = Substitute.ForPartsOf<SelectBaseLayout>(sheer, contextChecker, locator, processor);
            command.When(c => c.RunClientPipeline(Arg.Any<NameValueCollection>())).DoNotCallBase();

            // Act
            command.Execute(context);

            // Assert
            command.DidNotReceive().RunClientPipeline(Arg.Any<NameValueCollection>());
        }

        [Fact]
        public void Execute_WithOneItemInContext_RunsPipelineWithCorrectParameters()
        {
            // Arrange
            var item = MasterFakesFactory.CreateFakeItem();
            var context = new CommandContext(item);
            var sheer = Substitute.For<ISheerResponse>();
            var contextChecker = Substitute.For<ICommandContextChecker>();
            var locator = Substitute.For<IDialogLocator>();
            var processor = Substitute.For<IDialogResultProcessor>();
            var command = Substitute.ForPartsOf<SelectBaseLayout>(sheer, contextChecker, locator, processor);
            command.When(c => c.RunClientPipeline(Arg.Any<NameValueCollection>())).DoNotCallBase();

            // Act
            command.Execute(context);

            // Assert
            command.Received().RunClientPipeline(Arg.Is<NameValueCollection>(nvc => ItemUri.IsItemUri(nvc["items"])));
        }

        #endregion

        #region QueryState tests

        [Fact]
        public void QueryState_WithNoItemInContext_ReturnsDisabled()
        {
            // Arrange
            var sheer = Substitute.For<ISheerResponse>();
            var contextChecker = Substitute.For<ICommandContextChecker>();
            var locator = Substitute.For<IDialogLocator>();
            var processor = Substitute.For<IDialogResultProcessor>();
            var command = new SelectBaseLayout(sheer, contextChecker, locator, processor);

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
            var contextChecker = Substitute.For<ICommandContextChecker>();
            contextChecker.CanExecute(Arg.Any<Item>()).Returns(false);
            var locator = Substitute.For<IDialogLocator>();
            var processor = Substitute.For<IDialogResultProcessor>();
            var command = new SelectBaseLayout(sheer, contextChecker, locator, processor);

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
            var contextChecker = Substitute.For<ICommandContextChecker>();
            contextChecker.CanExecute(Arg.Any<Item>()).Returns(true);
            var locator = Substitute.For<IDialogLocator>();
            var processor = Substitute.For<IDialogResultProcessor>();
            var command = new SelectBaseLayout(sheer, contextChecker, locator, processor);

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
            var contextChecker = Substitute.For<ICommandContextChecker>();
            var locator = Substitute.For<IDialogLocator>();
            var processor = Substitute.For<IDialogResultProcessor>();
            var command = new SelectBaseLayout(sheer, contextChecker, locator, processor);

            // Act
            command.Run(args);

            // Assert
            sheer.DidNotReceive().Alert(Arg.Any<string>());
            sheer.DidNotReceive().ShowModalDialog(Arg.Any<string>());
            locator.DidNotReceive().GetDialogUrl(Arg.Any<Item>());
            string message;
            processor.DidNotReceive().ProcessResult(Arg.Any<Item>(), Arg.Any<string>(), out message);
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
            var contextChecker = Substitute.For<ICommandContextChecker>();
            var locator = Substitute.For<IDialogLocator>();
            var processor = Substitute.For<IDialogResultProcessor>();
            var command = new SelectBaseLayout(sheer, contextChecker, locator, processor);

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
            var contextChecker = Substitute.For<ICommandContextChecker>();
            var locator = Substitute.For<IDialogLocator>();
            locator.GetDialogUrl(Arg.Any<Item>()).Returns((string) null);
            var processor = Substitute.For<IDialogResultProcessor>();
            var command = new SelectBaseLayout(sheer, contextChecker, locator, processor);

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
            var contextChecker = Substitute.For<ICommandContextChecker>();
            var url = "/This/is/my/dialog/url";
            var locator = Substitute.For<IDialogLocator>();
            locator.GetDialogUrl(Arg.Any<Item>()).Returns(url);
            var processor = Substitute.For<IDialogResultProcessor>();
            var command = new SelectBaseLayout(sheer, contextChecker, locator, processor);

            // Act
            command.Run(args);

            // Assert
            sheer.Received().ShowModalDialog(url);
            args.Received().WaitForPostBack();
        }

        [Fact]
        public void Run_WithIsPostBackTrueAndHasResultTrue_CallsDialogResultProcessor()
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
            var contextChecker = Substitute.For<ICommandContextChecker>();
            var locator = Substitute.For<IDialogLocator>();
            var processor = Substitute.For<IDialogResultProcessor>();
            var command = new SelectBaseLayout(sheer, contextChecker, locator, processor);

            // Act
            command.Run(args);

            // Assert
            string message;
            processor.Received().ProcessResult(Arg.Is<Item>(i => i.ID == item.ID), result, out message);
        }

        [Fact]
        public void Run_WithIsPostBackTrueAndHasResultTrueAndResultProcessorReturnsTrue_CallsRefresh()
        {
            // Arrange
            var result = new ID().ToString();
            var item = MasterFakesFactory.CreateFakeItem();
            var args = Substitute.For<ClientPipelineArgs>();
            args.Parameters = new NameValueCollection { { "items", item.Uri.ToString() } };
            args.IsPostBack = true;
            args.Result = result;

            string message;
            var sheer = Substitute.For<ISheerResponse>();
            sheer.CheckModified().Returns(true);
            var contextChecker = Substitute.For<ICommandContextChecker>();
            var locator = Substitute.For<IDialogLocator>();
            var processor = Substitute.For<IDialogResultProcessor>();
            processor.ProcessResult(Arg.Any<Item>(), Arg.Any<string>(), out message).Returns(true);
            var command = Substitute.ForPartsOf<SelectBaseLayout>(sheer, contextChecker, locator, processor);
            command.When(c => c.Refresh()).DoNotCallBase();

            // Act
            command.Run(args);

            // Assert
            command.Received().Refresh();
        }

        [Fact]
        public void Run_WithIsPostBackTrueAndHasResultTrueAndResultProcessorReturnsFalse_CallsAlertWithMessage()
        {
            // Arrange
            var result = new ID().ToString();
            var item = MasterFakesFactory.CreateFakeItem();
            var args = Substitute.For<ClientPipelineArgs>();
            args.Parameters = new NameValueCollection { { "items", item.Uri.ToString() } };
            args.IsPostBack = true;
            args.Result = result;

            string message;
            var expectedAlert = "This is the expected alert message!";
            var sheer = Substitute.For<ISheerResponse>();
            sheer.CheckModified().Returns(true);
            var contextChecker = Substitute.For<ICommandContextChecker>();
            var locator = Substitute.For<IDialogLocator>();
            var processor = Substitute.For<IDialogResultProcessor>();
            processor.ProcessResult(Arg.Any<Item>(), Arg.Any<string>(), out message).Returns(false).AndDoes(ci => ci[2] = expectedAlert);
            var command = new SelectBaseLayout(sheer, contextChecker, locator, processor);

            // Act
            command.Run(args);

            // Assert
            sheer.Received().Alert(expectedAlert);
        }

        #endregion
    }
}