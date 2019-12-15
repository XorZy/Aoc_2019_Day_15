using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LibCompute
{
    public class ConsoleIO : IInputOutput
    {
        public event EventHandler IntOuputted;

        public event EventHandler ReadingInt;

        public void InputInt(long output)
        {
            throw new NotImplementedException();
        }

        public void OutputInt(long output)
        {
            Console.WriteLine(output);
            IntOuputted?.Invoke(this, EventArgs.Empty);
        }

        public long ReadInputInt()
        {
            ReadingInt?.Invoke(this, EventArgs.Empty);
            return long.Parse(Console.ReadLine());
        }

        public long ReadOutputInt()
        {
            throw new NotImplementedException();
        }
    }
}