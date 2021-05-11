using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Tests.ObjectCopy
{
    [TestClass]
    public class GenericObjectCopyTest
    {
        #region Public 方法

        [TestMethod]
        public void Copy_IgnoreCase_All()
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
        public void Copy_IgnoreCase_PublicOnly()
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
        public void Copy_NoIgnoreCase_All()
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
        public void Copy_NoIgnoreCase_PublicOnly()
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

        #endregion Public 方法

        #region Private 方法

        private static ObjectCopyClass1 CreateObjectCopyClass1()
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

        private static ObjectCopyClass2 CreateObjectCopyClass2()
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

        #endregion Private 方法
    }
}