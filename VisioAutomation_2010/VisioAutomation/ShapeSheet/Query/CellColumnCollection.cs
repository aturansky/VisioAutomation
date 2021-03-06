﻿using System.Collections.Generic;

namespace VisioAutomation.ShapeSheet.Query
{
    public class CellColumnCollection : ColumnCollectionBase<CellColumn>
    {
        private HashSet<ShapeSheet.Src> items;

        internal CellColumnCollection() :
            this(0)
        {
        }

        internal CellColumnCollection(int capacity) : base(capacity)
        {
        }

        internal CellColumn Add(ShapeSheet.Src src, string name)
        {
            check_deplicate_src(src);
            string norm_name = this.normalize_name(name);
            check_duplicate_column_name(norm_name);

            int ordinal = this._items.Count;
            var col = new CellColumn(ordinal, src, norm_name);
            this._items.Add(col);

            this.map_name_to_item[norm_name] = col;
            this.items.Add(src);
            return col;
        }

        private void check_deplicate_src(Src src)
        {
            if (this.items == null)
            {
                this.items = new HashSet<ShapeSheet.Src>();
            }

            if (this.items.Contains(src))
            {
                string msg = string.Format("Duplicate {0}({1},{2},{3})", nameof(Src),src.Section, src.Row, src.Cell);
                throw new System.ArgumentException(msg);
            }
        }
    }
}