using System;

namespace Incapsulation.Weights
{
    class Indexer
    {
        private static double[] array;
        private int start;
        private int length;

        public int Length => length;
        public int Start => start;
        
        public double this[int index]
        {
            get
            {
                if(CheckOutOfBounds(index))
                    throw new IndexOutOfRangeException();
                return array[Start + index];
            }
            set
            {
                if (CheckOutOfBounds(index))
                    throw new IndexOutOfRangeException();
                array[Start + index] = value;
            }
        }
        
        public Indexer(double[] ar, int start, int length)
        {
            if (CheckIncorrectBorders(ar, start, length))
                throw new ArgumentException();
            
            this.length = length;
            this.start = start;
            array = ar;
        }

        private bool CheckOutOfBounds(int index) => index < 0 || index > Length-1;
        private bool CheckIncorrectBorders(double[] ar, int start, int length) => 
            start < 0 || length < 0 || length > ar.Length || length > ar.Length - start;
    }
}