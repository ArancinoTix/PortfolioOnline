using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace VisualInspector.Editor.Core
{
    public class Evaluator
    {
        public T Evaluate<T>(InspectorData data, string expression)
        {
            var result = Evaluate(data, expression);

            if (result is T casted)
                return casted;

            throw new InvalidCastException($"The result of the expression is not of the expected type." +
                                           $"Expected: {typeof(T).Name}, Actual: {result.GetType().Name}");
        }
        
        public object Evaluate(InspectorData data, string expression)
        {
            if (expression.StartsWith("$")) return EvaluateMethod(data, expression);

            Debug.LogError("Other types of expressions are not supported yet");
            return null;
        }

        private object EvaluateMethod(InspectorData data, string expression)
        {
            if (string.IsNullOrEmpty(expression) || expression[0] != '$')
                throw new ArgumentException("Invalid expression format. It must start with a '$'.");

            // Extract the member name and parameters from the expression
            var memberRegex = new Regex(@"\$(\w+)(?:\((.*)\))?");
            var match = memberRegex.Match(expression);

            if (!match.Success) throw new ArgumentException("Invalid expression format.");

            var memberName = match.Groups[1].Value;
            var paramString = match.Groups[2].Value;

            // Get the member from the target object using reflection
            var targetType = data.Target.GetType();
            var member = targetType.GetMember(memberName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)[0];

            if (member == null) throw new ArgumentException($"Member '{memberName}' not found in the target object.");

            // If the member is a method, evaluate the parameters and invoke it
            if (member is MethodInfo method)
            {
                // Split the parameters and evaluate each one
                var paramValues = new List<object>();
                if (!string.IsNullOrEmpty(paramString))
                {
                    var rawParams = paramString.Split(',');
                    foreach (var rawParam in rawParams)
                    {
                        paramValues.Add(EvaluateParameterValue(data, rawParam));
                    }
                }

                // Invoke the method with the evaluated parameters
                return method.Invoke(data.Target, paramValues.ToArray());
            }

            // If the member is a property or field, return its value
            if (member is PropertyInfo property) return property.GetValue(data.Target);
            if (member is FieldInfo field) return field.GetValue(data.Target);

            throw new InvalidOperationException("Unsupported member type.");
        }

        private object EvaluateParameterValue(InspectorData data, string rawParam)
        {
            rawParam = rawParam.Trim();

            // If the parameter starts with a '$', evaluate it as an expression
            if (rawParam.StartsWith("$")) return Evaluate(data, rawParam);

            // Otherwise, parse the parameter as a constant
            if (int.TryParse(rawParam, out var intValue)) return intValue;
            if (double.TryParse(rawParam, out var doubleValue)) return doubleValue;
            if (bool.TryParse(rawParam, out var boolValue)) return boolValue;
            // Remove quotes if present and return as a string
            return rawParam.Trim('\'', '\"');
        }
    }
}