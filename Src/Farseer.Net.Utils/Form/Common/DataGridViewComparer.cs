﻿#if IsForm
using System.Collections;
using System.Windows.Forms;

namespace FS.Utils.Common
{
    /// <summary>
    ///     DataGridView排序
    ///     zgke@sina.com
    ///     qq:116149
    /// </summary>
    public class DataGridViewComparer : IComparer
    {
        private readonly DataGridViewColumn _column;

        /// <summary>
        ///     dataGridView1.Columns[0].HeaderCell.SortGlyphDirection = SortOrder.Descending; 根据这个进行排序列
        /// </summary>
        /// <param name="column"></param>
        public DataGridViewComparer(DataGridViewColumn column)
        {
            this._column = column;
        }

        int IComparer.Compare(object x, object y)
        {
            if (_column == null) return -1;
            var _X = ConvertHelper.ConvertType(((DataGridViewRow) x).Cells[_column.Name].Value, 0m);
            var _Y = ConvertHelper.ConvertType(((DataGridViewRow) y).Cells[_column.Name].Value, 0m);

            var compareValue = _X.CompareTo(_Y);
            if (_column.HeaderCell.SortGlyphDirection == SortOrder.Descending) return compareValue*-1;
            return compareValue;
        }
    }
}

#endif