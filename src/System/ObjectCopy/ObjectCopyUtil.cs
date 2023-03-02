using System.Reflection;
using System.Reflection.Emit;

namespace System;

/// <summary>
/// 结构体成员复制委托
/// </summary>
/// <typeparam name="TSource"></typeparam>
/// <typeparam name="TDestination"></typeparam>
/// <param name="source"></param>
/// <param name="destination"></param>
public delegate void StructMemberCopyDelegate<TSource, TDestination>(ref TSource source, ref TDestination destination);

/// <summary>
/// 结构体成员复制到对象委托
/// </summary>
/// <typeparam name="TSource"></typeparam>
/// <typeparam name="TDestination"></typeparam>
/// <param name="source"></param>
/// <param name="destination"></param>

public delegate void StructToObjectMemberCopyDelegate<TSource, TDestination>(ref TSource source, TDestination destination) where TSource : struct where TDestination : class;

/// <summary>
/// 对象复制工具
/// </summary>
public static class ObjectCopyUtil
{
    #region Public 方法

    /// <summary>
    /// 创建对象复制委托
    /// <para/>
    /// 委托将源对象的字段、属性赋值到目标对象的同名、同类型的字段、属性
    /// </summary>
    /// <typeparam name="TSource">源对象类型</typeparam>
    /// <typeparam name="TDestination">目标对象类型</typeparam>
    /// <param name="ignoreCase">忽略名称大小写</param>
    /// <param name="nonPublic">是否复制私有字段、属性</param>
    /// <returns></returns>
    public static Action<TSource, TDestination> CreateMemberCopyDelegate<TSource, TDestination>(bool ignoreCase, bool nonPublic) where TSource : class where TDestination : class
    {
        return CreateMemberCopyDelegate<Action<TSource, TDestination>>(typeof(TSource), typeof(TDestination), ignoreCase: ignoreCase, nonPublic: nonPublic);
    }

    /// <summary>
    /// 创建对象复制委托
    /// <para/>
    /// 委托将源对象的字段、属性赋值到目标对象的同名、同类型的字段、属性
    /// </summary>
    /// <typeparam name="TDelegate">创建的委托类型</typeparam>
    /// <param name="sourceType">源对象类型</param>
    /// <param name="destinationType">目标对象类型</param>
    /// <param name="ignoreCase">忽略名称大小写</param>
    /// <param name="nonPublic">是否复制私有字段、属性</param>
    /// <returns></returns>
    public static TDelegate CreateMemberCopyDelegate<TDelegate>(Type sourceType,
                                                                Type destinationType,
                                                                bool ignoreCase,
                                                                bool nonPublic) where TDelegate : Delegate
    {
        var methodName = $"ObjectCopy_{sourceType.Name}_to_{destinationType.Name}_{ignoreCase}_{nonPublic}_{Guid.NewGuid():n}";

        var parameterTypes = CheckMemberCopyDelegateInfo<TDelegate>();

        Action<ILGenerator> Ldarg0 = IsParameterRef(parameterTypes[0]) & sourceType.IsClass ? Ldarg_0_Ref : Ldarg_0;
        Action<ILGenerator> Ldarg1 = IsParameterRef(parameterTypes[1]) & destinationType.IsClass ? Ldarg_1_Ref : Ldarg_1;

        var dynamicMethod = new DynamicMethod(methodName,
                                              MethodAttributes.Static | MethodAttributes.Public,
                                              CallingConventions.Standard,
                                              typeof(void),
                                              parameterTypes,
                                              typeof(ObjectCopyUtil),
                                              true);

        var ilGenerator = dynamicMethod.GetILGenerator();

        var bindingFlags = BindingFlags.Public | BindingFlags.Instance;

        if (nonPublic)
        {
            bindingFlags |= BindingFlags.NonPublic;
        }

        var sourceProperties = sourceType.GetProperties(bindingFlags)
                                         .Where(m => m.CanRead)
                                         .ToDictionary(m => FormatName(m.Name), m => m);

        var sourceFields = sourceType.GetFields(bindingFlags)
                                     .ToDictionary(m => FormatName(m.Name), m => m);

        foreach (var property in destinationType.GetProperties(bindingFlags).Where(m => m.CanWrite))
        {
            var name = FormatName(property.Name);
            if (sourceProperties.TryGetValue(name, out var sourceProperty)
                && sourceProperty.CanRead
                && sourceProperty.PropertyType == property.PropertyType)
            {
                Ldarg1(ilGenerator);
                Ldarg0(ilGenerator);
                ilGenerator.Emit(OpCodes.Callvirt, sourceProperty.GetGetMethod(nonPublic));
                ilGenerator.Emit(OpCodes.Callvirt, property.GetSetMethod(nonPublic));
            }
            else if (sourceFields.TryGetValue(name, out var sourceField)
                     && sourceField.FieldType == property.PropertyType)
            {
                Ldarg1(ilGenerator);
                Ldarg0(ilGenerator);
                ilGenerator.Emit(OpCodes.Ldfld, sourceField);
                ilGenerator.Emit(OpCodes.Callvirt, property.GetSetMethod(nonPublic));
            }
        }

        foreach (var field in destinationType.GetFields(bindingFlags))
        {
            var name = FormatName(field.Name);
            if (sourceFields.TryGetValue(name, out var sourceField)
                && sourceField.FieldType == field.FieldType)
            {
                Ldarg1(ilGenerator);
                Ldarg0(ilGenerator);
                ilGenerator.Emit(OpCodes.Ldfld, sourceField);
                ilGenerator.Emit(OpCodes.Stfld, field);
            }
            else if (sourceProperties.TryGetValue(name, out var sourceProperty)
                     && sourceProperty.CanRead
                     && sourceProperty.PropertyType == field.FieldType)
            {
                Ldarg1(ilGenerator);
                Ldarg0(ilGenerator);
                ilGenerator.Emit(OpCodes.Callvirt, sourceProperty.GetGetMethod(nonPublic));
                ilGenerator.Emit(OpCodes.Stfld, field);
            }
        }

        ilGenerator.Emit(OpCodes.Ret);

        return (TDelegate)dynamicMethod.CreateDelegate(typeof(TDelegate));

        //格式化名称
        string FormatName(string name)
        {
            return ignoreCase ? name.ToLowerInvariant() : name;
        }
    }

    /// <summary>
    /// 创建 结构体 复制委托
    /// <para/>
    /// 委托将源对象的字段、属性赋值到目标结构体的同名、同类型的字段、属性
    /// </summary>
    /// <typeparam name="TSource">源对象类型</typeparam>
    /// <typeparam name="TDestination">目标结构体类型</typeparam>
    /// <param name="ignoreCase">忽略名称大小写</param>
    /// <param name="nonPublic">是否复制私有字段、属性</param>
    /// <returns></returns>
    public static StructMemberCopyDelegate<TSource, TDestination> CreateStructMemberCopyDelegate<TSource, TDestination>(bool ignoreCase, bool nonPublic) where TDestination : struct
    {
        return CreateMemberCopyDelegate<StructMemberCopyDelegate<TSource, TDestination>>(typeof(TSource), typeof(TDestination), ignoreCase: ignoreCase, nonPublic: nonPublic);
    }

    /// <summary>
    /// 创建 结构体 复制成员到 对象 委托
    /// <para/>
    /// 委托将源结构体的字段、属性赋值到目标对象的同名、同类型的字段、属性
    /// </summary>
    /// <typeparam name="TSource">源结构体类型</typeparam>
    /// <typeparam name="TDestination">目标对象类型</typeparam>
    /// <param name="ignoreCase">忽略名称大小写</param>
    /// <param name="nonPublic">是否复制私有字段、属性</param>
    /// <returns></returns>
    public static StructToObjectMemberCopyDelegate<TSource, TDestination> CreateStructToObjectMemberCopyDelegate<TSource, TDestination>(bool ignoreCase, bool nonPublic) where TSource : struct where TDestination : class
    {
        return CreateMemberCopyDelegate<StructToObjectMemberCopyDelegate<TSource, TDestination>>(typeof(TSource), typeof(TDestination), ignoreCase: ignoreCase, nonPublic: nonPublic);
    }

    #endregion Public 方法

    #region Private 方法

    /// <summary>
    /// 检查成员复制委托信息
    /// </summary>
    /// <typeparam name="TDelegate"></typeparam>
    private static Type[] CheckMemberCopyDelegateInfo<TDelegate>() where TDelegate : Delegate
    {
        var delegateInfo = typeof(TDelegate).GetTypeInfo().DeclaredMethods
                                            .FirstOrDefault(m => m.Name.Equals("Invoke", StringComparison.Ordinal));

        if (delegateInfo.ReturnType != typeof(void))
        {
            throw new ArgumentException("member copy delegate must return void.");
        }
        if (delegateInfo.GetParameters() is ParameterInfo[] parameters
            && parameters.Length == 2)
        {
            return parameters.Select(m => m.ParameterType).ToArray();
        }
        else
        {
            throw new ArgumentException("member copy delegate must has two parameter.");
        }
    }

    /// <summary>
    /// 参数是否为引用传递
    /// </summary>
    /// <param name="parameterType"></param>
    /// <returns></returns>
    private static bool IsParameterRef(Type parameterType)
    {
        return parameterType.IsClass && parameterType.IsByRef;
    }

    #region Emit

    private static void Ldarg_0(ILGenerator ilGenerator)
    {
        ilGenerator.Emit(OpCodes.Ldarg_0);
    }

    private static void Ldarg_0_Ref(ILGenerator ilGenerator)
    {
        ilGenerator.Emit(OpCodes.Ldarg_0);
        ilGenerator.Emit(OpCodes.Ldind_Ref);
    }

    private static void Ldarg_1(ILGenerator ilGenerator)
    {
        ilGenerator.Emit(OpCodes.Ldarg_1);
    }

    private static void Ldarg_1_Ref(ILGenerator ilGenerator)
    {
        ilGenerator.Emit(OpCodes.Ldarg_1);
        ilGenerator.Emit(OpCodes.Ldind_Ref);
    }

    #endregion Emit

    #endregion Private 方法
}