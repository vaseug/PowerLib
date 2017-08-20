using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Reflection;
using PowerLib.System.ComponentModel.DataAnnotations;
using PowerLib.System.Linq;
using PowerLib.System.Reflection;

namespace PowerLib.System.Data.Entity
{
  public class FunctionsConvention : IStoreModelConvention<EntityContainer>
  {
    public const string DefaultDatabaseSchema = "dbo";
    public const string DefaultResultColumnName = "value";
    public const string DefaultNamespaceName = "CodeFirstDatabaseSchema";
    private readonly Type _containerType;
    private readonly string _databaseSchema;
    private readonly string _resultColumnName;
    private readonly string _namespaceName;

    public FunctionsConvention(Type containerType, string databaseSchema = null, string resultColumnName = null, string namespaceName = null)
    {
      if (containerType == null)
        throw new ArgumentNullException("containerType");

      _containerType = containerType;
      _databaseSchema = databaseSchema ?? DefaultDatabaseSchema;
      _resultColumnName = resultColumnName ?? DefaultResultColumnName;
      _namespaceName = namespaceName ?? DefaultNamespaceName;
    }

    public string DatabaseSchema
    {
      get { return _databaseSchema; }
    }

    public string ResultColumnName
    {
      get { return _resultColumnName; }
    }

    private FunctionDescriptor BuildFunctionDescriptor(MethodInfo mi)
    {
      DbFunctionAttribute attrFunction = mi.GetCustomAttribute<DbFunctionAttribute>();
      if (attrFunction == null)
        throw new InvalidOperationException(string.Format("Method {0} of type {1} must be marked by'DbFunction...' attribute.", mi.Name, mi.DeclaringType));
      DbFunctionExAttribute attrFunctionEx = attrFunction as DbFunctionExAttribute;
      string methodName = mi.Name;
      string functionName = !string.IsNullOrWhiteSpace(attrFunction.FunctionName) ? attrFunction.FunctionName : methodName;
      string databaseSchema = attrFunctionEx != null && !string.IsNullOrWhiteSpace(attrFunctionEx.Schema) ? attrFunctionEx.Schema : _databaseSchema;
      if (string.IsNullOrWhiteSpace(databaseSchema))
        throw new InvalidOperationException(string.Format("Database schema for method {0} of type {1} is not defined.", mi.Name, mi.DeclaringType));
      //
      bool isExtension = mi.IsDefined(typeof(ExtensionAttribute), false);
      ParameterDescriptor[] parameters = mi.GetParameters()
        .SkipWhile(pi => isExtension && pi.Position == 0 && pi.ParameterType.IsAssignableFrom(typeof(DbContext)))
        .OrderBy(pi => pi.Position)
        .Select((pi, i) =>
        {
          FunctionParameterAttribute parameterAttr = pi.GetCustomAttribute<FunctionParameterAttribute>();
          Type parameterType = pi.ParameterType == typeof(ObjectParameter) ? parameterAttr.Type : pi.ParameterType;
          if (parameterType == null)
            throw new InvalidOperationException(string.Format("Method parameter '{0}' at position {1} is not defined.", pi.Name, pi.Position));
          //
          MinLengthAttribute minLengthAttr = pi.GetCustomAttribute<MinLengthAttribute>();
          MaxLengthAttribute maxLengthAttr = pi.GetCustomAttribute<MaxLengthAttribute>();
          FixedLengthAttribute fixedLengthAttr = pi.GetCustomAttribute<FixedLengthAttribute>();
          PrecisionScaleAttribute precisionScaleAttr = pi.GetCustomAttribute<PrecisionScaleAttribute>();
          //
          return new ParameterDescriptor(i, parameterType.IsByRef ? pi.IsOut ? ParameterDirection.Output : ParameterDirection.InputOutput : ParameterDirection.Input, parameterType)
          {
            Name = parameterAttr != null && !string.IsNullOrWhiteSpace(parameterAttr.Name) ? parameterAttr.Name : pi.Name,
            StoreTypeName = parameterAttr != null && !string.IsNullOrWhiteSpace(parameterAttr.TypeName) ? parameterAttr.TypeName : null,
            Length = maxLengthAttr != null ? maxLengthAttr.Length : default(int?),
            IsFixedLength = minLengthAttr != null && maxLengthAttr != null ? minLengthAttr.Length == maxLengthAttr.Length : fixedLengthAttr != null ? fixedLengthAttr.IsFixedLength : default(bool?),
            Precision = precisionScaleAttr != null ? precisionScaleAttr.Precision : default(byte?),
            Scale = precisionScaleAttr != null ? precisionScaleAttr.Scale : default(byte?)
          };
        })
        .ToArray();
      //  IQueryable<>
      Type returnType = mi.ReturnType.IsGenericType && mi.ReturnType.GetGenericTypeDefinition() == typeof(IQueryable<>) ? mi.ReturnType :
        mi.ReturnType.GetInterfaces().SingleOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IQueryable<>));
      if (returnType != null)
      {
        FunctionResultAttribute attrResult = mi.ReturnParameter.GetCustomAttribute<FunctionResultAttribute>();
        ResultDescriptor result = new ResultDescriptor(returnType.GetGenericArguments()[0])
        {
          ColumnName = attrResult != null ? attrResult.ColumnName : _resultColumnName,
          StoreTypeName = attrResult != null ? attrResult.TypeName : null
        };
        return new FunctionDescriptor(_namespaceName, databaseSchema, functionName, true,
          attrFunctionEx != null ? attrFunctionEx.IsComposable : default(bool?),
          isExtension ? attrFunctionEx != null ? attrFunctionEx.IsBuiltIn : default(bool?) : false,
          false,
          parameters.Length == 0 ? attrFunctionEx != null ? attrFunctionEx.IsNiladic : default(bool?) : false,
          attrFunctionEx != null ? attrFunctionEx.ParameterTypeSemantics : default(ParameterTypeSemantics?),
          parameters, Enumerable.Repeat(result, 1));
      }
      //  IQueryable
      returnType = mi.ReturnType == typeof(IQueryable) ? mi.ReturnType : mi.ReturnType.GetInterfaces().SingleOrDefault(t => t == typeof(IQueryable));
      if (returnType != null)
      {
        FunctionResultAttribute attrResult = mi.ReturnParameter.GetCustomAttribute<FunctionResultAttribute>();
        if (attrResult != null)
        {
          ResultDescriptor result = new ResultDescriptor(attrResult.Type)
          {
            ColumnName = attrResult.ColumnName ?? _resultColumnName,
            StoreTypeName = attrResult.TypeName
          };
          return new FunctionDescriptor(_namespaceName, databaseSchema, functionName, true,
            attrFunctionEx != null ? attrFunctionEx.IsComposable : default(bool?),
            isExtension ? attrFunctionEx != null ? attrFunctionEx.IsBuiltIn : default(bool?) : false,
            false,
            parameters.Length == 0 ? attrFunctionEx != null ? attrFunctionEx.IsNiladic : default(bool?) : false,
            attrFunctionEx != null ? attrFunctionEx.ParameterTypeSemantics : default(ParameterTypeSemantics?),
            parameters, Enumerable.Repeat(result, 1));
        }
        throw new InvalidOperationException("Result type is not specified in function");
      }
      //  IEnumerable<>
      returnType = mi.ReturnType == typeof(string) || mi.ReturnType == typeof(byte[]) ? null :
        mi.ReturnType.IsGenericType && mi.ReturnType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ? mi.ReturnType :
        mi.ReturnType.GetInterfaces().SingleOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
      if (returnType != null)
      {
        FunctionResultAttribute attrResult = mi.ReturnParameter.GetCustomAttribute<FunctionResultAttribute>();
        IEnumerable<ResultDescriptor> results =
          Enumerable.Repeat(new ResultDescriptor(returnType.GetGenericArguments()[0])
          {
            ColumnName = attrResult != null ? attrResult.ColumnName : _resultColumnName,
            StoreTypeName = attrResult != null ? attrResult.TypeName : null
          }, 1)
          .Concat(mi.GetCustomAttributes<FunctionResultAttribute>()
            .Select(a => new ResultDescriptor(a.Type)
              {
                ColumnName = a.ColumnName ?? _resultColumnName,
                StoreTypeName = a.TypeName
              }));
        return new FunctionDescriptor(_namespaceName, databaseSchema, functionName, true, false, false, false,
          parameters.Length == 0 ? attrFunctionEx != null ? attrFunctionEx.IsNiladic : default(bool?) : false,
          attrFunctionEx != null ? attrFunctionEx.ParameterTypeSemantics : default(ParameterTypeSemantics?),
          parameters, results);
      }
      //  IEnumerable
      returnType = mi.ReturnType == typeof(string) || mi.ReturnType == typeof(byte[]) ? null :
        mi.ReturnType == typeof(IEnumerable) ? mi.ReturnType : mi.ReturnType.GetInterfaces().SingleOrDefault(t => t == typeof(IEnumerable));
      if (returnType != null)
      {
        IEnumerable<ResultDescriptor> results = mi.GetCustomAttributes<FunctionResultAttribute>()
          .Select(a => new ResultDescriptor(a.Type)
          {
            ColumnName = a.ColumnName ?? _resultColumnName,
            StoreTypeName = a.TypeName
          });
        return new FunctionDescriptor(_namespaceName, databaseSchema, functionName, true, false, false, false,
          parameters.Length == 0 ? attrFunctionEx != null ? attrFunctionEx.IsNiladic : default(bool?) : false,
          attrFunctionEx != null ? attrFunctionEx.ParameterTypeSemantics : default(ParameterTypeSemantics?),
          parameters, results);
      }
      //  Scalar result
      returnType = mi.ReturnType;
      FunctionResultAttribute attr = mi.ReturnParameter.GetCustomAttribute<FunctionResultAttribute>();
      ResultDescriptor resultDescriptor = new ResultDescriptor(mi.ReturnType)
      {
        StoreTypeName = attr != null ? attr.TypeName : null
      };
      return new FunctionDescriptor(_namespaceName, databaseSchema, functionName, false,
        attrFunctionEx != null ? attrFunctionEx.IsComposable : default(bool?),
        isExtension ? attrFunctionEx != null ? attrFunctionEx.IsBuiltIn : default(bool?) : false,
        attrFunctionEx != null ? attrFunctionEx.IsAggregate : default(bool?),
        parameters.Length == 0 ? attrFunctionEx != null ? attrFunctionEx.IsNiladic : default(bool?) : false,
        attrFunctionEx != null ? attrFunctionEx.ParameterTypeSemantics : default(ParameterTypeSemantics?),
        parameters, Enumerable.Repeat(resultDescriptor, 1));
    }

    private static TypeUsage GetTypeUsage(EdmType edmType, SimpleTypeDescriptor simpleTypeDescriptor)
    {
      if (edmType.BuiltInTypeKind == BuiltInTypeKind.PrimitiveType)
      {
        PrimitiveType primitiveType = (PrimitiveType)edmType;
        switch (primitiveType.PrimitiveTypeKind)
        {
          case PrimitiveTypeKind.Binary:
            return simpleTypeDescriptor.Length.HasValue && simpleTypeDescriptor.Length.Value >= 0 ?
              TypeUsage.CreateBinaryTypeUsage(primitiveType, simpleTypeDescriptor.IsFixedLength.HasValue && simpleTypeDescriptor.IsFixedLength.Value, simpleTypeDescriptor.Length.Value) :
              simpleTypeDescriptor.IsFixedLength.HasValue ? TypeUsage.CreateBinaryTypeUsage(primitiveType, simpleTypeDescriptor.IsFixedLength.Value) : TypeUsage.CreateDefaultTypeUsage(primitiveType);
          case PrimitiveTypeKind.String:
            return simpleTypeDescriptor.Length.HasValue && simpleTypeDescriptor.Length.Value >= 0 ?
              TypeUsage.CreateStringTypeUsage(primitiveType, true, simpleTypeDescriptor.IsFixedLength.HasValue && simpleTypeDescriptor.IsFixedLength.Value, simpleTypeDescriptor.Length.Value) :
              simpleTypeDescriptor.IsFixedLength.HasValue ? TypeUsage.CreateStringTypeUsage(primitiveType, true, simpleTypeDescriptor.IsFixedLength.Value) : TypeUsage.CreateDefaultTypeUsage(primitiveType);
          case PrimitiveTypeKind.Decimal:
            return simpleTypeDescriptor.Precision.HasValue && simpleTypeDescriptor.Scale.HasValue ?
              TypeUsage.CreateDecimalTypeUsage(primitiveType, simpleTypeDescriptor.Precision.Value, simpleTypeDescriptor.Scale.Value) : TypeUsage.CreateDefaultTypeUsage(primitiveType);
          case PrimitiveTypeKind.DateTimeOffset:
            return TypeUsage.CreateDateTimeOffsetTypeUsage(primitiveType, simpleTypeDescriptor.Precision);
          case PrimitiveTypeKind.Time:
            return TypeUsage.CreateTimeTypeUsage(primitiveType, simpleTypeDescriptor.Precision);
        }
      }
      return TypeUsage.CreateDefaultTypeUsage(edmType);
    }

    private static TypeUsage GetStoreTypeUsage(DbModel model, SimpleTypeDescriptor simpleTypeDescriptor)
    {
      TypeUsage typeUsage = null;
      if (!string.IsNullOrEmpty(simpleTypeDescriptor.StoreTypeName))
      {
        PrimitiveType primitiveType = model.ProviderManifest.GetStoreTypes().SingleOrDefault(t => string.Compare(t.Name, simpleTypeDescriptor.StoreTypeName, true) == 0);
        if (primitiveType == null)
          throw new InvalidOperationException(string.Format("Store edm type with name '{0}' is not found.", simpleTypeDescriptor.StoreTypeName));
        typeUsage = GetTypeUsage(primitiveType.GetEdmPrimitiveType(), simpleTypeDescriptor);
        if (typeUsage == null)
          throw new InvalidOperationException(string.Format("Type usage for type with name '{0}' is not found.", simpleTypeDescriptor.StoreTypeName));
      }
      else
      {
        SimpleType simpleType = GetSimpleEdmType(model, simpleTypeDescriptor.Type);
        if (simpleType == null)
          throw new InvalidOperationException(string.Format("Edm type is not found for type '{0}'.", simpleTypeDescriptor.Type.FullName));
        typeUsage = GetTypeUsage(simpleType, simpleTypeDescriptor);
        if (typeUsage == null)
          throw new InvalidOperationException(string.Format("Type usage for type with name '{0}' is not found.", simpleTypeDescriptor.StoreTypeName));
      }
      return model.ProviderManifest.GetStoreType(typeUsage);
    }

    private static SimpleType GetSimpleEdmType(DbModel model, Type type)
    {
      SimpleType edmType = null;
      type = Nullable.GetUnderlyingType(type) ?? type;
      if (type.IsEnum)
      {
        edmType = model.ConceptualModel.EnumTypes.FirstOrDefault(t => t.FullName == type.FullName);
        if (edmType != null)
          return edmType;
        type = Enum.GetUnderlyingType(type);
      }
      edmType = PrimitiveType.GetEdmPrimitiveTypes().FirstOrDefault(t => t.ClrEquivalentType == type);
      return edmType;
    }

    private static StructuralType GetStructuralEdmType(DbModel model, Type type)
    {
      return model.ConceptualModel.EntityTypes
        .Cast<StructuralType>()
        .Concat(model.ConceptualModel.ComplexTypes)
        .FirstOrDefault(t => t.Name == type.Name);
    }

    private static EdmType CreateResultType(DbModel model, ResultDescriptor result)
    {
      EdmType edmType = GetSimpleEdmType(model, result.Type);
      if (edmType == null)
        edmType = GetStructuralEdmType(model, result.Type);
      if (edmType == null)
        throw new InvalidOperationException(string.Format("Edm type is not found for result type {0}.", result.Type.FullName));

      switch (edmType.BuiltInTypeKind)
      {
        case BuiltInTypeKind.EntityType:
          var propertyMappings = ((EntityType)edmType).Properties.Join(
            model.ConceptualToStoreMapping.EntitySetMappings
              .SelectMany(t => t.EntityTypeMappings)
              .Where(t => edmType.Yield(e => e.BaseType, (e, b) => b, e => e != null).Contains(t.EntityType))
              .SelectMany(tm => tm.Fragments.SelectMany(t => t.PropertyMappings))
              .OfType<ScalarPropertyMapping>(),
            p => p,
            pm => pm.Property,
            (p, pm) => new
            {
              Property = p,
              TypeUsage = TypeUsage.Create(pm.Column.TypeUsage.EdmType, pm.Column.TypeUsage.Facets.Where(f => !new[] { "StoreGeneratedPattern", "ConcurrencyMode" }.Contains(f.Name)))
            });
          return RowType.Create(propertyMappings.Select(pm => EdmProperty.Create(pm.Property.Name, pm.TypeUsage)), null);
        case BuiltInTypeKind.ComplexType:
          return RowType.Create(((StructuralType)edmType).Members.Select(m =>
          {
            string columnName = null;
            MetadataProperty metadata;
            //if (!m.MetadataProperties.TryGetValue("Configuration", true, out metadata) || !metadata.Value.TryGetProperty("ColumnName", out columnName))
            if (m.MetadataProperties.TryGetValue("ClrAttributes", true, out metadata))
            {
              var columnAttr = ((IEnumerable)m.MetadataProperties["ClrAttributes"].Value).OfType<ColumnAttribute>().SingleOrDefault();
              if (columnAttr != null && !string.IsNullOrEmpty(columnAttr.Name))
                columnName = columnAttr.Name;
            }
            return EdmProperty.Create(columnName ?? m.Name, model.ProviderManifest.GetStoreType(m.TypeUsage));
          }), null);
        case BuiltInTypeKind.EnumType:
          return RowType.Create(new[] { EdmProperty.CreateEnum(result.ColumnName, (EnumType)edmType) }, null);
        case BuiltInTypeKind.PrimitiveType:
          return RowType.Create(new[] { EdmProperty.CreatePrimitive(result.ColumnName, (PrimitiveType)edmType) }, null);
        default:
          throw new NotSupportedException();
      }
    }

    private static EdmFunction CreateStoreFunction(DbModel model, FunctionDescriptor descriptor)
    {
      var parameters = descriptor.Parameters.Select(p =>
        {
          TypeUsage typeUsage = GetStoreTypeUsage(model, p);
          return FunctionParameter.Create(p.Name, typeUsage.EdmType,
            p.Direction == ParameterDirection.Output ? ParameterMode.Out :
            p.Direction == ParameterDirection.InputOutput ? ParameterMode.InOut :
            p.Direction == ParameterDirection.ReturnValue ? ParameterMode.ReturnValue :
            ParameterMode.In);
        }).ToArray();

      var results =
        !descriptor.IsTableValued ?
          descriptor.Results.Select(r => FunctionParameter.Create("Result", GetStoreTypeUsage(model, r).EdmType, ParameterMode.ReturnValue))
          .ToArray() :
        !descriptor.IsComposable.HasValue || descriptor.IsComposable.Value ?
          descriptor.Results.Take(1).Select((r, i) => FunctionParameter.Create(string.Format("Result_{0}", i), CreateResultType(model, r).GetCollectionType(), ParameterMode.ReturnValue))
          .ToArray() :
        new FunctionParameter[0];

      var payload = new EdmFunctionPayload
      {
        StoreFunctionName = descriptor.FunctionName,
        IsFunctionImport = false,
        IsAggregate = descriptor.IsAggregate,
        IsBuiltIn = descriptor.IsBuiltIn,
        IsComposable = descriptor.IsComposable,
        IsNiladic = descriptor.IsNiladic,
        ParameterTypeSemantics = descriptor.ParameterTypeSemantics,
        Schema = descriptor.DatabaseSchema,
        Parameters = parameters,
        ReturnParameters = results,
      };
      return EdmFunction.Create(descriptor.FunctionName, descriptor.NamespaceName, DataSpace.SSpace, payload, null);
    }

    private static EdmFunction CreateFunctionImport(DbModel model, FunctionDescriptor descriptor)
    {
      var parameters = descriptor.Parameters.Select(p =>
        {
          TypeUsage typeUsage = GetTypeUsage(GetSimpleEdmType(model, p.Type), p);
          return FunctionParameter.Create(p.Name, typeUsage.EdmType,
            p.Direction == ParameterDirection.Output ? ParameterMode.Out :
            p.Direction == ParameterDirection.InputOutput ? ParameterMode.InOut :
            p.Direction == ParameterDirection.ReturnValue ? ParameterMode.ReturnValue :
            ParameterMode.In);
        }).ToArray();

      var entitySets = new EntitySet[descriptor.Results.Count];

      var results = descriptor.Results.Select((r, i) =>
        {
          EdmType edmType = GetStructuralEdmType(model, r.Type);
          if (edmType == null)
            edmType = GetTypeUsage(GetSimpleEdmType(model, r.Type), r).EdmType;
          entitySets[i] = edmType.BuiltInTypeKind != BuiltInTypeKind.EntityType ? null : model.ConceptualModel.Container.EntitySets.FirstOrDefault(s => edmType.Yield(t => t.BaseType, (t, b) => b, t => t != null).Contains(s.ElementType));
          return FunctionParameter.Create(string.Format("Result_{0}", i), edmType.GetCollectionType(), ParameterMode.ReturnValue);
        }).ToArray();

      var payload = new EdmFunctionPayload
      {
        StoreFunctionName = descriptor.FunctionName,
        IsFunctionImport = true,
        IsAggregate = descriptor.IsAggregate,
        IsBuiltIn = descriptor.IsBuiltIn,
        IsComposable = descriptor.IsComposable,
        IsNiladic = descriptor.IsNiladic,
        ParameterTypeSemantics = descriptor.ParameterTypeSemantics,
        Schema = descriptor.DatabaseSchema,
        Parameters = parameters,
        ReturnParameters = results,
        EntitySets = entitySets,
      };

      return EdmFunction.Create(descriptor.FunctionName, model.ConceptualModel.Container.Name, DataSpace.CSpace, payload, null);
    }

    void IStoreModelConvention<EntityContainer>.Apply(EntityContainer container, DbModel model)
    {
      foreach (MethodInfo mi in _containerType.GetMethods().Where(mi => mi.IsDefined(typeof(DbFunctionAttribute))))
      {
        FunctionDescriptor descriptor = BuildFunctionDescriptor(mi);
        EdmFunction storeFunction = CreateStoreFunction(model, descriptor);
        model.StoreModel.AddItem(storeFunction);
        if (descriptor.IsTableValued)
        {
          EdmFunction functionImport = CreateFunctionImport(model, descriptor);
          model.ConceptualModel.Container.AddFunctionImport(functionImport);
          if (functionImport.IsComposableAttribute)
          {
            FunctionImportResultMapping resultMapping = new FunctionImportResultMapping();
            if (functionImport.ReturnParameter.TypeUsage.EdmType.BuiltInTypeKind == BuiltInTypeKind.CollectionType)
            {
              EdmType itemType = ((CollectionType)functionImport.ReturnParameter.TypeUsage.EdmType).TypeUsage.EdmType;
              RowType rowType = (RowType)((CollectionType)storeFunction.ReturnParameter.TypeUsage.EdmType).TypeUsage.EdmType;
              if (itemType.BuiltInTypeKind == BuiltInTypeKind.ComplexType)
              {
                ComplexType complexType = (ComplexType)itemType;
                resultMapping.AddTypeMapping(new FunctionImportComplexTypeMapping(complexType, new Collection<FunctionImportReturnTypePropertyMapping>(
                  complexType.Properties.Select(p =>
                  {
                    var columnAttr = ((IEnumerable)p.MetadataProperties["ClrAttributes"].Value).OfType<ColumnAttribute>().SingleOrDefault();
                    return new FunctionImportReturnTypeScalarPropertyMapping(p.Name, columnAttr != null && !string.IsNullOrEmpty(columnAttr.Name) ? columnAttr.Name : p.Name);
                  }).ToArray())));
              }
            }
            model.ConceptualToStoreMapping.AddFunctionImportMapping((FunctionImportMapping)new FunctionImportMappingComposable(functionImport, storeFunction, resultMapping, model.ConceptualToStoreMapping));
          }
          else
            model.ConceptualToStoreMapping.AddFunctionImportMapping((FunctionImportMapping)new FunctionImportMappingNonComposable(functionImport, storeFunction, new FunctionImportResultMapping[0], model.ConceptualToStoreMapping));
        }
      }
    }
  }

  public class FunctionsConvention<T> : FunctionsConvention
  {
    public FunctionsConvention(string databaseSchema = null, string resultColumnName = null)
      : base(typeof(T), databaseSchema, resultColumnName)
    {
    }
  }
}
