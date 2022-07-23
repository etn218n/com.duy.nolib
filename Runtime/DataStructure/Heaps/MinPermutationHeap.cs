using System;

namespace Nolib.DataStructure
{
    public class MinPermutationHeap<T> : BaseBinaryPermutationHeap<T> where T : IComparable
    {
        public void Build(T[] elements)
        {
            base.Build(elements, Compare);
        }
        
        private bool Compare(T a, T b)
        {
            return a.CompareTo(b) > 0;
        }
    }
}