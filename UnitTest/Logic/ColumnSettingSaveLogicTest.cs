using System;
using System.Windows.Forms;
using DynamicsDataExplorer.Logic;

using NUnit.Framework;

namespace UnitTest.Logic
{
    [TestFixture]
    class ColumnSettingSaveLogicTest
    {
        private ListBox _list;
        private DataGridView _grid;

        [TestCase]
        public void ColumnSettingSaveLogicTest_SaveLoad01()
        {
            ColumnSettingSaveLogic cls = new ColumnSettingSaveLogic(_list, _grid);
            cls.Save("testEntity");

            _list.Items[0] = "item2";
            _list.Items[1] = "item1";

            _grid.Columns[0].DataPropertyName = "prop2";
            _grid.Columns[0].Name = "name2";
            _grid.Columns[0].HeaderText = "header2";
            _grid.Columns[1].DataPropertyName = "prop1";
            _grid.Columns[1].Name = "name1";
            _grid.Columns[1].HeaderText = "header1";

            Assert.True(cls.Load("testEntity"));

            Assert.AreEqual("item1", _list.Items[0].ToString());
            Assert.AreEqual("item2", _list.Items[1].ToString());
            Assert.AreEqual("item3", _list.Items[2].ToString());

            Assert.AreEqual("name1", _grid.Columns[0].Name);
            Assert.AreEqual("prop1", _grid.Columns[0].DataPropertyName);
            Assert.AreEqual("header1", _grid.Columns[0].HeaderText);
            Assert.AreEqual("name2", _grid.Columns[1].Name);
            Assert.AreEqual("prop2", _grid.Columns[1].DataPropertyName);
            Assert.AreEqual("header2", _grid.Columns[1].HeaderText);
            Assert.AreEqual("name3", _grid.Columns[2].Name);
            Assert.AreEqual("prop3", _grid.Columns[2].DataPropertyName);
            Assert.AreEqual("header3", _grid.Columns[2].HeaderText);
        }

        [TestCase]
        public void ColumnSettingSaveLogicTest_LoadErr01()
        {
            ColumnSettingSaveLogic cls = new ColumnSettingSaveLogic(_list, _grid);
            Assert.False(cls.Load("testEntity_notExist"));
        }

        [SetUp]
        public void SetUp()
        {
            _list = new ListBox();
            _list.Items.Add("item1");
            _list.Items.Add("item2");
            _list.Items.Add("item3");

            DataGridViewColumn col1 = new DataGridViewTextBoxColumn();
            col1.Name = "name1";
            col1.DataPropertyName = "prop1";
            col1.HeaderText = "header1";
            DataGridViewColumn col2 = new DataGridViewTextBoxColumn();
            col2.Name = "name2";
            col2.DataPropertyName = "prop2";
            col2.HeaderText = "header2";
            DataGridViewColumn col3 = new DataGridViewTextBoxColumn();
            col3.Name = "name3";
            col3.DataPropertyName = "prop3";
            col3.HeaderText = "header3";
            _grid = new DataGridView();
            _grid.Columns.Add(col1);
            _grid.Columns.Add(col2);
            _grid.Columns.Add(col3);
        }
    }
}
