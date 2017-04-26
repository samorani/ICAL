using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core;
using Problems;
using System.Collections.Generic;
using System.Diagnostics;

namespace Test
{
    [TestClass]
    public class KPTest
    {
        [TestMethod]
        public void KPBuildAttributesAndSelectOptions()
        {
            KP_ProblemInstance inst = new KP_ProblemInstance(new double[] { 1, 2, 3, 4 }, new double[] { 2, 2, 2, 1 }, 4);
            Assert.AreEqual(4, inst.C);
            Assert.AreEqual(4, inst.N);

            KP_ProblemSolution sol = new KP_ProblemSolution(inst);
            Assert.AreEqual(0, sol.CurrentValue);
            Assert.AreEqual(4, sol.RemainingCapacity);

            // check the current options
            List<KP_Option> options = new List<KP_Option>(sol.GetFeasibleOptions());
            Assert.AreEqual(4, options.Count);

            // build the option to select the third object
            KP_Option opt = new KP_Option(2);
            SortedList<string,double> attr = sol.GetAttributesOfOption(opt);
            Assert.AreEqual(2, attr["profit"]);
            Assert.AreEqual(3, attr["weight"]);
            Assert.AreEqual(2.0/3.0, attr["p/w"]);
            Assert.AreEqual(3.0/2.0, attr["w/p"]);
            Assert.AreEqual(1, attr["new remaining capacity"]);
            Assert.AreEqual(2, attr["new value"]);

            // select the third object
            KP_ProblemSolution sol2 = sol.ChooseOption(opt);
            Assert.AreEqual(2, sol2.CurrentValue);
            Assert.AreEqual(1, sol2.RemainingCapacity);
            Assert.AreEqual(true, sol2.X[2]);

            // check the current options
            options = new List<KP_Option>( sol2.GetFeasibleOptions());
            Assert.AreEqual(1, options.Count);

            // build the option to select the first object
            opt = new KP_Option(0);
            attr = sol2.GetAttributesOfOption(opt);
            Assert.AreEqual(2, attr["profit"]);
            Assert.AreEqual(1, attr["weight"]);
            Assert.AreEqual(2.0 / 1.0, attr["p/w"]);
            Assert.AreEqual(1.0 / 2.0, attr["w/p"]);
            Assert.AreEqual(0, attr["new remaining capacity"]);
            Assert.AreEqual(4, attr["new value"]);
            KP_ProblemSolution sol3 = sol2.ChooseOption(opt);

            // check the current options
            options = new List<KP_Option>(sol3.GetFeasibleOptions());
            Assert.AreEqual(0, options.Count);
        }
        [TestMethod]
        public void KPTestSequences()
        {
            KP_ProblemInstance inst = new KP_ProblemInstance(new double[] { 1, 2, 3, 4 }, new double[] { 2, 2, 2, 1 }, 6);
            KP_ProblemSolution sol = new KP_ProblemSolution(inst);
            KP_ProblemSolution target = new KP_ProblemSolution(inst, new bool[] {true,true,true,false });
            Assert.AreEqual(0, target.RemainingCapacity);
            Assert.AreEqual(6, target.CurrentValue);
            
            // build sequences
            List<Sequence<KP_ProblemSolution,KP_ProblemInstance,KP_Option>> sequences = 
                new List<Sequence<KP_ProblemSolution, KP_ProblemInstance, KP_Option>>(inst.SequencesThatMayBuild(target));
            foreach (Sequence<KP_ProblemSolution, KP_ProblemInstance, KP_Option> s in sequences)
            {
                foreach (KP_Option o in s.Options)
                    Debug.Write(o.Index + ",");
                Debug.WriteLine("");
            }
            Assert.AreEqual(6, sequences.Count);
        }
    }
}
