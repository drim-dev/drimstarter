using Drimstarter.Common.Database;
using Drimstarter.ProjectService.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Drimstarter.ProjectService.Features.Categories.Requests;

public class ListCategories
{
    public class RequestHandler : IRequestHandler<ListCategoriesRequest, ListCategoriesReply>
    {
        private readonly ProjectDbContext _db;

        public RequestHandler(ProjectDbContext db)
        {
            _db = db;
        }

        public async Task<ListCategoriesReply> Handle(ListCategoriesRequest request, CancellationToken cancellationToken)
        {
            var categories = await _db.Categories
                .OrderBy(x => x.Name)
                .Select(x => new CategoryDto
                {
                    Id = IdEncoding.Encode(x.Id),
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
