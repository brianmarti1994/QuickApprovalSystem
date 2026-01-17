using Application.Abstractions;
using Application.Auth;
using Domain.Users;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickApproval.Tests.Auth
{
    public class LoginHandlerTests
    {
        [Fact]
        public async Task Login_Returns_Token_When_Credentials_Are_Valid()
        {
            // Arrange
            var user = User.Create("a@b.com", "Alice", BCrypt.Net.BCrypt.HashPassword("1234"));
            user.AssignRole(Role.Employee);

            var db = new Mock<IAppDbContext>();
            db.Setup(x => x.Users).ReturnsDbSet(new[] { user });

            var jwt = new Mock<IJwtTokenService>();
            jwt.Setup(x => x.CreateToken(user.Id, user.Email, It.IsAny<IEnumerable<Role>>()))
               .Returns("TOKEN");

            var handler = new LoginHandler(db.Object, jwt.Object);

            // Act
            var result = await handler.Handle(new LoginCommand("a@b.com", "1234"), CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value!.Token.Should().Be("TOKEN");
            result.Value.Roles.Should().Contain(Role.Employee);
        }

        [Fact]
        public async Task Login_Fails_When_User_Not_Found()
        {
            // Arrange
            var db = new Mock<IAppDbContext>();
            db.Setup(x => x.Users).ReturnsDbSet(Array.Empty<User>());

            var jwt = new Mock<IJwtTokenService>();
            var handler = new LoginHandler(db.Object, jwt.Object);

            // Act
            var result = await handler.Handle(new LoginCommand("missing@b.com", "1234"), CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Code.Should().Be("auth.invalid");
        }

        [Fact]
        public async Task Login_Fails_When_Password_Invalid()
        {
            var user = User.Create("a@b.com", "Alice", BCrypt.Net.BCrypt.HashPassword("RIGHT"));
            user.AssignRole(Role.Employee);

            var db = new Mock<IAppDbContext>();
            db.Setup(x => x.Users).ReturnsDbSet(new[] { user });

            var jwt = new Mock<IJwtTokenService>();
            var handler = new LoginHandler(db.Object, jwt.Object);

            var result = await handler.Handle(new LoginCommand("a@b.com", "WRONG"), CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Error!.Code.Should().Be("auth.invalid");
        }
    }
}
