using System;
using System.Collections.Generic;

namespace Expressur;

[Serializable]
public class UnableToResolveFormulaException : Exception
{
    public UnableToResolveFormulaException() { }
    public UnableToResolveFormulaException(ICollection<KeyValuePair<string, string>> failedFormula) { foreach (var ff in failedFormula) { Data.Add(ff.Key, ff.Value); } }
    public UnableToResolveFormulaException(string message) : base(message) { }
    public UnableToResolveFormulaException(string message, Exception inner) : base(message, inner) { }
    protected UnableToResolveFormulaException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}