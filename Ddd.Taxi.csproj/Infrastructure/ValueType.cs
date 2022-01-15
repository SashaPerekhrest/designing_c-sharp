using System.Linq;
using System.Reflection;

namespace Ddd.Infrastructure
{
    public class ValueType<T>
        where T : class
    {
        private static readonly PropertyInfo[] properties = typeof(T)
            .GetProperties()
            .Where(p => !p.GetGetMethod().IsStatic)
            .OrderBy(p => p.Name)
            .ToArray();
             
        public override int GetHashCode()
        {
            unchecked
            {
                var result = properties.Where(p => p.GetValue(this) != null)
                    .Select(p => p.GetValue(this).ToString().GetHashCode() * 
                        p.GetIndexParameters().GetHashCode() + 
                        p.Name.GetHashCode());

                var sum = 0;
                foreach (var item in result) 
                    sum += item;
                return sum;
            }
        }

        public override bool Equals(object obj) => ReferenceEquals(this, obj) || Equals(obj as T);

        public bool Equals(T value) 
            => value != null && properties.All(p => Equals(p.GetValue(this), p.GetValue(value)));

        public override string ToString()
        {
            var propertiesInfo = properties
                .Where(p => !p.GetGetMethod().IsStatic)
                .OrderBy(p => p.Name)
                .Select(p =>$"{p.Name}: {p.GetValue(this)}");

            return $"{GetType().Name}({string.Join("; ", propertiesInfo)})";
        }
    }
}