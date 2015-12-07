using Assembler_SIC;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestIntermediateTableCreation()
        {
            bool t;
            IntermediateTable table = new IntermediateTable();

            while (t = LineParser.ParseLineFromSourceFile()) { }
            ObjectCodeGenerator.GenerateObjectRecords();
            table.Flush();
            System.Convert.ToInt64("0x005", 2);
            SymTab.Flush();

            LogFile.CloseLog();
        }
    }
}