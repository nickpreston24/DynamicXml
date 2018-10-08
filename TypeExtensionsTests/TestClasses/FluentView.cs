using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DynamicXmlTests1.TestClasses
{
    internal class FluentView : IView
    {
        public string Name { get; set; }
        public string Background { get; set; }

        //FluentView(object values)
        //{
        //    //todo: map all values here
        //    //this.Slurp(values);
        //}

        //private bool ValidateBuild()
        //{
        //    throw new NotImplementedException(MethodBase.GetCurrentMethod().Name);
        //}

        //static Builder Get()
        //{

        //}

        //protected internal class Builder
        //{
        //    Builder(string viewName) => this.Name = viewName;

        //}
    }

    internal interface IView
    {
        string Name { get; set; }
        string Background { get; set; }
    }

    internal class ViewBuilder
    {
        FluentView view = new FluentView();

        public ViewBuilder(string name)
        {
            view.Name = name;
        }

        public ViewBuilder SetBackground(string hex)
        {
            view.Background = hex;
            return this;
        }

        public FluentView ToView()
        {
            return view ?? new FluentView();
        }
    }

    internal class IViewBuilder
    {
        IView view = new FluentView();

        public IViewBuilder(string name)
        {
            view.Name = name;
        }

        public IViewBuilder SetBackground(string hex)
        {
            view.Background = hex;
            return this;
        }

        public IView ToView()
        {
            return (view ?? new FluentView()) as IView;
        }
    }

}
