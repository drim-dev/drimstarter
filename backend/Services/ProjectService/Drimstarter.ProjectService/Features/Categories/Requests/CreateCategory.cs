using Drimstarter.Common.Database;
using Drimstarter.Common.Utils;
using Drimstarter.Common.Validation.Extensions;
using Drimstarter.ProjectService.Database;
using Drimstarter.ProjectService.Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Drimstarter.ProjectService.Features.Categories.Errors.CategoryValidationErrors;

namespace Drimstarter.ProjectService.Features.Categories.Requests;

public static class CreateCategory
{
    public class RequestValidator : AbstractValidator<CreateCategoryRequest>
    {
        public RequestValidator(ProjectDbContext db)
        {
            RuleFor(x => x.Name)
                .NotEmpty(NameEmpty)
                .MinimumLength(Category.NameMinLength, NameLessMinLength)
                .MaximumLength(Category.NameMaxLength, NameGreaterMaxLength)
                .MustAsync(async (name, cancellationToken) =>
                {
                    var capitalizedName = name.CapitalizeWords();
                    return !await db.Categories.AnyAsync(x => x.Name == capitalizedName, cancellationToken);
                })
                    .WithMessage("Name must not yet exist")
                    .WithErrorCode(NameAlreadyExists);
        }
    }

    public class RequestHandler : IRequestHandler<CreateCategoryRequest, CreateCategoryReply>
    {
        private readonly ProjectDbContext _db;
        private readonly IdFactory _idFactory;

        public RequestHandler(
            ProjectDbContext db,
            IdFactory idFactory)
        {
            _db = db;
            _idFactory = idFactory;
        }

        public async Task<CreateCategoryReply> Handle(CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            var category = new Category
            {
                Id = _idFactory.Create(),
                // TODO: write tests
                Name = request.Name.CapitalizeWords(),
            };

            _db.Categories.Add(category);
            await _db.SaveChangesAsync(cancellationToken);

            var categoryDto = new CategoryDto
            {
                Id = IdEncoding.Encode(category.Id),
                Name = category.Name,
            };

            return new() { Category = categoryDto };
        }
    }
}
