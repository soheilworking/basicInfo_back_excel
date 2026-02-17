using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ORG.BasicInfo.Domain.FormAggregate;
using ORG.BasicInfo.Domain.FormUserAggregate;
using ORG.BasicInfo.Domain.UserAggregate;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace ORG.BasicInfo.Data
{
    public class FormsInfoDbContext(DbContextOptions<FormsInfoDbContext> options) : DbContext(options)
    {

        public DbSet<FormRawSys> FormRawSyss => Set<FormRawSys>();
        public DbSet<FilesRawSys> FilesRawSyss => Set<FilesRawSys>();
        public DbSet<User> Users => Set<User>();
        public DbSet<FormUserSys> FormUserSyss => Set<FormUserSys>();
        public DbSet<FilesFormsSys> FilesFormsSyss => Set<FilesFormsSys>();
        public DbSet<FormUserLogSys> FormUserLogSyss => Set<FormUserLogSys>();
        public DbSet<FormRawLogSys> FormRawLogSyss => Set<FormRawLogSys>();
        public DbSet<PermissionFund> PermissionFunds => Set<PermissionFund>();


        
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
                .Properties<string>()
                .AreUnicode(false)
                .HaveMaxLength(250);

            base.ConfigureConventions(configurationBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        // New async initialization method
        public async Task InitializeAsync()
        {
            // Example async initialization logic here
            var connection = Database.GetDbConnection();
            await connection.OpenAsync();

            // Perform your async initialization logic here
            // e.g., PositionsSysConfigurationObj = await PositionsSysConfiguration.Create(connection);

            //await connection.CloseAsync();
        }

        public override ChangeTracker ChangeTracker
        {
            get
            {
                base.ChangeTracker.LazyLoadingEnabled = false;
                return base.ChangeTracker;
            }
        }

        // حافظه‌ی کش برای Setterهای کامپایل‌شده
        private static readonly ConcurrentDictionary<string, object> _setterCache = new();

        /// <summary>
        /// BulkRead بر اساس کلیدها با بهینه‌سازی‌های زیر:
        /// - غیرفعال‌سازی AutoDetectChanges
        /// - استفاده از UseTempDB به جای BulkReadUseTempTable
        /// - Setter کامپایل‌شده
        /// </summary>
        public async Task<List<T>> BulkReadByKeysAsync<T, TKey>(
            IEnumerable<TKey> keys,
            Expression<Func<T, TKey>> keySelector,
            int batchSize = 5000)
            where T : class, new()
        {
            var result = new List<T>();
            var distinct = keys.Distinct().ToList();

            // 1. غیرفعال‌سازی موقت ChangeTracker
            ChangeTracker.AutoDetectChangesEnabled = false;

            // 2. آماده‌سازی Setter کامپایل‌شده از طریق Expression
            var memberExpr = (MemberExpression)keySelector.Body;
            var propInfo = (PropertyInfo)memberExpr.Member;
            var cacheKey = $"{typeof(T).FullName}.{propInfo.Name}";
            var setter = (Action<T, TKey>)_setterCache.GetOrAdd(cacheKey, _ =>
            {
                var objParam = Expression.Parameter(typeof(T), "obj");
                var valParam = Expression.Parameter(typeof(TKey), "val");
                var assign = Expression.Assign(
                    Expression.Property(objParam, propInfo),
                    Expression.Convert(valParam, propInfo.PropertyType));
                return Expression.Lambda<Action<T, TKey>>(assign, objParam, valParam).Compile();
            });

            // 3. کانفیگ به‌روز BulkConfig
            var bulkConfig = new BulkConfig
            {
                BatchSize = batchSize,
                UseTempDB = true,            // << اصلاح شده
                BulkCopyTimeout = 60,            // زمان تایم‌اوت دلخواه (ثانیه)
                CalculateStats = false,
            };

            // 4. پردازش در دسته‌های batchSize
            for (int i = 0; i < distinct.Count; i += batchSize)
            {
                var chunk = distinct.Skip(i).Take(batchSize).ToList();

                // مقداردهی اولیه‌ی موجودیت‌ها
                var entities = chunk
                    .Select(key =>
                    {
                        var e = new T();
                        setter(e, key);
                        return e;
                    })
                    .ToList();

                // فراخوانی BulkReadAsync با تنظیمات بهینه
                await this.BulkReadAsync(entities, bulkConfig);

                result.AddRange(entities);
            }

            // بازگرداندن AutoDetectChanges
            ChangeTracker.AutoDetectChangesEnabled = true;
            return result;
        }

        /// <summary>
        /// BulkWhere با AsNoTracking، تقسیم به batch و غیرفعال‌سازی AutoDetectChanges
        /// </summary>
        public async Task<List<T>> BulkWhereAsync<T, TKey>(
            IEnumerable<TKey> keys,
            Expression<Func<T, TKey>> keySelector,
            int batchSize = 5000)
            where T : class, new()
        {
            var result = new List<T>();
            var distinctKeys = keys.Distinct().ToList();

            // 1. unwrap any Convert/ConvertChecked
            Expression body = keySelector.Body;
            if (body is UnaryExpression unary &&
                (unary.NodeType == ExpressionType.Convert ||
                 unary.NodeType == ExpressionType.ConvertChecked))
            {
                body = unary.Operand;
            }

            // 2. آماده‌سازی فهرست پراپرتی‌ها و setter
            var propInfos = new List<PropertyInfo>();
            var propNames = new List<string>();
            Action<T, TKey> setter;

            if (body is MemberExpression memberExp)
            {
                var pi = (PropertyInfo)memberExp.Member;
                propInfos.Add(pi);
                propNames.Add(pi.Name);

                // متد set اگر وجود داشته باشد
                var setMethod = pi.GetSetMethod(true);

                // سعی می‌کنیم فیلد پشتیبان را پیدا کنیم
                FieldInfo backingField = null;
                if (setMethod == null)
                {
                    backingField = pi.DeclaringType
                        .GetField($"<{pi.Name}>k__BackingField",
                                  BindingFlags.Instance | BindingFlags.NonPublic);
                    if (backingField == null)
                        throw new InvalidOperationException(
                            $"Property '{pi.Name}' has no setter and no backing field.");
                }

                setter = (entity, key) =>
                {
                    if (setMethod != null)
                    {
                        // اگر setter موجود بود از آن استفاده می‌کنیم
                        setMethod.Invoke(entity, new object[] { key });
                    }
                    else
                    {
                        // در غیر این صورت روی فیلد پشتیبان مقدار می‌گذاریم
                        backingField.SetValue(entity, key);
                    }
                };
            }
            else if (body is NewExpression newExpr)
            {
                // حالت چندپراپرتی (Tuple یا anonymous new)
                // استخراج MemberExpressionهای داخل newExpr.Arguments
                for (int i = 0; i < newExpr.Arguments.Count; i++)
                {
                    if (newExpr.Arguments[i] is not MemberExpression argMem)
                        throw new ArgumentException(
                            $"قسمت {i} از keySelector باید مستقیم به یک پراپرتی اشاره کند.");

                    var pi = (PropertyInfo)argMem.Member;
                    propInfos.Add(pi);
                    propNames.Add(pi.Name);
                }

                // ساخت یک Action<T, TKey> که tuple را باز (unpack) کرده و مقادیر را ست می‌کند
                var entityParam = Expression.Parameter(typeof(T), "entity");
                var keyParam = Expression.Parameter(typeof(TKey), "key");
                var exprList = new List<Expression>();

                for (int i = 0; i < propInfos.Count; i++)
                {
                    var pi = propInfos[i];

                    // فرض بر این است که TKey یک ValueTuple است (Item1, Item2, …)
                    var field = typeof(TKey).GetField($"Item{i + 1}");
                    Expression valueExpr;

                    if (field != null)
                    {
                        // دسترسی به فیلد tuple.ItemX
                        valueExpr = Expression.Field(
                            Expression.Convert(keyParam, typeof(TKey)),
                            field);
                    }
                    else
                    {
                        // یا پراپرتی هم‌نام در TKey (مثل anonymous type)
                        var kp = typeof(TKey).GetProperty(pi.Name)
                            ?? throw new InvalidOperationException(
                                $"نمی‌توان پراپرتی '{pi.Name}' را در '{typeof(TKey).Name}' یافت.");
                        valueExpr = Expression.Property(
                            Expression.Convert(keyParam, typeof(TKey)),
                            kp);
                    }

                    // تبدیل نوع و ست کردن پراپرتی روی entity
                    var assign = Expression.Assign(
                        Expression.Property(entityParam, pi),
                        Expression.Convert(valueExpr, pi.PropertyType));

                    exprList.Add(assign);
                }

                setter = Expression
                    .Lambda<Action<T, TKey>>(
                        Expression.Block(exprList),
                        entityParam, keyParam)
                    .Compile();
            }
            else
            {
                throw new ArgumentException(
                    "keySelector باید یا یک پراپرتی ساده (p => p.Prop) " +
                    "یا مجموعه‌ای از پراپرتی‌ها (p => new (p.Prop1, p.Prop2)) باشد.",
                    nameof(keySelector));
            }

            // 3. صفحه‌بندی و اجرای BulkReadAsync
            for (int i = 0; i < distinctKeys.Count; i += batchSize)
            {
                var batch = distinctKeys.Skip(i).Take(batchSize).ToList();

                // مقداردهی اولیه موجودیت‌ها با setter
                var entities = batch
                    .Select(key =>
                    {
                        var e = new T();
                        setter(e, key);
                        return e;
                    })
                    .ToList();

                // خواندن دسته‌ای؛ فیلتر با propNames
                await this.BulkReadAsync(
                    entities,
                    new BulkConfig
                    {
                        UpdateByProperties = propNames,
                        SetOutputIdentity = false
                    });

                result.AddRange(entities);
            }

            return result;
        }
    }
}
