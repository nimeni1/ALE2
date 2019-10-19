using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALE2
{
    public class SetState : State
    {
        public List<State> componentStates;
        public SetState(string symbol, bool finalFlag, List<State> states) : base(symbol, finalFlag)
        {
            this.componentStates = new List<State>();
            this.addComponentStates(states);
            //this.setConnections();

        }

        public void removeConnectionDuplicates()
        {
            List<Dictionary<string, object>> connections = this.getConnections();
            List<Dictionary<string, object>> duplicateConnections = new List<Dictionary<string, object>>();
            for (int i = 0; i < connections.Count; i++)
                for (int j = i+1; j < connections.Count; j++)
                    if (connections[i]["endState"] == connections[j]["endState"] ||
                        connections[i]["transitionLabel"] == connections[j]["transitionLabel"])
                    {
                        duplicateConnections.Add(connections[j]);
                    }
            foreach(Dictionary<string, object> c in duplicateConnections)
            {
                this.removeConnection(c);
            }
                    
        }

        public void addComponentStates(List<State> states)
        {
            foreach(State state in states)
            {
                if (componentStates.IndexOf(state) == -1)
                {
                    this.componentStates.Add(state);
                }
                this.setSymbol();
                this.setFinalFlag();
                this.setConnections();
            }
        }

        void setConnections()
        {
            foreach(State state in this.componentStates)
            {
                List<Dictionary<string, object>> connectionsToAdd = state.getConnections();
                foreach(Dictionary<string, object> connection in connectionsToAdd)
                {
                    this.addConnection(connection);
                    this.makeSetConnections(Convert.ToChar(connection["transitionLabel"]));
                }
            }
            this.removeConnectionDuplicates();
        }

        //void printStructure()
        //{
        //    foreach(State state in this.componentStates)
        //    {
        //        Console.WriteLine("State symbol is " + state.symbol);
        //        List<Dictionary<string, object>> connections = state.getConnections();
        //        foreach(Dictionary<string, object> connection in connections)
        //        {
        //             Console.WriteLine(((State)connection["endState"]).symbol);
        //             Console.WriteLine((string)connection["transitionLabel"]);
        //             Console.WriteLine("----");
        //        }
        //    }
        //}

        void setSymbol()
        {
            string symbol = "{";
            for (int i = 0; i < componentStates.Count; i++)
            {
                symbol += this.componentStates[i].symbol;
                if (i != this.componentStates.Count - 1)
                {
                    symbol += ",";
                }
            }
            symbol += "}";
            this.symbol = symbol;
        }

        void setFinalFlag()
        {
            bool finalFlag = false;
            foreach(State state in this.componentStates)
            {
                if (state.finalFlag == true)
                {
                    finalFlag = true;
                    break;
                }
            }
            this.finalFlag = finalFlag;
        }

        public List<State> getComponentStates()
        {
            return this.componentStates;
        }

        public bool hasComponent(State state)
        {
            if (state is SetState)
            {
                List<State> components = ((SetState)state).getComponentStates();
                foreach (State component in components)
                {
                    if (this.componentStates.IndexOf(component) == -1)
                    {
                        return false;
                    }
                }
                return true;
            }

            else
            {
                if (this.componentStates.IndexOf(state) == -1)
                {
                    return false;
                }
                return true;
            }
        }

        public override bool isSubset(State state)
        {
            bool subsetFlag = true;
            if (!(state is SetState))
            {
                return false;
            }
            foreach(State component in this.componentStates)
            {
                if (((SetState)state).getComponentStates().IndexOf(component) == -1)
                {
                    subsetFlag = false;
                }
            }
            return subsetFlag;
        }
    }
}