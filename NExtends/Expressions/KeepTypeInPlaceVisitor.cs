using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static NExtends.Expressions.ExpressionHelper;

namespace NExtends.Expressions
{
	public class KeepTypeInPlaceVisitor<TSource, TKeep> : ExpressionVisitor
    {
        private Type _LambdaToKeep;
        private ParameterExpression _NewParameter = null;
        private bool _WithReplacement = false;
        private ExtractExpressionFromLambdaVisitor<TKeep> _LambdaExpressionExtractor = null;
        private ParameterReplacementVisitor _ParameterReplacementVisitor = null;

        public KeepTypeInPlaceVisitor(ParameterExpression originalParameter, ParameterExpression newParameter, Dictionary<string, string> propertyRenaming, bool withReplacement)
        {
            _LambdaToKeep = typeof(Func<TKeep, bool>);
            _NewParameter = newParameter;
            _WithReplacement = withReplacement;
            _LambdaExpressionExtractor = new ExtractExpressionFromLambdaVisitor<TKeep>(_NewParameter);
			_ParameterReplacementVisitor = new ParameterReplacementVisitor(originalParameter, _NewParameter, propertyRenaming);
        }
        public Expression VisitRoot(Expression node)
        {
            var visited = Visit(node);
            return visited;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (IsCorrectCall(node))
            {
                return _LambdaExpressionExtractor.VisitRoot(node);
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
                if (_WithReplacement)
                {

                    left = _ParameterReplacementVisitor.Visit(node.Left);
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
                if (_WithReplacement)
                {
                    right = _ParameterReplacementVisitor.Visit(node.Right);
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
            return node.NodeType == ExpressionType.Call && ((MethodCallExpression)node).Arguments.Any(arg => arg.Type == _LambdaToKeep);
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
                case ExpressionType.ExclusiveOr:
                    if (otherNode.NodeType == ExpressionType.Constant)
                    {
                        return Expression.Constant(true);
                    }
                    else
                    {
                        return Expression.Constant(false);
                    }
                case ExpressionType.AndAlso:
                case ExpressionType.And:
                default:
                    return Expression.Constant(true);
            }
        }

    }
}
