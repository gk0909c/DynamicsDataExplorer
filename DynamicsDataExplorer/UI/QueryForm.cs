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
        private ColumnMoveLogic _moveColLogic;
        private ColumnReplaceLogic _replaceColLogic;
        private ColumnSettingSaveLogic _saveColLogic;
        private string _entityName;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public QueryForm()
        {
            InitializeComponent();

            _logic = new QueryFormLogic();
            _moveColLogic = new ColumnMoveLogic(lstColumnSetting, dataGrid);
            _replaceColLogic = new ColumnReplaceLogic(lstColumnSetting, dataGrid);
            _saveColLogic = new ColumnSettingSaveLogic(lstColumnSetting, dataGrid);
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
        
        /// <summary>
        /// エラーメッセージ表示の共通メソッド
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="title"></param>
        private void ShowErrorMessage(string msg, string title)
        {
            MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 列設定のドラッグ開始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstColumnSetting_MouseDown(object sender, MouseEventArgs e)
        {
            _replaceColLogic.StartReplace(GetListIndex(Control.MousePosition));
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
            _replaceColLogic.EndReplace(
                e.Data.GetData(DataFormats.Text).ToString(),
                GetListIndex(Control.MousePosition));
        }

        /// <summary>
        /// マウスの位置にあるリストのインデックス取得
        /// </summary>
        /// <returns></returns>
        private int GetListIndex(Point p)
        {
            p = lstColumnSetting.PointToClient(p);
            return lstColumnSetting.IndexFromPoint(p);
        }

        /// <summary>
        /// 「一番上」ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnColumnSettingTop_Click(object sender, EventArgs e)
        {
            _moveColLogic.Top();
        }

        /// <summary>
        /// 「上」ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnColumnSettingUp_Click(object sender, EventArgs e)
        {
            _moveColLogic.Up();
        }

        /// <summary>
        /// 「下」ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnColumnSettingDown_Click(object sender, EventArgs e)
        {
            _moveColLogic.Down();
        }

        /// <summary>
        /// 「一番下」ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnColumnSettingBottom_Click(object sender, EventArgs e)
        {
            _moveColLogic.Bottom();
        }

        /// <summary>
        /// カラム設定保存ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveColumnSetting_Click(object sender, EventArgs e)
        {
            DoSaveColumnSetting();
        }

        /// <summary>
        /// カラム設定保存の実処理
        /// </summary>
        private void DoSaveColumnSetting()
        {
            if (!ChkEntitySelected())
            {
                return;
            }

            _saveColLogic.Save(_entityName);
            MessageBox.Show("設定を保存しました。", "完了");
        }

        /// <summary>
        /// カラム設定読み込みボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoadColumnSetting_Click(object sender, EventArgs e)
        {
            DoLoadColumnSetting();
        }

        /// <summary>
        /// カラム設定読み込みの実処理
        /// </summary>
        private void DoLoadColumnSetting()
        {
            if (!ChkEntitySelected())
            {
                return;
            }

            if (_saveColLogic.Load(_entityName))
            {
                MessageBox.Show("設定を読み込みました。", "完了");
            }
            else
            {
                ShowErrorMessage("保存されている設定がありません。", "読み込みエラー");
            }

        }

        /// <summary>
        /// エンティティ定義取得チェック
        /// </summary>
        /// <returns></returns>
        private bool ChkEntitySelected()
        {
            if (String.IsNullOrEmpty(_entityName))
            {
                ShowErrorMessage("先にエンティティの定義を取得してください。", "エンティティ選択エラー");
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// ショートカット割り当て
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueryForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F10 :
                    btnQuery.PerformClick();
                    break;
                case Keys.F11 :
                    DoLoadColumnSetting();
                    break;
                case Keys.F12:
                    DoSaveColumnSetting();
                    break;

            }
        }

    }
}
