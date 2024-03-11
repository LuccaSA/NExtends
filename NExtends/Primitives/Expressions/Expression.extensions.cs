using System;
using System.Linq.Expressions;

namespace NExtends.Primitives.Expressions
{
    public static class ExpressionExtensions
    {
        /// <summary>
        /// http://stackoverflow.com/questions/13705394/how-to-make-a-predicatebuilder-not
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
            => Expression.Lambda<Func<T, bool>>(Expression.Not(expression.Body), expression.Parameters);
    }
}
