using System.Collections.Generic;
using System.IO;

namespace Assembler_SIC
{
    /// <summary>
    /// Log file so to store the reords for each line
    /// </summary>
    public static class LogFile
    {
        private static StreamWriter logFile = new StreamWriter("Log.txt");

        static LogFile()
        {
            logFile.WriteLine("#  Loc label     operation       operands");
        }
        public static void logError(string message)
        {
            logFile.WriteLine($"Line number {LineParser.lineNumber}, Error: " + message);
        }

        public static void Write(string message)
        {
            logFile.WriteLine(message);
        }

        public static void logLine(string label, string operation, string operands)
        {
            logFile.WriteLine($"{LineParser.lineNumber,2} {Assembler.LocationCounter.ToString(),4} {label,-8} {operation,-8}        {operands,-8}");
        }

        /// <summary>
        /// Logs that the current line is a comment
        /// </summary>
        /// <param name="lineNumber">line number</param>
        public static void logLine(int lineNumber, string comment)
        {
            logFile.WriteLine($"{lineNumber,2} {comment}");
        }

        public static void CloseLog()
        {
            logFile.Dispose();
        }

        public static void logLine(List<IntermediateFileEntry> table)
        {
            LogFile.Write("--------------------");
            LogFile.Write("Logging intermediate file");

            foreach (IntermediateFileEntry item in table)
            {
                LogFile.Write($"{item.Address.ToString("X"),3} {item.ObjectCode,6} {item.Label,8} {item.Operation,8}        {item.Value}");
            }
        }
    }
}