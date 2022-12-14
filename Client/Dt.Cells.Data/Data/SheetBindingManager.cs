#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
#endregion

namespace Dt.Cells.Data
{
    internal sealed class SheetBindingManager : IRangeSupport, IDisposable, IXmlSerializable
    {
        ConnectionBase connection;
        SparseArray<string> fields;
        bool isBound;
        bool isLoadingData;
        List<int> rowIndexes;
        IBindableSheet sheet;
        object source;

        /// <summary>
        /// This method is only used for xml deserialization, 
        /// Do Not use it in any other cases.
        /// </summary>
        public SheetBindingManager()
        {
            this.fields = new SparseArray<string>();
        }

        public SheetBindingManager(IBindableSheet sheet)
        {
            this.fields = new SparseArray<string>();
            this.sheet = sheet;
            if ((this.sheet != null) && (this.fields == null))
            {
                this.fields = new SparseArray<string>();
                this.fields.Length = this.sheet.ColumnCount;
            }
        }

        public void Bind(object source, bool autoGeneratedColumns)
        {
            if (this.isBound)
            {
                throw new InvalidOperationException(ResourceStrings.DataBindingRebindError);
            }
            this.source = source;
            this.connection = ConnectionBuilder.Build(source);
            if (this.connection == null)
            {
                throw new NotSupportedException(ResourceStrings.DataBindingNullConnection);
            }
            this.connection.Open();
            this.isLoadingData = true;
            if (autoGeneratedColumns)
            {
                this.sheet.GenerateColumns(this.connection.DataFields);
            }
            this.sheet.RowCount = this.connection.GetRecordCount();
            this.rowIndexes = new List<int>();
            for (int i = 0; i < this.sheet.RowCount; i++)
            {
                this.rowIndexes.Add(i);
            }
            this.isBound = true;
            this.isLoadingData = false;
        }

        public void Dispose()
        {
            this.Unbind();
            this.sheet = null;
            this.fields = null;
        }

        internal DataMatrix<object> GetAllBoundData()
        {
            if (!this.isBound)
            {
                return null;
            }
            DataMatrix<object> data = new DataMatrix<object> {
                RowCount = this.rowIndexes.Count,
                ColumnCount = this.fields.Length
            };
            for (int k = 0; k < this.rowIndexes.Count; k++)
            {
                if (this.rowIndexes[k] != -1)
                {
                    for (int i = 0; i < this.fields.Length; i++)
                    {
                        string field = this.fields[i];
                        if (field != null)
                        {
                            object obj2 = this.Connection.GetValue(this.rowIndexes[k], field);
                            data.SetValue(k, i, obj2);
                        }
                    }
                }
            }
            return data;
        }

        internal DataMatrix<object> GetAllCachedData()
        {
            List<int> nonEmpty;
            DataMatrix<object> data = new DataMatrix<object>();
            if (this.isBound)
            {
                data.RowCount = this.rowIndexes.Count;
                data.ColumnCount = this.fields.Length;
                nonEmpty = this.Connection.GetDirtyRecordIndex();
                for (int k = 0; k < nonEmpty.Count; k++)
                {
                    int recordIndex = nonEmpty[k];
                    int row = this.rowIndexes.IndexOf(recordIndex);
                    if ((row >= 0) && (this.fields != null))
                    {
                        List<int> nonEmptyIndexes = this.fields.GetNonEmptyIndexes();
                        for (int i = 0; i < nonEmptyIndexes.Count; i++)
                        {
                            int column = nonEmptyIndexes[i];
                            string str = this.fields[column];
                            if (!string.IsNullOrEmpty(str))
                            {
                                object obj2 = this.Connection.GetValue(recordIndex, str);
                                data.SetValue(row, column, obj2);
                            }
                        }
                    }
                }
            }
            return data;
        }

        internal List<int> GetBoundColumns()
        {
            List<int> list = new List<int>();
            if (this.fields != null)
            {
                return this.fields.GetNonEmptyIndexes();
            }
            return list;
        }

        internal List<int> GetBoundRows()
        {
            List<int> list = new List<int>();
            if (this.rowIndexes != null)
            {
                for (int i = 0; i < this.rowIndexes.Count; i++)
                {
                    if (this.rowIndexes[i] >= 0)
                    {
                        list.Add(i);
                    }
                }
            }
            return list;
        }

        public Type GetColumnDataType(int column)
        {
            if ((this.connection != null) && this.connection.IsOpen)
            {
                string dataSourceColumnFromModelColumn = this.GetDataSourceColumnFromModelColumn(column);
                if (dataSourceColumnFromModelColumn != null)
                {
                    return this.connection.GetColumnDataType(dataSourceColumnFromModelColumn);
                }
            }
            return typeof(object);
        }

        internal string GetDataSourceColumnFromModelColumn(int modelColumnIndex)
        {
            string str = this.fields[modelColumnIndex];
            if (str == null)
            {
                str = string.Empty;
            }
            return str;
        }

        internal int GetDataSourceRowFromModelRow(int modelRowIndex)
        {
            return this.rowIndexes[modelRowIndex];
        }

        internal int GetFirstBoundRow()
        {
            if ((this.rowIndexes != null) && (this.rowIndexes.Count > 0))
            {
                for (int i = 0; i < this.rowIndexes.Count; i++)
                {
                    if (this.rowIndexes[i] >= 0)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        internal int GetLastBoundRow()
        {
            if ((this.rowIndexes != null) && (this.rowIndexes.Count > 0))
            {
                for (int i = this.rowIndexes.Count - 1; i >= 0; i--)
                {
                    if (this.rowIndexes[i] >= 0)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        internal int GetModelColumnFromFieldName(string fieldName)
        {
            if ((this.fields != null) && (this.fields.Length > 0))
            {
                for (int i = 0; i < this.fields.Length; i++)
                {
                    if (this.fields[i] == fieldName)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public object GetValue(int row, int column)
        {
            int dsRow = this.GetDataSourceRowFromModelRow(row);
            if (dsRow != -1)
            {
                string field = this.fields[column];
                if (field != null)
                {
                    return connection.GetValue(dsRow, field);
                }
            }
            return null;
        }

        void IRangeSupport.AddColumns(int column, int count)
        {
            if (!this.isLoadingData && (this.fields != null))
            {
                if (column < 0)
                {
                    column = 0;
                }
                this.fields.InsertRange(column, count);
            }
        }

        void IRangeSupport.AddRows(int row, int count)
        {
            if ((!this.isLoadingData && this.IsBound) && (this.rowIndexes != null))
            {
                if (row < 0)
                {
                    row = 0;
                }
                if (row > this.rowIndexes.Count)
                {
                    row = this.rowIndexes.Count;
                }
                this.rowIndexes.InsertRange(row, Enumerable.Repeat<int>(-1, count));
            }
        }

        void IRangeSupport.Clear(int row, int column, int rowCount, int columnCount)
        {
            if (!this.isLoadingData && this.IsBound)
            {
                if (row < 0)
                {
                    row = 0;
                    rowCount = this.rowIndexes.Count;
                }
                if (row > this.rowIndexes.Count)
                {
                    row = this.rowIndexes.Count;
                }
                if (column < 0)
                {
                    column = 0;
                    columnCount = this.fields.Length;
                }
                if (column > this.fields.Length)
                {
                    column = this.fields.Length;
                }
                for (int i = row; (i < (row + rowCount)) && (i < this.rowIndexes.Count); i++)
                {
                    for (int j = column; (j < (column + columnCount)) && (j < this.fields.Length); j++)
                    {
                        this.SetValue(i, j, null);
                    }
                }
            }
        }

        void IRangeSupport.Copy(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            if (!this.isLoadingData && this.IsBound)
            {
                if (fromRow < 0)
                {
                    fromRow = 0;
                    rowCount = this.rowIndexes.Count;
                }
                if (fromRow > this.rowIndexes.Count)
                {
                    fromRow = this.rowIndexes.Count;
                    rowCount = 0;
                }
                if (fromColumn < 0)
                {
                    fromColumn = 0;
                    columnCount = this.fields.Length;
                }
                if (fromColumn > this.fields.Length)
                {
                    fromColumn = this.fields.Length;
                    columnCount = 0;
                }
                if (toRow < 0)
                {
                    toRow = 0;
                }
                if (toRow > this.rowIndexes.Count)
                {
                    toRow = this.rowIndexes.Count;
                }
                if (toColumn < 0)
                {
                    toColumn = 0;
                }
                if (toColumn > this.fields.Length)
                {
                    toColumn = this.fields.Length;
                }
                DataMatrix<object> matrix = new DataMatrix<object>(rowCount, columnCount);
                for (int i = 0; i < rowCount; i++)
                {
                    for (int k = 0; k < columnCount; k++)
                    {
                        object obj2 = this.GetValue(fromRow + i, fromColumn + k);
                        if (obj2 != null)
                        {
                            matrix.SetValue(i, k, Worksheet.CloneObject(obj2));
                        }
                    }
                }
                for (int j = 0; j < rowCount; j++)
                {
                    for (int m = 0; m < columnCount; m++)
                    {
                        this.SetValue(toRow + j, toColumn + m, matrix.GetValue(j, m));
                    }
                }
            }
        }

        void IRangeSupport.Move(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            if (!this.isLoadingData && this.IsBound)
            {
                if (fromRow < 0)
                {
                    fromRow = 0;
                    rowCount = this.rowIndexes.Count;
                }
                if (fromRow > this.rowIndexes.Count)
                {
                    fromRow = this.rowIndexes.Count;
                    rowCount = 0;
                }
                if (fromColumn < 0)
                {
                    fromColumn = 0;
                    columnCount = this.fields.Length;
                }
                if (fromColumn > this.fields.Length)
                {
                    fromColumn = this.fields.Length;
                    columnCount = 0;
                }
                if (toRow < 0)
                {
                    toRow = 0;
                }
                if (toRow > this.rowIndexes.Count)
                {
                    toRow = this.rowIndexes.Count;
                }
                if (toColumn < 0)
                {
                    toColumn = 0;
                }
                if (toColumn > this.fields.Length)
                {
                    toColumn = this.fields.Length;
                }
                DataMatrix<object> matrix = new DataMatrix<object>(rowCount, columnCount);
                for (int i = 0; i < rowCount; i++)
                {
                    for (int k = 0; k < columnCount; k++)
                    {
                        object obj2 = this.GetValue(fromRow + i, fromColumn + k);
                        if (obj2 != null)
                        {
                            matrix.SetValue(i, k, obj2);
                        }
                    }
                }
                ((IRangeSupport) this).Clear(fromRow, fromColumn, rowCount, columnCount);
                for (int j = 0; j < rowCount; j++)
                {
                    for (int m = 0; m < columnCount; m++)
                    {
                        this.SetValue(toRow + j, toColumn + m, matrix.GetValue(j, m));
                    }
                }
            }
        }

        void IRangeSupport.RemoveColumns(int column, int count)
        {
            if (!this.isLoadingData && (this.fields != null))
            {
                if (column < 0)
                {
                    column = 0;
                }
                if (column > this.fields.Length)
                {
                    column = this.fields.Length;
                }
                count = Math.Min(count, this.fields.Length - column);
                this.fields.RemoveRange(column, count);
            }
        }

        void IRangeSupport.RemoveRows(int row, int count)
        {
            if ((!this.isLoadingData && this.IsBound) && (this.rowIndexes != null))
            {
                if (row < 0)
                {
                    row = 0;
                }
                if (row > this.rowIndexes.Count)
                {
                    row = this.rowIndexes.Count;
                }
                count = Math.Min(count, this.rowIndexes.Count - row);
                this.rowIndexes.RemoveRange(row, count);
            }
        }

        void IRangeSupport.Swap(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            if (!this.isLoadingData)
            {
                if (((this.fields != null) && (fromRow == -1)) && (toRow == -1))
                {
                    for (int i = 0; i < columnCount; i++)
                    {
                        string str = this.fields[toColumn + i];
                        this.fields[toColumn + i] = this.fields[fromColumn + i];
                        this.fields[fromColumn + i] = str;
                    }
                }
                else if (((this.rowIndexes != null) && (fromColumn == -1)) && (toColumn == -1))
                {
                    for (int j = 0; j < rowCount; j++)
                    {
                        int num3 = this.rowIndexes[toRow + j];
                        this.rowIndexes[toRow + j] = this.rowIndexes[fromRow + j];
                        this.rowIndexes[fromRow + j] = num3;
                    }
                }
                else if (this.IsBound)
                {
                    if (fromRow < 0)
                    {
                        fromRow = 0;
                    }
                    if (fromRow > this.rowIndexes.Count)
                    {
                        fromRow = this.rowIndexes.Count;
                    }
                    if (fromColumn < 0)
                    {
                        fromColumn = 0;
                    }
                    if (fromColumn > this.fields.Length)
                    {
                        fromColumn = this.fields.Length;
                    }
                    if (toRow < 0)
                    {
                        toRow = 0;
                    }
                    if (toRow > this.rowIndexes.Count)
                    {
                        toRow = this.rowIndexes.Count;
                    }
                    if (toColumn < 0)
                    {
                        toColumn = 0;
                    }
                    if (toColumn > this.fields.Length)
                    {
                        toColumn = this.fields.Length;
                    }
                    for (int k = 0; k < rowCount; k++)
                    {
                        for (int m = 0; m < columnCount; m++)
                        {
                            object obj2 = this.GetValue(fromRow + k, fromColumn + m);
                            object obj3 = this.GetValue(toRow + k, toColumn + m);
                            this.SetValue(fromRow + k, fromColumn + m, obj3);
                            this.SetValue(toRow + k, toColumn + m, obj2);
                        }
                    }
                }
            }
        }

        public bool IsBoundColumn(int column)
        {
            if (!this.isBound)
            {
                return false;
            }
            return this.fields.ContainsIndex(column);
        }

        public bool IsBoundRow(int row)
        {
            if (!this.isBound)
            {
                return false;
            }
            return (this.GetDataSourceRowFromModelRow(row) != -1);
        }

        public void ReadXml(XmlReader reader)
        {
            Serializer.InitReader(reader);
            while (reader.Read())
            {
                XmlReader reader3;
                string str;
                if ((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null))
                {
                    if (str == "BindingRowIndexMapping")
                    {
                        XmlReader reader2 = Serializer.ExtractNode(reader);
                        List<int> list = new List<int>();
                        Serializer.DeserializeList((IList) list, reader2);
                        reader2.Close();
                        this.rowIndexes = list;
                    }
                    else if (str == "BindingFieldsMapping")
                    {
                        goto Label_005D;
                    }
                }
                continue;
            Label_005D:
                reader3 = Serializer.ExtractNode(reader);
                SparseArray<string> array = new SparseArray<string>();
                Serializer.DeserializeArray<string>(array, null, reader3);
                this.fields = array;
                reader3.Close();
            }
        }

        public void ReloadBindingData()
        {
            if (this.connection != null)
            {
                this.isLoadingData = true;
                this.connection.ClearCachedData();
                this.connection.UpdateCollectionView();
                int recordCount = this.connection.GetRecordCount();
                this.sheet.RowCount = recordCount;
                this.rowIndexes = new List<int>(Enumerable.Range(0, recordCount));
                this.isLoadingData = false;
            }
        }

        internal void ReOpenDataSource(object datasource)
        {
            if (this.connection != null)
            {
                this.connection.Close();
            }
            this.source = datasource;
            this.connection = ConnectionBuilder.Build(this.source);
            if (this.connection == null)
            {
                throw new NotSupportedException(ResourceStrings.DataBindingNullConnection);
            }
            this.connection.Open();
        }

        public void SetBoundField(int index, string dataField)
        {
            if (this.sheet != null)
            {
                if (string.IsNullOrEmpty(dataField))
                {
                    this.fields.Clear(index);
                }
                else
                {
                    if (this.fields == null)
                    {
                        this.fields = new SparseArray<string>(this.sheet.ColumnCount);
                    }
                    this.fields[index] = dataField;
                    this.fields.Length = Math.Max(index + 1, this.fields.Length);
                }
            }
        }

        public void SetValue(int row, int column, object value)
        {
            int dataSourceRowFromModelRow = this.GetDataSourceRowFromModelRow(row);
            if (dataSourceRowFromModelRow != -1)
            {
                string field = this.fields[column];
                if (field != null)
                {
                    this.connection.SetValue(dataSourceRowFromModelRow, field, value);
                }
            }
        }

        public void Unbind()
        {
            if (this.isBound)
            {
                if (this.connection != null)
                {
                    this.connection.Close();
                }
                this.connection = null;
                this.rowIndexes = null;
                this.source = null;
                this.isBound = false;
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            Serializer.InitWriter(writer);
            if (this.rowIndexes != null)
            {
                writer.WriteStartElement("BindingRowIndexMapping");
                Serializer.SerializeList((IList) this.rowIndexes, writer);
                writer.WriteEndElement();
            }
            if (this.fields != null)
            {
                writer.WriteStartElement("BindingFieldsMapping");
                Serializer.SerializeArray<string>(this.fields, true, writer);
                writer.WriteEndElement();
            }
        }

        public ConnectionBase Connection
        {
            get { return  this.connection; }
        }

        internal bool IsBound
        {
            get { return  this.isBound; }
            set { this.isBound = value; }
        }

        /// <summary>
        /// This property is Only used for xml deserialization, 
        /// Do Not use it in any other cases.
        /// </summary>
        internal IBindableSheet Sheet
        {
            get { return  this.sheet; }
            set
            {
                if (this.isBound)
                {
                    throw new InvalidOperationException(ResourceStrings.DataBindingSetSheetWhenAlreadyBound);
                }
                this.sheet = value;
                if ((this.sheet != null) && (this.fields == null))
                {
                    this.fields = new SparseArray<string>();
                    this.fields.Length = this.sheet.ColumnCount;
                }
            }
        }

        public object Source
        {
            get { return  this.source; }
        }
    }
}

