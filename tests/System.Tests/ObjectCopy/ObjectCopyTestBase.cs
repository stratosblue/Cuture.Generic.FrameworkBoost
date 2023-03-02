namespace System.Tests.ObjectCopy;

[TestClass]
public class ObjectCopyTestBase
{
    #region Internal 方法

    internal static ObjectCopyClass1 CreateObjectCopyClass1()
    {
        var obj1 = new ObjectCopyClass1("privateFields", "privateProperty", "specialProperty")
        {
            PublicFields = nameof(ObjectCopyClass1.PublicFields),
            PublicProperty = nameof(ObjectCopyClass1.PublicProperty),
            casefields = nameof(ObjectCopyClass1.casefields),
            caseproperty = nameof(ObjectCopyClass1.caseproperty),
        };

        Assert.IsFalse(string.IsNullOrEmpty(obj1.PublicFields));
        Assert.IsFalse(string.IsNullOrEmpty(obj1.PublicProperty));
        Assert.IsFalse(string.IsNullOrEmpty(obj1.GetPrivateFieldsValue()));
        Assert.IsFalse(string.IsNullOrEmpty(obj1.GetPrivatePropertyValue()));
        Assert.IsFalse(string.IsNullOrEmpty(obj1.SpecialProperty));
        Assert.IsFalse(string.IsNullOrEmpty(obj1.casefields));
        Assert.IsFalse(string.IsNullOrEmpty(obj1.caseproperty));

        return obj1;
    }

    internal static ObjectCopyClass2 CreateObjectCopyClass2()
    {
        var obj2 = new ObjectCopyClass2();

        Assert.AreEqual(null, obj2.PublicFields);
        Assert.AreEqual(null, obj2.PublicProperty);
        Assert.AreEqual(null, obj2.GetPrivateFieldsValue());
        Assert.AreEqual(null, obj2.GetPrivatePropertyValue());
        Assert.AreEqual(null, obj2.GetSpecialPropertyValue());
        Assert.AreEqual(null, obj2.CaseFields);
        Assert.AreEqual(null, obj2.CaseProperty);

        return obj2;
    }

    internal static ObjectCopyStruct1 CreateObjectCopyStruct1()
    {
        var obj1 = new ObjectCopyStruct1(privateFields: "privateFields",
                                            publicFields: nameof(ObjectCopyStruct1.PublicFields),
                                            casefields: nameof(ObjectCopyStruct1.casefields),
                                            publicProperty: nameof(ObjectCopyStruct1.PublicProperty),
                                            caseproperty: nameof(ObjectCopyStruct1.caseproperty),
                                            privateProperty: "privateProperty",
                                            specialProperty: "specialProperty");

        Assert.IsFalse(string.IsNullOrEmpty(obj1.PublicFields));
        Assert.IsFalse(string.IsNullOrEmpty(obj1.PublicProperty));
        Assert.IsFalse(string.IsNullOrEmpty(obj1.GetPrivateFieldsValue()));
        Assert.IsFalse(string.IsNullOrEmpty(obj1.GetPrivatePropertyValue()));
        Assert.IsFalse(string.IsNullOrEmpty(obj1.SpecialProperty));
        Assert.IsFalse(string.IsNullOrEmpty(obj1.casefields));
        Assert.IsFalse(string.IsNullOrEmpty(obj1.caseproperty));

        return obj1;
    }

    internal static ObjectCopyStruct2 CreateObjectCopyStruct2()
    {
        var struct2 = new ObjectCopyStruct2();

        Assert.AreEqual(null, struct2.PublicFields);
        Assert.AreEqual(null, struct2.PublicProperty);
        Assert.AreEqual(null, struct2.GetPrivateFieldsValue());
        Assert.AreEqual(null, struct2.GetPrivatePropertyValue());
        Assert.AreEqual(null, struct2.GetSpecialPropertyValue());
        Assert.AreEqual(null, struct2.CaseFields);
        Assert.AreEqual(null, struct2.CaseProperty);

        return struct2;
    }

    #endregion Internal 方法
}