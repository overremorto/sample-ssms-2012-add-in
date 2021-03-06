using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using Microsoft.SqlServer.Management.UI.VSIntegration;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using SSMSBoost.Commands;
using Microsoft.SqlServer.Management.UI.VSIntegration.Editors;
using SqlParser;
using System.Data.SqlClient;

namespace SSMSAddin
{
    /// <summary>The object for implementing an Add-in.</summary>
    /// <seealso class='IDTExtensibility2' />
    public class Connect : IDTExtensibility2, IDTCommandTarget
    {
        /// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
        public Connect()
        {
        }

        /// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
        /// <param term='application'>Root object of the host application.</param>
        /// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
        /// <param term='addInInst'>Object representing this Add-in.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
        {
            //var namesToLookFor = new List<string>()
            //{
            //    //"Cop&y",
            //    //"Copy with Headers",
            //    //"Select &All",
            //    //"Sa&ve Results As",
            //    "Page Set&up",
            //    //"&Print"
            //};
            _applicationObject = (DTE2)application;
            _addInInstance = (AddIn)addInInst;
            if (connectMode == ext_ConnectMode.ext_cm_Startup)
            {
                object[] contextGUIDS = new object[] { };
                Commands2 commands = (Commands2)_applicationObject.Commands;
                string toolsMenuName = "Tools";

                foreach (Command c in commands)
                {

                }

                //Place the command on the tools menu.
                //Find the MenuBar command bar, which is the top-level command bar holding all the main menu items:
                Microsoft.VisualStudio.CommandBars.CommandBar menuBarCommandBar = ((Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)["MenuBar"];
                CommandBar resultGridCommandBar = ((Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)["SQL Results Grid Tab Context"];
                //var popup = resultGridCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, 1, true) as CommandBarPopup;
                //popup.Caption = "TSET BUTTON";
                //foreach (CommandBar commandBar in (Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)
                //{
                //    if (commandBar.Controls.Cast<CommandBarControl>().Any(c => namesToLookFor.Any(n => c.Caption.Contains(n))))
                //    {
                //        var commandNames = commandBar.Controls.Cast<CommandBarControl>().Select(c => c.Caption).ToList();
                //        int stop = 1;
                //        stop = 2;
                //    }
                //}

                //Find the Tools command bar on the MenuBar command bar:
                CommandBarControl toolsControl = menuBarCommandBar.Controls[toolsMenuName];
                CommandBarPopup toolsPopup = (CommandBarPopup)toolsControl;

                _applicationObject.Events.WindowEvents.WindowCreated += WindowEvents_WindowCreated;

                //This try/catch block can be duplicated if you wish to add multiple commands to be handled by your Add-in,
                //  just make sure you also update the QueryStatus/Exec method to include the new command names.
                try
                {
                    //Add a command to the Commands collection:
                    Command command = commands.AddNamedCommand2(_addInInstance, "ViewReferencedValue", "View Referenced Value", "Creates a new query to view the row of the referenced value", true, 59, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

                    ////Add a control for the command to the tools menu:
                    //if ((command != null) && (toolsPopup != null))
                    //{
                    //	command.AddControl(toolsPopup.CommandBar, 1);
                    //}
                    //foreach (CommandBar commandBar in (Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)
                    //{
                    //    //if (commandBar.Controls.Cast<CommandBarControl>().Any(c => namesToLookFor.Any(n => c.Caption.Contains(n))))
                    //    //{
                    //    //    var commandNames = commandBar.Controls.Cast<CommandBarControl>().Select(c => c.Caption).ToList();
                    //    //    int stop = 1;
                    //    //    stop = 2;
                    //    //}
                    //    command.AddControl(commandBar, 1);
                    //}
                    command.AddControl(resultGridCommandBar, 1);
                }
                catch (System.ArgumentException ex)
                {
                    //If we are here, then the exception is probably because a command with that name
                    //  already exists. If so there is no need to recreate the command and we can 
                    //  safely ignore the exception.
                }
            }
        }

        /// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
        /// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
        {
            Commands2 commands = (Commands2)_applicationObject.Commands;
            try
            {
                Command addinCommand = commands.Item("SSMSAddin.Connect.ViewReferencedValue");
                addinCommand.Delete();
            }
            catch (System.ArgumentException e)
            {
                System.Diagnostics.Debug.Print("Error deleting commands on disconnection!");
            }

        }

        /// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />		
        public void OnAddInsUpdate(ref Array custom)
        {
        }

        /// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnStartupComplete(ref Array custom)
        {
        }

        /// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnBeginShutdown(ref Array custom)
        {
        }

        /// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
        /// <param term='commandName'>The name of the command to determine state for.</param>
        /// <param term='neededText'>Text that is needed for the command.</param>
        /// <param term='status'>The state of the command in the user interface.</param>
        /// <param term='commandText'>Text requested by the neededText parameter.</param>
        /// <seealso class='Exec' />
        public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
        {
            if (neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
            {
                if (commandName == "SSMSAddin.Connect.ViewReferencedValue")
                {
                    status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                    return;
                }
            }
        }


        private void WindowEvents_WindowCreated(Window window)
        {
            var ctrl = Interop.GetControl(window.HWnd, "SqlScriptEditorControl");
            var scriptWindow = new ScriptWindow(ctrl, window);
            _windowExtensions[window] = scriptWindow;
        }


        /// <summary>Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.</summary>
        /// <param term='commandName'>The name of the command to execute.</param>
        /// <param term='executeOption'>Describes how the command should be run.</param>
        /// <param term='varIn'>Parameters passed from the caller to the command handler.</param>
        /// <param term='varOut'>Parameters passed from the command handler to the caller.</param>
        /// <param term='handled'>Informs the caller if the command was handled or not.</param>
        /// <seealso class='Exec' />
        public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
        {
            handled = false;
            if (executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
            {
                //ext.Documents.Add();
                //ext.Documents.Add(ext.ActiveDocument.Kind.ToString().Replace("{", "").Replace("}", ""));
                //ext.WindowConfigurations.Add("test window");
                //ext.ExecuteCommand("QueryDesigner.Select");
                //ext.ExecuteCommand("QueryDesigner.SQL");
                //ext.ExecuteCommand("QueryDesigner.ExecuteSQL");
                if (commandName == "SSMSAddin.Connect.ViewReferencedValue")
                {
                    var sqlParser = new SimpleSqlParser();
                    var ext = ((DTE2)ServiceCache.ExtensibilityModel);
                    IScriptFactory scriptFactory = ServiceCache.ScriptFactory;
                    var window = (Window)ext.ActiveWindow;
                    var scriptWindow = _windowExtensions.ContainsKey(window) ? _windowExtensions[window] : null;
                    var values = scriptWindow?.GetSelectedValues();
                    foreach (var val in values.Where(v => v?.Values?.Count > 0))
                    {

                        var s = scriptFactory.CreateNewBlankScript(ScriptType.Sql, scriptFactory.CurrentlyActiveWndConnectionInfo.UIConnectionInfo, null);

                        Document document = ext.ActiveDocument;
                        if (document != null)
                        {

                            var sql = scriptWindow.GetQuery();
                            var table = sqlParser.GetTableFromSql(sql, val.ColumnName);
                            var split = table.Split('.');
                            var schema = "dbo";
                            if (split.Length == 3)
                            {
                                schema = split[1].Replace("[", "").Replace("]", "");
                                table = split[2].Replace("[", "").Replace("]", "");
                            }
                            else if (split.Length == 2)
                            {
                                schema = split[0].Replace("[", "").Replace("]", "");
                                table = split[1].Replace("[", "").Replace("]", "");

                            }
                            else if (split.Length == 1)
                            {
                                table = split[0].Replace("[", "").Replace("]", "");
                            }

                            var sqlToGetReferences = $@"
SELECT S2.name AS SchemaName, T2.name AS TableName, C2.name AS ColumnName
FROM sys.tables	T
JOIN sys.schemas S ON S.schema_id = T.schema_id
JOIN sys.columns C ON C.object_id = T.object_id
JOIN sys.foreign_key_columns FK ON FK.parent_object_id = T.object_id AND FK.parent_column_id = C.column_id
JOIN sys.tables T2 ON T2.object_id = FK.referenced_object_id
JOIN sys.columns C2 ON C2.object_id = T2.object_id AND C2.column_id = FK.referenced_column_id
JOIN sys.schemas S2 ON S2.schema_id = T2.schema_id
WHERE T.name = '{table}' AND S.name = '{schema}' AND C.name = '{val.ColumnName}'
";
                            var conInfo = scriptFactory.CurrentlyActiveWndConnectionInfo.UIConnectionInfo;
                            SqlCommand cmd = new SqlCommand(sqlToGetReferences, new SqlConnection($"Server={conInfo.ServerNameNoDot};Database={conInfo.AdvancedOptions["DATABASE"]};Trusted_Connection=True;"));
                            cmd.Connection.Open();
                            var referencedTable = string.Empty;
                            var referencedSchema = string.Empty;
                            var referencedColumn = string.Empty;
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    referencedSchema = (string)reader["SchemaName"];
                                    referencedTable = (string)reader["TableName"];
                                    referencedColumn = (string)reader["ColumnName"];
                                }
                            }
                            cmd.Connection.Close();
                            //replace currently selected text
                            if (!string.IsNullOrEmpty(referencedSchema) && !string.IsNullOrEmpty(referencedTable) && !string.IsNullOrEmpty(referencedColumn))
                            {
                                TextSelection selection = (TextSelection)document.Selection;
                                selection.Insert($"SELECT * FROM [{referencedSchema}].[{referencedTable}] WHERE [{referencedColumn}] {val?.WhereClause()}", (Int32)EnvDTE.vsInsertFlags.vsInsertFlagsContainNewText);

                                ext.ExecuteCommand("Query.Execute");
                            }
                        }
                    }

                    handled = true;
                    return;
                }
            }
        }


        private DTE2 _applicationObject;
        private AddIn _addInInstance;

        private Dictionary<Window, ScriptWindow> _windowExtensions = new Dictionary<Window, ScriptWindow>();
    }
}
