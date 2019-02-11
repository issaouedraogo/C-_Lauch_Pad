using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace LauchPad
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        List<string> folderList = new List<string>();
        List<string> fileList = new List<string>();

        public void RessourceToListAndListBox(StreamReader sr, ListBox lb, List<string> ls)
        {
            lb.Items.Clear();
            ls.Clear();
            while(sr.Peek() > -1)
            {
                ls.Add(sr.ReadLine());
            }
            lb.Items.AddRange(ls.ToArray());
        }

        public void RefreshListBox(ListBox lb, string[] sa)
        {
            lb.Items.Clear();
            lb.Items.AddRange(sa);
        }
        public void UpdateRessourceFiles(string[] sa, string ressourcePath)
        {
            StreamWriter sw = new StreamWriter(ressourcePath, false);
            foreach (string line in sa)
            {
                sw.WriteLine(line);
            }
            sw.Close();
        }
        public void ListStringAddToOrSwitchToFirst(List<string> lst, string item)
        {
            if (lst.Contains(item))
            {
                lst.Remove(item);
            }
            lst.Insert(0, item);
        }
       

        private void Form1_Load(object sender, EventArgs e)
        {

            string specialFolder = AppDomain.CurrentDomain.BaseDirectory + @"LauchPadFiles";
            string MRUfilesPath = specialFolder + @"\MRUfiles.txt";
            string MRUfoldersPath = specialFolder + @"\MRUfolders.txt";

            if (!Directory.Exists(specialFolder))
            {
                Directory.CreateDirectory(specialFolder);
                File.CreateText(MRUfilesPath);
                File.CreateText(MRUfoldersPath);
            }

            StreamReader folderSR = File.OpenText(MRUfoldersPath);
            RessourceToListAndListBox(folderSR, lstFolders, folderList);

            StreamReader filesSR = File.OpenText(MRUfilesPath);
            RessourceToListAndListBox(filesSR, lstFiles, fileList);
            folderSR.Close();
            filesSR.Close();
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            OpenFileDialog oFile = new OpenFileDialog();
            oFile.Title = "Choose the file to be launched.";
            oFile.InitialDirectory = Environment.SpecialFolder.MyComputer.ToString();
            if(oFile.ShowDialog() == DialogResult.OK)
            {
                String newFile = oFile.FileName;
                ListStringAddToOrSwitchToFirst(fileList, newFile);
                RefreshListBox(lstFiles, fileList.ToArray());

                // get the new folder Path

                int lastSlash = newFile.LastIndexOf("\\");
                string newFolder = newFile.Substring(0, lastSlash);

                // for it is a root folder, slash\ is added for the sake consistency.

                if (!newFolder.Contains("\\"))
                {
                    newFolder = newFolder + "\\";
                    
                }
                ListStringAddToOrSwitchToFirst(folderList, newFolder);
                RefreshListBox(lstFolders, folderList.ToArray());

                // launch the user selected file
                Process pro = new Process();
                pro.StartInfo.FileName = newFile;
                pro.StartInfo.UseShellExecute = true;
                pro.Start();
            }
        }

        private void lstFolders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(lstFolders.SelectedItem != null)
            {
                if (Directory.Exists(lstFolders.SelectedItem.ToString()))
                {
                    // open Folder in window Explorer
                    Process.Start(@lstFolders.SelectedItem.ToString());
                    ListStringAddToOrSwitchToFirst(folderList, lstFolders.SelectedItem.ToString());
                    RefreshListBox(lstFolders, folderList.ToArray());
                }
                else
                {
                    folderList.Remove(lstFolders.SelectedItem.ToString());
                    RefreshListBox(lstFolders, folderList.ToArray());
                }
            }
        }

        private void lstFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
           if(lstFiles.SelectedItem != null)
            {
                if (File.Exists(lstFiles.SelectedItem.ToString()))
                {
                    // launch the selected files
                    Process pro = new Process();
                    pro.StartInfo.FileName = lstFiles.SelectedItem.ToString();
                    pro.StartInfo.UseShellExecute = true;
                    pro.Start();
                    ListStringAddToOrSwitchToFirst(fileList, lstFiles.SelectedItem.ToString());
                    RefreshListBox(lstFiles, fileList.ToArray());
                }
                else
                {
                    fileList.Remove(lstFolders.SelectedItem.ToString());
                    RefreshListBox(lstFiles, fileList.ToArray());
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            string specialFolder = AppDomain.CurrentDomain.BaseDirectory + @"LauchPadFiles";
            string MRUfilesPath = specialFolder + @"\MRUfiles.txt";
            string MRUfoldersPath = specialFolder + @"\MRUfolders.txt";
            UpdateRessourceFiles(fileList.ToArray(), MRUfilesPath);
            UpdateRessourceFiles(folderList.ToArray(), MRUfoldersPath);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
