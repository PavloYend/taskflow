using TaskFlow.API.DTOs;
using TaskFlow.API.Models;

namespace TaskFlow.API.Interfaces;

public interface ITaskService
{
    Task<List<TaskResponseDto>> GetAllTasksAsync(int userId, TaskQueryParameters queryParameters);

    Task<TaskResponseDto?> GetTaskByIdAsync(int id, int userId);

    Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto createTaskDto, int userId);

    Task<TaskResponseDto?> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto, int userId);

    Task<bool> DeleteTaskAsync(int id, int userId);
}
