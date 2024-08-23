using TaskManagement.Core.Commons.DTOs;
using TaskManagement.Core.Entities;

namespace TaskManagement.Core.Commons.Mappers
{
	public static class GroupMapper
	{
		public static GroupResponseDto ToDto(this Group group)
		{
			return new GroupResponseDto
			(
				Id: group.Id,
				Name: group.Name,
				Description: group.Description,
				ListIds: group.ListIds,
				CreatedBy: group.CreatedBy,
				UpdatedBy: group.UpdatedBy
			);
		}

		public static Group ToEntity(this GroupRequestDto dto)
		{
			if (string.IsNullOrWhiteSpace(dto.Id))
			{
				return new Group
				{
					Name = dto.Name,
					Description = dto.Description,
					ListIds = dto.ListIds,
					CreatedBy = dto.CreatedBy,
					UpdatedBy = dto.UpdatedBy
				};
			}

			return new Group
			{
				Id = dto.Id,
				Name = dto.Name,
				Description = dto.Description,
				ListIds = dto.ListIds,
				CreatedBy = dto.CreatedBy,
				UpdatedBy = dto.UpdatedBy
			};
		}

		public static Group ToEntity(this GroupResponseDto dto)
		{
			if (string.IsNullOrWhiteSpace(dto.Id))
			{
				return new Group
				{
					Name = dto.Name,
					Description = dto.Description,
					ListIds = dto.ListIds,
					CreatedBy = dto.CreatedBy,
					UpdatedBy = dto.UpdatedBy
				};
			}

			return new Group
			{
				Id = dto.Id,
				Name = dto.Name,
				Description = dto.Description,
				ListIds = dto.ListIds,
				CreatedBy = dto.CreatedBy,
				UpdatedBy = dto.UpdatedBy
			};
		}
	}
}