using Sitecore.BaseLayouts.Data;
using Sitecore.Diagnostics;

namespace Sitecore.BaseLayouts.Pipelines.SaveBaseLayout
{
    public class CheckForCircularReference : ISaveBaseLayoutProcessor
    {
        private readonly IBaseLayoutValidator _validator;

        /// <summary>
        /// Creates an instance of the processor
        /// </summary>
        /// <param name="validator">a base layout validator</param>
        public CheckForCircularReference(IBaseLayoutValidator validator)
        {
            Assert.ArgumentNotNull(validator, "validator");
            _validator = validator;
        }

        /// <summary>
        /// Checks for a circular reference in the base layout chain
        /// </summary>
        /// <param name="args">the pipeline arguments</param>
        public void Process(SaveBaseLayoutArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (args.NewBaseLayoutItem == null) return;

            if (_validator.CreatesCircularBaseLayoutReference(args.Item, args.NewBaseLayoutItem))
            {
                args.Successful = false;
                args.AddMessage("Selecting this base layout would create a circular reference in the base layout chain.");
                args.AbortPipeline();
            }
        }
    }
}