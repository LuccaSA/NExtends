using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NExtends.Expressions
{
	public class ExpressionReducer<TSource, TProp> : ExpressionVisitor
	{
		MemberExpression Reducer { get; set; }
		ParameterExpression Parameter { get; set; }

		bool CurrentNodeDependsOnPreviousParameter { get; set; }

		protected ExpressionReducer(MemberExpression reducer, ParameterExpression parameter)
		{
			Parameter = parameter;
			Reducer = reducer;

			if (Reducer.Member.DeclaringType != typeof(TSource)
				&& Reducer.Member.DeclaringType.IsAssignableFrom(typeof(TSource)))
			{
				PropertyInfo targetProperty;
				if (Reducer.Member.DeclaringType.IsInterface)
				{
					targetProperty = GetClassProperty(Reducer.Member.DeclaringType, typeof(TSource), Reducer.Member.Name);
				}
				else
				{
					targetProperty = typeof(TSource).GetProperty(Reducer.Member.Name);
				}
				Reducer = Expression.Property(Reducer.Expression as ParameterExpression, targetProperty);
			}
		}

		static PropertyInfo GetClassProperty(Type interfaceType, Type classType, string propertyName)
		{
			var nameProperty = interfaceType.GetProperty(propertyName);
			var mapping = classType.GetInterfaceMap(interfaceType);
			var nameGetter = nameProperty.GetGetMethod();

			MethodInfo targetMethod = null;
			for (var i = 0; i < mapping.InterfaceMethods.Length; i++)
			{
				if (mapping.InterfaceMethods[i] == nameGetter)
				{
					targetMethod = mapping.TargetMethods[i];
					break;
				}
			}

			PropertyInfo targetProperty = null;
			foreach (var property in classType.GetProperties(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.NonPublic))
			{
				if (targetMethod == property.GetGetMethod(true))
				{
					targetProperty = property;
					break;
				}
			}

			return targetProperty;
		}

		public static Expression<Func<TProp, bool>> Reduce(Expression<Func<TSource, bool>> expression, Expression<Func<TSource, TProp>> reducer, bool defaultValue)
		{
			var member = reducer.Body as MemberExpression;
			if (member == null)
				throw new ArgumentException("Ce converter ne fonctionne qu'avec des membres directs. ex : tSource => tSource.Prop");

			var param = Expression.Parameter(typeof(TProp), reducer.Parameters[0].Name);
			var expressionReducer = new ExpressionReducer<TSource, TProp>(member, param);
			var result = expressionReducer.Visit(expression.Body);

			//si on dépend du paramètre d'entrée en sortie, on renvoie plutôt la valeur par défaut
			if (expressionReducer.CurrentNodeDependsOnPreviousParameter)
			{
				result = Expression.Constant(defaultValue);
			}

			return Expression.Lambda<Func<TProp, bool>>(result, param);
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			var parameterDependancy = CurrentNodeDependsOnPreviousParameter;

			CurrentNodeDependsOnPreviousParameter = false;
			var left = Visit(node.Left);
			var leftIsObsolete = CurrentNodeDependsOnPreviousParameter;

			CurrentNodeDependsOnPreviousParameter = false;
			var right = Visit(node.Right);
			var rightIsObsolete = CurrentNodeDependsOnPreviousParameter;

			//opérateur boolean
			if (node.Type == typeof(bool) && left.Type == typeof(bool) && right.Type == typeof(bool))
			{
				//on vire les bras pas utiles
				CurrentNodeDependsOnPreviousParameter = parameterDependancy;

				//on ne réduit pas les constantes, car c'est risqué : 
				//false || obsolete !=> false     et      true && obsolete !=> true
				//on attend le niveau supérieur pour virer la branche si possible
				if (!leftIsObsolete && rightIsObsolete && left.NodeType != ExpressionType.Constant) return left;
				if (!rightIsObsolete && leftIsObsolete && right.NodeType != ExpressionType.Constant) return right;
			}

			CurrentNodeDependsOnPreviousParameter = parameterDependancy || leftIsObsolete || rightIsObsolete;

			return Expression.MakeBinary(node.NodeType, left, right);
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			//on sauvegarde la dépendance au paramètre
			var parameterDependancy = CurrentNodeDependsOnPreviousParameter;

			//on veut savoir si le noeud courant dépend du paramètre
			CurrentNodeDependsOnPreviousParameter = false;
			var result = base.VisitMember(node);

			//Même membre, branché sur le paramètre => on remplace
			if (CurrentNodeDependsOnPreviousParameter && AreEquals(node.Member, Reducer.Member))
			{
				CurrentNodeDependsOnPreviousParameter = false;
				result = Parameter;
			}

			//on réinitialise la dépendance au paramètre
			CurrentNodeDependsOnPreviousParameter = parameterDependancy || CurrentNodeDependsOnPreviousParameter;
			return result;
		}

        private bool AreEquals(MemberInfo member1, MemberInfo member2)
        {
            return member1.DeclaringType == member2.DeclaringType
                && member1.Name == member2.Name;
        }


        protected override Expression VisitParameter(ParameterExpression node)
		{
			//on sauvegarde la dépendance du noeud courant au paramètre d'entrée
			CurrentNodeDependsOnPreviousParameter = true;
			return base.VisitParameter(node);
		}
	}
}