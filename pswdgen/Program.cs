using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace pswdgen
{
    class Program
    {
        static string RandomString(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*()_+{},./<>?{}:~`";
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }

            return res.ToString();
        }


        //  <Sarang.Baheti> 06-Mar-2018
        //  https://stackoverflow.com/a/43078669/916549
        const int STD_OUTPUT_HANDLE = -11;
        const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 4;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        static void WriteWithUnderLine(string message, ConsoleColor color)
        {
            var handle = GetStdHandle(STD_OUTPUT_HANDLE);
            uint mode;
            GetConsoleMode(handle, out mode);
            mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
            SetConsoleMode(handle, mode);

            const string UNDERLINE = "\x1B[4m";
            const string RESET = "\x1B[0m";
            var bkpColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(UNDERLINE + message + RESET);
            Console.ForegroundColor = bkpColor;
        }

        static void Main(string[] args)
        {
            Console.WriteLine();

            var currentForeGroundColor = Console.ForegroundColor;
            try
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("A handy utility to generate random passwords");
                Console.WriteLine("Copyright c Sarang Baheti 2018");
                Console.Write("source available at: ");
                WriteWithUnderLine("https://www.github.com/angeleno/pswdgen", ConsoleColor.Green);
                Console.WriteLine();
                Console.WriteLine("usage: pswdgen <numchars>{opt} <numpswds>{opt}");
                Console.WriteLine("       default will generate 5<numpswds> of 20<numchars> each");
                Console.WriteLine();

                int length = 20;
                int numPasswords = 5;
                if (args != null)
                {
                    if (args.Length > 0)
                        length = Convert.ToInt32(args[0]);

                    if (args.Length > 1)
                        numPasswords = Convert.ToInt32(args[1]);
                }

                Console.WriteLine("------------------------Passwords------------------------\n");
                Console.ForegroundColor = ConsoleColor.Yellow;
                for (int idx = 0; idx < numPasswords; ++idx)
                    Console.WriteLine(RandomString(length));
            }
            finally
            {
                Console.ForegroundColor = currentForeGroundColor;
            }
        }
    }
}

