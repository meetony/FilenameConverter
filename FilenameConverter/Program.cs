using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FilenameConverter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool result = true;

            if (args.Length == 0) return;

            foreach (string path in args)
                result &= ConvertFileNames(path);

            if (!result)
            {
                Console.WriteLine("\nPress any key to exit.");
                Console.ReadKey();
            }
        }

        private static bool ConvertFileNames(string path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            
            if (!dirInfo.Exists)
            {
                Console.WriteLine("Directory not found: {0}", dirInfo.FullName);
                return false;
            };

            Stack<DirectoryInfo> directories = new Stack<DirectoryInfo>();
            directories.Push(dirInfo);

            bool result = true;

            while (directories.Count > 0)
            {
                DirectoryInfo currentDir = directories.Pop();
                string newDirName = ConvChars(currentDir.Name);

                if (currentDir.Name != newDirName)
                {
                    try
                    {
                        currentDir.MoveTo(currentDir.Parent.FullName + @"\" + newDirName);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("1 {0}: {1}", e.Message, currentDir.FullName);
                        result = false;
                    }
                }
                try
                {
                    foreach (DirectoryInfo dir in currentDir.GetDirectories()) directories.Push(dir);
                }
                catch(Exception e)
                {
                    Console.WriteLine("2 {0}: {1}", e.Message, currentDir.FullName);
                    result = false;
                }

                FileInfo[] files = null;
                try
                {
                    files = currentDir.GetFiles();
                }
                catch(Exception e)
                {
                    Console.WriteLine("3 {0}: {1}", e.Message, currentDir.FullName);
                    result = false;
                }

                foreach (FileInfo file in files)
                {
                    string newFileName = ConvChars(file.Name);
                    if (file.Name != newFileName)
                    {
                        try
                        {
                            file.MoveTo(file.DirectoryName + @"\" + newFileName);
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine("4 {0}: {1}", e.Message, file.FullName);
                            result = false;
                        }
                    }
                }
            }
            return result;
        }
        private static string ConvChars(string s)
        {
            StringBuilder sb = new StringBuilder(s.Length);

            foreach (char c in s)
            {
                char c2 = c;
                if (c >= '０' && c <= '９')
                    c2 = (char)(c - '０' + '0');
                else if (c >= 'Ａ' && c <= 'Ｚ')
                    c2 = (char)(c - 'Ａ' + 'A');
                else if (c >= 'ａ' && c <= 'ｚ')
                    c2 = (char)(c - 'ａ' + 'a');
                else
                    switch (c)
                    {
                        case ' ':
                            c2 = '_';
                            break;
                        case '＿':
                            c2 = '_';
                            break;
                        case '（':
                            c2 = '(';
                            break;
                        case '）':
                            c2 = ')';
                            break;
                        case '［':
                            c2 = '[';
                            break;
                        case '］':
                            c2 = ']';
                            break;
                        case '｛':
                            c2 = '{';
                            break;
                        case '｝':
                            c2 = '}';
                            break;
                        case '・':
                            c2 = '･';
                            break;
                    }
                sb.Append(c2);
            }
            return sb.ToString();
        }
    }
}
