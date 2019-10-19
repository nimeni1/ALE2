using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
//using Graphviz4Net;
//using Graphviz4Net.Graphs;
//using Graphviz4Net.Dot.AntlrParser;
//using QuickGraph;
//using QuickGraph.Collections;

namespace ALE2
{
    public class Parser
    {
        Automaton automaton;
        public Parser()
        {

        }

        public Automaton getAutomaton()
        {
            return this.automaton;
        }

        public Automaton handleAutomaton()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.ShowDialog();
            String fileName = fileDialog.FileName;
            Dictionary<string, object> data = this.parseData(fileName);
            String alphabet = (String)data["alphabet"];
            List<String> states = (List<String>)data["states"];
            List<String> final = (List<String>)data["final"];
            List<List<String>> transitions = (List<List<string>>)data["transitions"];
            this.createAutomaton(alphabet, states, final, transitions);
            this.renderAutomaton(this.automaton);
            this.automaton.checkDfa();
            return this.automaton;
        }

        public Dictionary<string, object> parseData(string filename)
        {
            Dictionary<string, object> parsedData = new Dictionary<string, object>();
            String alphabet = "";
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
            parsedData.Add("alphabet", alphabet);
            parsedData.Add("states", states);
            parsedData.Add("final", final);
            parsedData.Add("transitions", transitions);
            file.Close();
            return parsedData;

        }

        void createDotFile()
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter("automaton.dot"))
            {
                file.WriteLine("digraph myAutomaton " + "{");
                List<State> states = this.automaton.getStatesPool();
                foreach(State state in states)
                {
                    if (state.finalFlag)
                    {
                        file.WriteLine('"' + state.symbol + '"' + " [shape=doublecircle]");
                    }
                    else
                    {
                        file.WriteLine('"' + state.symbol + '"' + " [shape=circle]");
                    }
                    
                }
                file.WriteLine();
                foreach(State state in states)
                {
                    List<Dictionary<string, object>> connections = state.getConnections();
                    foreach(Dictionary<string, object> connection in connections)
                    {
                        State endState = (State)connection["endState"];
                        file.WriteLine('"' + state.symbol + '"' + " " + "->" + " " + '"' + endState.symbol + '"' + " " + 
                            "[label=" + '"' + connection["transitionLabel"].ToString() + '"' + "]");
                    }
                }
                file.WriteLine("}");
            }
        }

        public async void renderAutomaton(Automaton automaton)
        {
            createDotFile();
            Process dot = new Process();
            dot.StartInfo.FileName = "dot.exe";
            dot.StartInfo.Arguments = "-Tpng -oautomaton.png automaton.dot";
            dot.Start();
            dot.WaitForExit();
            //Process.Start("automaton.png");
        }

        public void createAutomaton(string alphabet, List<String> states, List<String> final, List<List<String>> transitions)
        {
           
                this.automaton = new Automaton(alphabet);
                bool finalFlag = false;
                foreach (String s in states)
                {
                    State state = new State(s, finalFlag);
                    this.automaton.addState(state);
                }
                List<State> auxStates = this.automaton.getStatesPool();
                foreach (String f in final)
                {
                    foreach (State s in auxStates)
                    {
                        if (s.symbol == f)
                        {
                            s.finalFlag = true;
                            this.automaton.addFinal(s);
                        }
                    }
                }
                foreach (List<String> transition in transitions)
                {
                    List<string> transitionInfo = transition[0].Split(',').ToList();
                    String start = transitionInfo[0];
                    String transitionLabel = transitionInfo[1];
                    String end = transition[2];
                    State startState = this.automaton.getAutomatonState(start);
                    State endState = this.automaton.getAutomatonState(end);
                    Dictionary<string, object> connection = new Dictionary<string, object>() {
                    { "transitionLabel", transitionLabel }, { "endState", endState } };
                    startState.addConnection(connection);

                }
            
        }
    }
}
