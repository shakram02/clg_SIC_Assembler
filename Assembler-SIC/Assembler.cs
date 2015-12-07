using System;
using System.IO;

namespace Assembler_SIC
{
    public static class Assembler
    {
        // Location counter
        public static int LocationCounter = 0;

        public static int ProgramLength = 0;
        public static int EndLocation = 0;
        public static int StartLocation = 0;

        // Source file name
        private static string _fileName;

        private static StreamReader sourceFile;

        // Kept this as a field because a property can't be an out parameter
        public static CodeLine CurrentCodeLine;

        /// <summary>
        /// Opens source file for reading
        /// </summary>
        /// <returns>True if the file was open, false otherwise</returns>
        public static bool OpenSourceFile()
        {
            try
            {
                sourceFile = new StreamReader(_fileName);
                return true;
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error reading source file:" + exc.Message);
            }
            return false;
        }
    }
}