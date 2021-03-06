﻿using System.Collections.Generic;
using NJasmine;

namespace NJasmineTests.Core
{
    public abstract class ObservableNJasmineFixture : GivenWhenThenFixture
    {
        List<string> _observations = new List<string>();

        public List<string> Observations
        {
            get { return _observations; }
        }

        public void Observe(string value)
        {
            _observations.Add(value);
        }

        public void ResetObservations()
        {
            _observations = new List<string>();
        }
    }
}