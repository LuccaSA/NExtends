using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static NExtends.Expressions.ExpressionHelper;

namespace NExtends.Expressions
{
	public class KeepTypeInPlaceVisitor<TSource, TKeep> : ExpressionVisitor
	{
		private readonly Type _lambdaToKeep;
		private readonly bool _withReplacement;
		private readonly ExtractExpressionFromLambdaVisitor<TKeep> _lambdaExpressionExtractor;
		private readonly ParameterReplacementVisitor _parameterReplacementVisitor;

		public KeepTypeInPlaceVisitor(ParameterExpression originalParameter, ParameterExpression newParameter, Dictionary<string, string> propertyRenaming, bool withReplacement)
		{
			_lambdaToKeep = typeof(Func<TKeep, bool>);
			_withReplacement = withReplacement;
			_lambdaExpressionExtractor = new ExtractExpressionFromLambdaVisitor<TKeep>(newParameter);
			_parameterReplacementVisitor = new ParameterReplacementVisitor(originalParameter, newParameter, propertyRenaming);
		}

		public Expression VisitRoot(Expression node)
		{
			return Visit(node);
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (IsCorrectCall(node))
			{
				return _lambdaExpressionExtractor.VisitRoot(node);
			}
			else
			{
				return base.VisitMethodCall(node);
			}
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			Expression left;
			Expression right;
			if (IsConditional(node.Left))
			{
				left = base.Visit(node.Left);
			}
			else if (IsCorrectCall(node.Left))
			{
				left = base.Visit(node.Left);
			}
			else if (HasExpressionWithCorrectType(node.Left))
			{
				if (_withReplacement)
				{

					left = _parameterReplacementVisitor.Visit(node.Left);
				}
				else
				{
					left = node.Left;
				}
			}
			else
			{
				left = GetReplacement(node.NodeType, node.Right);
			}

			if (IsConditional(node.Right))
			{
				right = base.Visit(node.Right);
			}
			else if (IsCorrectCall(node.Right))
			{
				right = base.Visit(node.Right);
			}
			else if (IsConstant(node.Right))
			{
				right = base.Visit(node.Right);
			}
			else if (HasExpressionWithCorrectType(node.Right))
			{
				if (_withReplacement)
				{
					right = _parameterReplacementVisitor.Visit(node.Right);
				}
				else
				{
					right = node.Right;
				}
			}
			else
			{
				right = GetReplacement(node.NodeType, left);
			}
			return Expression.MakeBinary(node.NodeType, left, right);
		}

		private bool IsConditional(Expression node)
		{
			return node.NodeType == ExpressionType.AndAlso || node.NodeType == ExpressionType.OrElse || node.NodeType == ExpressionType.Or || node.NodeType == ExpressionType.And || node.NodeType == ExpressionType.ExclusiveOr;
		}

		private bool IsCorrectCall(Expression node)
		{
			return node.NodeType == ExpressionType.Call && ((MethodCallExpression)node).Arguments.Any(arg => arg.Type == _lambdaToKeep);
		}

		private bool IsConstant(Expression node)
		{
			return node.NodeType == ExpressionType.Constant;
		}

		private bool HasExpressionWithCorrectType(Expression node)
		{
			return HasSubexpressionsOfType<TSource>(node);
		}

		private static Expression GetReplacement(ExpressionType nodeType, Expression otherNode)
		{
			switch (nodeType)
			{
				case ExpressionType.OrElse:
				case ExpressionType.Or:
				case ExpressionType.ExclusiveOr: return Expression.Constant(otherNode.NodeType == ExpressionType.Constant);
				case ExpressionType.AndAlso:
				case ExpressionType.And:
				default: return Expression.Constant(true);
			}
		}
	}
}