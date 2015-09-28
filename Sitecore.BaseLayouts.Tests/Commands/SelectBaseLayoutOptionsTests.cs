using System.Collections.Specialized;
using NSubstitute;
using Sitecore.BaseLayouts.Commands;
using Sitecore.Data;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Text;
using Xunit;

namespace Sitecore.BaseLayouts.Tests.Commands
{
    public class SelectBaseLayoutOptionsTests : FakeDbTestClass
    {
        [Fact]
        public void Execute_WithNoItemsInContext_DoesNotRunPipeline()
        {
            // Arrange
            var context = new CommandContext();
            var command = Substitute.ForPartsOf<SelectBaseLayout>();
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
            var context = new CommandContext(new[] {item, item2});
            var command = Substitute.ForPartsOf<SelectBaseLayout>();
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
            var command = Substitute.ForPartsOf<SelectBaseLayout>();
            command.When(c => c.RunClientPipeline(Arg.Any<NameValueCollection>())).DoNotCallBase();

            // Act
            command.Execute(context);

            // Assert
            command.Received().RunClientPipeline(Arg.Is<NameValueCollection>(nvc => ItemUri.IsItemUri(nvc["items"])));
        }

        [Fact]
        public void AddCurrentBaseLayout_WithCurrentBaseLayoutIdNull_DoesNotAddQueryStringParameter()
        {
            // Arrange
            var urlString = new UrlString();
            var options = new SelectBaseLayoutOptions();

            // Act
            var result = options.AddCurrentBaseLayout(urlString);

            // Assert
            Assert.Null(result.Parameters[SelectBaseLayoutOptions.CurrentBaseLayoutQueryKey]);
        }

        [Fact]
        public void AddCurrentBaseLayout_WithCurrentBaseLayoutIdNotNull_AddsQueryStringParameter()
        {
            // Arrange
            var urlString = new UrlString();
            var id = new ID();
            var options = new SelectBaseLayoutOptions {CurrentBaseLayoutId = id};

            // Act
            var result = options.AddCurrentBaseLayout(urlString);

            // Assert
            Assert.Equal(id.ToShortID().ToString(), result.Parameters[SelectBaseLayoutOptions.CurrentBaseLayoutQueryKey]);
        }

        [Fact]
        public void ParseCurrentBaseLayout_WithValidQueryString_SetsCurrentBaseLayoutId()
        {
            // Arrange
            var id = new ID();
            var options = Substitute.ForPartsOf<SelectBaseLayoutOptions>();
            options.When(o => o.GetQueryString(Arg.Any<string>())).DoNotCallBase();
            options.GetQueryString(SelectBaseLayoutOptions.CurrentBaseLayoutQueryKey).Returns(id.ToShortID().ToString());

            // Act
            options.ParseCurrentBaseLayout();

            // Assert
            Assert.Equal(id, options.CurrentBaseLayoutId);
        }

        [Fact]
        public void ParseCurrentBaseLayout_WithoutQueryString_DoesNotSetCurrentBaseLayoutId()
        {
            // Arrange
            var id = new ID();
            var options = Substitute.ForPartsOf<SelectBaseLayoutOptions>();
            options.When(o => o.GetQueryString(Arg.Any<string>())).DoNotCallBase();
            options.GetQueryString(SelectBaseLayoutOptions.CurrentBaseLayoutQueryKey).Returns((string)null);

            // Act
            options.ParseCurrentBaseLayout();

            // Assert
            Assert.Null(options.CurrentBaseLayoutId);
        }

        [Fact]
        public void ParseCurrentBaseLayout_WithInvalidQueryString_DoesNotSetCurrentBaseLayoutId()
        {
            // Arrange
            var id = new ID();
            var options = Substitute.ForPartsOf<SelectBaseLayoutOptions>();
            options.When(o => o.GetQueryString(Arg.Any<string>())).DoNotCallBase();
            options.GetQueryString(SelectBaseLayoutOptions.CurrentBaseLayoutQueryKey).Returns("this is not a short ID");

            // Act
            options.ParseCurrentBaseLayout();

            // Assert
            Assert.Null(options.CurrentBaseLayoutId);
        }
    }
}