using System;
using Parsely;
using Parsely.MarkdownBuilder;

namespace MarkdownTest.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Let's make some Markdown!");

            string result = MarkdownBuilder
                .CreateMarkdown("Hello, World!")
                .AddHeader("Gravity Wells", 4)
                .AddHeader("MEGA HEADER", 960)
                .Build();
            
            Console.WriteLine(result);
            // Console.ReadLine();
        }
    }
}