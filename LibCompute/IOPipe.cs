using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibCompute
{
    public class IOPipe : IInputOutput
    {
        private Queue<long> _inputBuffer = new Queue<long>();

        private Queue<long> _outputBuffer = new Queue<long>();

        public event EventHandler IntOuputted;

        public event EventHandler ReadingInt;

        public int FireEveryNbOutput = 1;

        public long ReadOutputInt()
        {
            var wait = new SpinWait();
            while (_outputBuffer.Count < 1)
            {
                wait.SpinOnce();
            }
            return _outputBuffer.Dequeue();
        }

        public void InputInt(long input)
        {
            _inputBuffer.Enqueue(input);
        }

        public void OutputInt(long output)
        {
            _outputBuffer.Enqueue(output);
            if (_outputBuffer.Count >= FireEveryNbOutput)
                IntOuputted?.Invoke(this, EventArgs.Empty);
        }

        public long ReadInputInt()
        {
            var wait = new SpinWait();

            ReadingInt?.Invoke(this, EventArgs.Empty);

            while (_inputBuffer.Count < 1)
            {
                wait.SpinOnce();
            }

            return _inputBuffer.Dequeue();
        }
    }
}