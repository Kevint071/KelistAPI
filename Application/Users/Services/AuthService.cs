using System.Security.Claims;
using Application.Common;
using Application.Data.Interfaces;
using Application.Data.Repositories;
using Application.TaskLists.Dtos;
using Application.Tasks.Dtos;
using Application.Users.Dtos;
using Domain.DomainErrors;
using Domain.TaskLists;
using Domain.Tasks;
using Domain.Users;
using Domain.ValueObjects.TaskItem;
using Domain.ValueObjects.TaskList;
using Domain.ValueObjects.User;
using ErrorOr;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Services
{
    public class AuthService(IUserRepository userRepository, IUnitOfWork unitOfWork, ITokenService tokenService) : IAuthService
    {
        private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        private readonly ITokenService _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));

        public async Task<ErrorOr<UserDTO>> RegisterAsync(RegisterUserDto request)
        {
            if (await _userRepository.ExistsByEmailAsync(request.Email)) return Errors.User.DuplicatedEmail;

            //var user = new User(
            //    new UserId(Guid.NewGuid()),
            //    PersonName.Create(request.Name).Value,
            //    LastName.Create(request.LastName).Value,
            //    Email.Create(request.Email).Value,
            //    HashPassword(request.Password)
            //);
            var validationResult = ValueObjectValidator.ValidateUserValueObjects(request.Email, request.Name, request.LastName);
            if (validationResult.IsError) return validationResult.Errors;

            var (name, lastname, email) = validationResult.Value;

            var userDto = new UserDTO(
                Guid.NewGuid(),
                name.Value,
                lastname.Value,
                email.Value,
                HashPassword(request.Password)
            );

            //var userDto = MapToDto(user);
            _userRepository.Add(userDto);
            await _unitOfWork.SaveChangesAsync();
            return userDto;
        }

        public async Task<ErrorOr<TokenResponseDto>> LoginAsync(LoginUserDto request)
        {
            var userDto = await _userRepository.GetByEmailAsync(request.Email);
            if (userDto == null || !VerifyPassword(request.Password, userDto.PasswordHash)) return Errors.User.InvalidCredentials;

            var user = MapToDomain(userDto);
            return await CreateTokenResponse(user, userDto);
        }

        public async Task<ErrorOr<TokenResponseDto>> RefreshTokensAsync(RefreshTokenRequestDto request)
        {
            var userDto = await _userRepository.GetByIdAsync(request.UserId);
            if (userDto == null || userDto.RefreshToken != request.RefreshToken || userDto.RefreshTokenExpiryTime <= DateTime.UtcNow) return Errors.User.InvalidRefreshToken;

            var user = MapToDomain(userDto);
            return await CreateTokenResponse(user, userDto);
        }

        private static string HashPassword(string password)
        {
            return new PasswordHasher<User>().HashPassword(null, password);
        }

        private static bool VerifyPassword(string password, string passwordHash)
        {
            return new PasswordHasher<User>().VerifyHashedPassword(null, passwordHash, password) != PasswordVerificationResult.Failed;
        }

        private async Task<TokenResponseDto> CreateTokenResponse(User user, UserDTO userDto)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.FullName),
                new(ClaimTypes.NameIdentifier, user.Id.Value.ToString()),
                new(ClaimTypes.Email, user.Email.Value)
            };

            var accessToken = _tokenService.CreateJwtToken(claims); // Usamos ITokenService con claims
            var refreshToken = _tokenService.GenerateRefreshToken();
            user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));

            userDto.RefreshToken = user.RefreshToken;
            userDto.RefreshTokenExpiryTime = user.RefreshTokenExpiryTime;

            _userRepository.Update(userDto);
            await _unitOfWork.SaveChangesAsync();
            return new TokenResponseDto(accessToken, refreshToken);
        }

        private static User MapToDomain(UserDTO userDto)
        {
            var user = new User(
                new UserId(userDto.Id),
                PersonName.Create(userDto.PersonName).Value,
                LastName.Create(userDto.LastName).Value,
                Email.Create(userDto.Email).Value,
                userDto.PasswordHash,
                userDto.RefreshToken,
                userDto.RefreshTokenExpiryTime
            );
            user.TaskLists.AddRange(userDto.TaskLists.Select(tl =>
            {
                var taskList = new TaskList(
                    new TaskListId(tl.Id),
                    TaskListName.Create(tl.TaskListName).Value
                );
                taskList.TaskItems.AddRange(tl.TaskItems.Select(ti => new TaskItem(
                    new TaskItemId(ti.Id),
                    Description.Create(ti.Description).Value,
                    ti.IsCompleted
                )));
                return taskList;
            }));
            return user;
        }

        private static UserDTO MapToDto(User user)
        {
            return new UserDTO(
                user.Id.Value,
                user.PersonName.Value,
                user.LastName.Value,
                user.Email.Value,
                user.PasswordHash,
                user.RefreshToken,
                user.RefreshTokenExpiryTime,
                [.. user.TaskLists.Select(tl => new TaskListDTO
                {
                    Id = tl.Id.Value,
                    TaskListName = tl.TaskListName.Value,
                    TaskItems = [.. tl.TaskItems.Select(ti => new TaskItemDTO
                    {
                        Id = ti.Id.Value,
                        Description = ti.Description.Value,
                        IsCompleted = ti.IsCompleted
                    })]
                })]
            );
        }
    }
}
