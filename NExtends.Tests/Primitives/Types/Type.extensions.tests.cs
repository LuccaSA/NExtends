using NExtends.Primitives.Types;
using NExtends.Tests.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace NExtends.Tests.Primitives.Types
{
	public class TypeExtensionsTest
	{
		[Fact]
		public void IsSubclassOfInterfaceShouldHandleNonGenericInterfaces()
		{
			Assert.True(typeof(System.Collections.ArrayList).IsSubclassOfInterface(typeof(System.Collections.IEnumerable)));
		}

        [Fact]
        public void GetRealTypeName_Should_ReturnBaseType()
        {
            var type = typeof(ConcreteClass);
            var result = type.GetRealTypeName();

            Assert.Equal("ConcreteClass", result);
        }

        class Test<T>
        {
        }
        class Test : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
        class Test2 : IEnumerable<string>
        {
            public IEnumerator GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        [Theory]
        [InlineData(typeof(IEnumerable), true)]
        [InlineData(typeof(ICollection), true)]
        [InlineData(typeof(IList), true)]
        [InlineData(typeof(IDictionary), true)]
        [InlineData(typeof(Array), true)]
        [InlineData(typeof(ArrayList), true)]
        [InlineData(typeof(Test), true)]

        [InlineData(typeof(IEnumerable<string>), true)]
        [InlineData(typeof(IReadOnlyCollection<string>), true)]
        [InlineData(typeof(ICollection<string>), true)]
        [InlineData(typeof(IList<string>), true)]
        [InlineData(typeof(IReadOnlyList<string>), true)]
        [InlineData(typeof(List<string>), true)]
        [InlineData(typeof(IReadOnlyDictionary<string, string>), true)]
        [InlineData(typeof(IDictionary<string, string>), true)]
        [InlineData(typeof(Dictionary<string,string>), true)]
        [InlineData(typeof(LinkedList<string>), true)]
        [InlineData(typeof(ISet<string>), true)]
        [InlineData(typeof(HashSet<string>), true)]
        [InlineData(typeof(string[]), true)]
        [InlineData(typeof(Test2), true)]

        [InlineData(typeof(string), false)]
        [InlineData(typeof(int), false)]
        public void IsEnumerableOrArrayTests(Type type, bool result)
        {
            Assert.Equal(result, type.IsEnumerableOrArray());
        }

        [Theory]
        [InlineData(typeof(IEnumerable), typeof(object))]
        [InlineData(typeof(ICollection), typeof(object))]
        [InlineData(typeof(IList), typeof(object))]
        [InlineData(typeof(IDictionary), typeof(object))]
        [InlineData(typeof(Array), typeof(object))]
        [InlineData(typeof(ArrayList), typeof(object))]
        [InlineData(typeof(Test), typeof(object))]

        [InlineData(typeof(IEnumerable<string>), typeof(string))]
        [InlineData(typeof(IReadOnlyCollection<string>), typeof(string))]
        [InlineData(typeof(ICollection<string>), typeof(string))]
        [InlineData(typeof(IList<string>), typeof(string))]
        [InlineData(typeof(IReadOnlyList<string>), typeof(string))]
        [InlineData(typeof(List<string>), typeof(string))]
        [InlineData(typeof(IReadOnlyDictionary<string, string>), typeof(KeyValuePair<string, string>))]
        [InlineData(typeof(IDictionary<string, string>), typeof(KeyValuePair<string, string>))]
        [InlineData(typeof(Dictionary<string, string>), typeof(KeyValuePair<string, string>))]
        [InlineData(typeof(LinkedList<string>), typeof(string))]
        [InlineData(typeof(ISet<string>), typeof(string))]
        [InlineData(typeof(HashSet<string>), typeof(string))]
        [InlineData(typeof(string[]), typeof(string))]
        [InlineData(typeof(Test2), typeof(string))]

        [InlineData(typeof(string), typeof(string))]
        [InlineData(typeof(int), typeof(int))]
        [InlineData(typeof(Test<object>), typeof(Test<object>))]
        public void EnumerableOrArrayElementTypeTests(Type type, Type result)
        {
            Assert.Equal(result, type.GetEnumerableOrArrayElementType());
        }

    }
}
