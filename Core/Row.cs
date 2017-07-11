using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSupport
{
    /// <summary>
    /// Class Row. It is a Dictionary of (Column, value) pairs
    /// </summary>
    public class Row
    {
        public SortedList<Column, double> AttributeValues { get; }
        private SortedList<string, Column> _columnsByName;
        public int Count { get { return AttributeValues.Count; } }


        public Row(List<Column> columns)
        {
            AttributeValues = new SortedList<Column, double>();
            _columnsByName = new SortedList<string, Column>();
            foreach (Column c in columns)
                AddAttribute(c, double.NaN);
        }

        public void AddAttribute(Column c, double v)
        {
            AttributeValues.Add(c, v);
            _columnsByName.Add(c.Name, c);
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Double"/> with the specified column.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns>System.Double.</returns>
        public double this[Column c]
        {
            get { return AttributeValues[c]; }
            set
            {
                AttributeValues[c] = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Double"/> with the specified column.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.Double.</returns>
        public double this[string name]
        {
            get { return AttributeValues[_columnsByName[name]]; }
            set
            {
                AttributeValues[_columnsByName[name]] = value;
            }
        }

        public override string ToString()
        {
            string s = "";
            foreach (string c in _columnsByName.Keys)
                s += c + ":" + this[c] + ", ";
            return s;
        }

        internal void RemoveColumn(Column c)
        {
            this.AttributeValues.Remove(c);
            this._columnsByName.Remove(c.Name);
        }
    }
}
