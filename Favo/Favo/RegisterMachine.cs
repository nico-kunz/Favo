using System;
using System.Collections.Generic;

namespace Favo
{
    class RegisterMachine
    {
        public int Accumulator { get; private set; }
        public Registers Heap { get; private set; }
        private List<string> Code;
        private List<Operation> Operations;
        private Dictionary<string, int> Labels;
        public int InstructionPointer {get; private set;}
        public int InstructionCounter {get; private set;}
        public bool JakobiIf;

        #region DataStructures
        /// <summary>
        /// Enumeration for different Operations
        /// </summary>
        enum OperationCode
        {
            LOAD,
            CLOAD,
            ILOAD,
            STORE,
            ISTORE,
            ADD,
            CADD,
            IADD,
            SUB,
            CSUB,
            ISUB,
            MUL,
            CMUL,
            IMUL,
            DIV,
            CDIV,
            IDIV,

            IGOTO,
            GOTO,
            END,

            IF,
            IFBIG,
            IFSM,

            CIF,
            CIFBIG,
            CIFSM,

            IIF,
            IIFBIG,
            IIFSM,

            GIF,

            IN,
            IIN,

            OUT,
            IOUT,

            NULL

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
        public RegisterMachine(List<string> codeToExecute, bool JakobiIf)
        {
            // initialize code the register machine has to execute
            Code = codeToExecute;

            // standard initializations
            InstructionPointer = 1;
            InstructionCounter = 0;
            Accumulator = 0;
            this.JakobiIf = JakobiIf;
            Heap = new Registers();
            Operations = new List<Operation>();
            Labels = new Dictionary<string, int>();

            // Translate code into usable format
            TranslateCode();
        }



        public void ExecuteRegisterMachine(bool stepByStep)
        {
            for(; InstructionPointer <= Operations.Count; InstructionPointer++)
            {
                if (!ExecuteStep(Operations[InstructionPointer - 1].operationCode, Operations[InstructionPointer - 1].argument))
                    return;
            }


        }
        public bool ExecuteOneStep()
        {
            // stop execution if ExecuteStep returns false;
            if (!ExecuteStep(Operations[InstructionPointer - 1].operationCode, Operations[InstructionPointer - 1].argument))
                return false;


            InstructionPointer++;
            if (InstructionPointer > Operations.Count)
                return false;
            else
                return true;
        }

        private bool ExecuteStep(OperationCode opcode, int argument)
        {
            InstructionCounter++;

            switch (opcode)
            {
                case OperationCode.LOAD:
                    Accumulator = Heap[argument];
                    break;

                case OperationCode.CLOAD:
                    Accumulator = argument;
                    break;

                case OperationCode.ILOAD:
                    Accumulator = Heap[Heap[argument]];
                    break;

                case OperationCode.STORE:
                    Heap[argument] = Accumulator;
                    break;

                case OperationCode.ISTORE:
                    Heap[Heap[argument]] = Accumulator;
                    break;

                case OperationCode.ADD:
                    Accumulator += Heap[argument];
                    break;

                case OperationCode.CADD:
                    Accumulator += argument;
                    break;

                case OperationCode.IADD:
                    Accumulator += Heap[Heap[argument]];
                    break;

                case OperationCode.SUB:
                    Accumulator -= Heap[argument];
                    break;

                case OperationCode.CSUB:
                    Accumulator -= argument;
                    break;

                case OperationCode.ISUB:
                    Accumulator -= Heap[Heap[argument]];
                    break;

                case OperationCode.MUL:
                    Accumulator *= Heap[argument];
                    break;

                case OperationCode.CMUL:
                    Accumulator *= argument;
                    break;

                case OperationCode.IMUL:
                    Accumulator *= Heap[Heap[argument]];
                    break;

                case OperationCode.DIV:
                    Accumulator /= Heap[argument];
                    break;

                case OperationCode.CDIV:
                    Accumulator /= argument;
                    break;

                case OperationCode.IDIV:
                    Accumulator /= Heap[Heap[argument]];
                    break;

                case OperationCode.GOTO:
                    InstructionPointer = argument - 1;
                    break;

                case OperationCode.IGOTO:
                    InstructionPointer = Heap[argument] - 1;
                    break;

                case OperationCode.END:
                    return false;

                case OperationCode.IF:
                    if (Accumulator != Heap[argument])
                        InstructionPointer++;
                    break;

                case OperationCode.IFBIG:
                    if (!(Accumulator > Heap[argument]))
                        InstructionPointer++;
                    break;

                case OperationCode.IFSM:
                    if (!(Accumulator < Heap[argument]))
                        InstructionPointer++;
                    break;

                case OperationCode.CIF:
                    if (Accumulator != argument)
                        InstructionPointer++;
                    break;

                case OperationCode.CIFBIG:
                    if (!(Accumulator > argument))
                        InstructionPointer++;
                    break;

                case OperationCode.CIFSM:
                    if (!(Accumulator < argument))
                        InstructionPointer++;
                    break;

                case OperationCode.IIF:
                    if (Accumulator != Heap[Heap[argument]])
                        InstructionPointer++;
                    break;

                case OperationCode.IIFBIG:
                    if (!(Accumulator > Heap[Heap[argument]]))
                        InstructionPointer++;
                    break;

                case OperationCode.IIFSM:
                    if (!(Accumulator < Heap[Heap[argument]]))
                        InstructionPointer++;
                    break;

                case OperationCode.GIF:
                    if (Accumulator != 0)
                        InstructionPointer++;
                    break;

                case OperationCode.IN:
                    Console.WriteLine("Please enter a didgit!");
                    try
                    {
                        Heap[argument] = int.Parse(Console.ReadLine());
                    }
                    catch
                    {
                        Console.WriteLine("Error: You cannot enter letters!");
                        throw new Exception("Error: You cannot enter letters!");
                    }
                    break;

                case OperationCode.IIN:
                    Console.WriteLine("Please enter a digit!");
                    try
                    {
                        Heap[Heap[argument]] = int.Parse(Console.ReadLine());
                    }
                    catch
                    {
                        Console.WriteLine("Error: You cannot enter letters!");
                        throw new Exception("Error: You cannot enter letters!");
                    }
                    break;

                case OperationCode.OUT:
                    Console.WriteLine("Output: " + Heap[argument].ToString());
                    break;

                case OperationCode.IOUT:
                    Console.WriteLine("Output: " + Heap[Heap[argument]].ToString());
                    break;
            }


            return true;
        }

        /// <summary>
        /// Translates Code object into operation-struct object and adds it to List<Operation> Operations
        /// </summary>
        private void TranslateCode()
        {
            // temporarily saving the demanded operation
            OperationCode opcode;
            int argument = 0;

            int counter = 1;

            // stop translation if code is empty
            if (Code.Count == 1 && Code[0] == "")
                return;

            ScanForLabels();

            foreach (string item in Code)
            {
                // ignore empty lines of code
                if(string.IsNullOrWhiteSpace(item))
                {
                    Operations.Add(new Operation(InstructionPointer, OperationCode.NULL, 0));
                    counter++;

                    continue;
                }
            		

                // ignore comments
                if (item.Length > 1 && item.Substring(0, 2) == "//")
                {
                    Operations.Add(new Operation(InstructionPointer, OperationCode.NULL, 0));
                    counter++;
                    continue;
                }


                // split item at whitespace
                string[] parts = item.Split(' ');


                // initialize opcode with correct operation code
                switch (parts[0].ToLower())
                {
                    case "load":
                        opcode = OperationCode.LOAD;
                        break;

                    case "cload":
                        opcode = OperationCode.CLOAD;
                        break;

                    case "iload":
                        opcode = OperationCode.ILOAD;
                        break;

                    case "store":
                        opcode = OperationCode.STORE;
                        break;

                    case "istore":
                        opcode = OperationCode.ISTORE;
                        break;

                    case "add":
                        opcode = OperationCode.ADD;
                        break;

                    case "cadd":
                        opcode = OperationCode.CADD;
                        break;

                    case "iadd":
                        opcode = OperationCode.IADD;
                        break;

                    case "sub":
                        opcode = OperationCode.SUB;
                        break;

                    case "csub":
                        opcode = OperationCode.CSUB;
                        break;

                    case "isub":
                        opcode = OperationCode.ISUB;
                        break;

                    case "mul":
                        opcode = OperationCode.MUL;
                        break;

                    case "cmul":
                        opcode = OperationCode.CMUL;
                        break;

                    case "imul":
                        opcode = OperationCode.IMUL;
                        break;

                    case "div":
                        opcode = OperationCode.DIV;
                        break;

                    case "cdiv":
                        opcode = OperationCode.CDIV;
                        break;

                    case "idiv":
                        opcode = OperationCode.IDIV;
                        break;

                    case "goto":
                        opcode = OperationCode.GOTO;
                        break;

                    case "igoto":
                        opcode = OperationCode.IGOTO;
                        break;

                    case "end":
                        opcode = OperationCode.END;
                        Operations.Add(new Operation(counter, opcode, 0));
                        counter++;
                        continue;

                    case "if":
                        if (JakobiIf == true)
                            throw new Exception("Error: You are not allowed to use this kind of if-command in the basic if mode! Wrong if-command used at line " + counter);

                        opcode = OperationCode.IF;
                        break;

                    case "if<":
                        if (JakobiIf == true)
                            throw new Exception("Error: You are not allowed to use this kind of if-command in the basic if mode! Wrong if-command used at line " + counter);

                        opcode = OperationCode.IFSM;
                        break;

                    case "if>":
                        if (JakobiIf == true)
                            throw new Exception("Error: You are not allowed to use this kind of if-command in the basic if mode! Wrong if-command used at line " + counter);

                        opcode = OperationCode.IFBIG;
                        break;

                    case "cif":
                        if (JakobiIf == true)
                            throw new Exception("Error: You are not allowed to use this kind of if-command in the basic if mode! Wrong if-command used at line " + counter);

                        opcode = OperationCode.CIF;
                        break;

                    case "cif<":
                        if (JakobiIf == true)
                            throw new Exception("Error: You are not allowed to use this kind of if-command in the basic if mode! Wrong if-command used at line " + counter);

                        opcode = OperationCode.CIFSM;
                        break;

                    case "cif>":
                        if (JakobiIf == true)
                            throw new Exception("Error: You are not allowed to use this kind of if-command in the basic if mode! Wrong if-command used at line " + counter);

                        opcode = OperationCode.CIFBIG;
                        break;

                    case "iif":
                        if (JakobiIf == true)
                            throw new Exception("Error: You are not allowed to use this kind of if-command in the basic if mode! Wrong if-command used at line " + counter);

                        opcode = OperationCode.IIF;
                        break;

                    case "iif<":
                        if (JakobiIf == true)
                            throw new Exception("Error: You are not allowed to use this kind of if-command in the basic if mode! Wrong if-command used at line " + counter);

                        opcode = OperationCode.IIFSM;
                        break;

                    case "iif>":
                        if (JakobiIf == true)
                            throw new Exception("Error: You are not allowed to use this kind of if-command in the basic if mode! Wrong if-command used at line " + counter);

                        opcode = OperationCode.IIFBIG;
                        break;

                    case "gif":
                        opcode = OperationCode.GIF;
                        Operations.Add(new Operation(counter, opcode, 0));
                        counter++;
                        continue;

                    case "in":
                        opcode = OperationCode.IN;
                        break;

                    case "iin":
                        opcode = OperationCode.IIN;
                        break;

                    case "out":
                        opcode = OperationCode.OUT;
                        break;

                    case "iout":
                        opcode = OperationCode.IOUT;
                        break;

                    default:
                        // if last character :
                        if(parts[0].Substring(parts[0].Length - 1) == ":")
                        {
                            // Add null operation to Operations to prevent gaps
                            Operations.Add(new Operation(counter, OperationCode.NULL, 0));
                            
                            // Increment Pointer and Counter
                            counter++;
                            continue;
                        }
                        else
                            throw new Exception("Unknown command at line " + (counter).ToString());
                }

                // throw exception if more than one whitespace
                if (parts.Length != 2 && opcode != OperationCode.END)
                {
                    throw new Exception("Wrong amount of whitespaces at line " + (counter).ToString());
                }



                // save argument 
                if (opcode == OperationCode.GOTO && int.TryParse(parts[1], out argument) == false)
                    argument = Labels[parts[1]];
                else
                {
                    argument = int.Parse(parts[1]);
                    if (argument < 0)
                        throw new Exception("Negative numbers are not supported! Line: " + counter.ToString());
                }
                    

                // add everything to List<Operation> Operations
                Operations.Add(new Operation(counter, opcode, argument));

                // Increment instruction counter for each new instruction
                counter++;
            }
        }

        /// <summary>
        /// Scan code for label definitions and add those to the dictionary Labels
        /// </summary>
        private void ScanForLabels()
        {
            int counter = 1;
            foreach(string item in Code)
            {
                if(item.EndsWith(":"))
                {
                    Labels.Add(item.Substring(0, item.Length - 1), counter);
                    Console.WriteLine(item);
                }
                counter++;
            }
        }

        /// <summary>
        /// Reset all important variables of the register machine, for making it work again (without recompiling the code)
        /// </summary>
        public void ResetState()
        {
            Accumulator = 0;
            Heap = new Registers();
            InstructionCounter = 0;
            InstructionPointer = 1;
        }
    }
}