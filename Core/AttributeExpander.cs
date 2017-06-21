using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using NCalc;
    
namespace DataSupport
{
    public class AttributeExpander
    {
        /// <summary>
        /// The formulas (num, denom)
        /// </summary>
        List<string> Formulas = new List<string>();

        public AttributeExpander()
        {
            //Formulas.Add("x");
            Formulas.Add("x*x");
            Formulas.Add("(x+x)/x");
        }
        public Table ExpandAttributes(Table t1)
        {
            List<Column> originalColumns = new List<Column>(t1.Columns);
            foreach (string f in Formulas)
            {
                // count the x's
                int xCount = f.Count(x => x == 'x');
                foreach (List<Column> permutation in Permutations(new List<Column>(),new List<Column>(originalColumns), xCount))
                {
                    // substitute the x's in order
                    string actualF = "";
                    int currentSymbol = 0;
                    for (int i = 0; i < f.Length; i++)
                        if (f[i] == 'x')
                            actualF += permutation[currentSymbol++].Name;
                        else
                            actualF += f[i];

                    Console.WriteLine(actualF);
                    Expression e = new Expression(actualF);
                    
                    // compute it for each row
                    Column nc = new Column(actualF, "", ColumnType.Numeric); // TODO: fix units of measurement
                    t1.AddColumn(nc);
                    foreach (Row r in t1.Rows)
                    {
                        foreach (Column c in permutation)
                            e.Parameters[c.Name] = r[c];
                        var v = e.Evaluate();
                        r[nc] = (double) v;
                    }
                    Console.WriteLine(t1);

                    //Console.ReadLine();


                }
            }
            // binarize the data set. Each attribute is whether that row maximizes it
            Table toReturn = new Table(new List<Column>());
            foreach (Row r in t1.Rows)
                toReturn.AddRow(new Row(new List<Column>()));
            foreach (Column c in t1.Columns)
            {
                double max = Double.MinValue;
                int maximizer = -1;
                for (int i=0;i<t1.Rows.Count;i++)
                {
                    double val = t1[i, c];
                    if (val == max) // ties don't count as maximizers
                    {
                        maximizer = -1;
                    }
                    if (val > max)
                    {
                        max = val;
                        maximizer = i;
                    }
                }
                // add one column to toReturn
                Column maxC = new Column("max " + c.Name, "", ColumnType.Bool);
                toReturn.AddColumn(maxC);
                for (int i = 0; i < t1.Rows.Count; i++)
                {
                    toReturn[i, maxC] = 0;
                    if (i == maximizer)
                        toReturn[i, maxC] = 1;
                }
            }

            return toReturn;
        }

        private IEnumerable<List<Column>> Permutations(List<Column> current, List<Column> availableColumns, int toSelect)
        {
            if (current.Count == toSelect)
                yield return new List<Column>( current);
            else
            {
                int n = availableColumns.Count;
                for (int i=0;i<n;i++)
                {
                    Column c = availableColumns[i];
                    current.Add(c);
                    availableColumns.RemoveAt(i);
                    foreach (List<Column> perm in Permutations(current, availableColumns,toSelect))
                        yield return perm;
                    current.RemoveAt(current.Count - 1);
                    availableColumns.Insert(i, c);
                }
            }
        }
    }
}
