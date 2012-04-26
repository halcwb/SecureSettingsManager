using TypeMock.ArrangeActAssert;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace scsm.Tests
{
    [TestClass]
    public class ArgumentsProcessorShould
    {
        private ArgumentProcessor<ArgumentProcessorTarget> _processor;
        private ArgumentProcessorTarget _target;
        private IList<string> _args;


        private void Setup()
        {
            _target = new ArgumentProcessorTarget();
            Isolate.WhenCalled(() => _target.SomeMethod()).IgnoreCall();

            _processor = new ArgumentProcessor<ArgumentProcessorTarget>(_target);

            _args = new List<string>();
        }

        [Isolated]
        [TestMethod]
        public void CallSomeMethodOnTarget()
        {
            Setup();

            _args.Add("someMethod");
            try
            {
                _processor.ProcessArguments(_args);
                Isolate.Verify.WasCalledWithAnyArguments(() => _target.SomeMethod());

            }
            catch (System.Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }

        [Isolated]
        [TestMethod]
        public void CallMethodWithOneParameter()
        {
            Setup();

            _args.Add("methodWithOneParameter");
            _args.Add("parameter");

            try
            {
                _processor.ProcessArguments(_args);
                Isolate.Verify.WasCalledWithExactArguments(() => _target.MethodWithOneParameter("parameter"));

            }
            catch (System.Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }

        [Isolated]
        [TestMethod]
        public void CallSomeMethodAndMethodWithOneParameter()
        {
            Setup();

            _args.Add("someMethod");
            _args.Add("methodWithOneParameter");
            _args.Add("parameter");

            try
            {
                _processor.ProcessArguments(_args);
                Isolate.Verify.WasCalledWithExactArguments(() => _target.SomeMethod());
                Isolate.Verify.WasCalledWithExactArguments(() => _target.MethodWithOneParameter("parameter"));

            }
            catch (System.Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }

        [Isolated]
        [TestMethod]
        public void CallAMethodWithTwoParameters()
        {
            Setup();

            _args.Add("methodWithTwoParameters");
            _args.Add("param1");
            _args.Add("param2");

            try
            {
                _processor.ProcessArguments(_args);
                Isolate.Verify.WasCalledWithExactArguments(() => _target.MethodWithTwoParameters("param1", "param2"));

            }
            catch (System.Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }

        [Isolated]
        [TestMethod]
        public void CallAllMethodsOnTarget()
        {

            Setup();

            _args.Add("someMethod");
            _args.Add("methodWithOneParameter");
            _args.Add("parameter");
            _args.Add("methodWithTwoParameters");
            _args.Add("param1");
            _args.Add("param2");

            try
            {
                _processor.ProcessArguments(_args);
                Isolate.Verify.WasCalledWithExactArguments(() => _target.SomeMethod());
                Isolate.Verify.WasCalledWithExactArguments(() => _target.MethodWithOneParameter("parameter"));
                Isolate.Verify.WasCalledWithExactArguments(() => _target.MethodWithTwoParameters("param1", "param2"));

            }
            catch (System.Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }

        [Isolated]
        [TestMethod]
        public void GetAListOfOptionsContainingSomeMethodMethodWithOneParameterAndMethodWithTwoParameters()
        {
            Setup();

            var list = _processor.ListOptions();

            Assert.IsTrue(list.Contains("someMethod") &&
                          list.Contains("methodWithOneParameter") &&
                          list.Contains("methodWithTwoParameters"));
        }
    }

    public class ArgumentProcessorTarget
    {
        [Alias("someMethod")]
        public void SomeMethod()
        {
            throw new System.NotImplementedException();
        }

        [Alias("methodWithOneParameter")]
        public void MethodWithOneParameter(string parameter)
        {
            throw new System.NotImplementedException();
        }

        [Alias("methodWithTwoParameters")]
        public void MethodWithTwoParameters(string param1, string param2)
        {
            throw new System.NotImplementedException();
        }
    }
}
