using DecisionPlatformWeb.Service.Solver;

namespace DecisionPlatformWeb.Service.Cache;

using System;
using System.Collections.Generic;
using System.Timers;

public class Cache
{
    private readonly object _lock = new();
    private readonly double _defaultTimeout;
    private Dictionary<Guid, (MultiStepSolver Data, Timer ExpirationTimer)> _cache;

    public Cache(double minutes)
    {
        _cache = new Dictionary<Guid, (MultiStepSolver Data, Timer ExpirationTimer)>();
        _defaultTimeout = (minutes + 1) * 60 * 1000;
    }

    public Guid AddObject(MultiStepSolver obj)
    {
        var guid = Guid.NewGuid();
        var timer = new Timer(_defaultTimeout);

        lock (_lock)
        {
            while (_cache.ContainsKey(guid))
            {
                guid = Guid.NewGuid();
            }
        }

        timer.Elapsed += (_, _) => OnExpired(guid, timer);
        timer.Start();

        lock (_lock)
        {
            _cache.Add(guid, (obj, timer));
        }

        return guid;
    }

    private void OnExpired(Guid objectId, Timer timer)
    {
        timer.Stop();
        timer.Dispose();

        lock (_lock)
        {
            if (_cache.ContainsKey(objectId))
            {
                _cache.Remove(objectId);
            }
        }
    }

    public MultiStepSolver? GetObject(Guid objectId)
    {
        lock (_lock)
        {
            return _cache.TryGetValue(objectId, out var cachedItem) ? cachedItem.Data : null;
        }
    }

    private Timer CreateTimer(Guid objectId)
    {
        var timer = new Timer(_defaultTimeout);
        timer.Elapsed += (_, _) => OnExpired(objectId, timer);
        timer.Start();
        return timer;
    }

    public void RemoveObject(Guid objectId)
    {
        lock (_lock)
        {
            if (_cache.ContainsKey(objectId))
            {
                var timer = _cache[objectId].ExpirationTimer;
                timer.Stop();
                timer.Dispose();
                _cache.Remove(objectId);
            }
        }
    }
    
    public void UpdateObject(Guid objectId, MultiStepSolver newData)
    {
        lock (_lock)
        {
            if (_cache.ContainsKey(objectId))
            {
                var (_, timer) = _cache[objectId];
                timer.Stop();
                timer.Dispose();

                var newTimer = CreateTimer(objectId);
                _cache[objectId] = (newData, newTimer);
            }
            else
            {
                throw new KeyNotFoundException("Object with the specified ID not found in the cache.");
            }
        }
    }

}
