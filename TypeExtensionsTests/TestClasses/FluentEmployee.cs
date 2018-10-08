using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FluentInterfaces
{
    /// <summary>
    /// Sample - Implementing the fluent interface pattern
    /// </summary>
    internal class FluentEmployee
    {
        private char[] splitChars = { ' ', ',' };

        public FluentEmployee()
        {
        }

        public string FullName { get; set; }
        public string LastName { get { return FullName.Split(splitChars).Last(); } }
        public string FirstName { get { return FullName.Split(splitChars).First(); } }

        public int Age { get; private set; }
        public int Anxiety { get; private set; } = 5;

        public FluentEmployee WithName(string fullName)
        {
            FullName = fullName;
            return this;
        }

        public FluentEmployee WithAge(int age)
        {
            Age = age;
            return this;
        }

        public async Task<FluentEmployee> TakeVacationAsync()
        {
            await Task.Run(() => { Thread.Sleep(500); });
            Anxiety = 1;
            return this;
        }

        public static implicit operator FluentEmployee(Task<FluentEmployee> task)
        {
            return task.Result ?? throw new NullReferenceException("Employee came back null!");
        }
    }
}
