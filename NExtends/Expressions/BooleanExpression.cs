using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NExtends.Expressions
{
	public static class BooleanExpression
	{
		public static Expression<Func<TEntity, bool>> OrElse<TEntity>(this Expression<Func<TEntity, bool>> left, Expression<Func<TEntity, bool>> right)
		{
			return OrFactory<TEntity>(left, right);
		}

		public static Expression<Func<TEntity, bool>> AndAlso<TEntity>(this Expression<Func<TEntity, bool>> left, Expression<Func<TEntity, bool>> right)
		{
			return AndFactory<TEntity>(left, right);
		}

		/// <summary>
		/// Produces the boolean Expression aggregating expressions using the 'OrElse' aggregator
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="filters"></param>
		/// <returns></returns>
		public static Expression<Func<TEntity, bool>> OrAggregation<TEntity>(this IEnumerable<Expression<Func<TEntity, bool>>> filters)
		{
			return Factory(Expression.OrElse, filters.ToArray());
		}

		/// <summary>
		/// Produces the boolean Expression aggregating expressions using the 'AndAlso' aggregator
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="filters"></param>
		/// <returns></returns>
		public static Expression<Func<TEntity, bool>> AndAggregation<TEntity>(this IEnumerable<Expression<Func<TEntity, bool>>> filters)
		{
			return Factory(Expression.AndAlso, filters.ToArray());
		}

		static Expression<Func<TEntity, bool>> OrFactory<TEntity>(params Expression<Func<TEntity, bool>>[] filters)
		{
			return Factory(Expression.OrElse, filters);
		}
		static Expression<Func<TEntity, bool>> AndFactory<TEntity>(params Expression<Func<TEntity, bool>>[] filters)
		{
			return Factory(Expression.AndAlso, filters);
		}

		static Expression<Func<TEntity, bool>> Factory<TEntity>(Func<Expression, Expression, Expression> aggregator, params Expression<Func<TEntity, bool>>[] filters)
		{
			if (filters.Length == 0)
				return null;

			var seed = filters[0];
			if (filters.Length == 1)
				return seed;

			var param = seed.Parameters[0];
			var correctParamsFilters = filters.Select(f => ParameterChanger.GetChangedParameterBody(f, param));

			return Expression.Lambda<Func<TEntity, bool>>(correctParamsFilters.Skip(1).Aggregate(seed.Body, aggregator), param);
		}
	}
}