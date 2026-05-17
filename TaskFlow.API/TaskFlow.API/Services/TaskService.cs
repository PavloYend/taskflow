using System.Threading.Tasks;
using TaskFlow.API.DTOs;
using TaskFlow.API.Interfaces;
using TaskFlow.API.Models;

namespace TaskFlow.API.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<List<TaskResponseDto>> GetAllTasksAsync(int userId, TaskQueryParameters queryParameters)
    {
        List<TaskItem> tasks = await _taskRepository.GetAllAsync(userId, queryParameters);

        return tasks.Select(task => new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            IsCompleted = task.IsCompleted,
            CreatedAt = task.CreatedAt,
            DueDate = task.DueDate,
            Category = task.Category == null ? null : new CategoryDto
            {
                Id = task.Category.Id,
                Name = task.Category.Name
            }
        }).ToList();
    }

    public async Task<TaskResponseDto?> GetTaskByIdAsync(int id, int userId)
    {
        TaskItem? task = await _taskRepository.GetByIdAsync(id, userId);

        if (task == null)
        {
            return null;
        }

        return new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            IsCompleted = task.IsCompleted,
            CreatedAt = task.CreatedAt,
            DueDate = task.DueDate,
            Category = task.Category == null ? null : new CategoryDto
            {
                Id = task.Category.Id,
                Name = task.Category.Name
            }
        };
    }

    public async Task<TaskResponseDto?> CreateTaskAsync(CreateTaskDto createTaskDto, int userId)
    {
        TaskItem task = new TaskItem
        {
            Title = createTaskDto.Title,
            Description = createTaskDto.Description,
            DueDate = createTaskDto.DueDate,
            CreatedAt = DateTime.UtcNow,
            CategoryId = createTaskDto.CategoryId,
            UserId = userId
        };

        await _taskRepository.AddAsync(task);

        await _taskRepository.SaveChangesAsync();

        TaskItem? createdTask = await _taskRepository.GetByIdAsync(task.Id, userId);

        if (createdTask == null)
        {
            return null;
        }

        return new TaskResponseDto
        {
            Id = createdTask.Id,
            Title = createdTask.Title,
            Description = createdTask.Description,
            IsCompleted = createdTask.IsCompleted,
            CreatedAt = createdTask.CreatedAt,
            DueDate = createdTask.DueDate,

            Category = createdTask.Category == null ? null : new CategoryDto
                {
                    Id = createdTask.Category.Id,
                    Name = createdTask.Category.Name
                }
        };
    }

    public async Task<TaskResponseDto?> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto, int userId)
    {
        TaskItem? existingTask = await _taskRepository.GetByIdAsync(id, userId);

        if (existingTask == null)
        {
            return null;
        }

        existingTask.Title = updateTaskDto.Title;
        existingTask.Description = updateTaskDto.Description;
        existingTask.IsCompleted = updateTaskDto.IsCompleted;
        existingTask.DueDate = updateTaskDto.DueDate;
        existingTask.CategoryId = updateTaskDto.CategoryId;

        await _taskRepository.SaveChangesAsync();

        return new TaskResponseDto
        {
            Id = existingTask.Id,
            Title = existingTask.Title,
            Description = existingTask.Description,
            IsCompleted = existingTask.IsCompleted,
            CreatedAt = existingTask.CreatedAt,
            DueDate = existingTask.DueDate,
            Category = existingTask.Category == null ? null : new CategoryDto
            {
                Id = existingTask.Category.Id,
                Name = existingTask.Category.Name
            }
        };
    }

    public async Task<bool> DeleteTaskAsync(int id, int userId)
    {
        TaskItem? existingTask = await _taskRepository.GetByIdAsync(id, userId);

        if (existingTask == null)
        {
            return false;
        }

        _taskRepository.Delete(existingTask);

        await _taskRepository.SaveChangesAsync();

        return true;
    }
}
