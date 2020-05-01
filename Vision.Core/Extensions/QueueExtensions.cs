using System;
using System.Collections.Generic;
using System.Text;

namespace Vision.Core.Extensions
{
    public static class QueueExtensions
    {
        public static void DequeueAll<T>(this Queue<T> queue, Action<T> consumer)
        {
            while (queue?.Count > 0) consumer.Invoke(queue.Dequeue());
        } 
    }
}
