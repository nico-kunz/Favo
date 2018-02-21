using System.Collections.Generic;

namespace Favo
{
    class Register
    {
        // actual register with dynamic size
        private List<int> list;

        // Indexer
        public int this[int index]
        {
            // return Item at index from list
            get { return list[index]; }

            set
            {
                // resize list if index out of range and add value
                if (list.Count < index)
                {
                    // add empty items between last element and wanted index
                    for (int i = list.Count; i < index; i++)
                        list.Add(0);

                    // add value to list 
                    list.Add(value);
                }

                // set value at wanted index if index not out of range
                else
                    list[index] = value;
            }
        }

        public Register()
        {
            list = new List<int>();
        }


    }
}
