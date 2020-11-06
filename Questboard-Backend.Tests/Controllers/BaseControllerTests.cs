using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using QuestBoard.Context;
using QuestBoard.Controllers;
using QuestBoard.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Questboard.Tests.Controllers
{
    public class BaseControllerTests<TController>
        where TController : ControllerBase
    {
        protected static User User {
            get
            {
                return new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "test@example.com",
                    EmailConfirmed = true,
                    UserName = "test@example.com",
                    TwoFactorEnabled = false,
                };
            }
            set { }
        }

        protected static readonly ClaimsPrincipal identity = new ClaimsPrincipal(
            new ClaimsIdentity(
                new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, User.UserName),
                    new Claim(ClaimTypes.NameIdentifier, User.Id),
                },
                "test"
            )
        );

        public static QuestboardContext MakeContext()
        {
            DbContextOptions<QuestboardContext> options = new DbContextOptionsBuilder<QuestboardContext>()
                .UseInMemoryDatabase(databaseName: "quest")
                .Options;

            return new QuestboardContext(options);
        }

        public static Mock<UserManager<TIDentityUser>> GetUserManagerMock<TIDentityUser>() where TIDentityUser : IdentityUser
        {
            return new Mock<UserManager<TIDentityUser>>(
                new Mock<IUserStore<TIDentityUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<TIDentityUser>>().Object,
                new IUserValidator<TIDentityUser>[0],
                new IPasswordValidator<TIDentityUser>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<TIDentityUser>>>().Object
            );
        }

        public static UserManager<User> MakeUserManager()
        {
            Mock<UserManager<User>> mockUserManager = GetUserManagerMock<User>();

            mockUserManager.Setup(u => u.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(User));
            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).Returns(Task.FromResult(User));
            mockUserManager.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult(User));

            return mockUserManager.Object;
        }

        public static ILogger<TController> MakeLogger() => new Mock<ILogger<TController>>().Object;
    }
}
