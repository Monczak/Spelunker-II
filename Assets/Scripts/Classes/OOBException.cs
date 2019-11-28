using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class OutOfBoundsException : Exception
{
    public OutOfBoundsException() { }
    public OutOfBoundsException(string message) : base(message) { }
    public OutOfBoundsException(string message, Exception inner) : base(message, inner) { }
}
