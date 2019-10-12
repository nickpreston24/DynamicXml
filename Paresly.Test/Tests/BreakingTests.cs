using DynamicXml;
using Shared.Classes;
using Shared.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xunit;

namespace Parsely.Test
{
    //[TestClass]
    public class BreakingTests
    {
        private readonly Scenario Scenario1 = new Scenario("Containers.xml");

        [Fact]
        public void CanUseCustomNugetPkg()
        {
        }

        [Fact]
        public void CanStreamArrays()
        {
            string xml = Scenario1.Xml;
            Validate(action: () =>
                 XmlStreamer.StreamInstances<ArraySets>(xml)
            );
            Debug.WriteLine(xml);
        }

        [Fact]
        public void CanStreamEnumerables()
        {
            string xml = Scenario1.Xml;
            Validate(action: () =>
                XmlStreamer.StreamInstances<EnumerableSets>(xml)
            );
            Debug.WriteLine(xml);
        }

        [Fact]
        //https://dotnetcodr.com/2017/02/07/convert-a-dynamic-type-to-a-concrete-object-in-net-c/
        public void DynamicToConcrete()
        {
            using (TimeIt.GetTimer())
            {
                dynamic dynamicKeyboard = new Keyboard()
                {
                    Name = "Corsair K70",
                    Price = 255,
                    Company = "Corsair",
                    SwitchType = "topre red"
                };
                Keyboard convertedKeyboard = dynamicKeyboard as dynamic;

                Assert.NotNull(convertedKeyboard);
                Assert.False(convertedKeyboard.HasNullProperties());
                //convertedKeyboard.Dump();
            }
        }

        [Fact]
        public void HappyPathTest()
        {
            var keyboard = XmlStreamer.StreamInstances<Keyboard>(XmlData.Keyboards).First();

            Debug.WriteLine(keyboard.Name);
            Debug.WriteLine(keyboard.Price);
            Assert.NotNull(keyboard);
        }

        private void Validate<T>(Func<IEnumerable<T>> action)
        {
            var sets = action().ToList();
            //var maybe = sets.ToMaybe();
            //maybe.Case(
            //    some: values => values.Dump(),
            //    none: () => Console.WriteLine($"No values of type '{nameof(EnumerableSets)}' discovered.."));
            Assert.NotNull(sets);
            Assert.True(sets.Any());
            //Assert.False(sets.HasNullProperties());
        }

        private class Scenario
        {
            private const string testDirectory = @"..\..\Files";

            public Scenario(string fileName) => FileName = fileName;

            public static string TestDirectory => testDirectory;

            public string FileName { get; }

            public string TestFilePath => $@"{testDirectory}\{FileName}";
            public string Xml => File.ReadAllText(TestFilePath);
        }
    }
}