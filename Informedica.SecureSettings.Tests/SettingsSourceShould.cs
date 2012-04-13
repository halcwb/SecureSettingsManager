using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            Action<Setting> fakeWriter = FakeWriters.Write;
            var setting = new Setting("Test", "Test", "Test", false);
            Isolate.WhenCalled(() => fakeWriter.Invoke(setting)).IgnoreCall();
            writers.Add(MySettingSource.SettingTypes.App, fakeWriter);

            try
            {
                var source = new MySettingSource(writers, new Dictionary<Enum, Func<string, Setting>>(), new Dictionary<Enum, Action<Setting>>());
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
            Func<string, Setting> fakeReader = FakeReaders.Read;
            Isolate.WhenCalled(() => fakeReader.Invoke("Test")).ReturnRecursiveFake();
            readers.Add(MySettingSource.SettingTypes.App, fakeReader);

            try
            {
                var source = new MySettingSource(new Dictionary<Enum, Action<Setting>>(), readers, new Dictionary<Enum, Action<Setting>>());
                source.ReadSetting(MySettingSource.SettingTypes.App, "Test");
                Isolate.Verify.WasCalledWithExactArguments(() => fakeReader.Invoke("Test"));
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }

        [Isolated]
        [TestMethod]
        public void UseTestTypeSettingRemovertoRemoveSetting()
        {
            var removers = new Dictionary<Enum, Action<Setting>>();
            Action<Setting> fakeRemover = FakeRemovers.Remove;
            var fakeSetting = Isolate.Fake.Instance<Setting>();
            Isolate.WhenCalled(() => fakeRemover.Invoke(fakeSetting)).IgnoreCall();
            removers.Add(MySettingSource.SettingTypes.App, fakeRemover);

            var source = new MySettingSource(new Dictionary<Enum, Action<Setting>>(),
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
        public void CanBeCreatedWithAWriteAppSettingFunctionCalledThatWritesAnAppSetting()
        {
            var source = MySettingSource.CreateMySettingSource();
           
            source.WriteSetting(new Setting("Test", "Test", MySettingSource.SettingTypes.App.ToString(), false));
            Assert.AreEqual("Test", source.ReadSetting(MySettingSource.SettingTypes.App, "Test").Value);
        }

        [TestMethod]
        public void HaveCountPlusOneWhenSettingIsAdded()
        {
            var source = MySettingSource.CreateMySettingSource();
            Assert.IsFalse(source.Any());

            source.WriteSetting(new Setting("Test", "Test", MySettingSource.SettingTypes.App.ToString(), false));
            var count = source.Count();

            source.WriteSetting(new Setting("Test", "Test", MySettingSource.SettingTypes.Conn.ToString(), false));
            Assert.AreEqual(count + 1, source.Count());
        }
        
        [TestMethod]
        public void HaveCountMinusOneWhenSettingIsRemoved()
        {
            var source = MySettingSource.CreateMySettingSource();
            var setting = new Setting("Test", "Test", MySettingSource.SettingTypes.App.ToString(), false);
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

    public sealed class MySettingSource: SettingSource
    {
        public enum SettingTypes
        {
            App,
            Conn
        }

        private Setting _appsetting;
        private Setting _connsetting;

        public MySettingSource(IDictionary<Enum, Action<Setting>> writers, 
                               IDictionary<Enum, Func<string, Setting>> readers, 
                               IDictionary<Enum, Action<Setting>> removers) : base(writers, readers, removers)
        {}

        private void WriteAppSetting(Setting setting)
        {
            _appsetting = setting;
        }

        private Setting ReadAppSetting(string name)
        {
            return _appsetting;
        }

        private void WriteConnSetting(Setting setting)
        {
            _connsetting = setting;
        }

        private Setting ReadConnSetting(string name)
        {
            return _connsetting;
        }

        public static MySettingSource CreateMySettingSource()
        {
            var writers = new Dictionary<Enum, Action<Setting>>();
            var readers = new Dictionary<Enum, Func<string, Setting>>();
            var removers = new Dictionary<Enum, Action<Setting>>();

            return new MySettingSource(writers, readers, removers);
        }

        #region Overrides of SettingSource

        protected override void RegisterReaders()
        {
            if (!Readers.ContainsKey(SettingTypes.App))
                Readers[SettingTypes.App] = ReadAppSetting;
            if (!Readers.ContainsKey(SettingTypes.Conn))
                Readers[SettingTypes.Conn] = ReadConnSetting;
        }

        protected override void RegisterWriters()
        {
            if (!Writers.ContainsKey(SettingTypes.App))
                Writers[SettingTypes.App] = WriteAppSetting;
            if (!Writers.ContainsKey(SettingTypes.Conn))
                Writers[SettingTypes.Conn] = WriteConnSetting;
        }

        protected override void RegisterRemovers()
        {
            if (!Removers.ContainsKey(SettingTypes.App))
                Removers[SettingTypes.App] = RemoveAppSetting;
            if (!Removers.ContainsKey(SettingTypes.Conn))
                Removers[SettingTypes.Conn] = RemoveConnSetting;
        }

        private void RemoveConnSetting(Setting setting)
        {
            _connsetting = null;
        }

        private void RemoveAppSetting(Setting setting)
        {
            _appsetting = null;
        }

        protected override Enum SettingTypeToString(Setting setting)
        {
            SettingTypes value;
            Enum.TryParse(setting.Type, out value);
            return value;
        }

        protected override IEnumerable<Setting> Settings
        {
            get
            {
                var settings = new List<Setting>();
                if (_appsetting != null) settings.Add(_appsetting);
                if (_connsetting != null) settings.Add(_connsetting);

                return settings;
            }
        }

        #endregion
    }

    public abstract class SettingSource: IEnumerable<Setting>
    {
        protected IDictionary<Enum, Action<Setting>> Writers;
        protected IDictionary<Enum, Func<string, Setting>> Readers;
        protected IDictionary<Enum, Action<Setting>> Removers;

        protected SettingSource(IDictionary<Enum, Action<Setting>> writers, 
                                IDictionary<Enum, Func<string, Setting>> readers, 
                                IDictionary<Enum, Action<Setting>> removers)
        {
            Writers = writers;
            Readers = readers;
            Removers = removers;

            Init();
        }

        private void Init()
        {
            RegisterReaders();
            RegisterWriters();
            RegisterRemovers();
        }

        public void WriteSetting(Setting setting)
        {
            Writers[SettingTypeToString(setting)].Invoke(setting);
        }

        public Setting ReadSetting(Enum type, string name)
        {
            return Readers[type].Invoke(name);
        }

        public void RemoveSetting(Setting setting)
        {
            Removers[SettingTypeToString(setting)].Invoke(setting);
        }

        protected abstract Enum SettingTypeToString(Setting setting);

        protected abstract void RegisterReaders();
        protected abstract void RegisterWriters();
        protected abstract void RegisterRemovers();

        #region Implementation of IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<Setting> GetEnumerator()
        {
            return Settings.GetEnumerator();
        }

        protected abstract IEnumerable<Setting> Settings { get; }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
