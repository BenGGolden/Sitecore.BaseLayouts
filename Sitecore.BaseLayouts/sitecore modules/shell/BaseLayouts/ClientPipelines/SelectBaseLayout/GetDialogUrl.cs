using Sitecore.BaseLayouts.Pipelines;
using Sitecore.Diagnostics;
using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using Sitecore.ExperienceEditor.Speak.Server.Requests;
using Sitecore.ExperienceEditor.Speak.Server.Responses;
using Sitecore.Globalization;

namespace Sitecore.BaseLayouts.ClientPipelines.SelectBaseLayout
{
    /// <summary>
    /// Speak pipeline server request to get the url of the base layouts dialog.
    /// </summary>
    public class GetDialogUrl : PipelineProcessorRequest<ItemContext>
    {
        private readonly IDialogLocator _dialogLocator;

        public GetDialogUrl() : this(new SelectBaseLayoutDialogLocator(new PipelineRunner()))
        {
        }

        public GetDialogUrl(IDialogLocator dialogLocator)
        {
            Assert.ArgumentNotNull(dialogLocator, "dialogLocator");
            _dialogLocator = dialogLocator;
        }

        /// <summary>
        ///     The process request.
        /// </summary>
        /// <returns>
        ///     The <see cref="PipelineProcessorResponseValue" />.
        /// </returns>
        public override PipelineProcessorResponseValue ProcessRequest()
        {
            var url = _dialogLocator.GetDialogUrl(RequestContext.Item);
            if (!string.IsNullOrEmpty(url))
            {
                return new PipelineProcessorResponseValue {Value = url};
            }

            return new PipelineProcessorResponseValue {AbortMessage = Translate.Text("Could not open the dialog.")};
        }
    }
}