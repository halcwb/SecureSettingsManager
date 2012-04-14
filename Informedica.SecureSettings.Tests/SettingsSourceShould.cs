using System;
using System.Collections.Generic;
using System.Linq;
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
                var source = new MyTestSettingSource(writers, new Dictionary<Enum, Func<string, Setting>>(), new Dictionary<Enum, Action<Setting>>());
                source.WriteSetting(setting);
                Isolate.Verify.WasCalledWithExactArguments(() => fakeWriter.Invoke(setting));

            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }

        [Isolated]
        [TestMethod]
        public void BeAbleToUseATestTypeSettingReaderToReadASetting()
        {
            var readers = new Dictionary<Enum, Func<string, Setting>>();
            Func<string, Setting> fakeReader = FakeReaders.Read;
            Isolate.WhenCalled(() => fakeReader.Invoke("Test")).ReturnRecursiveFake();
            readers.Add(MyTestSettingSource.SettingTypes.App, fakeReader);

            try
            {
                var source = new MyTestSettingSource(new Dictionary<Enum, Action<Setting>>(), readers, new Dictionary<Enum, Action<Setting>>());
                source.ReadSetting(MyTestSettingSource.SettingTypes.App, "Test");
                Isolate.Verify.WasCalledWithExactArguments(() => fakeReader.Invoke("Test"));
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
            var removers = new Dictionary<Enum, Action<Setting>>();
            Action<Setting> fakeRemover = FakeRemovers.Remove;
            var fakeSetting = Isolate.Fake.Instance<Setting>();
            Isolate.WhenCalled(() => fakeRemover.Invoke(fakeSetting)).IgnoreCall();
            removers.Add(MyTestSettingSource.SettingTypes.App, fakeRemover);

            var source = new MyTestSettingSource(new Dictionary<Enum, Action<Setting>>(),
                                             new Dictionary<Enum, Func<string, Setting>>(), removers);
            try
            {
                source.RemoveSetting(fakeSetting);
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
           
            source.WriteSetting(new Setting("Test", "Test", MyTestSettingSource.SettingTypes.App.ToString(), false));
            Assert.AreEqual("Test", source.ReadSetting(MyTestSettingSource.SettingTypes.App, "Test").Value);
        }

        [TestMethod]
        public void HaveCountPlusOneWhenSettingIsAdded()
        {
            var source = MyTestSettingSource.CreateMySettingSource();
            Assert.IsFalse(source.Any());

            source.WriteSetting(new Setting("Test", "Test", MyTestSettingSource.SettingTypes.App.ToString(), false));
            var count = source.Count();

            source.WriteSetting(new Setting("Test", "Test", MyTestSettingSource.SettingTypes.Conn.ToString(), false));
            Assert.AreEqual(count + 1, source.Count());
        }
        
        [TestMethod]
        public void HaveCountMinusOneWhenSettingIsRemoved()
        {
            var source = MyTestSettingSource.CreateMySettingSource();
            var setting = new Setting("Test", "Test", MyTestSettingSource.SettingTypes.App.ToString(), false);
            source.WriteSetting(setting);

            var count = source.Count();
            source.RemoveSetting(setting);

            Assert.AreEqual(count -1 , source.Count());
        }    

    }

    public static class FakeRemovers
    {
        public static void Remove(Setting setting) {}
    }

    public static class FakeReaders
    {
        public static Setting Read(string name)
        {
            return null;
        }
    }

    public static class FakeWriters
    {
        public static void Write(Setting setting)
        {
            
        }
    }
}
