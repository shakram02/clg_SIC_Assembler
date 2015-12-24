using System;
using System.Diagnostics;
using System.Globalization;
// New in C# 6
using static Assembler_SIC.LogFile;

namespace Assembler_SIC
{
    public static class LineParser
    {
        private static CodeLine line;
        public static int lineNumber = -1;
        private static int isStartLine = 0;
        private static int previousLocationCounter = 0;
        private static bool foundEnd = false;

        /// <summary>
        /// Parses the read line from the source file
        /// </summary>
        /// <param name="testLine"></param>
        /// <returns>False when the file has reached its end, true otherwise</returns>
        public static bool ParseLineFromSourceFile()
        {
            lineNumber++;
            if (SourceCodeFile.tryReadLineFromSourceFile(out Assembler.CurrentCodeLine))
            {
                isStartLine++;
                line = Assembler.CurrentCodeLine;

                // Verify fields, increment location counter

                if (verifyField(line.Label, true))
                {
                    // Add label to symtab if it's not whitespace
                    if (!String.IsNullOrWhiteSpace(line.Label))
                    {
                        if (SymTab.Exists(Assembler.CurrentCodeLine.Label) != true)
                        {
                            // Increment the location counter and inserts to intermediate file and checks the operands
                            if (updateLocationCounter())
                            {
                                // Write the entry in debug window
                                Debug.WriteLine("Inserted to symbol table: " + line.Label + "  " + Assembler.LocationCounter);
                                SymTab.Insert(line.Label, previousLocationCounter);
                                insertToIntermediateFile();
                            }
                        }
                        else
                        {
                            LogError("duplicate label definition");
                        }
                    }
                    else
                    {
                        // Label is empty, update the location counter directly
                        if (updateLocationCounter())
                        {
                            insertToIntermediateFile();
                        }
                    }
                }
                else
                {
                    // Can't assign this label, write to listing file, error in label format
                    LogError($"{line.Label} {line.Operation} {line.Operands} can't assign this label");
                }
            }
            else
            {
                // End of file
                if (Assembler.CurrentCodeLine.Equals(new CodeLine(null, null, null)))
                {
                    return false;
                }
                else if (Assembler.CurrentCodeLine.isComment)
                {
                    // Write the comment to the log file
                    LogLine(lineNumber, Assembler.CurrentCodeLine.Comment);
                }
            }

            // Continue reading the file
            return true;
        }

        /// <summary>
        /// Inserts parsed lines to intermediate table and writes list file line
        /// </summary>
        private static void insertToIntermediateFile()
        {
            // Store the instruction with the old value of the location counter
            if (line.Operation == "start")
            {
                IntermediateTable.Insert(new IntermediateFileEntry(line.Label, line.Operation, value: line.Operands, address: Assembler.StartLocation));
            }
            else
            {
                IntermediateTable.Insert(new IntermediateFileEntry(line.Label, line.Operation, value: line.Operands, address: previousLocationCounter));
            }
            // Prints the code file
            LogLine(line.Label, line.Operation, line.Operands);
        }

        /// <summary>
        /// Finds the instruction from the optable or in assembler directives and updates the
        /// location counter accordinly
        /// </summary>
        /// <returns>True if the instruction will be inserted to intermediate file for object code creation,
        ///  false if the line had an error</returns>
        private static bool updateLocationCounter()
        {
            previousLocationCounter = Assembler.LocationCounter;
            if (isStartLine == 1 && line.Operation == "start")
            {
                if (String.IsNullOrEmpty(line.Label))
                {
                    LogError("Empty program name");
                    return false;
                }
                // Initialize the location counter using the TryParse, if it fails intialize it to 0
                if (int.TryParse(line.Operands, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out Assembler.LocationCounter))
                {
                    // Set the program start location to the operand
                    Assembler.StartLocation = Assembler.LocationCounter;
                    previousLocationCounter = Assembler.LocationCounter;
                }
                else
                {
                    // Initialize the location counter using the TryParse, if it fails intialize it to 0
                    Assembler.LocationCounter = 0;

                    // Store the current line with the last advanced location counter
                    previousLocationCounter = Assembler.LocationCounter;

                    LogError("Didn't find starting address, initializing location counter to 0");

                    // Save the starting address as the current address
                    Assembler.StartLocation = Assembler.LocationCounter;
                }
                return true;
            }
            // Start operation is not on the first line
            else if ((isStartLine < 1 || isStartLine == 0) && line.Operation == "start")
            {
                LogError("Misplaced start statement");
                return false;
            }
            else if (line.Operation == "end" && !foundEnd)
            {
                Assembler.EndLocation = Assembler.LocationCounter;
                Assembler.ProgramLength = Assembler.EndLocation - Assembler.StartLocation;
                foundEnd = true;
                return true;
            }
            else if (line.Operation == "end" && foundEnd)
            {
                LogError("Misplaced end statement");
                return false;
            }

            // Find the operation in the optab
            if (OpTab.Exists(line.Operation))
            {
                // TODO Deal with literals
                if (line.Operands.StartsWith("="))
                {
                    // Add literal to table
                    try
                    {
                        new LitTabEntry(line.Operands, null);
                    }
                    catch (InvalidOperationException exc)
                    {
                        LogError("Inalid literal:" + exc.Message);
                        return false;
                    }

                }
                // then add 3(instruction length) to LOCCTR
                Assembler.LocationCounter += 3;
                return true;
            }
            else
            {
                // Assembler directives

                // if OPCODE = ‘WORD’ then add 3 to LOCCTR
                if (line.Operation == "word")
                {
                    if (verifyField(line.Label, canBeEmpty: false))
                    {
                        // Store the word with the label in the symbol table+
                        Assembler.LocationCounter += 3;

                        // Insert to intermediate file
                        return true;
                    }
                    else
                    {
                        LogError($">{line.Label}< illegal label in word statement");
                        return false;
                    }
                }

                //else if OPCODE = ‘RESW’
                //  then add 3 * #[OPERAND] to LOCCTR
                else if (line.Operation == "resw")
                {
                    // Check if the label is empty
                    if (verifyField(line.Label, canBeEmpty: false))
                    {
                        // Get the value of the operand
                        int numberOfWordsToReserve;

                        if (int.TryParse(line.Operands, out numberOfWordsToReserve))
                        {
                            // Multiply the word length by 3 and add it to the location counter
                            Assembler.LocationCounter += (3 * numberOfWordsToReserve);

                            // Insert to intermediate file
                            return true;
                        }
                        else
                        {
                            LogError($">{line.Operands}< illegal operand in resw statement");
                            return false;
                        }
                    }
                    else
                    {
                        LogError($">{line.Label}< illegal label in resw statement");
                        return false;
                    }
                }
                else if (line.Operation == "resb")
                {
                    // else if OPCODE = ‘RESB’
                    // then add #[OPERAND] to LOCCTR

                    if (verifyField(line.Label, canBeEmpty: false))
                    {
                        // Get operand length
                        // Increment location counter
                        // Get the value of the operand
                        int numberOfBytesToReserve;

                        if (int.TryParse(line.Operands, out numberOfBytesToReserve))
                        {
                            // Store the word with the label in the symbol table
                            SymTab.Insert(line.Label, previousLocationCounter);

                            // Multiply the word length by 1 * number of bytes, add it to the location counter
                            Assembler.LocationCounter += numberOfBytesToReserve;

                            // Insert to intermediate file
                            return true;
                        }
                        else
                        {
                            LogError($">{line.Operands}< illegal operand in resb statement");
                            return false;
                        }
                    }
                    else
                    {
                        LogError($">{line.Label}< illegal label in resb statement");
                        return false;
                    }
                }
                else if (line.Operation == "byte")
                {
                    // else if OPCODE = ‘BYTE’
                    // find length of constant in bytes add length to LOCCTR
                    if (verifyField(line.Label, canBeEmpty: false))
                    {
                        // Get the operand without the leading and trailing characters
                        string temp = line.Operands.Remove(0, 2);
                        temp = temp.Remove(temp.Length - 1);

                        if (line.Operands.StartsWith("c'") && line.Operands.EndsWith("'"))
                        {
                            line.Operands = temp;
                            // Increment the location counter by one byte for each character
                            Assembler.LocationCounter += temp.Length;

                            // Insert to intermediate file
                            return true;
                        }
                        else if (line.Operands.StartsWith("x'") && line.Operands.EndsWith("'"))
                        {
                            // Get the operand without the leading and trailing characters
                            line.Operands = temp;

                            if (temp.Length % 2 != 0)
                            {
                                LogError($">{line.Operands}< Odd length hex string in byte statement");
                                return false;
                            }

                            // Convert the number to hexadecimal value and put it as the operand
                            int result = int.Parse(temp, NumberStyles.HexNumber);
                            line.Operands = result.ToString();

                            // The number of bytes are the length of the hexstring divided by 2
                            Assembler.LocationCounter += temp.Length / 2;

                            // Insert to intermediate file
                            return true;
                        }
                        else
                        {
                            LogError($">{line.Operands}< illegal opearnd in byte statement");
                            return false;
                        }
                    }

                    else
                    {
                        LogError($">{line.Label}< illegal label in byte statement");
                        return false;
                    }
                }
                else if (line.Operation == "ltorg")
                {
                    // Flush littab into intermediate file
                    foreach (var item in Assembler.LitTab)
                    {
                        // Skip items already added
                        if (item.Value.Inserted)
                            continue;

                        // Set the address of the litteral table entry
                        item.Value.Address = previousLocationCounter;

                        // Insert items to the code
                        IntermediateTable.Insert(
                            new IntermediateFileEntry(
                                label: "*",
                            operation: item.Value.Name,
                            value: item.Value.Value,
                            address: previousLocationCounter, objectCode: item.Value.Value)
                            );

                        // Mark the item to avoid adding it twice to the table
                        item.Value.Inserted = true;
                        // Update location counter for the next variable
                        previousLocationCounter += item.Value.Size;
                    }

                    // Update location counter manually
                    Assembler.LocationCounter = previousLocationCounter;

                    // Don't insert to intermediate file
                    return false;
                }
                else
                {
                    LogError($">{line.Operation}< Unindentified operation");
                    return false;
                }
            }
        }

        /// <summary>
        /// This method throws an exception in case it detects an error, exception should be
        /// handeled in the calling function and written to the listfile
        /// </summary>
        /// <param name="Field"></param>
        /// <returns>True if the label was verified, else if the label contains errors</returns>
        private static bool verifyField(string Field, bool canBeEmpty = true)
        {
            // Errors to check:
            /*
            * white space in label
            * label defined twice
            * label starts with a number
            */
            if (String.IsNullOrEmpty(Field) && canBeEmpty)
            {
                return true;
            }
            else if (String.IsNullOrEmpty(Field) && !canBeEmpty)
            {
                // Label is empty when it shouldn't be
                return false;
            }
            else if (Field.StartsWith(" ") || Field.Contains(" "))
            {
                LogError("Label can't start with a whitespace");
                return false;
            }
            else if (Char.IsNumber(Field[0]))
            {
                LogError("Label can't start with a number");
                return false;
            }
            return true;
        }
    }
}