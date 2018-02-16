using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;

namespace Favo
{
    class FileHandler
    {
        
        /// <summary>
        /// Reads lines of file and adds them to List
        /// </summary>
        /// <param name="path">Path of the file to be read</param>
        /// <returns>List-object indexing textfile line by line</returns>
        public static List<string> GetFileContent(string path)
        {
            // Check if wanted file actually exists
            if (!File.Exists(path))
                throw new Exception("File at path " + path + " does not exist!");


            List<string> textFileLines = new List<string>();

            // Streamreader object for reading a file
            using (StreamReader Sr = new StreamReader(path))
            {
                // line = 0 ==> Last read line did not contain any text;
                string line = Sr.ReadLine();
                while (line != null)
                    textFileLines.Add(line);
            }

            
            return textFileLines;
        }

        /// <summary>
        /// Writes given string to file and saves file at path
        /// </summary>
        /// <param name="path">Path of the file that has to be saved</param>
        /// <param name="content">String being written into textfile</param>
        public static void SaveFileContent (string path, string content)
        {
            // Check if file extension is .txt
            if (Path.GetExtension(path) != ".txt")
                throw new Exception("Wrong file extension! Only \".txt\" is supported");

            // Write string content to file at path
            using (StreamWriter Sw = new StreamWriter(path))
                Sw.Write(content);
        }
    }
}
