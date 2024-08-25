using Drimstarter.AccountService.Database;
using Drimstarter.AccountService.Domain;
using Drimstarter.Common.Database;
using Drimstarter.Common.Validation.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Drimstarter.AccountService.Features.Accounts.Errors.AccountValidationErrors;

namespace Drimstarter.AccountService.Features.Accounts.Requests;

public static class CreateAccount
{
    public class RequestValidator : AbstractValidator<CreateAccountRequest>
    {
        public RequestValidator(AccountDbContext db)
        {
            RuleFor(r => r.Name)
                .NotEmpty(NameEmpty)
                .MinimumLength(Account.NameMinLength, NameLessMinLength)
                .MaximumLength(Account.NameMaxLength, NameGreaterMaxLength);

            RuleFor(r => r.Email)
                .EmailAddress()
                    .WithMessage("Email has an valid format")
                    .WithErrorCode(EmailInvalidFormat)
                .MaximumLength(Account.EmailMaxLength)
                .MustAsync(async (email, cancellationToken) =>
                {
                    return !await db.Accounts.AnyAsync(a => a.Email == email.ToLower(), cancellationToken);
                })
                    .WithMessage("Email already exists")
                    .WithErrorCode(EmailAlreadyExists);

            RuleFor(r => r.Password)
                .NotEmpty(PasswordEmpty)
                .MinimumLength(Account.PasswordMinLength, PasswordLessMinLength)
                .MaximumLength(Account.PasswordMaxLength, PasswordGreaterMaxLength)
                .Matches("[A-Z]")
                    .WithMessage("Password must contain at least one uppercase letter.")
                    .WithErrorCode(PasswordNoUppercase)
                .Matches("[a-z]")
                    .WithMessage("Password must contain at least one lowercase letter.")
                    .WithErrorCode(PasswordNoLowercase)
                .Matches("[0-9]")
                    .WithMessage("Password must contain at least one number.")
                    .WithErrorCode(PasswordNoNumber);
        }
    }

    public class RequestHandler : IRequestHandler<CreateAccountRequest, CreateAccountReply>
    {
        private readonly AccountDbContext _db;
        private readonly IdFactory _idFactory;

        public RequestHandler(
            AccountDbContext db,
            IdFactory idFactory)
        {
            _db = db;
            _idFactory = idFactory;
        }

        public async Task<CreateAccountReply> Handle(CreateAccountRequest request, CancellationToken cancellationToken)
        {
            var account = new Account
            {
                Id = _idFactory.Create(),
                Name = request.Name,
                Email = request.Email.ToLower(),
                PasswordHash = request.Password,
            };

            _db.Accounts.Add(account);
            await _db.SaveChangesAsync(cancellationToken);

            var accountDto = new AccountDto
            {
                Id = IdEncoding.Encode(account.Id),
                Name = account.Name,
                Email = account.Email,
            };

            return new CreateAccountReply
            {
                Account = accountDto,
            };
        }
    }
}
