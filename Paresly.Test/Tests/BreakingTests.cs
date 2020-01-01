using DynamicXml;
using Shared.Classes;
using Shared.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Utilities.Shared.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Parsely.Test
{
    //[TestClass]
    public class BreakingTests
    {
        private readonly ITestOutputHelper testOutputHelper;
        private readonly Scenario Scenario1 = new Scenario("Containers.xml");

        public BreakingTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void CanStreamArrays()
        {
            string xml = Scenario1.Xml;
            Validate(action: () => XmlStreamer.StreamInstances<ArraySets>(xml));
            Debug.WriteLine(xml);
        }

        [Fact]
        public void CanStreamEnumerables()
        {
            string xml = Scenario1.Xml;
            Validate(action: () => XmlStreamer.StreamInstances<EnumerableSets>(xml));
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
                convertedKeyboard.Dump();
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
            var collection = action().ToList();
            Assert.NotNull(collection);
            Assert.True(collection.Any());
        }

        private class Scenario
        {
            private const string testDirectory = "Files";

            public Scenario(string fileName) => FileName = fileName;

            public static string TestDirectory => testDirectory;

            public string FileName { get; }

            public string TestFilePath => $@"{TestDirectory}\{FileName}";
            public string Xml => File.ReadAllText(TestFilePath);
        }
    }
}