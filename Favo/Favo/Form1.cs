using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Favo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog Sfw = new SaveFileDialog()
            {
                DefaultExt = ".txt",
                Filter = "All Files|*.*",
                CheckFileExists = true
            };
            
            if(Sfw.ShowDialog() == DialogResult.OK)
            {
                FileHandler.SaveFileContent(Sfw.FileName, textEditorBox.Text);    
                Console.WriteLine(Sfw.FileName);

            }
        }
    }
}
