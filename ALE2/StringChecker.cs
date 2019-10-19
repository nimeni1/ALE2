using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALE2
{
    public class StringChecker
    {
        Automaton automaton;
        string stringToCheck;

        public StringChecker(Automaton automaton, string stringToCheck)
        {
            this.automaton = automaton;
            this.stringToCheck = stringToCheck;
        }

        public bool checkString()
        {
            bool accepted = false;
            Stack<Dictionary<string, object>> stack = new Stack<Dictionary<string, object>>();
            List<State> states = this.automaton.getStatesPool();
            State state = states[0];
            int index = 0;
            while(index < this.stringToCheck.Length || state.hasEmptyConnection())
            {
                State nextState = null;
                if (index >= this.stringToCheck.Length && state.hasEmptyConnection())
                {
                    List<Dictionary<string, object>> connections = state.getEmptyConnections();
                    foreach (Dictionary<string, object> connection in connections)
                    {
                        connection["index"] = index;
                        stack.Push(connection);   
                    }
                }
                else
                {
                    List<Dictionary<string, object>> connections = state.getConnections();
                    foreach (Dictionary<string, object> connection in connections)
                    {
                        if ((String)connection["transitionLabel"] == this.stringToCheck[index].ToString())
                        {
                            connection["index"] = index;
                            stack.Push(connection);
                        }
                    }
                }

                if (stack.Count == 0)
                {
                    break;
                }
                Dictionary<string, object> top = stack.Pop();
                if ((string)top["transitionLabel"] != "_") index = (int)top["index"] + 1;
                else index = (int)top["index"]; 
                nextState = (State)top["endState"];
                state = nextState;
                Console.WriteLine(state.symbol);
                if (index == this.stringToCheck.Length && state.finalFlag)
                {
                    accepted = true;
                }
            }
            return accepted;
        }
    }
}