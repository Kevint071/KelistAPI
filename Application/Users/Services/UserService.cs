﻿using Application.Common;
using Application.Data.Interfaces;
using Application.Data.Repositories;
using Application.TaskLists.Dtos;
using Application.Tasks.Dtos;
using Application.Users.Dtos;
using Domain.TaskLists;
using Domain.Users;
using Domain.Tasks;
using Domain.ValueObjects.TaskList;
using Domain.ValueObjects.User;

namespace Application.Users.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IDomainEventPublisher _domainEventPublisher;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork, IDomainEventPublisher domainEventPublisher)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _domainEventPublisher = domainEventPublisher ?? throw new ArgumentNullException(nameof(domainEventPublisher));
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            var userDto = await _userRepository.GetByIdAsync(id);
            return userDto == null ? null : MapToDomain(userDto);
        }

        public async Task<List<User>> GetAllAsync()
        {
            var userDtos = await _userRepository.GetAll();
            return [.. userDtos.Select(MapToDomain)];
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _userRepository.ExistsAsync(id);
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken)
        {
            var userDto = MapToDto(user);
            _userRepository.Add(userDto);
            user.NotifyCreate();
            var events = user.GetDomainEvents();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (events.Count != 0)
            {
                await _domainEventPublisher.PublishEventAsync(events, cancellationToken);
                user.ClearDomainEvents();
            }
        }

        public async Task UpdateAsync(User user, CancellationToken cancellationToken)
        {
            var userDto = MapToDto(user);
            _userRepository.Update(userDto);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            user.NotifyUpdate();
            var events = user.GetDomainEvents();

            if (events.Count != 0)
            {
                await _domainEventPublisher.PublishEventAsync(events, cancellationToken);
                user.ClearDomainEvents();
            }
        }

        public async Task DeleteAsync(UserDTO userDto, CancellationToken cancellationToken)
        {
            _userRepository.Delete(userDto);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            User user = MapToDomain(userDto);
            user.NotifyDelete();

            var events = user.GetDomainEvents();
            if (events.Count != 0)
            {
                await _domainEventPublisher.PublishEventAsync(events, cancellationToken);
                user.ClearDomainEvents();
            }
        }

        private static User MapToDomain(UserDTO userDto)
        {
            var user = new User(
                new UserId(userDto.Id),
                PersonName.Create(userDto.PersonName).Value,
                LastName.Create(userDto.LastName).Value,
                Email.Create(userDto.Email).Value
            );
            user.TaskLists.AddRange(userDto.TaskLists.Select(tl =>
            {
                var taskList = new TaskList(
                    new TaskListId(tl.Id),
                    TaskListName.Create(tl.TaskListName).Value
                );
                taskList.TaskItems.AddRange(tl.TaskItems.Select(ti => new TaskItem(
                    new TaskItemId(ti.Id),
                    ti.Description,
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
                [.. user.TaskLists.Select(tl => new TaskListDTO
                {
                    Id = tl.Id.Value,
                    TaskListName = tl.TaskListName.Value,
                    TaskItems = [.. tl.TaskItems.Select(ti => new TaskItemDTO
                    {
                        Id = ti.Id.Value,
                        Description = ti.Description,
                        IsCompleted = ti.IsCompleted
                    })]
                })]
            );
        }
    }
}
