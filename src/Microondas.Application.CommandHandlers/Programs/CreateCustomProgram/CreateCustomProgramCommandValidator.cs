using FluentValidation;
using Microondas.Domain.Heating.ValueObjects;
using Microondas.Domain.Programs.ValueObjects;

namespace Microondas.Application.Commands.Programs.CreateCustomProgram;

public sealed class CreateCustomProgramCommandValidator : AbstractValidator<CreateCustomProgramCommand>
{
    public CreateCustomProgramCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome do programa é obrigatório.")
            .MaximumLength(ProgramName.MaxLength)
            .WithMessage($"Nome não pode exceder {ProgramName.MaxLength} caracteres.");

        RuleFor(x => x.Food)
            .NotEmpty().WithMessage("Descrição do alimento é obrigatória.")
            .MaximumLength(FoodDescription.MaxLength)
            .WithMessage($"Alimento não pode exceder {FoodDescription.MaxLength} caracteres.");

        RuleFor(x => x.TimeInSeconds)
            .InclusiveBetween(HeatingTime.ManualMinimumSeconds, HeatingTime.ManualMaximumSeconds)
            .WithMessage(
                $"Tempo deve estar entre {HeatingTime.ManualMinimumSeconds} e {HeatingTime.ManualMaximumSeconds} segundos.");

        RuleFor(x => x.Power)
            .InclusiveBetween(PowerLevel.Minimum, PowerLevel.Maximum)
            .WithMessage($"Potência deve estar entre {PowerLevel.Minimum} e {PowerLevel.Maximum}.");

        RuleFor(x => x.HeatingChar)
            .Must(c => !char.IsWhiteSpace(c))
            .WithMessage("Caractere de aquecimento não pode ser um espaço em branco.");

        When(x => x.Instructions is not null, () =>
        {
            RuleFor(x => x.Instructions!)
                .MaximumLength(InstructionText.MaxLength)
                .WithMessage($"Instruções não podem exceder {InstructionText.MaxLength} caracteres.");
        });
    }
}