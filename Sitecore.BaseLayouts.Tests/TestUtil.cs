using NSubstitute;

namespace Sitecore.BaseLayouts.Tests
{
    public class TestUtil
    {
        public static IBaseLayoutSettings CreateFakeSettings(string[] supportedDatabases = null, bool alwaysCheckForCircularReference = false)
        {
            var settings = Substitute.For<IBaseLayoutSettings>();
            settings.SupportedDatabases.Returns(supportedDatabases ?? new[] { "master", "web" });
            settings.LayoutValueCacheSize.Returns(10485760);
            settings.AlwaysCheckForCircularReference.Returns(alwaysCheckForCircularReference);
            return settings;
        }
    }
}