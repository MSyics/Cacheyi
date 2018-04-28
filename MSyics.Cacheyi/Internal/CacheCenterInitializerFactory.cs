/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MSyics.Cacheyi
{
    internal class CacheCenterInitializerFactory
    {
        private ParameterExpression ParaX = Expression.Parameter(typeof(CacheCenter), "x");
        private ParameterExpression ParaCenter;

        public Action<CacheCenter> Create(Type center)
        {
            ParaCenter = Expression.Parameter(center, "center");
            var body = Expression.Block(new[] { ParaCenter }, GetExpressions(center));
            var lamda = Expression.Lambda<Action<CacheCenter>>(body, ParaX);
            return lamda.Compile();
        }

        private IEnumerable<Expression> GetExpressions(Type center)
        {
            // $center = $x
            yield return Expression.Assign(ParaCenter, Expression.Convert(ParaX, center));
            // $center.[store] = (StoreType)object;
            var properties = center.GetTypeInfo()
                                   .DeclaredProperties
                                   .Where(x =>
                                   {
                                       var type = x.PropertyType.GetGenericTypeDefinition();
                                       return type.Equals(typeof(CacheStore<,>)) || type.Equals(typeof(CacheStore<,,>));
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