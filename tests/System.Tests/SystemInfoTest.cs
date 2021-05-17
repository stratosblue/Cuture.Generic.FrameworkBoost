using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Tests
{
    [TestClass]
    public class SystemInfoTest
    {
        #region Public 方法

        [TestMethod]
        public void GenericUsage()
        {
            SystemInfo.Display();
        }

        #endregion Public 方法
    }
}