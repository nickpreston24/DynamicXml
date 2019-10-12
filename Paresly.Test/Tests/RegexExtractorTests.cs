using Shared.Diagnostics;
using System;
using Xunit;

namespace RegexBuilder.Tests
{
    public class RegexExtractorTests
    {
        private static readonly string text = "Michael 30 USA 3.4";

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
                Console.WriteLine(expected);
                Person mike = text.Extract<Person>(pattern);
                Console.WriteLine(mike.ToString());
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
                Console.WriteLine(pattern);
                Assert.NotNull(pattern);

                var actual = text.Extract<Person>(pattern);
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void CanBuildPattern()
        {
            string fullNameText = "Michael Nicholas Preston";
            FullName fullname = fullNameText;
            Console.WriteLine(fullname.ToString());
            var builder = new PatternBuilder<Person>();
            var regex = builder.Add<FullName>()
                //.Add<Email>()
                .Pattern;
            string pattern = regex.ToString();
            Console.WriteLine(pattern);
            Assert.NotNull(pattern);
        }
    }
}