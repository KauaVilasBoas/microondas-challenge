using FluentValidation;
using Microondas.Domain.Heating.ValueObjects;

namespace Microondas.Application.Commands.Heating.StartHeating;

public sealed class StartHeatingCommandValidator : AbstractValidator<StartHeatingCommand>
{
    public StartHeatingCommandValidator()
    {
        When(x => x.TimeInSeconds.HasValue, () =>
        {
            RuleFor(x => x.TimeInSeconds!.Value)
                .InclusiveBetween(HeatingTime.ManualMinimumSeconds, HeatingTime.ManualMaximumSeconds)
                .WithMessage(
                    $"Tempo deve estar entre {HeatingTime.ManualMinimumSeconds} e {HeatingTime.ManualMaximumSeconds} segundos.");
        });

        When(x => x.PowerLevel.HasValue, () =>
        {
            RuleFor(x => x.PowerLevel!.Value)
                .InclusiveBetween(PowerLevel.Minimum, PowerLevel.Maximum)
                .WithMessage($"Potência deve estar entre {PowerLevel.Minimum} e {PowerLevel.Maximum}.");
        });

        When(x => x.HeatingChar.HasValue, () =>
        {
            RuleFor(x => x.HeatingChar!.Value)
                .Must(c => !char.IsWhiteSpace(c))
                .WithMessage("Caractere de aquecimento não pode ser um espaço em branco.");
        });
    }
}