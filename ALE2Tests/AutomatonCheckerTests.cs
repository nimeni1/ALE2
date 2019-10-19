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
    public class AutomatonCheckerTests
    {
        Automaton automaton1;
        Automaton automaton2;

        [TestInitialize]
        public void TestInitialize()
        {
            automaton1 = new Automaton("ab_");
            State state1 = new State("1", false);
            State state2 = new State("2", false);
            State state3 = new State("3", true);


            Dictionary<string, object> connection12 = new Dictionary<string, object>(){
                { "transitionLabel", "_" },
                { "endState", state2}
            };
            state1.addConnection(connection12);


            Dictionary<string, object> connection23 = new Dictionary<string, object>() {
                { "transitionLabel", "_"},
                { "endState", state3 }
            };

            Dictionary<string, object> connection22 = new Dictionary<string, object>() {
                { "transitionLabel", "_"},
                { "endState", state2 }
            };

            Dictionary<string, object> connection31 = new Dictionary<string, object>() {
                { "transitionLabel", "_"},
                { "endState", state1 }
            };

            state2.addConnection(connection23);
            state2.addConnection(connection22);
            state3.addConnection(connection31);

            automaton1.addState(state1);
            automaton1.addState(state2);
            automaton1.addState(state3);
            automaton1.addFinal(state3);

            automaton2 = new Automaton("");
            State s1 = new State("1", false);
            State s2 = new State("2", false);
            State s3 = new State("3", false);
            State s4 = new State("4", true);

            connection12 = new Dictionary<string, object>()
            {
                { "transitionLabel", "_"},
                {"endState", s2 }
            };

            Dictionary<string, object> connection13 = new Dictionary<string, object>(){
                { "transitionLabel", '_' },
                { "endState", s3 }
            };
            s1.addConnection(connection12);
            s1.addConnection(connection13);

            Dictionary<string, object> connection34 = new Dictionary<string, object>(){
                { "transitionLabel", "_"},
                { "endState", s4 }
            };

            s3.addConnection(connection34);

            Dictionary<string, object> connection41 = new Dictionary<string, object>(){
                { "transitionLabel", "a" },
                { "endState", s1 }
            };
            s4.addConnection(connection41);

            automaton2.addState(s1);
            automaton2.addState(s2);
            automaton2.addState(s3);
            automaton2.addState(s4);
            automaton2.addFinal(s4);
        }

        [TestMethod()]
        public void checkTest()
        {
            bool actualResult;
            AutomatonChecker automatonChecker = new AutomatonChecker(automaton1);
            automatonChecker.initializeCheck();
            actualResult = automatonChecker.check();
            Assert.AreEqual(actualResult, true);

            automatonChecker = new AutomatonChecker(automaton2);
            automatonChecker.initializeCheck();
            actualResult = automatonChecker.check();
            Assert.AreEqual(actualResult, false);
        }
    }
}