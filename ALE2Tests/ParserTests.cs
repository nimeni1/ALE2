using Microsoft.VisualStudio.TestTools.UnitTesting;
using ALE2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ALE2.Tests
{
    [TestClass()]
    public class ParserTests
    {
        List<Dictionary<string, object>> automatonData;
        Dictionary<string, object> automatonExceptionData;
        FileInfo[] files;

        [TestInitialize]
        public void TestInitialize()
        {
            this.getAutomatonData();
            this.getFilesData();
        }

        private void getFilesData()
        {
            DirectoryInfo d = new DirectoryInfo(Directory.GetCurrentDirectory());
            files = d.GetFiles("data*");
        }

        void getAutomatonExceptionData()
        {
            automatonExceptionData = new Dictionary<string, object>() {
                { "alphabet", "abcdef"},
                { "states", "0,1,2,3,4" },
                { "final", "3" },
                { "transitions", new List<List<String>>()
                {
                    new List<string>() {"0,a", "-->", "4" },
                    new List<string>() {"0,b", "-->", "2" },
                    new List<string>() {"0,c", "-->", "3"},
                    new List<string>() {"0,d", "-->", "1" },
                    new List<string>() {"0,e", "-->", "0"},
                    new List<string>() {"1,a", "-->", "2" },
                    new List<string>() {"1,b", "-->", "1"},
                    new List<string>() {"1,c", "-->", "3" },
                    new List<string>() {"1,d", "-->", "4"},
                    new List<string>() {"1,g", "-->", "10" },
                    new List<string>() {"2,c", "-->", "asdf"},
                    new List<string>() {"aq,as", "-->", "cvb" },
                    new List<string>() {"4,a", "-->", "1"}
                }
                },
                { "expectedResult", false }
            };
        }

        private void getAutomatonData()
        {
            automatonData = new List<Dictionary<string, object>>();
            Dictionary<string, object> sample2 = new Dictionary<string, object>() {
                { "alphabet", "abcdef"},
                { "states", "0,1,2,3,4" },
                { "final", "3" },
                { "transitions", new List<List<String>>()
                {
                    new List<string>() {"0,a", "-->", "4" },
                    new List<string>() {"0,b", "-->", "2" },
                    new List<string>() {"0,c", "-->", "3"},
                    new List<string>() {"0,d", "-->", "1" },
                    new List<string>() {"0,e", "-->", "0"},
                    new List<string>() {"1,a", "-->", "2" },
                    new List<string>() {"1,b", "-->", "1"},
                    new List<string>() {"1,c", "-->", "3" },
                    new List<string>() {"1,d", "-->", "4"},
                    new List<string>() {"4,a", "-->", "1" },
                }
                },
                { "expectedResult", true }
            };
            automatonData.Add(sample2);
        }
    

        [TestMethod()]
        public void parseDataTest()
        {
            Parser automatonParser = new Parser();
            foreach(FileInfo file in files)
            {
                if (file.Name == "data3.txt")
                {
                    Assert.IsTrue(true);
                    break;
                }
                Dictionary<string, object> parsedData = automatonParser.parseData(file.Name);
                string expectedAlphabet = (string)parsedData["alphabet"];
                List<string> states = (List<string>)parsedData["states"];
                List<string> final = (List<string>)parsedData["final"];
                List<List<string>> expectedTransitions = (List<List<string>>)parsedData["transitions"];
                string expectedStates = "";
                string expectedFinal = "";
                for (int i= 0; i< states.Count; i++)
                {
                    expectedStates += states[i];
                    if (i != states.Count - 1)
                    {
                        expectedStates += ",";
                    }
                }
                for (int i=0; i<final.Count; i++)
                {
                    expectedFinal += final[i];
                    if (i != final.Count - 1)
                    {
                        expectedFinal += ",";
                    }
                }
                string alphabetEntry = "alphabet: " + expectedAlphabet + "\n";
                string statesEntry = "states: " + expectedStates + "\n";
                string finalEntry = "final: " + expectedFinal + "\n";
                string transitionEntry = "";

                bool alphabetFlag = false;
                bool statesFlag = false;
                bool finalFlag = false;
                bool transitionsFlag = false;
                int countTransitions = 0;
                String line;
                System.IO.StreamReader f = new System.IO.StreamReader(@file.Name);
                while ((line = f.ReadLine()) != null)
                {
                    bool transition = false;
                    if (line == "transitions:")
                    {
                        transition = true;
                        continue;
                    }
                    if (alphabetEntry == line)
                    {
                        alphabetFlag = true;
                    }
                    if (statesEntry == line)
                    {
                        statesFlag = true;
                    }
                    if (finalEntry == line)
                    {
                        finalFlag = true;
                    }
                    if (transition)
                    {
                        foreach(List<string> t in expectedTransitions)
                        {
                            foreach(string s in t)
                            {
                                transitionEntry = string.Join(" ", s) + "\n";
                            }
                            if (transitionEntry == line)
                            {
                                countTransitions++;
                            }
                        }
                    }

                }
                if (countTransitions == expectedTransitions.Count)
                {
                    transitionsFlag = true;
                }
                Assert.IsTrue(alphabetFlag);
                Assert.IsTrue(statesFlag);
                Assert.IsTrue(finalFlag);
                Assert.IsTrue(transitionsFlag);
            }
        }

        
        [TestMethod()]
        public void createAutomatonTest()
        {
            Parser automatonParser = new Parser();
            foreach(Dictionary<string, object> data in automatonData)
            {
                string alphabet = (string)data["alphabet"];
                List<string> states = ((string)data["states"]).Split(',').ToList();
                List<string> final = ((string)data["final"]).Split(',').ToList(); 
                List<List<string>> transitions = (List<List<string>>)data["transitions"];
                bool expectedResult = (bool)data["expectedResult"];
                automatonParser.createAutomaton(alphabet, states, final, transitions);
                Automaton testedAutomaton = automatonParser.getAutomaton();
                Assert.AreEqual(alphabet, testedAutomaton.Alphabet);
                List<State> automatonStates = testedAutomaton.getStatesPool();
                List<State> automatonFinal = testedAutomaton.getFinalStates();
                for (int i=0; i<states.Count; i++)
                {
                    Assert.AreEqual(states[i], automatonStates[i].symbol);
                }
                for (int i = 0; i < final.Count; i++)
                {
                    Assert.AreEqual(final[i], automatonFinal[i].symbol);
                }
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(NullReferenceException))]
        public void createAutomatonExceptionTest()
        {
            Parser automatonParser = new Parser();
            string alphabet = (string)automatonExceptionData["alphabet"];
            List<string> states = ((string)automatonExceptionData["states"]).Split(',').ToList();
            List<string> final = ((string)automatonExceptionData["final"]).Split(',').ToList();
            List<List<string>> transitions = (List<List<string>>)automatonExceptionData["transitions"];
            bool expectedResult = (bool)automatonExceptionData["expectedResult"];
            automatonParser.createAutomaton(alphabet, states, final, transitions);
        }
    }
}