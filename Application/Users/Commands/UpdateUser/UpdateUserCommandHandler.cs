using Application.Common;
using Application.Data.Interfaces;
using Application.Data.Repositories;
using Application.Users.Dtos;
using Domain.Users;
using Domain.ValueObjects;
using ErrorOr;
using MediatR;

namespace Application.Users.Commands.UpdateUser
{
    internal sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ErrorOr<Unit>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDomainEventPublisher _domainEventPublisher;

        public UpdateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IDomainEventPublisher domainEventPublisher)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _domainEventPublisher = domainEventPublisher ?? throw new ArgumentNullException(nameof(domainEventPublisher));
        }

        public async Task<ErrorOr<Unit>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            if (!await _userRepository.ExistsAsync(command.Id))
            {
                return Error.NotFound("User.NotFound", "The user with the provided Id was not found.");
            }

            var emailResult = Email.Create(command.Email);
            if (emailResult.IsError) return emailResult.Errors;
            var email = emailResult.Value;

            var user = new User(new UserId(command.Id), command.Name, command.LastName, email);
            user.NotifyUpdate();
            var events = user.GetDomainEvents();

            var userDto = new UserDTO(
                command.Id,
                user.Name!,
                user.LastName!,
                user.Email.Value
            );

            _userRepository.Update(userDto);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (events.Count != 0)
            {
                await _domainEventPublisher.PublishEventAsync(events, cancellationToken);
                user.ClearDomainEvents();
            }

            return Unit.Value;

        }
    }
}
