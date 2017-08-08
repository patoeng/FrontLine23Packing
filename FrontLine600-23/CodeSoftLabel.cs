using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using MetroFramework.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;


namespace FrontLine600_23
{
    public partial class CodeSoftLabel : MetroForm
    {
        public Lppx2Manager MLppx2Manager;
        public LabelManager2.Application CsApp;

        private Image.GetThumbnailImageAbort _myCallback;
        private LabelManager2.Document _loadedDocument;
        public bool NoDocOpened;
        Dictionary<string, string> _variables;
        public Image RealSizeImage;
        private readonly LabelType _labelType;
        private readonly XSetting _setting = new XSetting();

        public CodeSoftLabel(LabelType labelType, Dictionary<string,string> variables )
        {
            InitializeComponent();
            try
            {
                _labelType = labelType;
                Initiate(variables);
                lblTitle.Text = labelType.Title;
            }
            catch
            {
                // ignored
            }
        }

        public string VariablesToString(LabelManager2.IDocument document)
        {
            var temp = "";
            var cnt = document.Variables.FormVariables.Count;
            for (int i = 1; i <= cnt; i++)
            {   temp += document.Variables.FormVariables.Item(i).Name + " ==> ";
                temp += document.Variables.FormVariables.Item(i).Value + "\r\n";
            }
            return temp;
        }
        public void Initiate(Dictionary<string, string> vars)
        {
             Init();
            docPreview.Image = new Bitmap(1,1);
            _variables = vars;
            var loadLabel = LoadLabel(vars);
            if (loadLabel)
            {
                if (CreateRealSizePreview())
                {
                    if (!NoDocOpened && Visible && _loadedDocument!=null)
                    {
                        docPreview.Image = ResizeIfNeeded(RealSizeImage, docPreview.Width, docPreview.Height);
                    }
                }
                tbVariables.Text = VariablesToString(_loadedDocument);
            }
            cbbPrinter.Items.Clear();
            for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
            {
                var pkInstalledPrinters = PrinterSettings.InstalledPrinters[i];
                cbbPrinter.Items.Add(pkInstalledPrinters);
            }
        }
        private void Init()
        {
            try
            {
                MLppx2Manager = new Lppx2Manager();
                CsApp = MLppx2Manager.GetApplication();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            _myCallback = ThumbnailCallback;
        }

        private bool ThumbnailCallback()
        {
            return false;
        }

        public void AssignVariables ( Dictionary<string, string> data )
        {
            foreach (var key in data.Keys)
            {
                _loadedDocument.Variables.Item(key).Value = data[key];
            }
        }
        private bool LoadLabel(Dictionary<string, string> variables)
        {
            try
            {
                _loadedDocument?.Close();
                _loadedDocument = CsApp.Documents.Open(_labelType.TemplateFile);
                _loadedDocument.ViewMode = LabelManager2.enumViewMode.lppxViewModeValue;               
                AssignVariables(variables);
                if (CsApp.Documents.Count > 0)
                {
                    NoDocOpened = false;
                    return true;
                }
               
            }
            catch (Exception ex)
            {
                // ignored
            }
            return false;
        }

       
        private bool CreateRealSizePreview()
        {        
            if (_loadedDocument != null)
            {
                DisposeImages();
                try
                {
                    object obj = _loadedDocument.GetPreview(true, true, 300);
                    if (obj is Array)
                    {
                        byte[] data = (byte[])obj;
                        System.IO.MemoryStream pStream = new System.IO.MemoryStream(data);
                        RealSizeImage = new Bitmap(pStream);                    
                        return true;
                    }                   
                }
                catch (Exception ex)
                {
                    return false;
                }              
            }
            return false;
        }
        private void DisposeImages()
        {
            RealSizeImage?.Dispose();
        }
        public void Print()
        {
            
            try
            {
                if (NoDocOpened)
                {
                    MessageBox.Show(@"A document must be opened to print !");                   
                    return;
                }

                try
                {
                    _loadedDocument.Printer.SwitchTo(_labelType.Printer);
                    _loadedDocument.HorzPrintOffset = _labelType.LeftOffset;
                    _loadedDocument.VertPrintOffset = _labelType.TopOffset;
                    _loadedDocument.PrintDocument(1);

                }
                catch (Exception error)
                {
                    throw error;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Label " + ex.Message);

            }
        }
        public Image ResizeIfNeeded(Image originalImage, int width, int height)
        {
            if ((originalImage.Width > width) || (originalImage.Height > height))
            {
                if ((originalImage.Width / width) > (originalImage.Height / height))
                {
                    double a = ((double) width) / (((double) originalImage.Width));
                    var resultImage2 = originalImage.GetThumbnailImage((int)((double)originalImage.Width * a),
                        (int) ((double) originalImage.Height * a),
                        _myCallback, IntPtr.Zero);
                    return resultImage2;
                }
                else
                {
                    double a = ((double) height) / (((double) originalImage.Height));
                    var resultImage2 = originalImage.GetThumbnailImage((int) ((double) originalImage.Width * a),
                       (int)((double)originalImage.Height*a), _myCallback, IntPtr.Zero);
                    return resultImage2;
                }
            }
            return originalImage;
            
        }
        public Image ResizeFitBox(Image originalImage, int width, int height)
        {

            if ((originalImage.Width < width) || (originalImage.Height < height))
            {
                if (( width/ originalImage.Width) > ( height/ originalImage.Height))
                {
                    double a = ((double)width) / (((double)originalImage.Width));
                    var resultImage2 = originalImage.GetThumbnailImage((int)((double)originalImage.Width * a),
                        (int)((double)originalImage.Height * a),
                        _myCallback, IntPtr.Zero);
                    return resultImage2;
                }
                else
                {
                    double a = ((double)height) / (((double)originalImage.Height));
                    var resultImage2 = originalImage.GetThumbnailImage((int)((double)originalImage.Width * a),
                       (int)((double)originalImage.Height * a), _myCallback, IntPtr.Zero);
                    return resultImage2;
                }
            }
            return originalImage;

        }
        private void btnLocate_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                InitialDirectory = _labelType.TemplateFile             
            };
            dialog.Filter = @"Codesoft Label|*.lab";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textLocation.Text = dialog.FileName;
            }
        }

        private void CodeSoftLabel_Load(object sender, EventArgs e)
        {
            textLeftOffset.Text = _labelType.LeftOffset.ToString();
            textTopOffset.Text = _labelType.TopOffset.ToString();
            textLocation.Text = _labelType.TemplateFile;

            cbbPrinter.Text = _labelType.Printer;
            if (!NoDocOpened && Visible && _loadedDocument!=null)
            {
                docPreview.Image = ResizeIfNeeded(RealSizeImage, docPreview.Width, docPreview.Height);
            }

        }


        private void btnSavePrinter_Click(object sender, EventArgs e)
        {
            try
            {
                var left = Convert.ToInt32(textLeftOffset.Text);
                var top = Convert.ToInt32(textTopOffset.Text);

                _labelType.LeftOffset = left;
                _labelType.TopOffset = top;
            }
            catch
            {
                textTopOffset.Text = _labelType.TopOffset.ToString();
                textLeftOffset.Text = _labelType.ToString();
            }                
                    _labelType.TemplateFile = textLocation.Text;
                    _labelType.Printer = cbbPrinter.Text;
                    _setting.Reload();
                    _setting.UpdateLabelType(_labelType);
                    _setting.Save();                  
         }

        private void btnShow_Click(object sender, EventArgs e)
        {
            if (!NoDocOpened && Visible)
            {
                Initiate(_variables);
            }
        }

        private void btnManualPrint_Click(object sender, EventArgs e)
        {
            if (!NoDocOpened)
            {
                Print();
            }
        }

        private void CodeSoftLabel_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
