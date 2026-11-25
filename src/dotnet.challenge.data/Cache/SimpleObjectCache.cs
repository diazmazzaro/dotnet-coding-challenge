using System.Collections.Concurrent;

namespace dotnet.challenge.data.Cache;

public class SimpleObjectCache<TKey, TValue> : ISimpleObjectCache<TKey, TValue>
    where TKey : struct
    where TValue : class
{
    private readonly ConcurrentDictionary<TKey, TValue> _store = new();

    public Task AddAsync(TKey key, TValue value)
    {
        _store[key] = value;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(TKey key)
    {
        _store.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    public Task<TValue?> GetAsync(TKey key)
    {
        _store.TryGetValue(key, out var value);
        return Task.FromResult(value);
    }

    public Task<IEnumerable<TValue>> GetAllAsync()
    {
        IEnumerable<TValue> values = _store.Values.ToList();
        return Task.FromResult(values);
    }

    public Task UpdateAsync(TKey key, TValue value)
    {
        while (_store.TryGetValue(key, out var current) && !_store.TryUpdate(key, value, current))
        {
            // Retry if another writer replaced the entry after the lookup.
        }
        return Task.CompletedTask;
    }
}
