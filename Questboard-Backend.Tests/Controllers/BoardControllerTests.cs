using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuestBoard.Context;
using QuestBoard.Controllers;
using QuestBoard.Models;
using System.Reflection;
using System.Threading.Tasks;

namespace Questboard.Tests.Controllers
{
    [TestClass]
    public class BoardControllerTests : BaseControllerTests<BoardController>
    {
        private static readonly QuestboardContext context = MakeContext();

        private BoardController MakeController()
        {
            BoardController controller = new BoardController(context, MakeUserManager(), MakeLogger())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = identity,
                    }
                }
            };

            return controller;
        }

        [TestMethod]
        public async Task TestBoardGetsCreated()
        {
            BoardController controller = MakeController();

            IActionResult result = await controller.CreateAsync("Test");
            OkObjectResult okResult = result as OkObjectResult;

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.IsInstanceOfType(okResult.Value, typeof(object));

            dynamic response = new DynamicObjectResultValue(okResult.Value);

            Assert.IsNotNull(response);

            Assert.IsTrue(response.id is int);

            Board board = context.Boards.Find(response.id);
            Assert.AreEqual(board.BoardName, "Test");
        }

        [TestMethod]
        public async Task TestBoardAddsMember()
        {
            BoardController controller = MakeController();

            IActionResult result = await controller.CreateAsync("TestAdd");
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            dynamic response = new DynamicObjectResultValue((result as OkObjectResult).Value);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.id is int);

            result = await controller.AddMember(response.id, new BoardController.MemberEmail
            {
                Email = User.Email,
            });

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            response = new DynamicObjectResultValue((result as OkObjectResult).Value);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success is bool);
            Assert.IsTrue(response.Success);
        }
    }
}
