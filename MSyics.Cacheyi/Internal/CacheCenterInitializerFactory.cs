using System.Linq.Expressions;
using System.Reflection;

namespace MSyics.Cacheyi;

internal class CacheCenterInitializerFactory
{
    private readonly ParameterExpression paraX = Expression.Parameter(typeof(object), "x");
    private ParameterExpression paraCenter;

    public Action<object> Create(Type center)
    {
        paraCenter = Expression.Parameter(center, "center");
        var body = Expression.Block(new[] { paraCenter }, GetExpressions(center));
        var lamda = Expression.Lambda<Action<object>>(body, paraX);
        return lamda.Compile();
    }

    private IEnumerable<Expression> GetExpressions(Type center)
    {
        // $center = $x
        yield return Expression.Assign(paraCenter, Expression.Convert(paraX, center));
        // $center.[store] = (StoreType)object;
        var properties = center.
            GetTypeInfo().
            DeclaredProperties.
            Where(x =>
            {
                if (!x.PropertyType.IsGenericType) return false;
                var type = x.PropertyType.GetGenericTypeDefinition();
                return type.Equals(typeof(ICacheStore<,>))
                    || type.Equals(typeof(ICacheStore<,,>))
                    || type.Equals(typeof(CacheStore<,>))
                    || type.Equals(typeof(CacheStore<,,>))
                    || type.Equals(typeof(IAsyncCacheStore<,>))
                    || type.Equals(typeof(IAsyncCacheStore<,,>))
                    || type.Equals(typeof(AsyncCacheStore<,>))
                    || type.Equals(typeof(AsyncCacheStore<,,>));
            });

        foreach (var item in properties)
        {
            yield return
                Expression.Assign(
                    Expression.Property(paraCenter, item),
                    Expression.Convert(
                        Expression.Constant(
                            CacheCenter.stores.GetValue($"{center.FullName}.{item.Name}"),
                            typeof(object)),
                        item.PropertyType));
        }
    }
}
