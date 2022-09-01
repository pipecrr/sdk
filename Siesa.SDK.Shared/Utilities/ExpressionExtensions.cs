using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Siesa.SDK.Shared.Utilities
{
    public class EfUtil
    {
        public static T ResultOf<T>(T value)
        {
            return value;
        }
    }
    //Note this could probably use a better name
    internal class ExpressionEvaluator : ExpressionVisitor
    {
        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.Name == "ResultOf" && m.Method.DeclaringType == typeof(EfUtil))
            {
                Expression target = m.Arguments[0];

                object result = Expression.Lambda(target)
                    .Compile()
                    .DynamicInvoke();

                return Expression.Constant(result, target.Type);
            }
            else
                return base.VisitMethodCall(m);
        }
    }

    public static class ExpressionExtensions
    {
        public static Expression Simplify(this Expression expression)
        {
            var searcher = new ParameterlessExpressionSearcher();
            searcher.Visit(expression);
            return new ParameterlessExpressionEvaluator(searcher.ParameterlessExpressions).Visit(expression);
        }

        public static IQueryable<T> EvaluateResults<T>(this IQueryable<T> query)
        {
            return query.Provider.CreateQuery<T>(
                new ExpressionEvaluator().Visit(query.Expression));
        }


        public static Expression<T> Simplify<T>(this Expression<T> expression)
        {
            return (Expression<T>)Simplify((Expression)expression);
        }

        private class ParameterlessExpressionSearcher : ExpressionVisitor
        {
            public HashSet<Expression> ParameterlessExpressions { get; } = new HashSet<Expression>();
            private bool containsParameter = false;

            public override Expression Visit(Expression node)
            {
                bool originalContainsParameter = containsParameter;
                containsParameter = false;
                base.Visit(node);
                if (!containsParameter)
                {
                    if (node?.NodeType == ExpressionType.Parameter)
                        containsParameter = true;
                    else
                        ParameterlessExpressions.Add(node);
                }
                containsParameter |= originalContainsParameter;

                return node;
            }
        }
        private class ParameterlessExpressionEvaluator : ExpressionVisitor
        {
            private HashSet<Expression> parameterlessExpressions;
            public ParameterlessExpressionEvaluator(HashSet<Expression> parameterlessExpressions)
            {
                this.parameterlessExpressions = parameterlessExpressions;
            }
            public override Expression Visit(Expression node)
            {
                if (parameterlessExpressions.Contains(node))
                    return Evaluate(node);
                else
                    return base.Visit(node);
            }

            private Expression Evaluate(Expression node)
            {
                if (node.NodeType == ExpressionType.Constant)
                {
                    return node;
                }
                object value = Expression.Lambda(node).Compile().DynamicInvoke();
                return Expression.Constant(value, node.Type);
            }
        }

    }
}