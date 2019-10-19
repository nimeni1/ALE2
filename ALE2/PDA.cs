using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALE2
{
    public class PDA : Automaton
    {
        string alphabet;
        Stack<string> stack;

        public PDA(string alphabet) : base(alphabet)
        {
            this.alphabet = alphabet;
            stack = new Stack<string>();
        }

        public void setStack(List<String> stack)
        {
            foreach (String s in stack)
            {
                this.stack.Push(s);
            }
        }

        public bool checkString(string stringToCheck)
        {
            bool accepted = false;
            Stack<Dictionary<string, object>> stack = new Stack<Dictionary<string, object>>();
            List<State> states = this.getStatesPool();
            State state = states[0];
            Stack<String> currentStack = new Stack<string>();
            foreach (String s in this.stack)
            {
                currentStack.Push(s);
            }
            int index = 0;

            while (index < stringToCheck.Length || state.hasEmptyConnection())
            {
                State nextState = null;
                if (index >= stringToCheck.Length && state.hasEmptyConnection())
                {
                    List<Dictionary<string, object>> emptyConnections = state.getEmptyConnections();
                    foreach (Dictionary<string, object> connection in emptyConnections)
                    {
                        String stackToPop = "";
                        if (currentStack.Count > 0)
                        {
                            stackToPop = currentStack.Pop();
                            currentStack.Push(stackToPop);
                        }
                        if ((String)connection["stackToPop"] == stackToPop || (String)connection["stackToPop"] == "_")
                        {
                            Stack<String> dummyStack = new Stack<string>();
                            foreach (String s in currentStack)
                            {
                                dummyStack.Push(s);
                            }
                            if ((String)connection["stackToPop"] != "_")
                            {
                                dummyStack.Pop();
                            }

                            if ((String)connection["stackToPush"] != "_")
                            {
                                dummyStack.Push((String)connection["stackToPush"]);
                            }

                            Dictionary<string, object> dummyStackDictionary = new Dictionary<string, object>();
                            dummyStackDictionary["dummyStack"] = dummyStack;

                            connection["index"] = index;
                            stack.Push(dummyStackDictionary);
                            stack.Push(connection);
                        }
                    }
                }
                else
                {
                    List<Dictionary<string, object>> connections = state.getConnections();
                    foreach (Dictionary<string, object> connection in connections)
                    {
                        if ((String)connection["transitionLabel"] == stringToCheck[index].ToString() ||
                            (String)connection["transitionLabel"] == "_")
                        {
                            String stackToPop = "";
                            if (currentStack.Count > 0)
                            {
                                stackToPop = currentStack.Pop();
                                currentStack.Push(stackToPop);
                            }
                            if ((String)connection["stackToPop"] == stackToPop || (String)connection["stackToPop"] == "_")
                            {
                                Stack<String> dummyStack = new Stack<string>();
                                foreach (String s in currentStack)
                                {
                                    dummyStack.Push(s);
                                }
                                if ((String)connection["stackToPop"] != "_")
                                {
                                    dummyStack.Pop();
                                }

                                if ((String)connection["stackToPush"] != "_")
                                {
                                    dummyStack.Push((String)connection["stackToPush"]);
                                }

                                Dictionary<string, object> dummyStackDictionary = new Dictionary<string, object>();
                                dummyStackDictionary["dummyStack"] = dummyStack;

                                connection["index"] = index;
                                stack.Push(dummyStackDictionary);
                                stack.Push(connection);
                            }
                        }
                    }
                }
                
                if (stack.Count == 0)
                {
                    break;
                }
                Dictionary<string, object> top = stack.Pop();
                currentStack = (Stack<String>)stack.Pop()["dummyStack"];
                if ((String)top["transitionLabel"] != "_") index = (int)top["index"] + 1;
                else index = (int)top["index"];
                nextState = (State)top["endState"];
                state = nextState;
                if (index == stringToCheck.Length && state.finalFlag && currentStack.Count == 0)
                {
                    accepted = true;
                }
            }
            return accepted;
        }


    }
}
