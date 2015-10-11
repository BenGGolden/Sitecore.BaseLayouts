using NSubstitute;
using Sitecore.BaseLayouts.Data;
using Sitecore.BaseLayouts.Pipelines.SaveBaseLayout;
using Sitecore.Data.Items;
using Xunit;

namespace Sitecore.BaseLayouts.Tests.Pipelines.SaveBaseLayout
{
    public class CheckForCircularReferenceTests : FakeDbTestClass
    {
        [Fact]
        public void Process_WithNewBaseLayoutItemNull_DoesNotCallValidator()
        {
            // Arrange
            var item = MasterFakesFactory.CreateFakeItem();
            var args = new SaveBaseLayoutArgs(item);
            var validator = Substitute.For<IBaseLayoutValidator>();
            var processor = new CheckForCircularReference(validator);

            // Act
            processor.Process(args);

            // Assert
            validator.DidNotReceive().CreatesCircularBaseLayoutReference(Arg.Any<BaseLayoutItem>(), Arg.Any<Item>());
        }

        [Fact]
        public void Process_WhenValidatorReturnsTrue_SetsSuccessfulFalseAndAbortsPipeline()
        {
            // Arrange
            var item = MasterFakesFactory.CreateFakeItem();
            var item2 = MasterFakesFactory.CreateFakeItem();
            var args = new SaveBaseLayoutArgs(item) {NewBaseLayoutItem = item2};
            var validator = Substitute.For<IBaseLayoutValidator>();
            validator.CreatesCircularBaseLayoutReference(Arg.Any<BaseLayoutItem>(), Arg.Any<Item>()).Returns(true);
            var processor = new CheckForCircularReference(validator);

            // Act
            processor.Process(args);

            // Assert
            Assert.False(args.Successful);
            Assert.True(args.Aborted);
        }

        [Fact]
        public void Process_WhenValidatorReturnsFalse_DoesNotAbortPipeline()
        {
            // Arrange
            var item = MasterFakesFactory.CreateFakeItem();
            var item2 = MasterFakesFactory.CreateFakeItem();
            var args = new SaveBaseLayoutArgs(item) {NewBaseLayoutItem = item2};
            var validator = Substitute.For<IBaseLayoutValidator>();
            validator.CreatesCircularBaseLayoutReference(Arg.Any<BaseLayoutItem>(), Arg.Any<Item>()).Returns(false);
            var processor = new CheckForCircularReference(validator);

            // Act
            processor.Process(args);

            // Assert
            Assert.False(args.Aborted);
        }
    }
}