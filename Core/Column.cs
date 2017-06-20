using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSupport
{
    public enum ColumnType { Numeric, Bool, None};
    public struct Column : IComparable<Column>
    {
        public string Dimension { get; }
        public string Name { get; }
        public ColumnType ColumnType { get; }

        public int CompareTo(Column other)
        {
            return this.Name.CompareTo(other.Name);
        }

        public Column(string name, string dimension, ColumnType type)
        {
            Name = name;
            this.Dimension = dimension;
            ColumnType = type;
        }
    }
}
