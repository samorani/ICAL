using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILOG.CPLEX;
using Core;
using ILOG.Concert;

namespace KP
{
    public class KP_CPlex_solver : ISolver<KP_ProblemSolution, KP_ProblemInstance, KP_Action>
    {

        public KP_ProblemSolution Solve(KP_ProblemInstance instance)
        {
            int n = instance.N;
            double c = instance.C;
            double[] w = instance.W;
            double[] p = instance.P;
            Cplex model = new Cplex();
            IIntVar[] x = new IIntVar[n];
            for (int i = 0; i < n; i++)
                x[i] = model.BoolVar("x_" + i);

            // obj
            ILinearNumExpr expr = model.LinearNumExpr();
            for (int i = 0; i < n; i++)
                expr.AddTerm(p[i], x[i]);
            model.AddMaximize(expr);

            // capacity constraint
            expr = model.LinearNumExpr();
            for (int i = 0; i < n; i++)
                expr.AddTerm(w[i], x[i]);
            model.AddLe(expr,c);

            model.Solve();

            bool[] boolX = new bool[n];
            for (int i = 0; i < n; i++)
                boolX[i] = model.GetValue(x[i]) > .999;
            KP_ProblemSolution sol = new KP_ProblemSolution(instance, boolX);
            return sol;
        }
    }
}
