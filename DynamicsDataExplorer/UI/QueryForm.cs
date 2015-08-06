using System;
using System.Windows.Forms;
using System.Drawing;

using DynamicsDataExplorer.Logic;
using DynamicsDataExplorer.Properties;
using DynamicsDataExplorer.Dynamics;
using DynamicsDataExplorer.Constants;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace DynamicsDataExplorer.UI
{
    public partial class QueryForm : Form
    {
        private DynamicsCls _dynamics;
        private QueryFormLogic _logic;
        private ColumnSettingLogic _colLogic;
        private string _entityName;
        private int _columnSettingListFromIndex;
        private int _columnSettingListMaxIndex;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public QueryForm()
        {
            InitializeComponent();

            _logic = new QueryFormLogic();
            _colLogic = new ColumnSettingLogic(lstColumnSetting, dataGrid);
            txtUser.Text = Settings.Default.User;
            txtPass.Text = Settings.Default.Pass;
            txtUrl.Text = Settings.Default.URL;

            cmbOperator.DataSource = Enum.GetValues(typeof(CmbOperator));
        }

        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueryForm_Load(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            btnGetAttributes.Enabled = false;
            btnQuery.Enabled = false;
        }

        /// <summary>
        /// ログイン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                _dynamics = new DynamicsCls(txtUser.Text, txtPass.Text, txtUrl.Text);
            }
            catch (Exception ex)
            {
                ShowErrorMessage("ログインに失敗しました。\n" + ex.Message, "ログインエラー");
                return;
            }

            // エンティティ一覧を取得して、コンボボックスに設定
            EntityMetadata[] entities = _dynamics.getAllEntity();
            _logic.SetEntityCmb(entities, cmbEntities);

            // クエリ用のタブを表示
            tabCtrl.SelectedTab = tabQuery;

            btnGetAttributes.Enabled = true;
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// エンティティの項目取得
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetAttributes_Click(object sender, EventArgs e)
        {
            if (cmbEntities.SelectedValue != null)
            {
                Cursor.Current = Cursors.WaitCursor;

                try
                {
                    dataGrid.Columns.Clear();
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    // カラム設定をした後に例外が発生することがあるが、MSのバグっぽいので無視
                    Console.WriteLine(ex.Message);
                }

                _entityName = cmbEntities.SelectedValue.ToString();

                // 項目情報を取得して、グリッドのヘッダを取得
                AttributeMetadata[] attributes = _dynamics.getAttributes(_entityName);
                _logic.SetDataGridColumns(attributes, this.dataGrid);
                _logic.SetColumnSettingList(attributes, lstColumnSetting);
                _columnSettingListMaxIndex = lstColumnSetting.Items.Count;

                // 条件指定用のコンボボックスを設定
                _logic.SetAttributeCmb(attributes, cmbAttributes);

                // オプションセット情報の保持
                _logic.SetOptionSetData(attributes);

                btnQuery.Enabled = true;
                Cursor.Current = Cursors.Default;
            }
        }
            
        /// <summary>
        /// データ取得
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            dataGrid.Rows.Clear();

            // データを取得して、グリッドビューに設定
            try
            {
                EntityCollection result = _dynamics.getData(_entityName,
                    _logic.CreateCondition(
                        cmbAttributes.SelectedValue == null ? null : cmbAttributes.SelectedValue.ToString(),
                        (CmbOperator)cmbOperator.SelectedIndex,
                        txtCondValue.Text));
                _logic.SetDataGridValues(result, this.dataGrid);

            }
            catch (Exception ex)
            {
                ShowErrorMessage("検索条件を見直してください。\n" + ex.Message, "データ取得エラー");
            }

            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// 接続情報を保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveConnectSetting_Click(object sender, EventArgs e)
        {

            Settings.Default.User = txtUser.Text;
            Settings.Default.Pass = txtPass.Text;
            Settings.Default.URL = txtUrl.Text;
            Settings.Default.Save();

            MessageBox.Show("接続情報を保存しました。", "通知", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        

        private void ShowErrorMessage(string msg, string title)
        {
            MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void aaa(object sender, MouseEventArgs e)
        {

        }

        /// <summary>
        /// 列設定のドラッグ開始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstColumnSetting_MouseDown(object sender, MouseEventArgs e)
        {
            Point p = Control.MousePosition;
            p = lstColumnSetting.PointToClient(p);
            _columnSettingListFromIndex = lstColumnSetting.IndexFromPoint(p);
            if (_columnSettingListFromIndex > -1)
            {
                //ドラッグスタート
                lstColumnSetting.DoDragDrop(lstColumnSetting.Items[_columnSettingListFromIndex].ToString(), DragDropEffects.Copy);

            }
        }

        /// <summary>
        /// ドラッグ＆ドロップの効果を、コピーに設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstColumnSetting_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        /// <summary>
        /// 列設定のドロップ時の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstColumnSetting_DragDrop(object sender, DragEventArgs e)
        {
            string str = e.Data.GetData(DataFormats.Text).ToString();

            Point p = Control.MousePosition;
            p = lstColumnSetting.PointToClient(p);//ドロップ時のマウスの位置をクライアント座標に変換
            int columnSettingListToIndex = lstColumnSetting.IndexFromPoint(p);//マウス下のＬＢのインデックスを得る
            if (columnSettingListToIndex > -1 && columnSettingListToIndex < _columnSettingListMaxIndex)
            {
                // 列設定リストの入れ替え
                lstColumnSetting.Items[_columnSettingListFromIndex] = lstColumnSetting.Items[columnSettingListToIndex];
                lstColumnSetting.Items[columnSettingListToIndex] = str;

                
                // データ表示列の入れ替え
                _logic.ReplaceDataGridColumn(
                    dataGrid.Columns[_columnSettingListFromIndex],
                    dataGrid.Columns[columnSettingListToIndex]
                    );
            }
        }

        /// <summary>
        /// 「一番上」ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnColumnSettingTop_Click(object sender, EventArgs e)
        {
            _colLogic.SetSelectedIdx(lstColumnSetting.SelectedIndex);
            _colLogic.Top();
        }

        /// <summary>
        /// 「上」ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnColumnSettingUp_Click(object sender, EventArgs e)
        {
            _colLogic.SetSelectedIdx(lstColumnSetting.SelectedIndex);
            _colLogic.Up();
        }

        /// <summary>
        /// 「下」ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnColumnSettingDown_Click(object sender, EventArgs e)
        {
            _colLogic.SetSelectedIdx(lstColumnSetting.SelectedIndex);
            _colLogic.Down();
        }

        /// <summary>
        /// 「一番下」ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnColumnSettingBottom_Click(object sender, EventArgs e)
        {
            _colLogic.SetSelectedIdx(lstColumnSetting.SelectedIndex);
            _colLogic.Bottom();
        }

    }
}
