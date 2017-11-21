using NExtends.Primitives.Generics;
using NExtends.Primitives.Types;
using NExtends.Tests.Models;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NExtends.Tests.Primitives.Generics
{
    public class GenericsExtensionsTests
    {
        [Fact]
        public void CastingCollectionOfClassToAnotherThroughtInterfaceShouldWork()
        {
            var t = new GenericTestsClassT() { Id = 1, Name = "T", CustomT = "CustomT", UnexpectedCommonName = "Common" };

            var collection = new HashSet<GenericTestsClassT>() { t };

            var result = collection.Cast<GenericTestsClassT, GenericTestsClassU, GenericTestsInterfaceI>().SingleOrDefault();

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("T", result.Name);
            Assert.Null(result.CustomU);
            Assert.Null(result.UnexpectedCommonName);
        }

        [Fact]
        public void CastingObjectOfClassToAnotherThroughtInterfaceShouldWork()
        {
            var t = new GenericTestsClassT() { Id = 1, Name = "T", CustomT = "CustomT", UnexpectedCommonName = "Common" };

            var result = t.Cast<GenericTestsClassT, GenericTestsClassU, GenericTestsInterfaceI>();

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("T", result.Name);
            Assert.Null(result.CustomU);
            Assert.Null(result.UnexpectedCommonName);
        }

        [Theory]
        [MemberData(nameof(EquivalenceStructData))]
        public void TestEquivalentStruct(int[] source, int[] other, bool expected)
        {
            Assert.Equal(source.IsEquivalentTo(other), expected);
        }

        [Theory]
        [MemberData(nameof(EquivalenceClassData))]
        public void TestEquivalentClass(Tdata[] source, Tdata[] other, bool expected)
        {
            Assert.Equal(source.IsEquivalentTo(other, new TestDataEqualityComparer()), expected);
        }

        public static TheoryData EquivalenceStructData()
        {
            var theory = new TheoryData<int[], int[], bool>();
            foreach (var data in Generate())
            {
                theory.Add(data.source, data.other, data.expected);
            }
            return theory;
        }

        public static TheoryData EquivalenceClassData()
        {
            var theory = new TheoryData<Tdata[], Tdata[], bool>();
            foreach (var data in Generate())
            {
                theory.Add(
                    data.source.Select(i => new Tdata(i)).ToArray(),
                    data.other.Select(i => new Tdata(i)).ToArray(),
                    data.expected);
            }
            return theory;
        }

        private static IEnumerable<(int[] source, int[] other, bool expected)> Generate()
            => new[]
            {
                (new[] {1}, new[] {1}, true),
                (new[] {1, 2, 3}, new[] {1, 2, 3}, true),
                (new[] {1, 3, 2}, new[] {2, 3, 1}, true),
                (new[] {1, 1, 1, 4}, new[] {1, 1, 1, 4}, true),
                (new[] {1, 1, 1, 4}, new[] {1, 4, 1, 1}, true),
                (new[] {1, 1, 3}, new[] {1, 3, 3}, false),
                (new[] {1}, new[] {1, 1}, false),
                (new int[] { }, new[] {1}, false),
                (new int[] { }, new int[] { }, true)
            };

        public class Tdata
        {
            public Tdata(int data)
            {
                Data = data;
            }
            public int Data { get; }
        }

        private class TestDataEqualityComparer : IEqualityComparer<Tdata>
        {
            public bool Equals(Tdata x, Tdata y) => x.Data == y.Data;
            public int GetHashCode(Tdata obj) => obj.Data.GetHashCode();
        }
    }
}
