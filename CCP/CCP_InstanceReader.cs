using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using System.IO;

namespace CCP
{
    public class CCP_InstanceReader :
        Core.InstanceReader<CCP_ProblemSolution, CCP_ProblemInstance, CCP_Action>
    {
        public override CCP_ProblemInstance LoadInstanceFromFile(string filename)
        {
            StreamReader sr = new StreamReader(filename);
            CCP_ProblemInstance inst = new CCP_ProblemInstance(new FileInfo(filename).Name);
            string firstLine = sr.ReadLine();
            string[] split = firstLine.Split(new char[] { ' ' });
            if (split.Length > 1)
            {
                // long first line format
                inst.n = Int32.Parse(split[0]);
                inst.p = Int32.Parse(split[1]);
                inst.L = Double.Parse(split[3]);
                inst.U = Double.Parse(split[4]);
                int i = 5;
                for (i=5;i<100000;i++)
                {
                    if (split[i-1] == "W")
                        break;
                }
                // i is now pointing at the first weight
                inst.w = new double[inst.n];
                for (int j=0; j < inst.n; j++)
                {
                    inst.w[j] = Double.Parse(split[i]);
                    i++;
                }

                inst.c = new double[inst.n, inst.n];
                string line = sr.ReadLine();
                while (line != null)
                {
                    int i1 = Int32.Parse(line.Split(new char[] { ' ' })[0]);
                    int i2 = Int32.Parse(line.Split(new char[] { ' ' })[1]);
                    double c12 = Double.Parse(line.Split(new char[] { ' ' })[2]);
                    inst.c[i1, i2] = c12;
                    inst.c[i2, i1] = c12;
                    line = sr.ReadLine();
                }
            }
            else
            {
                // handover format
                inst.n = Int32.Parse(firstLine);
                string line = sr.ReadLine();
                inst.p = Int32.Parse(line);
                line = sr.ReadLine();
                inst.L = 0;
                inst.U = Double.Parse(line);
                inst.w = new double[inst.n];
                inst.c = new double[inst.n, inst.n];
                for (int i=0;i<inst.n;i++)
                {
                    line = sr.ReadLine();
                    inst.w[i] = Double.Parse(line);
                }
                line = sr.ReadLine();
                split = line.Split(new char[] { ' ' });
                for (int i = 0; i < inst.n; i++)
                    for (int j = 0; j < inst.n; j++)
                        inst.c[i, j] = Double.Parse(split[i * inst.n + j]);
            }
            sr.Close();
            return inst;
        }
    }
}
