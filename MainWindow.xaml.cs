using Event_Manager_v2;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
                //InitialDirectory = "c:\\",
                Filter = "Event files (*.Evt)|*.Evt|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                //Get the path of specified file
                //filePath = openFileDialog.FileName;

                //Read the contents of the file into a stream
                var fileStream = openFileDialog.OpenFile();

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    fileContent = reader.ReadToEnd();
                }
                StringReader ReaderOfEventNotes = new(fileContent);

                if(ListOfEvents.Count > 0)
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

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (StreamWriter writer = new StreamWriter("test.evt"))
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
            //MessageBox.Show(ListOfEvents.);
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
            var glossaryWindow= new GlossaryWindow { Owner = this };
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
    }
}
 
        
    



