using System;
using System.IO;
using FS = System.IO.FileStream;

namespace UsingDeclaration
{
    internal class Program
    {
        static void Main(string[] args)
        {
            long someValue = 0x123456789ABCDEF0;
            Console.WriteLine("{0,-1} : 0x{1:X16}", "Original Data", someValue);

            using (Stream outStream = new FS("a.dat", FileMode.Create))
            {
                byte[] wBytes = BitConverter.GetBytes(someValue);

                Console.Write("{0,-13} : ", "Byte array");

                foreach (byte b in wBytes)
                {
                    Console.Write("{0:X2} ", b);
                }
                Console.WriteLine();

                outStream.Write(wBytes, 0, wBytes.Length);
            }

            using Stream inStream = new FS("a.dat", FileMode.Open);
            byte[] rbytes = new byte[8];

            int i = 0;

            while (inStream.Position < inStream.Length)
            {
                rbytes[i++] = (byte)inStream.ReadByte();
            }

            long readValue = BitConverter.ToInt64(rbytes, 0);

            Console.WriteLine("{0,-13} : 0x{1:X16} ", "Read Data", readValue);

        }
    }
}
