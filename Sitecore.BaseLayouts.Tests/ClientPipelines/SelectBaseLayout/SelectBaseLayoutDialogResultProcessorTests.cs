using NSubstitute;
using Sitecore.BaseLayouts.ClientPipelines.SelectBaseLayout;
using Sitecore.BaseLayouts.Pipelines;
using Sitecore.BaseLayouts.Pipelines.SaveBaseLayout;
using Sitecore.Data;
using Xunit;

namespace Sitecore.BaseLayouts.Tests.ClientPipelines.SelectBaseLayout
{
    public class SelectBaseLayoutDialogResultProcessorTests : FakeDbTestClass
    {
        [Fact]
        public void ProcessResult_WithInvalidResult_ReturnsFalseAndMessageNotEmpty()
        {
            // Arrange
            var item = MasterFakesFactory.CreateFakeItem();

            var runner = Substitute.For<IPipelineRunner>();
            var processor = new SelectBaseLayoutDialogResultProcessor(runner);

            // Act
            string message;
            var result = processor.ProcessResult(item, "this is not a short id", out message);

            // Assert
            Assert.False(result);
            Assert.False(string.IsNullOrEmpty(message));
        }

        [Fact]
        public void ProcessResult_WithResultEqualToIdOfItemThatDoesNotExist_ReturnsFalseAndMessageNotEmpty()
        {
            // Arrange
            var item = MasterFakesFactory.CreateFakeItem();

            var runner = Substitute.For<IPipelineRunner>();
            var processor = new SelectBaseLayoutDialogResultProcessor(runner);

            // Act
            string message;
            var result = processor.ProcessResult(item, new ID().ToShortID().ToString(), out message);

            // Assert
            Assert.False(result);
            Assert.False(string.IsNullOrEmpty(message));
        }

        [Fact]
        public void ProcessResult_WithValidSelection_RunsSaveBaseLayoutPipeline()
        {
            // Arrange
            var item = MasterFakesFactory.CreateFakeItem();
            var item2 = MasterFakesFactory.CreateFakeItem();

            var runner = Substitute.For<IPipelineRunner>();
            var processor = new SelectBaseLayoutDialogResultProcessor(runner);

            // Act
            string message;
            var result = processor.ProcessResult(item, item2.ID.ToShortID().ToString(), out message);

            // Assert
            runner.Received()
                .Run(Arg.Is<SaveBaseLayoutArgs>(args => args.Item.ID == item.ID && args.NewBaseLayoutItem.ID == item2.ID));
        }

        [Fact]
        public void ProcessResult_WithValidSelectionAndPipelineSetsArgsSuccessfulToTrue_ReturnsTrue()
        {
            // Arrange
            var item = MasterFakesFactory.CreateFakeItem();
            var item2 = MasterFakesFactory.CreateFakeItem();

            var runner = Substitute.For<IPipelineRunner>();
            runner.When(r => r.Run(Arg.Any<SaveBaseLayoutArgs>()))
                .Do(ci => ci.Arg<SaveBaseLayoutArgs>().Successful = true);
            var processor = new SelectBaseLayoutDialogResultProcessor(runner);

            // Act
            string message;
            var result = processor.ProcessResult(item, item2.ID.ToShortID().ToString(), out message);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ProcessResult_WithValidSelectionAndPipelineSetsArgsSuccessfulToFalse_ReturnsFalse()
        {
            // Arrange
            var item = MasterFakesFactory.CreateFakeItem();
            var item2 = MasterFakesFactory.CreateFakeItem();

            var runner = Substitute.For<IPipelineRunner>();
            runner.When(r => r.Run(Arg.Any<SaveBaseLayoutArgs>()))
                .Do(ci => ci.Arg<SaveBaseLayoutArgs>().Successful = false);
            var processor = new SelectBaseLayoutDialogResultProcessor(runner);

            // Act
            string message;
            var result = processor.ProcessResult(item, item2.ID.ToShortID().ToString(), out message);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ProcessResult_WithValidSelectionAndPipelineArgsHasMessage_SetsOutMessage()
        {
            // Arrange
            var item = MasterFakesFactory.CreateFakeItem();
            var item2 = MasterFakesFactory.CreateFakeItem();

            var expectedMessage = "This is the expected error message!";
            var runner = Substitute.For<IPipelineRunner>();
            runner.When(r => r.Run(Arg.Any<SaveBaseLayoutArgs>()))
                .Do(ci => ci.Arg<SaveBaseLayoutArgs>().AddMessage(expectedMessage));
            var processor = new SelectBaseLayoutDialogResultProcessor(runner);

            // Act
            string message;
            var result = processor.ProcessResult(item, item2.ID.ToShortID().ToString(), out message);

            // Assert
            Assert.True(message.Contains(expectedMessage));
        }
    }
}