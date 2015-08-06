using System;
using System.Windows.Forms;
using System.Drawing;

namespace DynamicsDataExplorer.Logic
{
    class ColumnReplaceLogic
    {
        private ListBox _list;
        private DataGridView _grid;
        private int _fromIndex;

        public ColumnReplaceLogic(ListBox list, DataGridView grid)
        {
            _list = list;
            _grid = grid;
        }

        /// <summary>
        /// ドラッグ処理の開始時の処理
        /// </summary>
        public void StartReplace(int fromIndex)
        {
            _fromIndex = fromIndex;
            if (_fromIndex > -1)
            {
                //ドラッグスタート
                _list.DoDragDrop(_list.Items[_fromIndex].ToString(), DragDropEffects.Copy);

            }
        }

        /// <summary>
        /// ドロップ時の処理
        /// </summary>
        /// <param name="fromStr"></param>
        public void EndReplace(string fromStr, int toIndex)
        {
            if (toIndex > -1 && toIndex < _list.Items.Count)
            {
                // 列設定リストの入れ替え
                _list.Items[_fromIndex] = _list.Items[toIndex];
                _list.Items[toIndex] = fromStr;


                // データ表示列の入れ替え
                ReplaceDataGridColumn(
                    _grid.Columns[_fromIndex],
                    _grid.Columns[toIndex]
                    );
            }
        }

        /// <summary>
        /// DataGridのカラム情報の入れ替え
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        private void ReplaceDataGridColumn(DataGridViewColumn from, DataGridViewColumn to)
        {
            DataGridViewColumn fromClone = (DataGridViewColumn)from.Clone();

            from.DataPropertyName = to.DataPropertyName;
            from.Name = to.Name;
            from.HeaderText = to.HeaderText;

            to.DataPropertyName = fromClone.DataPropertyName;
            to.Name = fromClone.Name;
            to.HeaderText = fromClone.HeaderText;

        }
    }
}
