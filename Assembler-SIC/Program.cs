namespace Assembler_SIC
{
    public partial class Program
    {
        private static void Main(string[] args)
        {
            IntermediateTable intermediateFile = new IntermediateTable();
        }

        //        {
        //            // Find the source code file
        //            if (File.Exists("source.txt"))
        //                SourceCodeFile.FileReader = new StreamReader("source.txt");
        //            else
        //            {
        //                takeUserSourceFileInput();
        //            }

        //            // Pass 1
        //            Assembler.OpenSourceFile();

        //            // Read the source file
        //            while (LineParser.ParseLineFromSourceFile())
        //            {
        //                // Do stuff
        //            }
        //        }

        //        /// <summary>
        //        /// Ask the user for a source code file
        //        /// </summary>
        //        private static void takeUserSourceFileInput()
        //        {
        //            Console.WriteLine("Enter the source code path");
        //            bool repeatInput = false;

        //            do
        //            {
        //                // Read user input and check if this file exists
        //                if (!File.Exists(SourceCodeFile.FileReader = Console.ReadLine()))
        //                {
        //                    // The file doesn't exist

        //                    Console.WriteLine("The source file is not found, please re-enter it");

        //                    // Repeat the loop
        //                    repeatInput = true;
        //                }
        //                else
        //                {
        //                    Console.WriteLine("Source code file found");
        //                    repeatInput = false;
        //                }
        //            }
        //            while (repeatInput);
        //        }
    }
}