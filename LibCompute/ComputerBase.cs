using System;
using System.Collections.Generic;
using System.Text;

namespace LibCompute
{
    public enum MEMORY_ACCESS_MODE
    {
        POSITION = 0,
        IMMEDIATE = 1,
        RELATIVE = 2
    }

    public abstract class ComputerBase
    {
        private List<long> _memory;
        private long PC { get; set; }
        public string Name { get; }

        public bool ProgramTerminated { get; protected set; }

        public bool IsRunning { get; protected set; }

        public bool Aborted { get; private set; }

        #region MEMORY_ACCESS

        protected long MemoryReadAt(long index)
        {
            Allocate(index);
            return _memory[(int)index];
        }

        public void Abort()
        {
            Aborted = true;
        }

        public void MemoryWriteAt(long index, long value)
        {
            Allocate(index);
            _memory[(int)index] = value;
        }

        private void Allocate(long index)
        {
            if (index >= _memory.Count)
            {
                var length = index - _memory.Count + 1;
                Console.WriteLine($"Allocating {length} words");
                for (int i = 0; i < length; i++)
                    _memory.Add(0);
            }
        }

        protected long MemoryReadOne()
        {
            return MemoryReadAt((int)PC++);
        }

        public abstract bool Step();

        public void Run()
        {
            Console.WriteLine("Computer started");
            IsRunning = true;
            while (!Aborted && Step()) ;
            ProgramTerminated = true;
            IsRunning = false;
        }

        protected ComputerBase(string name, long[] memory, IInputOutput io)
        {
            Name = name;
            _memory = new List<long>(memory);
            IO = io;
        }

        #endregion MEMORY_ACCESS

        protected void JumpTo(long index)
        {
            PC = index;
        }

        public IInputOutput IO { get; }
    }
}