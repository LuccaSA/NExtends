using System;
using System.Linq.Expressions;

namespace NExtends.Expressions
{
	public class ExpressionMerger : ExpressionVisitor
	{
		Expression CurrentParameterExpression { get; set; }

		public static Expression<Func<TIn, TOut>> Merge<TIn, TOut>(LambdaExpression entryPoint, LambdaExpression expression1)
		{
			return new ExpressionMerger().MergeAll<TIn, TOut>(entryPoint, expression1);
		}

		public static Expression<Func<TIn, TOut>> Merge<TIn, TOut>(LambdaExpression entryPoint, LambdaExpression expression1, LambdaExpression expression2)
		{
			return new ExpressionMerger().MergeAll<TIn, TOut>(entryPoint, expression1, expression2);
		}

		protected Expression<Func<TIn, TOut>> MergeAll<TIn, TOut>(LambdaExpression entryPoint, params LambdaExpression[] expressions)
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