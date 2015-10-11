using Sitecore.BaseLayouts.Data;
using Sitecore.Data.Fields;
using Sitecore.Diagnostics;

namespace Sitecore.BaseLayouts.Caching
{
    public class CachedBaseLayoutValueProvider : IBaseLayoutValueProvider
    {
        private readonly IBaseLayoutValueProvider _innerProvider;
        private readonly IBaseLayoutValueCache _cache;

        public CachedBaseLayoutValueProvider(IBaseLayoutValueProvider innerProvider, IBaseLayoutValueCache cache)
        {
            Assert.ArgumentNotNull(innerProvider, "innerProvider");
            Assert.ArgumentNotNull(cache, "cache");

            _innerProvider = innerProvider;
            _cache = cache;
        }

        public string GetBaseLayoutValue(Field field)
        {
            var value = _cache.GetLayoutValue(field);
            if (string.IsNullOrWhiteSpace(value))
            {
                value = _innerProvider.GetBaseLayoutValue(field);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _cache.AddLayoutValue(field, value);
                }
            }

            return value;
        }
    }
}