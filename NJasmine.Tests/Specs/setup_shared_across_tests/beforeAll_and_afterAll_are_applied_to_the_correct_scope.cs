﻿using System;
using NJasmine.Marshalled;
using NJasmineTests.Core;
using NJasmineTests.Export;

namespace NJasmineTests.Specs.setup_shared_across_tests
{
    public class beforeAll_and_afterAll_are_applied_to_the_correct_scope : GivenWhenThenFixtureTracingToConsole, INJasmineInternalRequirement
    {
        public class RunOnDispose : IDisposable
        {
            private Action _action; 

            public RunOnDispose(Action action)
            {
                _action = action;
            }

            public void Dispose()
            {
                _action();
                _action = null;
            }
        }

        public override void Specify()
        {
            beforeAll(ResetTracing);

            beforeAll(delegate
            {
                Trace("BEFORE ALL");
                return new RunOnDispose(() => Trace("DISPOSING BEFORE ALL"));
            });

            afterAll(delegate
            {
                Trace("AFTER ALL");
            }); 

            it("first test", delegate
            {
                Trace("first test");
            });

            beforeAll(delegate
            {
                Trace("SECOND BEFORE ALL");
                return new RunOnDispose(() => Trace("DISPOSING SECOND BEFORE ALL"));
            });

            afterAll(delegate
            {
                Trace("SECOND AFTER ALL");
            });

            describe("in some context", delegate
            {
                beforeAll(delegate
                {
                    Trace("INNER BEFORE ALL");
                    return new RunOnDispose(() => Trace("DISPOSING INNER BEFORE ALL"));
                });

                afterAll(delegate
                {
                    Trace("INNER AFTER ALL");
                });

                it("second teest", delegate
                {
                    Trace("second test");
                });

                it("third teest", delegate
                {
                    Trace("third test");
                });
            });

            beforeAll(delegate
            {
                Trace("FINAL BEFORE ALL");
                return new RunOnDispose(() => Trace("DISPOSING FINAL BEFORE ALL"));
            });

            afterAll(delegate
            {
                Trace("FINAL AFTER ALL");
            }); 
        }

        public void Verify_NJasmine_implementation(IFixtureResult fixtureResult)
        {
            fixtureResult.succeeds();
            fixtureResult.containsTrace(@"
BEFORE ALL
first test
SECOND BEFORE ALL
INNER BEFORE ALL
second test
third test
INNER AFTER ALL
DISPOSING INNER BEFORE ALL
SECOND AFTER ALL
DISPOSING SECOND BEFORE ALL
AFTER ALL
DISPOSING BEFORE ALL
");
        }
    }
}
