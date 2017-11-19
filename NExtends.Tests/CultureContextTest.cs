using System;
using System.Globalization;
using System.Threading.Tasks;
using NExtends.Context;
using Xunit;

namespace NExtends.Tests
{
    public class CultureContextTest
    {
        [Fact]
        public void SimpleSwapString()
        {
            using (new CultureContext("fr-FR"))
            {
                Assert.Equal(new CultureInfo("fr-FR"), CultureInfo.CurrentCulture);
                using (new CultureContext("en-EN"))
                {
                    Assert.Equal(new CultureInfo("en-EN"), CultureInfo.CurrentCulture);
                }
                Assert.Equal(new CultureInfo("fr-FR"), CultureInfo.CurrentCulture);
            }
        }

        [Fact]
        public void SimpleSwapCultureInfo()
        {
            var fr = new CultureInfo("fr-FR");
            var en = new CultureInfo("en-EN");
            using (new CultureContext(fr))
            {
                Assert.Equal(fr, CultureInfo.CurrentCulture);
                using (new CultureContext(en))
                {
                    Assert.Equal(en, CultureInfo.CurrentCulture);
                }
                Assert.Equal(fr, CultureInfo.CurrentCulture);
            }
        }

        [Fact]
        public void UnknownCultureShouldThrow()
        {
            Assert.ThrowsAny<Exception>(() =>
            {
                new CultureContext("unknown");
            });
            Assert.ThrowsAny<Exception>(() =>
            {
                new CultureContext((string)null);
            });
            Assert.ThrowsAny<Exception>(() =>
            {
                new CultureContext((CultureInfo)null);
            });
        }

        [Fact]
        public async Task AsyncSwap()
        {
            var fr = new CultureInfo("fr-FR");
            var en = new CultureInfo("en-EN");
            using (new CultureContext(fr))
            {
                await AsyncDelay(() =>
                {
                    Assert.Equal(fr, CultureInfo.CurrentCulture);
                });
                Assert.Equal(fr, CultureInfo.CurrentCulture);
                await AsyncDelay(() =>
                {
                    Assert.Equal(fr, CultureInfo.CurrentCulture);
                    using (new CultureContext(en))
                    {
                        Assert.Equal(en, CultureInfo.CurrentCulture);
                    }
                });
                Assert.Equal(fr, CultureInfo.CurrentCulture);
                using (new CultureContext(en))
                {
                    Assert.Equal(en, CultureInfo.CurrentCulture);
                    await AsyncDelay(() =>
                    {
                        Assert.Equal(en, CultureInfo.CurrentCulture);
                    });
                }
                Assert.Equal(fr, CultureInfo.CurrentCulture);
            }
        }

        private async Task AsyncDelay(Action action)
        {
            await Task.Delay(100);
            await Task.Yield();
            if (action != null)
                action();
        }
    }
}
