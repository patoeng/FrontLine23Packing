using System;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace FrontLine600_23
{
    public enum AdjusmentMode
    {
        None,
        Kurangi,
        Tambah
    }
    public sealed partial class Adjust : MetroForm
    {
        public Adjust(string judul, string button2=null, string button1=null)
        {
            InitializeComponent();
            Text = judul;
            if (!string.IsNullOrEmpty(button1))
            {
                Button2.Text = button1;
                Button2.Visible = true;
            }
            if (!string.IsNullOrEmpty(button2))
            {
                Button1.Text = button2;
                Button1.Visible = true;
            }
        }
        public AdjusmentMode ActionToTake { get; protected set; }
        public int AdjustmentQty { get; protected set; }
        private void Adjust_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            if (e.KeyValue >= 48 && e.KeyValue <= 57)
            {
                txtAdjust.Text += Convert.ToChar(e.KeyValue);
                txtAdjust.Text = txtAdjust.Text.TrimStart('0');
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtAdjust.Text = @"0";
        }

        private void btnKurangi_Click(object sender, EventArgs e)
        {
            AdjustmentQty = ConvertToInt(txtAdjust.Text);
            ActionToTake = AdjusmentMode.Kurangi;
            Close();
        }

        private int ConvertToInt(string data)
        {
            try
            {
                var j = Convert.ToInt32(data);
                return j;
            }
            catch
            {
                return 0;
            }
        }

        private void btnTambah_Click(object sender, EventArgs e)
        {
            AdjustmentQty = ConvertToInt(txtAdjust.Text);
            ActionToTake = AdjusmentMode.Tambah;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ActionToTake = AdjusmentMode.None;
            Close();
        }

        private void Adjust_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void Adjust_Load(object sender, EventArgs e)
        {

        }

        private void btnTmbl0_Click(object sender, EventArgs e)
        {
            Button btn = (Button) sender;
            switch (btn.Name)
            {
                case "btnTmbl0": SendKeys.Send("0");
                    break;
                case "btnTmbl1":
                    SendKeys.Send("1");
                    break;
                case "btnTmbl2":
                    SendKeys.Send("2");
                    break;
                case "btnTmbl3":
                    SendKeys.Send("3");
                    break;
                case "btnTmbl4":
                    SendKeys.Send("4");
                    break;
                case "btnTmbl5":
                    SendKeys.Send("5");
                    break;
                case "btnTmbl6":
                    SendKeys.Send("6");
                    break;
                case "btnTmbl7":
                    SendKeys.Send("7");
                    break;
                case "btnTmbl8":
                    SendKeys.Send("8");
                    break;
                case "btnTmbl9":
                    SendKeys.Send("9");
                    break;
            }
        }
    }
}
