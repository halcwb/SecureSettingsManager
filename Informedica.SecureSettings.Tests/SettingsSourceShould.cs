using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeMock.ArrangeActAssert;

namespace Informedica.SecureSettings.Tests
{
    [TestClass]
    public class SettingsSourceShould
    {
        [Isolated]
        [TestMethod]
        public void UseTheTestTypeSettingWriterToWriteTheSetting()
        {
            var writers = new Dictionary<Enum, Action<Setting>>();
            Action<Setting> fakeWriter = FakeWriter.Write;
            var setting = new Setting("Test", "Test", "Test", false);
            Isolate.WhenCalled(() => fakeWriter.Invoke(setting)).IgnoreCall();
            writers.Add(MySettingSource.SettingTypes.App, fakeWriter);

            try
            {
                var source = new MySettingSource(writers, new Dictionary<Enum, Func<string, Setting>>());
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
        public void UseTestTypeSettingReaderToReadTheSetting()
        {
            var readers = new Dictionary<Enum, Func<string, Setting>>();
            Func<string, Setting> fakeReader = FakeReader.Read;
            Isolate.WhenCalled(() => fakeReader.Invoke("Test")).ReturnRecursiveFake();
            readers.Add(MySettingSource.SettingTypes.App, fakeReader);

            try
            {
                var source = new MySettingSource(new Dictionary<Enum, Action<Setting>>(), readers);
                source.ReadSetting(MySettingSource.SettingTypes.App, "Test");
                Isolate.Verify.WasCalledWithExactArguments(() => fakeReader.Invoke("Test"));
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }

        [TestMethod]
        public void CanBeCreatedWithAWriteAppSettingFunctionCalledThatWritesAnAppSetting()
        {
            var source = MySettingSource.CreateMySettingSource();
           
            source.WriteSetting(new Setting("Test", "Test", MySettingSource.SettingTypes.App.ToString(), false));
            Assert.AreEqual("Test", source.ReadSetting(MySettingSource.SettingTypes.App, "Test").Value);
        }

    }

    public static class FakeReader
    {
        public static Setting Read(string name)
        {
            return null;
        }
    }

    public static class FakeWriter
    {
        public static void Write(Setting setting)
        {
            
        }
    }

    public sealed class MySettingSource: SettingSource
    {
        public enum SettingTypes
        {
            App,
            Conn
        }

        private Setting _appsetting;

        public MySettingSource(Dictionary<Enum, Action<Setting>> writers, Dictionary<Enum, Func<string, Setting>> readers) : base(writers, readers)
        {
        }

        private void WriteAppSetting(Setting setting)
        {
            _appsetting = setting;
        }

        private Setting ReadAppSetting(string name)
        {
            return _appsetting;
        }

        public static MySettingSource CreateMySettingSource()
        {
            var writers = new Dictionary<Enum, Action<Setting>>();
            var readers = new Dictionary<Enum, Func<string, Setting>>();

            return new MySettingSource(writers, readers);
        }

        #region Overrides of SettingSource

        protected override void RegisterReaders()
        {
            if (!Readers.ContainsKey(SettingTypes.App))
                Readers[SettingTypes.App] = ReadAppSetting;
        }

        protected override void RegisterWriters()
        {
            if (!Writers.ContainsKey(SettingTypes.App))
                Writers[SettingTypes.App] = WriteAppSetting;   
        }

        protected override Enum SettingTypeToString(Setting setting)
        {
            SettingTypes value;
            Enum.TryParse(setting.Type, out value);
            return value;
        }

        #endregion
    }

    public abstract class SettingSource
    {
        protected Dictionary<Enum, Action<Setting>> Writers;
        protected Dictionary<Enum, Func<string, Setting>> Readers;

        protected SettingSource(Dictionary<Enum, Action<Setting>> writers, Dictionary<Enum, Func<string, Setting>> readers)
        {
            Writers = writers;
            Readers = readers;

            Init();
        }

        private void Init()
        {
            RegisterReaders();
            RegisterWriters();
        }

        public void WriteSetting(Setting setting)
        {
            Writers[SettingTypeToString(setting)].Invoke(setting);
        }

        public Setting ReadSetting(Enum type, string name)
        {
            return Readers[type].Invoke(name);
        }

        protected abstract Enum SettingTypeToString(Setting setting);

        protected abstract void RegisterReaders();
        protected abstract void RegisterWriters();
    }
}
