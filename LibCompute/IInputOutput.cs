using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LibCompute
{
    public interface IInputOutput
    {
        long ReadInputInt();

        long ReadOutputInt();

        void InputInt(long output);

        void OutputInt(long output);

        event EventHandler IntOuputted;

        event EventHandler ReadingInt;
    }
}