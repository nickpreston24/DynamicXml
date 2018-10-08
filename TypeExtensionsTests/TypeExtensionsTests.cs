using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared;
using System;
using System.Diagnostics;
using System.Reflection;

namespace DynamicXml.Extensions.Tests
{
    [TestClass()]
    public class TypeExtensionsTests
    {
        [TestMethod()]
        public void GetDefaultValueTest()
        {
            var civilian = new Wrapper<Civilian>();

            Debug.WriteLine((civilian.MyGenericTypeDefinitionInstance as Civilian).Name);

            Assert.IsNotNull(civilian);
        }

        [TestMethod()]
        public void ToListTypeTest()
        {
            //throw new NotImplementedException(MethodBase.GetCurrentMethod().Name);
            //Assert.Inconclusive();
        }

        /// <summary>
        /// Multiple implicit operators! :D yay!
        /// </summary>
        [TestMethod]
        public void CanConvert_MulitpleImplicitTypes()
        {
            /*PROMOTING*/

            //Assemble
            var person = new Civilian { Name = "Bob", Age = 21 };

            //Act
            Debug.WriteLine("Rank Up");

            Recruit recruit = person;
            Private @private = recruit;
            Officer officer = @private;

            //Assert
            Assert.IsNotNull(person);
            Assert.IsNotNull(recruit);
            Assert.IsNotNull(@private);
            Assert.IsNotNull(officer);

            //Debug.WriteLine(person.ToString());
            //Debug.WriteLine(@private.ToString());
            //Debug.WriteLine(recruit.ToString());
            //Debug.WriteLine(officer.ToString());

            person.Dump();
            @private.Dump();
            recruit.Dump();
            officer.Dump();

            /*DEMOTING!*/

            //Act
            Debug.WriteLine("Rank Down");
            @private = officer;
            person = @private;

            //Assert
            Assert.IsNotNull(person);
            Assert.IsNotNull(recruit);
            Assert.IsNotNull(@private);
            Assert.IsNotNull(officer);

            //Debug.WriteLine(person.ToString());
            //Debug.WriteLine(@private.ToString());
            //Debug.WriteLine(recruit.ToString());
            //Debug.WriteLine(officer.ToString());

            person.Dump();
            @private.Dump();
            recruit.Dump();
            officer.Dump();
        }
    }

    class Officer : Civilian
    {
        public new string Rank { get; set; } = nameof(Officer);
        public string Designation
        {
            get => $"{Rank} {Name}";
            set => Designation = value;
        }

        public static implicit operator Private(Officer officer) => new Private { Name = officer.Name, Rank = nameof(Private) };
        public override string ToString() => base.ToString();
    }
    class Private : MilitaryPersonnel
    {
        public Private() : base(nameof(Private)) { }

        public new string Rank { get; set; } = nameof(Private);
        public static implicit operator Officer(Private @private) => new Officer { Name = @private.Name, Age = @private.Age };
        public override string ToString() => base.ToString();
    }
    class Recruit : MilitaryPersonnel
    {
        public Recruit() : base(nameof(Recruit)) { }

        public static implicit operator Private(Recruit recruit) => new Private { Name = recruit.Name, Age = recruit.Age };
        public static implicit operator Recruit(Civilian civilian) => new Recruit() { Name = civilian.Name, Age = civilian.Age }; //recruited!
        public override string ToString() => $"{base.ToString()}\nRank: {Rank}";
    }
    public class Civilian : Person
    {
        public string Rank = nameof(Civilian);
        public override string ToString() => $"{base.ToString()}\nRank: {Rank}";
    }

    public abstract class MilitaryPersonnel : Person, IPersonnel
    {
        public virtual string Rank { get; set; }

        protected MilitaryPersonnel(string rank) => Rank = rank;

        public override string ToString() => $"{base.ToString()}\nRank: {Rank}";
    }
}