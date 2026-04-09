using Microondas.Domain.Heating.ValueObjects;

namespace Microondas.Domain.Programs;

public static class PredefinedProgramSeed
{
    public static IReadOnlyList<HeatingProgram> GetAll() =>
    [
        HeatingProgram.CreatePredefined(
            name: "Pipoca",
            food: "Pipoca (de micro-ondas)",
            timeSeconds: 180,
            power: 7,
            character: 'p',
            instructions:
            "Observar o barulho de estouros do milho, caso houver um intervalo de mais de 10 segundos entre um estouro e outro, interrompa o aquecimento."),

        HeatingProgram.CreatePredefined(
            name: "Leite",
            food: "Leite",
            timeSeconds: 300,
            power: 5,
            character: 'l',
            instructions:
            "Cuidado com aquecimento de líquidos, o choque térmico aliado ao movimento do recipiente pode causar fervura imediata causando risco de queimaduras."),

        HeatingProgram.CreatePredefined(
            name: "Carnes de boi",
            food: "Carne em pedaço ou fatias",
            timeSeconds: 840,
            power: 4,
            character: 'c',
            instructions:
            "Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima para o descongelamento uniforme."),

        HeatingProgram.CreatePredefined(
            name: "Frango",
            food: "Frango (qualquer corte)",
            timeSeconds: 480,
            power: 7,
            character: 'f',
            instructions:
            "Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima para o descongelamento uniforme."),

        HeatingProgram.CreatePredefined(
            name: "Feijão",
            food: "Feijão congelado",
            timeSeconds: 480,
            power: 9,
            character: 'j',
            instructions:
            "Deixe o recipiente destampado e em casos de plástico, cuidado ao retirar o recipiente pois o mesmo pode perder resistência em altas temperaturas.")
    ];

    public static IReadOnlySet<char> ReservedCharacters => new HashSet<char>(
        GetAll().Select(p => p.Character.Value))
    {
        HeatingCharacter.DefaultChar
    };
}