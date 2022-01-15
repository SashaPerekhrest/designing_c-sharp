using System;
using System.Numerics;

namespace Incapsulation.RationalNumbers
{
    public class Rational
    {
        private int numerator;
        private int denominator;

        public int Numerator => numerator;
        public int Denominator => denominator;
        public bool IsNan => denominator == 0;
        public static readonly Rational NaN = new Rational(0,0);

        public Rational(int a, int b)
        {
            if (b < 0)
            {
                a *= -1;
            }
            else if (b == 0) a = 0;
            else if (a==0) b = 1;
            
            numerator = a;
            denominator = Math.Abs(b);

            Reduce();
        }

        public Rational(int a)
        {
            numerator = a;
            denominator = 1;
        }

        private void Reduce()
        {
            var nod = 1;
            if(denominator != 0 && numerator != 0)
                nod = (int)BigInteger.GreatestCommonDivisor(denominator, numerator);
            
            numerator /= nod;
            denominator /= nod;
        }

        private static void ReduceCommonDenominator(Rational a, Rational b)
        {
            if (a.denominator != b.denominator)
            {
                var mn_a = b.denominator;
                var mn_b = a.denominator;

                a.denominator *= b.denominator;    
                a.numerator = a.numerator * mn_a;

                b.denominator = a.denominator;
                b.numerator = b.numerator * mn_b;
            }
        }
        
        public static Rational operator +(Rational a, Rational b)
        {
            if (a.IsNan || b.IsNan)
            {
                return NaN;
            } 
            ReduceCommonDenominator(a, b);
            return new Rational(a.Numerator + b.Numerator, a.Denominator);
        }

        public static Rational operator -(Rational a, Rational b)
        {
            if (a.IsNan || b.IsNan)
            {
                return NaN;
            }
            ReduceCommonDenominator(a, b);
            return new Rational(a.Numerator - b.Numerator, a.Denominator);
        }

        public static Rational operator *(Rational a, Rational b) => 
            new Rational(a.Numerator * b.Numerator, a.Denominator * b.Denominator);

        public static Rational operator /(Rational a, Rational b)
        {
            if (a.IsNan || b.IsNan)
            {
                return new Rational(a.Numerator * b.Denominator, 0);
            }
            return new Rational(a.Numerator * b.Denominator, a.Denominator * b.Numerator);
        }
        
        public static implicit operator double(Rational a) => (double)a.Numerator / a.Denominator;

        public static implicit operator Rational(int a) =>  new Rational(a);

        public static explicit operator int(Rational a)
        {
            var res = (double)a.Numerator / a.Denominator;
            var dec = Math.Truncate(res);
            if (res - dec == 0)
            {
                return (int)dec;
            } 
            throw new ArgumentException();
        }
    }
}