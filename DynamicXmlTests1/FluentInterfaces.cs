using DynamicXmlTests1.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared;
using System.Diagnostics;

namespace FluentInterfaces
{
    [TestClass]
    public class FluentInterfaceTest
    {
        [TestMethod]
        public void TryFluentClass()
        {
            FluentEmployee employee = new FluentEmployee()
                .WithName("Michael Bolton")
                .WithAge(55)
                .TakeVacationAsync();

            Debug.WriteLine(employee.LastName);
            Debug.WriteLine(employee.FirstName);

            Assert.IsTrue(string.Equals(employee.LastName, ("Bolton")));
            Assert.IsTrue(employee.Anxiety < 3);
            employee.Dump();
        }

        [TestMethod]
        public void ViewBuilderControl()
        {
            string hex = "#777777";
            var view = new ViewBuilder("Main")
                .SetBackground(hex)
                .ToView();
            view.Dump();

            Assert.IsNotNull(view);
            Assert.AreEqual(hex, view.Background);
        }

        [TestMethod]
        public void IViewBuilderControl()
        {
            string hex = "#777777";
            var view = new IViewBuilder("Main")
                .SetBackground(hex)
                .ToView();
            view.Dump();
            Assert.IsNotNull(view);
            Assert.AreEqual(hex, view.Background);
        }
    }
}
