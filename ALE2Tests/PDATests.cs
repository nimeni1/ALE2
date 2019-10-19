using Microsoft.VisualStudio.TestTools.UnitTesting;
using ALE2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALE2.Tests
{
    [TestClass()]
    public class PDATests
    {
        PDAparser pdaParser;
        PDA pdaAutomaton;
        List<String> testInput;

        [TestInitialize]
        public void TestInitialize()
        {
            String emptyInput = "";
            String shortestAccepted = "b";
            String alsoAccepted = "a";
            String notAccepted = "ab";
            String alsoNotAccepted = "c";
            testInput = new List<string>();
            testInput.Add(emptyInput);
            testInput.Add(shortestAccepted);
            testInput.Add(alsoAccepted);
            testInput.Add(notAccepted);
            testInput.Add(alsoNotAccepted);

            String singleInputAccepted = "ae";
            String shortestFail = "e";
            String longestPatternFail = "aaaee";
            String longestPatternAccepted = "aaaaee";
            testInput.Add(singleInputAccepted);
            testInput.Add(shortestFail);
            testInput.Add(longestPatternFail);
            testInput.Add(longestPatternAccepted);
        }

        [TestMethod()]
        public void checkStringTest()
        {
            pdaParser = new PDAparser();
            pdaAutomaton = pdaParser.readFromFileForTest("../../PDA.txt");
            foreach(String s in this.testInput)
            {
                bool result = pdaAutomaton.checkString(s);
                switch (this.testInput.IndexOf(s))
                {
                    case 0:
                        Assert.AreEqual(false, result);
                        break;
                    case 1:
                        Assert.AreEqual(true, result);
                        break;
                    case 2:
                        Assert.AreEqual(true, result);
                        break;
                    case 3:
                        Assert.AreEqual(false, result);
                        break;
                    case 4:
                        Assert.AreEqual(false, result);
                        break;
                    default:
                        break;
                }
            }

            pdaAutomaton = pdaParser.readFromFileForTest("../../PDA2.txt");
            foreach (String s in this.testInput)
            {
                bool result = pdaAutomaton.checkString(s);
                switch (this.testInput.IndexOf(s))
                {
                    case 5:
                        Assert.AreEqual(true, result);
                        break;
                    case 6:
                        Assert.AreEqual(false, result);
                        break;
                    case 7:
                        Assert.AreEqual(false, result);
                        break;
                    case 8:
                        Assert.AreEqual(true, result);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}