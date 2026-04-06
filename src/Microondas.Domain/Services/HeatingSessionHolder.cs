using Microondas.Domain.Heating;

namespace Microondas.Domain.Services;

public sealed class HeatingSessionHolder
{
    private HeatingSession? _current;
    private readonly object _lock = new();

    public HeatingSession? Current
    {
        get { lock (_lock) return _current; }
    }

    public void Set(HeatingSession session)
    {
        lock (_lock) _current = session;
    }

    public void Clear()
    {
        lock (_lock) _current = null;
    }
}
