using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NExtends.Expressions
{
	public class ExtractExpressionFromLambdaVisitor<TExtract> : ExpressionVisitor
    {
        private readonly ParameterExpression _newParameter;
        private Expression _extractedBody = null;

        public ExtractExpressionFromLambdaVisitor(ParameterExpression newParameter)
        {
            _newParameter = newParameter;
        }

        public Expression VisitRoot(Expression node)
        {
            _extractedBody = null;
            return Visit(node);
        }
        public override Expression Visit(Expression node)
        {
            if (_extractedBody != null) { return _extractedBody; }
            return base.Visit(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var lambda = (LambdaExpression)node.Arguments.FirstOrDefault(arg => arg.NodeType == ExpressionType.Lambda);
            if (lambda == null) { return base.VisitMethodCall(node); }
            if (lambda.Parameters.Any(p => p.Type == typeof(TExtract)))
            {
                var replacementVisitor = new ExpressionHelper.ParameterReplacementVisitor(lambda.Parameters.First(p => p.Type == typeof(TExtract)), _newParameter, new Dictionary<string, string>());
                _extractedBody = replacementVisitor.Visit(lambda.Body);
                return _extractedBody;
            }
            return base.VisitMethodCall(node);
        }
    }
}
