using Informedica.SecureSettings.CommandLine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using TypeMock.ArrangeActAssert;

namespace scsm.Tests
{
    [TestClass]
    public class CommandToolShould
    {

        [TestMethod]
        public void UseASecureSettingsSourceManagerToSetASecretKey()
        {
            var runner = new CommandRunner();
            SecureSettingsManager fakeManager = new TestManager();
            ObjectFactory.Inject(typeof(SecureSettingsManager), fakeManager);

            Assert.AreEqual("method was called", runner.GetCommandResult("set.secret this is a secret"));
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
