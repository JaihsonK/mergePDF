using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using WinRT;

namespace mergePDF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///

    public partial class MainWindow : Window
    {
        const int max_path = 50;

        public string [] paths = new string[max_path];
        public string outfile = "";

        public int find_free_index()
        {
            int res;
            for (res = 0; res < max_path; res++)
                if (paths[res] == "" || paths[res] == " ")
                    return res;

            return -1;
        }

        public void remove_item(object sender, RoutedEventArgs e)
        {
            if (paths_list.SelectedIndex < 0)
            {
                System.Windows.MessageBox.Show("Please select an item to remove");
                return;
            }
            // ListBoxItem lbi = (paths_list.SelectedItem as ListBoxItem);
            paths[paths_list.SelectedIndex] = "";
            paths_list.Items[paths_list.SelectedIndex] = "";
            prog_bar.Value = 0;
        }

        public void clear_all(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < max_path; i++)
            {
                paths_list.Items[i] = "";
                paths[i] = "";
            }
            prog_bar.Value = 0;
        }

        public void merge(object sender, RoutedEventArgs e)
        {
            if (outfile == "")
            {
                System.Windows.MessageBox.Show("Please specify an output file");
                return;
            }
            try
            {
                outfile = filename_out.Text;
                
                PdfDocument output_file = new PdfDocument();
                PdfDocument current;
                prog_bar.Value = 25;

                for (int i = 0; i < max_path; i++)
                {
                    try
                    {
                        if (paths[i] == "" || paths[i] == " ")
                            continue;
                        current = PdfReader.Open(paths[i], PdfDocumentOpenMode.Import);
                        for (int j = 0; j < current.PageCount; j++)
                        {
                            output_file.AddPage(current.Pages[j]);
                        }
                    }
                    catch(Exception ex)
                    {
                        System.Windows.MessageBox.Show("Error with file \"" + paths[i] + "\".\n\n" + ex.Message);
                    }
                }

                if (File.Exists(outfile))
                    File.Delete(outfile);

                output_file.Save(outfile);
                output_file.Close();

                prog_bar.Value = 100;

                //Process.Start(outfile); //for debugging
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        private void browse_file(object sender, RoutedEventArgs e)
        {
            prog_bar.Value = 0;
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".pdf";
            dlg.Filter = "PDF Files (*.pdf)|*.pdf";
            dlg.Multiselect = true;
            dlg.CheckFileExists = true;


            // Display OpenFileDialog by calling ShowDialog method 
            bool result = (bool)dlg.ShowDialog();


            if (result == true)
            { 
                foreach (string filename in dlg.FileNames)
                {
                    int index = find_free_index();
                    if (index < 0)
                        continue; //maxed out!

                    paths[index] = filename;
                    paths_list.Items[index] = Path.GetFileName(filename);
                }

            }
        }

        private void select(object sender, RoutedEventArgs e)
        {
            ListBoxItem lbi = sender as ListBoxItem;
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".pdf";
            dlg.Filter = "PDF Files (*.pdf)|*.pdf";
            dlg.Multiselect = false;
            dlg.CheckFileExists = true;


            // Display OpenFileDialog by calling ShowDialog method 
            bool result = (bool)dlg.ShowDialog();
            if(result == true)
            {
                lbi.IsSelected = true;
                int index = paths_list.SelectedIndex;
                paths_list.Items[index] = Path.GetFileName(dlg.FileName);
                paths[index] = dlg.FileName;
                lbi.IsSelected = false;
            }

        }

        private void browse_file2(object sender, RoutedEventArgs e)
        {
            prog_bar.Value = 0;
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".pdf";
            dlg.Filter = "PDF Files (*.pdf)|*.pdf";
            dlg.Multiselect = false;
            dlg.CheckFileExists = false;


            // Display OpenFileDialog by calling ShowDialog method 
            bool result = (bool)dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                outfile = dlg.FileName;
                filename_out.Text = outfile;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            paths_list.SelectionMode = System.Windows.Controls.SelectionMode.Single;
        }

    }
}
