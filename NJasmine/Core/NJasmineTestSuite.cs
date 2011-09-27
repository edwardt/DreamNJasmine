﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using NJasmine.Core.Discovery;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Core.GlobalSetup;
using NUnit.Core;
using NUnit.Framework;

namespace NJasmine.Core
{
    public class NJasmineTestSuite : TestSuite, INJasmineTest
    {
        public TestPosition Position { get; private set; }

        GlobalSetupManager _setupManager;

        public static Test CreateRootNJasmineSuite(Type type)
        {
            var fixtureFactory = GetFixtureFactoryForType(type);

            AllSuitesBuildContext buildContext = new AllSuitesBuildContext(fixtureFactory, new NameGenerator(), fixtureFactory());

            var globalSetup = new GlobalSetupManager();

            globalSetup.Initialize(fixtureFactory);

            NJasmineTestSuite rootSuite = new NJasmineTestSuite(globalSetup);
            rootSuite.TestName.FullName = type.Namespace + "." + type.Name;
            rootSuite.TestName.Name = type.Name;

            return rootSuite.BuildNJasmineTestSuite(buildContext, globalSetup, buildContext._fixtureInstanceForDiscovery.Run, true, new TestPosition());
        }

        public NJasmineTestSuite(GlobalSetupManager setupManager)
            : base("thistestname", "willbeoverwritten")
        {
            _setupManager = setupManager;
            maintainTestOrder = true;
        }

        public Test BuildNJasmineTestSuite(AllSuitesBuildContext buildContext, GlobalSetupManager globalSetup, Action action, bool isOuterScopeOfSpecification, TestPosition position)
        {
            Position = position;

            List<Test> accumulatedTests = new List<Test>();

            var branchDestiny = new BranchDestiny();
            var builder = new NJasmineTestSuiteBuilder(this, buildContext, branchDestiny, globalSetup, test => accumulatedTests.Add(test));

            return buildContext._fixtureInstanceForDiscovery.BuildChildSuite(builder, this.Position.GetFirstChildPosition(), delegate
            {
                Exception exception1 = null;

                try
                {
                    action();

                    if (isOuterScopeOfSpecification)
                        branchDestiny.RunPendingDiscoveryBranches(action);
                }
                catch (Exception e)
                {
                    exception1 = e;
                }

                if (exception1 == null)
                {
                    foreach (var test1 in accumulatedTests)
                        Add(test1);
                }
                else
                {
                    var nJasmineInvalidTestSuite1 = new NJasmineInvalidTestSuite(exception1, Position);

                    nJasmineInvalidTestSuite1.TestName.FullName = TestName.FullName;
                    nJasmineInvalidTestSuite1.TestName.Name = TestName.Name;

                    nJasmineInvalidTestSuite1.SetMultilineName(this.GetMultilineName());

                    if (isOuterScopeOfSpecification)
                    {
                        Add(nJasmineInvalidTestSuite1);
                    }
                    else
                    {
                        return nJasmineInvalidTestSuite1;
                    }
                }

                return this;
            });
        }

        protected override void DoOneTimeTearDown(TestResult suiteResult)
        {
            try
            {
                _setupManager.Cleanup(Position);
            }
            catch (Exception innerException)
            {
                NUnitException exception2 = innerException as NUnitException;
                if (exception2 != null)
                {
                    innerException = exception2.InnerException;
                }

                TestResultUtil.Error(suiteResult, innerException, null, FailureSite.TearDown);
            }
        }

        public static Func<SpecificationFixture> GetFixtureFactoryForType(Type type)
        {
            var constructor = type.GetConstructor(new Type[0]);

            return delegate()
            {
                var fixture = constructor.Invoke(new object[0]) as SpecificationFixture;
                return fixture;
            };
        }
    }
}
