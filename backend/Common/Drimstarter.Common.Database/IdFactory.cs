using IdGen;
using IdGen.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Drimstarter.Common.Database;

public class IdFactory
{
    private readonly IdGenerator _idGenerator;

    public IdFactory(IdGenerator idGenerator)
    {
        _idGenerator = idGenerator;
    }

    public long Create() => _idGenerator.CreateId();
}

public static class IdGeneratorExtensions
{
    public static IServiceCollection AddIdFactory(this IServiceCollection services, int generatorId)
    {
        services.AddIdGen(generatorId, () => new IdGeneratorOptions
        {
            IdStructure = new IdStructure(52, 8, 3)
        });

        services.AddSingleton<IdFactory>();

        return services;
    }
}
