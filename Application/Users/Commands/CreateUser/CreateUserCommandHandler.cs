using Application.Data.Repositories;
using Application.Data.Interfaces;
using Domain.Users;
using Domain.ValueObjects;
using ErrorOr;
using MediatR;
using Application.Users.Dtos;
using Application.Common;
using Application.Users.Services;

namespace Application.Users.Commands.CreateUser
{
    internal sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ErrorOr<Unit>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDomainEventPublisher _domainEventPublisher;
        private readonly UserService _userService;

        public CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IDomainEventPublisher domainEventPublisher, UserService userService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _domainEventPublisher = domainEventPublisher ?? throw new ArgumentNullException(nameof(domainEventPublisher));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<ErrorOr<Unit>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var validationResult = ValueObjectValidator.ValidateUserValueObjects(command.Email, command.Name, command.LastName);
            if (validationResult.IsError) return validationResult.Errors;

            var (name, lastname, email) = validationResult.Value;
            var user = new User(new UserId(Guid.NewGuid()), name, lastname, email);

            await _userService.AddAsync(user, cancellationToken);
            return Unit.Value;
        }
    }
}
