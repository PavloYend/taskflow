using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.Data;
using TaskFlow.API.Models;

namespace TaskFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class CategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(
        ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory(
        TaskCategory category)
    {
        await _context.Categories.AddAsync(category);

        await _context.SaveChangesAsync();

        return Ok(category);
    }

    [HttpGet]
    public IActionResult GetCategories()
    {
        return Ok(_context.Categories.ToList());
    }
}
