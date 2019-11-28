using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class CaveErosionException : Exception
{
    public CaveErosionException() { }
    public CaveErosionException(string message) : base(message) { }
    public CaveErosionException(string message, Exception inner) : base(message, inner) { }
}

