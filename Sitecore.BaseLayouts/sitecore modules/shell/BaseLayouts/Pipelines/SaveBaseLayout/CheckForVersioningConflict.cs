using Sitecore.BaseLayouts.Data;
using Sitecore.Diagnostics;

namespace Sitecore.BaseLayouts.Pipelines.SaveBaseLayout
{
    public class CheckForVersioningConflict : ISaveBaseLayoutProcessor
    {
        private readonly IBaseLayoutValidator _validator;

        /// <summary>
        /// Creates an instance of the processor
        /// </summary>
        /// <param name="validator">a base layout validator</param>
        public CheckForVersioningConflict(IBaseLayoutValidator validator)
        {
            Assert.ArgumentNotNull(validator, "validator");
            _validator = validator;
        }

        /// <summary>
        /// Checks for a versioning conflict with the base layout.
        /// It is not allowed to have an item apply a shared layout delta over a
        /// versioned layout value on a base layout.
        /// </summary>
        /// <param name="args">the pipeline arguments</param>
        public void Process(SaveBaseLayoutArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (args.NewBaseLayoutItem == null) return;

            if (_validator.CreatesVersioningConflict(args.Item, args.NewBaseLayoutItem))
            {
                args.Successful = false;
                args.AddMessage("Selecting this base layout would create apply a shared layout delta over a versioned layout value.");
                args.AbortPipeline();
            }
        }
    }
}