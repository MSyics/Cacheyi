/****************************************************************
© 2017 MSyics
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
        private ParameterExpression m_paraX = Expression.Parameter(typeof(CacheCenter), "x");
        private ParameterExpression m_paraCenter;

        public Action<CacheCenter> Create(CacheCenter center)
        {
            m_paraCenter = Expression.Parameter(center.GetType(), "center");
            var body = Expression.Block(new[] { m_paraCenter }, GetExpressions(center));
            var lamda = Expression.Lambda<Action<CacheCenter>>(body, m_paraX);
            return lamda.Compile();
        }

        private IEnumerable<Expression> GetExpressions(CacheCenter center)
        {
            // $center = $x
            yield return Expression.Assign(m_paraCenter, Expression.Convert(m_paraX, center.Context.CenterType));
            // $center.[store] = (StoreType)object;
            var properties = center.Context
                                   .CenterType
                                   .GetTypeInfo()
                                   .DeclaredProperties
                                   .Where(x => x.PropertyType.GetGenericTypeDefinition().Equals(typeof(CacheStore<,>)));
            foreach (var item in properties)
            {
                yield return
                    Expression.Assign(
                        Expression.Property(m_paraCenter, item),
                        Expression.Convert(
                            Expression.Constant(
                                center.Context.StoreInstanceNamedMapping.Get(center.Context.CenterType.FullName, item.Name),
                                typeof(object)),
                            item.PropertyType));
            }
        }
    }
}
