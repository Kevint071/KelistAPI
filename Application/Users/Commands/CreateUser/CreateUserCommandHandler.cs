using Application.Common;
using Application.Common.Mappers;
using Application.Data.Interfaces;
using Application.Data.Repositories;
using Application.Users.Dtos;
using Domain.DomainErrors;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Users.Commands.CreateUser
{
    internal sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ErrorOr<UserDTO>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordService _passwordService;
        private readonly IDomainEventPublisher _domainEventPublisher;

        public CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IPasswordService passwordService, IDomainEventPublisher domainEventPublisher)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
            _domainEventPublisher = domainEventPublisher ?? throw new ArgumentNullException(nameof(domainEventPublisher));
        }

        public async Task<ErrorOr<UserDTO>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            if (await _userRepository.ExistsByEmailAsync(command.Email)) return Errors.User.DuplicatedEmail;

            var validationResult = ValueObjectValidator.ValidateUserValueObjects(command.Email, command.Name, command.LastName);
            if (validationResult.IsError) return validationResult.Errors;

            var (name, lastname, email) = validationResult.Value;
            var user = new User(new UserId(Guid.NewGuid()), name, lastname, email, _passwordService.HashPassword(command.Password), DateTime.UtcNow, DateTime.UtcNow);

            var userDto = UserMapper.ToDto(user);
            _userRepository.Add(userDto);

            user.NotifyCreate();
            var events = user.GetDomainEvents();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (events.Count != 0)
            {
                await _domainEventPublisher.PublishEventAsync(events, cancellationToken);
                user.ClearDomainEvents();
            }
            return userDto;
        }
    }
}