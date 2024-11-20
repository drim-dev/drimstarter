using Drimstarter.Common.Database;
using Drimstarter.Common.Grpc.Shared.Utils;
using Drimstarter.ProjectService.Database;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Drimstarter.ProjectService.Features.Projects.Requests;

public static class ListProjectsKeyset
{
    public class RequestHandler : IRequestHandler<ListProjectsKeysetRequest, ListProjectsKeysetReply>
    {
        private readonly ProjectDbContext _db;

        public RequestHandler(ProjectDbContext db)
        {
            _db = db;
        }

        public async Task<ListProjectsKeysetReply> Handle(ListProjectsKeysetRequest request,
            CancellationToken cancellationToken)
        {
            var startAfter = request.StartAfter is null
                ? long.MaxValue
                : IdEncoding.Decode(request.StartAfter);
            var pageSize = request.PageSize ?? 10;

            var requestCategoryId = request.CategoryId?.ToLower();

            var query = _db.Projects.AsNoTracking().AsQueryable();

            // TODO: generalize
            if (!string.IsNullOrEmpty(requestCategoryId))
            {
                var categoryId = IdEncoding.Decode(requestCategoryId);
                query = query.Where(x => x.CategoryId == categoryId);
            }

            var projects = await query
                .OrderByDescending(x => x.Id)
                .Where(x => x.Id < startAfter)
                .Take(pageSize)
                .Select(x => new ProjectDto
                {
                    Id = IdEncoding.Encode(x.Id),
                    Title = x.Title,
                    Description = x.Description,
                    Story = x.Story,
                    FundingGoal = x.FundingGoal.ToGrpcDecimal(),
                    CurrentFunding = x.CurrentFunding.ToGrpcDecimal(),
                    StartDate = Timestamp.FromDateTime(x.StartDate),
                    EndDate = Timestamp.FromDateTime(x.EndDate),
                })
                .ToListAsync(cancellationToken);

            string? nextStartAfter = null;
            if (projects.Count == pageSize)
            {
                nextStartAfter = projects.Last().Id;
            }

            return new ListProjectsKeysetReply
            {
                Projects = { projects },
                NextStartAfter = nextStartAfter,
            };
        }
    }
}
