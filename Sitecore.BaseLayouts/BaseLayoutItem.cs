using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Sitecore.BaseLayouts
{
    public class BaseLayoutItem : CustomItem
    {
        /// <summary>
        /// Initializes a new BaseLayoutItem
        /// </summary>
        /// <param name="innerItem"></param>
        public BaseLayoutItem(Item innerItem) : base(innerItem)
        {
        }


        /// <summary>
        /// Converts a regular item to a BaseLayoutItem
        /// </summary>
        /// <param name="innerItem"></param>
        /// <returns></returns>
        public static implicit operator BaseLayoutItem(Item innerItem)
        {
            return innerItem == null ? null : new BaseLayoutItem(innerItem);
        }

        private BaseLayoutItem _baseLayout;
        public BaseLayoutItem BaseLayout
        {
            get
            {
                return _baseLayout ??
                       (_baseLayout = new ReferenceField(InnerItem.Fields[BaseLayoutSettings.FieldId]).TargetItem);
            }
        }
    }
}