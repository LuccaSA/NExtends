using System;
using System.Collections.Generic;
using System.Linq;
using NExtends.Tests.Models;
using NExtends.Primitives;
using NExtends.Primitives.Types;
using Xunit;

namespace NExtends.Tests
{
    public class GenericsTests
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
            var full = new List<string> {""};
            var empty = new List<string>();
            List<string> nulled = null;

            Assert.True(empty.IsNullOrEmpty());
            Assert.True(nulled.IsNullOrEmpty());
            Assert.False(full.IsNullOrEmpty());
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
    }
}
