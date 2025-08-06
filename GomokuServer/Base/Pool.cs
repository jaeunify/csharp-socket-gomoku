using System.Collections.Generic;

public class Pool<T> where T : class, new()
{
    private readonly Queue<T> pool = new Queue<T>();

    public T RentFromPool()
    {
        if (pool.Count > 0)
        {
            return pool.Dequeue();
        }
        return new T();
    }

    public void ReturnToPool(T obj)
    {
        pool.Enqueue(obj);
    }

    public int Count => pool.Count;
}
