using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Favo
{
    public partial class MainWindow : Form
    {
        #region Variables
        public Point mouseLocation;
        public int startup;
        Registers register;
        RegisterMachine rM;
        private static string openPath;
        private DataTable dt;
        bool saved, compiled, ifMode; //ifMode indicates whether simple if is used instead of complex if (replaces IF,IIF,CIF)

        const int EM_LINESCROLL = 0x00B6;

        #endregion

        // custom colorTable class for MenuStrip (custom appearance)
        public class CustomColorTable : ProfessionalColorTable
        {
            public override Color MenuItemSelected { get { return Color.FromArgb(44, 47, 51); } }
            public override Color MenuItemBorder { get { return Color.FromArgb(114, 137, 218); } }
        }


        // Constructor, initialize important components and variables 
        public MainWindow()
        {
            InitializeComponent();
            menuStrip1.Renderer = new ToolStripProfessionalRenderer(new CustomColorTable());

            rM = new RegisterMachine(new List<string>() { "" });
            register = new Registers();
            dt = new DataTable();
            saved = true;
            startup = 0;
            compiled = false;
            ifMode = false;

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
            {
                FileHandler.SaveFileContent(s, textEditorBox.Text);
                saved = true;
            }
        }

        // Event Handler for the "Speichern" item from the MenuStrip
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Execute SaveAs method when openPath not initialized
            if (openPath != null)
            {
                FileHandler.SaveFileContent(openPath, textEditorBox.Text);
                saved = true;
            }
            else
                SaveAsToolStripMenuItem_Click(null, null);
        }

        // Event Handler for the "Öffnen" item from the MenuStrip
        private void ÖffnenToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Main functionality in CheckSavedStatus method
            CheckSavedStatus("open");
        }

        // Event Handler for the "Neu" item from the MenuStrip, resets all variables and TextEditorBox.Text
        private void NewToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Main functionality in CheckSavedStatus method
            CheckSavedStatus("new");
        }

        //Event Handler for the "Run" item in the MenuStrip, compiles and runs the program
        private void RunToolStripMenuItemClick(object sender, System.EventArgs e)
        {
            errorBox.Text = "";

            try
            {
                rM = new RegisterMachine(textEditorBox.Text.Split('\n').ToList());
                compiled = true;
                rM.ExecuteRegisterMachine(false);
            }
            catch (Exception exception)
            {
                errorBox.Text = exception.Message;
            }
            
            UpdateLabels();
            UpdateDataGridView();

            rM.ResetState();
        }

        // Event Handler for StepByStep ToolStripMenuItem, executes one line of code per click
        private void StepByStepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Don't compile twice if code is already compiled
                if(!compiled)
                {
                    rM = new RegisterMachine(textEditorBox.Text.Split('\n').ToList());
                    compiled = true;
                }

                // Check if there are more steps available or end of code is reached
                if (rM.ExecuteOneStep() == false)
                {
                    Highlight();
                    UpdateLabels();
                    UpdateDataGridView();
                    errorBox.Text = "Program execution finished!";

                    rM.ResetState();
                }
                else
                {
                    Highlight();
                    UpdateLabels();
                    UpdateDataGridView();
                }
            }
            catch (Exception exc)
            {
                errorBox.Text = exc.Message;
            }
        }

        //Event handler for the "imode"item in the MenuStrip, switches between if-modes
        void IfModeToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Invert ifMode bool
            ifMode = !ifMode;

            // Change icon of ifModeToolStripMenuItem according to ifMode bool value
            if (ifMode == true)
            {
                ifModeToolStripMenuItem.Image = global::Favo.Properties.Resources.SyncArrowRed_16x;
            }
            else if (ifMode == false)
            {
                ifModeToolStripMenuItem.Image = global::Favo.Properties.Resources.SyncArrow_16x;
            }
        }

        // Event Handler for the close button in the top right corner
        private void CloseButton_Click(object sender, EventArgs e)
        {
            //Main functionality in CheckSavedStatus method
            CheckSavedStatus("exit");
        }

        // Event Handler for help ToolStripMenuItem, opens new Window
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form HelpMenu = new HelpMenu();
            HelpMenu.Show();
        }

        // Form1 has no FormBorderStyle, this makes up for the missing dragability. Resize could be added.
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
            UpdateCodelines();

            // scroll codelinesTextBox to the position of textEditorBox
            ScrollTo(GetScrollPos(textEditorBox.Handle, 1));

            saved = false;
            compiled = false;
        }

        /// <summary>
        /// Checks for changes, calls for execution afterwards
        /// <paramref name="task"/>Defines which task to execute afterwards.</param>
        /// </summary>
        void CheckSavedStatus(string task)
        {
            //Check if latest changes are saved, show MessageBox.
            if (!saved)
            {
                DialogResult dialogResult = MessageBox.Show("Änderungen am Code speichern?", "Ungespeicherte Änderungen",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                //Save changes
                if (dialogResult == DialogResult.Yes)
                {
                    SaveToolStripMenuItem_Click(null, null);
                    ToolbarExecution(task);
                }
                //Abort changes
                else if (dialogResult == DialogResult.No)
                    ToolbarExecution(task);

            }
            else
                ToolbarExecution(task);
        }        

        /// <summary>
        /// Executes the tasks
        /// </summary>
        /// <param name="task">Defines which task to execute.</param>
        void ToolbarExecution(string task)
        {
            //What's the task?
            //New File
            if (task == "new")
            {
                openPath = null;
                textEditorBox.Text = "";
                saved = true;
            }
            //Open File
            else if (task == "open")
            {
                // Get file path from LoadFileDialog, read file from path and set TextEditorBox.Text to Filetext
                string s = Dialog.LoadFileDialog();
                openPath = s;

                if (s != null)
                {
                    textEditorBox.Text = String.Join(System.Environment.NewLine, FileHandler.GetFileContent(s));
                    saved = true;
                }
            }
            //Close Application
            else if (task == "exit")
                Application.Exit();
        }
        
        // Update Scrollbar pos
        private void TextEditorBox_VScroll(object sender, EventArgs e)
        {           
            ScrollTo(GetScrollPos(textEditorBox.Handle, 1));
        }

        // Changes focus to textEditorBox when Codelines somehow enters focus
        private void Codelines_Enter(object sender, EventArgs e)
        {
            textEditorBox.Focus();
        }

        #endregion

        #region ScrollSync

        // Imported windows functions for controlling scrollbars of control elements
        [DllImport("user32.dll")]
        static extern int SetScrollPos(IntPtr hWnd, int nbar, int nPos, bool bRedraw);

        [DllImport("user32.dll", EntryPoint = "PostMessageA")]
        static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        static extern int GetScrollPos(IntPtr hWnd, int nBar);  

        public void ScrollTo(int pos)
        {
            SetScrollPos(codelines.Handle, 0x1, pos, true);
            PostMessage(codelines.Handle, 0x115, 4 + 0x10000 * pos, 0);
        }

        #endregion

        // Update indicator for current line for step by step
        private void Highlight()
        {
            UpdateCodelines();
            codelines.Text = codelines.Text.Insert(codelines.Text.IndexOf((rM.InstructionPointer - 1).ToString()) + 1,  " <--"); //Text selection highlighting too complex and prone to bugs
        }

        #region Updates

        // Update Variable Labels
        private void UpdateLabels()
        {
            labelaccumulator.Text = rM.Accumulator.ToString();
            labeloperations.Text = rM.InstructionCounter.ToString();
        }

        


        // compare textEditorBox number of lines with codelines number of lines
        // add or remove linenumbers from codelinesTextBox if necessary
        private void UpdateCodelines()
        {
            if (textEditorBox.Lines.Length >= codelines.Lines.Length)
            {
                codelines.Text = "";
                for (int i = 1; i <= textEditorBox.Lines.Length; i++)
                {
                    codelines.Text += i + Environment.NewLine;
                }
            }

            else if (textEditorBox.Lines.Length < codelines.Lines.Length)
            {
                codelines.Text = "";
                for (int i = 1; i <= textEditorBox.Lines.Length; i++)
                {
                    codelines.Text += i + Environment.NewLine;
                }
            }
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
        #endregion

    }
}
