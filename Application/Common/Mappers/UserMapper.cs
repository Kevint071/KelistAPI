using Application.TaskLists.Dtos;
using Application.Tasks.Dtos;
using Application.Users.Dtos;
using Domain.TaskLists;
using Domain.Tasks;
using Domain.Users;
using Domain.ValueObjects.TaskItem;
using Domain.ValueObjects.TaskList;
using Domain.ValueObjects.User;

namespace Application.Common.Mappers
{
    public static class UserMapper
    {
        public static User ToDomain(UserDTO userDto)
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
            user.SetRole(userDto.Role);
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

        public static UserDTO ToDto(User user)
        {
            return new UserDTO(
                user.Id.Value,
                user.PersonName.Value,
                user.LastName.Value,
                user.Email.Value,
                user.PasswordHash,
                user.Role,
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
