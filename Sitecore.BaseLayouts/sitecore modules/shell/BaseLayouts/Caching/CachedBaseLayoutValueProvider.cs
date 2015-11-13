using Sitecore.BaseLayouts.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Sitecore.BaseLayouts.Caching
{
    public class CachedBaseLayoutValueProvider : IBaseLayoutValueProvider
    {
        private readonly IBaseLayoutValueCache _cache;
        private readonly IBaseLayoutValueProvider _innerProvider;

        public CachedBaseLayoutValueProvider(IBaseLayoutValueProvider innerProvider, IBaseLayoutValueCache cache)
        {
            Assert.ArgumentNotNull(innerProvider, "innerProvider");
            Assert.ArgumentNotNull(cache, "cache");

            _innerProvider = innerProvider;
            _cache = cache;
        }

        public string GetBaseLayoutValue(Item item)
        {
            var value = _cache.GetLayoutValue(item);
            if (string.IsNullOrWhiteSpace(value))
            {
                value = _innerProvider.GetBaseLayoutValue(item);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _cache.AddLayoutValue(item, value);
                }
            }

            return value;
        }
    }
}