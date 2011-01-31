﻿using System;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine
{
    public abstract class SkeleFixture
    {
        protected ISpecVisitor _visitor = new DoNothingFixtureVisitor();

        public ISpecVisitor SpecVisitor
        {
            get { return _visitor; }
        }

        public virtual NJasmineFixture.VisitorChangedContext UseVisitor(ISpecVisitor visitor)
        {
            var currentVisitor = _visitor;

            _visitor = visitor;

            return new NJasmineFixture.VisitorChangedContext(() => _visitor = currentVisitor);
        }

        public abstract void Specify();

        public class VisitorChangedContext : IDisposable
        {
            Action _action;

            public VisitorChangedContext(Action action)
            {
                _action = action;
            }

            public void Dispose()
            {
                if (_action != null)
                {
                    _action();
                    _action = null;
                }
            }
        }
    }
}