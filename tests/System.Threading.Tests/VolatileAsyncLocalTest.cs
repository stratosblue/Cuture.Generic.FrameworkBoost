namespace System.Threading.Tests;

[TestClass]
public class VolatileAsyncLocalTest
{
    private AsyncLocal<int>? _asyncLocalInt32;
    private VolatileAsyncLocal<int>? _volatileAsyncLocalInt32;

    [TestInitialize]
    public void Initialize()
    {
        _asyncLocalInt32 = new();
        _volatileAsyncLocalInt32 = new();
    }

    [TestMethod]
    public async Task ShouldReadChangedValue()
    {
        for (int value = 0; value < 100; value++)
        {
            await ChangeValueAsync(value);
            CheckAsyncLocalValue(default);
            CheckVolatileAsyncLocalValue(value);
        }
    }

    private async Task ChangeValueAsync(int value)
    {
        await Task.Yield();
        _asyncLocalInt32.Value = value;
        _volatileAsyncLocalInt32.Value = value;
        await Task.Yield();
    }

    private void CheckAsyncLocalValue(int value)
    {
        Assert.AreEqual(value, _asyncLocalInt32.Value);
    }

    private void CheckVolatileAsyncLocalValue(int value)
    {
        Assert.AreEqual(value, _volatileAsyncLocalInt32.Value);
    }
}
