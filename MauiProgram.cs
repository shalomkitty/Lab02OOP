using Microsoft.Extensions.Logging;
using System.Xml.Linq;
using System.Xml;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Maui.Controls;

namespace Lab2MAUI
{
    public static class MauiProgram
    {
        public class Publication
        {
            public string Name { get; set; }
            public string Faculty { get; set; }
            public string Department { get; set; }
            public string Subjects { get; set; }
            public string Marks { get; set; }
 
            public Publication() { }
        }

        static private string xmlFilePath;

        public interface IStrategy
        {
            List<Publication> Search(Publication publication);
        }
        public class Searcher
        {
            private Publication publication;
            private IStrategy strategy;

            public Searcher(Publication p, IStrategy str, string path)
            {
                publication = p;
                strategy = str;
                xmlFilePath = path;
            }

            public List<Publication> SearchAlgorithm()
            {
                return strategy.Search(publication);
            }
        }

        public class Sax : IStrategy
        {
            public List<Publication> Search(Publication publication)
            {
                List<Publication> results = new List<Publication>();
                XmlTextReader xmlReader;

                try
                {
                    xmlReader = new XmlTextReader(xmlFilePath);
                }
                catch
                {
                    return null;
                }

                while (xmlReader.Read())
                {
                    if (xmlReader.HasAttributes)
                    {
                        while (xmlReader.MoveToNextAttribute())
                        {
                            string name = "";
                            string faculty = "";
                            string department = "";
                            string subjects = "";
                            string marks = "";

                            if (xmlReader.Name.Equals("NAME") && (xmlReader.Value.Equals(publication.Name) || publication.Name == null))
                            {
                                name = xmlReader.Value;
                                xmlReader.MoveToNextAttribute();
                                if (xmlReader.Name.Equals("FACULTY") && (xmlReader.Value.Equals(publication.Faculty) || publication.Faculty == null))
                                {
                                    faculty = xmlReader.Value;
                                    xmlReader.MoveToNextAttribute();
                                    if (xmlReader.Name.Equals("DEPARTMENT") && (xmlReader.Value.Equals(publication.Department) || publication.Department == null))
                                    {
                                        department = xmlReader.Value;
                                        xmlReader.MoveToNextAttribute();
                                        if (xmlReader.Name.Equals("SUBJECTS") && (xmlReader.Value.Equals(publication.Subjects) || publication.Subjects == null))
                                        {
                                            subjects = xmlReader.Value;
                                            xmlReader.MoveToNextAttribute();
                                            if (xmlReader.Name.Equals("MARKS") && (xmlReader.Value.Equals(publication.Marks) || publication.Marks == null))
                                            {
                                                marks = xmlReader.Value;
                                            }
                                        }
                                    }
                                }
                            }

                            if (name != "" && faculty != "" && department != "" && subjects != "" && marks != "")
                            {
                                Publication newPublication = new Publication { Name = name, Department = department, Faculty = faculty, Subjects = subjects, Marks = marks };
                                results.Add(newPublication);
                            }
                        }
                    }
                }
                xmlReader.Close();
                return results;
            }
        }

        public class Dom : IStrategy
        {
            public List<Publication> Search(Publication publication)
            {
                List<Publication> results = new List<Publication>();
                XmlDocument doc = new XmlDocument();

                try
                {
                    doc.Load(xmlFilePath);
                }
                catch
                {
                    return null;
                }

                XmlNode node = doc.DocumentElement;
                foreach (XmlNode n in node.ChildNodes)
                {
                    string name = "";
                    string faculty = "";
                    string department = "";
                    string subjects = "";
                    string marks = "";

                    foreach (XmlAttribute attribute in n.Attributes)
                    {
                        if (attribute.Name.Equals("NAME") && (attribute.Value.Equals(publication.Name) || publication.Name == null))
                            name = attribute.Value;
                        if (attribute.Name.Equals("FACULTY") && (attribute.Value.Equals(publication.Faculty) || publication.Faculty == null))
                            faculty = attribute.Value;
                        if (attribute.Name.Equals("DEPARTMENT") && (attribute.Value.Equals(publication.Department) || publication.Department == null))
                            department = attribute.Value;
                        if (attribute.Name.Equals("SUBJECTS") && (attribute.Value.Equals(publication.Subjects) || publication.Subjects == null))
                            subjects = attribute.Value;
                        if (attribute.Name.Equals("MARKS") && (attribute.Value.Equals(publication.Marks) || publication.Marks == null))
                            marks = attribute.Value;
                    }

                    if (name != "" && faculty != "" && department != "" && subjects != "" && marks != "")
                    {
                        Publication newPublication = new Publication { Name = name, Department = department, Faculty = faculty, Subjects = subjects, Marks = marks };
                        results.Add(newPublication);
                    }
                }
                return results;
            }
        }

        public class Linq : IStrategy
        {
            public List<Publication> Search(Publication publication)
            {
                List<Publication> results = new List<Publication>();
                XDocument doc;

                Debug.WriteLine(1);

                doc = XDocument.Load(xmlFilePath);

                var result = from obj in doc.Descendants("publication")
                             where
                             (
                             (obj.Attribute("NAME").Value.Equals(publication.Name) || publication.Name == null) &&
                             (obj.Attribute("FACULTY").Value.Equals(publication.Faculty) || publication.Faculty == null) &&
                             (obj.Attribute("DEPARTMENT").Value.Equals(publication.Department) || publication.Department == null) &&
                             (obj.Attribute("SUBJECTS").Value.Equals(publication.Subjects) || publication.Subjects == null) &&
                             (obj.Attribute("MARKS").Value.Equals(publication.Marks) || publication.Marks == null)    
                             )
                             select new
                             {
                                 name = (string)obj.Attribute("NAME"),
                                 faculty = (string)obj.Attribute("FACULTY"),
                                 department = (string)obj.Attribute("DEPARTMENT"),
                                 subjects = (string)obj.Attribute("SUBJECTS"),
                                 marks = (string)obj.Attribute("MARKS"),
                             };

                Debug.WriteLine(2);

                foreach (var p in result)
                {
                    Debug.WriteLine(3);
                    Publication newPublication = new Publication { Name = p.name, Department = p.department, Faculty = p.faculty, Subjects = p.subjects, Marks = p.marks };
                    results.Add(newPublication);
                }
                return results;
            }
        }

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
