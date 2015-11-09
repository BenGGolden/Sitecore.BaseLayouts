using Sitecore.BaseLayouts.Pipelines;
using Sitecore.Diagnostics;
using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using Sitecore.ExperienceEditor.Speak.Server.Requests;
using Sitecore.ExperienceEditor.Speak.Server.Responses;

namespace Sitecore.BaseLayouts.ClientPipelines.SelectBaseLayout
{
    /// <summary>
    ///     The execute.
    /// </summary>
    public class Execute : PipelineProcessorRequest<ValueItemContext>
    {
        private readonly IDialogResultProcessor _dialogResultProcessor;

        public Execute() : this(new SelectBaseLayoutDialogResultProcessor(new PipelineRunner()))
        {
        }

        public Execute(IDialogResultProcessor dialogResultProcessor)
        {
            Assert.ArgumentNotNull(dialogResultProcessor, "dialogResultProcessor");
            _dialogResultProcessor = dialogResultProcessor;
        }

        /// <summary>
        ///     The process request.
        /// </summary>
        /// <returns>
        ///     The <see cref="PipelineProcessorResponseValue" />.
        /// </returns>
        public override PipelineProcessorResponseValue ProcessRequest()
        {
            string message;
            if (_dialogResultProcessor.ProcessResult(RequestContext.Item, RequestContext.Value, out message))
            {
                return new PipelineProcessorResponseValue {Value = true};
            }

            return new PipelineProcessorResponseValue {AbortMessage = message};
        }
    }
}