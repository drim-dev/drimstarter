using AutoBogus;
using Drimstarter.AccountService;
using Drimstarter.ProjectService;

namespace Drimstarter.ApiGateway.Tests.Utils;

public static class FakeFactory
{
    public static T Create<T>() => AutoFaker.Generate<T>();

    public static CategoryDto CreateCategoryDto() =>
        new AutoFaker<CategoryDto>()
            .RuleFor(x => x.Id, f => f.Random.Word())
            .Generate();

    public static List<CategoryDto> CreateCategoryDtos(int count = 5) =>
        new AutoFaker<CategoryDto>()
            .RuleFor(x => x.Id, f => f.Random.Word())
            .Generate(count);

    public static AccountDto CreateAccountDto() =>
        new AutoFaker<AccountDto>()
            .RuleFor(d => d.Id, f => f.Random.Word())
            .RuleFor(d => d.Email, f => f.Internet.Email())
            .Generate();
}
