using System;
using System.Collections.Generic;

namespace Parsely.MarkdownBuilder
{
    public class MarkdownBuilder
    {
        // Queue<IMarkDownPart> markDownParts = new Queue<IMarkDownPart>(new List<IMarkDownPart>());
        public IMarkDownPart CurrentPart { get; } = null;

        // readonly List<Func<<IMarkDownPart>, <IMarkDownPart>>> steps = new List<Func<Queue<IMarkDownPart>, Queue<IMarkDownPart>>>(0);    
        List<Func<IMarkDownPart, IMarkDownPart>> steps = new List<Func<IMarkDownPart, IMarkDownPart>>(0);

        Func<IMarkDownPart, IMarkDownPart> constructorFunc = null;

        MarkdownBuilder(string initialText)
        {
            CurrentPart = new MarkDownHeader(initialText);
            constructorFunc = _ => CurrentPart;
        }

        public static MarkdownBuilder CreateMarkdown(string initialText) => new MarkdownBuilder(initialText);

        // MarkdownBuilder ApplyStyle<T>(T style, int block) where T : MarkDownStyle
        // {
        //
        //     return this;
        // }

        public MarkdownBuilder AddHeader(string text, uint number) //string headerText, uint number
        {
            if (number > 6)
                number = 6;

            Func<IMarkDownPart, IMarkDownPart> func = part => new MarkDownHeader(text, (int)number) { Next = part };
            constructorFunc = constructorFunc.Compose(func);
            // steps.Add(func);
            return this;
        }

        // /// <summary>
        // /// Returns the current snapshot of the Markdown text 
        // /// </summary>
        // /// <returns></returns>
        // string Preview()
        // {
        //
        //     return string.Empty;
        // }

        /// <summary>
        /// Commits the entire build of Markdown.  No going back!
        /// </summary>
        /// <returns></returns>
        public string Build()
        {
            // return steps.Aggregate(
            //         new StringBuilder(),
            //         (builder, step) => step(builder))
            //     .ToString();


            return constructorFunc(null).Text;
        }
    }

    /// <summary>
    /// Represents a Markdown Header and its rules
    /// </summary>
    internal class MarkDownHeader : IMarkDownPart
    {
        // public HeaderType Header { get; }
        public string Text { get; }
        public IMarkDownPart Next { get; set; }
        public uint Header { get; }

        public MarkDownHeader(string text = null, int header = 1)
        {
            Text = $"{new string('#', header)} {text?.Trim() ?? string.Empty}";
            Header = (uint)header;
        }
    }

    public static partial class Extensions
    {
        public static Func<A, C> Compose<A, B, C>(this Func<A, B> f, Func<B, C> g) => a => g(f(a));
    }

    internal enum HeaderType
    {
        Header1 = 1,
        Header2 = 2,
        Header3 = 3,
        Header4 = 4,
        Header5 = 5,
        Header6 = 6
    }

    public class MarkDownStyle
    {
    }

    public interface IMarkDownPart
    {
        string Text { get; }
        IMarkDownPart Next { get; }
    }
}