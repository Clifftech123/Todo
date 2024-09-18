using Microsoft.AspNetCore.Mvc;
using TodoAPI.Contracts;
using TodoAPI.Interface;
using TodoAPI.Models;

namespace TodoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoServices _todoServices;

        public TodoController(ITodoServices todoServices)
        {
            _todoServices = todoServices;
        }

        // Creating new Todo Item
        [HttpPost]
        public async Task<IActionResult> CreateTodoAsync(CreateTodoRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _todoServices.CreateTodoAsync(request);
                return Ok(new { message = "Blog post successfully created" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the Todo Item", error = ex.Message });
            }
        }

        // Get all Todo Items
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var todos = await _todoServices.GetAllAsync();
                if (todos == null || !todos.Any())
                {
                    return Ok(new List<Todo>()); // Return an empty list if no items found
                }
                return Ok(todos); // Return the list of todos directly
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving all Todo items", error = ex.Message });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            try
            {
                var todo = await _todoServices.GetByIdAsync(id);
                if (todo == null)
                {
                    return NotFound(new { message = $"No Todo item with id {id} found" });
                }
                return Ok(todo); // Return the todo item directly
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred while retrieving the Todo item with id {id}", error = ex.Message });
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateTodoAsync(Guid id, UpdateTodoRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var todo = await _todoServices.GetByIdAsync(id);
                if (todo == null)
                {
                    return NotFound(new { message = $"Todo Item with id {id} not found" });
                }

                await _todoServices.UpdateTodoAsync(id, request);
                return Ok(new { message = $"Todo Item with id {id} successfully updated" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred while updating the Todo item with id {id}", error = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteTodoAsync(Guid id)
        {
            try
            {
                await _todoServices.DeleteTodoAsync(id);
                return Ok(new { message = $"Todo with id {id} successfully deleted" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred while deleting the Todo item with id {id}", error = ex.Message });
            }
        }
    }
}
