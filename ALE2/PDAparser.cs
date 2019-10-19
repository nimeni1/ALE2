using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALE2
{
    public class PDAparser
    {
        PDA pdaAutomaton;

        public PDAparser()
        {

        }

        public PDA readFromFile()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.ShowDialog();
            String fileName = fileDialog.FileName;
            Dictionary<string, object> data = this.parseData(fileName);
            String alphabet = (String)data["alphabet"];
            List<String> stack = (List<String>)data["stack"];
            List<String> states = (List<String>)data["states"];
            List<String> final = (List<String>)data["final"];
            List<List<String>> transitions = (List<List<string>>)data["transitions"];
            this.createAutomaton(alphabet, stack, states, final, transitions);
            return this.pdaAutomaton;
        }

        public PDA readFromFileForTest(String fileName)
        {
            Dictionary<string, object> data = this.parseData(fileName);
            String alphabet = (String)data["alphabet"];
            List<String> stack = (List<String>)data["stack"];
            List<String> states = (List<String>)data["states"];
            List<String> final = (List<String>)data["final"];
            List<List<String>> transitions = (List<List<string>>)data["transitions"];
            this.createAutomaton(alphabet, stack, states, final, transitions);
            return this.pdaAutomaton;
        }

        public Dictionary<string, object> parseData(string filename)
        {
            Dictionary<string, object> parsedData = new Dictionary<string, object>();
            String alphabet = "";
            List<String> stack = new List<string>();
            List<String> states = new List<string>();
            List<String> final = new List<string>();
            List<List<String>> transitions = new List<List<string>>();
            bool transitionFlag = false;

            String line;
            System.IO.StreamReader file = new System.IO.StreamReader(@filename);
            while ((line = file.ReadLine()) != null)
            {
                line.Replace(System.Environment.NewLine, String.Empty);
                if (line == "end." || line == ".end")
                {
                    break;
                }
                if (line == "")
                {
                    continue;
                }
                List<string> lineElements = line.Split(' ').ToList();
                if (lineElements[0] == "alphabet:")
                {
                    alphabet = lineElements[1];
                }
                if (lineElements[0] == "stack:")
                {
                    stack = lineElements[1].Split(',').ToList();
                }
                if (lineElements[0] == "states:")
                {
                    states = lineElements[1].Split(',').ToList();
                }
                if (lineElements[0] == "final:")
                {
                    final = lineElements[1].Split(',').ToList();
                }
                if (lineElements[0] == "transitions:")
                {
                    transitionFlag = true;
                }
                if (transitionFlag && lineElements[0] != "transitions:")
                {
                    transitions.Add(lineElements);
                }
            }
            foreach(List<String> transition in transitions)
            {
                int count = 0;
                transition.RemoveAll((String s) => {
                    if (s == "" && count < 5) {
                        count++;
                        return true;
                    }
                    return false;
                });
            }
            parsedData.Add("alphabet", alphabet);
            parsedData.Add("stack", stack);
            parsedData.Add("states", states);
            parsedData.Add("final", final);
            parsedData.Add("transitions", transitions);
            file.Close();
            return parsedData;

        }

        public void createAutomaton(
            string alphabet, List<String> stack, List<String> states, List<String> final, List<List<String>> transitions)
        {

            this.pdaAutomaton = new PDA(alphabet);
            this.pdaAutomaton.setStack(stack);
            bool finalFlag = false;
            foreach (String s in states)
            {
                State state = new State(s, finalFlag);
                this.pdaAutomaton.addState(state);
            }
            List<State> auxStates = this.pdaAutomaton.getStatesPool();
            foreach (String f in final)
            {
                foreach (State s in auxStates)
                {
                    if (s.symbol == f)
                    {
                        s.finalFlag = true;
                        this.pdaAutomaton.addFinal(s);
                    }
                }
            }
            foreach (List<String> transition in transitions)
            {
                List<string> transitionInfo = transition[0].Split(',').ToList();
                String start = transitionInfo[0];
                String transitionLabel = transitionInfo[1];
                String transitionStack = transition[1];
                String stackToPush;
                String stackToPop;
                if (transitionStack != "") {
                    transitionStack = transitionStack.Replace("[", "");
                    transitionStack = transitionStack.Replace("]", "");
                    stackToPush = transitionStack[0].ToString();
                    stackToPop = transitionStack[2].ToString();
                }
                else
                {
                    stackToPush = "_";
                    stackToPop = "_";
                }
                
                String end = transition[3];
                State startState = this.pdaAutomaton.getAutomatonState(start);
                State endState = this.pdaAutomaton.getAutomatonState(end);
                Dictionary<string, object> connection = new Dictionary<string, object>() {
                    { "transitionLabel", transitionLabel },
                    { "endState", endState },
                    { "stackToPush", stackToPush },
                    { "stackToPop", stackToPop }
                };
                startState.addConnection(connection);

            }
        }
    }
}
