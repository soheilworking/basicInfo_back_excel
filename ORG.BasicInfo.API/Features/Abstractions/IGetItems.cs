using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace ORG.BasicInfo.API.Features.Abstractions
{
    public static class QueryableExtensions
    {
        // Cache for generated LambdaExpressions keyed by "Type.FullName|PropertyName"
        private static readonly ConcurrentDictionary<string, LambdaExpression> _lambdaCache
            = new ConcurrentDictionary<string, LambdaExpression>();

        public static IQueryable<T> ApplySorting<T>(
            this IQueryable<T> source,
            IEnumerable<string[]> sortFields)
        {
            if (sortFields == null || !sortFields.Any())
                return source;

            var entityType = typeof(T);
            IOrderedQueryable<T> ordered = null!;

            foreach (var sf in sortFields)
            {
                if (sf.Length != 2)
                    throw new ArgumentException(
                        "هر آرایه‌ی sortFields باید دقیقاً دو عنصر داشته باشد: نام فیلد و جهت (asc|desc).");

                var field = sf[0];
                var dir = sf[1].ToLower();
                var cacheKey = $"{entityType.FullName}|{field}";

                // get or build Expression<Func<T, PropertyType>>
                var keySelector = (LambdaExpression)_lambdaCache.GetOrAdd(cacheKey, _ =>
                {
                    var prop = entityType.GetProperty(
                        field,
                        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (prop == null)
                        throw new ArgumentException(
                            $"فیلد '{field}' در موجودیت '{entityType.Name}' یافت نشد.");

                    var param = Expression.Parameter(entityType, "x");
                    var body = Expression.Property(param, prop);
                    return Expression.Lambda(body, param);
                });

                // choose method name
                var method = ordered == null
                    ? (dir == "desc" ? "OrderByDescending" : "OrderBy")
                    : (dir == "desc" ? "ThenByDescending" : "ThenBy");

                // get generic MethodInfo
                var mi = typeof(Queryable)
                    .GetMethods()
                    .First(m =>
                        m.Name == method
                        && m.GetParameters().Length == 2)
                    .MakeGenericMethod(entityType, keySelector.Body.Type);

                // invoke
                ordered = (IOrderedQueryable<T>)mi
                    .Invoke(null, new object[] { ordered ?? source, keySelector })!;
            }

            return ordered;
        }
    }
    public class ResponseQuery<TResponseList>
    {
        public IEnumerable<TResponseList> Items { get; set; }
        public ulong Counts { get; set; }
    }

     public abstract class IGetItems<TResponseList,TResponseInfo, TEntity, TDB>
        where TEntity : class
        where TDB : DbContext
    {
        public abstract  Task<ResponseQuery<TResponseList>>  SearchWithNameLike(string name, int pageNumber, int pageSize, IEnumerable<string[]> sortFields, TDB dbContext, CancellationToken cancellationToken);       
        public abstract Task<TResponseList> GetInfoWithIdCode(ulong idCode, TDB dbContext, CancellationToken cancellationToken);
        public abstract Task<TResponseInfo> GetInfoWithId(Guid id, TDB dbContext, CancellationToken cancellationToken);
        public abstract Task<ResponseQuery<TResponseList>> GetInfoAll(int pageNumber, int pageSize, IEnumerable<string[]> sortFields, TDB dbContext, CancellationToken cancellationToken);


        protected async Task<ResponseQuery<TResponseList>> paginationData<TEntityQuery>(
               int pageNumber,
              int pageSize,
              IQueryable<TEntityQuery> baseQuery,
           
              IEnumerable<string[]> sortFields,
              CancellationToken cancellationToken)
        {
            // 1. count without sorting
            var totalItems = await baseQuery
                
                .CountAsync(cancellationToken)
                
                .ConfigureAwait(false);

            // 2. apply dynamic sorting, then paging
            var pagedQuery = baseQuery
                .ApplySorting(sortFields)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // 3. fetch page
            var list = await pagedQuery
          
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            // 4. map or cast
            IEnumerable<TResponseList> items;
            if (typeof(TEntityQuery) == typeof(TResponseList))
            {
                items = list.Cast<TResponseList>();
            }
            else
            {
                items = list.Adapt<IEnumerable<TResponseList>>();
            }

            return new ResponseQuery<TResponseList>
            {
                Counts = (ulong)totalItems,
                Items = items
            };
        }

        protected async Task<RepeatedResult> ValidateUniqueAsync<TProperty>(
          Guid id,
          string fieldName,
          TProperty fieldValue,
          string entityDisplayName,
          TDB dbContext,
          CancellationToken cancellationToken
      )
        {
            // 1. دسترسی به DbSet<TEntity>
            var set = dbContext.Set<TEntity>().AsNoTracking();

            // 2. متد Where برای فیلتر on-the-fly (داخل EF)
            var parameter = Expression.Parameter(typeof(TEntity), "e");
            var efPropCall = Expression.Call(
                typeof(EF), nameof(EF.Property),
                new[] { typeof(TProperty) },
                parameter,
                Expression.Constant(fieldName)
            );
            var equalsFilter = Expression.Equal(
                efPropCall,
                Expression.Constant(fieldValue, typeof(TProperty))
            );
            var lambdaWhere = Expression.Lambda<Func<TEntity, bool>>(equalsFilter, parameter);

            // Create scenario: فقط Any
            if (id == Guid.Empty)
            {
                var exists = await set.AnyAsync(lambdaWhere, cancellationToken);
                if (exists)
                    return new RepeatedResult
                    {
                        StatusCode = 405,
                        Message = $"{entityDisplayName} با همین مقدار '{fieldName}' از قبل وجود دارد."
                    };
            }
            else
            {
                // 3. پیدا کردن رکورد جاری بر اساس کلید "Id"
                var idParameter = Expression.Parameter(typeof(TEntity), "e");
                var idPropCall = Expression.Call(
                    typeof(EF), nameof(EF.Property),
                    new[] { typeof(Guid) },
                    idParameter,
                    Expression.Constant("Id")
                );
                var equalsId = Expression.Equal(
                    idPropCall,
                    Expression.Constant(id, typeof(Guid))
                );
                var lambdaById = Expression.Lambda<Func<TEntity, bool>>(equalsId, idParameter);

                var entity = await set.FirstOrDefaultAsync(lambdaById, cancellationToken);
                if (entity == null)
                    return new RepeatedResult
                    {
                        StatusCode = 404,
                        Message = $"{entityDisplayName} مورد نظر یافت نشد."
                    };

                // 4. با Reflection مقدار فیلد را از شیء بخوان
                var propInfo = typeof(TEntity)
                    .GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (propInfo == null)
                    throw new ArgumentException($"فیلد '{fieldName}' در موجودیت '{typeof(TEntity).Name}' یافت نشد.");

                var currentValue = (TProperty)propInfo.GetValue(entity)!;

                // اگر تغییر کرده بود، دوباره Any
                if (!Equals(currentValue, fieldValue))
                {
                    var exists = await set.AnyAsync(lambdaWhere, cancellationToken);
                    if (exists)
                        return new RepeatedResult
                        {
                            StatusCode = 405,
                            Message = $"{entityDisplayName} با مقدار جدید '{fieldName}' تکراری است."
                        };
                }
            }

            return new RepeatedResult
            {
                StatusCode = 200,
                Message = string.Empty
            };
        }

    }
}
