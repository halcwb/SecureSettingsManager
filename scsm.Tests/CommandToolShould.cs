using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace scsm.Tests
{
    [TestClass]
    public class CommandToolShould
    {

        private static void OptionsContains(string option)
        {
            Assert.IsTrue((new CommandRunner()).GetCommandResult("").Contains(option));
        }

        [TestMethod]
        public void ReturnOptionsAreWhenNoArgumentsGiven()
        {
            var option = "Options are";
            OptionsContains(option);
        }

        [TestMethod]
        public void HaveAnOptionSetSetting()
        {
            var option = "set.setting";
            OptionsContains(option);
        }

        [TestMethod]
        public void HaveAnOptionGetSetting()
        {
            var option = "get.setting";
            OptionsContains(option);
        }

        [TestMethod]
        public void ReturnSuccessAfterSettingAndGettingASetting()
        {
            var runner = new CommandRunner();
            var result =
                runner.GetCommandResult("set.setting testsetting test \"this is a test setting\" get.setting testsetting test");
            Assert.IsTrue(result.Contains("success"));
        }

    }
}
