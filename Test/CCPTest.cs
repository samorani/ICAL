using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core;
using CCP;
using System.Collections.Generic;
using System.Diagnostics;

namespace Test
{
    [TestClass]
    public class CCPTest
    {
        [TestMethod]
        public void CPPBuildAttributesAndSelectActions()
        {
            CCP_ProblemInstance inst = new CCP_ProblemInstance(4, 2, new double[,] { { 0, 10, 3, 5 },
            {10,0,5,6 }, {3,5,0,20 }, {5,6,20,0 } }, new double[] { 1, 2, 2, 1 }, 2, 4, "noname");

            Assert.AreEqual(20, inst.c[2, 3]);
            Assert.AreEqual(10, inst.c[1, 0]);
            Assert.AreEqual(6, inst.c[1, 3]);
            Assert.AreEqual(2, inst.L);
            Assert.AreEqual(4, inst.U);
            Assert.AreEqual(4, inst.n);
            Assert.AreEqual(2, inst.p);
            Assert.AreEqual(2, inst.w[1]);

            CCP_ProblemSolution sol = new CCP_ProblemSolution(inst);
            Assert.AreEqual(0, sol.Value);
            Assert.AreEqual(-1, sol.X[1]);
            Assert.AreEqual(0, sol.ObjectsInCluster[1].Count);

            // check the current options
            List<CCP_Action> options = new List<CCP_Action>(sol.GetFeasibleActions());
            Assert.AreEqual(8, options.Count);

            // what if we assign object 0 to cluster 0?
            CCP_Action a = new CCP_Action(0, 0);
            SortedList<string, double> attr = sol.GetAttributesOfAction(a);
            Assert.AreEqual(0, attr["curval"]);
            Assert.AreEqual(0, attr["newval"]);

            // do it
            CCP_ProblemSolution sol2 = sol.ChooseAction(a);
            Assert.AreEqual(0, sol2.Value);
            Assert.AreEqual(1, sol2.CurWeights[0]);
            Assert.AreEqual(-1, sol2.X[1]);
            Assert.AreEqual(0, sol2.X[0]);
            Assert.AreEqual(1, sol2.ObjectsInCluster[0].Count);
            Assert.IsTrue(sol2.ObjectsInCluster[0].Contains(0));

            // what if we assign object 2 to cluster 1?
            a = new CCP_Action(1, 2);
            attr = sol2.GetAttributesOfAction(a);
            Assert.AreEqual(0, attr["curval"]);
            Assert.AreEqual(0, attr["newval"]);

            // do it
            sol2 = sol2.ChooseAction(a);
            Assert.AreEqual(0, sol2.Value);
            Assert.AreEqual(1, sol2.CurWeights[0]);
            Assert.AreEqual(2, sol2.CurWeights[1]);
            Assert.AreEqual(0, sol2.X[0]);
            Assert.AreEqual(1, sol2.X[2]);
            Assert.AreEqual(1, sol2.ObjectsInCluster[0].Count);
            Assert.AreEqual(1, sol2.ObjectsInCluster[1].Count);
            Assert.IsTrue(sol2.ObjectsInCluster[0].Contains(0));
            Assert.IsTrue(sol2.ObjectsInCluster[1].Contains(2));

            // what if we assign object 3 to cluster 1?
            a = new CCP_Action(1, 3);
            attr = sol2.GetAttributesOfAction(a);
            Assert.AreEqual(0, attr["curval"]);
            Assert.AreEqual(20, attr["newval"]);

            // do it
            sol2 = sol2.ChooseAction(a);
            Assert.AreEqual(20, sol2.Value);
            Assert.AreEqual(1, sol2.CurWeights[0]);
            Assert.AreEqual(3, sol2.CurWeights[1]);
            Assert.AreEqual(0, sol2.X[0]);
            Assert.AreEqual(1, sol2.X[2]);
            Assert.AreEqual(1, sol2.X[3]);
            Assert.AreEqual(1, sol2.ObjectsInCluster[0].Count);
            Assert.AreEqual(2, sol2.ObjectsInCluster[1].Count);
            Assert.IsTrue(sol2.ObjectsInCluster[0].Contains(0));
            Assert.IsTrue(sol2.ObjectsInCluster[1].Contains(2));
            Assert.IsTrue(sol2.ObjectsInCluster[1].Contains(3));

            // check the current options. I can only put 1 into cluster 0
            options = new List<CCP_Action>(sol2.GetFeasibleActions());
            Assert.AreEqual(1, options.Count);
            Assert.AreEqual(0, options[0].Cluster);
            Assert.AreEqual(1, options[0].Object);

        }
        [TestMethod]
        public void CCPTestSequences()
        {

            CCP_ProblemInstance inst = new CCP_ProblemInstance(4, 2, new double[,] { { 0, 10, 3, 5 },
            {10,0,5,6 }, {3,5,0,20 }, {5,6,20,0 } }, new double[] { 1, 2, 2, 1 }, 2, 4, "noname");
            CCP_ProblemSolution sol = new CCP_ProblemSolution(inst);
            CCP_ProblemSolution target = new CCP_ProblemSolution(inst, new int[] {0,0,1,1 } );
            Assert.AreEqual(3, target.CurWeights[0]);
            Assert.AreEqual(3, target.CurWeights[1]);
            Assert.AreEqual(30, target.Value);
            Assert.IsTrue(target.ObjectsInCluster[0].Contains(0));
            Assert.IsTrue(target.ObjectsInCluster[0].Contains(1));
            Assert.IsTrue(target.ObjectsInCluster[1].Contains(2));
            Assert.IsTrue(target.ObjectsInCluster[1].Contains(3));

            // build sequences
            List<Sequence<CCP_ProblemSolution, CCP_ProblemInstance, CCP_Action>> sequences =
                new List<Sequence<CCP_ProblemSolution, CCP_ProblemInstance, CCP_Action>>(inst.SequencesThatMayBuild(target));
            foreach (Sequence<CCP_ProblemSolution, CCP_ProblemInstance, CCP_Action> s in sequences)
            {
                foreach (CCP_Action o in s.Actions)
                    Debug.Write(o.Object  +" into " + o.Cluster +", ");
                Debug.WriteLine("");
            }
            Assert.AreEqual(48, sequences.Count);
        }
    }
}
