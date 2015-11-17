using System;
using Sitecore.Data;
using Sitecore.Data.Validators;
using Sitecore.Diagnostics;

namespace Sitecore.BaseLayouts.Data
{
    public class BaseLayoutCircularReferenceValidator : StandardValidator
    {
        private readonly IBaseLayoutValidator _baseLayoutValidator;

        public BaseLayoutCircularReferenceValidator() : this(new BaseLayoutValidator(new BaseLayoutSettings()))
        {
        }

        public BaseLayoutCircularReferenceValidator(IBaseLayoutValidator baseLayoutValidator)
        {
            Assert.ArgumentNotNull(baseLayoutValidator, "baseLayoutValidator");
            _baseLayoutValidator = baseLayoutValidator;
        }

        protected override ValidatorResult Evaluate()
        {
            var value = GetControlValidationValue();
            if (string.IsNullOrEmpty(value))
            {
                return ValidatorResult.Valid;
            }

            ID id;
            if (!ID.TryParse(value, out id))
            {
                return GetFailedResult(ValidatorResult.Error);
            }

            var item = GetItem();
            var baseLayout = item.Database.GetItem(id);
            if (baseLayout == null)
            {
                return GetFailedResult(ValidatorResult.Error);
            }

            if (_baseLayoutValidator.CreatesCircularBaseLayoutReference(item, baseLayout))
            {
                return GetFailedResult(ValidatorResult.FatalError);
            }

            return ValidatorResult.Valid;
        }

        protected override ValidatorResult GetMaxValidatorResult()
        {
            return GetFailedResult(ValidatorResult.FatalError);
        }

        public override string Name
        {
            get { return "Base Layout Circular Reference Validator"; }
        }
    }
}