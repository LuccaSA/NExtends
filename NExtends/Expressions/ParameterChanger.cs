using System;
using System.Linq.Expressions;

namespace NExtends.Expressions
{
	public class ParameterChanger : ExpressionVisitor
	{
		ParameterExpression RemplacingParameter { get; set; }
		ParameterExpression RemplacedParameter { get; set; }

		ParameterChanger(ParameterExpression remplacedParameter, ParameterExpression remplacingParameter)
		{
			RemplacedParameter = remplacedParameter;
			RemplacingParameter = remplacingParameter;
		}

		public static Expression<Func<T, bool>> ChangeParameter<T>(Expression<Func<T, bool>> lambda, ParameterExpression replacingParameter)
		{
			var newBody = GetChangedParameterBody(lambda, replacingParameter);
			return Expression.Lambda<Func<T, bool>>(newBody, replacingParameter);
		}

		public static Expression GetChangedParameterBody<T>(Expression<Func<T, bool>> lambda, ParameterExpression replacingParameter)
		{
			return new ParameterChanger(lambda.Parameters[0], replacingParameter).Visit(lambda.Body);
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{
			return node == RemplacedParameter ? RemplacingParameter : node;
		}
	}
}