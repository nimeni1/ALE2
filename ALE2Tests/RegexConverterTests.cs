using Microsoft.VisualStudio.TestTools.UnitTesting;
using ALE2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALE2.Tests
{
    [TestClass()]
    public class RegexConverterTests
    {
        List<Dictionary<string, object>> regexData;
        List<Dictionary<string, object>> connectionInput;

        [TestInitialize]
        public void TestInitialize()
        {
            this.getRegexTree();
            this.getConnectionInput();

        }

        private void getConnectionInput()
        {
            Node node1 = new Node(".");
            Node node2 = new Node("a");
            Node node3 = new Node("b");
            node1.LeftChild = node2;
            node1.RightChild = node3;

            Automaton automaton = new Automaton("ab");
            State state1 = new State("1", false);
            State state2 = new State("2", true);
            State state3 = new State("3", false);

            Dictionary<string, object> connection13 = new Dictionary<string, object>()
            {
                { "transitionLabel", "a"},
                {"endState", state3 }
            };
            state1.addConnection(connection13);
            Dictionary<string, object> connection32 = new Dictionary<string, object>()
            {
                { "transitionLabel", "b"},
                {"endState", state2 }
            };
            state3.addConnection(connection32);

            automaton.addState(state1);
            automaton.addState(state2);
            automaton.addState(state3);
            automaton.addFinal(state2);

            Dictionary<string, object> input1 = new Dictionary<string, object>()
            {
                {"automaton", automaton },
                {"tree", node1 }
            };

            Node node21 = new Node("|");
            Node node22 = new Node(".");
            Node node23 = new Node(".");
            Node node221 = new Node("a");
            Node node222 = new Node("b");
            Node node231 = new Node("a");
            Node node232 = new Node("b");
            node21.LeftChild = node22;
            node21.RightChild = node23;
            node22.LeftChild = node221;
            node22.RightChild = node222;
            node23.LeftChild = node231;
            node23.RightChild = node232;

            Automaton automaton2 = new Automaton("ab");
            State state21 = new State("1", false);
            State state22 = new State("2", true);
            State state23 = new State("3", false);
            State state24 = new State("4", false);

            Dictionary<string, object> connection213 = new Dictionary<string, object>()
            {
                {"transitionLabel", "a" },
                {"endState", state23 }
            };
            Dictionary<string, object> connection214 = new Dictionary<string, object>()
            {
                {"transitionLabel", "a" },
                {"endState", state24 }
            };
            state21.addConnection(connection213);
            state21.addConnection(connection214);
            Dictionary<string, object> connection232 = new Dictionary<string, object>()
            {
                {"transitionLabel", "b" },
                {"endState", state22 }
            };
            state23.addConnection(connection232);
            Dictionary<string, object> connection242 = new Dictionary<string, object>()
            {
                {"transitionLabel", "b" },
                {"endState", state22 }
            };
            state24.addConnection(connection242);

            automaton2.addState(state21);
            automaton2.addState(state22);
            automaton2.addState(state23);
            automaton2.addState(state24);
            automaton.addFinal(state22);

            Dictionary<string, object> input2 = new Dictionary<string, object>()
            {
                { "automaton", automaton2},
                {"tree", node21 }
            };

            this.connectionInput = new List<Dictionary<string, object>>()
            {
                input1, input2
            };
        }

        private void getRegexTree()
        {
            List<Node> regexTree = new List<Node>();
            regexTree = new List<Node>();
            Node tree = new Node(".");
            tree.LeftChild = new Node("a");
            tree.RightChild = new Node("b");
            regexTree.Add(tree);
            tree = new Node("|");
            tree.LeftChild = new Node("*");
            tree.LeftChild.LeftChild = new Node("|");
            tree.LeftChild.LeftChild.LeftChild = new Node("a");
            tree.LeftChild.LeftChild.RightChild = new Node("d");
            tree.RightChild = new Node(".");
            tree.RightChild.LeftChild = new Node("b");
            tree.RightChild.RightChild = new Node("c");
            regexTree.Add(tree);
            this.regexData = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    {"regexInput", ".(a,b)" },
                    {"expectedTree", regexTree[0] }
                },
                new Dictionary<string, object>()
                {
                    {"regexInput", "|(*(|(a,d)),.(b,c))" },
                    {"expectedTree", regexTree[1] }
                }
            };
        }

        [TestMethod()]
        public void createConnectionsTest()
        {
            foreach(Dictionary<string, object> connectionData in connectionInput)
            {
                RegexConverter regexConverter = new RegexConverter();
                int stateLabel = 1;
                State startState = new State(stateLabel.ToString(), false);
                stateLabel++;
                State endState = new State(stateLabel.ToString(), true);
                stateLabel++;
                Automaton automaton = regexConverter.getAutomaton();
                automaton.Alphabet = "ab";
                automaton.addState(startState);
                automaton.addState(endState);
                automaton.addFinal(endState);
                Node tree = (Node)connectionData["tree"];
                Automaton expectedAutomaton = (Automaton)connectionData["automaton"];
                regexConverter.createConnections(startState, endState, tree, stateLabel);
                Assert.IsTrue(expectedAutomaton == automaton);
            }
        }

        bool matchTree(Node tree1, Node tree2)
        {
            if (tree1 == null && tree2 == null){
                return true;
            }
            if (tree1 != null && tree2 != null)
            {
                if (tree1.Symbol != tree2.Symbol)
                {
                    return false;
                }
                return this.matchTree(tree1.LeftChild, tree2.LeftChild) &&
                    this.matchTree(tree1.RightChild, tree2.RightChild);
            }
            return false;
        }

        [TestMethod()]
        public void buildRegexTreeTest()
        {
            foreach(Dictionary<string, object> data in regexData)
            {
                string regexInput = (string)data["regexInput"];
                Node expectedTree = (Node)data["expectedTree"];
                RegexConverter regexConverter = new RegexConverter();
                Node tree = regexConverter.buildRegexTree(regexInput);
                Assert.IsTrue(this.matchTree(expectedTree, tree));
            }
        }

        //[TestMethod()]
        //public void generateAutomatonFileTest()
        //{
        //    Assert.Fail();
        //}
    }
}