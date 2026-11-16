using Application.Dto.request;
using Application.Dto.response;
using Application.Interface;
using Application.Services;
using Domain.Interface;
using Domain.Model;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    public class UserServiceTests
    {
        private (IUserRepository, UserService) CreateMockedIURepoAndUServ()
        {
            var userRepository = Substitute.For<IUserRepository>();
            var userService = new UserService(userRepository);
            return (userRepository, userService);
        }

        [Fact]
        public async Task CreateUserAsync_WithCreateUserRequestDto_ReturnsCreateUserResponseDto()
        {
            //  Arrange
            var (userRepository, userService) = CreateMockedIURepoAndUServ();

            var requestDto = new CreateUserRequestDto
            {
                Username = "testuser",
                Email = "testuser@example.com",
                Password = "password123"
            };

            User user = new User
            {
                Username = requestDto.Username,
                Email = requestDto.Email,
                PasswordHash = requestDto.Password,
            };

            Task<User> userTask = Task.FromResult(user);

            userRepository.AddAsync(Arg.Any<User>()).Returns(userTask);

            //  Act
            var result = await userService.CreateUserAsync(requestDto);

            //  Assert
            Assert.NotNull(result);
            Assert.Equal(requestDto.Username, result.Username);
            Assert.Equal(requestDto.Email, result.Email);
            Assert.Equal(Role.USER, result.Role);

            await userRepository.Received(1).AddAsync(Arg.Any<User>());
        }

        [Fact]
        public async Task GetUserByIdAsync_WithIntId_ReturnsGetUserResponseDto()
        {
            //  Arrange
            var (userRepository, userService) = CreateMockedIURepoAndUServ();

            int id = 1;

            User user = new User
            {
                Id = id,
                Username = "testuser",
                Email = "testuser@example.com",
                PasswordHash = "Hashedpassword123",
                Role = Role.USER,
                CreatedAt = DateTime.UtcNow
            };

            Task<User> userTask = Task.FromResult(user);

            userRepository.GetByIdAsync(Arg.Any<int>()).Returns(userTask);

            //  Act
            var result = await userService.GetUserByIdAsync(id);

            //  Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Username, result.Username);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.Role, result.Role);
            Assert.Equal(user.CreatedAt, result.CreatedAt);

            await userRepository.Received(1).GetByIdAsync(Arg.Any<int>());
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsListOfGetUserResponseDtos()
        {
            //  Arrange
            var (userRepository, userService) = CreateMockedIURepoAndUServ();

            User user = new User
            {
                Id = 1,
                Username = "testuser",
                Email = "testuser@example.com",
                PasswordHash = "Hashedpassword123",
                Role = Role.USER,
                CreatedAt = DateTime.UtcNow
            };

            Task<IEnumerable<User>> userTask = Task.FromResult(Enumerable.Repeat(user, 1));

            userRepository.GetAllAsync().Returns(userTask);

            //  Act
            var result = await userService.GetAllUsersAsync();

            //  Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.First().Id);
            Assert.Equal(user.Username, result.First().Username);
            Assert.Equal(user.Email, result.First().Email);
            Assert.Equal(user.Role, result.First().Role);
            Assert.Equal(user.CreatedAt, result.First().CreatedAt);

            await userRepository.Received(1).GetAllAsync();
        }
    }
}
