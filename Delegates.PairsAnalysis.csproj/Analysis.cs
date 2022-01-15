/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace Delegates.PairsAnalysis
{
    public static class Analysis
    {
        public static int FindMaxPeriodIndex(params DateTime[] data)
        {
            return data.Pairs().MaxIndex(getLenght=> (dynamic)getLenght.Item2 - (dynamic)getLenght.Item1);
        }
 
        public static double FindAverageRelativeDifference(params double[] data)
        {
            return data.Pairs().MaxIndex(getLenght => (getLenght.Item2 - getLenght.Item1)/getLenght.Item1);
        }
        
        public static Tuple<T, T>[] Pairs<T>(this T[] data)
        {
            if (data.Length < 2) throw new InvalidOperationException();
 
            var temp = new List<Tuple<T, T>>();
            for (int i = 0; i < data.Length - 1; i++)
                temp.Add(new Tuple<T, T>(data[i], data[i + 1]));
            return temp.ToArray();
        }

        public static int MaxIndex<T>(this Tuple<T, T>[] data, Func<Tuple<T, T>, dynamic> getLenght)
        {
            var bestIndex = 0;
            var max = getLenght(data[0]);
            
            for (int i = 1; i < data.Length; i++)
            {
                var len = getLenght(data[i]);
                if (len > max)
                {
                    max = len;
                    bestIndex = i;
                }
            }
 
            return bestIndex;
        }
        public static double MaxIndex<T>(this Tuple<T, T>[] data, Func<Tuple<T, T>, double> getLenght)
        {
            var result = 0.0;

            for (int i = 0; i < data.Length; i++)
            {
                result += getLenght(data[i]);
            }

            return result/data.Length;
        }
    }
}*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;


namespace Delegates.PairsAnalysis
{
    public static class Analysis
    {
        public static int FindMaxPeriodIndex(params DateTime[] data)
        {
            if (data.Length < 2) throw new InvalidOperationException();
            return data.Pairs().MaxIndex();
        }
 
        public static double FindAverageRelativeDifference(params double[] data)
        {
            if (data.Length < 2) throw new InvalidOperationException();
            return new AverageDifferenceFinder().Analyze(data);
        }
       
        public static IEnumerable<Tuple<T, T>> Pairs<T>(this IEnumerable<T> data)
        {
            var i = data.GetEnumerator();
            i.MoveNext();
            var prev = i.Current;

            while (i.MoveNext())
            {
                yield return new Tuple<T, T>(prev, i.Current);
                prev = i.Current;
            }
        }
        
        public static int MaxIndex<T>(this IEnumerable<Tuple<T, T>> data)
            where T : IComparable
        {
            var bestIndex = 0;
            var index = 0;
            dynamic max = default;

            foreach (var elem in data)
            {
                var len = (dynamic)elem.Item2 - (dynamic)elem.Item1;
                if (index == 0)
                    max = len;
                else if (len > max)
                {
                    max = len;
                    bestIndex = index;
                }

                index++;
            }
            return bestIndex;
        }
        
        public static int MaxIndex<T>(this IEnumerable<T> data) where T : IComparable
        {
            var enumerator = data.GetEnumerator();
            enumerator.MoveNext();

            var bestIndex = -1;
            var index = 1;
            var max = enumerator.Current;
 
            while(enumerator.MoveNext())
            {
                if ((dynamic)enumerator.Current > (dynamic)max)
                {
                    max = enumerator.Current;
                    bestIndex = index;
                }

                index++;
            }
			if (bestIndex == -1) throw new InvalidOperationException();
			
            return bestIndex;
        }
    }
}