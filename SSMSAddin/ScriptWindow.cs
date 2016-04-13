using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using EnvDTE;
using Microsoft.SqlServer.Management.UI.Grid;

namespace SSMSAddin
{
    public class ScriptWindow
    {

        //private readonly ILog log = LogManager.GetLogger(typeof(ScriptWindow));
        private readonly Control ctrl;
        private readonly TabControl tabControl;
        private string planText;
        private readonly PropertyInfo isExecutingPi;
        private Timer timer;
        private Control _gridControl;
        private Window _window;
        private string _lastRanSql;

        private BlockOfCellsCollection _currentSelectedBlocks = new BlockOfCellsCollection();
        //private string _selectedValue;

        private List<SelectedCell> _selectedCells;

        public bool IsExecuting
        {
            get
            {
                try
                {
                    return (bool)this.isExecutingPi.GetValue((object)this.ctrl, new object[0]);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public ScriptWindow(Control ctrl, Window window)
        {
            try
            {
                _window = window;
                this.ctrl = ctrl;
                System.Type type = ctrl.GetType();
                PropertyInfo property = type.GetProperty("TabPageHost", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (property != null)
                {
                    TabControl tabControl = property.GetValue((object)ctrl, new object[0]) as TabControl;
                    if (tabControl != null)
                    {
                        this.tabControl = tabControl;
                        this.tabControl.ControlAdded += new ControlEventHandler(this.OnTabAdded);
                    }
                }
                else
                {
                    TabControl control = Interop.GetControl<TabControl>(ctrl);
                    if (control != null)
                    {
                        this.tabControl = control;
                        this.tabControl.ControlAdded += new ControlEventHandler(this.OnTabAdded);
                    }
                }
                this.isExecutingPi = type.GetProperty("IsExecuting", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void OnTabAdded(object sender, ControlEventArgs e)
        {
            try
            {
                if (e.Control == null || this.tabControl == null)
                    return;

                if (_gridControl == null)
                {
                    _lastRanSql = ((TextSelection)_window.Selection).Text;
                    if (string.IsNullOrEmpty(_lastRanSql))
                    {
                        using (var fs = new FileStream(_window.Document.FullName, FileMode.Open))
                        {
                            using (var fileReader = new StreamReader(fs))
                            {
                                _lastRanSql = fileReader.ReadToEnd();
                            }
                        }
                    }
                    foreach (Control ctrl in this.tabControl.Controls)
                    {
                        if (ctrl.Text == "Results")
                        {
                            _gridControl = ctrl;
                            break;
                        }
                    }

                    if (_gridControl != null)
                    {
                        HandleResults(_gridControl);
                    }
                }

                //foreach (Control childCtrl in e.Control.Controls)
                //{
                //    MethodInfo method = childCtrl.GetType().GetMethod("GetShowPlanXml", BindingFlags.Instance | BindingFlags.NonPublic);
                //    if (method != null)
                //        this.GetPlan(method, childCtrl);
                //}
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        private void HandleResults(Control childCtrl)
        {
            try
            {
                _selectedCells = null;
                //_selectedValue = null;

                if (timer != null)
                {
                    this.timer.Stop();
                    this.timer.Dispose();
                }

                this.timer = new Timer()
                {
                    Interval = 500
                };

                var failCount = 0;
                this.timer.Tick += (EventHandler)((s, ee) =>
                {
                    try
                    {
                        var controls = childCtrl.Controls;
                        var grid = controls[0].Controls[0].Controls[0] as GridControl;
                        grid.SelectionChanged += Grid_SelectionChanged;
                        //_selectedValue = grid.GetDataObject(true).GetText();

                        var columns = (StringCollection)grid.GridStorage.GetType().GetProperty("ColumnNames").GetValue(grid.GridStorage);

                        _selectedCells = new List<SelectedCell>() {
                            new SelectedCell()
                            {
                                ColumnName = columns?.Count > 0 ? columns[0] : string.Empty,
                                Values = new List<string>()
                                {
                                    grid.GetDataObject(true).GetText(),
                                }
                            }
                        };
                        if (this.IsExecuting)
                            return;
                        this.timer.Stop();
                        this.timer.Dispose();
                        _gridControl = null;
                        //this.timer = (Timer)null;

                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        if (failCount > 10)
                        {
                            this.timer.Stop();
                            this.timer.Dispose();
                        }
                    }
                });
                this.timer.Start();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void Grid_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (!CompareBlockCollections(_currentSelectedBlocks, args.SelectedBlocks))
            {
                var grid = (sender as GridControl);
                var value = grid.GetDataObject(true).GetText();
                Debug.WriteLine(value);

                var columns = (StringCollection)grid.GridStorage.GetType().GetProperty("ColumnNames").GetValue(grid.GridStorage);

                //_selectedValue = ParseValue(value);
                _selectedCells = new List<SelectedCell>();
                _currentSelectedBlocks.Clear();

                foreach (BlockOfCells block in args.SelectedBlocks)
                {
                    _selectedCells.Add(new SelectedCell()
                    {
                        ColumnName = block.X - 1 < columns?.Count ? columns[block.X - 1] : string.Empty,
                        Values = new List<string>()
                        {
                            ParseValue(value),
                        }
                    });
                    _currentSelectedBlocks.Add(new BlockOfCells(block.Y, block.X));
                }
            }
        }

        public List<SelectedCell> GetSelectedValues()
        {
            return _selectedCells;
        }

        public string GetQuery()
        {
            return _lastRanSql;
        }

        private string ParseValue(string value)
        {
            int i = 0;
            if (int.TryParse(value, out i))
            {
                return value;
            }

            return $"'{value}'";
        }

        private bool CompareBlockCollections(BlockOfCellsCollection collection1, BlockOfCellsCollection collection2)
        {
            if (collection1.Count != collection2.Count)
            {
                return false;
            }

            for (var i = 0; i < collection1.Count; i++)
            {
                var block1 = collection1[i];
                var block2 = collection2[i];

                if (block1.X != block2.X || block1.Y != block2.Y)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
