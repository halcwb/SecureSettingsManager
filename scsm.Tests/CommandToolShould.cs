using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using TypeMock.ArrangeActAssert;

namespace scsm.Tests
{
    [TestClass]
    public class CommandToolShould
    {
        private ArgumentProcessor<SecureSettingsManager> _processor;


        private static ArgumentProcessor<SecureSettingsManager> GetProcessor()
        {
            return new ArgumentProcessor<SecureSettingsManager>();
        }

        [Isolated]
        [TestMethod]
        public void UseAnArgumentsProcessorToRunTheCommands()
        {
            IList<String> fakeIList = Isolate.Fake.Instance<IList<String>>();
            _processor = GetProcessor();
            Isolate.WhenCalled(() => _processor.ProcessArguments(fakeIList)).WillReturn("");
            Isolate.WhenCalled(() => _processor.ListOptions()).WillReturn("Argument processor has been called");

            Assert.AreEqual("Argument processor has been called", new CommandRunner().GetCommandResult(""));
        }


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
