using System;
using Microsoft.Xrm.Sdk;

namespace DynamicsDataExplorer.Bean
{
    /// <summary>
    /// コンボボックスのDatasource用のBeanです。
    /// Datasourceに設定する場合はこのBeanをリストにします。
    /// </summary>
    class CmbBean
    {
        public const string NAME_LOGICAL = "LogicalName";
        public const string NAME_DISPLAY = "DisplayName";

        /// <summary>
        /// キー
        /// </summary>
        public string LogicalName { get; set; }
        /// <summary>
        /// 表示値
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="logicalName"></param>
        /// <param name="displayName"></param>
        public CmbBean(string logicalName, Label displayName)
        {
            this.LogicalName = logicalName;
            this.DisplayName = logicalName;


            if (displayName.UserLocalizedLabel != null)
            {
                this.DisplayName = String.Format("{0}（{1}）",
                    displayName.UserLocalizedLabel.Label,
                    logicalName);
            }
        }
    }
}
