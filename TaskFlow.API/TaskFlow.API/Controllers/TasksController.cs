using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.Interfaces;
using TaskFlow.API.Models;
using TaskFlow.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TaskFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    private int GetCurrentUserId()
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.Parse(userId!);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTasks([FromQuery] TaskQueryParameters queryParameters)
    {
        int userId = GetCurrentUserId();

        List<TaskResponseDto> tasks = await _taskService.GetAllTasksAsync(userId, queryParameters);

        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskById(int id)
    {
        int userId = GetCurrentUserId();

        TaskResponseDto? task = await _taskService.GetTaskByIdAsync(id, userId);

        if (task == null)
        {
            return NotFound();
        }

        return Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(CreateTaskDto createTaskDto)
    {
        int userId = GetCurrentUserId();

        TaskResponseDto createdTask = await _taskService.CreateTaskAsync(createTaskDto, userId);

        return CreatedAtAction(
            nameof(GetTaskById),
            new { id = createdTask.Id },
            createdTask);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, UpdateTaskDto updateTaskDto)
    {
        int userId = GetCurrentUserId();

        TaskResponseDto? updatedTask = await _taskService.UpdateTaskAsync(id, updateTaskDto, userId);

        if (updatedTask == null)
        {
            return NotFound();
        }

        return Ok(updatedTask);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        int userId = GetCurrentUserId();

        bool deleted = await _taskService.DeleteTaskAsync(id, userId);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
