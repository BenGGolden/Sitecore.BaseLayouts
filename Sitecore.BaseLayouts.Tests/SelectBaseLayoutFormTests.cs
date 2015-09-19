using System;
using System.Linq;
using System.Web.UI;
using NSubstitute;
using Sitecore.Data;
using Sitecore.Data.Items;
using Xunit;

namespace Sitecore.BaseLayouts.Tests
{
    public class SelectBaseLayoutFormTests : FakeDbTestClass
    {
        [Fact]
        public void RenderPreviews_WithNullItemsList_ThrowsArgumentNullException()
        {
            // Arrange
            var form = new SelectBaseLayoutForm();

            // Act => Assert
            Assert.Throws<ArgumentNullException>(() => form.RenderPreviews(null, new ID()));
        }

        [Fact]
        public void RenderPreviews_WithItemsAndNullSelectedItemId_RendersItemPreviews()
        {
            // Arrange
            var form = Substitute.ForPartsOf<SelectBaseLayoutForm>();
            form.When(x => x.RenderItemPreview(Arg.Any<Item>(), Arg.Any<HtmlTextWriter>())).DoNotCallBase();
            var items = Enumerable.Range(0, 4).Select(x => MasterFakesFactory.CreateFakeItem());

            // Act
            var result = form.RenderPreviews(items, null);

            // Assert
            form.Received(4).RenderItemPreview(Arg.Any<Item>(), Arg.Any<HtmlTextWriter>());
        }

        [Fact]
        public void RenderPreviews_WithItemsAndSelectedItemId_RendersItemPreviewsWithOneSelected()
        {
            // Arrange
            var form = Substitute.ForPartsOf<SelectBaseLayoutForm>();
            form.When(x => x.RenderItemPreview(Arg.Any<Item>(), Arg.Any<HtmlTextWriter>())).DoNotCallBase();
            form.When(x => x.RenderSelectedItemPreview(Arg.Any<Item>(), Arg.Any<HtmlTextWriter>())).DoNotCallBase();
            var items = Enumerable.Range(0, 4).Select(x => MasterFakesFactory.CreateFakeItem()).ToList();
            var selectedItemId = items.ElementAt(2).ID;

            // Act
            var result = form.RenderPreviews(items, selectedItemId);

            // Assert
            Received.InOrder(() =>
            {
                form.RenderItemPreview(Arg.Any<Item>(), Arg.Any<HtmlTextWriter>());
                form.RenderItemPreview(Arg.Any<Item>(), Arg.Any<HtmlTextWriter>());
                form.RenderSelectedItemPreview(Arg.Any<Item>(), Arg.Any<HtmlTextWriter>());
                form.RenderItemPreview(Arg.Any<Item>(), Arg.Any<HtmlTextWriter>());
            });
        }
    }
}