using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Problems;
using ILOG.CPLEX;
using ILOG.Concert;

namespace OtherSolvers
{
    /// <summary>
    /// Solves the CPP with CPlex
    /// </summary>
    public class CPP_InstanceSolver
    {
        Cplex _model;
        IIntVar[,] _x;

        public CCP_ProblemSolution Solve(CCP_ProblemInstance inst)
        {
            _model = new Cplex();

            // set up variables
            _x = new IIntVar[inst.n, inst.p];
            for (int i = 0; i < inst.n; i++)
                for (int k = 0; k < inst.p; k++)
                    _x[i, k] = _model.BoolVar("x_" + i + "_" + k);

            // objective
            IQuadNumExpr obj = _model.QuadNumExpr();
            for (int i = 0; i < inst.n; i++)
                for (int k = 0; k < inst.p; k++)
                    for (int j = i + 1; j < inst.n; j++)
                        obj.AddTerm(inst.c[i, j], _x[i, k], _x[j, k]);
            _model.AddMaximize(obj);

            // constraint 1
            for (int i = 0; i < inst.n; i++)
            {
                ILinearNumExpr c1 = _model.LinearNumExpr();
                for (int k = 0; k < inst.p; k++)
                    c1.AddTerm(1, _x[i, k]);
                _model.AddEq(1, c1, "c1_" + i);
            }

            // constraint 2
            for (int k = 0; k < inst.p; k++)
            {
                ILinearNumExpr c2 = _model.LinearNumExpr();
                for (int i = 0; i < inst.n; i++)
                    c2.AddTerm(inst.w[i], _x[i, k]);
                _model.AddLe(inst.L, c2, "c2_L_" + k);
                _model.AddLe(c2, inst.U, "c2_U_" + k);
            }

            // Solve
            _model.ExportModel("..\\..\\..\\model.lp");
            _model.Solve();

            CCP_ProblemSolution sol = new CCP_ProblemSolution(inst);
            for (int i = 0; i < inst.n; i++)
                for (int k = 0; k < inst.p; k++)
                    if (Math.Abs(_model.GetValue(_x[i,k]) - 1) < 0.0001)
                        sol = sol.ChooseAction(new CCP_Action(k, i));
            return sol;
        }
    }
}
