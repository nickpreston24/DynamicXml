using DynamicXml;
using DynamicXmlTests.Classes.Extractables;
using DynamicXmlTests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Diagnostics;
using Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Maybe;

namespace DynamicXmlTests
{
    [TestClass]
    public class BreakingTests
    {
        private readonly Scenario Scenario1 = new Scenario("Containers.xml");

        [TestMethod]
        public void CanStreamArrays()
        {
            string xml = Scenario1.Xml;
            Validate(action: () =>
                 XmlStreamer.StreamInstances<ArraySets>(xml)
            );
            Debug.WriteLine(xml);
        }

        [TestMethod]
        public void CanStreamEnumerables()
        {
            string xml = Scenario1.Xml;
            Validate(action: () =>
                XmlStreamer.StreamInstances<EnumerableSets>(xml)
            );
            Debug.WriteLine(xml);
        }

        [TestMethod]
        //https://dotnetcodr.com/2017/02/07/convert-a-dynamic-type-to-a-concrete-object-in-net-c/
        public void DynamicToConcrete()
        {
            using (var timer = TimeIt.GetTimer())
            {
                dynamic dynamicKeyboard = new Keyboard()
                {
                    Name = "Corsair K70",
                    Price = 255,
                    Company = "Corsair",
                    SwitchType = "topre red"
                };
                Keyboard convertedKeyboard = dynamicKeyboard as dynamic;

                Assert.IsNotNull(convertedKeyboard);
                Assert.IsFalse(convertedKeyboard.HasNullProperties());
                //convertedKeyboard.Dump();
            }
        }

        [TestMethod]
        public void HappyPathTest()
        {
            var keyboard = XmlStreamer.StreamInstances<Keyboard>(XmlData.Keyboards).First();

            Debug.WriteLine(keyboard.Name);
            Debug.WriteLine(keyboard.Price);
            Assert.IsNotNull(keyboard);
        }

        private void Validate<T>(Func<IEnumerable<T>> action)
        {
            var sets = action().ToList();
            var maybe = sets.ToMaybe();
            //maybe.Case(
            //    some: values => values.Dump(),
            //    none: () => Console.WriteLine($"No values of type '{nameof(EnumerableSets)}' discovered.."));
            Assert.IsNotNull(sets);
            Assert.IsTrue(sets.Any());
            Assert.IsFalse(sets.HasNullProperties());
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