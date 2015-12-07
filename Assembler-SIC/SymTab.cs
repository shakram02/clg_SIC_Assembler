using System.Collections.Generic;

namespace Assembler_SIC
{
    /// <summary>
    /// This class contains register and labels as paird of string, int
    /// </summary>
    public static class SymTab
    {
        private static Dictionary<string, int> _symTab = new Dictionary<string, int>();

        static SymTab()
        {
            InitializeSymTab();
        }

        private static void InitializeSymTab()
        {
            // Add the registers
            _symTab.Add("a", 0);
            _symTab.Add("x", 1);
            _symTab.Add("l", 2);
            _symTab.Add("b", 3);
            _symTab.Add("s", 4);
            _symTab.Add("t", 5);
        }

        /// <summary>
        /// Checks if the target symbol exists in the symbol table
        /// </summary>
        /// <param name="TargetSymbol">Target symbol to be found</param>
        /// <returns></returns>
        public static bool Exists(string TargetSymbol)
        {
            return _symTab.ContainsKey(TargetSymbol);
        }

        public static void Insert(string mnemonic, int address)
        {
            _symTab.Add(mnemonic, address);
        }

        public static void Flush()
        {
            LogFile.Write("Logging symtab");
            foreach (KeyValuePair<string, int> item in _symTab)
            {
                LogFile.Write($"{item.Key} , {item.Value}");
            }
        }
    }
}