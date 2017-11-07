using NExtends.Expressions;
using Xunit;

namespace NExtends.Tests.Expressions
{
	interface IUser
	{
		int Id { get; }
	}

	class User : IUser
	{
		public int Id { get; set; }
		public User Manager { get; set; }
	}

	class Department
	{
		public User Head { get; set; }
	}

	public class ExpressionMergerTests
	{

		[Fact]
		public void Expressions2Merger()
		{
			var user = new User { Id = 2 };

			var result = ExpressionMerger.Merge<User, int, bool>(u => u.Id, i => i == 2);

			Assert.True(result.Compile()(user));
		}

		[Fact]
		public void Expressions3Merger()
		{
			var dpt = new Department { Head = new User { Id = 2 } };

			var result = ExpressionMerger.Merge<Department, IUser, bool>(d => d.Head, u => u.Id == 2);

			Assert.True(result.Compile()(dpt));
		}
	}
}