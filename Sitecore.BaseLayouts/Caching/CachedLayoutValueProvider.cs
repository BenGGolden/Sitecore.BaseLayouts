using Sitecore.Data.Fields;
using Sitecore.Diagnostics;

namespace Sitecore.BaseLayouts.Caching
{
    public class CachedLayoutValueProvider : ILayoutValueProvider
    {
        private readonly ILayoutValueProvider _innerProvider;
        private readonly ILayoutValueCache _cache;

        public CachedLayoutValueProvider(ILayoutValueProvider innerProvider, ILayoutValueCache cache)
        {
            Assert.ArgumentNotNull(innerProvider, "innerProvider");
            Assert.ArgumentNotNull(cache, "cache");

            _innerProvider = innerProvider;
            _cache = cache;
        }

        public string GetLayoutValue(Field field)
        {
            var value = _cache.GetLayoutValue(field);
            if (string.IsNullOrWhiteSpace(value))
            {
                value = _innerProvider.GetLayoutValue(field);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _cache.AddLayoutValue(field, value);
                }
            }

            return value;
        }
    }
}