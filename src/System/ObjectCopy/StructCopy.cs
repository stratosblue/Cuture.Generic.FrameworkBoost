namespace System
{
    internal static class StructAllCopy<TSource, TDestination> where TDestination : struct
    {
        #region Private 字段

        private static readonly StructMemberCopyDelegate<TSource, TDestination> s_memberValueCopyAction
            = ObjectCopyUtil.CreateStructMemberCopyDelegate<TSource, TDestination>(ignoreCase: false, nonPublic: true);

        #endregion Private 字段

        #region Public 方法

        public static void Copy(ref TSource source, ref TDestination destination) => s_memberValueCopyAction(ref source, ref destination);

        #endregion Public 方法
    }

    internal static class StructCopy<TSource, TDestination> where TDestination : struct
    {
        #region Private 字段

        private static readonly StructMemberCopyDelegate<TSource, TDestination> s_memberValueCopyAction
            = ObjectCopyUtil.CreateStructMemberCopyDelegate<TSource, TDestination>(ignoreCase: false, nonPublic: false);

        #endregion Private 字段

        #region Public 方法

        public static void Copy(ref TSource source, ref TDestination destination) => s_memberValueCopyAction(ref source, ref destination);

        #endregion Public 方法
    }

    internal static class StructIgnoreCaseAllCopy<TSource, TDestination> where TDestination : struct
    {
        #region Private 字段

        private static readonly StructMemberCopyDelegate<TSource, TDestination> s_memberValueCopyAction
            = ObjectCopyUtil.CreateStructMemberCopyDelegate<TSource, TDestination>(ignoreCase: true, nonPublic: true);

        #endregion Private 字段

        #region Public 方法

        public static void Copy(ref TSource source, ref TDestination destination) => s_memberValueCopyAction(ref source, ref destination);

        #endregion Public 方法
    }

    internal static class StructIgnoreCaseCopy<TSource, TDestination> where TDestination : struct
    {
        #region Private 字段

        private static readonly StructMemberCopyDelegate<TSource, TDestination> s_memberValueCopyAction
            = ObjectCopyUtil.CreateStructMemberCopyDelegate<TSource, TDestination>(ignoreCase: true, nonPublic: false);

        #endregion Private 字段

        #region Public 方法

        public static void Copy(ref TSource source, ref TDestination destination) => s_memberValueCopyAction(ref source, ref destination);

        #endregion Public 方法
    }
}