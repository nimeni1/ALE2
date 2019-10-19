using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALE2
{
    public class Automaton
    {
        List<State> states;
        List<State> finalStates;
        public bool dfaFlag = true;
        List<State> setStates;
        List<State> noTreeStates;
        List<State> standAloneStates;

        public Automaton(string alphabet)
        {
            this.Alphabet = alphabet;
            states = new List<State>();
            finalStates = new List<State>();
            setStates = new List<State>();
        }

        public string Alphabet {get;set;}

        public bool getDfaFlag()
        {
            return this.dfaFlag;
        }

        public static bool operator ==(Automaton automaton1, Automaton automaton2)
        {
            List<State> statesPool1 = automaton1.getStatesPool();
            List<State> final1 = automaton1.getStatesPool();
            string alphabet1 = automaton1.Alphabet;
            List<State> statesPool2 = automaton2.getStatesPool();
            List<State> final2 = automaton2.getStatesPool();
            string alphabet2 = automaton2.Alphabet;
            if (statesPool1.Count != statesPool2.Count)
            {
                return false;
            }
            if (final1.Count != final2.Count)
            {
                return false;
            }
            if (alphabet1 != alphabet2)
            {
                return false;
            }
            for (int i = 0; i < statesPool1.Count; i++)
            {
                if (statesPool1[i].symbol != statesPool2[i].symbol)
                {
                    return false;
                }
            }
            statesPool1.Sort((x, y) => String.Compare(x.symbol, y.symbol));
            statesPool2.Sort((x, y) => String.Compare(x.symbol, y.symbol));
            for (int i=0; i<statesPool1.Count; i++)
            {
                List<Dictionary<string, object>> connections1 = statesPool1[i].getConnections();
                List<Dictionary<string, object>> connections2 = statesPool2[i].getConnections();
                connections1.Sort((x,y) => String.Compare(((State)x["endState"]).symbol, ((State)y["endState"]).symbol));
                connections2.Sort((x, y) => String.Compare(((State)x["endState"]).symbol, ((State)y["endState"]).symbol));
                if (connections1.Count != connections2.Count)
                {
                    return false;
                }
                for (int j=0; j<connections1.Count; j++)
                {
                    Dictionary<string, object> connection1 = connections1[j];
                    Dictionary<string, object> connection2 = connections2[j];
                    if ((string)connection1["transitionLabel"] != (string)connection2["transitionLabel"]
                        || ((State)connection1["endState"]).symbol != ((State)connection2["endState"]).symbol)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool operator!= (Automaton automaton1, Automaton automaton2)
        {
            return !(automaton1 == automaton2);
        }

        public List<State> getSetStates()
        {
            return this.setStates;
        }

        public void setSetStates(List<SetState> newSetStates)
        {
            this.setStates.Clear();
            foreach(SetState ss in newSetStates)
            {
                this.setStates.Add(ss);
            }
        }

        public void addState(State state)
        {
            states.Add(state);
        }

        public List<State> getStatesPool()
        {
            return states;
        }

        public void addFinal(State s)
        {
            finalStates.Add(s);
        }

        public List<State> getFinalStates()
        {
            return finalStates;
        }

        internal State getAutomatonState(string symbol)
        {
            foreach(State state in states)
            {
                if (state.symbol == symbol)
                {
                    return state;
                }
            }
            return null;
        }

        public void checkDfa()
        {
            foreach(State state in states)
            {
                List<Dictionary<string, object>> connections = state.getConnections();
                List<String> transitionLabels = new List<string>();
                foreach(Dictionary<string, object> connection in connections)
                {
                    if ( (String)connection["transitionLabel"] == "_")
                    {
                        this.dfaFlag = false;
                        break;
                    }
                    foreach(String label in transitionLabels)
                    {
                        if ((String)connection["transitionLabel"] == label)
                        {
                            this.dfaFlag = false;
                            break;
                        }
                    }
                    transitionLabels.Add((String)connection["transitionLabel"]);
                }
                if (this.dfaFlag == false)
                {
                    break;
                }
            }
        }

        public void clearStates()
        {
            this.finalStates.Clear();
            this.states.Clear();
        }

        public void eliminateSilentTransitions()
        {
            this.createSetStates();
            this.determineStandAloneStates();
            this.combineStandaloneSetStatesPool();
            this.reallocateTransitions();
            //this.removeStateConnectionDuplicates();
        }

        public void removeStateConnectionDuplicates()
        {
            foreach(State state in this.states)
            {
                List<Dictionary<string, object>> connections = state.getConnections();
                List<Dictionary<string, object>> noDuplicateConnections = new List<Dictionary<string, object>>();
                foreach(Dictionary<string, object> connection in connections)
                {
                    bool isDuplicate = false;
                    foreach(Dictionary<string, object> noDupConnection in noDuplicateConnections)
                    {
                        if (((State)connection["endState"]).symbol == ((State)noDupConnection["endState"]).symbol && (string)connection["transitionLabel"] == (string)noDupConnection["transitionLabel"])
                        {
                            isDuplicate = true;
                            break;
                        }
                    }
                    if (!isDuplicate)
                    {
                        noDuplicateConnections.Add(connection);
                    }
                }
                state.clearConnections();
                foreach(Dictionary<string, object> noDupConnection in noDuplicateConnections)
                {
                    state.addConnection(noDupConnection);
                }
            }
        }

        public void reallocateTransitions()
        {
            foreach(State state in this.states)
            {
                List<Dictionary<string, object>> connections = state.getConnections();
                List<Dictionary<string, object>> newConnections = new List<Dictionary<string, object>>();
                foreach (Dictionary<string, object> connection in connections)
                {
                    State endState = (State)connection["endState"];
                    foreach(SetState setState in this.setStates)
                    {
                        if (endState is SetState)
                        {
                            if (setState.getComponentStates()[0] == ((SetState)endState).getComponentStates()[0])
                            {
                                connection["endState"] = setState;
                                endState = (State)connection["endState"];
                            }
                        }
                        else
                        {
                            if (setState.getComponentStates()[0] == endState)
                            {
                                connection["endState"] = setState;
                                endState = (State)connection["endState"];
                            }
                        }
                        
                            //if(!(endState is SetState))
                            //{
                            //    connection["endState"] = setState;
                            //    endState = (State)connection["endState"];
                            //}
                            //else
                            //{
                            //    Dictionary<string, object> newConnection = new Dictionary<string, object>()
                            //    {
                            //        {"transitionLabel", (string)connection["transitionLabel"] },
                            //        {"endState", endState }
                            //    };
                            //    newConnections.Add(newConnection);
                            //}
                        
                    }
                }
                foreach (Dictionary<string, object> newConnection in newConnections)
                {
                    state.addConnection(newConnection);
                }
            }
        }

        private void combineStandaloneSetStatesPool()
        {
            this.clearStates();
            this.states = new List<State>();
            foreach(State standalone in standAloneStates)
            {
                this.states.Add(standalone);
                if (standalone.finalFlag)
                {
                    this.finalStates.Add(standalone);
                }
            }
            foreach(SetState setState in this.setStates)
            {
                this.states.Add(setState);
                if (setState.finalFlag)
                {
                    this.finalStates.Add(setState);
                }
            }
        }

        private void determineStandAloneStates()
        {
            this.standAloneStates = new List<State>();
            foreach(State state in this.noTreeStates)
            {
                bool inSetFlag = false;
                foreach(SetState setState in this.setStates)
                {
                    if (setState.hasComponent(state))
                    {
                        inSetFlag = true;
                        break;
                    }
                }
                if (!inSetFlag)
                {
                    this.standAloneStates.Add(state);
                }
            }
        }

        private void createSetStates()
        {
            this.setStates = new List<State>();
            this.noTreeStates = new List<State>();
            foreach(State state in states)
            {
                List<State> silentTree = state.determineSilentTree();
                if(silentTree.Count == 0)
                {
                    this.noTreeStates.Add(state);
                }
                else
                {
                    SetState setState = new SetState("", false, silentTree);
                    this.setStates.Add(setState);
                }
            }
        }
    }
}