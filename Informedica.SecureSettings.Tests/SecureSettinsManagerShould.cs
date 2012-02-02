using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Informedica.SecureSettings.Tests
{
    [TestClass]
    public class SecureSettinsManagerShould
    {
        private const string Secret = "This is a secret";

        [TestMethod]
        public void SecureSettingsManagerShouldBeAbleToGetAndSetSecret()
        {
            var man = new SecureSettingsManager();
            man.SetSecret(Secret);

            Assert.IsTrue(man.HasSecret(Secret));
        }
    }
}
