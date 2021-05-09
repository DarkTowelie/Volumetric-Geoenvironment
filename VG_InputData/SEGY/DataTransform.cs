using System;
using System.Collections.Generic;
using System.Text;

namespace VG_InputData.SEGY
{
    public static class DataTransform
    {
        public static double IBMToDouble(byte[] data)
        {
            double k;
            int sign = (data[0] >> 7) & 1;
            int exp = data[0] & 0x7F;
            int fraction = data[1] << 16 | (data[2] << 8) | (data[3]);

            double sum = 0;
            for (int i = 0; i < 24; i++)
            {
                k = Math.Pow(2, i - 24);
                sum += (fraction & 0x01) * k;
                fraction >>= 1;
            }

            if (sign == 1) sum = -sum;
            return sum * Math.Pow(16, exp - 64);
        }
        public static byte[] DoubleToIBM(double data)
        {
            int ind, c1;
            int i = 0, j;
            double b, c;

            //порядок
            b = Math.Abs(data);
            if (b >= 1)
            {
                while (b >= 1)
                {
                    b /= 16;
                    i++;
                }
                i += 64;
            }
            else
            {
                while (b <= 1 && i <= 64)
                {
                    b *= 16;
                    i++;
                }
                b /= 16;
                i = 64 - i + 1;
            }
            ind = (i & 0x7f) << 24;

            //мантисса
            for (i = 1; i <= 6; i++)
            {
                b *= 16;
                c = Math.Truncate(b);
                b -= c;
                j = 24 - i * 4;

                c1 = Convert.ToInt32(c);
                c1 = (c1 & 0xf) << j;
                ind |= c1;
            }
            // знак
            if (data < 0)
            {
                ind += -2147483648;
            }
            byte[] fourbytedata = BitConverter.GetBytes(ind);
            Array.Reverse(fourbytedata);
            return fourbytedata;
        }
        public static double ByteToDouble(int Format_code, byte[] byte_buffer)
        {
            switch (Format_code)
            {
                case (1):
                    return IBMToDouble(byte_buffer);
                case (5):
                    return BitConverter.ToDouble(byte_buffer, 0);
                default:
                    return 0;
            }
        }
        public static int ByteToInt(byte[] buffer)
        {
            int buffer_length = buffer.GetLength(0);
            switch (buffer_length)
            {
                case (2): return BitConverter.ToInt16(buffer, 0);
                case (4): return BitConverter.ToInt32(buffer, 0);
                default: return (0);
            }
        }
        public static byte[] IntToReversedByte(int buffer_length, int variable)
        {
            byte[] copy_buffer = null;
            switch (buffer_length)
            {
                case (2): { Int16 i16_variable = Convert.ToInt16(variable); copy_buffer = BitConverter.GetBytes(i16_variable); break; }
                case (4): copy_buffer = BitConverter.GetBytes(variable); break;
            }
            byte[] buffer = new byte[buffer_length];
            for (int i = 0; i < buffer_length; i++)
            {
                buffer[i] = copy_buffer[buffer_length - i - 1];
            }
            return buffer;

        }
        public static byte[] GetRevBytesFromHeaderBuffer(byte[] digidal_header_buffer, int[,] Template, int current_position)
        {
            int buffer_size = Template[current_position, 1];
            byte[] buffer = new byte[buffer_size];
            for (int i = 0; i < buffer_size; i++)
            {
                buffer[buffer_size - (i + 1)] = digidal_header_buffer[Template[current_position, 0] + i];
            }
            return buffer;
        }
        public static byte[] GetBytesFromBuffer(byte[] buffer, int position, int num_of_reading_samples)
        {
            byte[] extracted_bytes = new byte[num_of_reading_samples];
            for (int i = 0; i < num_of_reading_samples; i++)
            {
                extracted_bytes[i] = buffer[position + i];
            }
            return extracted_bytes;
        }
    }
}
