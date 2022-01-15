using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Reflection.Randomness
{
    public class Generator<T> where T : new()
    {
        private Dictionary<PropertyInfo, IContinuousDistribution> properties;
        private string temporaryPropertyName;
 
        public Generator()
        {
            properties = typeof(T)
                .GetProperties()
                .Where(x => x.GetCustomAttributes(typeof(FromDistribution), false).Length != 0)
                .ToDictionary(x => x,
                              x => ((FromDistribution)x
                                  .GetCustomAttribute(typeof(FromDistribution), false))
                                  .Distribution);
        }
 
        public T Generate(Random rnd)
        {
            var result = new T();
            var settedProperties = properties
                .Select(x => x.Key)
                .ToArray();
            var distribution = properties
                .Select(x => x.Value)
                .ToArray();
            
            for (var i = 0; i < settedProperties.Length; i++)
            {
                settedProperties[i].SetValue(result, distribution[i].Generate(rnd));
            }

            return result;
        }
    }
 
    public class FromDistribution:Attribute
    {
        public IContinuousDistribution Distribution { get; }
 
        public FromDistribution(Type type, params object[] parameters)
        {
            if(type.GetInterface(typeof(IContinuousDistribution).Name) is null)
                throw new ArgumentException(
                    $"Type \"{type.Name}\" does not implement interface \"{typeof(IContinuousDistribution).Name}\"");
            if (!type.GetConstructors().Any(c => c.GetParameters().Length == parameters.Length))
                throw new ArgumentException(
                    $"Type \"{type.Name}\" does not have constructor with \"{parameters.Length}\" parametrs");
            
            Distribution = (IContinuousDistribution) Activator.CreateInstance(type, parameters);
        }
    }
}