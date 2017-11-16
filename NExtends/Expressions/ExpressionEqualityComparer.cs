using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NExtends.Expressions
{
	/// <summary>
	/// source : http://stackoverflow.com/questions/283537/most-efficient-way-to-test-equality-of-lambda-expressions
	/// </summary>
	public static class ExpressionEqualityComparer
	{
		public static bool Eq(LambdaExpression x, LambdaExpression y)
		{
			return ExpressionsEqual(x, y, null, null);
		}

		public static bool Eq<TSource1, TSource2, TValue>(Expression<Func<TSource1, TSource2, TValue>> x, Expression<Func<TSource1, TSource2, TValue>> y)
		{
			return ExpressionsEqual(x, y, null, null);
		}

		public static Expression<Func<Expression<Func<TSource, TValue>>, bool>> Eq<TSource, TValue>(Expression<Func<TSource, TValue>> y)
		{
			return x => ExpressionsEqual(x, y, null, null);
		}

		private static bool ExpressionsEqual(Expression x, Expression y, LambdaExpression rootX, LambdaExpression rootY)
		{
			if (ReferenceEquals(x, y)) return true;
			if (x == null || y == null) return false;

			var valueX = TryCalculateConstant(x);
			var valueY = TryCalculateConstant(y);

			if (valueX.IsDefined && valueY.IsDefined)
				return ValuesEqual(valueX.Value, valueY.Value);

			if (x.NodeType != y.NodeType
				|| x.Type != y.Type)
			{
				if (IsAnonymousType(x.Type) && IsAnonymousType(y.Type))
					throw new NotImplementedException("Comparison of Anonymous Types is not supported");
				return false;
			}

			if (x is LambdaExpression lx)
			{
				var ly = (LambdaExpression)y;
				var paramsX = lx.Parameters;
				var paramsY = ly.Parameters;
				return CollectionsEqual(paramsX, paramsY, lx, ly) && ExpressionsEqual(lx.Body, ly.Body, lx, ly);
			}
			if (x is MemberExpression mex)
			{
				var mey = (MemberExpression)y;
				return Equals(mex.Member, mey.Member) && ExpressionsEqual(mex.Expression, mey.Expression, rootX, rootY);
			}
			if (x is BinaryExpression bx)
			{
				var by = (BinaryExpression)y;
				return bx.Method == @by.Method && ExpressionsEqual(bx.Left, @by.Left, rootX, rootY) &&
					   ExpressionsEqual(bx.Right, @by.Right, rootX, rootY);
			}
			if (x is UnaryExpression ux)
			{
				var uy = (UnaryExpression)y;
				return ux.Method == uy.Method && ExpressionsEqual(ux.Operand, uy.Operand, rootX, rootY);
			}
			if (x is ParameterExpression px)
			{
				var py = (ParameterExpression)y;
				return rootX.Parameters.IndexOf(px) == rootY.Parameters.IndexOf(py);
			}
			if (x is MethodCallExpression mcx)
			{
				var mcy = (MethodCallExpression)y;
				return mcx.Method == mcy.Method
					   && ExpressionsEqual(mcx.Object, mcy.Object, rootX, rootY)
					   && CollectionsEqual(mcx.Arguments, mcy.Arguments, rootX, rootY);
			}
			if (x is MemberInitExpression mix)
			{
				var miy = (MemberInitExpression)y;
				return ExpressionsEqual(mix.NewExpression, miy.NewExpression, rootX, rootY)
					   && MemberInitsEqual(mix.Bindings, miy.Bindings, rootX, rootY);
			}
			if (x is NewArrayExpression nax)
			{
				var nay = (NewArrayExpression)y;
				return CollectionsEqual(nax.Expressions, nay.Expressions, rootX, rootY);
			}
			if (x is NewExpression nx)
			{
				var ny = (NewExpression)y;
				return
					Equals(nx.Constructor, ny.Constructor)
					&& CollectionsEqual(nx.Arguments, ny.Arguments, rootX, rootY)
					&& (nx.Members == null && ny.Members == null
						|| nx.Members != null && ny.Members != null && CollectionsEqual(nx.Members, ny.Members));
			}
			if (x is ConditionalExpression cx)
			{
				var cy = (ConditionalExpression)y;
				return
					ExpressionsEqual(cx.Test, cy.Test, rootX, rootY)
					&& ExpressionsEqual(cx.IfFalse, cy.IfFalse, rootX, rootY)
					&& ExpressionsEqual(cx.IfTrue, cy.IfTrue, rootX, rootY);
			}

			throw new NotImplementedException(x.ToString());
		}

		private static Boolean IsAnonymousType(Type type)
		{
			Boolean hasCompilerGeneratedAttribute = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any();
			Boolean nameContainsAnonymousType = type.FullName.Contains("AnonymousType");
			Boolean isAnonymousType = hasCompilerGeneratedAttribute && nameContainsAnonymousType;

			return isAnonymousType;
		}

		private static bool MemberInitsEqual(ICollection<MemberBinding> bx, ICollection<MemberBinding> by, LambdaExpression rootX, LambdaExpression rootY)
		{
			if (bx.Count != by.Count)
			{
				return false;
			}

			if (bx.Concat(by).Any(b => b.BindingType != MemberBindingType.Assignment))
				throw new NotImplementedException("Only MemberBindingType.Assignment is supported");

			return
				bx.Cast<MemberAssignment>().OrderBy(b => b.Member.Name).Select((b, i) => new { Expr = b.Expression, b.Member, Index = i })
				.Join(
					  by.Cast<MemberAssignment>().OrderBy(b => b.Member.Name).Select((b, i) => new { Expr = b.Expression, b.Member, Index = i }),
					  o => o.Index, o => o.Index, (xe, ye) => new { XExpr = xe.Expr, XMember = xe.Member, YExpr = ye.Expr, YMember = ye.Member })
					   .All(o => Equals(o.XMember, o.YMember) && ExpressionsEqual(o.XExpr, o.YExpr, rootX, rootY));
		}

		private static bool ValuesEqual(object x, object y)
		{
			if (ReferenceEquals(x, y))
				return true;
			if (x is ICollection && y is ICollection)
				return CollectionsEqual((ICollection)x, (ICollection)y);

			return Equals(x, y);
		}

		private static ConstantValue TryCalculateConstant(Expression e)
		{
            var constantExp = e as ConstantExpression;
            if (constantExp != null)
            {
                return new ConstantValue(true, constantExp.Value);
            }

            var memberExp = e as MemberExpression;
            if (memberExp != null)
			{
				var parentValue = TryCalculateConstant(memberExp.Expression);
				if (parentValue.IsDefined)
				{
					var result =
                        memberExp.Member is FieldInfo
							? ((FieldInfo)memberExp.Member).GetValue(parentValue.Value)
							: ((PropertyInfo)memberExp.Member).GetValue(parentValue.Value);
					return new ConstantValue(true, result);
				}
			}

            var newArrayExp = e as NewArrayExpression;
            if (newArrayExp != null)
			{
				var result = newArrayExp.Expressions.Select(TryCalculateConstant);
				if (result.All(i => i.IsDefined))
					return new ConstantValue(true, result.Select(i => i.Value).ToArray());
			}

            var conditionalExp = e as ConditionalExpression;
            if (conditionalExp != null)
			{
				var evaluatedTest = TryCalculateConstant(conditionalExp.Test);
				if (evaluatedTest.IsDefined)
				{
					return TryCalculateConstant(Equals(evaluatedTest.Value, true) ? conditionalExp.IfTrue : conditionalExp.IfFalse);
				}
			}

			return default(ConstantValue);
		}

		private static bool CollectionsEqual(IEnumerable<Expression> x, IEnumerable<Expression> y, LambdaExpression rootX, LambdaExpression rootY)
		{
			return x.Count() == y.Count()
				   && x.Select((e, i) => new { Expr = e, Index = i })
					   .Join(y.Select((e, i) => new { Expr = e, Index = i }),
							 o => o.Index, o => o.Index, (xe, ye) => new { X = xe.Expr, Y = ye.Expr })
					   .All(o => ExpressionsEqual(o.X, o.Y, rootX, rootY));
		}

		private static bool CollectionsEqual(ICollection x, ICollection y)
		{
			return x.Count == y.Count
				   && x.Cast<object>().Select((e, i) => new { Expr = e, Index = i })
					   .Join(y.Cast<object>().Select((e, i) => new { Expr = e, Index = i }),
							 o => o.Index, o => o.Index, (xe, ye) => new { X = xe.Expr, Y = ye.Expr })
					   .All(o => Equals(o.X, o.Y));
		}

		private struct ConstantValue
		{
			public ConstantValue(bool isDefined, object value)
				: this()
			{
				IsDefined = isDefined;
				Value = value;
			}

			public bool IsDefined { get; private set; }

			public object Value { get; private set; }
		}
	}
}
