using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace DynamicsDataExplorer.Bean
{
    /// <summary>
    /// オプションセットのキーと値の紐づきを保持するBeanです。
    /// </summary>
    class OptionSetBean
    {
        private Dictionary<int, Label> valueMap;

        /// <summary>
        /// フィールドのオプションセットを取得する。
        /// </summary>
        /// <param name="attr"></param>
        public OptionSetBean(AttributeMetadata attr)
        {
            valueMap = new Dictionary<int, Label>();
            PicklistAttributeMetadata picklistAttr = attr as PicklistAttributeMetadata;
            StateAttributeMetadata stateAttr = attr as StateAttributeMetadata;
            StatusAttributeMetadata statusAttr = attr as StatusAttributeMetadata;

            // オプションセットを取得する。
            OptionSetMetadata option = null;
            if (picklistAttr != null)
            {
                option = picklistAttr.OptionSet; 
            }
            else if (stateAttr != null)
            {
                option = stateAttr.OptionSet; 
            }
            else if (statusAttr != null)
            {
                option = statusAttr.OptionSet; 
            }
            else
            {
                return;
            }


            foreach (OptionMetadata opt in option.Options)
            {
                if (opt.Value != null)
                {

                    valueMap.Add((int)opt.Value, opt.Label);
                }
            }
        }

        /// <summary>
        /// 該当項目にオプションセットがあるかどうかを判定する。
        /// </summary>
        /// <returns></returns>
        public Boolean HasOptionSet()
        {
            return !(valueMap.Count == 0);
        }

        /// <summary>
        /// キーに対応するオプションセットのラベルを返却する。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(int key)
        {
            if (!valueMap.ContainsKey(key))
            {
                return null;
            }

            Label label = valueMap[key];

            if (label.UserLocalizedLabel == null)
            {
                return null;
            }
            else
            {
                return label.UserLocalizedLabel.Label.ToString();
            }
        }
    }
}
