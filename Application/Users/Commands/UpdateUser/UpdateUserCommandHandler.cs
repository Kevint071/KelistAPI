using Application.Common;
using Application.Data.Interfaces;
using Application.Data.Repositories;
using Application.Users.Dtos;
using Domain.DomainErrors;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Users.Commands.UpdateUser
{
    internal sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ErrorOr<UserDTO>>
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

        public async Task<ErrorOr<UserDTO>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            var existingUserDto = await _userRepository.GetByIdAsync(command.Id);
            if (existingUserDto == null) return Errors.User.NotFound;

            var validationResult = ValueObjectValidator.ValidateUserValueObjects(command.Email, command.Name, command.LastName);
            if (validationResult.IsError) return validationResult.Errors;

            var (name, lastName, email) = validationResult.Value;
            string passwordHash = existingUserDto.PasswordHash;

            var user = new User(new UserId(command.Id), name, lastName, email, passwordHash, existingUserDto.CreatedAt, existingUserDto.UpdatedAt, existingUserDto.RefreshToken, existingUserDto.RefreshTokenExpiryTime);

            existingUserDto.UpdateProfile(name.Value, lastName.Value, email.Value);
            existingUserDto.SetUpdatedAt(DateTime.UtcNow);
            user.NotifyUpdate();

            var events = user.GetDomainEvents();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (events.Count != 0)
            {
                await _domainEventPublisher.PublishEventAsync(events, cancellationToken);
                user.ClearDomainEvents();
            }

            return existingUserDto;
        }
    }
}
