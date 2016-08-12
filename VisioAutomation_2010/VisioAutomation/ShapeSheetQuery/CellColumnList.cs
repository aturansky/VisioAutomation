﻿using System.Collections.Generic;

namespace VisioAutomation.ShapeSheetQuery
{
    public class CellColumnList : IEnumerable<CellColumn>
    {
        private readonly IList<CellColumn> _items;
        private readonly Dictionary<string, CellColumn> _dic_columns;
        private HashSet<ShapeSheet.SRC> _src_set;

        internal CellColumnList() :
            this(0)
        {
        }

        internal CellColumnList(int capacity)
        {
            this._items = new List<CellColumn>(capacity);
            this._dic_columns = new Dictionary<string, CellColumn>(capacity);
        }

        public IEnumerator<CellColumn> GetEnumerator()
        {
            return (this._items).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public CellColumn this[int index] => this._items[index];

        public CellColumn this[string name] => this._dic_columns[name];

        public bool Contains(string name) => this._dic_columns.ContainsKey(name);

        internal CellColumn Add(ShapeSheet.SRC src) => this.Add(src, null);

        internal CellColumn Add(ShapeSheet.SRC src, string name)
        {
            name = this.fixup_name(name);

            if (this._dic_columns.ContainsKey(name))
            {
                throw new AutomationException("Duplicate Column Name");
            }

            if (this._src_set == null)
            {
                this._src_set = new HashSet<ShapeSheet.SRC>();
            }

            if (this._src_set.Contains(src))
            {
                string msg = "Duplicate SRC";
                throw new AutomationException(msg);
            }

            int ordinal = this._items.Count;
            var col = new CellColumn(ordinal, src, name);
            this._items.Add(col);

            this._dic_columns[name] = col;
            this._src_set.Add(src);
            return col;
        }

        private string fixup_name(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = string.Format("Col{0}", this._items.Count);
            }
            return name;
        }

        public int Count => this._items.Count;
    }

    public class SubQueryCellColumnList : IEnumerable<SubQueryCellColumn>
    {
        private readonly IList<SubQueryCellColumn> _items;
        private readonly Dictionary<string, SubQueryCellColumn> _dic_columns;
        private HashSet<ShapeSheet.SRC> _src_set;
        private HashSet<short> _cellindex_set;

        internal SubQueryCellColumnList() :
            this(0)
        {
        }

        internal SubQueryCellColumnList(int capacity)
        {
            this._items = new List<SubQueryCellColumn>(capacity);
            this._dic_columns = new Dictionary<string, SubQueryCellColumn>(capacity);
        }

        public IEnumerator<SubQueryCellColumn> GetEnumerator()
        {
            return (this._items).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public SubQueryCellColumn this[int index] => this._items[index];

        public SubQueryCellColumn this[string name] => this._dic_columns[name];

        public bool Contains(string name) => this._dic_columns.ContainsKey(name);

        public SubQueryCellColumn Add(short cell, string name)
        {
            if (this._cellindex_set == null)
            {
                this._cellindex_set = new HashSet<short>();
            }

            if (this._cellindex_set.Contains(cell))
            {
                string msg = "Duplicate Cell Index";
                throw new AutomationException(msg);
            }

            name = this.fixup_name(name);
            int ordinal = this._items.Count;
            var col = new SubQueryCellColumn(ordinal, cell, name);
            this._items.Add(col);
            this._cellindex_set.Add(cell);
            return col;
        }

        private string fixup_name(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = string.Format("Col{0}", this._items.Count);
            }
            return name;
        }

        public int Count => this._items.Count;
    }

}