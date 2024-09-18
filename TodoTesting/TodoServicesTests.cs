using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TodoAPI.AppDataContext;
using TodoAPI.Contracts;
using TodoAPI.Models;
using TodoAPI.Services;

/// <summary>
/// Unit tests for the TodoServices class.
/// </summary>
public class TodoServicesTests : IDisposable
{
    private readonly TodoDbContext _context;
    private readonly Mock<ILogger<TodoServices>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly TodoServices _todoServices;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoServicesTests"/> class.
    /// </summary>
    public TodoServicesTests()
    {
        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var mockDbSettings = new Mock<IOptions<DbSettings>>();
        mockDbSettings.Setup(m => m.Value).Returns(new DbSettings { ConnectionString = "InMemory" });

        _context = new TodoDbContext(options, mockDbSettings.Object);

        _mockLogger = new Mock<ILogger<TodoServices>>();
        _mockMapper = new Mock<IMapper>();
        _todoServices = new TodoServices(_context, _mockLogger.Object, _mockMapper.Object);
    }

    /// <summary>
    /// Disposes the context and ensures the database is deleted.
    /// </summary>
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    /// <summary>
    /// Tests that a valid CreateTodoAsync request creates a new todo item.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task CreateTodoAsync_ShouldCreateTodo()
    {
        // Arrange
        var request = new CreateTodoRequest { Description = "Test", Title = "Test" };
        var todo = new Todo
        {
            id = Guid.NewGuid(),
            Title = "Test Todo",
            Description = "Testing todo",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            IsComplete = false,
            Priority = 10,
            DueDate = DateTime.Now
        };
        _mockMapper.Setup(m => m.Map<Todo>(It.IsAny<CreateTodoRequest>())).Returns(todo);

        // Act
        await _todoServices.CreateTodoAsync(request);

        // Assert
        var createdTodo = await _context.Todos.FirstOrDefaultAsync(t => t.Title == "Test Todo");
        Assert.NotNull(createdTodo);
        Assert.Equal("Test Todo", createdTodo.Title);
    }

    /// <summary>
    /// Tests that an invalid CreateTodoAsync request throws an exception.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task CreateTodoAsync_InvalidRequest_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _todoServices.CreateTodoAsync(null));
    }

    /// <summary>
    /// Tests that GetByIdAsync returns a todo item for a valid id.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsTodo()
    {
        // Arrange
        var todo = new Todo
        {
            id = Guid.NewGuid(),
            Title = "Test Todo",
            Description = "Testing todo",
        };

        _context.Todos.Add(todo);
        await _context.SaveChangesAsync();

        // Act
        var result = await _todoServices.GetByIdAsync(todo.id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(todo.id, result.id);
    }

    /// <summary>
    /// Tests that GetByIdAsync throws an exception for an invalid id.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task GetByIdAsync_InvalidId_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _todoServices.GetByIdAsync(Guid.NewGuid()));
    }

    /// <summary>
    /// Tests that UpdateTodoAsync updates a todo item for a valid id and request.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task UpdateTodoAsync_ValidIdAndRequest_UpdatesTodo()
    {
        // Arrange
        var todo = new Todo
        {
            id = Guid.NewGuid(),
            Title = "Test Todo",
            Description = "Testing todo",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            IsComplete = false,
            Priority = 10,
            DueDate = DateTime.Now
        };

        _context.Todos.Add(todo);
        await _context.SaveChangesAsync();

        var request = new UpdateTodoRequest
        {
            Title = "Updated Title",
            Description = "Updated Description",
            IsComplete = true,
            DueDate = DateTime.Now.AddDays(1),
            Priority = 5
        };

        // Act
        await _todoServices.UpdateTodoAsync(todo.id, request);

        // Assert
        var updatedTodo = await _context.Todos.FindAsync(todo.id);
        Assert.NotNull(updatedTodo);
        Assert.Equal(request.Title, updatedTodo.Title);
        Assert.Equal(request.Description, updatedTodo.Description);
        Assert.Equal(request.IsComplete, updatedTodo.IsComplete);
        Assert.Equal(request.DueDate, updatedTodo.DueDate);
        Assert.Equal(request.Priority, updatedTodo.Priority);
    }

    /// <summary>
    /// Tests that UpdateTodoAsync throws an exception for an invalid id.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task UpdateTodoAsync_InvalidId_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _todoServices.UpdateTodoAsync(Guid.NewGuid(), new UpdateTodoRequest()));
    }

    /// <summary>
    /// Tests that GetAllAsync returns all todo items.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task GetAllAsync_ReturnsAllTodos()
    {
        // Arrange
        var todo1 = new Todo
        {
            id = Guid.NewGuid(),
            Title = "Test Todo 1",
            Description = "Testing todo 1",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            IsComplete = false,
            Priority = 10,
            DueDate = DateTime.Now
        };
        var todo2 = new Todo
        {
            id = Guid.NewGuid(),
            Title = "Test Todo 2",
            Description = "Testing todo 2",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            IsComplete = false,
            Priority = 10,
            DueDate = DateTime.Now
        };

        _context.Todos.AddRange(todo1, todo2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _todoServices.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    /// <summary>
    /// Tests that GetAllAsync returns an empty list when no todo items are found.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task GetAllAsync_NoTodos_ReturnsEmptyList()
    {
        // Act
        var result = await _todoServices.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    /// <summary>
    /// Tests that DeleteTodoAsync deletes a todo item for a valid id.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task DeleteTodoAsync_ValidId_DeletesTodo()
    {
        // Arrange
        var todo = new Todo
        {
            id = Guid.NewGuid(),
            Title = "Test Todo",
            Description = "Testing todo",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            IsComplete = false,
            Priority = 10,
            DueDate = DateTime.Now
        };

        _context.Todos.Add(todo);
        await _context.SaveChangesAsync();

        // Act
        await _todoServices.DeleteTodoAsync(todo.id);

        // Assert
        var deletedTodo = await _context.Todos.FindAsync(todo.id);
        Assert.Null(deletedTodo);
    }

    /// <summary>
    /// Tests that DeleteTodoAsync throws an exception for an invalid id.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task DeleteTodoAsync_InvalidId_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _todoServices.DeleteTodoAsync(Guid.NewGuid()));
    }
}
