using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibCompute
{
    internal enum OPCODES
    {
        ADD = 1,
        MULTIPLY = 2,
        READ = 3,
        WRITE = 4,
        JUMP_ON_TRUE = 5,
        JUMP_ON_FALSE = 6,
        LESS_THAN = 7,
        EQUALS = 8,
        SET_RELATIVE_BASE = 9,
        END = 99
    }

    public class IntcodeComputer : ComputerBase
    {
        protected long PullNextParameter(MEMORY_ACCESS_MODE mode)
        {
            var value = MemoryReadOne();
            if (mode == MEMORY_ACCESS_MODE.POSITION)
                return MemoryReadAt((int)value);
            else if (mode == MEMORY_ACCESS_MODE.RELATIVE)
                return MemoryReadAt((int)(_relativeBase + value));
            else if (mode == MEMORY_ACCESS_MODE.IMMEDIATE)
                return value;
            else
                throw new ArgumentException("UNKNOWN MODE");
        }

        private long _relativeBase = 0;

        private bool Interpret(long rawOpcode)
        {
            var strOpCode = (rawOpcode + "").PadLeft(5, '0');

            var firstMode = (MEMORY_ACCESS_MODE)int.Parse(strOpCode[2] + "");
            var secondMode = (MEMORY_ACCESS_MODE)int.Parse(strOpCode[1] + "");
            var thirdMode = (MEMORY_ACCESS_MODE)int.Parse(strOpCode[0] + "");

            var opcode = (OPCODES)int.Parse(strOpCode.Substring(3));

            long a, b, c;

            //Console.WriteLine($"{Name}->{opcode}");

            //THREE OPERANDS

            if (opcode == OPCODES.ADD || opcode == OPCODES.MULTIPLY || opcode == OPCODES.LESS_THAN || opcode == OPCODES.EQUALS)
            {
                a = PullNextParameter(firstMode);
                b = PullNextParameter(secondMode);
                c = PullNextParameter(MEMORY_ACCESS_MODE.IMMEDIATE);

                if (thirdMode == MEMORY_ACCESS_MODE.RELATIVE)
                    c += _relativeBase;

                if (opcode == OPCODES.ADD || opcode == OPCODES.MULTIPLY)
                    MemoryWriteAt(c, opcode == OPCODES.ADD ? a + b : a * b);
                else
                   if (opcode == OPCODES.LESS_THAN)
                    MemoryWriteAt(c, a < b ? 1 : 0);
                else
                    MemoryWriteAt(c, a == b ? 1 : 0);
            }

            //TWO OPERANDS
            else if (opcode == OPCODES.JUMP_ON_TRUE || opcode == OPCODES.JUMP_ON_FALSE)
            {
                a = PullNextParameter(firstMode);
                b = PullNextParameter(secondMode);

                if (opcode == OPCODES.JUMP_ON_TRUE)
                {
                    if (a != 0)
                    {
                        JumpTo(b);
                    }
                }
                else if (a == 0)
                {
                    JumpTo(b);
                }
            }

            //ONE OPERAND
            else if (opcode == OPCODES.READ)
            {
                var input = IO.ReadInputInt();
                a = PullNextParameter(MEMORY_ACCESS_MODE.IMMEDIATE);
                if (firstMode == MEMORY_ACCESS_MODE.RELATIVE)
                    a += _relativeBase;
                MemoryWriteAt(a, input);
            }
            else if (opcode == OPCODES.WRITE)
            {
                a = PullNextParameter(firstMode);
                IO.OutputInt(a);
            }
            else if (opcode == OPCODES.SET_RELATIVE_BASE)
            {
                a = PullNextParameter(firstMode);
                _relativeBase += a;
            }

            //NO OPERAND
            else if (opcode == OPCODES.END)
            {
                ProgramTerminated = true;
                return false;
            }
            else
            {
                throw new ArgumentException("Unknown opcode");
            }

            return true;
        }

        public IntcodeComputer(string name, string rom, IInputOutput io) : base(name, File.ReadAllText(rom).Split(',').Select(long.Parse).ToArray(), io)
        {
            JumpTo(0);
        }

        public override bool Step()
        {
            var rawOpcode = MemoryReadOne();
            return Interpret(rawOpcode);
        }
    }
}