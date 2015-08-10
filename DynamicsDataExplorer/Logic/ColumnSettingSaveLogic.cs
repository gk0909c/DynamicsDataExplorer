using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;

using DynamicsDataExplorer.Bean;

namespace DynamicsDataExplorer.Logic
{
    class ColumnSettingSaveLogic
    {
        private const string FILE_SUFFIX = "_ColSetting.xml";

        private DataGridView _grid;
        private ListBox _list;

        public ColumnSettingSaveLogic(ListBox list, DataGridView grid)
        {
            _grid = grid;
            _list = list;
        }

        /// <summary>
        /// 設定の保存
        /// </summary>
        /// <param name="_entityName"></param>
        public void Save(string _entityName)
        {
            string fileName = _entityName + FILE_SUFFIX;

            List<ColumnSettingBean> settingList = new List<ColumnSettingBean>();

            foreach (DataGridViewColumn col in _grid.Columns)
            {
                ColumnSettingBean bean = new ColumnSettingBean()
                {
                    DataPropertyName = col.DataPropertyName,
                    Name = col.Name,
                    HeaderText = col.HeaderText,

                    ListItem = _list.Items[col.Index]
                };
                settingList.Add(bean);
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<ColumnSettingBean>));
            StreamWriter sw = new StreamWriter(fileName, false, new System.Text.UTF8Encoding(false));
            serializer.Serialize(sw, settingList);
            sw.Close();
        }

        /// <summary>
        /// 設定の読み込み
        /// </summary>
        /// <param name="_entityName"></param>
        /// <returns>読み込み対象が存在しない場合false</returns>
        public bool Load(string _entityName)
        {
            string fileName = _entityName + FILE_SUFFIX;

            if (!File.Exists(fileName)) return false;

            XmlSerializer serializer = new XmlSerializer(typeof(List<ColumnSettingBean>));
            StreamReader sr = new StreamReader(fileName, new System.Text.UTF8Encoding(false));
            List<ColumnSettingBean> settingList = (List<ColumnSettingBean>)serializer.Deserialize(sr);

            for (int i = 0; i < settingList.Count; i++ )
            {
                _grid.Columns[i].DataPropertyName = settingList[i].DataPropertyName;
                _grid.Columns[i].Name = settingList[i].Name;
                _grid.Columns[i].HeaderText = settingList[i].HeaderText;

                _list.Items[i] = settingList[i].ListItem;
            }

            sr.Close();

            return true;
        }
    }
}
