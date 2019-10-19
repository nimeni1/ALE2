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
    public class SetStateTests
    {
        SetState setSubState;
        SetState setSuperState;

        [TestInitialize]
        public void TestInitialize()
        {
            List<State> listStates1 = new List<State>();
            State state1 = new State("a", false);
            State state2 = new State("b", false);
            listStates1.Add(state1);
            listStates1.Add(state2);
            setSubState = new SetState("ab", false, listStates1);

            State state3 = new State("c", true);
            List<State> listStates2 = new List<State>();
            listStates2.Add(state1);
            listStates2.Add(state2);
            listStates2.Add(state3);
            setSuperState = new SetState("abc", true, listStates2);
        }

        [TestMethod()]
        public void isSubsetTest()
        {
            Assert.IsTrue(setSubState.isSubset(setSuperState));
        }
    }
}