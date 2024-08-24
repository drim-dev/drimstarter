using Drimstarter.Common.Utils;
using Drimstarter.Common.Validation.Extensions;
using Drimstarter.ProjectService.Database;
using Drimstarter.ProjectService.Domain;
using FluentValidation;
using MediatR;
using static Drimstarter.ProjectService.Features.Categories.Errors.CategoriesValidationErrors;

namespace Drimstarter.ProjectService.Features.Categories.Requests;

public static class CreateCategory
{
    public class RequestValidator : AbstractValidator<CreateCategoryRequest>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty(NameMustNotBeEmpty)
                .MinimumLength(Category.NameMinLength, NameMustBeGreaterOrEqualMinLength)
                .MaximumLength(Category.NameMaxLength, NameMustBeLessOrEqualMaxLength);

            // TODO: check that category name is unique
        }
    }

    public class RequestHandler : IRequestHandler<CreateCategoryRequest, CreateCategoryReply>
    {
        private readonly ProjectDbContext _db;

        public RequestHandler(ProjectDbContext db)
        {
            _db = db;
        }

        public async Task<CreateCategoryReply> Handle(CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            var category = new Category
            {
                // TODO: write tests
                Name = request.Name.CapitalizeWords(),
            };

            _db.Categories.Add(category);
            await _db.SaveChangesAsync(cancellationToken);

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
            };

            return new() { Category = categoryDto };
        }
    }
}
