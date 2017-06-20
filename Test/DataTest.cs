using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core;
using CCP;
using System.Collections.Generic;
using System.Diagnostics;
using Pandas;

namespace Test
{
    [TestClass]
    public class DataTest
    {
        [TestMethod]
        public void TestTable()
        {
            List<Column> cols = new List<Column>();
            cols.Add(new Column("w", "lb", ColumnType.Numeric));
            cols.Add(new Column("p", "$", ColumnType.Numeric));
            Table t = new Table(cols);

            Row r = new Row(cols);
            r["p"] = 10;
            r["w"] = -1;
            Assert.AreEqual(10, r["p"]);
            Assert.AreEqual(-1, r["w"]);
            Console.WriteLine(r);

            t.AddRow(r);
            Assert.AreEqual(10, t[0,"p"]);
            Assert.AreEqual(-1, t[0, "w"]);

            // create and add another row
            Row r1 = new Row(cols);
            r1["p"] = 12;
            r1["w"] = 3;
            Assert.AreEqual(12, r1["p"]);
            Assert.AreEqual(3, r1["w"]);
            Console.WriteLine(r1);

            t.AddRow(r1);
            Assert.AreEqual(10, t[0, "p"]);
            Assert.AreEqual(-1, t[0, "w"]);
            Assert.AreEqual(12, t[1, "p"]);
            Assert.AreEqual(3, t[1, "w"]);

            // add attribute
            t.AddColumn(new Column("p/w", "", ColumnType.None));
            foreach (Row row in t.Rows)
                row["p/w"] = row["p"] / row["w"];
            Console.WriteLine(t);

            Console.WriteLine(t);
        }
    }
}
