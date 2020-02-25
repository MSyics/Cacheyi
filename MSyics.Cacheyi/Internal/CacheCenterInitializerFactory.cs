using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MSyics.Cacheyi
{
    internal class CacheCenterInitializerFactory
    {
        private readonly ParameterExpression ParaX = Expression.Parameter(typeof(object), "x");
        private ParameterExpression ParaCenter;

        public Action<object> Create(Type center)
        {
            ParaCenter = Expression.Parameter(center, "center");
            var body = Expression.Block(new[] { ParaCenter }, GetExpressions(center));
            var lamda = Expression.Lambda<Action<object>>(body, ParaX);
            return lamda.Compile();
        }

        private IEnumerable<Expression> GetExpressions(Type center)
        {
            // $center = $x
            yield return Expression.Assign(ParaCenter, Expression.Convert(ParaX, center));
            // $center.[store] = (StoreType)object;
            var properties = center.
                GetTypeInfo().
                DeclaredProperties.
                Where(x =>
                {
                    if (!x.PropertyType.IsGenericType) return false;
                    var type = x.PropertyType.GetGenericTypeDefinition();
                    return type.Equals(typeof(ICacheStore<,>)) || type.Equals(typeof(ICacheStore<,,>)) || type.Equals(typeof(CacheStore<,>)) || type.Equals(typeof(CacheStore<,,>));
                });

            var context = new CacheContext();
            foreach (var item in properties)
            {
                yield return
                    Expression.Assign(
                        Expression.Property(ParaCenter, item),
                        Expression.Convert(
                            Expression.Constant(
                                context.Stores.GetValue($"{center.FullName}.{item.Name}"),
                                typeof(object)),
                            item.PropertyType));
            }
        }
    }
}