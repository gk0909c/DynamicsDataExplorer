using DynamicsDataExplorer.Bean;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;

namespace UnitTest.Bean
{
    [TestFixture]
    public class CmbBeanTest
    {
        private const int LANG_CODE = 1041;

        [TestCase]
        public void CmbBeanTest01()
        {
            Label label = new Label();
            label.UserLocalizedLabel = new LocalizedLabel("ラベル", LANG_CODE);
            CmbBean cls = new CmbBean("logicalName", label);

            Assert.AreEqual("logicalName", cls.LogicalName);
            Assert.AreEqual("ラベル（logicalName）", cls.DisplayName);

        }

        [TestCase]
        public void CmbBeanTest02()
        {
            Label label = new Label();
            CmbBean cls = new CmbBean("logicalName", label);

            Assert.AreEqual("logicalName", cls.LogicalName);
            Assert.AreEqual("logicalName", cls.DisplayName);

        }
    }
}
