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
        Registers register;
        private static string openPath;

        public Form1()
        {
            InitializeComponent();
            register = new Registers();
            // RegisterMachine Rm = new RegisterMachine(new List<string>() {"test:", "store 1", "cadd 1", "goto test", "kevin:", "// testkommentar", "label:", "goto 2"});
        }

        // Event Handler for the "Speichern als.." item from the ToolStripMenu
        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get path from SaveFileDialog, save TextEditorBox content at path
            string s = Dialog.SaveFileDialog();
            openPath = s;

            if (s != null)
                FileHandler.SaveFileContent(s, TextEditorBox.Text);

        }

        // Event Handler for the "Speichern" item from the ToolStripMenu
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Execute SaveAs method when openPath not initialized
            if (openPath != null)
                FileHandler.SaveFileContent(openPath, TextEditorBox.Text);
            else
                SaveAsToolStripMenuItem_Click(null, null);
        }

        // Event Handler for the "Öffnen" item from the ToolStripMenu
        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get file path from LoadFileDialog, read file from path and set TextEditorBox.Text to Filetext
            string s = Dialog.LoadFileDialog();
            openPath = s;

            if(s != null)
                TextEditorBox.Text = String.Join(System.Environment.NewLine, FileHandler.GetFileContent(s));
            
        }

        // Event Handler for the "Neu" item from the ToolStripMenu, resets all variables and TextEditorBox.Text
        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openPath = null;
            TextEditorBox.Text = "";
        }

        // Event Handler for the "Schließen" item from the ToolStripMenu
        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            RegisterMachine Rm = new RegisterMachine(new List<string>() { "test:", "store 1", "cadd 1", "goto test", "kevin:", "// testkommentar", "label:", "goto 2" });
        }
    }
}
