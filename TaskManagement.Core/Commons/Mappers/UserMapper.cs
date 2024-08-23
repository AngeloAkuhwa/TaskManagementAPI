using TaskManagement.Core.Commons.DTOs;
using TaskManagement.Core.Entities;

namespace TaskManagement.Core.Commons.Mappers
{
	public static class UserMapper
	{
		public static ResponseUserDto ToDto(this User user)
		{
			return new ResponseUserDto
			(
				Id: user.Id,
				Username: user.Username,
				Email: user.Email,
				FirstName: user.FirstName,
				LastName: user.LastName,
				AssignedTaskIds: user.AssignedTaskIds
			);
		}

		public static User ToEntity(this RequestUserDto dto)
		{
			if (string.IsNullOrWhiteSpace(dto.Id))
			{
				return new User
				{
					Username = dto.Username,
					Email = dto.Email,
					FirstName = dto.FirstName,
					LastName = dto.LastName,
					AssignedTaskIds = dto.AssignedTaskIds
				};
			}

			return new User
			{
				Id = dto.Id,
				Username = dto.Username,
				Email = dto.Email,
				FirstName = dto.FirstName,
				LastName = dto.LastName,
				AssignedTaskIds = dto.AssignedTaskIds
			};
		}

		public static User ToEntity(this ResponseUserDto dto)
		{
			if (string.IsNullOrWhiteSpace(dto.Id))
			{
				return new User
				{
					Username = dto.Username,
					Email = dto.Email,
					FirstName = dto.FirstName,
					LastName = dto.LastName,
					AssignedTaskIds = dto.AssignedTaskIds
				};
			}

			return new User
			{
				Id = dto.Id,
				Username = dto.Username,
				Email = dto.Email,
				FirstName = dto.FirstName,
				LastName = dto.LastName,
				AssignedTaskIds = dto.AssignedTaskIds
			};
		}

	}
}