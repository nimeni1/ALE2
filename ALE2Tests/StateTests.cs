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
    public class StateTests
    {
        State state;

        [TestInitialize]
        public void TestInitialize()
        {
            this.getState();
        }

        private void getState()
        {
            state = new State("0", false);
            State state1 = new State("1", false);
            State state2 = new State("2", false);
            State state3 = new State("3", false);

            Dictionary<string, object> connection01 = new Dictionary<string, object>()
            {
                {"transitionLabel", "a" },
                {"endState", state1 }
            };
            state.addConnection(connection01);

            Dictionary<string, object> connection02 = new Dictionary<string, object>()
            {
                {"transitionLabel", "b" },
                {"endState", state2 }
            };
            state.addConnection(connection02);

            Dictionary<string, object> connection13 = new Dictionary<string, object>()
            {
                {"transitionLabel", "a" },
                {"endState", state3 }
            };
            state1.addConnection(connection13);
        }

        [TestMethod()]
        public void determineSilentTreeTest()
        {
            List<State> expectedSilentTree = new List<State>();
            List<State> actualSilentTree = state.determineSilentTree();
            Assert.AreEqual(expectedSilentTree.Count, actualSilentTree.Count);
        }
    }
}