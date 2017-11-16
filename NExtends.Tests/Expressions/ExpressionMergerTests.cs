using NExtends.Expressions;
using System;
using System.Linq.Expressions;
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
            Expression<Func<User, int>> entryPoint = u => u.Id;
            Expression<Func<int, bool>> expression = i => i == 2;

            var result = ExpressionMerger.Merge<User, bool>(entryPoint, expression);

            Assert.True(result.Compile()(user));
        }

        [Fact]
        public void Expressions3Merger()
        {
            var dpt = new Department { Head = new User { Id = 2 } };
            Expression<Func<Department, IUser>> entryPoint = d => d.Head;
            Expression<Func<IUser, bool>> expression = u => u.Id == 2;

            var result = ExpressionMerger.Merge<Department, bool>(entryPoint, expression);

            Assert.True(result.Compile()(dpt));
        }
    }
}