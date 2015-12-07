using System;
using Assembler_SIC;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestLineParserClass
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            
            LineParser.ParseLineFromSourceFile();
            Assert.AreEqual("true", "true");
        }
    }
}
