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

        public static void LogError(string message) =>
            logFile.WriteLine($"Line number {LineParser.lineNumber}, Error: " + message);

        public static void Write(string message) => logFile.WriteLine(message);

        public static void LogLine(string label, string operation, string operands) =>
            logFile.WriteLine($"{LineParser.lineNumber,2} {Assembler.LocationCounter.ToString(),4} {label,-8} {operation,-8}        {operands,-8}");

        /// <summary>
        /// Logs that the current line is a comment
        /// </summary>
        /// <param name="lineNumber">line number</param>
        public static void LogLine(int lineNumber, string comment) => logFile.WriteLine($"{lineNumber,2} {comment}");

        public static void CloseLog() => logFile.Dispose();

        public static void LogLine(List<IntermediateFileEntry> table)
        {
            Write("--------------------");
            Write("Logging intermediate file");

            foreach (IntermediateFileEntry item in table)
            {
                // Don't print the object code if it's empy
                Write($"{item.Address.ToString("X"),3} {(item.ObjectCode != "" ? item.ObjectCode.PadLeft(6, '0') : null),6} {item.Label,8} {item.Operation,8}        {item.Value}");
            }
        }

        public static void LogLine(Dictionary<string, LitTabEntry> table)
        {
            Write("--------------------");
            Write("Logging litteral table");

            foreach (var item in table)
            {
                // Don't print the object code if it's empy
                Write($"{item.Value.Name},{item.Value.Address},{item.Value.Value}");
            }
        }
    }
}