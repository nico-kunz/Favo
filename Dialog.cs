using System;
using System.Windows.Forms;

namespace Favo
{
    static class Dialog
    {
        /// <summary>
        /// Generates a dialog for loading a file
        /// </summary>
        /// <returns>Path the user chose to load from</returns>
        public static string LoadFileDialog()
        {
            // Declare and initialize OpenFileDialog object
            OpenFileDialog Ofd = new OpenFileDialog()
            {
                Filter = "Text File|*.txt"
            };

            Console.WriteLine("Dialog.LoadFileDialog");

            // return path user chose, if user clicked on "Ok", otherwise return null
            return (Ofd.ShowDialog() == DialogResult.OK) ? Ofd.FileName : null; 
        }

        /// <summary>
        /// Generates a dialog for saving a file
        /// </summary>
        /// <returns>Path the user chose to save the file at</returns>
        public static string SaveFileDialog()
        {
            // Declare and initialize SaveFileDialog
            SaveFileDialog Sfd = new SaveFileDialog()
            {
                DefaultExt = ".txt",
                Filter = "All Files|*.*"
            };

            Console.WriteLine("Dialog.SaveFileDialog");

            // return path user chose, if user clicked on "Ok", otherwise return null
            return (Sfd.ShowDialog() == DialogResult.OK) ? Sfd.FileName : null;
        }
    }
}
