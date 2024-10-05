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

    [TestMethod]
    public void Should_Success_With_MaxValue()
    {
        var size = new StorageSize(long.MaxValue);
        Assert.AreEqual("8EB", size.ToString());

        size = new StorageSize(ulong.MaxValue);
        Assert.AreEqual("16EB", size.ToString());
    }

    [TestMethod]
    public void Should_Throw_With_ErrorValue()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new StorageSize(-1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new StorageSize(long.MinValue));
    }

    [TestMethod]
    public void Should_ToString_Correct()
    {
        const long B = 1;
        const long KB = B * 1024;
        const long MB = KB * 1024;
        const long GB = MB * 1024;
        const long TB = GB * 1024;
        const long PB = TB * 1024;
        const long EB = PB * 1024;

        Assert.AreEqual("0B", new StorageSize(0).ToString());
        Assert.AreEqual("1B", new StorageSize(B).ToString());
        Assert.AreEqual("1023B", new StorageSize(KB - B).ToString());

        Assert.AreEqual("1KB", new StorageSize(KB).ToString());
        Assert.AreEqual("1.5KB", new StorageSize(KB + KB / 2).ToString());
        Assert.AreEqual("1.499KB", new StorageSize(KB + KB / 2 - B).ToString());
        Assert.AreEqual("1.999KB", new StorageSize(KB + KB - B).ToString());
        Assert.AreEqual("1023KB", new StorageSize(MB - KB).ToString());

        Assert.AreEqual("1MB", new StorageSize(MB).ToString());
        Assert.AreEqual("1.5MB", new StorageSize(MB + MB / 2).ToString());
        Assert.AreEqual("2MB", new StorageSize(MB + MB).ToString());
        Assert.AreEqual("1.999MB", new StorageSize(MB + MB - KB).ToString());
        Assert.AreEqual("1023MB", new StorageSize(GB - MB).ToString());

        Assert.AreEqual("1GB", new StorageSize(GB).ToString());
        Assert.AreEqual("1.5GB", new StorageSize(GB + GB / 2).ToString());
        Assert.AreEqual("2GB", new StorageSize(GB + GB).ToString());
        Assert.AreEqual("1.999GB", new StorageSize(GB + GB - MB).ToString());
        Assert.AreEqual("1023GB", new StorageSize(TB - GB).ToString());

        Assert.AreEqual("1TB", new StorageSize(TB).ToString());
        Assert.AreEqual("1.5TB", new StorageSize(TB + TB / 2).ToString());
        Assert.AreEqual("2TB", new StorageSize(TB + TB).ToString());
        Assert.AreEqual("1.999TB", new StorageSize(TB + TB - GB).ToString());
        Assert.AreEqual("1023TB", new StorageSize(PB - TB).ToString());

        Assert.AreEqual("1PB", new StorageSize(PB).ToString());
        Assert.AreEqual("1.5PB", new StorageSize(PB + PB / 2).ToString());
        Assert.AreEqual("2PB", new StorageSize(PB + PB).ToString());
        Assert.AreEqual("1.999PB", new StorageSize(PB + PB - TB).ToString());
        Assert.AreEqual("1023PB", new StorageSize(EB - PB).ToString());

        Assert.AreEqual("1EB", new StorageSize(EB).ToString());
        Assert.AreEqual("1.5EB", new StorageSize(EB + EB / 2).ToString());
        Assert.AreEqual("2EB", new StorageSize(EB + EB).ToString());
        Assert.AreEqual("1.999EB", new StorageSize(EB + EB - PB).ToString());
    }

    #endregion Public 方法
}
