using TaskFlow.API.DTOs;
using TaskFlow.API.Models;

namespace TaskFlow.API.Interfaces;

public interface ITaskRepository
{
    Task<List<TaskItem>> GetAllAsync(int userId, TaskQueryParameters queryParameters);

    Task<TaskItem?> GetByIdAsync(int id, int userId);

    Task AddAsync(TaskItem task);

    void Delete(TaskItem task);

    Task SaveChangesAsync();
}
