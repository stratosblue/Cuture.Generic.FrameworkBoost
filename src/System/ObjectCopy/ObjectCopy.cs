namespace System
{
    internal static class ObjectAllCopy<TSource, TDestination> where TSource : class where TDestination : class
    {
        #region Private 字段

        private static readonly Action<TSource, TDestination> s_memberValueCopyAction
            = ObjectCopyUtil.CreateMemberCopyDelegate<TSource, TDestination>(ignoreCase: false, nonPublic: true);

        #endregion Private 字段

        #region Public 方法

        public static void Copy(TSource source, TDestination destination) => s_memberValueCopyAction(source, destination);

        #endregion Public 方法
    }

    internal static class ObjectCopy<TSource, TDestination> where TSource : class where TDestination : class
    {
        #region Private 字段

        private static readonly Action<TSource, TDestination> s_memberValueCopyAction
            = ObjectCopyUtil.CreateMemberCopyDelegate<TSource, TDestination>(ignoreCase: false, nonPublic: false);

        #endregion Private 字段

        #region Public 方法

        public static void Copy(TSource source, TDestination destination) => s_memberValueCopyAction(source, destination);

        #endregion Public 方法
    }

    internal static class ObjectIgnoreCaseAllCopy<TSource, TDestination> where TSource : class where TDestination : class
    {
        #region Private 字段

        private static readonly Action<TSource, TDestination> s_memberValueCopyAction
            = ObjectCopyUtil.CreateMemberCopyDelegate<TSource, TDestination>(ignoreCase: true, nonPublic: true);

        #endregion Private 字段

        #region Public 方法

        public static void Copy(TSource source, TDestination destination) => s_memberValueCopyAction(source, destination);

        #endregion Public 方法
    }

    internal static class ObjectIgnoreCaseCopy<TSource, TDestination> where TSource : class where TDestination : class
    {
        #region Private 字段

        private static readonly Action<TSource, TDestination> s_memberValueCopyAction
            = ObjectCopyUtil.CreateMemberCopyDelegate<TSource, TDestination>(ignoreCase: true, nonPublic: false);

        #endregion Private 字段

        #region Public 方法

        public static void Copy(TSource source, TDestination destination) => s_memberValueCopyAction(source, destination);

        #endregion Public 方法
    }

    #region Struct

    internal static class StructToObjectAllCopy<TSource, TDestination> where TSource : struct where TDestination : class
    {
        #region Private 字段

        private static readonly StructToObjectMemberCopyDelegate<TSource, TDestination> s_memberValueCopyAction
            = ObjectCopyUtil.CreateStructToObjectMemberCopyDelegate<TSource, TDestination>(ignoreCase: false, nonPublic: true);

        #endregion Private 字段

        #region Public 方法

        public static void Copy(TSource source, TDestination destination) => s_memberValueCopyAction(ref source, destination);

        #endregion Public 方法
    }

    internal static class StructToObjectCopy<TSource, TDestination> where TSource : struct where TDestination : class
    {
        #region Private 字段

        private static readonly StructToObjectMemberCopyDelegate<TSource, TDestination> s_memberValueCopyAction
            = ObjectCopyUtil.CreateStructToObjectMemberCopyDelegate<TSource, TDestination>(ignoreCase: false, nonPublic: false);

        #endregion Private 字段

        #region Public 方法

        public static void Copy(TSource source, TDestination destination) => s_memberValueCopyAction(ref source, destination);

        #endregion Public 方法
    }

    internal static class StructToObjectIgnoreCaseAllCopy<TSource, TDestination> where TSource : struct where TDestination : class
    {
        #region Private 字段

        private static readonly StructToObjectMemberCopyDelegate<TSource, TDestination> s_memberValueCopyAction
            = ObjectCopyUtil.CreateStructToObjectMemberCopyDelegate<TSource, TDestination>(ignoreCase: true, nonPublic: true);

        #endregion Private 字段

        #region Public 方法

        public static void Copy(TSource source, TDestination destination) => s_memberValueCopyAction(ref source, destination);

        #endregion Public 方法
    }

    internal static class StructToObjectIgnoreCaseCopy<TSource, TDestination> where TSource : struct where TDestination : class
    {
        #region Private 字段

        private static readonly StructToObjectMemberCopyDelegate<TSource, TDestination> s_memberValueCopyAction
            = ObjectCopyUtil.CreateStructToObjectMemberCopyDelegate<TSource, TDestination>(ignoreCase: true, nonPublic: false);

        #endregion Private 字段

        #region Public 方法

        public static void Copy(TSource source, TDestination destination) => s_memberValueCopyAction(ref source, destination);

        #endregion Public 方法
    }

    #endregion Struct
}