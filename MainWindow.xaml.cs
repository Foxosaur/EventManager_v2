using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
        public string[] FunctionParams = Array.Empty<string>();
        public string ParamAsString;


        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void LoadEVTFile_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Menu item clicked");

            OpenFileDialog openFileDialog = new()
            {
                InitialDirectory = "c:\\",
                Filter = "Event files (*.Evt)|*.Evt|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                //Get the path of specified file
                filePath = openFileDialog.FileName;

                //Read the contents of the file into a stream
                var fileStream = openFileDialog.OpenFile();

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    fileContent = reader.ReadToEnd();
                }
                StringReader ReaderOfEventNotes = new(fileContent);
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


                            FunctionParams = line.Split(' ').Skip(2).ToArray();
                        }
                        if (line.StartsWith(";"))
                        {
                            LineEvent.Note = line;
                        }
                        if (TypeFunction != '-')
                        {
                            LineEvent.ID = EventNum;
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
                        //if (item.Note.StartsWith(";"))
                        //  {
                        //writer.WriteLine(item.Note);
                        //   }
                        //else
                        // {
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
    }
}
