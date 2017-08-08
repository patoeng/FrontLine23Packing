using System;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace FrontLine600_23
{
    public delegate bool ParserMethod(string data);

    public delegate void ComPortData(string data, bool parseResult);

    public partial class ComPortForm : MetroForm
    {
        public event ComPortData CompPortDataUpdated;
        public ComPort ComPort;
        public ParserMethod ParserMethod;
        private ComPortType _comPortType;
        
        public ComPortForm(ComPortType comPortType)
        {
          
            InitializeComponent();
            InitializeCom(comPortType);
            IterateComPorts();
        }

        public void InitializeCom(ComPortType comPortType)
        {

            try
            {
                ComPort.ComPortSuccessfulRead -= ComPortOnComPortSuccessfulRead;
            }
            catch
            {
                // ignored
            }
            finally
            {
                ComPort = new ComPort(comPortType);
                _comPortType = comPortType;
                ComPort.ComPortSuccessfulRead += ComPortOnComPortSuccessfulRead;
                ComPort.Open();
                BtnOpenCloseLabel();
            }
        }
        private void ComPortOnComPortSuccessfulRead(object sender, string data)
        {
            var j = ParserMethod != null ? ParserMethod(data) : true;
            UpdateWithInvoke(data, j);
            CompPortDataUpdated?.Invoke(data,j);
        }

        private void UpdateWithInvoke(string data, bool parseResult)
        {
            if (txtRead.InvokeRequired)
            {
                ComPortData d = UpdateWithInvoke;
                Invoke(d, data,parseResult);
            }
            else
            {
                try
                {
                    txtRead.Text = data;
                    btnParse.BackColor = parseResult ? Color.Green : Color.Red;
                }
                catch
                {
                    //  MessageBox.Show(@"Convert Error");
                }
            }
        }
        private void ComPortForm_Load(object sender, EventArgs e)
        {
            Text = _comPortType.Name;
        }

        private void IterateComPorts()
        {
            cboCom.Items.Clear();
            foreach (var data in SerialPort.GetPortNames())
            {
                cboCom.Items.Add(data);
            }
            cboCom.Text = _comPortType.ComName;

            cboBaudRate.Items.Clear();
            cboBaudRate.Items.Add(300);
            cboBaudRate.Items.Add(600);
            cboBaudRate.Items.Add(1200);
            cboBaudRate.Items.Add(2400);
            cboBaudRate.Items.Add(9600);
            cboBaudRate.Items.Add(14400);
            cboBaudRate.Items.Add(19200);
            cboBaudRate.Items.Add(38400);
            cboBaudRate.Items.Add(57600);
            cboBaudRate.Items.Add(115200);

            cboBaudRate.Text = _comPortType.BaudRate.ToString();

            cboDataBits.Items.Clear();
            cboDataBits.Items.Add(7);
            cboDataBits.Items.Add(8);
            cboDataBits.Text = _comPortType.DataBits.ToString();

            cboStopBits.Items.Clear();
            cboStopBits.Items.Add("One");
            cboStopBits.Items.Add("OnePointFive");
            cboStopBits.Items.Add("Two");
            cboStopBits.Text = _comPortType.StopBits.ToString();

            cboParity.Items.Clear();
            cboParity.Items.Add("None");
            cboParity.Items.Add("Even");
            cboParity.Items.Add("Mark");
            cboParity.Items.Add("Odd");
            cboParity.Items.Add("Space");
            cboParity.Text = _comPortType.Parity.ToString();


        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtRead.Clear();
        }

        private void btnOpenClose_Click(object sender, EventArgs e)
        {
            if (btnOpenClose.Text.Contains("Open"))
            {
                ComPort.Open();
            }
            else
            {
                ComPort.Close();
            }
            BtnOpenCloseLabel();
        }

        private void BtnOpenCloseLabel()
        {
            btnOpenClose.Text = ComPort.IsOpen ? "Close Port" : "Open Port";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Parity par;
                StopBits stop;
                Enum.TryParse(cboParity.Text, true, out par);
                Enum.TryParse(cboStopBits.Text, true, out stop);
                var baud = Convert.ToInt32(cboBaudRate.Text);
                var databits = Convert.ToInt32(cboDataBits.Text);
                var name = cboCom.Text;
                var tempComPortType = new ComPortType
                {
                    Parity = par,
                    StopBits = stop,
                    BaudRate = baud,
                    ComName = name == "" ? "COM1" : name,
                    DataBits = databits
                };

                var set = new XSetting();
                set.UpdateComPort(tempComPortType);
                _comPortType = tempComPortType;
                set.Save();
                ComPort.Close();
                InitializeCom(_comPortType);
                ComPort.Open();
                MessageBox.Show(@"Successful");
            }
            catch
            {
                MessageBox.Show(@"Failed");
            }
        }

        private void ComPortForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ComPort.StartEmulator();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ComPort.UpdateEmulatorData(textBox1.Text +"\r\n");
        }
    }
}
