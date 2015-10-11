using NSubstitute;
using Sitecore.Data;
using Sitecore.Text;
using Xunit;

namespace Sitecore.BaseLayouts.Tests
{
    public class SelectBaseLayoutOptionsTests : FakeDbTestClass
    {
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