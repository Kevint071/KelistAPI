using Application.Data.Interfaces;
using Application.Data.Repositories;
using Application.Users.Dtos;
using Application.Common.Mappers;
using Domain.DomainErrors;
using ErrorOr;
using MediatR;
using Application.Common;

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
            if (await _userRepository.GetByIdAsync(command.Id) is not UserDTO userDto) return Errors.User.NotFound;

            _userRepository.Delete(userDto);

            var user = UserMapper.ToDomain(userDto);
            user.NotifyDelete();
            var events = user.GetDomainEvents();

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
