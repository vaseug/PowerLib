using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data.Entity.Core.Metadata.Edm;

namespace PowerLib.System.Data.Entity
{
  public class FunctionDescriptor
  {
    private string _namespaceName;
    private string _functionName;
    private string _databaseSchema;
    private bool _isTableValued;
    private bool? _isComposable;
    private bool? _isBuitIn;
    private bool? _isAggregate;
    private bool? _isNiladic;
    private ParameterTypeSemantics? _parameterTypeSemantics;
    private IReadOnlyList<ParameterDescriptor> _parameters;
    private IReadOnlyList<ResultDescriptor> _results;

    internal FunctionDescriptor(string namespaceName, string databaseSchema, string functionName,
      bool isTableValued, bool? isComposable, bool? isBuitIn, bool? isAggregate, bool? isNiladic, ParameterTypeSemantics? parameterTypeSemantics,
      IEnumerable<ParameterDescriptor> parameters, IEnumerable<ResultDescriptor> results)
    {
      _namespaceName = namespaceName;
      _databaseSchema = databaseSchema;
      _functionName = functionName;
      _isTableValued = isTableValued;
      _isComposable = isComposable;
      _isBuitIn = isBuitIn;
      _isAggregate = isAggregate;
      _isNiladic = isNiladic;
      _parameterTypeSemantics = parameterTypeSemantics;
      _parameters = new ReadOnlyCollection<ParameterDescriptor>(parameters.ToArray());
      _results = new ReadOnlyCollection<ResultDescriptor>(results is IList<ResultDescriptor> ? (IList<ResultDescriptor>)results : results.ToArray());
    }

    public string NamespaceName
    {
      get { return _namespaceName; }
    }

    public string DatabaseSchema
    {
      get { return _databaseSchema; }
    }

    public string FunctionName
    {
      get { return _functionName; }
    }

    public bool IsTableValued
    {
      get { return _isTableValued; }
    }

    public bool? IsComposable
    {
      get { return _isComposable; }
    }

    public bool? IsAggregate
    {
      get { return _isAggregate; }
    }

    public bool? IsBuiltIn
    {
      get { return _isBuitIn; }
    }

    public bool? IsNiladic
    {
      get { return _isNiladic; }
    }

    public ParameterTypeSemantics? ParameterTypeSemantics
    {
      get { return _parameterTypeSemantics; }
    }

    public IReadOnlyList<ParameterDescriptor> Parameters
    {
      get { return _parameters; }
    }

    public IReadOnlyList<ResultDescriptor> Results
    {
      get { return _results; }
    }
  }
}
