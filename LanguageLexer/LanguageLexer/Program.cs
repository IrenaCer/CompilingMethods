using System;
using System.IO;

namespace LanguageLexer
{
    class MainClass
    {
        public static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please enter name of the file");
                return 1;
            }

            string path = args[0];

            string input = string.Empty;

            if (File.Exists(path))
            {
                using (StreamReader sr = File.OpenText(path))
                {
                    input = sr.ReadToEnd();
                }

                //Console.WriteLine(input);

                Lexer lexer = new Lexer(input, path);
                lexer.lex();

                return 0;
            }
            else
            {
                Console.WriteLine("Invalid Filename");
                return 1;
            }

        }
    }
}
