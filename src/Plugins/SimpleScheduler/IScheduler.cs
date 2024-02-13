
namespace SimpleScheduler;

public interface IScheduler
{
    IReadOnlyCollection<Slot>? Slots { get; }
    void AddEntries(IEnumerable<Slot> entries);
    void AddActivationHandler(Func<Slot, int, Task> handler);
    void AddErrorHandler(Func<Exception, Slot, Task> handler);
}
