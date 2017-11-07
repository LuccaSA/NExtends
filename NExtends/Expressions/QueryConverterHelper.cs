using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NExtends.Expressions
{
	public static class QueryConverterHelper
    {
        public static Expression<Func<TKeep, bool>> KeepExpressionsOfTypeInTree<TSource, TKeep>(Expression<Func<TSource, bool>> original, Dictionary<string, string> propertyRenaming, bool withReplacement)
        {
            var parameter = Expression.Parameter(typeof(TKeep));
            var visitor = new KeepTypeInPlaceVisitor<TSource, TKeep>(original.Parameters[0], parameter, propertyRenaming, withReplacement);
            var body = visitor.VisitRoot(original.Body);
            return Expression.Lambda<Func<TKeep, bool>>(body, parameter);
        }
    }
}
