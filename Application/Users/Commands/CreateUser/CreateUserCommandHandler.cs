using Application.Data.Repositories;
using Application.Data.Interfaces;
using Domain.Users;
using Domain.ValueObjects;
using ErrorOr;
using MediatR;
using Application.Users.Dtos;
using Application.Common;

namespace Application.Users.Commands.CreateUser
{
    internal sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ErrorOr<Unit>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDomainEventPublisher _domainEventPublisher;

        public CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IDomainEventPublisher domainEventPublisher)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _domainEventPublisher = domainEventPublisher ?? throw new ArgumentNullException(nameof(domainEventPublisher));
        }

        public async Task<ErrorOr<Unit>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var emailResult = Email.Create(command.Email);
            if (emailResult.IsError) return emailResult.Errors;

            var email = emailResult.Value;

            var user = new User(new UserId(Guid.NewGuid()), command.Name, command.LastName, email);
            user.NotifyCreate();

            var events = user.GetDomainEvents();

            var userDto = new UserDTO(
                user.Id.Value,
                user.Name!,
                user.LastName!,
                user.Email.Value
            );

            _userRepository.Add(userDto);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (events.Count != 0) {
                await _domainEventPublisher.PublishEventAsync(events, cancellationToken);
                user.ClearDomainEvents();
            }

            return Unit.Value;
        }
    }
}
