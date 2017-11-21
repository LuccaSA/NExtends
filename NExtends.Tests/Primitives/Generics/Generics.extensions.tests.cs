using NExtends.Primitives.Generics;
using NExtends.Primitives.Types;
using NExtends.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NExtends.Tests.Primitives.Generics
{
    public class GenericsExtensionsTests
    {
        [Fact]
        public void ToDictionaryTest()
        {
            var dic = new Dictionary<string, string>
            {
                { "1","1"},
                { "2","2"}
            };

            Dictionary<string, string> newDictionary = dic.Where(i => i.Key == "1").ToDictionary();

            Assert.True(newDictionary.ContainsKey("1"));
            Assert.Single(newDictionary);

            Assert.Throws<ArgumentNullException>(() =>
            {
                IEnumerable<KeyValuePair<string, string>> nulled = null;
                nulled.ToDictionary();
            });
            Assert.Empty(Enumerable.Empty<KeyValuePair<string, string>>().ToDictionary());
        }

        [Fact]
        public void UpdateKeyTest()
        {
            var dic = new Dictionary<string, string>
            {
                { "x","value_x"},
                { "y","value_y"},
                { "z","value_z"}
            };
            dic.UpdateKey("x", "xx");
            Assert.Equal("value_x", dic["xx"]);
            Assert.Throws<ArgumentException>(() =>
            {
                dic.UpdateKey("y", "z");
            });
            Assert.Throws<KeyNotFoundException>(() =>
            {
                dic.UpdateKey("aaa", "bbbb");
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                Dictionary<string, string> nulled = null;
                nulled.UpdateKey("aaa", "bbbb");
            });
        }

        [Fact]
        public void CollectionTests()
        {
            ICollection<string> collection = new List<string>();
            collection.AddMany("1");
            collection.AddRange(new[] { "2", "3", "4", "5" });
            collection.RemoveRange(new[] { "4", "5" });
            Assert.Equal(3, collection.Count);
            collection = null;
            Assert.Throws<ArgumentNullException>(() =>
            {
                collection.AddMany("1");
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                collection.AddRange(new[] { "1" });
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                collection.RemoveRange(new[] { "1" });
            });
        }

        [Fact]
        public void IsNullOrEmptyTest()
        {
            var full = new List<string> { "" };
            var empty = new List<string>();
            List<string> nulled = null;

            Assert.True(empty.IsNullOrEmpty());
            Assert.True(nulled.IsNullOrEmpty());
            Assert.False(full.IsNullOrEmpty());
        }

        [Fact]
        public void ToHashSetTest()
        {
            var full = new List<string> { "" };
            var hashset = GenericExtensions.ToHashSet(full);
            Assert.NotEmpty(hashset);
            Assert.Throws<ArgumentNullException>(() =>
            {
                full = null;
                hashset = GenericExtensions.ToHashSet(full);
            });
        }

        [Fact]
        public void ContainsTest()
        {
            var x = new[] { "1", "2" };
            var y = new[] { "2", "1" };
            Assert.True(x.Contains(y));
            Assert.True(y.Contains(x));
            y = null;
            Assert.False(x.Contains(y));
            Assert.False(y.Contains(x));
            x = null;
            Assert.True(x.Contains(y));
            Assert.True(y.Contains(x));

            x = new[] { "1", "2", "3" };
            y = new[] { "2", "1" };
            Assert.False(x.Contains(y));
            Assert.False(y.Contains(x));

            x = new[] { "1", "2", "1" };
            y = new[] { "2", "1" };
            Assert.False(x.Contains(y));
            Assert.False(y.Contains(x));

            x = new[] { "1", "2", "1" };
            y = new[] { "2", "1", "2" };
            Assert.False(x.Contains(y));
            Assert.False(y.Contains(x));

            x = new[] { "1", "2", "1", null };
            y = new[] { "2", "1", null, null };
            Assert.False(x.Contains(y));
            Assert.False(y.Contains(x));
        }


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

        [Fact]
        public void UniqueOrDefault0()
        {
            Assert.Equal(0, new List<int>().UniqueOrDefault());
        }

        [Fact]
        public void UniqueOrDefault1()
        {
            Assert.Equal(456, new List<int> { 456 }.UniqueOrDefault());
        }

        [Fact]
        public void UniqueOrDefault2()
        {
            Assert.Equal(0, new List<int> { 456, 452 }.UniqueOrDefault());
        }

        [Fact]
        public void UniqueOrDefaultWithCondition0()
        {
            Assert.Equal(0, new List<int> { 455 }.UniqueOrDefault(e => e % 2 == 0));
        }

        [Fact]
        public void UniqueOrDefaultWithCondition1()
        {
            Assert.Equal(456, new List<int> { 456, 455 }.UniqueOrDefault(e => e % 2 == 0));
        }

        [Fact]
        public void UniqueOrDefaultWithCondition2()
        {
            Assert.Equal(0, new List<int> { 456, 455, 454 }.UniqueOrDefault(e => e % 2 == 0));
        }
    }
}
