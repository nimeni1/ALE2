using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALE2
{
    public class State
    {
        public String symbol;
        public bool finalFlag;
        List<Dictionary<string, object>> connections;
        public int processedConnections;
        List<State> silentTree;

        public State(string symbol, bool finalFlag)
        {
            this.symbol = symbol;
            this.finalFlag = finalFlag;
            this.connections = new List<Dictionary<string,object>> ();
            this.processedConnections = 0;
        }

        public void makeSetConnections(char letter)
        {
            //3 cases:
            //state-state
            //state-setstate
            //setstate-setstate
            List<Dictionary<string, object>> connectionsForLetter = this.getFullConnections(letter);
            List<SetState> alreadySetStates = new List<SetState>();
            List<State> simpleStates = new List<State>();
            List<Dictionary<string, object>> connectionsToRemove = new List<Dictionary<string, object>>();

            foreach(Dictionary<string, object> c in connectionsForLetter)
            {
                if (c["endState"] is SetState) alreadySetStates.Add((SetState)c["endState"]);
                else simpleStates.Add((State)c["endState"]);
                connectionsToRemove.Add(c);
            }

            foreach (Dictionary<string, object> c in connectionsToRemove)
            {
                this.removeConnection(c);
            }
            SetState setState = new SetState("", false, new List<State>());
            if (simpleStates.Count > 0) setState = new SetState("", false, simpleStates);
            for(int i=1; i< alreadySetStates.Count; i++)
            {
                alreadySetStates[0].addComponentStates(alreadySetStates[i].getComponentStates());
            }
            setState.addComponentStates(alreadySetStates[0].getComponentStates());
            Dictionary<string, object> newSetConnection = new Dictionary<string, object>()
            {
                { "transitionLabel", letter },
                { "endState", setState }
            };
            this.connections.Add(newSetConnection);

        }

        public void addConnection(Dictionary<string, object> connection)
        {
            connections.Add(connection);
        }

        public List<Dictionary<string, object>> getConnections()
        {
            return connections;
        }

        public List<Dictionary<string, object>> getFullConnections(char letter)
        {
            List<Dictionary<string, object>> selectedConnections = new List<Dictionary<string, object>>();
            foreach (Dictionary<string, object> connection in connections)
            {
                if ((String)connection["transitionLabel"] == letter.ToString())
                {
                    selectedConnections.Add(connection);
                }
            }
            return selectedConnections;
        }

        public List<State> getConnections(char letter)
        {
            List<State> selectedConnections = new List<State>();
            foreach (Dictionary<string, object> connection in connections)
            {
                if ((String)connection["transitionLabel"] == letter.ToString())
                {
                    selectedConnections.Add((State)connection["endState"]);
                }
            }
            return selectedConnections;
        }

        public List<Dictionary<string, object>> getStateConnections(State state)
        {
            List<Dictionary<string, object>> stateConnections = new List<Dictionary<string, object>>();
            foreach(Dictionary<string, object> connection in this.connections)
            {
                State endState = (State)connection["endState"];
                if (endState.symbol == state.symbol)
                {
                    stateConnections.Add(connection);
                }
            }
            return stateConnections;
        }

        public bool hasNonEmptyConnection(State state)
        {
            foreach(Dictionary<string, object> connection in connections)
            {
                if (((State)connection["endState"]).symbol == state.symbol)
                {
                    if ((String)connection["transitionLabel"] != "_")
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public List<State> determineSilentTree()
        {
            List<State> tree = new List<State>();
            List<State> silentStates = this.getConnections('_');
            foreach(State silentS in silentStates)
            {
                tree.Add(silentS);
                List<State> silentAncestors = silentS.determineSilentTree();
                foreach(State silentA in silentAncestors)
                {
                    tree.Add(silentA);
                }
            }
            if(tree.Count == 0)
            {
                return tree;
            }
            tree.Add(this);
            List<State> noDuplicatesTree = new List<State>();
            foreach(State state in tree)
            {
                if(noDuplicatesTree.IndexOf(state) != -1)
                {
                    noDuplicatesTree.Add(state);
                }
            }
            this.silentTree = noDuplicatesTree;
            return this.silentTree;
        }

        public virtual bool isSubset(State state)
        {
            bool subsetFlag = true;
            if(!(state is SetState))
            {
                return false;
            }
            if(((SetState)state).getComponentStates().IndexOf(this) == -1)
            {
                subsetFlag = false;
            }
            return subsetFlag;
        }

        internal bool hasSameConnectionsAs(State state)
        {
            bool connectionFlag = true;
            List<Dictionary<string, object>> connections = state.getConnections();
            foreach(Dictionary<string, object> connection in connections)
            {
                State endState = (State)connection["endState"];
                if (!this.hasConnection(endState))
                {
                    connectionFlag = false;
                }
            }
            return connectionFlag;
        }

        public bool hasConnection(State state)
        {
            bool connectionFlag = false;
            foreach(Dictionary<string, object> connection in this.connections)
            {
                State endState = (State)connection["endState"];
                if(endState.symbol == state.symbol)
                {
                    connectionFlag = true;
                    break;
                }
            }
            return connectionFlag;
        }

        internal void clearConnections()
        {
            this.connections.Clear();
        }

        public void removeConnection(Dictionary<string, object> connection)
        {
            this.connections.Remove(connection);
        }

        public bool hasEmptyConnection()
        {
            foreach(Dictionary<String, object> connection in this.connections)
            {
                if ((String)connection["transitionLabel"] == "_") return true;
            }
            return false;
        }

        public List<Dictionary<string, object>> getEmptyConnections()
        {
            List<Dictionary<string, object>> emptyConnections = new List<Dictionary<string, object>>();
            foreach(Dictionary<string, object> connection in this.connections)
            {
                if ((string)connection["transitionLabel"] == "_")
                {
                    emptyConnections.Add(connection);
                }
            }
            return emptyConnections;
        }
    }
}
