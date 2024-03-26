using AutoMapper;
using backend.Core.Context;
using backend.Core.Dtos.ToDo;
using backend.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ToDoController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("Create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] ToDoCreateDto dto)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            string username = identity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var newTask = _mapper.Map<ToDo>(dto);

            newTask.Username = username;

            await _context.AddAsync(newTask);
            await _context.SaveChangesAsync();

            return Ok("New Task was created successfully");
        }

        [HttpGet]
        [Route("Get")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ToDoGetDto>>> Get()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            string username = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var tasks = await _context.ToDos.Where(toDo => toDo.Username == username).OrderByDescending(q => q.UpdatedAt).ToListAsync();

            var convertedTasks = _mapper.Map<IEnumerable<ToDoGetDto>>(tasks);

            return Ok(convertedTasks);
        }

        [HttpPatch]
        [Route("Edit{id}")]
        [Authorize]
        public async Task<IActionResult> Edit([FromRoute] string id, [FromBody] ToDoEditDto dto)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            string username = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            bool isUpdated = false;

            var task = await _context.ToDos.FirstOrDefaultAsync(todo => todo.Id == id);

            if (task is null)
                return NotFound("Task not found");

            if (task.Username != username)
                return Unauthorized("You have no permission to edit this task");

            if (dto.NewTitle != task.Title && dto.NewTitle != String.Empty)
            {
                isUpdated = true;
                task.Title = dto.NewTitle;
            }

            if (dto.NewDescription != task.Description && dto.NewDescription != String.Empty)
            {
                isUpdated = true;
                task.Description = dto.NewDescription;
            }

            if (isUpdated)
            {
                task.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
               
            return Ok(200);
        }

        [HttpPatch]
        [Route("MakeDone{id}")]
        [Authorize]
        public async Task<IActionResult> MakeDone([FromRoute] string id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            string username = identity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var task = await _context.ToDos.FirstOrDefaultAsync(toDo => toDo.Id == id);

            if (task is null)
                return NotFound("Task not found");

            if (task.Username != username)
                return Unauthorized("You have no permission to delete this task");

            task.IsDone = !task.IsDone;

            await _context.SaveChangesAsync();

            return Ok("Task is done");
        }

        [HttpPatch]
        [Route("MakeImportant{id}")]
        [Authorize]
        public async Task<IActionResult> MakeImportant([FromRoute] string id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            string username = identity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var task = await _context.ToDos.FirstOrDefaultAsync(toDo => toDo.Id == id);

            if (task is null)
                return NotFound("Task not found");

            if (task.Username != username)
                return Unauthorized("You have no permission to delete this task");

            task.IsImportant = !task.IsImportant;

            await _context.SaveChangesAsync();

            return Ok("Task is done");
        }

        [HttpDelete]
        [Route("Delete{id}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            string username = identity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var task = await _context.ToDos.FirstOrDefaultAsync(toDo => toDo.Id == id);

            if (task is null)
                return NotFound("Task not found");

            if (task.Username != username)
                return Unauthorized("You have no permission to delete this task");

            _context.ToDos.Remove(task);
            await _context.SaveChangesAsync();

            return Ok(200);
        }
    }
}