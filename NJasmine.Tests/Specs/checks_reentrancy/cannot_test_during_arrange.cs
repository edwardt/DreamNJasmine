﻿using System;
using NJasmine;
using NJasmine.Marshalled;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Specs.checks_reentrancy
{
    [Explicit]
    public class cannot_reenter_during_arrange : GivenWhenThenFixture, INJasmineInternalRequirement
    {
        public override void Specify()
        {
            describe("when the arrange code tries to re-enter", delegate
            {
                arrange(delegate()
                {
                    it("has test within arrange", delegate { });
                });

                it("has a valid test that will now fail", delegate()
                {
                });
            });

            describe("when the arrange cleanup code tries to re-enter", delegate
            {
                var fail = arrange(() => 
                    new ActOnDispose(() => this.it("test within arrange()d dispose", delegate { })));

                it("has a valid test that will now fail", delegate()
                {
                });
            });
        }

        public class ActOnDispose : IDisposable
        {
            readonly Action _action;

            public ActOnDispose(Action action)
            {
                _action = action;
            }

            public void Dispose()
            {
                _action();
            }
        }

        public void Verify_NJasmine_implementation(IFixtureResult fixtureResult)
        {
            fixtureResult.failed();

            fixtureResult.hasTest("when the arrange code tries to re-enter, has a valid test that will now fail")
                .thatErrors()
                .withFailureMessage("System.InvalidOperationException : Called it() within arrange().");

            fixtureResult.hasTest("when the arrange cleanup code tries to re-enter, has a valid test that will now fail")
                .thatErrors()
                .withFailureMessage("System.InvalidOperationException : Called it() within arrange().");
        }
    }
}
