using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NExtends.Expressions
{
	public class ExtractExpressionFromLambdaVisitor<TExtract> : ExpressionVisitor
    {
        private Expression _ExtractedBody = null;
        private ParameterExpression _NewParameter = null;
        public ExtractExpressionFromLambdaVisitor(ParameterExpression newParameter)
        {
            _NewParameter = newParameter;
        }
        public Expression VisitRoot(Expression node)
        {
            _ExtractedBody = null;
            return Visit(node);
        }
        public override Expression Visit(Expression node)
        {
            if (_ExtractedBody != null) { return _ExtractedBody; }
            return base.Visit(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var lambda = (LambdaExpression)node.Arguments.FirstOrDefault(arg => arg.NodeType == ExpressionType.Lambda);
            if (lambda == null) { return base.VisitMethodCall(node); }
            if (lambda.Parameters.Any(p => p.Type == typeof(TExtract)))
            {
                var replacementVisitor = new ExpressionHelper.ParameterReplacementVisitor(lambda.Parameters.First(p => p.Type == typeof(TExtract)), _NewParameter, new Dictionary<string, string>());
                _ExtractedBody = replacementVisitor.Visit(lambda.Body);
                return _ExtractedBody;
            }
            return base.VisitMethodCall(node);
        }
    }
}
