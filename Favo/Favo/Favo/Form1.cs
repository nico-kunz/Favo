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
        RegisterMachine rM;
        private static string openPath;
        private DataTable dt;
        bool saved, iMode;

        // custom colorTable class for MenuStrip (custom appearance)
        public class CustomColorTable : ProfessionalColorTable
        {
            public override Color MenuItemSelected { get { return Color.FromArgb(44, 47, 51); } }
            public override Color MenuItemBorder { get { return Color.FromArgb(114, 137, 218); } }
        }

        // Constructor, initialize important components and variables 
        public Form1()
        {
            InitializeComponent();
            menuStrip1.Renderer = new ToolStripProfessionalRenderer(new CustomColorTable());

            rM = new RegisterMachine(new List<string>() { "" });
            register = new Registers();
            dt = new DataTable();
            saved = true;
            iMode = false;

            // Columns
            dt.Columns.Add("Index");
            dt.Columns.Add("Value");

            // set columns
            dataGridView2.DataSource = dt;


            // disable sorting of columns
            foreach (DataGridViewColumn column in dataGridView2.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
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
            CheckSavedStatus();

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
            CheckSavedStatus();
            TextEditorBox.Text = "";

            saved = true;
        }


        //Event Handler for the "Run" item in the MenuStrip, compiles and runs the program
        private void RunToolStripMenuItemClick(object sender, System.EventArgs e)
        {
            ErrorBox.Text = "";

            try
            {
                rM = new RegisterMachine(TextEditorBox.Text.Split('\n').ToList());
                rM.ExecuteRegisterMachine(false);
            }
            catch (Exception exception)
            {
                ErrorBox.Text = exception.Message;
            }
            
            UpdateLabels();
            UpdateDataGridView();

            rM.ResetState();
        }


        //Event Handler for the "imode"item in the MenuStrip, switches between if-modes
        void ImodeToolStripMenuItemClick(object sender, EventArgs e)
        {
            rM.ExecuteOneStep();
            UpdateLabels();
            UpdateDataGridView();
        }

        // Event Handler for the "Schließen" item from the MenuStrip
        private void CloseButton_Click(object sender, EventArgs e)
        {
            CheckSavedStatus();
            Application.Exit();
        }


        #region MovableWindow

        //Panels can be used to move the Window
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

        #endregion 

        // Event Handler for the "TextBox" item, if it's changed
        void TextEditorBoxTextChanged(object sender, EventArgs e)
        {
            if (TextEditorBox.Lines.Length >= codelines.Lines.Length)
            {
                codelines.Text = "";
                for (int i = 1; i <= TextEditorBox.Lines.Length; i++)
                {       
                    codelines.Text += i + Environment.NewLine;
                }
            }
            else if (TextEditorBox.Lines.Length < codelines.Lines.Length)
            {
                codelines.Text = "";
                for (int i = 1; i <= TextEditorBox.Lines.Length; i++)
                {
                    codelines.Text += i + Environment.NewLine;
                }
            }

           

            saved = false;
        }

        // Method to check, if latest changes are saved. Shows MessageBox.
        /// <summary>
        /// Checks if latest changes are saved.
        /// </summary>
        void CheckSavedStatus()
        {
            if (!saved)
            {
                DialogResult dialogResult = MessageBox.Show("Änderungen am Code speichern?", "Ungespeicherte Änderungen",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                if (dialogResult == DialogResult.Yes)
                    SaveToolStripMenuItem_Click(null, null);
            }
        }

        #endregion


        /*private void TextEditorBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                codelines.Text += TextEditorBox.Lines.Length.ToString() + Environment.NewLine;
            }   
        }*/


        // Update Variable Labels
        private void UpdateLabels()
        {
            labelaccumulator.Text = rM.Accumulator.ToString();
            labeloperations.Text = rM.InstructionCounter.ToString();
        }

        /// <summary>
        /// Updates DataGridView2 to show values of registers
        /// </summary>
        private void UpdateDataGridView()
        {
            dt.Clear();
            
            for (int i = 0; i < rM.Heap.Length; i++)
            {
                dt.Rows.Add(i.ToString(), rM.Heap[i]);
            }
        }
    }
}
