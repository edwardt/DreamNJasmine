﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using NJasmine.Extras;
using NJasmine.Marshalled;

namespace NJasmine.VS2012
{
    [FileExtension(".dll")]
    [FileExtension(".exe")]
    [DefaultExecutorUri(TestDiscoverer.VSExecutorUri)]
    [ExtensionUri(TestDiscoverer.VSExecutorUri)]
    public class TestExecutor : ITestExecutor
    {
        public TestExecutor()
        {
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            foreach(var group in tests.GroupBy(t => t.Source))
            {
                using (var appDomain = new AppDomainWrapper(group.Key))
                {
                    Executor.RunTests(appDomain, tests.Select(t => t.FullyQualifiedName).ToArray());
                }
            }
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            throw new NotImplementedException();
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }
    }
}
