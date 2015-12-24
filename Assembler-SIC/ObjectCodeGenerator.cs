using System;
using System.Collections.Generic;
using System.Text;

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
                StringBuilder objCode = new StringBuilder();
                MachineOperation currentOperation;

                // The target address is calculated only when the operation is not an assembler directive
                if ((currentOperation = OpTab.Get(line.Operation)) != null)
                {
                    // Calculate the address
                    if (line.Value.EndsWith(",x"))
                    {
                        // Set the indexed mode bit, 
                        int temp = currentOperation.OpCode | 1 << 15;

                        objCode.Append(temp.ToString("X"));
                    }
                    else
                    {
                        objCode.Append($"{currentOperation.OpCode.ToString("X")}");

                        // Get the address of the operand and convert it to hexa, then add it to the object code
                        if (line.Value.StartsWith("="))
                        {
                            // TODO get the address from the literal table
                            objCode.Append(
                                ((int)Assembler.LitTab[line.Value].Address).ToString("X")
                                );
                        }
                        else
                        {
                            objCode.Append(SymTab.GetAdress(line.Value).ToString("X"));
                        }
                    }
                    // Convert the address to hexadecimal, ERROR address shall not be added in object code
                    //objCode += line.Address.ToString("X");

                    // Store the object code in its place in the intermediate file
                    line.ObjectCode = objCode.ToString();
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
                                StringBuilder objectCodeBuilder = new StringBuilder();
                                foreach (char item in tempCharArray)
                                {
                                    // Convert ascii value to hexadeciaml value
                                    objectCodeBuilder.Append(((int)item).ToString("X"));
                                }
                                line.ObjectCode = objectCodeBuilder.ToString();
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
            StringBuilder headerRecord = new StringBuilder();

            // Get the program name
            if (String.IsNullOrEmpty(table[0].Label) == false)
            {
                headerRecord.Append($"{table[0].Label,8}");
            }
            else
            {
                LogFile.LogError("Empty program name");
            }
            headerRecord.Append(Assembler.ProgramLength.ToString("X"));

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
            headerRecord.Append(objProgramLength.ToString("X"));

            ObjectCodeFile.WriteRecord("H" + headerRecord.ToString().Trim());
        }

        private static void writeTextRecord()
        {
            int limit = table.Count;

            for (int i = 0; i < limit; i++)
            {
                int recordObjectCodeLength = 0;
                StringBuilder textRecord = new StringBuilder();

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
                        textRecord.Append(currentEntry.ObjectCode);
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