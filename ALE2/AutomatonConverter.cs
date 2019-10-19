using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALE2
{
    public class AutomatonConverter
    {
        private Automaton automaton;
        State sink;
        String alphabet;
        List<State> processedStates;
        List<State> optimizedStates;

        public AutomatonConverter()
        {

        }

        public List<State> getProcessedStates()
        {
            return this.processedStates;
        }

        public void setProcessedStates(List<State> states)
        {
            this.processedStates = states;
        }

        public List<State> getOptimizedStates()
        {
            return this.optimizedStates;
        }

        public void setOptimizedStates(List<State> states)
        {
            this.optimizedStates = states;
        }

        public AutomatonConverter(Automaton automaton)
        {
            this.automaton = automaton;
            this.sink = new State("sink", false);
            this.alphabet = this.automaton.Alphabet;
            foreach(char letter in this.alphabet)
            {
                Dictionary<string, object> connection = new Dictionary<string, object>()
                {
                    { "transitionLabel", letter.ToString()},
                    { "endState", this.sink }
                };
                this.sink.addConnection(connection);
            }
        }

        public void determineNewStates()
        {
            List<State> statesPool = this.automaton.getStatesPool();
            State initialState = statesPool[0];
            List<State> unprocessedStatedStack = new List<State>();
            this.processedStates = new List<State>();
            unprocessedStatedStack.Add(initialState);
            while (unprocessedStatedStack.Count != 0)
            {
                State currentState = unprocessedStatedStack[0];
                unprocessedStatedStack.RemoveAt(0);
                this.processedStates.Add(currentState);
                foreach (char letter in this.alphabet)
                {
                    List<State> connectedStates = currentState.getConnections(letter);
                    if (currentState.symbol == "{B,A}")
                    {
                        foreach (Dictionary<string, object> c in currentState.getConnections())
                        {
                            Console.WriteLine(((State)c["endState"]).symbol);
                            Console.WriteLine(c["transitionLabel"]);
                        }
                            
                        break;
                    }
                    if (connectedStates.Count > 1)
                    {
                        List<SetState> setStates = new List<SetState>();
                        List<State> standardStates = new List<State>();
                        foreach(State state in connectedStates)
                        {
                            if (state is SetState)
                            {
                                setStates.Add((SetState)state);
                            } 
                            else
                            {
                                standardStates.Add(state);
                            }
                        }
                        if(setStates.Count != 0)
                        {
                            SetState firstSetState = (SetState)setStates[0];
                            setStates.RemoveAt(0);
                            foreach(SetState state in setStates)
                            {
                                firstSetState.addComponentStates(state.getComponentStates());
                            }
                            firstSetState.addComponentStates(standardStates);
                            if (!processedStates.Contains(firstSetState))
                                unprocessedStatedStack.Add(firstSetState);
                        }
                        else
                        {
                            SetState newSetState = new SetState("", false, connectedStates);
                            newSetState.makeSetConnections(letter);
                            Console.WriteLine("At the creation of the BA setstate");
                            if (!processedStates.Contains(newSetState))
                                unprocessedStatedStack.Add(newSetState);
                        }
                    }
                    if (connectedStates.Count == 1)
                    {
                        if (!processedStates.Contains(connectedStates[0]))
                            unprocessedStatedStack.Add(connectedStates[0]);
                    }
                    if (connectedStates.Count == 0)
                    {
                        Dictionary<string, object> connection = new Dictionary<string, object>()
                        {
                            { "transitionLabel", letter.ToString() },
                            { "endState", this.sink }
                        };
                        currentState.addConnection(connection);
                    }
                }
            }
        }

        public void convert()
        {
            this.automaton = new Automaton(this.alphabet);
            //foreach(State state in this.optimizedStates)
            //{
            //    this.automaton.addState(state);
            //    if (state.finalFlag)
            //    {
            //        this.automaton.addFinal(state);
            //    }
            //}
            //this.reallocateTransitions();
            Parser parser = new Parser();
            parser.renderAutomaton(this.automaton);
        }

        public void reallocateTransitions()
        {
            this.automaton.clearStates();
            foreach(State optimized in this.optimizedStates)
            {
                this.automaton.addState(optimized);
                if (optimized.finalFlag)
                {
                    this.automaton.addFinal(optimized);
                }
            }
            this.automaton.reallocateTransitions();
            //this.automaton.removeStateConnectionDuplicates();
        }

        public void squashDuplicates()
        {
            optimizedStates = new List<State>();
            int duplicateCounter = 0;
            foreach(State state in this.processedStates)
            {
                duplicateCounter = 0;
                foreach(State state2 in this.processedStates)
                {
                    if (state.isSubset(state2) && state.hasSameConnectionsAs(state2))
                    {
                        if(this.optimizedStates.IndexOf(state2) == -1)
                        {
                            this.optimizedStates.Add(state2);
                        }
                    }
                    else
                    {
                        duplicateCounter++;
                    }
                }
                if(duplicateCounter == this.processedStates.Count)
                {
                    this.optimizedStates.Add(state);
                }
            }
        }
    }
}
