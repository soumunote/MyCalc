using System;

class Program
{
    static void Main(string[] args)
    {
        //Console.WriteLine(new SafeDecimal("123") / new SafeDecimal("14"));
        //Console.WriteLine(new SafeDecimal("0.05") / new SafeDecimal("0.1"));
        //Console.WriteLine(new SafeDecimal("73") / new SafeDecimal("0.34"));
        var dec = new SafeDecimal("73") / new SafeDecimal("0.34");
        Console.WriteLine(dec);
        Console.ReadKey();
    }
}
