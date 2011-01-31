﻿using System;
using System.Linq.Expressions;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine
{
    public abstract class GivenWhenThenFixture : SkeleFixture
    {
        public void given(string givenPhrase, Action specification)
        {
            _visitor.visitFork(SpecElement.given, "given " + givenPhrase, specification);
        }

        public void when(string whenPhrase, Action specification)
        {
            _visitor.visitFork(SpecElement.when, "when " + whenPhrase, specification);
        }

        public void then(string thenPhrase, Action test)
        {
            _visitor.visitTest(SpecElement.then, "then " + thenPhrase, test);
        }

        public void then(string thenPhrase)
        {
            _visitor.visitTest(SpecElement.then, "then " + thenPhrase, null);
        }

        public void cleanup(Action cleanup)
        {
            _visitor.visitAfterEach(SpecElement.cleanup, cleanup);
        }

        public void arrange(Action arrangeAction)
        {
            _visitor.visitBeforeEach(SpecElement.arrange, null, delegate() { arrangeAction(); return (string)null; });
        }

        public T arrange<T>(Func<T> arrangeAction)
        {
            return _visitor.visitBeforeEach(SpecElement.arrange, null, arrangeAction);
        }

        public void expect(Expression<Func<bool>> expectation)
        {
            PowerAssert.PAssert.IsTrue(expectation);
        }

        public TFixture importNUnit<TFixture>() where TFixture : class, new()
        {
            return _visitor.visitImportNUnit<TFixture>();
        }

        public class InnerFixture : NJasmineFixture
        {
            public override void Specify()
            {
                throw new Exception("this Specify method won't be used");
            }
        }
    }
}