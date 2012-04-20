using Informedica.SecureSettings.CommandLine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace scsm.Tests
{
    [TestClass]
    public class CommandToolShould
    {

        [TestMethod]
        public void BeAbleToRunMethodByAlias()
        {
            var runner = new CommandRunner();
            Assert.IsTrue(runner.RunOptionWithArguments("set.setting", "testsetting \"this is a test setting\""));
        }

        [TestMethod]
        public void ReturnsSuccesWhenCompleted()
        {
            var runner = new CommandRunner();
            var result =
                runner.GetCommandResult("set.setting testsetting \"this is a test setting\" get.setting testsetting");
            Assert.IsTrue(result.Contains("success"));
        }

    }
}
