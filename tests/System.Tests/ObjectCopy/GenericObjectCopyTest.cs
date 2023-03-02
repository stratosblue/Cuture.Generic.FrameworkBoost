namespace System.Tests.ObjectCopy;

[TestClass]
public class GenericObjectCopyTest : ObjectCopyTestBase
{
    #region Copy From Struct

    [TestMethod]
    public void Copy_IgnoreCase_All_FromStruct()
    {
        var struct1 = CreateObjectCopyStruct1();
        var obj2 = CreateObjectCopyClass2();

        var struct1_back = struct1;

        obj2.CopyFrom(ref struct1, ignoreCase: true, nonPublic: true);

        Assert.IsTrue(struct1.Equals(struct1_back));

        Assert.IsTrue(string.Equals(struct1.PublicFields, obj2.PublicFields, StringComparison.Ordinal));
        Assert.IsTrue(string.Equals(struct1.PublicProperty, obj2.PublicProperty, StringComparison.Ordinal));
        Assert.IsTrue(string.Equals(struct1.GetPrivateFieldsValue(), obj2.GetPrivateFieldsValue(), StringComparison.Ordinal));
        Assert.IsTrue(string.Equals(struct1.GetPrivatePropertyValue(), obj2.GetPrivatePropertyValue(), StringComparison.Ordinal));
        Assert.IsTrue(string.Equals(struct1.SpecialProperty, obj2.GetSpecialPropertyValue(), StringComparison.Ordinal));
        Assert.IsTrue(string.Equals(struct1.casefields, obj2.CaseFields, StringComparison.Ordinal));
        Assert.IsTrue(string.Equals(struct1.caseproperty, obj2.CaseProperty, StringComparison.Ordinal));
    }

    [TestMethod]
    public void Copy_IgnoreCase_PublicOnly_FromStruct()
    {
        var struct1 = CreateObjectCopyStruct1();
        var obj2 = CreateObjectCopyClass2();

        var struct1_back = struct1;

        obj2.CopyFrom(ref struct1, ignoreCase: true);

        Assert.IsTrue(struct1.Equals(struct1_back));

        Assert.IsTrue(string.Equals(struct1.PublicFields, obj2.PublicFields, StringComparison.Ordinal));
        Assert.IsTrue(string.Equals(struct1.PublicProperty, obj2.PublicProperty, StringComparison.Ordinal));
        Assert.AreEqual(null, obj2.GetPrivateFieldsValue());
        Assert.AreEqual(null, obj2.GetPrivatePropertyValue());
        Assert.IsTrue(string.Equals(struct1.SpecialProperty, obj2.GetSpecialPropertyValue(), StringComparison.Ordinal));
        Assert.IsTrue(string.Equals(struct1.casefields, obj2.CaseFields, StringComparison.Ordinal));
        Assert.IsTrue(string.Equals(struct1.caseproperty, obj2.CaseProperty, StringComparison.Ordinal));
    }

    [TestMethod]
    public void Copy_NoIgnoreCase_All_FromStruct()
    {
        var struct1 = CreateObjectCopyStruct1();
        var obj2 = CreateObjectCopyClass2();

        var struct1_back = struct1;

        obj2.CopyFrom(ref struct1, ignoreCase: false, nonPublic: true);

        Assert.IsTrue(struct1.Equals(struct1_back));

        Assert.IsTrue(string.Equals(struct1.PublicFields, obj2.PublicFields, StringComparison.Ordinal));
        Assert.IsTrue(string.Equals(struct1.PublicProperty, obj2.PublicProperty, StringComparison.Ordinal));
        Assert.IsTrue(string.Equals(struct1.GetPrivateFieldsValue(), obj2.GetPrivateFieldsValue(), StringComparison.Ordinal));
        Assert.IsTrue(string.Equals(struct1.GetPrivatePropertyValue(), obj2.GetPrivatePropertyValue(), StringComparison.Ordinal));
        Assert.IsTrue(string.Equals(struct1.SpecialProperty, obj2.GetSpecialPropertyValue(), StringComparison.Ordinal));
        Assert.AreEqual(null, obj2.CaseFields);
        Assert.AreEqual(null, obj2.CaseProperty);
    }

    [TestMethod]
    public void Copy_NoIgnoreCase_PublicOnly_FromStruct()
    {
        var struct1 = CreateObjectCopyStruct1();
        var obj2 = CreateObjectCopyClass2();

        var struct1_back = struct1;

        obj2.CopyFrom(ref struct1);

        Assert.IsTrue(struct1.Equals(struct1_back));

        Assert.IsTrue(string.Equals(struct1.PublicFields, obj2.PublicFields, StringComparison.Ordinal));
        Assert.IsTrue(string.Equals(struct1.PublicProperty, obj2.PublicProperty, StringComparison.Ordinal));
        Assert.AreEqual(null, obj2.GetPrivateFieldsValue());
        Assert.AreEqual(null, obj2.GetPrivatePropertyValue());
        Assert.IsTrue(string.Equals(struct1.SpecialProperty, obj2.GetSpecialPropertyValue(), StringComparison.Ordinal));
        Assert.AreEqual(null, obj2.CaseFields);
        Assert.AreEqual(null, obj2.CaseProperty);
    }

    #endregion Copy From Struct

    #region Copy From Class

    [TestMethod]
    public void Copy_IgnoreCase_All_FromClass()
    {
        var obj1 = CreateObjectCopyClass1();
        var obj2 = CreateObjectCopyClass2();

        var obj1_back = obj1.Clone();

        obj1.CopyTo(obj2, ignoreCase: true, nonPublic: true);

        Assert.IsFalse(ReferenceEquals(obj1_back, obj1));
        Assert.IsTrue(obj1.Equals(obj1_back));

        Assert.IsTrue(string.Equals(obj1.PublicFields, obj2.PublicFields, StringComparison.Ordinal));
        Assert.IsTrue(string.Equals(obj1.PublicProperty, obj2.PublicProperty, StringComparison.Ordinal));
        Assert.IsTrue(string.Equals(obj1.GetPrivateFieldsValue(), obj2.GetPrivateFieldsValue(), StringComparison.Ordinal));
        Assert.IsTrue(string.Equals(obj1.GetPrivatePropertyValue(), obj2.GetPrivatePropertyValue(), StringComparison.Ordinal));
        Assert.IsTrue(string.Equals(obj1.SpecialProperty, obj2.GetSpecialPropertyValue(), StringComparison.Ordinal));
        Assert.IsTrue(string.Equals(obj1.casefields, obj2.CaseFields, StringComparison.Ordinal));
        Assert.IsTrue(string.Equals(obj1.caseproperty, obj2.CaseProperty, StringComparison.Ordinal));
    }

    [TestMethod]
    public void Copy_IgnoreCase_PublicOnly_FromClass()
    {
        var obj1 = CreateObjectCopyClass1();
        var obj2 = CreateObjectCopyClass2();

        var obj1_back = obj1.Clone();

        obj1.CopyTo(obj2, ignoreCase: true);

        Assert.IsFalse(ReferenceEquals(obj1_back, obj1));
        Assert.IsTrue(obj1.Equals(obj1_back));

        Assert.IsTrue(string.Equals(obj1.PublicFields, obj2.PublicFields, StringComparison.Ordinal));
        Assert.IsTrue(string.Equals(obj1.PublicProperty, obj2.PublicProperty, StringComparison.Ordinal));
        Assert.AreEqual(null, obj2.GetPrivateFieldsValue());
        Assert.AreEqual(null, obj2.GetPrivatePropertyValue());
        Assert.IsTrue(string.Equals(obj1.SpecialProperty, obj2.GetSpecialPropertyValue(), StringComparison.Ordinal));
        Assert.IsTrue(string.Equals(obj1.casefields, obj2.CaseFields, StringComparison.Ordinal));
        Assert.IsTrue(string.Equals(obj1.caseproperty, obj2.CaseProperty, StringComparison.Ordinal));
    }

    [TestMethod]
    public void Copy_NoIgnoreCase_All_FromClass()
    {
        var obj1 = CreateObjectCopyClass1();
        var obj2 = CreateObjectCopyClass2();

        var obj1_back = obj1.Clone();

        obj1.CopyTo(obj2, ignoreCase: false, nonPublic: true);

        Assert.IsFalse(ReferenceEquals(obj1_back, obj1));
        Assert.IsTrue(obj1.Equals(obj1_back));

        Assert.IsTrue(string.Equals(obj1.PublicFields, obj2.PublicFields, StringComparison.Ordinal));
        Assert.IsTrue(string.Equals(obj1.PublicProperty, obj2.PublicProperty, StringComparison.Ordinal));
        Assert.IsTrue(string.Equals(obj1.GetPrivateFieldsValue(), obj2.GetPrivateFieldsValue(), StringComparison.Ordinal));
        Assert.IsTrue(string.Equals(obj1.GetPrivatePropertyValue(), obj2.GetPrivatePropertyValue(), StringComparison.Ordinal));
        Assert.IsTrue(string.Equals(obj1.SpecialProperty, obj2.GetSpecialPropertyValue(), StringComparison.Ordinal));
        Assert.AreEqual(null, obj2.CaseFields);
        Assert.AreEqual(null, obj2.CaseProperty);
    }

    [TestMethod]
    public void Copy_NoIgnoreCase_PublicOnly_FromClass()
    {
        var obj1 = CreateObjectCopyClass1();
        var obj2 = CreateObjectCopyClass2();

        var obj1_back = obj1.Clone();

        obj1.CopyTo(obj2);

        Assert.IsFalse(ReferenceEquals(obj1_back, obj1));
        Assert.IsTrue(obj1.Equals(obj1_back));

        Assert.IsTrue(string.Equals(obj1.PublicFields, obj2.PublicFields, StringComparison.Ordinal));
        Assert.IsTrue(string.Equals(obj1.PublicProperty, obj2.PublicProperty, StringComparison.Ordinal));
        Assert.AreEqual(null, obj2.GetPrivateFieldsValue());
        Assert.AreEqual(null, obj2.GetPrivatePropertyValue());
        Assert.IsTrue(string.Equals(obj1.SpecialProperty, obj2.GetSpecialPropertyValue(), StringComparison.Ordinal));
        Assert.AreEqual(null, obj2.CaseFields);
        Assert.AreEqual(null, obj2.CaseProperty);
    }

    #endregion Copy From Class
}