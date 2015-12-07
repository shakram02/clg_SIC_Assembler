using System.IO;

namespace Assembler_SIC
{
    public static class HeaderRecord
    {
        /// <summary>
        /// Col 2-7 Program name
        /// </summary>
        public static string ProgramName;

        /// <summary>
        /// Col 8-13 program starting address in hexadecimal
        /// </summary>
        public static string StartingAddressHexa;

        /// <summary>
        /// Col 14 - 19 program length in hexadecimal
        /// </summary>
        public static string ProgramLengthLengthHexa;
    }

    public struct TextRecord
    {
        /// 1 T
        /// 2-7 Starting address of this text record
        /// 8-9 length of object code in this record ( hexa )
        /// 10-69 object code
    }

    public struct EndRecord
    {
        /// 1 E
        /// 2-7 Address of the first executable instruction in object program
    }

    public class ObjectCodeFile
    {
        public static StreamWriter objectFile = new StreamWriter("Object code.txt");

        public static void Close()
        {
            objectFile.Close();
        }

        public static void WriteRecord(string record)
        {
            objectFile.WriteLine(record);
        }
    }
}