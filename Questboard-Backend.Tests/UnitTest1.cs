using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuestBoard.Models.UserModels;

namespace QuestBoard.Tests
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
