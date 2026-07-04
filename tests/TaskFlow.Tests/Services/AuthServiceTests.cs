using FluentAssertions;
using Moq;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Services.Implementations;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _sut = new AuthService(_userRepoMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUser_WhenUsernameIsAvailable()
    {
        var dto = new RegisterDto
        {
            Username = "testuser",
            FullName = "Test User",
            Password = "password123",
            ConfirmPassword = "password123",
            SecurityQuestion = "Pet name?",
            SecurityAnswer = "Fluffy"
        };

        _userRepoMock.Setup(r => r.UsernameExistsAsync(dto.Username)).ReturnsAsync(false);
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var result = await _sut.RegisterAsync(dto);

        result.Should().NotBeNull();
        result.Username.Should().Be(dto.Username);
        result.FullName.Should().Be(dto.FullName);
        _userRepoMock.Verify(r => r.AddAsync(It.Is<User>(u =>
            u.Username == dto.Username &&
            u.FullName == dto.FullName &&
            string.IsNullOrEmpty(u.PasswordHash) == false &&
            string.IsNullOrEmpty(u.SecurityAnswerHash) == false)), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrow_WhenUsernameExists()
    {
        var dto = new RegisterDto
        {
            Username = "existinguser",
            FullName = "Test",
            Password = "pass123456",
            ConfirmPassword = "pass123456",
            SecurityQuestion = "Q?",
            SecurityAnswer = "A"
        };

        _userRepoMock.Setup(r => r.UsernameExistsAsync(dto.Username)).ReturnsAsync(true);

        await _sut.Invoking(s => s.RegisterAsync(dto))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*já está em uso*");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnUser_WhenCredentialsAreValid()
    {
        var password = "correctpassword";
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            FullName = "Test User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            SecurityQuestion = "Q?",
            SecurityAnswerHash = BCrypt.Net.BCrypt.HashPassword("a")
        };

        _userRepoMock.Setup(r => r.GetByUsernameAsync("testuser")).ReturnsAsync(user);

        var result = await _sut.LoginAsync(new LoginDto { Username = "testuser", Password = password });

        result.Should().NotBeNull();
        result!.Username.Should().Be("testuser");
        result.FullName.Should().Be("Test User");
        _userRepoMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenPasswordIsWrong()
    {
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            FullName = "Test",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct"),
            SecurityQuestion = "Q?",
            SecurityAnswerHash = "hash"
        };

        _userRepoMock.Setup(r => r.GetByUsernameAsync("testuser")).ReturnsAsync(user);

        var result = await _sut.LoginAsync(new LoginDto { Username = "testuser", Password = "wrong" });

        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenUserNotFound()
    {
        _userRepoMock.Setup(r => r.GetByUsernameAsync("ghost")).ReturnsAsync((User?)null);

        var result = await _sut.LoginAsync(new LoginDto { Username = "ghost", Password = "any" });

        result.Should().BeNull();
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldSucceed_WhenCurrentPasswordIsCorrect()
    {
        var oldPassword = "oldpass";
        var user = new User
        {
            Id = 1,
            Username = "u",
            FullName = "F",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(oldPassword),
            SecurityQuestion = "Q?",
            SecurityAnswerHash = "hash"
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        var result = await _sut.ChangePasswordAsync(1, new ChangePasswordDto
        {
            CurrentPassword = oldPassword,
            NewPassword = "newpass123",
            ConfirmNewPassword = "newpass123"
        });

        result.Should().BeTrue();
        BCrypt.Net.BCrypt.Verify("newpass123", user.PasswordHash).Should().BeTrue();
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldReturnFalse_WhenCurrentPasswordIsWrong()
    {
        var user = new User
        {
            Id = 1,
            Username = "u",
            FullName = "F",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct"),
            SecurityQuestion = "Q?",
            SecurityAnswerHash = "hash"
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        var result = await _sut.ChangePasswordAsync(1, new ChangePasswordDto
        {
            CurrentPassword = "wrong",
            NewPassword = "newpass",
            ConfirmNewPassword = "newpass"
        });

        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetSecurityQuestionAsync_ShouldReturnQuestion_WhenUserExists()
    {
        var user = new User
        {
            Id = 1,
            Username = "u",
            FullName = "F",
            PasswordHash = "hash",
            SecurityQuestion = "What is your pet?",
            SecurityAnswerHash = "hash"
        };

        _userRepoMock.Setup(r => r.GetByUsernameAsync("u")).ReturnsAsync(user);

        var result = await _sut.GetSecurityQuestionAsync("u");

        result.Should().Be("What is your pet?");
    }

    [Fact]
    public async Task GetSecurityQuestionAsync_ShouldReturnNull_WhenUserNotFound()
    {
        _userRepoMock.Setup(r => r.GetByUsernameAsync("ghost")).ReturnsAsync((User?)null);

        var result = await _sut.GetSecurityQuestionAsync("ghost");

        result.Should().BeNull();
    }
}
