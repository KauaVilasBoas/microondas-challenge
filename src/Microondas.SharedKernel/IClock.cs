namespace Microondas.SharedKernel;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
