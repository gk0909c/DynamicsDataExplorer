using DynamicsDataExplorer.Bean;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using NUnit.Framework;

namespace UnitTest.Bean
{
    [TestFixture]
    class OptionSetBeanTest
    {
        private const int LANG_CODE = 1041;

        [TestCase]
        public void OptionSetBeanTest01()
        {
            LocalizedLabel lLabel = new LocalizedLabel("Option1", 1041);
            LocalizedLabel lLabel2 = new LocalizedLabel("Option2", LANG_CODE);

            PicklistAttributeMetadata meta = new PicklistAttributeMetadata();
            meta.OptionSet = new OptionSetMetadata()
            {
                Name = "optionSet",
                DisplayName = new Label("optiondisplay", LANG_CODE),
                Options =
                {
                     new OptionMetadata(new Label(lLabel, null), 1),
                     new OptionMetadata(new Label("Option2", LANG_CODE), 2),
                     new OptionMetadata(new Label(lLabel2, null), 3)
                }
            };

            OptionSetBean cls = new OptionSetBean(meta);

            Assert.True(cls.HasOptionSet());
            Assert.AreEqual("Option1", cls.GetValue(1), "ラベルあり");
            Assert.Null(cls.GetValue(2), "ラベルなし");
            Assert.AreEqual("Option2", cls.GetValue(3), "ラベルあり");
        }

        [TestCase]
        public void OptionSetBeanTest02()
        {
            LocalizedLabel lLabel = new LocalizedLabel("Option1", LANG_CODE);
            LocalizedLabel lLabel2 = new LocalizedLabel("Option2", LANG_CODE);

            StateAttributeMetadata meta = new StateAttributeMetadata();
            meta.OptionSet = new OptionSetMetadata()
            {
                Name = "optionSet",
                DisplayName = new Label("optiondisplay", LANG_CODE),
                Options =
                {
                     new OptionMetadata(new Label(lLabel, null), 1),
                     new OptionMetadata(new Label(lLabel2, null), null)
                }
            };

            OptionSetBean cls = new OptionSetBean(meta);

            Assert.True(cls.HasOptionSet());
            Assert.AreEqual("Option1", cls.GetValue(1), "ラベルあり");
            Assert.Null(cls.GetValue(2), "ラベルなし");
        }

        [TestCase]
        public void OptionSetBeanTest03()
        {
            LocalizedLabel lLabel = new LocalizedLabel("Option1", 1041);

            StatusAttributeMetadata meta = new StatusAttributeMetadata();
            meta.OptionSet = new OptionSetMetadata()
            {
                Name = "optionSet",
                DisplayName = new Label("optiondisplay", LANG_CODE),
                Options =
                {
                     new OptionMetadata(new Label(lLabel, null), 1)
                }
            };

            OptionSetBean cls = new OptionSetBean(meta);

            Assert.True(cls.HasOptionSet());
            Assert.AreEqual("Option1", cls.GetValue(1), "ラベルあり");
        }

        [TestCase]
        public void OptionSetBeanTest04()
        {
            LocalizedLabel lLabel = new LocalizedLabel("Option1", 1041);

            StringAttributeMetadata meta = new StringAttributeMetadata();

            OptionSetBean cls = new OptionSetBean(meta);

            Assert.False(cls.HasOptionSet());
        }
    }
}
