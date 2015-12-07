using System.Collections.Generic;

namespace Assembler_SIC
{
    public class MachineOperation
    {
        public string Mnemonic;
        public int OpCode;
        public int Format;

        public MachineOperation(string Mnemonic, int OpCode, int Format)
        {
            this.Mnemonic = Mnemonic;
            this.OpCode = OpCode;
            this.Format = Format;
        }
    }

    public static class OpTab
    {
        private static List<MachineOperation> OpTable = new List<MachineOperation>();

        static OpTab()
        {
            InitializeOpTable();
        }

        /// <summary>
        /// Adds the instructions to the op-table
        /// </summary>
        private static void InitializeOpTable()
        {
            #region Add items to OpTab

            OpTable.AddRange(new List<MachineOperation>{
                new MachineOperation("add", 24, '3'),

                new MachineOperation("and", 64, '3'),

                new MachineOperation("comp", 40, '3'),

                new MachineOperation("div", 36, '3'),

                new MachineOperation("j", 60, '3'),

                new MachineOperation("jeq", 48, '3'),

                new MachineOperation("jgt", 52, '3'),

                new MachineOperation("jlt", 56, '3'),

                new MachineOperation("jsub", 72, '3'),

                new MachineOperation("lda", 0, '3'),

                new MachineOperation("ldb", 104, '3'),

                new MachineOperation("ldch", 80, '3'),

                new MachineOperation("ldl", 8, '3'),

                new MachineOperation("lds", 108, '3'),

                new MachineOperation("ldt", 116, '3'),

                new MachineOperation("ldx", 4, '3'),

                new MachineOperation("mul", 32, '3'),

                new MachineOperation("or", 68, '3'),

                new MachineOperation("rd", 216, '3'),

                new MachineOperation("rsub", 76, '3'),

                new MachineOperation("sta", 12, '3'),

                new MachineOperation("stb", 120, '3'),

                new MachineOperation("stch", 84, '3'),

                new MachineOperation("stl", 20, '3'),

                new MachineOperation("sts", 124, '3'),

                new MachineOperation("stt", 132, '3'),

                new MachineOperation("stx", 16, '3'),

                new MachineOperation("sub", 28, '3'),

                new MachineOperation("td", 224, '3'),

                new MachineOperation("tix", 44, '3'),

                new MachineOperation("wd", 220, '3'),
            });

            #endregion Add items to OpTab
        }

        /// <summary>
        /// This method is used to get machine operations during the assembly process
        /// </summary>
        /// <param name="Mnemonic">Target mnemonic</param>
        /// <returns>The target machine operation</returns>
        public static MachineOperation Get(string Mnemonic)
        {
            // Find the target
            return OpTable.Find(op => op.Mnemonic == Mnemonic);
        }

        public static bool Exists(string Mnemonic)
        {
            // Found something in op tab ? return true
            return OpTable.Find(op => op.Mnemonic == Mnemonic) != null ? true : false;
        }
    }
}