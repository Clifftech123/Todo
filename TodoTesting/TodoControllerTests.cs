using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoAPI.Contracts;
using TodoAPI.Controllers;
using TodoAPI.Interface;
using TodoAPI.Models;

/// <summary>
/// Unit tests for the TodoController class.
/// </summary>
public class TodoControllerTests
{
    private readonly Mock<ITodoServices> _mockTodoServices;
    private readonly TodoController _controller;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoControllerTests"/> class.
    /// </summary>
    public TodoControllerTests()
    {
        _mockTodoServices = new Mock<ITodoServices>();
        _controller = new TodoController(_mockTodoServices.Object);
    }

    /// <summary>
    /// Tests that a valid CreateTodoAsync request returns an Ok result.
    /// </summary>
    /// <param name="title">The title of the todo item.</param>
    /// <param name="description">The description of the todo item.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Theory]
    [InlineData("Test", "Test")]
    public async Task CreateTodoAsync_ValidRequest_ReturnsOk(string title, string description)
    {
        // Arrange
        var request = new CreateTodoRequest { Title = title, Description = description };
        _mockTodoServices.Setup(s => s.CreateTodoAsync(request)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.CreateTodoAsync(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    /// <summary>
    /// Tests that an invalid CreateTodoAsync request returns a BadRequest result.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task CreateTodoAsync_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("Title", "Required");

        // Act
        var result = await _controller.CreateTodoAsync(new CreateTodoRequest());

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    /// <summary>
    /// Tests that GetAllAsync returns a list of todo items.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task GetAllAsync_ReturnsTodos()
    {
        // Arrange
        var todos = new List<Todo> { new Todo { Title = "Test" } };
        _mockTodoServices.Setup(s => s.GetAllAsync()).ReturnsAsync(todos);

        // Act
        var result = await _controller.GetAllAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<Todo>>(okResult.Value);
        Assert.Single(returnValue);
    }

    /// <summary>
    /// Tests that GetAllAsync returns an empty list when no todo items are found.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task GetAllAsync_NoTodos_ReturnsEmpty()
    {
        // Arrange
        var todos = new List<Todo>();
        _mockTodoServices.Setup(s => s.GetAllAsync()).ReturnsAsync(todos);

        // Act
        var result = await _controller.GetAllAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<Todo>>(okResult.Value);
        Assert.Empty(returnValue);
    }

    /// <summary>
    /// Tests that GetByIdAsync returns a todo item for a valid id.
    /// </summary>
    /// <param name="title">The title of the todo item.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Theory]
    [InlineData("Test")]
    public async Task GetByIdAsync_ValidId_ReturnsTodo(string title)
    {
        // Arrange
        var todo = new Todo { Title = title };
        _mockTodoServices.Setup(s => s.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(todo);

        // Act
        var result = await _controller.GetByIdAsync(Guid.NewGuid());

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Todo>(okResult.Value);
        Assert.Equal(title, returnValue.Title);
    }

    /// <summary>
    /// Tests that GetByIdAsync returns a NotFound result for an invalid id.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task GetByIdAsync_InvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockTodoServices.Setup(s => s.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Todo)null);

        // Act
        var result = await _controller.GetByIdAsync(Guid.NewGuid());

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    /// <summary>
    /// Tests that UpdateTodoAsync returns an Ok result for a valid id.
    /// </summary>
    /// <param name="title">The updated title of the todo item.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Theory]
    [InlineData("Updated")]
    public async Task UpdateTodoAsync_ValidId_ReturnsOk(string title)
    {
        // Arrange
        var request = new UpdateTodoRequest { Title = title };
        _mockTodoServices.Setup(s => s.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Todo());
        _mockTodoServices.Setup(s => s.UpdateTodoAsync(It.IsAny<Guid>(), request)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateTodoAsync(Guid.NewGuid(), request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    /// <summary>
    /// Tests that UpdateTodoAsync returns a NotFound result for an invalid id.
    /// </summary>
    /// <param name="title">The updated title of the todo item.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Theory]
    [InlineData("Updated")]
    public async Task UpdateTodoAsync_InvalidId_ReturnsNotFound(string title)
    {
        // Arrange
        var request = new UpdateTodoRequest { Title = title };
        _mockTodoServices.Setup(s => s.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Todo)null);

        // Act
        var result = await _controller.UpdateTodoAsync(Guid.NewGuid(), request);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    /// <summary>
    /// Tests that DeleteTodoAsync returns an Ok result for a valid id.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task DeleteTodoAsync_ValidId_ReturnsOk()
    {
        // Arrange
        _mockTodoServices.Setup(s => s.DeleteTodoAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteTodoAsync(Guid.NewGuid());

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    /// <summary>
    /// Tests that DeleteTodoAsync returns a NotFound result for an invalid id.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task DeleteTodoAsync_InvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockTodoServices.Setup(s => s.DeleteTodoAsync(It.IsAny<Guid>())).Throws(new Exception());

        // Act
        var result = await _controller.DeleteTodoAsync(Guid.NewGuid());

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }
}
