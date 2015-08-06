using System;
using System.Collections.Generic;
using System.Windows.Forms;

using DynamicsDataExplorer.Bean;
using DynamicsDataExplorer.Constants;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace DynamicsDataExplorer.Logic
{
    /// <summary>
    /// QueryFormのロジックを保持するクラスです。
    /// </summary>
    class QueryFormLogic
    {
        private Dictionary<string, OptionSetBean> _optionSetMap;

        /// <summary>
        /// エンティティ指定用のコンボボックスDataSource生成
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public void SetEntityCmb(EntityMetadata[] entities, ComboBox cmb)
        {
            List<CmbBean> entityList = new List<CmbBean>();

            foreach (EntityMetadata entity in entities)
            {
                if (!entity.IsCustomizable.Value)
                {
                    // Internal entitiesをMultipleRetrieveができないが、
                    // とりあえずカスタマイズ不可なものを除く
                    // https://msdn.microsoft.com/en-us/library/gg328086.aspx
                    continue;
                }

                CmbBean bean = new CmbBean(entity.LogicalName, entity.DisplayName);
                entityList.Add(bean);
            }
            entityList.Sort((a, b) => a.DisplayName.CompareTo(b.DisplayName));
            SetListToCmb(entityList, cmb);
        }

        /// <summary>
        /// DataGridViewのヘッダを設定します。
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="dataGrid"></param>
        public void SetDataGridColumns(AttributeMetadata[] attributes, DataGridView dataGrid)
        {
            foreach (AttributeMetadata attr in attributes)
            {
                // 条件が微妙だが、不要カラムの非表示
                string header = attr.LogicalName;
                if (attr.DisplayName.UserLocalizedLabel == null)
                {
                    continue;
                }
                else
                {
                    header = attr.DisplayName.UserLocalizedLabel.Label;
                }

                DataGridViewTextBoxColumn textColumn = new DataGridViewTextBoxColumn();
                textColumn.DataPropertyName = attr.LogicalName;
                textColumn.Name = attr.LogicalName;
                textColumn.HeaderText = header;

                dataGrid.Columns.Add(textColumn);
            }
        }

        /// <summary>
        /// 条件指定用のコンボボックスを設定
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public void SetAttributeCmb(AttributeMetadata[] attributes, ComboBox cmb)
        {
            List<CmbBean> attributeList = new List<CmbBean>();

            foreach (AttributeMetadata attr in attributes)
            {
                CmbBean bean = new CmbBean(attr.LogicalName, attr.DisplayName);
                attributeList.Add(bean);
            }
            attributeList.Sort((a, b) => a.DisplayName.CompareTo(b.DisplayName));
            SetListToCmb(attributeList, cmb);
        }

        public void SetColumnSettingList(AttributeMetadata[] attributes, ListBox lst)
        {
            List<CmbBean> attributeList = new List<CmbBean>();
            lst.Items.Clear();
            

            foreach (AttributeMetadata attr in attributes)
            {
                string header = attr.LogicalName;
                if (attr.DisplayName.UserLocalizedLabel == null)
                {
                    continue;
                }
                else
                {
                    header = attr.DisplayName.UserLocalizedLabel.Label;
                    lst.Items.Add(header);
                }

                //CmbBean bean = new CmbBean(attr.LogicalName, attr.DisplayName);
                //attributeList.Add(bean);
            }
            //attributeList.Sort((a, b) => a.DisplayName.CompareTo(b.DisplayName));
            //SetListToListBox(attributeList, lst);
        }

        /// <summary>
        /// 選択リストの表示値を取得する
        /// </summary>
        /// <param name="attributes"></param>
        public void SetOptionSetData(AttributeMetadata[] attributes)
        {
            _optionSetMap = new Dictionary<string, OptionSetBean>();

            foreach (AttributeMetadata attr in attributes)
            {
                // 表示用にオプションセットのマップを保持しておく
                OptionSetBean bean = new OptionSetBean(attr);

                if (bean.HasOptionSet())
                {
                    _optionSetMap.Add(attr.LogicalName, bean);
                }
            }
        }

        /// <summary>
        /// データをグリッドビューに設定
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataGrid"></param>
        public void SetDataGridValues(EntityCollection data, DataGridView dataGrid)
        {
            for (int i = 0; i < data.Entities.Count; i++)
            {
                Entity entity = data.Entities[i];

                dataGrid.Rows.Add();

                for (int j = 0; j < dataGrid.Columns.Count; j++)
                {
                    string key = dataGrid.Columns[j].DataPropertyName;

                    if (entity.Attributes.ContainsKey(key))
                    {
                        Object attr = entity.Attributes[key];

                        if (attr is OptionSetValue)
                        {
                            int optionKey = ((OptionSetValue)attr).Value;

                            if (_optionSetMap.ContainsKey(key))
                            {
                                OptionSetBean bean = _optionSetMap[key];
                                dataGrid.Rows[i].Cells[j].Value = bean.GetValue(optionKey);
                            }
                            else
                            {
                                dataGrid.Rows[i].Cells[j].Value = optionKey;
                            }
                        }
                        else if (attr is EntityReference)
                        {
                            dataGrid.Rows[i].Cells[j].Value = ((EntityReference)attr).Id;
                        }
                        else if (attr is Money)
                        {
                            dataGrid.Rows[i].Cells[j].Value = ((Money)attr).Value;
                        }
                        else
                        {
                            dataGrid.Rows[i].Cells[j].Value = attr;
                        }
                    }

                }
            }
        }

        /// <summary>
        /// 検索条件を作成します。
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ConditionExpression CreateCondition(string attr, CmbOperator operatorValue, string value)
        {
            if (String.IsNullOrEmpty(attr) || string.IsNullOrEmpty(value))
            {
                return null;
            }

            ConditionExpression cond = new ConditionExpression();
            cond.AttributeName = attr;
            cond.Operator = operatorValue.GetOpeator();
            cond.Values.Add(value);

            return cond;
        }

        public void ReplaceDataGridColumn(DataGridViewColumn from, DataGridViewColumn to)
        {
            DataGridViewColumn fromClone = (DataGridViewColumn)from.Clone();

            from.DataPropertyName = to.DataPropertyName;
            from.Name = to.Name;
            from.HeaderText = to.HeaderText;

            to.DataPropertyName = fromClone.DataPropertyName;
            to.Name = fromClone.Name;
            to.HeaderText = fromClone.HeaderText;

        }

        /// <summary>
        /// コンボボックスBeanのリストをコンボボックスに割り当てます。
        /// </summary>
        /// <param name="list"></param>
        /// <param name="cmb"></param>
        private void SetListToCmb(List<CmbBean> list, ComboBox cmb)
        {
            cmb.DataSource = list;
            cmb.DisplayMember = CmbBean.NAME_DISPLAY;
            cmb.ValueMember = CmbBean.NAME_LOGICAL;

        }
    }
}
