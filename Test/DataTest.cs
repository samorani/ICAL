using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core;
using CCP;
using System.Collections.Generic;
using System.Diagnostics;
using DataSupport;

namespace Test
{
    [TestClass]
    public class DataTest
    {
        [TestMethod]
        public void TestStandardizer()
        {
            List<Column> cols = new List<Column>();
            cols.Add(new Column("a1", "lb", ColumnType.Numeric));
            cols.Add(new Column("a2", "lb", ColumnType.Numeric));
            cols.Add(new Column("a3", "lb", ColumnType.Numeric));
            Table t = new Table(cols);

            Row r = new Row(cols);
            r["a1"] = 10;
            r["a2"] = 0;
            r["a3"] = 0;
            t.AddRow(r);
            r = new Row(cols);
            r["a1"] = 10;
            r["a2"] = 5;
            r["a3"] = -5;
            t.AddRow(r);
            r = new Row(cols);
            r["a1"] = 10;
            r["a2"] = 10;
            r["a3"] = 2;
            t.AddRow(r);

            StandardizerTableModifier st = new StandardizerTableModifier();
            Table t2 = st.Modify(t);
            Assert.AreEqual(3, t2.Columns.Count);
            Assert.AreEqual(0, t2[0, "a1"]);
            Assert.AreEqual(0, t2[1, "a1)"]);
            Assert.AreEqual(0, t2[2, "a1"]);
            Assert.IsTrue(Math.Abs(t2[0, "a2"] + 1.22474) < 0.01);
            Assert.IsTrue(Math.Abs(t2[1, "a2"] - 0) < 0.01);
            Assert.IsTrue(Math.Abs(t2[2, "a2"] - 1.22474) < 0.01);
            Assert.IsTrue(Math.Abs(t2[0, "a3"] - 0.3396) < 0.01);
            Assert.IsTrue(Math.Abs(t2[1, "a3"] + 1.35873) < 0.01);
            Assert.IsTrue(Math.Abs(t2[2, "a3"] - 1.019) < 0.01);

            // silence std(a2)
            st.SilenceAttribute("a2");
            t2 = st.Modify(t);
            Assert.AreEqual(2, t2.Columns.Count);
            Assert.AreEqual(0, t2[0, "a1"]);
            Assert.AreEqual(0, t2[1, "a1)"]);
            Assert.AreEqual(0, t2[2, "a1"]);
            Assert.IsTrue(Math.Abs(t2[0, "a3"] - 0.3396) < 0.01);
            Assert.IsTrue(Math.Abs(t2[1, "a3"] + 1.35873) < 0.01);
            Assert.IsTrue(Math.Abs(t2[2, "a3"] - 1.019) < 0.01);
        }

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
            Assert.AreEqual(10, t[0, "p"]);
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

        [TestMethod]
        public void TestSymbolicModifier()
        {
            List<Column> cols = new List<Column>();
            cols.Add(new Column("w", "lb", ColumnType.Numeric));
            cols.Add(new Column("p", "$", ColumnType.Numeric));
            cols.Add(new Column("c", "lb", ColumnType.Numeric));
            Table t = new Table(cols);
            t.AddRow(new Row(cols));
            t.AddRow(new Row(cols));
            t[0, "w"] = 3;
            t[0, "p"] = 1;
            t[0, "c"] = 3;
            t[1, "w"] = 10;
            t[1, "p"] = 5;
            t[1, "c"] = 2;
            Console.WriteLine(t);
            SymbolicExpansionTableModifier exp = new SymbolicExpansionTableModifier(false);
            Table t2 = exp.Modify(t);
            Assert.AreEqual(23, t2.Columns.Count);
            Assert.AreEqual(3, t2[0, "[c]*[p]"]);
            Assert.AreEqual(10, t2[1, "[c]*[p]"]);
            exp.SilenceAttribute("[c]*[p]");
            t2 = exp.Modify(t);
            Assert.AreEqual(22, t2.Columns.Count);
        }
    }
}