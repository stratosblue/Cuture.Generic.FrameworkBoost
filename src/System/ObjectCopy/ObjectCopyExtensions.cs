namespace System
{
    /// <summary>
    /// 对象Copy拓展
    /// </summary>
    public static class ObjectCopyExtensions
    {
        #region Public 方法

        /// <summary>
        /// 将源结构体的字段、属性赋值到目标对象的同名、同类型的字段、属性
        /// <para/>
        /// 引用传递 <paramref name="source"/> 避免结构体复制，不会改变其值。
        /// </summary>
        /// <typeparam name="TSource">源结构体类型</typeparam>
        /// <typeparam name="TDestination">目标对象类型</typeparam>
        /// <param name="destination">目标对象</param>
        /// <param name="source">源结构体</param>
        public static void CopyFrom<TSource, TDestination>(this TDestination destination, ref TSource source) where TSource : struct where TDestination : class
        {
            if (destination is null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            StructToObjectCopy<TSource, TDestination>.Copy(source, destination);
        }

        /// <summary>
        /// 将源结构体的字段、属性赋值到目标对象的同名、同类型的字段、属性
        /// <para/>
        /// 引用传递 <paramref name="source"/> 避免结构体复制，不会改变其值。
        /// </summary>
        /// <typeparam name="TSource">源结构体类型</typeparam>
        /// <typeparam name="TDestination">目标对象类型</typeparam>
        /// <param name="destination">目标对象</param>
        /// <param name="source">源结构体</param>
        /// <param name="ignoreCase">忽略名称大小写</param>
        /// <param name="nonPublic">是否复制私有字段、属性</param>
        public static void CopyFrom<TSource, TDestination>(this TDestination destination,
                                                           ref TSource source,
                                                           bool ignoreCase = false,
                                                           bool nonPublic = false) where TSource : struct where TDestination : class
        {
            if (destination is null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (ignoreCase)
            {
                if (nonPublic)
                {
                    StructToObjectIgnoreCaseAllCopy<TSource, TDestination>.Copy(source, destination);
                    return;
                }
                StructToObjectIgnoreCaseCopy<TSource, TDestination>.Copy(source, destination);
                return;
            }

            if (nonPublic)
            {
                StructToObjectAllCopy<TSource, TDestination>.Copy(source, destination);
                return;
            }
            StructToObjectCopy<TSource, TDestination>.Copy(source, destination);
        }

        /// <inheritdoc cref="ObjectCopyExtensions.CopyTo{TSource, TDestination}(TSource, TDestination)"/>
        public static TDestination CopyTo<TSource, TDestination>(this TSource source) where TSource : class where TDestination : class, new()
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            TDestination destination = new();

            ObjectCopy<TSource, TDestination>.Copy(source, destination);

            return destination;
        }

        /// <summary>
        /// 将源对象的字段、属性赋值到目标对象的同名、同类型的字段、属性
        /// </summary>
        /// <typeparam name="TSource">源对象类型</typeparam>
        /// <typeparam name="TDestination">目标对象类型</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="destination">目标对象</param>
        public static void CopyTo<TSource, TDestination>(this TSource source, TDestination destination) where TSource : class where TDestination : class
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destination is null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            ObjectCopy<TSource, TDestination>.Copy(source, destination);
        }

        /// <inheritdoc cref="ObjectCopyExtensions.CopyTo{TSource, TDestination}(TSource, TDestination, bool, bool)"/>
        public static TDestination CopyTo<TSource, TDestination>(this TSource source,
                                                                 bool ignoreCase = false,
                                                                 bool nonPublic = false) where TSource : class where TDestination : class, new()
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            TDestination destination = new();

            source.CopyTo(destination, ignoreCase: ignoreCase, nonPublic: nonPublic);

            return destination;
        }

        /// <summary>
        /// 将源对象的字段、属性赋值到目标对象的同名、同类型的字段、属性
        /// </summary>
        /// <typeparam name="TSource">源对象类型</typeparam>
        /// <typeparam name="TDestination">目标对象类型</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="destination">目标对象</param>
        /// <param name="ignoreCase">忽略名称大小写</param>
        /// <param name="nonPublic">是否复制私有字段、属性</param>
        public static void CopyTo<TSource, TDestination>(this TSource source,
                                                         TDestination destination,
                                                         bool ignoreCase = false,
                                                         bool nonPublic = false) where TSource : class where TDestination : class
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destination is null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (ignoreCase)
            {
                if (nonPublic)
                {
                    ObjectIgnoreCaseAllCopy<TSource, TDestination>.Copy(source, destination);
                    return;
                }
                ObjectIgnoreCaseCopy<TSource, TDestination>.Copy(source, destination);
                return;
            }

            if (nonPublic)
            {
                ObjectAllCopy<TSource, TDestination>.Copy(source, destination);
                return;
            }
            ObjectCopy<TSource, TDestination>.Copy(source, destination);
        }

        #endregion Public 方法
    }
}