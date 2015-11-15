﻿using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Sitecore.BaseLayouts.Data
{
    public class BaseLayoutItem : CustomItem
    {
        private BaseLayoutItem _baseLayout;

        /// <summary>
        ///     Initializes a new BaseLayoutItem
        /// </summary>
        /// <param name="innerItem"></param>
        public BaseLayoutItem(Item innerItem) : base(innerItem)
        {
        }

        public BaseLayoutItem BaseLayout
        {
            get
            {
                return _baseLayout ??
                       (_baseLayout = new ReferenceField(InnerItem.Fields[BaseLayoutSettings.FieldId]).TargetItem);
            }
        }


        /// <summary>
        ///     Converts a regular item to a BaseLayoutItem
        /// </summary>
        /// <param name="innerItem"></param>
        /// <returns></returns>
        public static implicit operator BaseLayoutItem(Item innerItem)
        {
            return innerItem == null ? null : new BaseLayoutItem(innerItem);
        }

        /// <summary>
        ///     Converts a BaseLayoutItem to a regular item
        /// </summary>
        /// <param name="item">the BaseLayoutItem</param>
        /// <returns>the inner item</returns>
        public static implicit operator Item(BaseLayoutItem item)
        {
            return item.InnerItem;
        }
    }
}