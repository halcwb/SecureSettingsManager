using System;
using System.Collections.Generic;
using System.Configuration;
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
        private ISetting _setting;
        private string _keyValue = "Test";
        private string _value = "Test";

        [TestInitialize]
        public void SetupIsolatedSettingSource()
        {
            _setting = Isolate.Fake.Instance<ISetting>();
            Isolate.WhenCalled(() => _setting.Type).WillReturn(typeof(ConnectionStringSettings));
            Isolate.WhenCalled(() => _setting.Key).WillReturn(_keyValue);
            Isolate.WhenCalled(() => _setting.Value).WillReturn(_value);
        }


        [Isolated]
        [TestMethod]
        public void BeAbleToUseATestTypeSettingWriterToWriteASetting()
        {
            var writers = new Dictionary<Type, Action<ISetting>>();
            Action<ISetting> fakeWriter = FakeWriters.Write;
            var setting = Isolate.Fake.Instance<ISetting>();
            Isolate.WhenCalled(() => setting.Type).WillReturn(typeof(ConnectionStringSettings));
            Isolate.WhenCalled(() => fakeWriter.Invoke(setting)).IgnoreCall();
            writers.Add(typeof(ConnectionStringSettings), fakeWriter);

            try
            {
                var source = new MyTestSettingSource(writers, new Dictionary<Type, Func<ISetting, bool>>()) {setting};
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
            var removers = new Dictionary<Type, Func<ISetting, bool>>();
            Func<ISetting, bool> fakeRemover = FakeRemovers.Remove;
            var fakeSetting = Isolate.Fake.Instance<ISetting>();
            Isolate.WhenCalled(() => fakeSetting.Type).WillReturn(typeof(ConnectionStringSettings));
            Isolate.WhenCalled(() => fakeRemover.Invoke(fakeSetting)).WillReturn(true);
            removers.Add(typeof(ConnectionStringSettings), fakeRemover);

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
           
            source.Add(_setting);
            var setting = source.SingleOrDefault(s => s.Type == typeof(ConnectionStringSettings) && s.Key == _keyValue);
            Assert.IsNotNull(setting);
            Assert.AreEqual(_value, setting.Value);
        }

        [TestMethod]
        public void HaveCountPlusOneWhenSettingIsAdded()
        {
            var source = MyTestSettingSource.CreateMySettingSource();
            var count = source.Count();            

            source.Add(_setting);

            Assert.AreEqual(count + 1, source.Count());
        }
        
        [TestMethod]
        public void HaveCountMinusOneWhenSettingIsRemoved()
        {
            var source = MyTestSettingSource.CreateMySettingSource();
            source.Add(_setting);

            var count = source.Count();
            source.Remove(_setting);

            Assert.AreEqual(count -1 , source.Count());
        }
    
        [Isolated]
        [TestMethod]
        public void ThrowAnMethodNotFoundExceptionWhenMethodIsCalledButNotRegistered()
        {
            var source = SetupSettingSourceWithoutWritersOrReadersOrRemovers();

            try
            {
                source.Add(_setting);

            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(MissingMethodException));
            }
        }

        private static MyTestSettingSource SetupSettingSourceWithoutWritersOrReadersOrRemovers()
        {
            var fakeRemovers = Isolate.Fake.Instance<IDictionary<Type, Func<ISetting, bool>>>();
            var fakeWriters = Isolate.Fake.Instance<IDictionary<Type, Action<ISetting>>>();
            var source = new MyTestSettingSource(fakeWriters, fakeRemovers);
            return source;
        }
    }

    public static class FakeRemovers
    {
        public static bool Remove(ISetting setting)
        {
            return true;
        }
    }

    public static class FakeWriters
    {
        public static void Write(ISetting setting)
        {
            
        }
    }
}
