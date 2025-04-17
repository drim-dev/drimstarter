using Drimstarter.ProjectService.Database;
using MediatR;

namespace Drimstarter.ProjectService.Features.Categories.Requests;

public static class DeleteCategory
{
    public class RequestHandler : IRequestHandler<DeleteCategoryRequest, DeleteCategoryReply>
    {
        private readonly ProjectDbContext _db;
        
        public RequestHandler(ProjectDbContext db)
        {
            _db = db;
        }
        
        public async Task<DeleteCategoryReply> Handle(DeleteCategoryRequest request, CancellationToken cancellationToken)
        {
            var category = await _db.Categories.FindAsync(request.Id, cancellationToken);

            if (category == null)
                throw new NullReferenceException();

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync(cancellationToken); 
            
            return new DeleteCategoryReply { };
        }
    }
}