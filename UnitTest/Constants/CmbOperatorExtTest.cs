using DynamicsDataExplorer.Constants;
using Microsoft.Xrm.Sdk.Query;
using NUnit.Framework;

namespace UnitTest.Constants
{
    [TestFixture]
    class CmbOperatorExtTest
    {
        [TestCase]
        public void CmbOperatorExtTest01()
        {
            Assert.AreEqual(ConditionOperator.Equal, CmbOperator.Equal.GetOpeator());
            Assert.AreEqual(ConditionOperator.Like, CmbOperator.Match.GetOpeator());
            Assert.AreEqual(ConditionOperator.BeginsWith, CmbOperator.ForwardMath.GetOpeator());
            Assert.AreEqual(ConditionOperator.EndsWith, CmbOperator.BackwardMath.GetOpeator());
            Assert.AreEqual(ConditionOperator.GreaterThan, CmbOperator.GraterThan.GetOpeator());
            Assert.AreEqual(ConditionOperator.GreaterEqual, CmbOperator.GraterEqual.GetOpeator());
            Assert.AreEqual(ConditionOperator.LessThan, CmbOperator.LessThan.GetOpeator());
            Assert.AreEqual(ConditionOperator.LessEqual, CmbOperator.LessEqual.GetOpeator());
        }
    }
}
