using Drimstarter.Common.Database;
using Drimstarter.Common.Errors.Exceptions;
using Drimstarter.Common.Grpc.Shared.Utils;
using Drimstarter.ProjectService.Database;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Drimstarter.ProjectService.Features.Projects.Requests;

public static class ListProjects
{
    public class RequestHandler : IRequestHandler<ListProjectsRequest, ListProjectsReply>
    {
        private readonly ProjectDbContext _db;
        private readonly LimitOffsetPaging _paging;

        public RequestHandler(
            ProjectDbContext db,
            LimitOffsetPaging paging)
        {
            _db = db;
            _paging = paging;
        }

        public async Task<ListProjectsReply> Handle(ListProjectsRequest request, CancellationToken cancellationToken)
        {
            var requestCategoryId = request.CategoryId?.ToLower();
            var requestSort = request.Sort?.ToLower();

            if (!_paging.TryGetMaxPageSize(request.MaxPageSize, out var maxPageSize))
            {
                throw new ValidationErrorsException("maxPageSize", "Invalid max page size", "validation:max_page_size_invalid");
            }

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

            if (!_paging.TryGetOffsetAndLimit(request.PageToken, maxPageSize, out var offset, out var limit,
                        requestCategoryId, requestSort))
            {
                throw new ValidationErrorsException("pageToken", "Invalid page token", "validation:page_token_invalid");
            }

            var projects = await query
                .Skip(offset!.Value)
                .Take(limit!.Value)
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

            var nextPageToken = _paging.CreateNextPageToken(projects.Count, offset.Value, limit.Value, requestCategoryId,
                requestSort);

            var reply = new ListProjectsReply
            {
                Projects = { projects },
                NextPageToken = nextPageToken,
            };

            return reply;
        }
    }
}
