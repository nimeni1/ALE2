using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALE2
{
    public class AutomatonChecker
    {
        Automaton automaton;
        List<State> statesPool;
        State initialState;
        public AutomatonChecker(Automaton automaton)
        {
            this.automaton = automaton;
        }

        public void initializeCheck()
        {
            statesPool = this.automaton.getStatesPool();
            foreach(State state in statesPool)
            {
                state.processedConnections = 0;
            }
            this.initialState = statesPool[0];
        }

        public bool check()
        {
            return this.dfsCheck(this.initialState, new List<State>());
        }

        public bool dfsCheck(State state, List<State> path)
        {
            if (this.ownLoop(state))
            {
                return false;
            }
            if (this.pathLoop(state, path))
            {
                return false;
            }

            List<Dictionary<String, object>> connections = state.getConnections();
            path.Add(state);
            foreach(Dictionary<string, object> connection in connections)
            {
                state.processedConnections++;
                State newState = (State)connection["endState"];
                if (newState.processedConnections != newState.getConnections().Count)
                {
                    if (!this.dfsCheck(newState, path))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool ownLoop(State state)
        {
            List<Dictionary<string, object>> connections = state.getConnections();
            foreach(Dictionary<string, object> connection in connections)
            {
                List<Dictionary<string, object>> stateConnections = state.getStateConnections(state);
                foreach(Dictionary<string, object> stateConnection in stateConnections)
                {
                    if ((String)stateConnection["transitionLabel"] != "_")
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool pathLoop(State state, List<State> path)
        {
            foreach(State pathState in path)
            {
                List<Dictionary<string, object>> connections = state.getStateConnections(pathState);
                foreach(Dictionary<string, object> connection in connections)
                {
                    if((String)connection["transitionLabel"] != "_")
                    {
                        return true;
                    }
                    int pathStateIndex = path.IndexOf(pathState);
                    while (pathStateIndex < path.Count - 1)
                    {
                        if (path[pathStateIndex].hasNonEmptyConnection(path[pathStateIndex + 1]))
                        {
                            return true;
                        }
                        pathStateIndex++;
                    }
                }
            }
            return false;
        }
    }
}
