﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NJasmine.Core.Discovery;
using NJasmine.Core.Execution;
using NJasmine.Core.FixtureVisitor;
using NUnit.Core;

namespace NJasmine.Core
{
    public class NJasmineTestMethod : TestMethod, INJasmineTest
    {
        readonly Func<SpecificationFixture> _fixtureFactory;
        readonly IGlobalSetupManager _globalSetup;

        public NJasmineTestMethod(Func<SpecificationFixture> fixtureFactory, TestPosition position, IGlobalSetupManager globalSetup)
            : base(new Action(delegate() { }).Method)
        {
            Position = position;
            _fixtureFactory = fixtureFactory;
            _globalSetup = globalSetup;
        }

        public TestPosition Position { get; private set; }

        public override TestResult Run(EventListener listener, ITestFilter filter)
        {
            listener.TestStarted(base.TestName);
            long ticks = DateTime.Now.Ticks;
            TestResult testResult = new TestResult(this);

            Exception existingError = null;

            _globalSetup.PrepareForTestPosition(Position, out existingError);

            if (existingError != null)
            {
                TestResultUtil.Error(testResult, existingError, null, FailureSite.SetUp);
            }
            else
            {
                List<string> traceMessages = null;
                try
                {
                    RunTestMethod(testResult, out traceMessages);
                }
                catch (Exception e)
                {
                    var globalTraceMessages = _globalSetup.GetTraceMessages();
                    TestResultUtil.Error(testResult, e, globalTraceMessages.Concat(traceMessages));
                }
            }

            double num3 = ((double)(DateTime.Now.Ticks - ticks)) / 10000000.0;
            testResult.Time = num3;
            listener.TestFinished(testResult);
            return testResult;
        }

        public void RunTestMethod(TestResult testResult, out List<string> traceMessages)
        {
            RunTestMethodInner(new TestResultShim(testResult), out traceMessages);
        }

        public void RunTestMethodInner(TestResultShim testResult, out List<string> traceMessages)
        {
            traceMessages = new List<string>();

            var executionContext = new NJasmineTestRunContext(Position, _globalSetup, traceMessages);
            var runner = new NJasmineTestRunner(executionContext);

            SpecificationFixture fixture = this._fixtureFactory();

            fixture.CurrentPosition = new TestPosition(0);
            fixture.Visitor = runner;
            try
            {
                fixture.Run();
            }
            finally
            {
                executionContext.RunAllPerTestTeardowns();
            }
            testResult.Success();
        }
    }
}
