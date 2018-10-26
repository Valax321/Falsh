using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhileFalseStudios.Falsh
{
    /// <summary>
    /// Compatibility classes for porting between .NET core and .NET framework
    /// </summary>
    static class CoreCompat
    {
        public static void AppendJoin(this StringBuilder sb, char separator, IEnumerable<string> args)
        {
            foreach (var a in args)
            {
                sb.Append(a);
                sb.Append(separator);
            }

            sb.Remove(sb.Length - 1, 1);
        }

        public static void AppendJoin(this StringBuilder sb, string separator, IEnumerable<string> args)
        {
            foreach (var a in args)
            {
                sb.Append(a);
                sb.Append(separator);
            }

            sb.Remove(sb.Length - separator.Length, separator.Length);
        }

        public static void AppendJoin(this StringBuilder sb, IEnumerable<string> args)
        {
            foreach (var a in args)
            {
                sb.Append(a);
            }
        }

        public static bool TryDequeue<T>(this Queue<T> queue, out T value)
        {
            try
            {
                value = queue.Dequeue();
                return true;
            }
            catch (Exception)
            {
                value = default(T);
                return false;
            }
        }
    }
}
