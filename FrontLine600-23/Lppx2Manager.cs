using System;
using System.Windows.Forms;

// For the MessageBox class

namespace FrontLine600_23
{
	/// <summary>
	/// Wrapper class to manage the connection with CODESOFT ActiveX interface
	/// </summary>
	public class Lppx2Manager
	{
		private LabelManager2.Application _mCsApp;
		private bool _bDeleteCsApp = false;
		private readonly bool bUnableToLoad = false;
		private const string Lppx2ProgId = "Lppx2.Application";
		
		public Lppx2Manager()
		{
			
		}

		~Lppx2Manager()
		{
		}

		private void ConnectToLppx2()
		{
			if(_mCsApp != null)
				return;

			Object csObject;

			try
			{
				csObject = System.Runtime.InteropServices.Marshal.GetActiveObject(Lppx2ProgId);
			}
			catch
			{
				//No CODESOFT object Running !
				csObject = null;
			}

			try
			{
				if(csObject == null)
				{
                    _mCsApp = new LabelManager2.Application();
					_bDeleteCsApp = true;
				}
				else
				{
					_mCsApp = (LabelManager2.Application)csObject;
				}
				//m_CsApp.Visible = true;
			}
			catch(Exception e)
			{
				string szerror = e.Message.ToString();
				MessageBox.Show(szerror);
			}
				
			if(bUnableToLoad == false)
			{
				if( _mCsApp.IsEval )
				{
					MessageBox.Show(null,
						@"DemoMode, Check Dongle License",
						@"Quick Print",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning);
				}
			}
			else
			{
				MessageBox.Show(null,
					@"Unable to load CODESOFT",
					@"Quick Print",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}

		public void QuitLppx2()
		{
			if(_mCsApp != null)
			{
				if(_bDeleteCsApp)
				{
					_mCsApp.Documents.CloseAll(false);
					_mCsApp.Quit();

                    _bDeleteCsApp = false;
                }

                System.Runtime.InteropServices.Marshal.ReleaseComObject(_mCsApp);
                _mCsApp = null;
            }
		}

		public LabelManager2.Application GetApplication()
		{
			ConnectToLppx2();
			return _mCsApp;
		}

		public LabelManager2.Document GetActiveDocument()
		{
			ConnectToLppx2();
			return _mCsApp.ActiveDocument;
		}

		public void SwitchPrinter(string PrinterName)
		{
			ConnectToLppx2();

			LabelManager2.Document ActiveDoc = GetActiveDocument();
			if(ActiveDoc != null)
			{
				LabelManager2.PrinterSystem PrnSystem = _mCsApp.PrinterSystem();
				LabelManager2.Strings PrintersName = PrnSystem.Printers(LabelManager2.enumKindOfPrinters.lppxAllPrinters);
				
				string CurrentPrinter = ActiveDoc.Printer.Name;
				if(CurrentPrinter != PrinterName)
				{
					short NbPrinters = PrintersName.Count;
					string FullPrinterName = "";
					for(short i=1;i<=NbPrinters;i++)
					{
						FullPrinterName = PrintersName.Item(i);
						int pos = FullPrinterName.LastIndexOf(',');
						if(pos != -1)
						{
							string CurrentPrinterName = FullPrinterName.Substring(0,pos);
							if(CurrentPrinterName == PrinterName)
							{
								bool bDirectAccess = false;
								string PortName = FullPrinterName.Substring(pos+1);
								if(PortName.StartsWith("->"))
								{
									PortName = PortName.Substring(2);
									bDirectAccess = true;
								}
								ActiveDoc.Printer.SwitchTo(PrinterName, PortName, bDirectAccess);
							}
						}
					}
					System.Runtime.InteropServices.Marshal.ReleaseComObject(PrintersName);
					System.Runtime.InteropServices.Marshal.ReleaseComObject(PrnSystem);
					System.Runtime.InteropServices.Marshal.ReleaseComObject(ActiveDoc);
				}
			}
		}
	}
}
