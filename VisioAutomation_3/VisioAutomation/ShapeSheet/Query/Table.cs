﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VisioAutomation.ShapeSheet.Query
{

    /// <summary>
    /// Used to store the output of the QueryRows and QueryCells methods. Stores a string for the formula and a typed value (int|bool|double|string) for the result.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Table<T>
    {
        private readonly T[,] _values;
        private readonly TableRowList<T> _rows;
        private readonly TableColumnList<T> _cols;

        public ReadOnlyCollection<TableRowGroup> Groups { get; private set; }

        internal Table(int rows, int cols, ReadOnlyCollection<TableRowGroup> groups) :
            this( rows, cols, groups, new T[rows,cols])
        {
        }

        internal Table(int rows, int cols, ReadOnlyCollection<TableRowGroup> groups, T[,] vals)
        {
            this._values = vals;
            this.Groups = groups;
            this._rows = new TableRowList<T>(this,rows);
            this._cols = new TableColumnList<T>(this, cols);
        }

        public T this[int row, int column]
        {
            get { return this._values[row, column]; }
            set { this._values[row, column] = value; }
        }

        public T this[int row, QueryColumn column]
        {
            get
            {
                if (column==null)
                {
                    throw new System.ArgumentNullException("column");
                }
                return this._values[row, column.Ordinal];
            }
            set { this._values[row, column.Ordinal] = value; }
        }

        public TableRowList<T> Rows
        {
            get { return _rows; }
        }

        public TableColumnList<T> Columns
        {
            get { return this._cols; }
        }

    }
}