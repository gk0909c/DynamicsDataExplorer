using System;
using System.Windows.Forms;
using System.Collections.Generic;

using DynamicsDataExplorer.Bean;
using DynamicsDataExplorer.Logic;
using DynamicsDataExplorer.Constants;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using NUnit.Framework;

namespace UnitTest.Logic
{
    [TestFixture]
    class QueryFormLogicTest
    {
        private const int LANG_CODE = 1041;
        private AttributeMetadata[] _attributes = new AttributeMetadata[0];

        [TestCase]
        public void QueryFormLogicTest_SetEntityCmb01()
        {
            ComboBox cmb = new ComboBox();

            EntityMetadata[] entities = new EntityMetadata[]
            {
                new EntityMetadata(){
                    LogicalName = "entity1",
                    DisplayName = new Microsoft.Xrm.Sdk.Label("Disp1", LANG_CODE),
                    IsCustomizable = new BooleanManagedProperty(true)
                },
                new EntityMetadata(){
                    LogicalName = "entity2",
                    DisplayName = new Microsoft.Xrm.Sdk.Label("Disp2", LANG_CODE),
                    IsCustomizable = new BooleanManagedProperty(false)
                },
                new EntityMetadata(){
                    LogicalName = "entity0",
                    DisplayName = new Microsoft.Xrm.Sdk.Label(new LocalizedLabel("Disp0", LANG_CODE), null),
                    IsCustomizable = new BooleanManagedProperty(true)
                }
            };

            QueryFormLogic cls = new QueryFormLogic();
            cls.SetEntityCmb(entities, cmb);
            List<CmbBean> list = cmb.DataSource as List<CmbBean>;

            Assert.NotNull(list);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("entity0", list[0].LogicalName);
            Assert.AreEqual("Disp0（entity0）", list[0].DisplayName);
            Assert.AreEqual("entity1", list[1].LogicalName);
            Assert.AreEqual("entity1", list[1].DisplayName);
        }

        [TestCase]
        public void QueryFormLogicTest_SetDataGridColumns01()
        {
            DataGridView grid = new DataGridView();

            QueryFormLogic cls = new QueryFormLogic();
            cls.SetDataGridColumns(_attributes, grid);

            Assert.AreEqual(3, grid.ColumnCount);
            Assert.AreEqual("attr1", grid.Columns[0].DataPropertyName);
            Assert.AreEqual("attr1", grid.Columns[0].Name);
            Assert.AreEqual("Disp1", grid.Columns[0].HeaderText);
            Assert.AreEqual("attr3", grid.Columns[1].DataPropertyName);
            Assert.AreEqual("attr3", grid.Columns[1].Name);
            Assert.AreEqual("Disp3", grid.Columns[1].HeaderText);
            Assert.AreEqual("attr4", grid.Columns[2].DataPropertyName);
            Assert.AreEqual("attr4", grid.Columns[2].Name);
            Assert.AreEqual("Disp4", grid.Columns[2].HeaderText);
        }

        [TestCase]
        public void QueryFormLogicTest_SetAttributeCmb01()
        {
            ComboBox cmb = new ComboBox();

            QueryFormLogic cls = new QueryFormLogic();
            cls.SetAttributeCmb(_attributes, cmb);
            List<CmbBean> list = cmb.DataSource as List<CmbBean>;

            Assert.NotNull(list);
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual("attr2", list[0].LogicalName);
            Assert.AreEqual("attr2", list[0].DisplayName);
            Assert.AreEqual("attr1", list[1].LogicalName);
            Assert.AreEqual("Disp1（attr1）", list[1].DisplayName);
            Assert.AreEqual("attr3", list[2].LogicalName);
            Assert.AreEqual("Disp3（attr3）", list[2].DisplayName);
            Assert.AreEqual("attr4", list[3].LogicalName);
            Assert.AreEqual("Disp4（attr4）", list[3].DisplayName);
        }

        [TestCase]
        public void QueryFormLogicTest_SetDataGridValues01()
        {
            List<Entity> entities = new List<Entity>()
            {
                new Entity()
                {
                    Id = new Guid("10000000-0000-0000-0000-000000000001"),
                    Attributes = new AttributeCollection()
                    {
                        {"attr1", "strVal1"},
                        {"attr2", new Money(1000)},
                        {"attr3", new OptionSetValue(1)},
                        {"attr4", new EntityReference("Ref1", new Guid("20000000-0000-0000-0000-000000000001"))}
                    }
                },
                new Entity()
                {
                    Id = new Guid("10000000-0000-0000-0000-000000000002"),
                    Attributes = new AttributeCollection()
                    {
                        {"attr1", "strVal2"},
                        {"attr2", new Money(2000)},
                        {"attr3", new OptionSetValue(2)},
                        {"attr4", new EntityReference("Ref2", new Guid("20000000-0000-0000-0000-000000000002"))}
                    }
                }
            };
            EntityCollection records = new EntityCollection(entities);

            DataGridView grid = new DataGridView();
            grid.AllowUserToAddRows = false;

            _attributes[1].DisplayName = new Microsoft.Xrm.Sdk.Label(new LocalizedLabel("Disp2", LANG_CODE), null);

            QueryFormLogic cls = new QueryFormLogic();
            cls.SetDataGridColumns(_attributes, grid);
            cls.SetOptionSetData(_attributes);
            cls.SetDataGridValues(records, grid);

            Assert.AreEqual(2, grid.RowCount);
            
            Assert.AreEqual("strVal1", grid.Rows[0].Cells[0].Value);
            Assert.AreEqual(1000, grid.Rows[0].Cells[1].Value);
            Assert.AreEqual("list1", grid.Rows[0].Cells[2].Value);
            Assert.AreEqual(new Guid("20000000-0000-0000-0000-000000000001"), grid.Rows[0].Cells[3].Value);

            Assert.AreEqual("strVal2", grid.Rows[1].Cells[0].Value);
            Assert.AreEqual(2000, grid.Rows[1].Cells[1].Value);
            Assert.AreEqual("list2", grid.Rows[1].Cells[2].Value);
            Assert.AreEqual(new Guid("20000000-0000-0000-0000-000000000002"), grid.Rows[1].Cells[3].Value);
        }

        [TestCase]
        public void QueryFormLogicTest_CreateCondition01()
        {
            QueryFormLogic cls = new QueryFormLogic();

            Assert.NotNull(cls.CreateCondition("attr", CmbOperator.Equal, "val"));
            Assert.Null(cls.CreateCondition("", CmbOperator.Equal, "val"));
            Assert.Null(cls.CreateCondition("attr", CmbOperator.Equal, ""));
            Assert.Null(cls.CreateCondition("", CmbOperator.Equal, ""));
        }

        [SetUp]
        public void SetAttributes()
        {
            _attributes = new AttributeMetadata[]
            {
                new StringAttributeMetadata(){
                    LogicalName = "attr1",
                    DisplayName = new Microsoft.Xrm.Sdk.Label(new LocalizedLabel("Disp1", LANG_CODE), null),
                    IsCustomizable = new BooleanManagedProperty(true)
                },
                new MoneyAttributeMetadata(){
                    LogicalName = "attr2",
                    DisplayName = new Microsoft.Xrm.Sdk.Label("Disp2", LANG_CODE),
                    IsCustomizable = new BooleanManagedProperty(false)
                },
                new PicklistAttributeMetadata(){
                    LogicalName = "attr3",
                    DisplayName = new Microsoft.Xrm.Sdk.Label(new LocalizedLabel("Disp3", LANG_CODE), null),
                    IsCustomizable = new BooleanManagedProperty(true),
                    OptionSet = new OptionSetMetadata()
                    {
                        Name = "optionSet",
                        DisplayName = new Microsoft.Xrm.Sdk.Label(new LocalizedLabel("Opt1", LANG_CODE), null),
                        Options =
                        {
                             new OptionMetadata(new Microsoft.Xrm.Sdk.Label(new LocalizedLabel("list1", LANG_CODE), null), 1),
                             new OptionMetadata(new Microsoft.Xrm.Sdk.Label(new LocalizedLabel("list2", LANG_CODE), null), 2)
                        }
                    }
                },
                new LookupAttributeMetadata()
                {
                    LogicalName = "attr4",
                    DisplayName = new Microsoft.Xrm.Sdk.Label(new LocalizedLabel("Disp4", LANG_CODE), null),
                    IsCustomizable = new BooleanManagedProperty(true),
                    
                }
            };

        }
    }
}
