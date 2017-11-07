using NExtends.Expressions;
using System;
using System.Collections.Generic;
using Xunit;

namespace NExtends.Tests.Expressions
{
	class UserDate
	{
		public int OwnerId { get; set; }
		public User Owner { get; set; }
		public DateTime Date { get; set; }

		public UserDate(User owner, DateTime date)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			Date = date;
		}
	}

	public class ExpressionReducerTests
	{
		DateTime today = DateTime.Today;
		List<int> elements = new List<int> { 0, 1, 2 };
		UserDate fakeUd = new UserDate(new User(), DateTime.Today);

		[Fact]
		public void ExpressionReducerEmptyLogic()
		{
			var result = ExpressionReducer<UserDate, User>.Reduce(ud => (ud.OwnerId == 1 || ud.OwnerId == 2) && ud.Date > today, u => u.Owner, false);

			Assert.Equal("u => False", result.ToString());
		}

		[Fact]
		public void ExpressionReducerOrLogic()
		{
			var result = ExpressionReducer<UserDate, User>.Reduce(ud => (ud.Owner.Id == 1 || ud.Owner.Id == 2) && ud.Date > today, u => u.Owner, false);

			Assert.Equal("u => ((u.Id == 1) OrElse (u.Id == 2))", result.ToString());
		}

		[Fact]
		public void ExpressionReducerConstantMemberTrick()
		{
			var result = ExpressionReducer<UserDate, User>.Reduce(ud => (ud.Owner.Id == 1 || ud.Owner.Id == 2) && (fakeUd.Owner.Id != 2), u => u.Owner, false);

			Assert.Equal("u => (((u.Id == 1) OrElse (u.Id == 2)) AndAlso (value(NExtends.Tests.Expressions.ExpressionReducerTests).fakeUd.Owner.Id != 2))", result.ToString());
		}

		[Fact]
		public void ExpressionReducerConstantMemberEdgeCase()
		{
			var result = ExpressionReducer<User, User>.Reduce(u => u.Manager.Manager.Id == 1, u => u.Manager, false);

			Assert.Equal("u => (u.Manager.Id == 1)", result.ToString());
		}

		[Fact]
		public void ExpressionReducerContainsLogic()
		{
			var result = ExpressionReducer<UserDate, User>.Reduce(ud => elements.Contains(ud.Owner.Id) && ud.Date > today, u => u.Owner, false);

			Assert.Equal("u => value(NExtends.Tests.Expressions.ExpressionReducerTests).elements.Contains(u.Id)", result.ToString());
		}

		[Fact]
		public void ExpressionReducerFalseOrElse()
		{
			var result = ExpressionReducer<UserDate, User>.Reduce(ud => elements.Contains(ud.Owner.Id) && (false || ud.Date > today), u => u.Owner, false);

			Assert.Equal("u => value(NExtends.Tests.Expressions.ExpressionReducerTests).elements.Contains(u.Id)", result.ToString());
		}

		[Fact]
		public void ExpressionReducerTrueAndAlso()
		{
			var result = ExpressionReducer<UserDate, User>.Reduce(ud => elements.Contains(ud.Owner.Id) || (true && ud.Date > today), u => u.Owner, false);

			Assert.Equal("u => value(NExtends.Tests.Expressions.ExpressionReducerTests).elements.Contains(u.Id)", result.ToString());
		}

		[Fact]
		public void ExpressionReducerRootTrueAndAlso()
		{
			var result = ExpressionReducer<UserDate, User>.Reduce(ud => true && elements.Contains(ud.Owner.Id), u => u.Owner, false);

			Assert.Equal("u => (True AndAlso value(NExtends.Tests.Expressions.ExpressionReducerTests).elements.Contains(u.Id))", result.ToString());
		}

		interface ITruc
		{
			Truc Truc { get; }
		}

		class Truc
		{
			public string Content { get; set; }
		}

		class BiduleContainer : ITruc
		{
			public Truc Machin { get; set; }
			Truc ITruc.Truc { get { return Machin; } }
		}

		[Fact]
		public void ExpressionReducerInterfaceMember()
		{
			var result = ExpressionReducer<BiduleContainer, Truc>.Reduce(bc => bc.Machin.Content != null, u => u.Machin, true);

			Assert.Equal("u => (u.Content != null)", result.ToString());
		}
	}
}