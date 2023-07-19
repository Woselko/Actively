using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Security.AccessControl;
using Resources;
using Xunit;

namespace Resources.Tests
{
    public class ResourceMatchTest
    {
        [Fact]
        public void ResourceMatchTest_HaveSameAmount_AndNotEmpty()
        {
            ResourceManager rm = new ResourceManager("Resources.Common", typeof(Common).Assembly);
            List<CultureInfo> list = new List<CultureInfo>()
            {
                new CultureInfo("en"),
            };
            var plKeys = new List<string>();
            var enKeys = new List<string>();
            ResourceSet resourceSetPl = rm.GetResourceSet(new CultureInfo("pl"), true, true);
            ResourceSet resourceSetEn = rm.GetResourceSet(new CultureInfo("en"), true, true);
            int enAmount = 0;
            int plAmount = 0;

            Assert.True(resourceSetEn is not null);
            Assert.True(resourceSetPl is not null);

            foreach (DictionaryEntry entry in resourceSetEn)
            {
                string resourceKey = entry.Key.ToString();
                string resource = entry.Value.ToString();
                Assert.True(!string.IsNullOrEmpty(resourceKey));
                Assert.True(!string.IsNullOrEmpty(resource));
                enAmount++;
                enKeys.Add(resourceKey);
            }

            foreach (DictionaryEntry entry in resourceSetPl)
            {
                string resourceKey = entry.Key.ToString();
                string resource = entry.Value.ToString();
                Assert.True(!string.IsNullOrEmpty(resourceKey));
                Assert.True(!string.IsNullOrEmpty(resource));
                plAmount++;
                plKeys.Add(resourceKey);
            }

            Assert.True(enAmount.Equals(plAmount));
            plKeys.ForEach(x => Assert.True(enKeys.Contains(x)));

        }
    }
}