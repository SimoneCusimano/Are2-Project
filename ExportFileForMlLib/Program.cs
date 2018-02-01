using Are2Project.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ExportFileForMlLib
{
    class Program
    {
        static void Main(string[] args)
        {
            var rows = new List<object>();

            foreach (var file in Directory.EnumerateFiles("./input", "*.json"))
            {
                var content = File.ReadAllText(file);
                var photoDescriptions = JsonConvert.DeserializeObject<IList<PhotoDescription>>(content);

                foreach (var photoDescription in photoDescriptions)
                {
                    var row = new
                    {
                        Url = photoDescription?.Url,
                        Tags = photoDescription?.Description?.Description?.Tags?.Except(
                            new List<string>
                            {
                                "person", "man", "people", "woman", "crowd", "girl", "boy", "group", "child", "lady", "male", "female", "family", "crowded", "young", "player"
                            }).ToList()?.Aggregate((i, j) => i + " " + j),
                        ResultType = GuessResultType(photoDescription?.Description?.Faces)
                    };
                    rows.Add(row);
                }
            }

            var jsonRows = JsonConvert.SerializeObject(rows);
            File.WriteAllText(@"./output/result.json", jsonRows);
        }

        private static string GuessResultType(IList<Face> faces)
        {
            var result = ResultType.Nobody;

            var thereAreMale = faces.Any(f => f.Gender.Equals("Male", StringComparison.InvariantCultureIgnoreCase));
            var thereAreFemale = faces.Any(f => f.Gender.Equals("Female", StringComparison.InvariantCultureIgnoreCase));

            if (thereAreMale && thereAreFemale)
                result = ResultType.Both;
            else if (thereAreMale && !thereAreFemale)
                result = ResultType.Male;
            else if (!thereAreMale && thereAreFemale)
                result = ResultType.Female;

            return GetEnumDescription(result);
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }

    enum ResultType
    {
        [Description("Nobody")]
        Nobody,
        [Description("Male")]
        Male,
        [Description("Female")]
        Female,
        [Description("Both")]
        Both
    };

}
