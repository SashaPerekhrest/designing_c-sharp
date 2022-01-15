using System;

namespace SRP.ControlDigit
{
    public static class Extensions
    {
        public static string AppendIfNeed(this string s, int count)
        {
            int p = count - s.Length; 
            for (int i = 0; i < p; i++)
            {
                s = "0" + s;
            }
            return s;
        }
        
        public static int RemoveAmbiguity(this int n) 
            => n > 9 ? n - 9 : n;
    }

    public static class ControlDigitAlgo
    {
        public static int Upc(long number)
        {
            var sum = 0;
            var factor = 3;
            
            for (;number > 0; number /= 10, factor = 4 - factor)
                sum += factor * (int)(number % 10);
            
            var result = sum % 10;
            return result != 0 ? 10-result : result;
        }

        public static int Isbn10(long number)
        {
            var numbs = number.ToString().AppendIfNeed(9);
            var sum = 0;
            for (var i = 0; i < numbs.Length; i++)
                sum += (numbs[i] - '0') * (numbs.Length - i + 1);
            var rem = sum % 11;
            if (rem == 0) return '0';
            if (rem == 1) return 'X';
            return (11 - rem).ToString().ToCharArray()[0];
        }

        public static int Luhn(long number)
        {
            var sNumber = number.ToString();
            var sum = 0;
            var isEven = sNumber.Length % 2 == 0;
            if (isEven) 
                sum = FindAmount(sNumber, i => (i - 1) % 2 == 0);
            else
                sum = FindAmount(sNumber, i => (i - 1) % 2 != 0);
            return sum * 9 % 10;
        }

        private static int FindAmount(string number, Func<int, bool> odd)
        {
            var sum = 0;
            for (var i = 0; i < number.Length; i++)
            {
                var n = number[i] - '0';
                if (odd(i)) n = (n*2).RemoveAmbiguity();
                sum += n;
            }
            return sum;
        }
    }
}