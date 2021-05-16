# Cuture.Generic.FrameworkBoost

## Intro
通用的增强功能方法、类等的集合

-------

## 项目目录

| 名称 | 描述 |
| ---- | ---- |
|Microsoft.Extensions|针对 Microsoft.Extensions.* 官方库的拓展|
|System|针对 System 命名空间的功能拓展|
|System.Collections|针对 System.Collections 命名空间的功能拓展|
|System.Threading|针对 System.Threading 命名空间的功能拓展|

-------

## 功能列表

| 类名 | 命名空间 | 描述 |
| ---- | ---- | ---- |
| BoundedObjectPool | Microsoft.Extensions.ObjectPool | 有限大小的对象池 |
| BoundedMemoryCache | Microsoft.Extensions.Caching.Memory | 有限大小的内存缓存 |
| ObjectCopyExtensions | System | 快速将源对象的字段、属性赋值到目标对象的同名、同类型的字段、属性的拓展方法 |
| DeferFlushCollection | System.Collections.Concurrent | 延时冲洗集合 |
| AsyncCallbackDeferFlushCollection | System.Collections.Concurrent | 异步回调的延时冲洗集合 |
| ExclusiveThreadTaskScheduler | System.Threading.Tasks | 独占一个线程的TaskScheduler，所有调用都在同一线程上执行 |
