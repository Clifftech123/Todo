

using TodoAPI.Contracts;
using TodoAPI.Models;

namespace TodoAPI.Interface
{
     public interface ITodoServices
 {
     Task<IEnumerable<Todo>> GetAllAsync();
     Task<Todo> GetByIdAsync(Guid id);
     Task CreateTodoAsync(CreateTodoRequest request);
     Task UpdateTodoAsync(Guid id, UpdateTodoRequest request);
     Task DeleteTodoAsync(Guid id);
 }
}