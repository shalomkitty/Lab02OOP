using System.Reflection;
using System.Xml.Xsl;
using System.Xml;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using Microsoft.Maui;

namespace Lab2MAUI
{
    public partial class MainPage : ContentPage
    {
        private string xmlFilePath = "";
        public MainPage()
        {
            InitializeComponent();
            SaxBtn.IsChecked = true;
        }

        private async void GetAllAuthors()
        {
            XmlDocument doc = new XmlDocument();
            var appLocation = Assembly.GetEntryAssembly().Location;
            var appPath = Path.GetDirectoryName(appLocation);
            Directory.SetCurrentDirectory(appPath);

            try
            {
                doc.Load(xmlFilePath);
            }
            catch 
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Invalid file type!", "Ok");
                return;
            }

            XmlElement xRoot = doc.DocumentElement;
            XmlNodeList childNodes = xRoot.SelectNodes("publication");

            Debug.WriteLine(childNodes.Count);
            for (int i = 0; i < childNodes.Count; i++)
            {
                XmlNode n = childNodes.Item(i);
                addItems(n);
            }

        }

        private void addItems(XmlNode n)
        {
            if (!NamePicker.Items.Contains(n.SelectSingleNode("@NAME").Value))
                NamePicker.Items.Add(n.SelectSingleNode("@NAME").Value);
            if (!FacultyPicker.Items.Contains(n.SelectSingleNode("@FACULTY").Value))
                FacultyPicker.Items.Add(n.SelectSingleNode("@FACULTY").Value);
            if (!DepartmentPicker.Items.Contains(n.SelectSingleNode("@DEPARTMENT").Value))
                DepartmentPicker.Items.Add(n.SelectSingleNode("@DEPARTMENT").Value);
            if (!SubjectsPicker.Items.Contains(n.SelectSingleNode("@SUBJECTS").Value))
                SubjectsPicker.Items.Add(n.SelectSingleNode("@SUBJECTS").Value);
            if (!MarksPicker.Items.Contains(n.SelectSingleNode("@MARKS").Value))
                MarksPicker.Items.Add(n.SelectSingleNode("@MARKS").Value);
        }


        private void SearchBtnHandler(object sender, EventArgs e)
        {
            editor.Text = "";

            MauiProgram.Publication publication = GetSelectedParameters();
            MauiProgram.IStrategy analyzer = GetSelectedAnalyzer();
            PerformSearch(publication, analyzer);
        }

        private MauiProgram.Publication GetSelectedParameters()
        {
            MauiProgram.Publication publication = new MauiProgram.Publication();

            if (NameCheckBox.IsChecked)
            {
                if (NamePicker.SelectedIndex != -1)
                    publication.Name = NamePicker.SelectedItem.ToString();
                else
                    publication.Name = "";
            }
            if (FacultyCheckBox.IsChecked)
            {
                if (FacultyPicker.SelectedIndex != -1)
                    publication.Faculty = FacultyPicker.SelectedItem.ToString();
                else
                    publication.Faculty = "";
            }
            if (DepartmentCheckBox.IsChecked)
            {
                if (DepartmentPicker.SelectedIndex != -1)
                    publication.Department = DepartmentPicker.SelectedItem.ToString();
                else
                    publication.Department = "";
            }
            if (SubjectsCheckBox.IsChecked)
            {
                if (SubjectsPicker.SelectedIndex != -1)
                    publication.Subjects = SubjectsPicker.SelectedItem.ToString();
                else
                    publication.Subjects = "";
            }
            if (MarksCheckBox.IsChecked)
            {
                if (MarksPicker.SelectedIndex != -1)
                    publication.Marks = MarksPicker.SelectedItem.ToString();
                else
                    publication.Marks = "";
            }
            return publication;
        }

        private MauiProgram.IStrategy GetSelectedAnalyzer()
        {
            MauiProgram.IStrategy analyzer = null;

            try
            {
                if (SaxBtn.IsChecked)
                {
                    analyzer = new MauiProgram.Sax();
                }
                if (DomBtn.IsChecked)
                {
                    analyzer = new MauiProgram.Dom();
                }
                if (LinqBtn.IsChecked)
                {
                    analyzer = new MauiProgram.Linq();
                }
            }
            catch
            {
                return null;
            }

            return analyzer;
        }

        private void PerformSearch(MauiProgram.Publication publication, MauiProgram.IStrategy analyzer)
        {
            MauiProgram.Searcher search = new MauiProgram.Searcher(publication, analyzer, xmlFilePath);
            List<MauiProgram.Publication> results = search.SearchAlgorithm();

            if (results == null) return;

            foreach (MauiProgram.Publication p in results)
            {
                
                editor.Text += "Name: " + p.Name + "\n";
                editor.Text += "Faculty: " + p.Faculty + "\n";
                editor.Text += "Department: " + p.Department + "\n";
                editor.Text += "Subjects: " + p.Subjects + "\n";
                editor.Text += "Marks: " + p.Marks + "\n";
                editor.Text += "\n";
            }
        }

        private void ClearFields(object sender, EventArgs e)
        {
            editor.Text = "";

            NameCheckBox.IsChecked = false;
            FacultyCheckBox.IsChecked = false;
            DepartmentCheckBox.IsChecked = false;
            SubjectsCheckBox.IsChecked = false;
            MarksCheckBox.IsChecked = false;

            NamePicker.SelectedItem = null;
            FacultyPicker.SelectedItem = null;
            DepartmentPicker.SelectedItem = null;
            SubjectsPicker.SelectedItem = null;
            MarksPicker.SelectedItem = null;
        }

        private async void OnTransformToHTMLBtnClicked(object sender, EventArgs e)
        {
            XslCompiledTransform xct = LoadXSLT();
            if (xct == null) return;

            string xmlPath = xmlFilePath;

            if (xmlFilePath.Length == 0) return;
            string htmlPath = xmlFilePath.Substring(0, xmlFilePath.Length - 4) + ".html";

            XsltArgumentList xslArgs = await CreateXSLTArguments();

            TransformXMLToHTML(xct, xslArgs, xmlPath, htmlPath);

            await Application.Current.MainPage.DisplayAlert("Message", "File saved.", "Ok");
        }

        private async void OnOpenFileButton(object sender, EventArgs e)
        {
            var fileResult = await FilePicker.PickAsync();

            if (fileResult != null)
            {
                xmlFilePath = fileResult.FullPath;
            }

            editor.Text = "";

            GetAllAuthors();
        }

        private XslCompiledTransform LoadXSLT()
        {
            XslCompiledTransform xct = new XslCompiledTransform();

            try
            {
                xct.Load("C:\\Users\\artem\\OneDrive\\Desktop\\Lab2_OOP-main\\Transform.xslt");
            }
            catch { }

            return xct;
        }

        private Task CreateXSLError()
        {
            return Application.Current.MainPage.DisplayAlert("Error", "Fill in all attributes!", "Ok");
        }

        private async Task<XsltArgumentList> CreateXSLTArguments()
        {
            XsltArgumentList xslArgs = new XsltArgumentList();

            // Add filter values from Pickers (null means "show all")
            AddParamIfNotNull(xslArgs, "name", NamePicker.SelectedItem?.ToString());
            AddParamIfNotNull(xslArgs, "faculty", FacultyPicker.SelectedItem?.ToString());
            AddParamIfNotNull(xslArgs, "department", DepartmentPicker.SelectedItem?.ToString());
            AddParamIfNotNull(xslArgs, "subjects", SubjectsPicker.SelectedItem?.ToString());
            AddParamIfNotNull(xslArgs, "marks", MarksPicker.SelectedItem?.ToString());

            // Add visibility flags from CheckBoxes
            xslArgs.AddParam("showName", "", NameCheckBox.IsChecked);
            xslArgs.AddParam("showFaculty", "", FacultyCheckBox.IsChecked);
            xslArgs.AddParam("showDepartment", "", DepartmentCheckBox.IsChecked);
            xslArgs.AddParam("showSubjects", "", SubjectsCheckBox.IsChecked);
            xslArgs.AddParam("showMarks", "", MarksCheckBox.IsChecked);

            return xslArgs;
        }

        private void AddParamIfNotNull(XsltArgumentList xslArgs, string name, string? value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                xslArgs.AddParam(name, "", value);
            }
        }

        private void TransformXMLToHTML(XslCompiledTransform xct, XsltArgumentList xslArgs, string xmlPath, string htmlPath)
        {
            using (XmlReader xr = XmlReader.Create(xmlPath))
            {
                using (XmlWriter xw = XmlWriter.Create(htmlPath))
                {
                    try
                    {
                        xct.Transform(xr, xslArgs, xw);
                    }
                    catch { }
                }
            }
        }

        private async void OnExitBtnClicked(object sender, EventArgs e)
        {
            var result = await Application.Current.MainPage.DisplayAlert("Exit", "Are you sure you?", "Yes", "No");
            if (result)
            {
                System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            }
        }

        private void Editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            int textLength = editor.Text.Length;
            int fontSize = CalculateFontSize(textLength);
            editor.FontSize = fontSize;
        }

        private int CalculateFontSize(int textLength)
        {
            if (textLength < 100)
            {
                return 18;
            }
            else if (textLength < 500)
            {
                return 14;
            }
            else
            {
                return 10;
            }
        }
    }

}
