using Microondas.Domain.Heating.ValueObjects;

namespace Microondas.Domain.Services;

public sealed class HeatingOutputRenderer : IHeatingOutputRenderer
{
    private const string CompletionMessage = "Aquecimento concluído";

    public string RenderSegment(HeatingCharacter character, PowerLevel power) =>
        new(character.Value, power.Value);

    public string RenderCompletion(string accumulatedOutput) =>
        string.IsNullOrEmpty(accumulatedOutput)
            ? CompletionMessage
            : $"{accumulatedOutput} {CompletionMessage}";
}
