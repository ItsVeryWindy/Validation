using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Validation
{ 
    public interface IField<out T>
    {
        PropertyPath Path { get; }
        object OriginalValue { get; }
        T Value { get; }
        IFieldInfo Info { get; }

        IField<TChild> CreateChildField<TChild>(IFieldInfo fieldInfo, TChild value, object originalValue);
        IField<TChild> CreateChildField<TChild>(IFieldInfo fieldInfo, TChild value, object originalValue, IErrorMessageFactory factory);
        IValidatorContext<T> CreateValidatorContext();
    }

    public static class FieldExtensions
    {
        public static IField<TChild> CreateChildField<TParent, TChild>(this IField<TParent> field, IFieldInfo fieldInfo, TChild value)
        {
            return field.CreateChildField(fieldInfo, value, value);
        }

        public static IField<TChild> CreateChildField<TParent, TChild>(this IField<TParent> field, IFieldInfo fieldInfo, TChild value, IErrorMessageFactory errorMessageFactory)
        {
            return field.CreateChildField(fieldInfo, value, value);
        }
    }
}
