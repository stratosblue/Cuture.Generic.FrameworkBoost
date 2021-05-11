using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace System
{
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
        public static Action<TSource, TDestination> CreateMemberCopyDelegate<TSource, TDestination>(bool ignoreCase, bool nonPublic)
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
        public static TDelegate CreateMemberCopyDelegate<TDelegate>(Type sourceType, Type destinationType, bool ignoreCase, bool nonPublic) where TDelegate : Delegate
        {
            var methodName = $"ObjectCopy_{sourceType.Name}_to_{destinationType.Name}_{ignoreCase}_{nonPublic}_{Guid.NewGuid():n}";
            var dynamicMethod = new DynamicMethod(methodName, typeof(void), new[] { sourceType, destinationType }, destinationType, true);
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
                    ilGenerator.Emit(OpCodes.Ldarg_1);
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Callvirt, sourceProperty.GetGetMethod(nonPublic));
                    ilGenerator.Emit(OpCodes.Callvirt, property.GetSetMethod(nonPublic));
                }
                else if (sourceFields.TryGetValue(name, out var sourceField)
                         && sourceField.FieldType == property.PropertyType)
                {
                    ilGenerator.Emit(OpCodes.Ldarg_1);
                    ilGenerator.Emit(OpCodes.Ldarg_0);
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
                    ilGenerator.Emit(OpCodes.Ldarg_1);
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Ldfld, sourceField);
                    ilGenerator.Emit(OpCodes.Stfld, field);
                }
                else if (sourceProperties.TryGetValue(name, out var sourceProperty)
                         && sourceProperty.CanRead
                         && sourceProperty.PropertyType == field.FieldType)
                {
                    ilGenerator.Emit(OpCodes.Ldarg_1);
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Callvirt, sourceProperty.GetGetMethod(nonPublic));
                    ilGenerator.Emit(OpCodes.Stfld, field);
                }
            }

            ilGenerator.Emit(OpCodes.Ret);

            return (TDelegate)dynamicMethod.CreateDelegate(typeof(TDelegate));

            string FormatName(string name)
            {
                return ignoreCase ? name.ToLowerInvariant() : name;
            }
        }

        #endregion Public 方法
    }
}