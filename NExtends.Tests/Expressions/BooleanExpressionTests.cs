using Xunit;
using System;
using System.Linq;
using System.Linq.Expressions;
using NExtends.Expressions;

namespace NExtends.Tests.Expressions
{
	public class BooleanExpressionTests
	{
		//en fait là je ne teste rien. C'est pour me rappeler que ce genre de choses ne marche pas
		[Theory]
		[InlineData(false,2)]
		[InlineData(false,3)]
		[InlineData(true,2)]
		[InlineData(true,3)]
		public void BasicBooleanExpressionsAreBroken(bool result, int param)
		{
			Assert.Throws<InvalidOperationException>(() =>
			{
				Expression<Func<int, bool>> isEven = i => i%2 == 0;
				Expression<Func<int, bool>> isOdd = i => i%2 == 1;

				var isFalse = Expression.Lambda<Func<int, bool>>(Expression.AndAlso(isEven.Body, isOdd.Body), isEven.Parameters[0]);
				var isTrue = Expression.Lambda<Func<int, bool>>(Expression.OrElse(isEven.Body, isOdd.Body), isEven.Parameters[0]);

				Assert.Equal("i => (((i % 2) == 0) AndAlso ((i % 2) == 1))", isFalse.ToString());
				Assert.Equal("i => (((i % 2) == 0) OrElse ((i % 2) == 1))", isTrue.ToString());

				Assert.Equal(result, isFalse.Compile()(param)); //plante car "i"(ie le paramètre de la seconde expression) n'est pas reconnu 
			});
		}

		[Fact]
		public void BooleanExpressionAndAlsoOrElse()
		{
			Expression<Func<int, bool>> isEven = i => i % 2 == 0;
			Expression<Func<int, bool>> isOdd = i => i % 2 == 1;

			var isFalse = isEven.AndAlso(isOdd);
			var isTrue = isEven.OrElse(isOdd);

			Assert.Equal("i => (((i % 2) == 0) AndAlso ((i % 2) == 1))", isFalse.ToString());
			Assert.Equal("i => (((i % 2) == 0) OrElse ((i % 2) == 1))", isTrue.ToString());

			Assert.False(isFalse.Compile()(2));
			Assert.False(isFalse.Compile()(3));
			Assert.True(isTrue.Compile()(2));
			Assert.True(isTrue.Compile()(3));
		}

		[Fact]
		public void BooleanExpressionWithRecursiveLambda()
		{
			Expression<Func<int[], bool>> areEven = a => a.All(i => i % 2 == 0);
			Expression<Func<int[], bool>> areOdd = b => b.All(i => i % 2 == 1);

			var isFalse = areEven.AndAlso(areOdd);
			var isTrue = areEven.OrElse(areOdd);

			Assert.Equal("a => (a.All(i => ((i % 2) == 0)) AndAlso a.All(i => ((i % 2) == 1)))", isFalse.ToString());
			Assert.Equal("a => (a.All(i => ((i % 2) == 0)) OrElse a.All(i => ((i % 2) == 1)))", isTrue.ToString());

			Assert.False(isFalse.Compile()(new[] { 2 }));
			Assert.False(isFalse.Compile()(new[] { 3 }));
			Assert.True(isTrue.Compile()(new[] { 2 }));
			Assert.True(isTrue.Compile()(new[] { 3 }));
		}

		[Fact]
		public void BooleanExpressionWithRecursiveLambdaUsingOriginalParameter()
		{
			Expression<Func<int[], bool>> areEvenOrEqualToLength = a => a.All(i => i % 2 == 0 || i == a.Length);
			Expression<Func<int[], bool>> areOddOrEqualToLength = b => b.All(i => i % 2 == 1 || i == b.Length);

			var isFalseOrLucky = areEvenOrEqualToLength.AndAlso(areOddOrEqualToLength);

			Assert.Equal("a => (a.All(i => (((i % 2) == 0) OrElse (i == ArrayLength(a)))) AndAlso a.All(i => (((i % 2) == 1) OrElse (i == ArrayLength(a)))))", isFalseOrLucky.ToString());

			Assert.False(isFalseOrLucky.Compile()(new[] { 2 }));
			Assert.False(isFalseOrLucky.Compile()(new[] { 3 }));
			Assert.True(isFalseOrLucky.Compile()(new[] { 1 }));
		}

		static readonly int[] tested = new[] { 1 };

		[Fact]
		public void BooleanExpressionWithParametersOnlyInRecursiveLambda()
		{
			Expression<Func<int[], bool>> containsOne = a => BooleanExpressionTests.tested.All(i => a.Contains(i));
			Expression<Func<int[], bool>> doesNotContainsOne = b => BooleanExpressionTests.tested.All(i => !b.Contains(i));

			var isFalse = containsOne.AndAlso(doesNotContainsOne);
			var isTrue = containsOne.OrElse(doesNotContainsOne);

			Assert.Equal("a => (BooleanExpressionTests.tested.All(i => a.Contains(i)) AndAlso BooleanExpressionTests.tested.All(i => Not(a.Contains(i))))", isFalse.ToString());
			Assert.Equal("a => (BooleanExpressionTests.tested.All(i => a.Contains(i)) OrElse BooleanExpressionTests.tested.All(i => Not(a.Contains(i))))", isTrue.ToString());

			Assert.False(isFalse.Compile()(new[] { 2 }));
			Assert.False(isFalse.Compile()(new[] { 1 }));
			Assert.True(isTrue.Compile()(new[] { 2 }));
			Assert.True(isTrue.Compile()(new[] { 1 }));
		}
	}
}