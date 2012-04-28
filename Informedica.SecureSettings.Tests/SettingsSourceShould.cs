using System;
using System.Collections.Generic;
using System.Linq;
using Informedica.SecureSettings.Sources;
using Informedica.SecureSettings.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeMock.ArrangeActAssert;

namespace Informedica.SecureSettings.Tests
{
    [TestClass]
    public class SettingsSourceShould
    {
        [Isolated]
        [TestMethod]
        public void BeAbleToUseATestTypeSettingWriterToWriteASetting()
        {
            var writers = new Dictionary<Enum, Action<Setting>>();
            Action<Setting> fakeWriter = FakeWriters.Write;
            var setting = new Setting("Test", "Test", "Test", false);
            Isolate.WhenCalled(() => fakeWriter.Invoke(setting)).IgnoreCall();
            writers.Add(MyTestSettingSource.SettingTypes.App, fakeWriter);

            try
            {
                var source = new MyTestSettingSource(writers, new Dictionary<Enum, Func<Setting, bool>>());
                source.Add(setting);
                Isolate.Verify.WasCalledWithExactArguments(() => fakeWriter.Invoke(setting));

            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }


        [Isolated]
        [TestMethod]
        public void BeAbleToUseATestTypeSettingRemovertoRemoveASetting()
        {
            var removers = new Dictionary<Enum, Func<Setting, bool>>();
            Func<Setting, bool> fakeRemover = FakeRemovers.Remove;
            var fakeSetting = Isolate.Fake.Instance<Setting>();
            Isolate.WhenCalled(() => fakeRemover.Invoke(fakeSetting)).WillReturn(true);
            removers.Add(MyTestSettingSource.SettingTypes.App, fakeRemover);

            var source = new MyTestSettingSource(null, removers);
            try
            {
                source.Add(fakeSetting);
                Assert.IsTrue(source.Any(s => s.Key == fakeSetting.Key));
                source.Remove(fakeSetting);
                Isolate.Verify.WasCalledWithExactArguments(() => fakeRemover.Invoke(fakeSetting));
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }

        [TestMethod]
        public void BeAbleToBeCreatedWithAWriteAppSettingFunction()
        {
            var source = MyTestSettingSource.CreateMySettingSource();
           
            source.Add(new Setting("Test", "Test", MyTestSettingSource.SettingTypes.App.ToString(), false));
            var setting = source.SingleOrDefault(s => s.Type == "App" && s.Key == "Test");
            Assert.IsNotNull(setting);
            Assert.AreEqual("Test", setting.Value);
        }

        [TestMethod]
        public void HaveCountPlusOneWhenSettingIsAdded()
        {
            var source = MyTestSettingSource.CreateMySettingSource();
            Assert.IsFalse(source.Any());

            source.Add(new Setting("Test1", "Test", MyTestSettingSource.SettingTypes.App.ToString(), false));
            var count = source.Count();

            source.Add(new Setting("Test2", "Test", MyTestSettingSource.SettingTypes.Conn.ToString(), false));
            Assert.AreEqual(count + 1, source.Count());
        }
        
        [TestMethod]
        public void HaveCountMinusOneWhenSettingIsRemoved()
        {
            var source = MyTestSettingSource.CreateMySettingSource();
            var setting = new Setting("Test", "Test", MyTestSettingSource.SettingTypes.App.ToString(), false);
            source.Add(setting);

            var count = source.Count();
            source.Remove(setting);

            Assert.AreEqual(count -1 , source.Count());
        }
    
        [Isolated]
        [TestMethod]
        public void ThrowAnMethodNotFoundExceptionWhenMethodIsCalledButNotRegistered()
        {
            var source = SetupSettingSourceWithoutWritersOrReadersOrRemovers();
            var setting = new Setting("Test", "Test", MyTestSettingSource.SettingTypes.App.ToString(), false);

            try
            {
                source.Add(setting);

            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(MissingMethodException));
            }
        }

        private static MyTestSettingSource SetupSettingSourceWithoutWritersOrReadersOrRemovers()
        {
            var fakeRemovers = Isolate.Fake.Instance<IDictionary<Enum, Func<Setting, bool>>>();
            var fakeWriters = Isolate.Fake.Instance<IDictionary<Enum, Action<Setting>>>();
            var source = new MyTestSettingSource(fakeWriters, fakeRemovers);
            return source;
        }
    }

    public static class FakeRemovers
    {
        public static bool Remove(Setting setting)
        {
            return true;
        }
    }

    public static class FakeWriters
    {
        public static void Write(Setting setting)
        {
            
        }
    }
}
