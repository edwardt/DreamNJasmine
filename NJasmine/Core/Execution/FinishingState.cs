﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Execution
{
    class FinishingState : ISpecPositionVisitor
    {
        private readonly NJasmineTestRunContext _runContext;

        public FinishingState(NJasmineTestRunContext runContext)
        {
            _runContext = runContext;
        }

        public void visitFork(SpecificationElement origin, string description, Action action, TestPosition position)
        {
        }

        public TArranged visitBeforeAll<TArranged>(SpecificationElement origin, Func<TArranged> action, TestPosition position)
        {
            return default(TArranged);
        }

        public void visitAfterAll(SpecificationElement origin, Action action, TestPosition position)
        {
        }

        public TArranged visitBeforeEach<TArranged>(SpecificationElement origin, Func<TArranged> factory, TestPosition position)
        {
            return default(TArranged);
        }

        public void visitAfterEach(SpecificationElement origin, Action action, TestPosition position)
        {
        }

        public void visitTest(SpecificationElement origin, string description, Action action, TestPosition position)
        {
        }

        public void visitIgnoreBecause(SpecificationElement origin, string reason, TestPosition position)
        {
        }

        public void visitExpect(SpecificationElement origin, Expression<Func<bool>> expectation, TestPosition position)
        {
        }

        public void visitWaitUntil(SpecificationElement origin, Expression<Func<bool>> expectation, int totalWaitMs, int waitIncrementMs, TestPosition position)
        {
        }

        public void visitWithCategory(SpecificationElement origin, string category, TestPosition position)
        {
        }

        public void visitTrace(SpecificationElement origin, string message, TestPosition position)
        {
            _runContext.AddTrace(message);
        }

        public void visitLeakDisposable(SpecificationElement origin, IDisposable disposable, TestPosition position)
        {
            _runContext.LeakDisposable(disposable);
        }
    }
}
