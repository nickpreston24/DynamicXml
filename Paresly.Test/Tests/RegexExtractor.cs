using Shared.Diagnostics;
using System;
using Xunit;
using Xunit.Abstractions;

namespace RegexBuilder.Tests
{
    public class RegexExtractorTests
    {
        private readonly ITestOutputHelper debugger;
        private static readonly string lineItem = "Michael     30     USA     3.4 abdc!xi#45?          ";

        public RegexExtractorTests(ITestOutputHelper debugger) => this.debugger = debugger;

        [Fact]
        public void ExtractTest()
        {
            string pattern = @"\s*(?<FirstName>[a-zA-Z]+)\s*(?<Age>\d+)\s*(?<Country>[a-zA-Z]+)\s*(?<GPA>\d+.\d{1})";
            var expected = new Person
            {
                FirstName = "Michael",
                Age = 30,
                GPA = 3.4,
                Country = "USA",
            };
            using (TimeIt.GetTimer())
            {
                Print(expected.ToString());
                Person mike = lineItem.Extract<Person>(pattern);
                Print(mike.ToString());
                Assert.Equal(mike, expected);
            }
        }

        [Fact]
        public void GeneratePattern()
        {
            var expected = new Person
            {
                FirstName = "Michael",
                Age = 30,
                Country = "USA",
                GPA = 3.4,
            };
            using (TimeIt.GetTimer())
            {
                string pattern = expected.GenerateRegex();
                Print(pattern);
                Assert.NotNull(pattern);

                var actual = lineItem.Extract<Person>(pattern);
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void CanBuildPattern()
        {
            string fullNameText = "Michael Nicholas Preston";
            FullName fullname = fullNameText;
            Print(fullname.ToString());
            var builder = new PatternBuilder<Person>();
            var regex = builder.Add<FullName>()
                //.Add<Email>()
                .Pattern;
            string pattern = regex.ToString();
            Print(pattern);
            Assert.NotNull(pattern);
        }
        
        private void Print(string text) => debugger.WriteLine(text);
    }
}