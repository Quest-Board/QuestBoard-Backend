using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuestBoard_Backend.Models.UserModels;

namespace Questboard_Backend.Tests
{
    [TestClass]
    public class UnitTest1
    {
        User user = new User();

        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual(true, true);
        }
    }
}
