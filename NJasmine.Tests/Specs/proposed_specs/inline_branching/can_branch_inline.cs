﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmine.Marshalled;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Specs.proposed_specs.inline_branching
{
    [Explicit]
    class can_branch_inline : GivenWithThenFixtureWithInlineBranching, INJasmineInternalRequirement
    {
        public override void Specify()
        {
            given("some precondition", delegate
            {
                int input = -1;
                string expectedResult = null;

                fork(
                    join => when("the input is 0", delegate
                    {
                        input = 0;
                        expectedResult = input.ToString();

                        // do special setup

                        join();
                    }),
                    join => when("the input is 1", delegate
                    {
                        input = 1;
                        expectedResult = input.ToString();

                        // other special setup

                        join();
                    }));

                then("it runs" + input, delegate
                {
                    expect(() => input.ToString() == expectedResult);
                });
            });
        }

        public void Verify_NJasmine_implementation(IFixtureResult fixtureResult)
        {
            fixtureResult.hasTest("given some precondition, when the input is 0, then it runs").thatSucceeds();
            fixtureResult.hasTest("given some precondition, when the input is 1, then it runs").thatSucceeds();
        }
    }

    public abstract class GivenWithThenFixtureWithInlineBranching : GivenWhenThenFixture
    {
        protected void fork(params Action<Action>[] action)
        {
            throw new NotImplementedException();
        }
    }
}
