using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favo
{
    class RegisterMachine
    {
        private int Accumulator;
        private Registers Registers;
        private List<string> Code;
        private List<Operation> Operations;
        private int InstructionCount;

#region DataStructures
        /// <summary>
        /// Enumeration for different Operations
        /// </summary>
        enum OperationCode
        {
            LOAD,
            STORE,
            ADD,
            SUB
        }

        /// <summary>
        /// Struct for saving lineNumber, operationCode 
        /// </summary>
        private struct Operation
        {

            public int lineNumber;
            public OperationCode operationCode;
            public int argument;

            /// <summary>
            /// Constructor of Operation Struct
            /// </summary>
            /// <param name="line">Numbered line of code</param>
            /// <param name="opcode">Code of wanted operation</param>
            /// <param name="arg">argument for given operation</param>
            public Operation(int line, OperationCode opcode, int arg)
            {
                lineNumber = line;
                operationCode = opcode;
                argument = arg;
            }
        }
        #endregion

        /// <summary>
        /// Constructor of class RegisterMaschine
        /// </summary>
        /// <param name="codeToExecute">Code that is to be executed by the register machine</param>
        public RegisterMachine(List<string> codeToExecute)
        {
            // initialize code the register machine has to execute
            Code = codeToExecute;

            InstructionCount = 1;
            Accumulator = 0;
            Registers = new Registers();
            Operations = new List<Operation>();

            TranslateCode();

        }



        public void ExecuteRegisterMachine(List<string> code, bool stepByStep)
        {


        }

        private void ExecuteStep(OperationCode opcode, int argument)
        {
            
        }

        /// <summary>
        /// Translates Code object into operation-struct object and adds it to List<Operation> Operations
        /// </summary>
        private void TranslateCode()
        {
            // temporarily saving the demanded operation
            OperationCode opcode;

            foreach (string item in Code)
            {
                // ignore comments
                if (item.Substring(0, 2) == "//")
                {
                    InstructionCount++;
                    continue;
                }

                

                // split item at whitespace
                string[] parts = item.Split(' ');

                // throw exception if more than one whitespace
                if (parts.Length != 2)
                {
                    throw new Exception("Invalid wasauchimmer at line " + (InstructionCount + 1).ToString());
                }


                // initialize opcode with correct operation code
                switch (parts[0].ToLower())
                {
                    case "load":
                        opcode = OperationCode.LOAD;
                        break;
                    case "store":
                        opcode = OperationCode.STORE;
                        break;
                    case "add":
                        opcode = OperationCode.ADD;
                        break;
                    case "sub":
                        opcode = OperationCode.SUB;
                        break;
                    default:
                        throw new Exception("Unknown command at line " + (InstructionCount + 1).ToString());
                }

                // save argument 
                int argument = int.Parse(parts[1]);

                // add everything to List<Operation> Operations
                Operations.Add(new Operation(InstructionCount, opcode, argument));
                
                // Increment instruction counter for each new instruction
                InstructionCount++;
            }
        }
    }
}
