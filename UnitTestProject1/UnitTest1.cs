using Assembler_SIC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ciloci.Flee.CalcEngine;
using System.Diagnostics;
using Ciloci.Flee;

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
            //System.Convert.ToInt64("0x005", 2);
            SymTab.Flush();
            LogFile.CloseLog();
        }
        [TestMethod]
        public void TestStringLiterals()
        {
            string lit0 = "=C'ddf'";
            new LitTabEntry(lit0, null);

            LitTabEntry entry1 = Assembler.LitTab[lit0];
            Debug.WriteLine($"Name:{entry1.Name}, Value: {entry1.Value}, Size:{entry1.Size}");
            Assert.AreEqual("=C'ddf'", entry1.Name);
            Assert.AreEqual(3, entry1.Size);

            string lit1 = "=c'3ddF'";
            new LitTabEntry(lit1, null);

            LitTabEntry entry2 = Assembler.LitTab[lit1];
            Debug.WriteLine($"Name:{entry2.Name}, Value: {entry2.Value}, Size:{entry2.Size}");
            Assert.AreEqual("=c'3ddF'", entry2.Name);
            Assert.AreEqual("33646446", entry2.Value);
            Assert.AreEqual(4, entry2.Size);
        }

        [TestMethod]
        public void TestHexLiterals()
        {
            string lit1 = "=X'33f'";
            new LitTabEntry(lit1, null);

            LitTabEntry entry2 = Assembler.LitTab[lit1];
            Assert.AreEqual("33F", int.Parse(entry2.Value).ToString("X"));
            //Assert.AreEqual(3, entry1.Size);
        }

        [TestMethod]
        public void TestFlee()
        {

            Ciloci.Flee.ExpressionContext exp = new Ciloci.Flee.ExpressionContext();
            IDynamicExpression e = exp.CompileDynamic("1+2");
            Assert.AreEqual(3, e.Evaluate());


        }
    }
}