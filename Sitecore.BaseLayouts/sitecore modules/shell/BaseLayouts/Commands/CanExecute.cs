using Sitecore.BaseLayouts.Abstractions;
using Sitecore.BaseLayouts.Pipelines;
using Sitecore.Diagnostics;
using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using Sitecore.ExperienceEditor.Speak.Server.Requests;
using Sitecore.ExperienceEditor.Speak.Server.Responses;

namespace Sitecore.BaseLayouts.Commands
{
    /// <summary>
    ///     Speak server request to determine if the base layouts dialog can execute on the current item.
    /// </summary>
    public class CanExecute : PipelineProcessorRequest<ItemContext>
    {
        private readonly ICommandContextChecker _contextChecker;

        public CanExecute() : this(new SelectBaseLayoutContextChecker(new PageModeAccess(), new PipelineRunner()))
        {
        }

        public CanExecute(ICommandContextChecker contextChecker)
        {
            Assert.ArgumentNotNull(contextChecker, "commandContextChecker");
            _contextChecker = contextChecker;
        }

        /// <summary>
        ///     Process the request to determine if the base layouts dialog can execute on the current item.
        /// </summary>
        /// <returns>a value indicating if base layouts are supported for the current item</returns>
        public override PipelineProcessorResponseValue ProcessRequest()
        {
            return new PipelineProcessorResponseValue {Value = _contextChecker.CanExecute(RequestContext.Item)};
        }
    }
}