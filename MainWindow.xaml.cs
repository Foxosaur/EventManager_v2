using Event_Manager_v2;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace WPF_test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            moomin.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(moominDataGrid_PreviewMouseLeftButtonDown);
            moomin.Drop += new DragEventHandler(moominDataGrid_Drop);
        }
        public List<EventLines> ListOfEvents = new();

        public string filePath;
        public string fileContent;
        public string line;
        public bool InsideEvent = false;
        public string EventNum = "";
        public Char TypeFunction = '-';
        public string Function = "";
        public string[] FunctionParams;
        public string ParamAsString = " ";


        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void LoadEVTFile_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Menu item clicked");

            OpenFileDialog openFileDialog = new()
            {
                Filter = "Event files (*.Evt)|*.Evt|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };
            if (openFileDialog.ShowDialog() == true)
            {


                //Read the contents of the file into a stream
                var fileStream = openFileDialog.OpenFile();

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    fileContent = reader.ReadToEnd();
                }
                StringReader ReaderOfEventNotes = new(fileContent);

                if (ListOfEvents.Count > 0)
                {
                    //If we've already loaded a evt file in this session, lets clear the list and refresh the datagrid

                    moomin.SelectedIndex = 0;
                    moomin.ItemsSource = null;

                    ListOfEvents.Clear();


                }
                while ((line = ReaderOfEventNotes.ReadLine()) != null)
                {
                    /*if (line.StartsWith(";"))
                    {
                        textBox1.Text += line.Substring(1) + "\r\n";
                    }*/
                    if (InsideEvent == false)
                    {
                        if (line.StartsWith("EVENT "))
                        {
                            InsideEvent = true;
                            EventNum = line.Split(' ').Skip(1).FirstOrDefault();
                            EventLines LineEvent = new();
                            LineEvent.ID = EventNum;
                            LineEvent.Function = "EVENT " + EventNum;
                            ListOfEvents.Add(LineEvent);
                        }
                    }
                    if (InsideEvent == true && (line.StartsWith("A") || (line.StartsWith("O") || line.StartsWith("E ") || line.StartsWith(";")))) // Watch the trailing space of the startswith("E_") its needed.
                    {

                        EventLines LineEvent = new();
                        if (line.StartsWith("A") || (line.StartsWith("O") || line.StartsWith("E ")))
                        {
                            TypeFunction = line[0];
                            Function = line.Split(' ').Skip(1).FirstOrDefault();

                            //FunctionParams = (String[])line.Split(' ').Skip(2).DefaultIfEmpty("");
                            FunctionParams = line.Split(' ').Skip(2).ToArray();
                            if (FunctionParams.Length <= 0 || FunctionParams is null)
                            { FunctionParams.Append("-"); }
                        }

                        //BELOW WE SETUP THE custom EVENT line ready to be added to the list 
                        if (line.StartsWith(";"))
                        {
                            LineEvent.Note = line;
                        }
                        if (TypeFunction != '-') // '-' is a holding char as it can't be null....so if it's '-' it's going to be a EVENT XXX or END line
                        {
                            //LineEvent.ID = EventNum;
                            LineEvent.TypeFunction = TypeFunction;
                            LineEvent.Function = Function;
                            LineEvent.FunctionParams = FunctionParams;
                        }
                        else
                        {
                            LineEvent.ID = EventNum;
                            //LineEvent.TypeFunction = TypeFunction;
                            LineEvent.Function = Function;
                            LineEvent.FunctionParams = FunctionParams;
                        }
                        //End of construction ^

                        ListOfEvents.Add(LineEvent);
                    }


                    if (line == "END")
                    {

                        InsideEvent = false;
                        EventLines LineEvent = new();
                        LineEvent.ID = EventNum;
                        LineEvent.Function = "END";
                        ListOfEvents.Add(LineEvent);
                    }
                }
                ReaderOfEventNotes.Dispose();
                moomin.ItemsSource = ListOfEvents;
            }
            //loading done
        }

        private void SavetoFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Event files (*.Evt)|*.Evt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.Title = "Save an Myth of Soma event file";
            saveFileDialog1.OverwritePrompt = true;
            if (saveFileDialog1.ShowDialog() == true) ;
            {

                // If the file name is not an empty string open it for saving.
                if (saveFileDialog1.FileName != "")
                {
                    using (StreamWriter writer = new StreamWriter(saveFileDialog1.FileName))
                    {
                        foreach (EventLines item in ListOfEvents)
                        {

                            if (item.Function.StartsWith("EVENT ") || item.Function.StartsWith("END"))
                            {
                                writer.WriteLine(item.Function);
                                if (item.Function.StartsWith("EVENT "))
                                {
                                    //  writer.WriteLine(item.Note + "\r\n");
                                }
                                if (item.Function.StartsWith("END"))
                                {
                                    writer.WriteLine("\r\n");
                                }
                            }
                            else
                            {
                                if (item.Note is not null)
                                {
                                    if (item.Note.StartsWith(";"))
                                    {
                                        writer.WriteLine(item.Note);
                                    }
                                }


                                ParamAsString = "";

                                foreach (string param in item.FunctionParams)
                                {
                                    ParamAsString += param + " ";
                                }

                                ParamAsString.Trim();

                                writer.WriteLine(item.TypeFunction + " " + item.Function + " " + ParamAsString.ToString());
                            }

                        }
                    }
                }
                //MessageBox.Show(ListOfEvents.);
            }
        }

        private void LoadGlossaryHelpFile_Click(object sender, RoutedEventArgs e)
        {
            string GlossaryPath = "eventglossary.txt";

            //Read the contents of the file into a stream
            var fileStream = GlossaryPath;

            using (StreamReader reader = new StreamReader(fileStream))
            {
                fileContent = reader.ReadToEnd();
            }
            StringReader ReaderOfGlossary = new(fileContent);
            var glossaryWindow = new GlossaryWindow { Owner = this };
            glossaryWindow.Show();
            glossaryWindow.GlossaryTextXox.Text = ReaderOfGlossary.ReadToEnd();
        }

        private void moomin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void LoadDB_NPC_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Config_DB_Click(object sender, RoutedEventArgs e)
        {
            var dBConfigureWindow = new DBConfigureWindow { Owner = this };
            dBConfigureWindow.Show();
            //glossaryWindow.GlossaryTextXox.Text = ReaderOfGlossary.ReadToEnd();
        }

        private void ListView_SelectionChanged()
        {

        }

        /*  private void AddRow_Click(object sender, RoutedEventArgs e)
          {

              if (ListOfEvents is not null)
              {
                  moomin.SelectedIndex = 0;
                  moomin.ItemsSource = null;

                  //ListOfEvents.Clear();
                  EventLines AddingRow = new EventLines();
                  AddingRow.Note = "; This is your new event line";
                  ListOfEvents.Add(AddingRow);

                  moomin.ItemsSource = ListOfEvents;

              }
          }*/
        public delegate Point GetPosition(IInputElement element);
        int rowIndex = -1;


        void moominDataGrid_Drop(object sender, DragEventArgs e)
        {
            if (ListOfEvents is not null)
            {
                if (rowIndex < 0)
                    return;
                int index = this.GetCurrentRowIndex(e.GetPosition);
                if (index < 0)
                    return;
                if (index == rowIndex)
                    return;
                if (index == moomin.Items.Count - 1)
                {
                    MessageBox.Show("This row-index cannot be drop");
                    return;
                }
                EventLines changedProduct = ListOfEvents[rowIndex];
                ListOfEvents.RemoveAt(rowIndex);
                ListOfEvents.Insert(index, changedProduct);
                moomin.ItemsSource = null;
                moomin.ItemsSource = ListOfEvents;
            }
        }

        void moominDataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            rowIndex = GetCurrentRowIndex(e.GetPosition);
            if (rowIndex < 0)
                return;
            moomin.SelectedIndex = rowIndex;
            EventLines selectedeventline = (EventLines)moomin.Items[rowIndex];
            if (selectedeventline == null)
                return;
            DragDropEffects dragdropeffects = DragDropEffects.Move;
            if (DragDrop.DoDragDrop(moomin, selectedeventline, dragdropeffects)
                                != DragDropEffects.None)
            {
                moomin.SelectedItem = selectedeventline;
            }
        }

        private bool GetMouseTargetRow(Visual theTarget, GetPosition position)
        {
            Rect rect = VisualTreeHelper.GetDescendantBounds(theTarget);
            Point point = position((IInputElement)theTarget);
            return rect.Contains(point);
        }

        private DataGridRow GetRowItem(int index)
        {
            if (moomin.ItemContainerGenerator.Status
                    != GeneratorStatus.ContainersGenerated)
                return null;
            return moomin.ItemContainerGenerator.ContainerFromIndex(index)
                                                            as DataGridRow;
        }

        private int GetCurrentRowIndex(GetPosition pos)
        {
            int curIndex = -1;
            for (int i = 0; i < moomin.Items.Count; i++)
            {
                DataGridRow itm = GetRowItem(i);
                if (GetMouseTargetRow(itm, pos))
                {
                    curIndex = i;
                    break;
                }
            }
            return curIndex;
        }
        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            int rowindex = row.GetIndex();
            moomin.ItemsSource = null;
            EventLines newitem = new EventLines();
            newitem.Note = ";Your new row - delete this text";
            ListOfEvents.Insert(rowindex + 1, newitem);
            moomin.ItemsSource = ListOfEvents;
        }
        private void AddRow_Btn(object sender, EventArgs e)
        {
            if (moomin.SelectedIndex > 0)
            {
                //DataGridRow row = new();
                int rowindex = moomin.SelectedIndex;
                moomin.ItemsSource = null;
                EventLines newitem = new EventLines();
                newitem.Note = ";Your new row - delete this text";
                ListOfEvents.Insert(rowindex + 1, newitem);
                moomin.ItemsSource = ListOfEvents;
                //Grid.RowDefinitions.Add(new RowDefinition());
            }
            else
            {
                //moomin.SelectedIndex = 0;
                MessageBox.Show("Please select a row first to add new line below");
            }
        }
        private void DeleteRow_Btn(object sender, EventArgs e)
        {
            if (moomin.SelectedIndex > 0)
            {
                //DataGridRow row = new();
                int rowindex = moomin.SelectedIndex;
                moomin.ItemsSource = null;
                //EventLines newitem = new EventLines();
                // newitem.Note = ";Your new row - delete this text";
                ListOfEvents.RemoveAt(rowindex);
                moomin.ItemsSource = ListOfEvents;
                //Grid.RowDefinitions.Add(new RowDefinition());
            }
            else
            {
                //moomin.SelectedIndex = 0;
                MessageBox.Show("Please select a row first to delete");
            }
        }

        private void SearchEvent_Btn(object sender, EventArgs e)
        {
            
            if (SearchField.Text != "")
            {
                string enumber = SearchField.Text; //String to handle the double digit events that use a leading 0
                int rowindex = ListOfEvents.IndexOf(ListOfEvents.Find(item => item.Function.StartsWith("EVENT " + enumber)));
                moomin.SelectedIndex = rowindex;
                moomin.ScrollIntoView(moomin.SelectedItem);
                enumber = "";
                rowindex = 0;
            }
            else
            {
                MessageBox.Show("Please enter a value");
            }
        }

    }
}






