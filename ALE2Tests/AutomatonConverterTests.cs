using Microsoft.VisualStudio.TestTools.UnitTesting;
using ALE2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ALE2.Tests
{
    [TestClass()]
    public class AutomatonConverterTests
    {
        Dictionary<string, object> automatonData;
        Dictionary<string, object> processedStatesData;
        Dictionary<string, object> rellocationData;

        public void getAutomatonData()
        {
            Automaton automaton = new Automaton("ab_");
            State s1 = new State("1", false);
            State s2 = new State("2", false);
            State s3 = new State("3", true);
            automaton.addState(s1);
            automaton.addState(s2);
            automaton.addState(s3);
            automaton.addFinal(s3);

            Dictionary<string, object> connection12 = new Dictionary<string, object>()
            {
                { "transitionLabel", "a" },
                { "endState", s2 }
            };
            Dictionary<string, object> connection13 = new Dictionary<string, object>()
            {
                { "transitionLabel", "a" },
                { "endState", s3 }
            };
            s1.addConnection(connection12);
            s1.addConnection(connection13);

            SetState setState = new SetState("", false, new List<State> { s2, s3 });
            List<State> expectedStates = new List<State>() { s1, setState };
            automatonData = new Dictionary<string, object>()
            {
                {"automaton", automaton },
                { "expectedStates", expectedStates}
            };
        }

        public void getProcessedStatesData()
        {
            State state1 = new State("1", false);
            State state2 = new State("2", false);
            State state3 = new State("3", true);
            Dictionary<string, object> connection13 = new Dictionary<string, object>()
            {
                { "transitionLabel", "a" },
                { "endState", state3 }
            };
            state1.addConnection(connection13);
            Dictionary<string, object> connection23 = new Dictionary<string, object>()
            {
                { "transitionLabel", "a" },
                { "endState", state3 }
            };
            state2.addConnection(connection23);
            SetState state12 = new SetState("", false, new List<State>() { state1, state2 });
            List<State> processedStates = new List<State>() { state12, state2, state3 };
            processedStatesData = new Dictionary<string, object>()
            {
                { "processedStates", processedStates },
                { "expectedOptimizedStates", new List<State>{state12, state3} }
            };
            //List<Dictionary<string,object>> processedStatesList = new List<Dictionary<string, object>>();
            //processedStatesList.Add(processedStatesEntry);
            
        }

        public void getRellocationData()
        {
            Automaton automaton = new Automaton("");
            State state1 = new State("1", false);
            State state2 = new State("2", false);
            State state3 = new State("3", false);
            State state4 = new State("4", true);
            Dictionary<string, object> connection13 = new Dictionary<string, object>()
            {
                { "transitionLabel", "a" },
                { "endState", state3 }
            };
            state1.addConnection(connection13);
            SetState state12 = new SetState("", false, new List<State>() { state1, state2 });
            SetState state34 = new SetState("", false, new List<State>() { state3, state4 });
            List<State> optimizedStates = new List<State>() { state12, state34 };
            automaton.clearStates();
            automaton.addState(state12);
            automaton.addState(state34);
            automaton.setSetStates(new List<SetState>() { state12, state34 });
            List<Dictionary<State, State>> expectedConnections = new List<Dictionary<State, State>>() {
                new Dictionary<State, State>(){ { state12, state34} }
            };
            rellocationData = new Dictionary<string, object>()
            {
                { "automaton", automaton},
                {"expectedConnections", expectedConnections },
                { "optimizedStates", optimizedStates}
            };

        }

        [TestInitialize]
        public void TestInitialize()
        {
            this.getAutomatonData();
            this.getProcessedStatesData();
            this.getRellocationData();
        }

        [TestMethod()]
        public void determineNewStatesTest()
        {
            bool sameStatesFlag = true;
            Automaton automaton = (Automaton)this.automatonData["automaton"];
            List<State> expectedStates = (List<State>)this.automatonData["expectedStates"];
            AutomatonConverter automatonConverter = new AutomatonConverter(automaton);
            automatonConverter.determineNewStates();
            List<State> states = automatonConverter.getProcessedStates();
            states.Sort((x, y) => String.Compare(x.symbol, y.symbol));
            expectedStates.Sort((x,y) => String.Compare(x.symbol, y.symbol));
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].symbol != expectedStates[i].symbol)
                {
                    sameStatesFlag = false;
                }
            }
            Assert.IsTrue(sameStatesFlag);
        }

        [TestMethod()]
        public void reallocateTransitionsTest()
        {
            Automaton automaton = (Automaton)this.rellocationData["automaton"];
            List<Dictionary<State, State>> expectedConnections = (List<Dictionary<State, State>>)rellocationData["expectedConnections"];
            List<State> optimizedStates = (List<State>)rellocationData["optimizedStates"];
            AutomatonConverter automatonConverter = new AutomatonConverter(automaton);
            automatonConverter.setOptimizedStates(optimizedStates);
            automatonConverter.reallocateTransitions();
            Assert.IsTrue(optimizedStates[0].hasConnection(optimizedStates[1]));
            //foreach(KeyValuePair<State, State> pair in expectedConnections[0])
            //{
            //    Assert.IsTrue(pair.Key.hasConnection(pair.Value));
            //}
        }

        [TestMethod()]
        public void squashDuplicatesTest()
        {
            bool squashFlag = true;
            Automaton dummyAutomaton = new Automaton("");
            List<State> processedStates = (List<State>)processedStatesData["processedStates"];
            List<State> expectedOptimizedStates = (List<State>)processedStatesData["expectedOptimizedStates"];
            AutomatonConverter automatonConverter = new AutomatonConverter(dummyAutomaton);
            automatonConverter.setProcessedStates(processedStates);
            automatonConverter.squashDuplicates();
            List<State> actualOptimizedStates = automatonConverter.getOptimizedStates();
            expectedOptimizedStates.Sort((x, y) => String.Compare(x.symbol, y.symbol));
            actualOptimizedStates.Sort((x, y) => String.Compare(x.symbol, y.symbol));
            for (int i = 0; i < actualOptimizedStates.Count; i++)
            {
                if (actualOptimizedStates[i].symbol != expectedOptimizedStates[i].symbol)
                {
                    squashFlag = false;
                    break;
                }
            }
            Assert.IsTrue(squashFlag);
        }
    }
}