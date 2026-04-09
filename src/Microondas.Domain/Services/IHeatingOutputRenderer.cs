using Microondas.Domain.Heating.ValueObjects;

namespace Microondas.Domain.Services;

public interface IHeatingOutputRenderer
{
    string RenderSegment(HeatingCharacter character, PowerLevel power);
    string RenderCompletion(string accumulatedOutput);
}