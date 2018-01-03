using System.Runtime.CompilerServices;
using System.Text;

namespace Beeline.Writers
{
    public static class Comma
    {
        private const byte CommaByte = 44;
        private const byte SpaceByte = 32;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(byte[] buffer, ref int pos)
        {
            if (pos == 2) return;
            buffer[pos++] = CommaByte;
            buffer[pos++] = SpaceByte;
        }
    }
}