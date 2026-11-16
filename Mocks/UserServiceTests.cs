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
        private (IUserRepository, UserService) Arrange()
        {
            var userRepository = Substitute.For<IUserRepository>();
            var userService = new UserService(userRepository);
            return (userRepository, userService);
        }

        [Fact]
        public async Task CreateUserAsync_WithCreateUserRequestDto_ReturnsCreateUserResponseDto()
        {
            //  Arrange
            var (userRepository, userService) = Arrange();

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

            await userRepository.Received(1).AddAsync(Arg.Is<User>(u =>
                u.Username == requestDto.Username
                && u.Email == requestDto.Email
                && u.PasswordHash == requestDto.Password));
        }
    }
}
