using Drimstarter.ProjectService.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Drimstarter.ProjectService.Features.Categories.Requests;

public class ListCategories
{
    public class RequestHandler(ProjectDbContext _db) : IRequestHandler<ListCategoriesRequest, ListCategoriesReply>
    {
        public async Task<ListCategoriesReply> Handle(ListCategoriesRequest request, CancellationToken cancellationToken)
        {
            var categories = await _db.Categories
                .OrderBy(x => x.Name)
                .Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                .ToListAsync(cancellationToken);

            var reply = new ListCategoriesReply
            {
                Categories = { categories }
            };

            return reply;
        }
    }
}
