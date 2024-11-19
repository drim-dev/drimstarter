using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
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
        private static readonly byte[] EncryptionKey = [0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10];
        private static readonly byte[] Iv = [0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10];

        private readonly ProjectDbContext _db;

        public RequestHandler(ProjectDbContext db)
        {
            _db = db;
        }

        // TODO: refactor
        public async Task<ListProjectsReply> Handle(ListProjectsRequest request, CancellationToken cancellationToken)
        {
            var requestCategoryId = request.CategoryId?.ToLower();
            var requestSort = request.Sort?.ToLower();
            var maxPageSize = Math.Min(100, request.MaxPageSize ?? 10);

            var query = _db.Projects.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(requestCategoryId))
            {
                var categoryId = IdEncoding.Decode(requestCategoryId);
                query = query.Where(x => x.CategoryId == categoryId);
            }

            if (!string.IsNullOrEmpty(requestSort))
            {
                // TODO: generalize
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

            if (!TryGetOffsetAndLimit(request.PageToken, maxPageSize, requestCategoryId, requestSort,
                    out var offset, out var limit))
            {
                throw new ValidationErrorsException("page_token", "Invalid page token", "validation:page_token_invalid");
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

            var nextPageToken = CreateNextPageToken(projects.Count, offset.Value, limit.Value, requestCategoryId, requestSort);

            var reply = new ListProjectsReply
            {
                Projects = { projects },
                NextPageToken = nextPageToken,
            };

            return reply;
        }

        private static string? CreateNextPageToken(int count, int offset, int limit, string? requestCategoryId, string? requestSort)
        {
            if (count < limit)
            {
                return null;
            }

            var queryHash = Hash(requestCategoryId, requestSort);

            var pageToken = new PageToken(offset + limit, queryHash);

            var pageTokenString = JsonSerializer.Serialize(pageToken);

            return Encrypt(pageTokenString);
        }

        private static bool TryGetOffsetAndLimit(string? pageTokenString, int maxPageSize, string? categoryId,
            string? sort, out int? offset, out int? limit)
        {
            if (string.IsNullOrEmpty(pageTokenString))
            {
                offset = 0;
                limit = maxPageSize;
                return true;
            }

            if (!TryDecrypt(pageTokenString, out var decryptedPageTokenString))
            {
                offset = null;
                limit = null;
                return false;
            }

            try
            {
                var pageToken = JsonSerializer.Deserialize<PageToken>(decryptedPageTokenString!);

                var expectedHash = Hash(categoryId, sort);
                if (pageToken!.QueryHash != expectedHash)
                {
                    offset = null;
                    limit = null;
                    return false;
                }

                offset = pageToken.Offset;
                limit = maxPageSize;
                return true;
            }
            catch
            {
                offset = null;
                limit = null;
                return true;
            }
        }

        private static bool TryDecrypt(string cypherText, out string? plainText)
        {
            try
            {
                var cypherTextBytes = Base64Url.DecodeFromChars(cypherText);
                using var aes = Aes.Create();
                var decryptor = aes.CreateDecryptor(EncryptionKey, Iv);
                using var memoryStream = new MemoryStream(cypherTextBytes);
                using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                using var streamReader = new StreamReader(cryptoStream);
                plainText = streamReader.ReadToEnd();
                return true;
            }
            catch
            {
                plainText = null;
                return false;
            }
        }

        private static string Encrypt(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using var aes = Aes.Create();
            var encryptor = aes.CreateEncryptor(EncryptionKey, Iv);
            using var memoryStream = new MemoryStream();
            using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            return Base64Url.EncodeToString(memoryStream.ToArray());
        }

        private static int Hash(string? categoryId, string? sort) => HashStable($"{categoryId}-{sort}");

        private static int HashStable(string input)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToInt32(hash, 0);
        }

        private record PageToken(int Offset, int QueryHash);
    }
}
