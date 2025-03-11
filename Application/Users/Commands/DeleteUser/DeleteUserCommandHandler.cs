using Application.Common;
using Application.Data.Interfaces;
using Application.Data.Repositories;
using Application.Users.Dtos;
using Domain.Events.User;
using Domain.Users;
using Domain.ValueObjects;
using ErrorOr;
using MediatR;

namespace Application.Users.Commands.DeleteUser
{
    internal sealed class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, ErrorOr<Unit>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDomainEventPublisher _domainEventPublisher;

        public DeleteUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IDomainEventPublisher domainEventPublisher)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _domainEventPublisher = domainEventPublisher ?? throw new ArgumentNullException(nameof(domainEventPublisher));
        }

        public async Task<ErrorOr<Unit>> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
        {
            if (await _userRepository.GetByIdAsync(command.Id) is not UserDTO userDto)
            {
                return Error.NotFound("User.NotFound", "The user with the provided Id was not found.");
            }

            var emailResult = Email.Create(userDto.Email);
            if (emailResult.IsError) return emailResult.Errors;
            var email = emailResult.Value;

            var user = new User(
                new UserId(userDto.Id),
                userDto.Name,
                userDto.LastName,
                email
            );

            user.NotifyDelete();

            _userRepository.Delete(userDto);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var events = user.GetDomainEvents();

            if (events.Count != 0)
            {
                await _domainEventPublisher.PublishEventAsync(events, cancellationToken);
                user.ClearDomainEvents();
            }

            return Unit.Value;
        }
    }
}
