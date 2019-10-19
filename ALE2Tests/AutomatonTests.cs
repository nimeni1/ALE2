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
    public class AutomatonTests
    {
        Automaton dfaCheckAutomaton;
        Automaton relocationAutomaton;
        Automaton connectionDuplicatesAutomaton;

        public void getDfaCheckAutomaton()
        {
            this.dfaCheckAutomaton = new Automaton("ab");
            State stateZ = new State("Z", false);
            State stateA = new State("A", false);
            State stateB = new State("B", true);

            dfaCheckAutomaton.addState(stateZ);
            dfaCheckAutomaton.addState(stateA);
            dfaCheckAutomaton.addState(stateB);
            dfaCheckAutomaton.addFinal(stateB);

            Dictionary<string, object> connectionZA = new Dictionary<string, object>()
            {
                { "transitionLabel", "a"},
                { "endState", stateA }
            };
            stateZ.addConnection(connectionZA);
            Dictionary<string, object> connectionZZ = new Dictionary<string, object>()
            {
                {"transitionLabel", "b" },
                {"endState", stateZ }
            };
            stateZ.addConnection(connectionZZ);

            Dictionary<string, object> connectionAB = new Dictionary<string, object>()
            {
                {"transitionLabel", "a" },
                {"endState", stateB }
            };
            stateA.addConnection(connectionAB);
            Dictionary<string, object> connectionAZ = new Dictionary<string, object>()
            {
                {"transitionLabel", "b" },
                {"endState", stateZ }
            };
            stateA.addConnection(connectionAZ);

            Dictionary<string, object> connectionBB = new Dictionary<string, object>()
            {
                {"transitionLabel", "b" },
                {"endState", stateB }
            };
            stateB.addConnection(connectionBB);
            Dictionary<string, object> connectionBZ = new Dictionary<string, object>()
            {
                {"transitionLabel", "a" },
                {"endState", stateZ }
            };
            stateB.addConnection(connectionBZ);
        }

        public void getRelocationAutomaton()
        {
            relocationAutomaton = new Automaton("ab");
            State state1 = new State("1", false);
            State state2 = new State("2", false);
            State state3 = new State("3", true);

            relocationAutomaton.addState(state1);
            relocationAutomaton.addState(state2);
            relocationAutomaton.addState(state3);
            relocationAutomaton.addFinal(state3);

            Dictionary<string, object> connection12 = new Dictionary<string, object>()
            {
                { "transitionLabel", "a" },
                {"endState", state2 }
            };
            state1.addConnection(connection12);

            Dictionary<string, object> connection23 = new Dictionary<string, object>()
            {
                { "transitionLabel", "a"},
                { "endState", state3 }
            };
            state2.addConnection(connection23);

            List<SetState> setStates = new List<SetState>();
            SetState setState = new SetState("", false, new List<State>() { state2, state3 });
            setStates.Add(setState);
            relocationAutomaton.setSetStates(setStates);
        }

        public void getConnectionDuplicatesAutomaton()
        {
            connectionDuplicatesAutomaton = new Automaton("a");
            State state1 = new State("1", false);
            State state2 = new State("2", true);
            connectionDuplicatesAutomaton.addState(state1);
            connectionDuplicatesAutomaton.addState(state2);
            connectionDuplicatesAutomaton.addFinal(state2);

            Dictionary<string, object> connection12 = new Dictionary<string, object>()
            {
                { "transitionLabel", "a"},
                { "endState", state2}
            };
            state1.addConnection(connection12);

            Dictionary<string, object> connection12Duplicate = new Dictionary<string, object>()
            {
                { "transitionLabel", "a"},
                { "endState", state2}
            };
            state1.addConnection(connection12Duplicate);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this.getDfaCheckAutomaton();
            //this.getRelocationAutomaton();
            this.getConnectionDuplicatesAutomaton();
        }

        //[TestMethod()]
        //public void eliminateSilentTransitionsTest()
        //{
        //    Assert.Fail();
        //}

        [TestMethod()]
        public void removeStateConnectionDuplicatesTest()
        {
            List<State> states = connectionDuplicatesAutomaton.getStatesPool();
            Dictionary<string, object> expectedConnection = new Dictionary<string, object>()
            {
                { "transitionLabel", "a"},
                {"endState", states[1] }
            };
            connectionDuplicatesAutomaton.removeStateConnectionDuplicates();
            List<Dictionary<string, object>> actualConnections = states[0].getConnections();
            Assert.AreEqual(actualConnections.Count, 1);
            Assert.IsTrue(actualConnections[0]["transitionLabel"] == expectedConnection["transitionLabel"] &&
                actualConnections[0]["endState"] == expectedConnection["endState"]);
        }

        [TestMethod()]
        public void reallocateTransitionsTest()
        {
            List<State> states = relocationAutomaton.getStatesPool();
            State state1 = states[0];
            State state2 = states[1];
            List<SetState> setStates = new List<SetState>();
            foreach(State s in relocationAutomaton.getSetStates()){
                setStates.Add((SetState)s);
            }
            relocationAutomaton.reallocateTransitions();
            Assert.IsTrue(state1.hasConnection(setStates[0]) && state2.hasConnection(setStates[0]));
        }

        [TestMethod()]
        public void checkDfaTest()
        {
            bool expectedDfaFlag = true;
            dfaCheckAutomaton.checkDfa();
            bool actualDfaFlag = dfaCheckAutomaton.getDfaFlag();
            Assert.AreEqual(expectedDfaFlag, actualDfaFlag);
        }
    }
}