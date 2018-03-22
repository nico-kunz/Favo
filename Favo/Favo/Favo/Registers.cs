using System.Collections.Generic;

namespace Favo
{
    class Registers
    {
        // actual register with dynamic size
        private List<int> register;
        public int Length { get; private set; }

        // Indexer
        public int this[int index]
        {
            // return Item at index from list
            get { return register[index]; }

            set
            {
                // resize list if index out of range and add value
                if (register.Count < index)
                {
                    // add empty items between last element and wanted index
                    for (int i = register.Count; i < index; i++)
                    {
                        register.Add(0);
                        Length++;
                    }
                        

                    // add value to list 
                    register.Add(value);
                    Length++; 
                }

                // set value at wanted index if index not out of range
                else
                    register[index] = value;
            }
        }

        public Registers()
        {
            register = new List<int>();
        }

       


    }
}
