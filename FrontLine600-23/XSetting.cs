using System.IO;
using System.Linq;
using System.Xml.Linq;
using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace FrontLine600_23
{
    public class XSetting : IDisposable
    {
        private  XDocument _xDocument;
        
        private static string exeFile = System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "");
        private static string _settingLoacation = Path.GetDirectoryName(exeFile);

        private string FileName => _settingLoacation+ @"\XSetting.xml";

        public XSetting()
        {
            var exist = CheckFileExist(FileName);
            if (!exist)
            {
                CreateSetting(FileName);
            }
            LoadSetting(FileName);
        }

        private void LoadSetting(string fileName)
        {
            _xDocument = XDocument.Load(fileName);
        }

        private void CreateSetting(string fileName)
        {
            XDocument doc =
              new XDocument(
                new XElement("Setting",
                  new XElement("Comport", new XAttribute("Name", "Weighing"),
                            new XElement("ComName",  "COM1"),
                            new XElement("BaudRate", 9600),
                            new XElement("Databits", 8),
                            new XElement("Stopbits",  StopBits.One),
                            new XElement("Parity", Parity.None),
                            new XElement("Prefix",  "Weight:"),
                            new XElement("Suffix",  "Kg"),
                            new XElement("Eof",  13)
                  ),
                   new XElement("Comport", new XAttribute("Name", "Barcode1"),
                            new XElement("ComName", "COM1"),
                            new XElement("BaudRate", 9600),
                            new XElement("Databits", 8),
                            new XElement("Stopbits", StopBits.One),
                            new XElement("Parity", Parity.None),
                            new XElement("Prefix", ""),
                            new XElement("Suffix", ""),
                            new XElement("Eof", 13)
                  ),
                    new XElement("Comport", new XAttribute("Name", "Barcode2"),
                            new XElement("ComName", "COM1"),
                            new XElement("BaudRate", 9600),
                            new XElement("Databits", 8),
                            new XElement("Stopbits", StopBits.One),
                            new XElement("Parity", Parity.None),
                            new XElement("Prefix", ""),
                            new XElement("Suffix", ""),
                            new XElement("Eof", 13)
                  ),
                  new XElement("Label", new XAttribute("title", "Individual Label 1"), new XAttribute("Name", "Individual1"),
                            new XElement("Location", ""),
                            new XElement("TopOffset", 0),
                            new XElement("LeftOffset", 0),
                            new XElement("Printer", 0)
                  ),
                  new XElement("Label", new XAttribute("title", "Grouping Label 1"), new XAttribute("Name", "Group1"),
                            new XElement("Location", ""),
                            new XElement("TopOffset", 0),
                            new XElement("LeftOffset", 0),
                            new XElement("Printer", 0)
                  ),
                   new XElement("Label", new XAttribute("title", "Incomplete Label 1"), new XAttribute("Name", "Incomplete1"),
                            new XElement("Location", ""),
                            new XElement("TopOffset", 0),
                            new XElement("LeftOffset",  0),
                            new XElement("Printer",  0)
                  ),
                  new XElement("User", new XAttribute("password", "Pass1234")),
                 new XElement("Lastrunning", 
                            new XElement("Reference", ""),
                            new XElement("Article", ""),
                            new XElement("IndividualPass", 0),
                            new XElement("IndividualFail", 0),
                            new XElement("Box", 0)
                 ),
                  new XElement("Database",
                            new XElement("Provider", "Microsoft.ACE.OLEDB.12.0"),
                            new XElement("FileLocation", @"D:\GITSRC\SCH_PEM_FL603_CSHARP\TesysPackaging\bin\Debug\DBase\TeSys.mdb"),
                            new XElement("TableName", "Tesys_Contactors_FL")
                 ),
                  new XElement("LabelVariables", new XAttribute("Name", "Individual1"),
                            new XElement("var", "Art_number"),
                            new XElement("var", "Bitmap"),
                            new XElement("var", "Current"),
                            new XElement("var", "Desc_Eng"),
                            new XElement("var", "Desc_Fre"),
                            new XElement("var", "Desc_Ger"),
                            new XElement("var", "Desc_Spa"),
                            new XElement("var", "Desc_Ita"),
                            new XElement("var", "Load_power"),
                            new XElement("var", "Power"),
                            new XElement("var", "Reference"),
                            new XElement("var", "Back"),
                            new XElement("var", "voltage"),
                            new XElement("var", "imgEAC"),
                            new XElement("var", "imgCTP"),
                            new XElement("var", "imgUL"),
                            new XElement("var", "imgKC"),
                            new XElement("var", "KC_Number"),
                            new XElement("var", "C_Indonesia"),
                            new XElement("var", "Madein_New"),
                            new XElement("var", "Madein_Ru"),
                            new XElement("var", "Madein_Ch"),
                            new XElement("var", "Madein_Old"),
                            new XElement("var", "DescPro_Ru"),
                            new XElement("var", "Path")
                 ),
                  new XElement("LabelVariables", new XAttribute("Name", "Group1"),
                                new XElement("var", "Art_number"),
                                new XElement("var", "Bitmap"),
                                new XElement("var", "Current"),
                                new XElement("var", "Desc_Eng"),
                                new XElement("var", "Desc_Fre"),
                                new XElement("var", "Desc_Ger"),
                                new XElement("var", "Desc_Spa"),
                                new XElement("var", "Desc_Ita"),
                                new XElement("var", "Load_power"),
                                new XElement("var", "Power"),
                                new XElement("var", "Reference"),
                                new XElement("var", "Back"),
                                new XElement("var", "voltage"),
                                new XElement("var", "Qty_Group"),
                                new XElement("var", "imgEAC"),
                                new XElement("var", "imgCTP"),
                                new XElement("var", "imgUL"),
                                new XElement("var", "imgKC"),
                                new XElement("var", "KC_Number"),
                                new XElement("var", "C_Indonesia"),
                                new XElement("var", "Madein_New"),
                                new XElement("var", "Madein_Ch"),
                                new XElement("var", "Madein_Ru"),
                                new XElement("var", "Madein_Old"),
                                new XElement("var", "DescPro_Ru"),
                                new XElement("var", "Path")
                 ),
                  new XElement("LabelVariables", new XAttribute("Name", "Incomplete1"),
                                new XElement("var", "Art_number"),
                                new XElement("var", "Reference"),
                                new XElement("var", "Qty_Group"),
                                new XElement("var", "Qty_InBox")
                            ))
              );
            doc.Save(fileName);
        }

        private bool CheckFileExist(string data)
        {
            var fileInfo  = new FileInfo(data);
            return fileInfo.Exists;
        }

        public void Save()
        {
            _xDocument?.Save(FileName);
        }

        public void Reload()
        {
            LoadSetting(FileName);
        }
        public LabelType GetLabelType(string name)
        {
            XElement label = (from xml2 in _xDocument.Descendants("Label")
                              where xml2.Attribute("Name")?.Value == name                     
                                select xml2).FirstOrDefault();
            if (label != null)
            {
                try
                {
                    var labeltype = new LabelType
                    {
                        LeftOffset = Convert.ToInt32(label.Element("LeftOffset")?.Value),
                        TopOffset = Convert.ToInt32(label.Element("TopOffset")?.Value),
                        Printer = label.Element("Printer")?.Value,
                        TemplateFile = label.Element("Location")?.Value,
                        Title = label.Attribute("title")?.Value,
                        Name = label.Attribute("Name")?.Value
                    };
                    return labeltype;
                }
                catch
                {
                    return new LabelType();
                }
            }
            return  new LabelType();
        }

        public ComPortType GetComPort(string name)
        {
            XElement comport = (from xml2 in _xDocument.Descendants("Comport")
                              where xml2.Attribute("Name")?.Value == name
                              select xml2).FirstOrDefault();
            if (comport != null)
            {
                try
                {
                    Parity par;
                    StopBits stop;
                    Enum.TryParse(comport.Element("Parity")?.Value, true, out par);
                    Enum.TryParse(comport.Element("Stopbits")?.Value, true, out stop);
                    var comportType = new ComPortType
                    {
                        ComName = comport.Element("ComName")?.Value,
                        BaudRate = Convert.ToInt32(comport.Element("BaudRate")?.Value),
                        Parity = par,
                        StopBits = stop,
                        DataBits = Convert.ToInt32(comport.Element("Databits")?.Value),
                        Prefix = comport.Element("Prefix")?.Value,
                        Suffix = comport.Element("Suffix")?.Value,
                        Eof = Convert.ToByte(comport.Element("Eof")?.Value),
                        Name = comport.Attribute("Name")?.Value,
                    };
                    return comportType;
                }
                catch
                {
                    return new ComPortType();
                }
            }

            return new ComPortType();
        }

        public ProcessType GetLastRunning()
        {
            XElement last = (from xml2 in _xDocument.Descendants("Lastrunning")
                              select xml2).FirstOrDefault();
            if (last != null)
            {
                try
                {
                    var lastVars = new ProcessType
                    {
                        Article = last.Element("Article").Value,
                        Reference = last.Element("Reference").Value,
                        IndividualPass =
                            last.Element("IndividualPass").Value == ""
                                ? 0
                                : Convert.ToInt32(last.Element("IndividualPass").Value),
                        IndividualFail =
                            last.Element("IndividualFail").Value == ""
                                ? 0
                                : Convert.ToInt32(last.Element("IndividualFail").Value),
                        Box = last.Element("Box").Value == "" ? 0 : Convert.ToInt32(last.Element("Box").Value),
                    };
                    return lastVars;
                }
                catch
                {
                    // ignored
                }
            }
            return  new ProcessType();
        }
        public void UpdateLabelType(LabelType data)
        {
            XElement label = (from xml2 in _xDocument.Descendants("Label")
                              where xml2.Attribute("Name")?.Value == data.Name
                              select xml2).FirstOrDefault();
            if (label != null)
            {
                label.Element("Location").Value = data.TemplateFile;
                label.Element("LeftOffset").Value = data.LeftOffset.ToString();
                label.Element("TopOffset").Value = data.TopOffset.ToString();
                label.Element("Printer").Value = data.Printer;
            }
        }

        public void UpdateComPort(ComPortType data)
        {
            XElement comport = (from xml2 in _xDocument.Descendants("Comport")
                                where xml2.Attribute("Name")?.Value == data.Name
                                select xml2).FirstOrDefault();
            if (comport != null)
            {
                comport.Element("ComName").Value = data.ComName;
                comport.Element("BaudRate").Value = data.BaudRate.ToString();
                comport.Element("Parity").Value = data.Parity.ToString();
                comport.Element("Stopbits").Value = data.StopBits.ToString();
                comport.Element("Databits").Value = data.DataBits.ToString();
            }
        }
        public string GetPassword()
        {
            XElement password = (from xml2 in _xDocument.Descendants("User")
                                select xml2).FirstOrDefault();
            if (password != null)
            {
                return password.Attribute("Password")?.Value;
            }
            return "";
        }

        public DatabaseConnectionType GetDatabaseConnection()
        {

            XElement db = (from xml2 in _xDocument.Descendants("Database")
                                select xml2).FirstOrDefault();
            var dbs = new DatabaseConnectionType();
            if (db!=null)
            {
                dbs.FileLocation = db.Element("FileLocation").Value;
                dbs.Provider = db.Element("Provider").Value;
                dbs.TableName = db.Element("TableName").Value;
            }
            return dbs;
        }

        public void UpdateDatabaseConnection(DatabaseConnectionType dbs)
        {
            XElement db = (from xml2 in _xDocument.Descendants("Database")
                           select xml2).FirstOrDefault();
            if (db != null)
            {
                db.Element("FileLocation").Value = dbs.FileLocation;
                db.Element("Provider").Value= dbs.Provider ;
                db.Element("TableName").Value= dbs.TableName;
            }
        }
        public void UpdateLastRunning(ProcessType lastData)
        {

            XElement last = (from xml2 in _xDocument.Descendants("Lastrunning")
                select xml2).FirstOrDefault();
            if (last != null)
            {
                last.Element("Reference").Value = lastData.Reference;
                last.Element("Article").Value = lastData.Article;
                last.Element("IndividualPass").Value = lastData.IndividualPass.ToString();
                last.Element("IndividualFail").Value = lastData.IndividualFail.ToString();
                last.Element("Box").Value = lastData.Box.ToString();               
            }
        }

        public List<string> GetLabelVariables(string variableGroupName)
        {
            XElement varlabel = (from xml2 in _xDocument.Descendants("LabelVariables")
                                where xml2.Attribute("Name")?.Value == variableGroupName
                                select xml2).FirstOrDefault();
            var list = new List<string>();

            if (varlabel != null)
            {
                foreach (var el in varlabel.Elements())
                {
                    list.Add(el.Value);
                }
               
            }
            return list;
        }

        public void Dispose()
        {
        }
    }
}
