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
        public Point mouseLocation;
        Registers register;
        private static string openPath;
        private DataTable dt;
        bool saved = true;

        // custom colorTable class for MenuStrip (+ legacy)
        public class CustomColorTable : ProfessionalColorTable
        {
            public override Color MenuItemSelected { get { return Color.FromArgb(44, 47, 51); } }
            //public override Color MenuBorder {get {return Color.FromArgb(44,47,51);}}
            public override Color MenuItemBorder { get { return Color.FromArgb(114, 137, 218); } }

            
            

        }

        // Constructor, initialize important components and variables 
        public Form1()
        {

            InitializeComponent();
            menuStrip1.Renderer = new ToolStripProfessionalRenderer(new CustomColorTable());
            register = new Registers();
            dt = new DataTable();

            register[2] = 0;

            dt.Columns.Add("index");
            dt.Columns.Add("value");
            
            /*foreach(DataGridViewColumn dtc in dataGridView2.Columns)
            {
                dtc.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            WHY YOU NOT WORKING
            */
            RegisterMachine Rm = new RegisterMachine(new List<string>() { "load 1" });


        }

        #region EventHandler

        // Render Form with Custom Colors
        private void Form_Load(object sender, EventArgs e)
        {
            ToolStripManager.Renderer = new ToolStripProfessionalRenderer(new CustomColorTable());
        }

        // Event Handler for the "Speichern als.." item from the MenuStrip
        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get path from SaveFileDialog, save TextEditorBox content at path
            string s = Dialog.SaveFileDialog();
            openPath = s;

            if (s != null)
                FileHandler.SaveFileContent(s, TextEditorBox.Text);
                
            saved = true;

        }

        // Event Handler for the "Speichern" item from the MenuStrip
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Execute SaveAs method when openPath not initialized
            if (openPath != null)
                FileHandler.SaveFileContent(openPath, TextEditorBox.Text);
            else
                SaveAsToolStripMenuItem_Click(null, null);
                
            saved = true;
        }

        // Event Handler for the "Öffnen" item from the MenuStrip
        private void ÖffnenToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!saved)
            {
            DialogResult dialogResult = MessageBox.Show(
                "Änderungen am Code speichern?", "Ungespeicherte Änderungen",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                
                if (dialogResult == DialogResult.Yes)
                    SaveToolStripMenuItem_Click(null, null);
            }
            
            // Get file path from LoadFileDialog, read file from path and set TextEditorBox.Text to Filetext
            string s = Dialog.LoadFileDialog();
            openPath = s;

            if (s != null)
                TextEditorBox.Text = String.Join(System.Environment.NewLine, FileHandler.GetFileContent(s));
            
            saved = true;
        }

        // Event Handler for the "Neu" item from the MenuStrip, resets all variables and TextEditorBox.Text
        private void NewToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            openPath = null;
            if (!saved)
        	{
        		DialogResult dialogResult = MessageBox.Show(
        			"Änderungen am Code speichern?", "Ungespeicherte Änderungen",
        			MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            	
				if(dialogResult == DialogResult.Yes)
					SaveToolStripMenuItem_Click(null, null);
        	}
            TextEditorBox.Text = "";
            
            saved = true;
        }

        // Event Handler for the "Schließen" item from the MenuStrip
        private void CloseButton_Click(object sender, EventArgs e)
        {
            if (!saved)
        	{
        		DialogResult dialogResult = MessageBox.Show(
        			"Änderungen am Code speichern?", "Ungespeicherte Änderungen",
        			MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
        		
				if(dialogResult == DialogResult.Yes)
					SaveToolStripMenuItem_Click(null, null);
        	}
        	
        	Application.Exit();
        }

        private void Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseLocation = new Point(-e.X, -e.Y);
        }

        private void Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mousePosition = Control.MousePosition;
                mousePosition.Offset(mouseLocation.X, mouseLocation.Y);
                Location = mousePosition;
            }
        }
        
        void TextEditorBoxTextChanged(object sender, EventArgs e)
        {
        	saved = false;
        }

        #endregion

        /// <summary>
        /// Updates DataGridView2 to show values of registers
        /// </summary>
        private void UpdateDataGridView()
        {
            dt.Clear();
            
            for (int i = 0; i < register.Length; i++)
            {
                dt.Rows.Add(i.ToString(), register[i]);
            }
        }

    }
}
