using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Tests.ObjectCopy
{
    [TestClass]
    public class GenericStructCopyTest : ObjectCopyTestBase
    {
        #region Copy From Class

        [TestMethod]
        public void Copy_IgnoreCase_All_FromClass()
        {
            var obj1 = CreateObjectCopyClass1();
            var struct2 = CreateObjectCopyStruct2();

            var obj1_back = obj1.Clone();

            obj1.CopyTo(ref struct2, ignoreCase: true, nonPublic: true);

            Assert.IsFalse(ReferenceEquals(obj1_back, obj1));
            Assert.IsTrue(obj1.Equals(obj1_back));

            Assert.IsTrue(string.Equals(obj1.PublicFields, struct2.PublicFields, StringComparison.Ordinal));
            Assert.IsTrue(string.Equals(obj1.PublicProperty, struct2.PublicProperty, StringComparison.Ordinal));
            Assert.IsTrue(string.Equals(obj1.GetPrivateFieldsValue(), struct2.GetPrivateFieldsValue(), StringComparison.Ordinal));
            Assert.IsTrue(string.Equals(obj1.GetPrivatePropertyValue(), struct2.GetPrivatePropertyValue(), StringComparison.Ordinal));
            Assert.IsTrue(string.Equals(obj1.SpecialProperty, struct2.GetSpecialPropertyValue(), StringComparison.Ordinal));
            Assert.IsTrue(string.Equals(obj1.casefields, struct2.CaseFields, StringComparison.Ordinal));
            Assert.IsTrue(string.Equals(obj1.caseproperty, struct2.CaseProperty, StringComparison.Ordinal));
        }

        [TestMethod]
        public void Copy_IgnoreCase_PublicOnly_FromClass()
        {
            var obj1 = CreateObjectCopyClass1();
            var struct2 = CreateObjectCopyStruct2();

            var obj1_back = obj1.Clone();

            obj1.CopyTo(ref struct2, ignoreCase: true);

            Assert.IsFalse(ReferenceEquals(obj1_back, obj1));
            Assert.IsTrue(obj1.Equals(obj1_back));

            Assert.IsTrue(string.Equals(obj1.PublicFields, struct2.PublicFields, StringComparison.Ordinal));
            Assert.IsTrue(string.Equals(obj1.PublicProperty, struct2.PublicProperty, StringComparison.Ordinal));
            Assert.AreEqual(null, struct2.GetPrivateFieldsValue());
            Assert.AreEqual(null, struct2.GetPrivatePropertyValue());
            Assert.IsTrue(string.Equals(obj1.SpecialProperty, struct2.GetSpecialPropertyValue(), StringComparison.Ordinal));
            Assert.IsTrue(string.Equals(obj1.casefields, struct2.CaseFields, StringComparison.Ordinal));
            Assert.IsTrue(string.Equals(obj1.caseproperty, struct2.CaseProperty, StringComparison.Ordinal));
        }

        [TestMethod]
        public void Copy_NoIgnoreCase_All_FromClass()
        {
            var obj1 = CreateObjectCopyClass1();
            var struct2 = CreateObjectCopyStruct2();

            var obj1_back = obj1.Clone();

            obj1.CopyTo(ref struct2, ignoreCase: false, nonPublic: true);

            Assert.IsFalse(ReferenceEquals(obj1_back, obj1));
            Assert.IsTrue(obj1.Equals(obj1_back));

            Assert.IsTrue(string.Equals(obj1.PublicFields, struct2.PublicFields, StringComparison.Ordinal));
            Assert.IsTrue(string.Equals(obj1.PublicProperty, struct2.PublicProperty, StringComparison.Ordinal));
            Assert.IsTrue(string.Equals(obj1.GetPrivateFieldsValue(), struct2.GetPrivateFieldsValue(), StringComparison.Ordinal));
            Assert.IsTrue(string.Equals(obj1.GetPrivatePropertyValue(), struct2.GetPrivatePropertyValue(), StringComparison.Ordinal));
            Assert.IsTrue(string.Equals(obj1.SpecialProperty, struct2.GetSpecialPropertyValue(), StringComparison.Ordinal));
            Assert.AreEqual(null, struct2.CaseFields);
            Assert.AreEqual(null, struct2.CaseProperty);
        }

        [TestMethod]
        public void Copy_NoIgnoreCase_PublicOnly_FromClass()
        {
            var obj1 = CreateObjectCopyClass1();
            var struct2 = CreateObjectCopyStruct2();

            var obj1_back = obj1.Clone();

            obj1.CopyTo(ref struct2);

            Assert.IsFalse(ReferenceEquals(obj1_back, obj1));
            Assert.IsTrue(obj1.Equals(obj1_back));

            Assert.IsTrue(string.Equals(obj1.PublicFields, struct2.PublicFields, StringComparison.Ordinal));
            Assert.IsTrue(string.Equals(obj1.PublicProperty, struct2.PublicProperty, StringComparison.Ordinal));
            Assert.AreEqual(null, struct2.GetPrivateFieldsValue());
            Assert.AreEqual(null, struct2.GetPrivatePropertyValue());
            Assert.IsTrue(string.Equals(obj1.SpecialProperty, struct2.GetSpecialPropertyValue(), StringComparison.Ordinal));
            Assert.AreEqual(null, struct2.CaseFields);
            Assert.AreEqual(null, struct2.CaseProperty);
        }

        #endregion Copy From Class

        #region Copy From Struct

        [TestMethod]
        public void Copy_IgnoreCase_All_FromStruct()
        {
            var struct1 = CreateObjectCopyStruct1();
            var struct2 = CreateObjectCopyStruct2();

            var struct1_back = struct1;

            struct1.CopyTo(ref struct2, ignoreCase: true, nonPublic: true);

            Assert.IsTrue(struct1.Equals(struct1_back));

            Assert.IsTrue(string.Equals(struct1.PublicFields, struct2.PublicFields, StringComparison.Ordinal));
            Assert.IsTrue(string.Equals(struct1.PublicProperty, struct2.PublicProperty, StringComparison.Ordinal));
            Assert.IsTrue(string.Equals(struct1.GetPrivateFieldsValue(), struct2.GetPrivateFieldsValue(), StringComparison.Ordinal));
            Assert.IsTrue(string.Equals(struct1.GetPrivatePropertyValue(), struct2.GetPrivatePropertyValue(), StringComparison.Ordinal));
            Assert.IsTrue(string.Equals(struct1.SpecialProperty, struct2.GetSpecialPropertyValue(), StringComparison.Ordinal));
            Assert.IsTrue(string.Equals(struct1.casefields, struct2.CaseFields, StringComparison.Ordinal));
            Assert.IsTrue(string.Equals(struct1.caseproperty, struct2.CaseProperty, StringComparison.Ordinal));
        }

        [TestMethod]
        public void Copy_IgnoreCase_PublicOnly_FromStruct()
        {
            var struct1 = CreateObjectCopyStruct1();
            var struct2 = CreateObjectCopyStruct2();

            var struct1_back = struct1;

            struct1.CopyTo(ref struct2, ignoreCase: true);

            Assert.IsTrue(struct1.Equals(struct1_back));

            Assert.IsTrue(string.Equals(struct1.PublicFields, struct2.PublicFields, StringComparison.Ordinal));
            Assert.IsTrue(string.Equals(struct1.PublicProperty, struct2.PublicProperty, StringComparison.Ordinal));
            Assert.AreEqual(null, struct2.GetPrivateFieldsValue());
            Assert.AreEqual(null, struct2.GetPrivatePropertyValue());
            Assert.IsTrue(string.Equals(struct1.SpecialProperty, struct2.GetSpecialPropertyValue(), StringComparison.Ordinal));
            Assert.IsTrue(string.Equals(struct1.casefields, struct2.CaseFields, StringComparison.Ordinal));
            Assert.IsTrue(string.Equals(struct1.caseproperty, struct2.CaseProperty, StringComparison.Ordinal));
        }

        [TestMethod]
        public void Copy_NoIgnoreCase_All_FromStruct()
        {
            var struct1 = CreateObjectCopyStruct1();
            var struct2 = CreateObjectCopyStruct2();

            var struct1_back = struct1;

            struct1.CopyTo(ref struct2, ignoreCase: false, nonPublic: true);

            Assert.IsTrue(struct1.Equals(struct1_back));

            Assert.IsTrue(string.Equals(struct1.PublicFields, struct2.PublicFields, StringComparison.Ordinal));
            Assert.IsTrue(string.Equals(struct1.PublicProperty, struct2.PublicProperty, StringComparison.Ordinal));
            Assert.IsTrue(string.Equals(struct1.GetPrivateFieldsValue(), struct2.GetPrivateFieldsValue(), StringComparison.Ordinal));
            Assert.IsTrue(string.Equals(struct1.GetPrivatePropertyValue(), struct2.GetPrivatePropertyValue(), StringComparison.Ordinal));
            Assert.IsTrue(string.Equals(struct1.SpecialProperty, struct2.GetSpecialPropertyValue(), StringComparison.Ordinal));
            Assert.AreEqual(null, struct2.CaseFields);
            Assert.AreEqual(null, struct2.CaseProperty);
        }

        [TestMethod]
        public void Copy_NoIgnoreCase_PublicOnly_FromStruct()
        {
            var struct1 = CreateObjectCopyStruct1();
            var struct2 = CreateObjectCopyStruct2();

            var struct1_back = struct1;

            struct1.CopyTo(ref struct2);

            Assert.IsTrue(struct1.Equals(struct1_back));

            Assert.IsTrue(string.Equals(struct1.PublicFields, struct2.PublicFields, StringComparison.Ordinal));
            Assert.IsTrue(string.Equals(struct1.PublicProperty, struct2.PublicProperty, StringComparison.Ordinal));
            Assert.AreEqual(null, struct2.GetPrivateFieldsValue());
            Assert.AreEqual(null, struct2.GetPrivatePropertyValue());
            Assert.IsTrue(string.Equals(struct1.SpecialProperty, struct2.GetSpecialPropertyValue(), StringComparison.Ordinal));
            Assert.AreEqual(null, struct2.CaseFields);
            Assert.AreEqual(null, struct2.CaseProperty);
        }

        #endregion Copy From Struct
    }
}