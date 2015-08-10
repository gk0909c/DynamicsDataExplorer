using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicsDataExplorer.Bean
{
    /// <summary>
    /// カラム設定を保存するためのBean
    /// </summary>
    [Serializable]
    public class ColumnSettingBean
    {
        public string DataPropertyName { get; set; }
        public string Name { get; set; }
        public string HeaderText { get; set; }
        public Object ListItem { get; set; }
    }
}
