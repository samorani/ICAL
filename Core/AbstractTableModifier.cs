using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;

namespace DataSupport
{
    public abstract class AbstractTableModifier
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AbstractTableModifier"/> is initialized.
        /// </summary>
        /// <value><c>true</c> if initialized; otherwise, <c>false</c>.</value>
        protected bool Initialized { private set; get; }

        /// <summary>
        /// Gets or sets the silenced attributes.
        /// </summary>
        /// <value>The silenced attributes.</value>
        private HashSet<string> SilencedAttributes { set; get; }

        public AbstractTableModifier()
        {
            SilencedAttributes = new HashSet<string>();
        }

        public Table Modify(Table t)
        {
            if (!Initialized)
            {
                Initialize(t.Columns);
                Initialized = true;
            }
            return ModifyLogic(t, SilencedAttributes);
        }

        /// <summary>
        /// Modifies the table by returning another table.
        /// </summary>
        /// <param name="t">The table.</param>
        /// <param name="silencedAttributes">The attributes NOT to produce.</param>
        /// <returns>Table.</returns>
        protected abstract Table ModifyLogic(Table t, HashSet<string> silencedAttributes);

        /// <summary>
        /// Initializes the object given the specified columns. This method is called the first time that Modify is called.
        /// </summary>
        /// <param name="columns">The columns.</param>
        protected abstract void Initialize(List<Column> columns);

        /// <summary>
        /// Silences the attribute. Call this method to prevent a certain attribute to be computed
        /// </summary>
        /// <param name="att">The att.</param>
        public void SilenceAttribute(string att)
        {
            SilencedAttributes.Add(att);
        }
    }
}
