using Informedica.SecureSettings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;

namespace scsm.Tests
{
    [TestClass]
    public class CommandToolShould
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            ObjectFactory.Initialize(x => x.For<ISettingSource>().Use<Informedica.SecureSettings.Testing.TestSettingSource>());
        }

        [TestMethod]
        public void BeAbleToRunMethodByAlias()
        {
            var runner = new CommandRunner();
            Assert.IsTrue(runner.RunCommandWithArgument("set.setting", "test setting"));
        }

        [TestMethod]
        public void ReturnsSuccesWhenCompleted()
        {
            var runner = new CommandRunner();
            var result =
                runner.GetCommandResult("set.setting testsetting \"this is a test setting\" get.setting testsetting");

            Assert.IsTrue(result.Contains("success"));
        }

        [TestMethod]
        public void BeAbleToWriteTheSettingToASource()
        {
            var runner = new CommandRunner();
            Assert.IsTrue(runner.RunCommandWithArgument("set.source", "default"));
        }
    }
}
