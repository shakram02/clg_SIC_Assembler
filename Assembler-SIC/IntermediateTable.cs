using System.Collections.Generic;

namespace Assembler_SIC
{
    public class IntermediateFileEntry
    {
        public string Label;

        /// <summary>
        /// Location counter value
        /// </summary>
        public int Address;

        /// <summary>
        /// Operand value
        /// </summary>
        public string Value;

        public string Operation;
        public string ObjectCode;

        public bool Equals(IntermediateFileEntry other)
        {
            return
                this.Label == other.Label
                    && this.Address == other.Address
                        && this.Value == other.Value ?
                        true : false;
        }

        public IntermediateFileEntry(string label, string operation, string value, int address, string objectCode = null)
        {
            this.Label = label;
            this.Address = address;
            this.Value = value;
            this.Operation = operation;
            this.ObjectCode = objectCode;
        }

        //public IntermediateFileEntry(string comment)
        //{
        //    this.Label = "." + comment;
        //}
    }

    public class IntermediateTable
    {
        public static List<IntermediateFileEntry> intermediateList = new List<IntermediateFileEntry>();

        public static void Insert(IntermediateFileEntry entry)
        {
            intermediateList.Add(entry);
        }

        public static bool Exists(string targetLabel, IntermediateFileEntry targetEntry)
        {
            // Find the target
            targetEntry = intermediateList.Find(op => op.Label == targetLabel);

            // Does the target exist ?
            return targetEntry != null ? true : false;
        }

        public static bool Exists(string targetLabel)
        {
            return intermediateList.Find(op => op.Label == targetLabel) == null ? true : false;
        }

        public void Flush()
        {
            // Create a pointer to the private list
            List<IntermediateFileEntry> list = intermediateList;
            LogFile.LogLine(list);
        }
    }
}