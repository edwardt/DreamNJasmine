﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace NJasmineTests
{
    public class RunExternalAttribute : Attribute
    {
        public class TestDefinition
        {
            public string Name;
            public bool Passes;
            public string[] ExpectedStrings;
            public string ExpectedExtraction;
        }

        public bool TestPasses { get; set; }
        public string[] ExpectedStrings { get; set; }
        public string ExpectedExtraction { get; set; }

        public RunExternalAttribute(bool testPasses)
        {
            TestPasses = testPasses;
        }

        public static IEnumerable<TestDefinition> GetAll()
        {
            return from t in typeof (RunExternalAttribute).Assembly.GetTypes()
                   where t.IsDefined(typeof (RunExternalAttribute), false)
                   from a in t.GetCustomAttributes(typeof (RunExternalAttribute), false).Cast<RunExternalAttribute>()
                   select new TestDefinition
                   {
                       Name = t.FullName,
                       Passes = a.TestPasses,
                       ExpectedStrings = a.ExpectedStrings ?? new string[0],
                       ExpectedExtraction = a.ExpectedExtraction
                   };
        }
    }
}