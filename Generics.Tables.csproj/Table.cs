using System;
using System.Collections.Generic;

namespace Generics.Tables
{
    public class Cell<TypeRows, TypeColumns, TypeCells>
    {
        public TypeRows Row;
        public TypeColumns Column;
        public TypeCells Data;

        public Cell(TypeRows pRow, TypeColumns pColumn, TypeCells pData)
        {
            Row = pRow;
            Column = pColumn;
            Data = pData;
        }
    }

    public class COpen<TypeRows, TypeColumns, TypeCells> where TypeRows : IComparable
                                                         where TypeColumns : IComparable
                                                         where TypeCells : IComparable
    {
        public Table<TypeRows, TypeColumns, TypeCells> Reference;

        public TypeCells this[TypeRows index_row, TypeColumns index_column]
        {
            set
            {
                var index_global = Reference.GetCellIndex(index_row, index_column);
                
                if (index_global == -1)
                {
                    Reference.Cells.Add(new Cell<TypeRows, TypeColumns, TypeCells>(index_row, index_column, value));

                    if (!Reference.Rows.Contains(index_row))
                    {
                        Reference.Rows.Add(index_row);
                    }
                    if (!Reference.Columns.Contains(index_column))
                    {
                        Reference.Columns.Add(index_column);
                    }
                }
                else
                {
                    Reference.Cells[index_global].Data = value;
                }
            } 
            
            get
            {
                var index_global = Reference.GetCellIndex(index_row, index_column);

                if (Reference.Columns.Contains(index_column) &&
                    Reference.Rows.Contains(index_row) &&
                    index_global != -1)
                    return Reference.Cells[index_global].Data;
                
                return (TypeCells)Convert.ChangeType("0", typeof(TypeCells));
            }
        }
    }

    public class CExisted<TypeRows, TypeColumns, TypeCells>  where TypeRows : IComparable
                                                             where TypeColumns : IComparable
                                                             where TypeCells : IComparable
    {
        public Table<TypeRows, TypeColumns, TypeCells> Reference;

        public TypeCells this[TypeRows index_row, TypeColumns index_column]
        {
            set
            {
                var index_global = Reference.GetCellIndex(index_row, index_column);

                if (Reference.Columns.Contains(index_column) && Reference.Rows.Contains(index_row))
                {
                    if (index_global == -1)
                    {
                        Reference.Cells.Add(new Cell<TypeRows, TypeColumns, TypeCells>(index_row, index_column, value));
                    } 
                    else
                    {
                        Reference.Cells[index_global].Data = value;
                    }                    
                } 
                else
                {
                    throw new ArgumentException();
                }                                
            }
            
            get
            {
                var index_global = Reference.GetCellIndex(index_row, index_column);

                if (Reference.Columns.Contains(index_column) && Reference.Rows.Contains(index_row))
                {
                    if (index_global == -1)
                        return (TypeCells) Convert.ChangeType("0", typeof(TypeCells));
                    
                    return Reference.Cells[index_global].Data;
                } 
                throw new ArgumentException();
            }
        }
    }

    public class Table<TypeRows, TypeColumns, TypeCells> where TypeRows : IComparable
                                                  where TypeColumns : IComparable
                                                  where TypeCells : IComparable
    {

        public List<TypeRows> Rows;
        public List<TypeColumns> Columns;
        public List<Cell<TypeRows, TypeColumns, TypeCells>> Cells;
        public COpen<TypeRows, TypeColumns, TypeCells> Open;
        public CExisted<TypeRows, TypeColumns, TypeCells> Existed;

        public void AddRow(TypeRows index_row)
        {
            if (!Rows.Contains(index_row))
                Rows.Add(index_row);
        }

        public void AddColumn(TypeColumns index_column)
        {
            if (!Columns.Contains(index_column))
                Columns.Add(index_column);
        }

        public int GetCellIndex(TypeRows index_row, TypeColumns index_column)
        {
            for (var i = 0; i < Cells.Count; i++)
            {
                if (Cells[i].Row.CompareTo(index_row) == 0 && Cells[i].Column.CompareTo(index_column) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        public Table()
        {
            Rows = new List<TypeRows>();
            Columns = new List<TypeColumns>();
            Open = new COpen<TypeRows, TypeColumns, TypeCells>();
            Open.Reference = this;
            Existed = new CExisted<TypeRows, TypeColumns, TypeCells>();
            Existed.Reference = this;
            Cells = new List<Cell<TypeRows, TypeColumns, TypeCells>>();
        }
    }
}