using System;
using System.Windows.Forms;

namespace DynamicsDataExplorer.Logic
{
    /// <summary>
    /// データグリッドのカラム設定関連のロジック
    /// </summary>
    class ColumnMoveLogic
    {
        private ListBox _list;
        private DataGridView _grid;
        // 追加等の操作後にリストからとると動作がおかしくなるため、CheckDoメソッド内で設定
        private int _selectedIdx;

        public ColumnMoveLogic(ListBox list, DataGridView grid)
        {
            _list = list;
            _grid = grid;
            _selectedIdx = list.SelectedIndex;
        }

        /// <summary>
        /// 一番上に移動
        /// </summary>
        /// <param name="list"></param>
        /// <param name="grid"></param>
        public void Top()
        {
            if (CheckDo(_list.SelectedIndex, 0))
            {
                InsCommon(0);
                RemoveCommon(_selectedIdx + 1);
            }
        }

        /// <summary>
        /// 上に移動
        /// </summary>
        /// <param name="list"></param>
        /// <param name="grid"></param>
        public void Up()
        {
            if (CheckDo(_list.SelectedIndex, 0))
            {
                InsCommon(_selectedIdx - 1);
                RemoveCommon(_selectedIdx + 1);
            }
        }

        /// <summary>
        /// 下に移動
        /// </summary>
        /// <param name="list"></param>
        /// <param name="grid"></param>
        public void Down()
        {
            if (CheckDo(_list.SelectedIndex + 1, _list.Items.Count))
            {
                InsCommon(_selectedIdx + 2);
                RemoveCommon(_selectedIdx);
            }
        }

        /// <summary>
        /// 最後に移動
        /// </summary>
        /// <param name="list"></param>
        /// <param name="grid"></param>
        public void Bottom()
        {
            if (CheckDo(_list.SelectedIndex + 1, _list.Items.Count))
            {
                string selected = _list.SelectedItem.ToString();
                DataGridViewColumn org = (DataGridViewColumn)_grid.Columns[_selectedIdx].Clone();

                _list.Items.Add(selected);
                _grid.Columns.Add(org);

                RemoveCommon(_selectedIdx);
            }
        }

        private bool CheckDo(int check1, int check2)
        {
            _selectedIdx = _list.SelectedIndex;

            if (_selectedIdx > -1 && (check1 != check2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 要素挿入
        /// </summary>
        /// <param name="list"></param>
        /// <param name="grid"></param>
        /// <param name="insertTo"></param>
        private void InsCommon(int insertTo)
        {
            string selected = _list.SelectedItem.ToString();
            DataGridViewColumn org = (DataGridViewColumn)_grid.Columns[_selectedIdx].Clone();

            _list.Items.Insert(insertTo, selected);
            _grid.Columns.Insert(insertTo, org);
        }

        /// <summary>
        /// 選択元削除
        /// </summary>
        /// <param name="list"></param>
        /// <param name="grid"></param>
        /// <param name="removeAt"></param>
        private void RemoveCommon(int removeAt)
        {
            _list.Items.RemoveAt(removeAt);
            _grid.Columns.RemoveAt(removeAt);
        }
    }
}
