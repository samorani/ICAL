using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using NCalc;
    
namespace DataSupport
{
    /// <summary>
    /// TODO: The generation of formulas must be executed only once. Then, I store the instructions to compute the attributes. 
    /// Then, I need to implement a method to compute a Table with just the attributes whose coefficient is non-zero. 
    /// </summary>
    public class AttributeExpander
    {
        /// <summary>
        /// The formulas (num, denom)
        /// </summary>
        List<string> Formulas = new List<string>();
        public HashSet<string> ZeroColumns;
        SortedList<string, Expression> _attributeExpressions = new SortedList<string, Expression>();

        public AttributeExpander()
        {
            Formulas.Add("x/x");
            Formulas.Add("x*x");
            Formulas.Add("x-x");
            Formulas.Add("x+x");
            Formulas.Add("x*x/x");
            Formulas.Add("(x+x)/x");
            Formulas.Add("x/(x+x)");
            ZeroColumns = new HashSet<string>();
        }


        /// <summary>
        /// Builds the attribute expressions from the given columns. 
        /// </summary>
        /// <param name="columns">The columns.</param>
        public void BuildAttributeExpressions(List<Column> columns)
        {
            _attributeExpressions = new SortedList<string, Expression>();
            // find all expressions to build and put them in toAdd
            foreach (string f in Formulas)
            {
                // count the x's
                int xCount = f.Count(x => x == 'x');
                foreach (List<Column> permutation in Permutations(new List<Column>(), new List<Column>(columns), xCount))
                {
                    // substitute the x's in order
                    string actualF = "";
                    int currentSymbol = 0;
                    string lastDim = "------";
                    bool mustSkip = false;
                    for (int i = 0; i < f.Length; i++)
                    {
                        if (f[i] == 'x')
                        {
                            Column thisCol = permutation[currentSymbol++];
                            // check dimension:
                            if (i >= 2 && f[i - 2] == 'x' && (f[i - 1] == '+' || f[i - 1] == '-' && lastDim != thisCol.Dimension))
                            {
                                mustSkip = true;
                                break;
                            }
                            actualF += "[" + thisCol.Name + "]";
                            lastDim = thisCol.Dimension;
                        }
                        else
                            actualF += f[i];
                    }
                    if (mustSkip)
                        continue;

                    //Console.WriteLine(actualF);
                    Expression e = new Expression(actualF);
                    foreach (Column c in permutation)
                        e.Parameters[c.Name] = 0;
                    _attributeExpressions.Add(actualF, e);
                }
            }

        }

        public Table ExpandAttributes(Table t1)
        {
            // compute each expression for each row
            DateTime now = DateTime.Now;
            foreach (KeyValuePair<string,Expression> kv in _attributeExpressions)
            {
                if (ZeroColumns.Contains(kv.Key))
                    continue;
                Column nc = new Column(kv.Key, "", ColumnType.Numeric); // TODO: fix units of measurement
                t1.AddColumn(nc);
                List<string> parameters = new List<string>(kv.Value.Parameters.Keys);
                foreach (Row r in t1.Rows)
                {
                    foreach (string c in parameters)
                        kv.Value.Parameters[c] = r[c];
                    var v = kv.Value.Evaluate();
                    double val = (double)v;
                    r[nc] = val;
                }
            }
            // remove columns of t1 that appear in ZeroColumns
            foreach (Column c in new List<Column>(t1.Columns))
                if (ZeroColumns.Contains(c.Name))
                    t1.RemoveColumn(c);

            // find Max and Min in each column of t1
            SortedList<Column, double> Max = new SortedList<Column, double>(); // max in each col
            SortedList<Column, double> Min = new SortedList<Column, double>(); // min in each col
            foreach (Column c in t1.Columns)
            {
                Max.Add(c, Double.MinValue);
                Min.Add(c, Double.MaxValue);
                foreach (Row r in t1.Rows)
                {
                    if (r[c] > Max[c])
                        Max[c] = r[c];
                    if (r[c] < Min[c])
                        Min[c] = r[c];
                }
            }
            //Console.Write("Compute expressions: " + Math.Round((DateTime.Now - now).TotalMilliseconds, 0) + "\t");

            // binarize the data set. Each attribute is whether that row maximizes it
            now = DateTime.Now;
            Table toReturn = new Table(new List<Column>());
            foreach (Row r in t1.Rows)
                toReturn.AddRow(new Row(new List<Column>()));
            foreach (Column c in t1.Columns)
            {
                // add two columns to toReturn (min and max)
                Column minC = new Column("min " + c.Name, "", ColumnType.Bool);
                Column maxC = new Column("max " + c.Name, "", ColumnType.Bool);
                toReturn.AddColumn(minC);
                toReturn.AddColumn(maxC);
                for (int i = 0; i < t1.Rows.Count; i++)
                {
                    // min
                    toReturn[i, minC] = 0;
                    if (t1[i, c] == Min[c])
                        toReturn[i, minC] = 1;

                    // max
                    toReturn[i, maxC] = 0;
                    if (t1[i, c] == Max[c])
                        toReturn[i, maxC] = 1;
                }
            }
            //Console.Write("Binarize: " + Math.Round((DateTime.Now - now).TotalMilliseconds, 0) + "\t");
            //Console.WriteLine(toReturn);

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
