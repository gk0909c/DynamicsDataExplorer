using System;
using System.Windows.Forms;

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
        private string _entityName;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public QueryForm()
        {
            InitializeComponent();

            _logic = new QueryFormLogic();
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
            Cursor.Current = Cursors.WaitCursor;
            dataGrid.Columns.Clear();
            _entityName = cmbEntities.SelectedValue.ToString();

            // 項目情報を取得して、グリッドのヘッダを取得
            AttributeMetadata[] attributes = _dynamics.getAttributes(_entityName);
            _logic.SetDataGridColumns(attributes, this.dataGrid);

            // 条件指定用のコンボボックスを設定
            _logic.SetAttributeCmb(attributes, cmbAttributes);

            // オプションセット情報の保持
            _logic.SetOptionSetData(attributes);

            btnQuery.Enabled = true;
            Cursor.Current = Cursors.Default;
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
                        cmbAttributes.SelectedValue.ToString(),
                        (CmbOperator)cmbOperator.SelectedIndex,
                        txtCondValue.Text));
                _logic.SetDataGridValues(result, this.dataGrid);

            } catch (Exception ex)
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
    }
}
