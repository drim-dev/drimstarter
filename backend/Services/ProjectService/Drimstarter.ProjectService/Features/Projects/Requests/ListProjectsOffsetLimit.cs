using Drimstarter.Common.Database;
using Drimstarter.Common.Errors.Exceptions;
using Drimstarter.Common.Grpc.Shared.Utils;
using Drimstarter.ProjectService.Database;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Drimstarter.ProjectService.Features.Projects.Requests;

public static class ListProjectsOffsetLimit
{
    public class RequestHandler : IRequestHandler<ListProjectsOffsetLimitRequest, ListProjectsOffsetLimitReply>
    {
        private readonly ProjectDbContext _db;

        public RequestHandler(ProjectDbContext db)
        {
            _db = db;
        }

        public async Task<ListProjectsOffsetLimitReply> Handle(ListProjectsOffsetLimitRequest request,
            CancellationToken cancellationToken)
        {
            var offset = request.Offset ?? 0;
            var limit = request.Limit ?? 10;

            var requestCategoryId = request.CategoryId?.ToLower();
            var requestSort = request.Sort?.ToLower();

            var query = _db.Projects.AsNoTracking().AsQueryable();

            // TODO: generalize
            if (!string.IsNullOrEmpty(requestCategoryId))
            {
                var categoryId = IdEncoding.Decode(requestCategoryId);
                query = query.Where(x => x.CategoryId == categoryId);
            }

            // TODO: generalize
            if (!string.IsNullOrEmpty(requestSort))
            {
                var sortedQuery = requestSort switch
                {
                    "-startdate" => query.OrderByDescending(x => x.StartDate),
                    "startdate" => query.OrderBy(x => x.StartDate),
                    "-enddate" => query.OrderByDescending(x => x.EndDate),
                    "enddate" => query.OrderBy(x => x.EndDate),
                    "-title" => query.OrderByDescending(x => x.Title),
                    "title" => query.OrderBy(x => x.Title),
                    "-fundinggoal" => query.OrderByDescending(x => x.FundingGoal),
                    "fundinggoal" => query.OrderBy(x => x.FundingGoal),
                    "-currentfunding" => query.OrderByDescending(x => x.CurrentFunding),
                    "currentfunding" => query.OrderBy(x => x.CurrentFunding),
                    _ => throw new ValidationErrorsException("sort", "Invalid sort value", "validation:sort_invalid")
                };

                // Sort by StartDate as a secondary sort to ensure deterministic order
                query = sortedQuery.ThenByDescending(x => x.StartDate);
            }
            else
            {
                query = query.OrderByDescending(x => x.StartDate);
            }

            var projects = await query
                .Skip(offset)
                .Take(limit)
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

            bool hasMore;
            if (projects.Count < limit)
            {
                hasMore = false;
            }
            else
            {
                hasMore = await query.Skip(offset + limit).AnyAsync(cancellationToken);
            }

            return new ListProjectsOffsetLimitReply
            {
                Projects = { projects },
                HasMore = hasMore,
            };
        }
    }
}
