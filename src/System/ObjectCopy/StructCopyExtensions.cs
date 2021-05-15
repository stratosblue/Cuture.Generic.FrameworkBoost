namespace System
{
    /// <summary>
    /// 结构体Copy拓展
    /// </summary>
    public static class StructCopyExtensions
    {
        #region Public 方法

        /// <summary>
        /// 将源结构体的字段、属性赋值到目标结构体的同名、同类型的字段、属性
        /// </summary>
        /// <typeparam name="TSource">源结构体类型</typeparam>
        /// <typeparam name="TDestination">目标结构体类型</typeparam>
        /// <param name="source">源结构体</param>
        /// <param name="destination">目标结构体</param>
        public static void CopyTo<TSource, TDestination>(this TSource source, ref TDestination destination) where TDestination : struct
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            StructCopy<TSource, TDestination>.Copy(ref source, ref destination);
        }

        /// <summary>
        /// 将源结构体的字段、属性赋值到目标结构体的同名、同类型的字段、属性
        /// </summary>
        /// <typeparam name="TSource">源结构体类型</typeparam>
        /// <typeparam name="TDestination">目标结构体类型</typeparam>
        /// <param name="source">源结构体</param>
        /// <param name="destination">目标结构体</param>
        /// <param name="ignoreCase">忽略名称大小写</param>
        /// <param name="nonPublic">是否复制私有字段、属性</param>
        public static void CopyTo<TSource, TDestination>(this TSource source,
                                                         ref TDestination destination,
                                                         bool ignoreCase = false,
                                                         bool nonPublic = false) where TDestination : struct
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (ignoreCase)
            {
                if (nonPublic)
                {
                    StructIgnoreCaseAllCopy<TSource, TDestination>.Copy(ref source, ref destination);
                    return;
                }
                StructIgnoreCaseCopy<TSource, TDestination>.Copy(ref source, ref destination);
                return;
            }

            if (nonPublic)
            {
                StructAllCopy<TSource, TDestination>.Copy(ref source, ref destination);
                return;
            }
            StructCopy<TSource, TDestination>.Copy(ref source, ref destination);
        }

        #endregion Public 方法
    }
}