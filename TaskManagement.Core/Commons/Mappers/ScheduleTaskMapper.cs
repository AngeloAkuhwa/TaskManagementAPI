using TaskManagement.Core.Commons.DTOs;
using TaskManagement.Core.Entities;

namespace TaskManagement.Core.Commons.Mappers
{
	public static class ScheduleTaskMapper
	{
		public static ResponseScheduleTaskDto ToDto(this ScheduleTask task)
		{
			return new ResponseScheduleTaskDto
			(
				Id: task.Id,
				Title: task.Title,
				Description: task.Description,
				Status: task.Status,
				Priority: task.Priority,
				ListId: task.ListId,
				GroupId: task.GroupId,
				AssignedUserIds: task.AssignedUserIds,
				CreatedBy: task.CreatedBy,
				UpdatedBy: task.UpdatedBy,
				IsDeleted: task.IsDeleted
			);
		}

		public static ScheduleTask ToEntity(this RequestScheduleTaskDto dto)
		{
			if (string.IsNullOrWhiteSpace(dto.Id))
			{
				return new ScheduleTask
				{
					Title = dto.Title,
					Description = dto.Description,
					Status = dto.Status,
					Priority = dto.Priority,
					ListId = dto.ListId,
					GroupId = dto.GroupId,
					AssignedUserIds = dto.AssignedUserIds,
					CreatedBy = dto.CreatedBy,
					UpdatedBy = dto.UpdatedBy,
					IsDeleted = dto.IsDeleted
				};
			}

			return new ScheduleTask
			{
				Id = dto.Id,
				Title = dto.Title,
				Description = dto.Description,
				Status = dto.Status,
				Priority = dto.Priority,
				ListId = dto.ListId,
				GroupId = dto.GroupId,
				AssignedUserIds = dto.AssignedUserIds,
				CreatedBy = dto.CreatedBy,
				UpdatedBy = dto.UpdatedBy,
				IsDeleted = dto.IsDeleted
			};
		}

		public static ScheduleTask ToEntity(this ResponseScheduleTaskDto dto)
		{
			if (string.IsNullOrWhiteSpace(dto.Id))
			{
				return new ScheduleTask
				{
					Title = dto.Title,
					Description = dto.Description,
					Status = dto.Status,
					Priority = dto.Priority,
					ListId = dto.ListId,
					GroupId = dto.GroupId,
					AssignedUserIds = dto.AssignedUserIds,
					CreatedBy = dto.CreatedBy,
					UpdatedBy = dto.UpdatedBy,
					IsDeleted = dto.IsDeleted
				};
			}

			return new ScheduleTask
			{
				Id = dto.Id,
				Title = dto.Title,
				Description = dto.Description,
				Status = dto.Status,
				Priority = dto.Priority,
				ListId = dto.ListId,
				GroupId = dto.GroupId,
				AssignedUserIds = dto.AssignedUserIds,
				CreatedBy = dto.CreatedBy,
				UpdatedBy = dto.UpdatedBy,
				IsDeleted = dto.IsDeleted
			};
		}
	}
}