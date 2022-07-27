#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class CellCancelEventArgs : CancelEventArgs
    {
        internal CellCancelEventArgs(int row, int column)
        {
            Row = row;
            Column = column;
        }

        /// <summary>
        /// Gets the column index of the cell.
        /// </summary>
        /// <value>The column index of the cell.</value>
        public int Column { get; private set; }

        /// <summary>
        /// Gets the row index of the cell.
        /// </summary>
        /// <value>The row index of the cell.</value>
        public int Row { get; private set; }
    }
}

