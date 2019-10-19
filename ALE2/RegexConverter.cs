using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALE2
{
    public class RegexConverter
    {
        Automaton automaton;
        public RegexConverter()
        {
            this.automaton = new Automaton("");
        }

        public Automaton getAutomaton()
        {
            return this.automaton;
        }

        public Automaton createAutomaton(string regexInput, int stateLabel)
        {
            State state1 = new State(stateLabel.ToString(), false);
            this.automaton.addState(state1);
            stateLabel++;
            State state2 = new State(stateLabel.ToString(), true);
            this.automaton.addState(state2);
            this.automaton.addFinal(state2);
            stateLabel++;
            if (regexInput == "")
            {
                state1.addConnection(new Dictionary<string, object>() { { "transitionLabel", "_" }, { "endState", state2 } });
                this.automaton.Alphabet = "_";
            }
            else
            {
                Node root = this.buildRegexTree(regexInput);
                this.createConnections(state1, state2, root, stateLabel);
            }
            this.generateAutomatonFile();
            Parser parser = new Parser();
            parser.renderAutomaton(this.automaton);
            return this.automaton;
        }

        public Node buildRegexTree(string regexInput)
        {
            List<char> punctuationSymbols = new List<char>() { '(', ')', ',' };
            List<char> regexSymbols = new List<char>() { '*', '.', '|' };
            int counter = 0;
            string rootValue = regexInput[0].ToString();
            Node root = new Node(rootValue);
            root.Label = "Node" + counter.ToString();
            String alphabet = "";
            counter++;
            Node currentState = root;
            for(int i = 1; i < regexInput.Length; i++)
            {
                if (regexInput[i] == '(')
                {
                    currentState.LeftChild = new Node(" ");
                    currentState.LeftChild.Parent = currentState;
                    currentState.LeftChild.Label = "Node" + counter.ToString();
                    counter++;
                    currentState = currentState.LeftChild;
                }

                if (regexSymbols.IndexOf(regexInput[i]) > -1)
                {
                    currentState.Symbol = regexInput[i].ToString();
                }
                if (regexSymbols.IndexOf(regexInput[i]) == -1 && punctuationSymbols.IndexOf(regexInput[i]) == -1)
                {
                    currentState.Symbol = regexInput[i].ToString();
                    currentState = currentState.Parent;
                    alphabet += regexInput[i];
                }
                if (regexInput[i] == ',')
                {
                    currentState.RightChild = new Node(" ");
                    currentState.RightChild.Parent = currentState;
                    currentState.RightChild.Label = "Node" + counter.ToString();
                    counter++;
                    currentState = currentState.RightChild;
                }
                if (regexInput[i] == ')')
                {
                    currentState = currentState.Parent;
                }
            }
            this.automaton.Alphabet = alphabet;
            return root;
        }

        public int createConnections(State startState, State endState, Node node, int stateLabel)
        {
            if (node.Symbol == ".")
            {
                State intermediateState = new State(stateLabel.ToString(), false);
                this.automaton.addState(intermediateState);
                stateLabel++;
                if (node.LeftChild.LeftChild is null)
                {
                    Dictionary<string, object> connection12 = new Dictionary<string, object>() { 
                        { "transitionLabel", node.LeftChild.Symbol }, { "endState", intermediateState} };
                    startState.addConnection(connection12);
                }
                else
                {
                    stateLabel = this.createConnections(startState, intermediateState, node.LeftChild, stateLabel);
                }

                if (node.RightChild.LeftChild is null)
                {
                    Dictionary<string, object> connection23 = new Dictionary<string, object>()
                    {
                        { "transitionLabel", node.RightChild.Symbol},
                        {"endState", endState}
                    };
                    intermediateState.addConnection(connection23);
                }
                else
                {
                    stateLabel = this.createConnections(intermediateState, endState, node.RightChild, stateLabel);
                }
            }

            if (node.Symbol == "|")
            {
                if (node.LeftChild.LeftChild is null)
                {
                    Dictionary<string, object> connection1 = new Dictionary<string, object>()
                    {
                        {"transitionLabel", node.LeftChild.Symbol},
                        {"endState", endState}
                    };
                    startState.addConnection(connection1);
                }
                else
                {
                    stateLabel = this.createConnections(startState, endState, node.LeftChild, stateLabel);
                }

                if (node.RightChild.LeftChild is null)
                {
                    Dictionary<string, object> connection2 = new Dictionary<string, object>()
                    {
                        { "transitionLabel", node.RightChild.Symbol},
                        {"endState", endState }
                    };
                    startState.addConnection(connection2);
                }
                else
                {
                    stateLabel = this.createConnections(startState, endState, node.RightChild, stateLabel);
                }
            }

            if (node.Symbol == "*")
            {
                State state2 = new State(stateLabel.ToString(), false);
                this.automaton.addState(state2);
                stateLabel++;
                State state3 = new State(stateLabel.ToString(), false);
                this.automaton.addState(state3);
                stateLabel++;
                Dictionary<string, object> connection12 = new Dictionary<string, object>()
                {
                    {"transitionLabel", '_'}, {"endState", state2}
                };
                Dictionary<string, object> connection34 = new Dictionary<string, object>()
                {
                    {"transitionLabel", '_'}, {"endState", endState}
                };
                Dictionary<string, object> connection32 = new Dictionary<string, object>()
                {
                    {"transitionLabel", '_'}, {"endState", state2}
                };
                Dictionary<string, object> connection14 = new Dictionary<string, object>()
                {
                    {"transitionLabel", '_'}, {"endState", endState}
                };

                startState.addConnection(connection12);
                state3.addConnection(connection34);
                state3.addConnection(connection32);
                startState.addConnection(connection14);

                if (node.LeftChild.LeftChild is null)
                {
                    Dictionary<string, object> connection23 = new Dictionary<string, object>()
                    {
                        { "transitionLabel", node.LeftChild.Symbol},
                        { "endState", state3 }
                    };
                    state2.addConnection(connection23);
                }
                else
                {
                    stateLabel = this.createConnections(state2, state3, node.LeftChild, stateLabel);
                }
            }
            return stateLabel;
        }

        public void generateAutomatonFile()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"GeneratedAutomaton.txt", true))
            {
                String alphabetEntry = "alphabet: " + this.automaton.Alphabet;
                file.WriteLine(alphabetEntry);
                List<State> states = this.automaton.getStatesPool();
                String statesString = "";
                for (int index = 0; index < states.Count; index++)
                {
                    statesString += states[index].symbol;
                    if (index != states.Count - 1)
                    {
                        statesString += ',';
                    }
                }
                String statesEntry = "states:" + statesString;
                file.WriteLine(statesEntry);
                List<State> finalStates = this.automaton.getFinalStates();
                String finalStatesString = "";
                for (int index = 0; index < finalStates.Count; index++)
                {
                    finalStatesString += finalStates[index].symbol;
                    if (index != finalStates.Count - 1)
                    {
                        finalStatesString += ',';
                    }
                }
                String finalStatesEntry = "final: " + finalStatesString;
                file.WriteLine(finalStatesEntry);
                file.WriteLine();
                file.WriteLine("transitions:");
                foreach(State state in states)
                {
                    List<Dictionary<string, object>> connections = state.getConnections();
                    foreach(Dictionary<string, object> connection in connections)
                    {
                        String endState = ((State)connection["endState"]).symbol;
                        String transitionLabel = connection["transitionLabel"].ToString();
                        String transitionEntry = String.Format("{0},{1} => {2}", state.symbol, transitionLabel, endState);
                        file.WriteLine(transitionEntry);
                    }
                }
                file.WriteLine("end.");
            }
        }
    }
}