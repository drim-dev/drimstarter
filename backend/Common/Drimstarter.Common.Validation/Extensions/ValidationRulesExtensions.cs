using FluentValidation;

namespace Drimstarter.Common.Validation.Extensions;

public static class ValidationRulesExtensions
{
    public static IRuleBuilderOptions<T, string> NotEmpty<T>(this IRuleBuilder<T, string> ruleBuilder, string errorCode)
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage("{PropertyName} is empty")
            .WithErrorCode(errorCode);
    }

    public static IRuleBuilderOptions<T, string> MinimumLength<T>(this IRuleBuilder<T, string> ruleBuilder,
        int minimumLength, string errorCode) => ruleBuilder
        .MinimumLength(minimumLength)
            .WithMessage($"{{PropertyName}} length is less than {minimumLength}")
            .WithErrorCode(errorCode);

    public static IRuleBuilderOptions<T, string> MaximumLength<T>(this IRuleBuilder<T, string> ruleBuilder,
        int maximumLength, string errorCode) => ruleBuilder
        .MaximumLength(maximumLength)
            .WithMessage($"{{PropertyName}} length is greater than {maximumLength}")
            .WithErrorCode(errorCode);
}
