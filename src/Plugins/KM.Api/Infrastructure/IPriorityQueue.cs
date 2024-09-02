namespace KM.Api.Infrastructure;
public interface IPriorityQueue
{
    void Dequeue(string key);
    void Enqueue<TData>(string key, TData data, Func<TData, Task> handler, DateTimeOffset expiration = default);
}
