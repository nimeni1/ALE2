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
    public class StringCheckerTests
    {
        List<Dictionary<string, object>> checkerData;

        [TestInitialize]
        public void TestInitialize()
        {
            this.getCheckerData();
        }

        private void getCheckerData()
        {
            Automaton automaton1 = new Automaton("");
            for (int i=0; i< 7 ; i++)
            {
                bool finalFlag = false;
                int label = i + 1;
                State state = new State(label.ToString(), finalFlag);
                if (label == 7 || label == 5)
                {
                    finalFlag = true;
                    state.finalFlag = finalFlag;
                    automaton1.addFinal(state);
                }
                automaton1.addState(state);
            }
            string alphabet = "ab";
            automaton1.Alphabet = alphabet;
            List<State> states = automaton1.getStatesPool();
            Dictionary<string, object> connection12 = new Dictionary<string, object>()
            {
                {"transitionLabel", "a" },
                {"endState", states[1] }
            };
            states[0].addConnection(connection12);
            Dictionary<string, object> connection23 = new Dictionary<string, object>()
            {
                { "transitionLabel", "a"},
                {"endState", states[2] }
            };
            states[1].addConnection(connection23);
            Dictionary<string, object> connection34 = new Dictionary<string, object>()
            {
                { "transitionLabel", "a"},
                {"endState", states[3] }
            };
            states[2].addConnection(connection34);
            Dictionary<string, object> connection45 = new Dictionary<string, object>()
            {
                { "transitionLabel", "a"},
                {"endState", states[4] }
            };
            states[3].addConnection(connection45);
            Dictionary<string, object> connection56 = new Dictionary<string, object>()
            {
                { "transitionLabel", "b"},
                {"endState", states[5] }
            };
            states[4].addConnection(connection56);
            Dictionary<string, object> connection47 = new Dictionary<string, object>()
            {
                { "transitionLabel", "b"},
                {"endState", states[6] }
            };
            states[3].addConnection(connection47);

            Automaton automaton2 = new Automaton("");
            for (int i=0; i< 4; i++)
            {
                bool finalFlag = false;
                int label = i + 1;
                State state = new State(label.ToString(), finalFlag);
                if (label == 4)
                {
                    finalFlag = true;
                    state.finalFlag = finalFlag;
                    automaton2.addFinal(state);
                }
                automaton2.addState(state);
            }
            alphabet = "ab";
            automaton2.Alphabet = alphabet;
            List<State> states2 = automaton2.getStatesPool();

            Dictionary<string, object> connection01 = new Dictionary<string, object>()
            {
                {"transitionLabel", "a" },
                {"endState", states2[1] }
            };
            Dictionary<string, object> connection00 = new Dictionary<string, object>()
            {
                {"transitionLabel", "b" },
                {"endState", states2[0] }
            };
            Dictionary<string, object> connection02 = new Dictionary<string, object>()
            {
                {"transitionLabel", "a" },
                {"endState", states2[2] }
            };
            Dictionary<string, object> connection03 = new Dictionary<string, object>()
            {
                {"transitionLabel", "a" },
                {"endState", states2[3] }
            };
            states2[0].addConnection(connection01);
            states2[0].addConnection(connection00);
            states2[0].addConnection(connection02);
            states2[0].addConnection(connection03);

            Dictionary<string, object> connection212 = new Dictionary<string, object>()
            {
                { "transitionLabel", "b"},
                {"endState", states2[2] }
            };
            states2[1].addConnection(connection212);
            Dictionary<string, object> connection223 = new Dictionary<string, object>()
            {
                {"transitionLabel", "a" },
                {"endState", states2[3] }
            };
            states2[2].addConnection(connection223);

            Dictionary<string, object> data1Dfa = new Dictionary<string, object>()
            {
                { "automaton", automaton1},
                { "inputString", "aaaa" },
                { "expectedResult", true }
            };
            Dictionary<string, object> data2Dfa = new Dictionary<string, object>()
            {
                { "automaton", automaton1},
                { "inputString", "aaa" },
                { "expectedResult", false }
            };
            Dictionary<string, object> data3Dfa = new Dictionary<string, object>()
            {
                { "automaton", automaton1},
                { "inputString", "aaaab" },
                { "expectedResult", false }
            };
            Dictionary<string, object> data1Ndfa = new Dictionary<string, object>()
            {
                { "automaton", automaton2},
                { "inputString", "aba" },
                { "expectedResult", true }
            };
            Dictionary<string, object> data2Ndfa = new Dictionary<string, object>()
            {
                { "automaton", automaton2},
                { "inputString", "aaa" },
                { "expectedResult", false}
            };
            Dictionary<string, object> data3Ndfa = new Dictionary<string, object>()
            {
                { "automaton", automaton2},
                { "inputString", "abba" },
                { "expectedResult", false }
            };

            checkerData = new List<Dictionary<string, object>>()
            {
                data1Dfa, data2Dfa, data3Dfa, data1Ndfa, data2Ndfa, data3Ndfa
            };
        }

        [TestMethod()]
        public void checkStringTest()
        {
            foreach(Dictionary<string, object> data in checkerData)
            {
                Automaton automaton = (Automaton)data["automaton"];
                string inputString = (string)data["inputString"];
                bool expectedResult = (bool)data["expectedResult"];
                StringChecker stringChecker = new StringChecker(automaton, inputString);
                bool accepted = stringChecker.checkString();
                Console.WriteLine(accepted);
                Assert.AreEqual(expectedResult, accepted);
            }
        }
    }
}