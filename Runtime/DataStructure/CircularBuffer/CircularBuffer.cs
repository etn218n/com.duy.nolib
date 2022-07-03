using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nolib.DataStructure
{
    public class CircularBuffer<T> : IEnumerable
    {
        private T[] buffer;
        private int head;
        private int tail;
        private int count;
        private int currentCapacity => buffer.Length;
        
        public int Head => head;
        public int Tail => tail;
        public int Count => count;
        public bool IsEmpty => count == 0;

        public CircularBuffer(int capacity)
        {
            head = tail = 0;
            buffer = new T[capacity];
        }

        public void Resize(int newCapacity)
        {
            if (newCapacity == currentCapacity)
                return;
            
            if (newCapacity > currentCapacity)
                GrowBy(newCapacity);
            else
                ShrinkHeadBy(newCapacity);
        }

        public void GrowBy(int deltaCapacity)
        {
            var newCapacity = currentCapacity + deltaCapacity;
            
            if (newCapacity <= currentCapacity)
                return;

            buffer = ResizeBuffer(newCapacity, head, count);
        }

        public void ShrinkHeadBy(int deltaCapacity)
        {
            var newCapacity = currentCapacity - deltaCapacity;
            
            if (newCapacity >= buffer.Length)
                return;
            
            var shrinkSlots    = currentCapacity - newCapacity;
            var remainingSlots = currentCapacity - count;
            var removeSlots    = Math.Abs(remainingSlots - shrinkSlots);

            buffer = ResizeBuffer(newCapacity, IncrementIndex(head, removeSlots), count - removeSlots);
            count -= removeSlots;
        }
        
        public void ShrinkTailBy(int deltaCapacity)
        {
            var newCapacity = currentCapacity - deltaCapacity;
            
            if (newCapacity >= buffer.Length)
                return;
            
            var shrinkSlots    = currentCapacity - newCapacity;
            var remainingSlots = currentCapacity - count;
            var removeSlots    = Math.Abs(remainingSlots - shrinkSlots);

            buffer = ResizeBuffer(newCapacity, head, count - removeSlots);
            count -= removeSlots;
        }

        public void PushHead(IEnumerable<T> elements)
        {
            foreach (var element in elements)
                PushHead(element);
        }
        
        public void PushTail(IEnumerable<T> elements)
        {
            foreach (var element in elements)
                PushTail(element);
        }
        
        public void PushHead(T element)
        {
            if (count == 0)
            {
                buffer[head] = element;
                count++;
                return;
            }
            
            head = DecrementIndex(head);

            if (head == tail)
                tail = DecrementIndex(tail);
            else
                count++;

            buffer[head] = element;
        }

        public void PushTail(T element)
        {
            if (count == 0)
            {
                buffer[tail] = element;
                count++;
                return;
            }
            
            tail = IncrementIndex(tail);

            if (tail == head)
                head = IncrementIndex(head);
            else
                count++;

            buffer[tail] = element;
        }

        public T[] PopHead(int popCount)
        {
            if (popCount > count)
                return Array.Empty<T>();

            var resultBuffer = new T[popCount];
            
            for (int i = 0; i < popCount; i++)
                resultBuffer[i] = PopHead();

            return resultBuffer;
        }
        
        public T[] PopTail(int popCount)
        {
            if (popCount > count)
                return Array.Empty<T>();

            var resultBuffer = new T[popCount];
            
            for (int i = 0; i < popCount; i++)
                resultBuffer[i] = PopTail();

            return resultBuffer;
        }
        
        public void PopHead(ref T[] resultBuffer)
        {
            if (resultBuffer.Length > count)
                return;

            for (int i = 0; i < resultBuffer.Length; i++)
                resultBuffer[i] = PopHead();
        }
        
        public void PopTail(ref T[] resultBuffer)
        {
            if (resultBuffer.Length > count)
                return;

            for (int i = 0; i < resultBuffer.Length; i++)
                resultBuffer[i] = PopTail();
        }
        
        public T PopHead()
        {
            if (count == 0)
                return default;

            var element = buffer[head];

            if (count != 1)
                head = IncrementIndex(head);
            
            count--;
            return element;
        }

        public T PopTail()
        {
            if (count == 0)
                return default;

            var element = buffer[tail];
            
            if (count != 1)
                tail = DecrementIndex(tail);
            
            count--;
            
            return element;
        }
        
        public T PeekHead()
        {
            if (count == 0)
                return default;

            return buffer[head];
        }
        
        public T PeekTail()
        {
            if (count == 0)
                return default;

            return buffer[tail];
        }

        public bool Contains(T element)
        {
            var bufferIndex = head;
            
            for (int i = 0; i < count; i++)
            {
                if (element.Equals(buffer[bufferIndex]))
                    return true;
                
                bufferIndex = IncrementIndex(bufferIndex);
            }

            return false;
        }

        public void Clear()
        {
            head  = 0;
            tail  = 0;
            count = 0;
        }

        public T[] ToArray()
        {
            var array = new T[count];
            var bufferIndex = head;
            
            for (int i = 0; i < count; i++)
            {
                array[i] = buffer[bufferIndex];
                bufferIndex = IncrementIndex(bufferIndex);
            }

            return array;
        }

        public IEnumerator GetEnumerator()
        {
            var bufferIndex = head;
            
            for (int i = 0; i < count; i++)
            {
                yield return buffer[bufferIndex];
                bufferIndex = IncrementIndex(bufferIndex);
            }
        }

        public override string ToString()
        {
            return $"Buffer = [{string.Join(", ", buffer.ToArray())}] | Head = {head} | Tail = {tail} | Count = {count}";
        }

        private T[] ResizeBuffer(int newCapacity, int startIndex, int length)
        {
            var resizedBuffer = new T[newCapacity];
            var bufferIndex   = startIndex;
            
            for (int i = 0; i < length; i++)
            {
                resizedBuffer[i] = buffer[bufferIndex];
                bufferIndex = IncrementIndex(bufferIndex);
            }
            
            head = 0;
            tail = Math.Clamp(length - 1, 0, Int32.MaxValue);
            
            return resizedBuffer;
        }
        
        private int IncrementIndex(int index, int step = 1)
        {
            return (index + step) % currentCapacity;
        }

        private int DecrementIndex(int index)
        {
            if (index == 0)
                return currentCapacity - 1;

            return index - 1;
        }
    }
}
