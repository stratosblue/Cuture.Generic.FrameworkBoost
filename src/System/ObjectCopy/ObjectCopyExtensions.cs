namespace System
{
    /// <summary>
    /// 对象Copy拓展
    /// </summary>
    public static class ObjectCopyExtensions
    {
        #region Public 方法

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