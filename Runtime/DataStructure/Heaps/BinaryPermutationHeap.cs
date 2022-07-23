using System;

namespace Nolib.DataStructure
{
    public class BinaryPermutationHeap<T> : BaseBinaryPermutationHeap<T>
    {
        public new void Build(T[] elements, Func<T, T, bool> comparer)
        {
            base.Build(elements, comparer);
        }
    }
}