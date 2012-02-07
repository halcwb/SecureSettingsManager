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
            using (new Testing(true))
            {
            var runner = new CommandRunner();
            Assert.IsTrue(runner.RunOptionWithArguments("set.setting", "\"test setting\""));
            }
        }

        [TestMethod]
        public void ReturnsSuccesWhenCompleted()
        {
            using (new Testing(true))
            {
                var runner = new CommandRunner();
                var result =
                    runner.GetCommandResult("set.setting testsetting \"this is a test setting\" get.setting testsetting");
                Assert.IsTrue(result.Contains("success"));
            }

        }

    }
}
