using System.Diagnostics;

using Microsoft.Extensions.Caching.Memory;

namespace Microsoft.Extensions.Tests.Caching.Memory.LRUMemoryCache;

[TestClass]
public class LRUSpecializedLinkedListTest
{
    #region Public 方法

    [TestMethod]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(5)]
    [DataRow(1000)]
    [DataRow(10000)]
    public void InsertAtHead_Until_Remove(int capacity)
    {
        var cacheItem = new TestCacheItem();
        var linkedList = new LRUSpecializedLinkedList<TestCacheItem>(capacity);

        //添加缓存到头部，直到第一个缓存被遗弃
        linkedList.InsertAtHead(new(cacheItem));
        Assert.IsTrue(linkedList.ElementAt(0).Equals(cacheItem));

        for (int i = 0; i < capacity - 1; i++)
        {
            linkedList.InsertAtHead(new(new()));
            Assert.IsTrue(linkedList.ElementAt(i + 1).Equals(cacheItem));
            TwoWaySizeCheck(i + 2, linkedList);
        }

        Assert.IsTrue(linkedList.ElementAt(capacity - 1).Equals(cacheItem));
        linkedList.InsertAtHead(new(new()));
        Assert.IsFalse(linkedList.ElementAt(capacity - 1).Equals(cacheItem));
        TwoWaySizeCheck(capacity, linkedList);
    }

    [TestMethod]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(5)]
    [DataRow(1000)]
    [DataRow(10000)]
    public void MoveToHead(in int capacity)
    {
        var cacheItem = new TestCacheItem();
        var cacheEntry = new LRUSpecializedLinkedListNode<TestCacheItem>(cacheItem);
        var linkedList = new LRUSpecializedLinkedList<TestCacheItem>(capacity);

        //持续添加缓存到头部，并在最后刷新第一个缓存
        linkedList.InsertAtHead(cacheEntry);
        Assert.IsTrue(linkedList.ElementAt(0).Equals(cacheItem));

        for (int i = 0; i < capacity - 1; i++)
        {
            linkedList.InsertAtHead(new(new()));
            Assert.IsTrue(linkedList.ElementAt(i + 1).Equals(cacheItem));
        }

        var count = linkedList.Count();

        TwoWaySizeCheck(count, linkedList);

        linkedList.MoveToHead(cacheEntry);

        TwoWaySizeCheck(count, linkedList);

        //随机移动到头部
        for (int i = 1; i < capacity * 2; i++)
        {
            if (i % (capacity * 2 / 3) == 0
                || i % 333 == 0)
            {
                Debug.WriteLine($"Random MoveToHead - {i}");
                linkedList.MoveToHead(cacheEntry);
                Assert.IsTrue(linkedList.ElementAt(0).Equals(cacheItem));
                TwoWaySizeCheck(capacity, linkedList);
                continue;
            }
            else
            {
                Debug.WriteLine($"Random InsertAtHead - {i}");
                linkedList.InsertAtHead(new(new()));
                TwoWaySizeCheck(capacity, linkedList);
            }
        }
        linkedList.MoveToHead(cacheEntry);
        Assert.IsTrue(linkedList.ElementAt(0).Equals(cacheItem));
        TwoWaySizeCheck(count, linkedList);
    }

    [TestMethod]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(5)]
    [DataRow(1000)]
    [DataRow(10000)]
    public void Overload_InsertAtHead(in int capacity)
    {
        var linkedList = new LRUSpecializedLinkedList<TestCacheItem>(capacity);
        for (int i = 0; i < capacity * 2; i++)
        {
            linkedList.InsertAtHead(new(new()));

            if (i < capacity - 1)
            {
                TwoWaySizeCheck(i + 1, linkedList);
            }
            else
            {
                TwoWaySizeCheck(capacity, linkedList);
            }
        }
    }

    [TestMethod]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(5)]
    [DataRow(1000)]
    [DataRow(10000)]
    public void Remove(in int capacity)
    {
        var linkedList = new LRUSpecializedLinkedList<TestCacheItem>(capacity);

        FillLinkedList(capacity, linkedList);

        //remove tail
        for (int i = capacity - 1; i >= 0; i--)
        {
            var count = linkedList.Count();
            var tailNode = linkedList.NodeElementAt(count - 1);
            linkedList.Remove(tailNode);
            TwoWaySizeCheck(count - 1, linkedList);
            if (count > 1)
            {
                Assert.IsFalse(ReferenceEquals(tailNode, linkedList.NodeElementAt(count - 2)));
            }
        }

        FillLinkedList(capacity, linkedList);

        //remove head
        for (int i = 0; i < capacity; i++)
        {
            var count = linkedList.Count();
            var headNode = linkedList.NodeElementAt(0);
            linkedList.Remove(headNode);
            TwoWaySizeCheck(count - 1, linkedList);
            if (count > 1)
            {
                Assert.IsFalse(ReferenceEquals(headNode, linkedList.NodeElementAt(0)));
            }
        }

        FillLinkedList(capacity, linkedList);

        var random = new Random();
        //random remove
        for (int i = 0; i < capacity; i++)
        {
            var count = linkedList.Count();
            var index = random.Next(count);
            var node = linkedList.NodeElementAt(index);
            linkedList.Remove(node);
            TwoWaySizeCheck(count - 1, linkedList);
            if (count > 1
                && index < count - 1)
            {
                Assert.IsFalse(ReferenceEquals(node, linkedList.NodeElementAt(index)));
            }
        }

        FillLinkedList(capacity, linkedList);
    }

    #endregion Public 方法

    #region Private 方法

    private static void FillLinkedList(int capacity, LRUSpecializedLinkedList<TestCacheItem> linkedList)
    {
        TwoWaySizeCheck(0, linkedList);

        for (int i = 0; i < capacity; i++)
        {
            linkedList.InsertAtHead(new(new()));
        }

        TwoWaySizeCheck(capacity, linkedList);
    }

    private static void TwoWaySizeCheck(int size, LRUSpecializedLinkedList<TestCacheItem> linkedList)
    {
        Assert.AreEqual(size, linkedList.Count());
        Assert.AreEqual(size, (linkedList as IReverseEnumerable<TestCacheItem>).GetEnumerable().Count());
    }

    #endregion Private 方法
}

internal static class IEnumerableExtensions
{
    #region Public 方法

    public static int Count<T>(this LRUSpecializedLinkedList<T> list) => (list as IEnumerable<T>).Count();

    public static T ElementAt<T>(this LRUSpecializedLinkedList<T> list, int index) => (list as IEnumerable<T>).ElementAt(index);

    public static LRUSpecializedLinkedListNode<T> NodeElementAt<T>(this LRUSpecializedLinkedList<T> list, int index) => (list as IEnumerable<LRUSpecializedLinkedListNode<T>>).ElementAt(index);

    #endregion Public 方法
}