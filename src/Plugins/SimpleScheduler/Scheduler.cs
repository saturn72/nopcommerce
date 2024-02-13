namespace SimpleScheduler;

using Timer = System.Timers.Timer;
public class Scheduler : IScheduler, IDisposable
{
    private LinkedList<Slot>? _slots;
    private Timer? _timer;
    private ICollection<Func<Slot, int, Task>>? _activationHandlers;
    private ICollection<Func<Exception, Slot, Task>>? _errorHandlers;
    private event Func<Slot, int, Task>? OnHandleEvent;
    private event Func<Exception, Slot, Task>? OnErrorEvent;

    public IReadOnlyCollection<Slot>? Slots => _slots;
    public void AddActivationHandler(Func<Slot, int, Task> handler)
    {
        (_activationHandlers ??= new List<Func<Slot, int, Task>>()).Add(handler);
        OnHandleEvent += (slot, slotsLeft) => handler(slot, slotsLeft);
    }
    public void AddErrorHandler(Func<Exception, Slot, Task> handler)
    {
        (_errorHandlers ??= new List<Func<Exception, Slot, Task>>()).Add(handler);
        OnErrorEvent += (ex, s) => handler(ex, s);
    }

    protected void OrganizeTimer()
    {
        if (_timer == null)
        {
            _timer = new();
            _timer.Elapsed += (s, e) => OnTimerElapsedAsync();
        }

        if (_slots == null)
            return;

        var f = _slots.First();
        var interval = f.ExecutedOnUtc.Subtract(DateTime.UtcNow).TotalMilliseconds;
        if (interval < 0)
            interval = 0;

        if (_timer.Enabled)
            _timer.Stop();
        _timer.Interval = interval;
        _timer.Start();
    }

    private void OnTimerElapsedAsync()
    {
        _timer.Stop();

        var current = _slots?.First.Value;
        _slots.RemoveFirst();
        OrganizeTimer();

        try
        {
            if (OnHandleEvent != null)
            {
                _ = OnHandleEvent(current, _slots.Count);
            }
        }
        catch (Exception ex)
        {
            if (OnErrorEvent != null)
                _ = OnErrorEvent(ex, current);
        }
    }

    public void AddEntries(IEnumerable<Slot> entries)
    {
        if (entries == null || !entries.Any())
            return;

        var ordered = entries.OrderBy(x => x.ExecutedOnUtc);

        var c = (_slots ??= new()).First;
        _timer?.Stop();

        for (var i = 0; i < ordered.Count(); i++)
        {
            var cur = ordered.ElementAt(i);

            while (c != null && c.Value.ExecutedOnUtc <= cur.ExecutedOnUtc)
                c = c.Next;
            if (c == null)
                c = _slots.AddLast(cur);
            else
                _slots.AddBefore(c, cur);
        }
        OrganizeTimer();
    }

    public void Dispose()
    {
        if (_errorHandlers != null && _errorHandlers.Any())
        {
            foreach (var handler in _errorHandlers)
            {
                OnErrorEvent -= handler;
                _errorHandlers.Remove(handler);
            }
        }
        if (_activationHandlers != null && _activationHandlers.Any())
        {
            foreach (var handler in _activationHandlers)
            {
                OnHandleEvent -= handler;
                _activationHandlers.Remove(handler);
            }
        }
    }
}
