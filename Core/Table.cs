using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSupport
{
    public class Table
    {
        public List<Column> Columns { get; }
        private SortedList<string, Column> _columnsByName;
        public List<Row> Rows { get; }


        public Table(List<Column> columns)
        {
            Columns = new List<Column>();
            Rows = new List<Row>();
            _columnsByName = new SortedList<string, Column>();
            foreach (Column c in columns)
                AddColumn(c);
        }

        public void AddRow(Row r)
        {
            foreach (KeyValuePair<Column, double> kv in r.AttributeValues)
                if (!Columns.Contains(kv.Key))
                    throw new Exception("Row " + r + " does not contain column " + kv.Key.Name);
            Rows.Add(r);
        }


        public void AddColumn (Column c)
        {
            Columns.Add(c);
            foreach (Row r in Rows)
                r.AddAttribute(c, Double.NaN);
            _columnsByName.Add(c.Name, c);
        }

        /// <summary>
        /// Gets the <see cref="Column"/> with the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>Column.</returns>
        public Column this[string columnName]
        {
            get { return _columnsByName[columnName]; }
        }

        /// <summary>
        /// Gets the <see cref="List{System.Double}"/> all of the values of the object in the specified column.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns>List&lt;System.Double&gt;.</returns>
        public List<double> this[Column c]
        {
            get
            {
                List<double> res = new List<double>();
                foreach (Row r in Rows)
                    res.Add(r.AttributeValues[c]);
                return res;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Double"/> value with the specified index and column.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="c">The c.</param>
        /// <returns>System.Double.</returns>
        public double this[int i, Column c]
        {
            get { return Rows[i][c]; }
            set { Rows[i][c] = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Double"/> value with the specified index.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.Double.</returns>
        public double this[int i, string columnName]
        {
            get { return Rows[i][columnName]; }
            set { Rows[i][columnName] = value; }
        }

        public override string ToString()
        {
            string s = "";
            foreach (Row r in Rows)
                s += r + "\n";
            return s;
        }

        public void RemoveColumn(Column c)
        {
            Columns.Remove(c);
        }
    }
}
