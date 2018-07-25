using System;
using System.Globalization;
using System.Threading.Tasks;
using NExtends.Context;
using NExtends.Tasks;
using Xunit;

namespace NExtends.Tests
{
    public class AsyncLazyTests
    {
        private static int Value;

        [Fact]
        public async Task AsyncLazyFromAction()
        {
            Value = 32;
            var lazy = new AsyncLazy<int>(() => Value++);
            Assert.Equal(32, await lazy.Value);
            Assert.Equal(32, await lazy.Value);
        }

        [Fact]
        public async Task AsyncLazyFromTask()
        {
            Value = 32;
            var lazy = new AsyncLazy<int>(AsyncDelay);
            Assert.Equal(32, await lazy.Value);
            Assert.Equal(32, await lazy.Value);
        }

        private async Task<int> AsyncDelay()
        {
            await Task.Delay(10);
            return Value++;
        }
    }
}