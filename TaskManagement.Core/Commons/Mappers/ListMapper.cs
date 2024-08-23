using TaskManagement.Core.Commons.DTOs;
using TaskManagement.Core.Entities;

namespace TaskManagement.Core.Commons.Mappers
{
	public static class ListMapper
	{
		public static ResponseListDto ToDto(this List list)
		{
			return new ResponseListDto
			(
				Id: list.Id,
				Name: list.Name,
				Description: list.Description,
				GroupId: list.GroupId,
				TaskIds: list.TaskIds,
				CreatedBy: list.CreatedBy,
				UpdatedBy: list.UpdatedBy
			);
		}

		public static List ToEntity(this RequestListDto dto)
		{
			if (string.IsNullOrWhiteSpace(dto.Id))
			{
				return new List
				{
					Name = dto.Name,
					Description = dto.Description,
					GroupId = dto.GroupId,
					TaskIds = dto.TaskIds,
					CreatedBy = dto.CreatedBy,
					UpdatedBy = dto.UpdatedBy
				};
			}

			return new List
			{
				Id = dto.Id,
				Name = dto.Name,
				Description = dto.Description,
				GroupId = dto.GroupId,
				TaskIds = dto.TaskIds,
				CreatedBy = dto.CreatedBy,
				UpdatedBy = dto.UpdatedBy
			};
		}

		public static List ToEntity(this ResponseListDto dto)
		{
			if (string.IsNullOrWhiteSpace(dto.Id))
			{
				return new List
				{
					Name = dto.Name,
					Description = dto.Description,
					GroupId = dto.GroupId,
					TaskIds = dto.TaskIds,
					CreatedBy = dto.CreatedBy,
					UpdatedBy = dto.UpdatedBy
				};
			}

			return new List
			{
				Id = dto.Id,
				Name = dto.Name,
				Description = dto.Description,
				GroupId = dto.GroupId,
				TaskIds = dto.TaskIds,
				CreatedBy = dto.CreatedBy,
				UpdatedBy = dto.UpdatedBy
			};
		}

	}
}