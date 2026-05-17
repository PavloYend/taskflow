using Microsoft.EntityFrameworkCore;
using TaskFlow.API.Data;
using TaskFlow.API.DTOs;
using TaskFlow.API.Interfaces;
using TaskFlow.API.Models;

namespace TaskFlow.API.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly ApplicationDbContext _context;

    public TaskRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaskItem>> GetAllAsync(int userId, TaskQueryParameters queryParameters)
    {
        IQueryable<TaskItem> query = _context.Tasks.Include(t => t.Category).Where(t => t.UserId == userId);

        if (!string.IsNullOrWhiteSpace(queryParameters.SearchTerm))
        {
            query = query.Where(t => t.Title.Contains(queryParameters.SearchTerm));
        }

        if (queryParameters.IsCompleted.HasValue)
        {
            query = query.Where(t => t.IsCompleted == queryParameters.IsCompleted.Value);
        }

        //query = query.Skip((queryParameters.PageNumber - 1)* queryParameters.PageSize).Take(queryParameters.PageSize);

        return await query.ToListAsync();
    }

    public async Task<TaskItem?> GetByIdAsync(int id, int userId)
    {
        return await _context.Tasks.Include(t => t.Category).FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    }

    public async Task AddAsync(TaskItem task)
    {
        await _context.Tasks.AddAsync(task);
    }

    public void Delete(TaskItem task)
    {
        _context.Tasks.Remove(task);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
