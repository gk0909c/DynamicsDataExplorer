using Microsoft.Xrm.Sdk.Query;

namespace DynamicsDataExplorer.Constants
{
    /// <summary>
    /// CmbOperatorの拡張クラス
    /// </summary>
    public static class CmbOperatorExt
    {
        /// <summary>
        /// CmbOperatorの値に応じて、ConditionOperatorを生成。
        /// </summary>
        /// <param name="ope"></param>
        /// <returns></returns>
        public static ConditionOperator GetOpeator(this CmbOperator ope)
        {
            ConditionOperator conditionOperator = ConditionOperator.Equal;
            switch (ope)
            {
                case CmbOperator.Match:
                    conditionOperator = ConditionOperator.Like;
                    break;
                case CmbOperator.ForwardMath:
                    conditionOperator = ConditionOperator.BeginsWith;
                    break;
                case CmbOperator.BackwardMath:
                    conditionOperator = ConditionOperator.EndsWith;
                    break;
                case CmbOperator.GraterThan:
                    conditionOperator = ConditionOperator.GreaterThan;
                    break;
                case CmbOperator.GraterEqual:
                    conditionOperator = ConditionOperator.GreaterEqual;
                    break;
                case CmbOperator.LessThan:
                    conditionOperator = ConditionOperator.LessThan;
                    break;
                case CmbOperator.LessEqual:
                    conditionOperator = ConditionOperator.LessEqual;
                    break;
            }

            return conditionOperator;
        }
    }
}
