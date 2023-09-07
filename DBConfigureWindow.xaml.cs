using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Event_Manager_v2
{
    /// <summary>
    /// Interaction logic for DBConfigureWindow.xaml
    /// </summary>
    public partial class DBConfigureWindow : Window
    {
        public DBConfigureWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Test_DBConnect_Click(object sender, RoutedEventArgs e)
        {
            if (DBHostServer_text.Text == "" || DBName_text.Text == "" || DBUsername_text.Text == "" || DBPassword_text.Text == "")
            {
                MessageBox.Show("You are missing some data");
            }
            else
            {
                //test the connection
                try
                {
                    SqlConnection thisConnection = new SqlConnection(@"Server=" + DBHostServer_text.Text + ";Database=" + DBName_text.Text + ";User Id=" + DBUsername_text.Text + ";Password=" + DBPassword_text.Text + ";Trusted_connection=True;Encrypt=False");
                    thisConnection.Open();

                    //string Get_Data = "SELECT * FROM NPCCHAT";

                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM NPCCHAT", thisConnection);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "NPCCHAT");
                    NPCChat_datagrid.ItemsSource = ds.Tables["NPCCHAT"].DefaultView;
                }
                catch (Exception ex)
                {

                    MessageBox.Show("db error: " + ex);

                }
            }
        }

        private void DBHostServer_text_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void DBName_text_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void DBUsername_text_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void DBPassword_text_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
