using System;
using System.Collections.Generic;

namespace Assembler_SIC
{
    public static class ObjectCodeGenerator
    {
        private static List<IntermediateFileEntry> table = IntermediateTable.intermediateList;

        public static void GenerateObjectRecords()
        {
            assembleInstructions();
            // Write the total object program length in the header record
            writeHeaderRecord();
            writeTextRecord();
            writeEndRecord();
            ObjectCodeFile.Close();
        }

        private static void assembleInstructions()
        {
            foreach (IntermediateFileEntry line in table)
            {
                string objCode = "";
                MachineOperation currentOperation;

                // The target address is calculated only when the operation is not an assembler directive
                if ((currentOperation = OpTab.Get(line.Operation)) != null)
                {
                    // Operation found
                    objCode += $"{currentOperation.OpCode.ToString("X")}";

                    // Calculate the address
                    if (line.Value.EndsWith(",x"))
                    {
                        // Set the indexed mode bit
                        line.Address |= (1 << 15);
                    }
                    else
                    {
                        // TODO Get the address of the operand and convert it to hexa, then add it to the object code
                        if (line.Value.StartsWith("="))
                        {
                            // TODO get the address from the literal table
                        }
                        else
                        {
                            objCode += SymTab.GetAdress(line.Value).ToString("X");
                        }
                    }
                    // Convert the address to hexadecimal, ERROR address shall not be added in object code
                    //objCode += line.Address.ToString("X");

                    // Store the object code in its place in the intermediate file
                    line.ObjectCode = objCode;
                }
                // Check the operations
                else
                    switch (line.Operation)
                    {
                        case "byte":
                            int decVal;
                            if (int.TryParse(line.Value, out decVal))
                            {
                                // Operand is a number, convert it to hex
                                line.ObjectCode = decVal.ToString("X");
                                break;
                            }
                            else
                            {
                                // Operand is a text string
                                char[] tempCharArray = line.Value.ToCharArray();
                                foreach (char item in tempCharArray)
                                {
                                    // Convert ascii value to hexadeciaml value
                                    line.ObjectCode += ((int)item).ToString("X");
                                }
                                break;
                            }
                        case "word":
                            // Store the hex string of the word
                            line.ObjectCode = int.Parse(line.Value).ToString("X");
                            break;

                        case "resb":
                        case "resw":
                        case "start":
                        case "end":
                            line.ObjectCode = "";
                            break;

                        default:
                            line.ObjectCode = "??";
                            break;
                    }
            }
        }

        private static void writeHeaderRecord()
        {
            string headerRecord = "";

            // Get the program name
            if (String.IsNullOrEmpty(table[0].Label) == false)
            {
                headerRecord += $"{table[0].Label,8}";
            }
            else
            {
                LogFile.logError("Empty program name");
            }
            headerRecord += Assembler.ProgramLength.ToString("X");

            int objectProgramLength = 0;
            foreach (IntermediateFileEntry item in IntermediateTable.intermediateList)
            {
                if (item != null)
                    objectProgramLength += item.ObjectCode.Length;
            }
            // Write the object code lentgh in Hexa
            int objProgramLength = 0;
            foreach (IntermediateFileEntry line in table)
            {
                objProgramLength += line.ObjectCode.Length;
            }
            headerRecord += objProgramLength.ToString("X");

            ObjectCodeFile.WriteRecord("H" + headerRecord.Trim());
        }

        private static void writeTextRecord()
        {
            int limit = table.Count;

            for (int i = 0; i < limit; i++)
            {
                int recordObjectCodeLength = 0;
                string textRecord = "";

                for (; recordObjectCodeLength < 60 && i < limit; i++)
                {
                    IntermediateFileEntry currentEntry = table[i];

                    if (recordObjectCodeLength + currentEntry.ObjectCode.Length > 60)
                    {
                        // End the small loop and make a new text record
                        break;
                    }
                    else
                    {
                        recordObjectCodeLength += currentEntry.ObjectCode.Length;

                        // Append the object code
                        textRecord += currentEntry.ObjectCode;
                    }
                }

                // Write the text record to the file
                ObjectCodeFile.WriteRecord("T" + textRecord);
            }
        }

        private static void writeEndRecord()
        {
            ObjectCodeFile.WriteRecord($"E{Assembler.StartLocation.ToString("X")}");
        }
    }
}