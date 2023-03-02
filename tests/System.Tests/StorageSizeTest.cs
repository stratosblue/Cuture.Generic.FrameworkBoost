namespace System.Tests;

[TestClass]
public class StorageSizeTest
{
    #region Public 方法

    [TestMethod]
    public void GenericUsage()
    {
        var a = new StorageSize(12345);
        var b = new StorageSize(12345);
        var c = new StorageSize(12345);

        var total = new StorageSize(12345 * 3);

        Assert.IsTrue(a.Equals(b));
        Assert.IsTrue(b.Equals(c));

        Assert.IsTrue(total.Equals(a * 3));
        Assert.IsTrue(total.Equals(b * 3));
        Assert.IsTrue(total.Equals(c * 3));

        Assert.IsTrue(a == b);
        Assert.IsTrue(b == c);
        Assert.IsTrue(c == a);

        Assert.IsTrue(a != total);
        Assert.IsTrue(b != total);
        Assert.IsTrue(c != total);

        Assert.AreEqual(a + b, b + c);
        Assert.AreEqual(b + c, c + a);

        Assert.AreEqual(a - b, b - c);
        Assert.AreEqual(b - c, c - a);

        Assert.AreEqual(a + b + c, total);

        Assert.AreEqual(a + b + c, a * 3);
        Assert.AreEqual(a + b + c, b * 3);
        Assert.AreEqual(a + b + c, c * 3);

        Assert.AreEqual(a, total / 3);
        Assert.AreEqual(b, total / 3);
        Assert.AreEqual(c, total / 3);

        Assert.IsTrue(total > a);
        Assert.IsTrue(total > b);
        Assert.IsTrue(total > c);

        Assert.IsTrue(a < b + c);
        Assert.IsTrue(b < a + c);
        Assert.IsTrue(c < a + b);

        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        Assert.AreEqual(b.GetHashCode(), c.GetHashCode());
    }

    #endregion Public 方法
}