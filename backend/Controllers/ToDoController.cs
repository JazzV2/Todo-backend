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

        /*[HttpPatch]
        [Route("Edit{id}")]
        [Authorize]
        public async Task<IActionResult> Edit([FromRoute] string id)*/
    }
}
