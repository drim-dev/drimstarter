using Drimstarter.Common.Utils;
using Drimstarter.ProjectService.Database;
using Drimstarter.ProjectService.Domain;
using MediatR;

namespace Drimstarter.ProjectService.Features.Categories.Requests;

public static class CreateCategory
{
    public class RequestHandler(ProjectDbContext _db) : IRequestHandler<CreateCategoryRequest, CreateCategoryReply>
    {
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
