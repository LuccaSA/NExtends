using System;
using System.Linq.Expressions;

namespace NExtends.Expressions
{
	public class ExpressionMerger : ExpressionVisitor
	{
		Expression CurrentParameterExpression { get; set; }

		public static Expression<Func<TIn, TOut>> Merge<TIn, TA, TOut>(Expression<Func<TIn, TA>> entryPoint, Expression<Func<TA, TOut>> expression1)
		{
			return new ExpressionMerger().MergeAll<TIn, TA, TOut>(entryPoint, expression1);
		}

		public static Expression<Func<TIn, TOut>> Merge<TIn, TA, TB, TOut>(Expression<Func<TIn, TA>> entryPoint, Expression<Func<TA, TB>> expression1, Expression<Func<TB, TOut>> expression2)
		{
			return new ExpressionMerger().MergeAll<TIn, TA, TOut>(entryPoint, expression1, expression2);
		}

		protected Expression<Func<TIn, TOut>> MergeAll<TIn, T, TOut>(Expression<Func<TIn, T>> entryPoint, params LambdaExpression[] expressions)
		{
			CurrentParameterExpression = entryPoint.Body;

			foreach (var expression in expressions)
			{
				CurrentParameterExpression = Visit(expression.Body);
			}

			return Expression.Lambda<Func<TIn, TOut>>(CurrentParameterExpression, entryPoint.Parameters[0]);
		} 

		protected override Expression VisitParameter(ParameterExpression node)
		{
			//replace current lambda parameter with ~previous lambdas
			return CurrentParameterExpression;
		}
	}
}