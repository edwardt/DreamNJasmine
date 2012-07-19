using System;
using NJasmine.Core;
using NJasmine.Core.Discovery;
using NJasmine.Core.GlobalSetup;

namespace NJasmine.NUnit.TestElements
{
    public class NJasmineTestSuite
    {
        readonly INativeTestFactory _nativeTestFactory;
        private readonly TestPosition _position;
        private GlobalSetupManager _globalSetup;

        public NJasmineTestSuite(INativeTestFactory nativeTestFactory, TestPosition position, GlobalSetupManager globalSetup)
        {
            _nativeTestFactory = nativeTestFactory;
            _position = position;
            _globalSetup = globalSetup;
        }

        public TestBuilder BuildNJasmineTestSuite(string parentName, string name, FixtureDiscoveryContext buildContext, GlobalSetupManager globalSetup, Action action, bool isOuterScopeOfSpecification)
        {
            var position = _position;

            var resultBuilder = new TestBuilder(_nativeTestFactory.ForSuite(position, () => _globalSetup.Cleanup(position)));
            resultBuilder.Name.FullName = parentName + "." + name;
            resultBuilder.Name.Shortname = name;
            resultBuilder.Name.MultilineName = resultBuilder.Name.FullName;

            return RunSuiteAction(buildContext, globalSetup, action, isOuterScopeOfSpecification, resultBuilder);
        }

        public TestBuilder RunSuiteAction(FixtureDiscoveryContext buildContext, GlobalSetupManager globalSetup, Action action,
                                    bool isOuterScopeOfSpecification, TestBuilder resultBuilder)
        {
            var builder = new NJasmineTestSuiteBuilder(_nativeTestFactory, resultBuilder, buildContext, globalSetup);

            var exception = buildContext.RunActionWithVisitor(_position.GetFirstChildPosition(), action, builder);

            if (exception == null)
            {
                builder.VisitAccumulatedTests(v => resultBuilder.AddChildTest(v));
            }
            else
            {
                var failingSuiteAsTest = new TestBuilder(_nativeTestFactory.ForFailingSuite(_position, exception));

                failingSuiteAsTest.Name.FullName = resultBuilder.Name.FullName;
                failingSuiteAsTest.Name.Shortname = resultBuilder.Name.Shortname;
                failingSuiteAsTest.Name.MultilineName = resultBuilder.Name.MultilineName;

                buildContext.NameGenator.ReserveName(failingSuiteAsTest.Name);

                if (isOuterScopeOfSpecification)
                {
                    resultBuilder.AddChildTest(failingSuiteAsTest);
                }
                else
                {
                    return failingSuiteAsTest;
                }
            }
            return resultBuilder;
        }
    }
}
