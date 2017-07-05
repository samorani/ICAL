using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSupport
{
    /// <summary>
    /// Modifies a table by standardizing the attributes. a = (a - mean(a)) / stdDev(a)
    /// </summary>
    /// <seealso cref="DataSupport.AbstractTableModifier" />
    public class StandardizerTableModifier : AbstractTableModifier
    {
        private Dictionary<string, string> old_to_newname = new Dictionary<string, string>();
        private Dictionary<string, string> new_to_oldname = new Dictionary<string, string>();

        protected override void Initialize(List<Column> columns)
        {
            old_to_newname = new Dictionary<string, string>();
            new_to_oldname = new Dictionary<string, string>();
            foreach (Column c in columns)
            {
                old_to_newname.Add(c.Name, c.Name);
                new_to_oldname.Add(c.Name, c.Name);
            }
        }

        protected override Table ModifyLogic(Table t, HashSet<string> silencedAttributes)
        {
            Table t2 = null;
            List<Column> toProduce = new List<Column>();
            foreach (Column c in t.Columns)
                if (!silencedAttributes.Contains(old_to_newname[c.Name]))
                    toProduce.Add(new Column( old_to_newname[c.Name],c.Dimension,c.ColumnType));

            t2 = new Table(toProduce);
            foreach (Row r in t.Rows)
                t2.AddRow(new Row(toProduce));

            // queue1 contains the lists of columns
            var queue1 = new BlockingCollection<Column>();
            foreach (Column c in toProduce)
                queue1.Add(c);
            queue1.CompleteAdding();

            // here, all the threads will deposit the output: a tuple mean and stdev
            var queue2 = new BlockingCollection<Tuple<Column, double, double>>(100000);

            // producers get an instance from queue1, solve it, and add them to queue2
            var producers = Enumerable.Range(1, 10).Select(_ => Task.Factory.StartNew(() =>
            {
                foreach (Column c in queue1.GetConsumingEnumerable())
                {
                    double sum = 0;
                    foreach (double d in t[t[new_to_oldname[c.Name]]])
                        sum += d;
                    double n = t[t[new_to_oldname[c.Name]]].Count;
                    double mean = sum / n;
                    double stdev = 0;
                    foreach (double d in t[t[new_to_oldname[c.Name]]])
                        stdev += Math.Pow(d - mean,2);
                    stdev = Math.Sqrt(stdev / n);
                    queue2.Add(new Tuple<Column, double, double>(c,mean, stdev));
                }
            })).ToArray();

            // a single consumer gets the solutions from queue2 and adds them to the dictionaries
            var consumers = Enumerable.Range(1, 1).Select(_ => Task.Factory.StartNew(() =>
            {
                // standardize each column
                foreach (Tuple<Column, double, double> dd in queue2.GetConsumingEnumerable())
                {
                    Column c = dd.Item1;
                    double mean = dd.Item2;
                    double stdev = dd.Item3;
                    for (int i = 0; i < t.Rows.Count; i++)
                        t2[i, c] = stdev == 0? 0 : (t[i, t[new_to_oldname[c.Name]]] - mean) / stdev;
                }
            })).ToArray();

            Task.WaitAll(producers);
            queue2.CompleteAdding();
            Task.WaitAll(consumers);

            // if there is one row only, add some value
            if (t2.Rows.Count == 1)
                foreach (Column c in t2.Columns)
                    t2[0, c] = 0.0001;

            return t2;
        }
    }
}
