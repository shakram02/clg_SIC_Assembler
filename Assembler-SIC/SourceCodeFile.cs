using System;
using System.Diagnostics;
using System.IO;

namespace Assembler_SIC
{
    public struct CodeLine
    {
        public bool isComment;
        public bool isError;
        public string Label;
        public string Operation;
        public string Operands;
        public string Comment;

        public CodeLine(string Label, string Operation, string Operands, bool isComment = false, string Comment = null, bool isError = false)
        {
            this.Label = Label;
            this.isComment = isComment;
            this.Operation = Operation;
            this.Operands = Operands;
            this.Comment = Comment;
            this.isError = isError;
        }

        public bool Equals(CodeLine other)
        {
            return
                this.isComment == other.isComment ?
                    this.Label == other.Label ?
                        this.Operation == other.Operation ?
                            this.Operands == other.Operands ?
                                this.Comment == other.Comment ?
                                    this.isError == other.isError ?
                                    true : false
                                : false
                            : false
                        : false
                    : false
                : false;
        }
    }

    public static class SourceCodeFile
    {
        private static StreamReader sourceFile;
        private static int lineNumber = -1;

        public static StreamReader FileReader
        {
            get { return sourceFile; }
            set { sourceFile = value; }
        }

        static SourceCodeFile()
        {
            sourceFile = new StreamReader("source.txt");
        }

        /// <summary>
        /// Reads a line from file and splits it Reports read from file error, mismatched input fields
        /// </summary>
        /// <param name="lineStruct">The output line struct that contains the fields</param>
        /// <param name="testLine">Unit testing parameter</param>
        /// <returns>True if the location counter will be incremented, false otherwise</returns>
        public static bool tryReadLineFromSourceFile(out CodeLine lineStruct)
        {
            string line;

            // Check for end of file
            if (sourceFile.EndOfStream)
            {
                // This will occur after reading the end line
                lineStruct = new CodeLine(null, null, null);
                return false;
            }

            line = sourceFile.ReadLine().ToLower();
            Debug.WriteLine(line);

            lineNumber++;

            if (line.StartsWith("."))
            {
                // return the comment line
                lineStruct = new CodeLine(null, null, null, true, line);
                return false;
            }

            // Instruction line
            string temp = null, label = null, operationCode = null, operands = null, comment = null;
            var empty = String.Empty;

            try
            {
                // TODO Add variable names for numbers
                label = empty.Insert(0, line.Substring(0, 8)).TrimEnd();

                operationCode = empty.Insert(0, line.Substring(9, 8)).TrimEnd();

                temp = line.Substring(17);

                int indexOfSpace = temp.IndexOf(" ");

                if (operationCode == "byte" || operationCode == "resb" || operationCode == "resw" || operationCode == "word")
                {
                    // Assembler directrives cannot containt inline comments, so I use the operand field directly
                    operands = empty.Insert(0, line.Substring(17).TrimEnd());
                }
                // Space not found
                else if (indexOfSpace == -1)
                {
                    operands = temp;
                }
                else
                {
                    operands = empty.Insert(0, line.Substring(17, indexOfSpace - 17)).TrimEnd();
                    comment = empty.Insert(0, line.Substring(indexOfSpace, line.Length - indexOfSpace)).TrimEnd();
                }
            }
            catch (Exception ex)
            {
                // Write the line as it is to the list file

                // Splitting Error
                LogFile.logError($"file read error in input line number {lineNumber} Details:\n " + ex.Message);

                // Return false to indicate that the location counter won't be incremented
                lineStruct = new CodeLine(label, operationCode, operands, isComment: false, isError: true);
                return false;
            }

            lineStruct = new CodeLine(label, operationCode, operands, isComment: false);
            return true;
        }
    }
}