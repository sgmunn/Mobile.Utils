// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleLog.cs" company="sgmunn">
//   (c) sgmunn 2013  
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//   the Software.
//
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//   THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//   IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Mobile.Utils.Diagnostics
{
    using System;

    public sealed class ConsoleLog : ILog
    {
        public static readonly ILog Instance = new ConsoleLog();

        private ConsoleLog()
        {
        }

        public void Write(string message)
        {
            Console.Write(message);
        }

        public void Write(string message, params object[] args)
        {
            Console.Write(message, args);
        }

        public void Debug(string message)
        {
            #if DEBUG
            Console.Write(message);
            #endif
        }

        public void Debug(string message, params object[] args)
        {
            #if DEBUG
            Console.Write(message, args);
            #endif
        }
    }
}
