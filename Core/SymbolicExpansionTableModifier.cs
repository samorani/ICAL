using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCalc;

namespace DataSupport
{
    public class SymbolicExpansionTableModifier : AbstractTableModifier
    {
        List<string> Formulas = new List<string>();
        SortedList<string, Expression> _attributeExpressions = new SortedList<string, Expression>();
        StandardizerTableModifier _std;

        public SymbolicExpansionTableModifier(bool standardize)
        {
            _std = standardize ? new StandardizerTableModifier() : null;
            Formulas.Add("x/x");
            Formulas.Add("x*x");
            Formulas.Add("x-x");
            Formulas.Add("x+x");
            Formulas.Add("x*x/x");
            Formulas.Add("(x+x)/x");
            Formulas.Add("(x-x)/x");
            Formulas.Add("x/(x+x)");
            Formulas.Add("x/(x-x)");
        }

        protected override void Initialize(List<Column> columns)
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

        protected override Table ModifyLogic(Table t, HashSet<string> silencedAttributes)
        {
            // copy t into t2
            Table t2 = new Table();
            foreach (Column c in t.Columns)
                t2.AddColumn(new Column(c.Name, c.Dimension, c.ColumnType));
            foreach (Row r in t.Rows)
                t2.AddRow(new Row(t2.Columns));
            for (int i = 0; i < t2.Rows.Count; i++)
                foreach (Column c in t2.Columns)
                    t2[i, c] = t[i, c.Name];

            // compute each expression for each row
            DateTime now = DateTime.Now;
            foreach (KeyValuePair<string, Expression> kv in _attributeExpressions)
            {
                if (silencedAttributes.Contains(kv.Key))
                    continue;
                Column nc = new Column(kv.Key, "", ColumnType.Numeric); // TODO: fix units of measurement
                t2.AddColumn(nc);
                List<string> parameters = new List<string>(kv.Value.Parameters.Keys);
                foreach (Row r in t2.Rows)
                {
                    foreach (string c in parameters)
                        kv.Value.Parameters[c] = r[c];
                    var v = kv.Value.Evaluate();
                    double val = (double)v;
                    // if you find a null value, set val to 0
                    if (Double.IsNaN(val) || Double.IsInfinity(val))
                        val = 0;
                    r[nc] = val;
                }
            }
            // remove columns of t2 that appear in ZeroColumns
            foreach (Column c in new List<Column>(t2.Columns))
                if (silencedAttributes.Contains(c.Name))
                    t2.RemoveColumn(c);

            if (_std != null)
                return _std.Modify(t2);
            else
                return t2;
        }

        private IEnumerable<List<Column>> Permutations(List<Column> current, List<Column> availableColumns, int toSelect)
        {
            if (current.Count == toSelect)
                yield return new List<Column>(current);
            else
            {
                int n = availableColumns.Count;
                for (int i = 0; i < n; i++)
                {
                    Column c = availableColumns[i];
                    current.Add(c);
                    availableColumns.RemoveAt(i);
                    foreach (List<Column> perm in Permutations(current, availableColumns, toSelect))
                        yield return perm;
                    current.RemoveAt(current.Count - 1);
                    availableColumns.Insert(i, c);
                }
            }
        }
    }
}
