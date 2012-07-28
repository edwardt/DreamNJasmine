﻿using System;
using NJasmine;
using NJasmine.Marshalled;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Specs.setup_shared_across_tests
{
    [Explicit]
    public class beforeAll_doesnt_reexecute : GivenWhenThenFixture, INJasmineInternalRequirement
    {
        public static int TotalRuns = 0;

        public override void Specify()
        {
            beforeAll(() =>
            {
                TotalRuns++;

                throw new Exception("Failed with TotalRuns: " + TotalRuns);
            });

            it("then reports the test with the correct count", delegate { });
            it("then reports the test with the correct count", delegate { });
            it("then reports the test with the correct count", delegate { });
            it("then reports the test with the correct count", delegate { });
        }

        public void Verify_NJasmine_implementation(IFixtureResult fixtureResult)
        {
            fixtureResult.failed();

            fixtureResult.hasTest("then reports the test with the correct count")
                .withFailureMessage("Failed with TotalRuns: 1");

            fixtureResult.hasTest("then reports the test with the correct count`2")
                .withFailureMessage("Failed with TotalRuns: 1");

            fixtureResult.hasTest("then reports the test with the correct count`3")
                .withFailureMessage("Failed with TotalRuns: 1");

            fixtureResult.hasTest("then reports the test with the correct count`4")
                .withFailureMessage("Failed with TotalRuns: 1");
        }
    }
}
