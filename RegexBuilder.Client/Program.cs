using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Parsely.RegexBuilder;
using Utils.Debugging;


namespace RegexBuilder.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var importer = new ImportJobService();
            var job = importer.GetImportJob(5);
            string ProjectDirPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"../../../"));

            var grepper = new FSGrep
            {
                RootPath = Path.Combine(ProjectDirPath, "TestFiles"),
                FileSearchMask = job.Extension
            };

            var files = grepper.GetFileNames();
            files.Dump(nameof(files));

            job.Dump(nameof(job));

            "1234_Notes_20200103"
                .Extract<ImportFile>(@"(?<LoanNumber>\d+)_(?<EFolder>[a-zA-Z]+)_(?<Date>\d+)")
                .Dump();

#if DEBUG
            // Console.ReadLine();
#endif
        }

        public class ImportFile
        {
            public int LoanNumber { get; set; }
            public string eFoLdEr { get; set; }
            public string Date { get; set; }
        }

        public class FilePartsRepo
        {
            public List<FileNamePart> GetParts()
            {
                return new List<FileNamePart>
                {
                    new FileNamePart
                    {
                        Delimiter = '_',
                        Position = 2,
                        Name = "eFolder",
                        Value = "AlternateImport-File",
                        Type = "Literal"
                    },
                    new FileNamePart
                    {
                        Delimiter = '_',
                        Position = 3,
                        Name = "LoanDate",
                        Value = "yyyyMM:dd",
                        Type = "DateTimeFormat"
                    },
                    new FileNamePart
                    {
                        Delimiter = '_',
                        Position = 1,
                        Name = "LoanNumber",
                        Value = @"\d+",
                        Type = "Number"
                    }
                    // ,
                    // new FileNamePart
                    // {
                    //     Delimiter = '_',
                    //     Position = 1,
                    //     Name = "Loan.Number",
                    //     Value = @"\d+",
                    //     Type="Guid"
                    // }
                };
            }
        }

        public class ImportJobService
        {
            // loop thru all file name parts and append all Values to their delimiters
            bool useRegex = true;

            public ImportJob GetImportJob(uint id) => new ImportJob
            {
                SearchPattern = new FilePartsRepo()
                    .GetParts().Dump("parts")
                    .OrderBy(part => part.Position)
                    .Aggregate(new StringBuilder(),
                        (builder, part) =>
                        {
                            //patch:
                            if (part.Type.Equals("Literal"))
                                // part.Value = @"\"{part.Value}";
                                part.Value = @"\d"; //TODO: fix literal quotes 
                            
                            if (useRegex)
                                builder.Append(part.IsPrefixed
                                    ? $"{part.Delimiter}(?<{part.Name}>{part.Value})"
                                    : $"(?<{part.Name}>{part.Value}){part.Delimiter}");
                            else
                                builder.Append(part.IsPrefixed
                                    ? $"{part.Delimiter}{part.Value}"
                                    : $"{part.Value}{part.Delimiter}");

                            return builder;
                        })
                    .ToString()
            };
        }

        public class ImportJob
        {
            public string SourceDirectory { get; set; } = string.Empty;
            public string Extension { get; set; } = "*.txt";
            public string SearchPattern { get; set; } = string.Empty;
        }

        public class FileNamePart
        {
            public char Delimiter { get; set; } = char.MinValue;
            public string Value { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public uint Position { get; set; } = 1;
            public bool IsPrefixed { get; set; }
            public string Type { get; set; } = string.Empty;
        }
    }
}