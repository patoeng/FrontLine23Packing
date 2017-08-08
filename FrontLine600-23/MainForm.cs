using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MetroFramework.Forms;
using Packing.Grouping;

namespace FrontLine600_23
{
    public partial class MainForm : MetroForm
    {
        public MainForm()
        {
            InitializeComponent();
            InitBarcode1();
            InitBarcode2();
            InitWeighing();
            InitIndividualLabel();
            InitGroupLabel();
            InitIncompleteLabel();

            _currentRunning = ReloadLastRunning();
            if (_currentRunning.Article != "")
            {
                LoadActiveReference(_currentRunning.Article, _currentRunning);
                InitPackingGroup(_currentRunning.GroupSize, _currentRunning.Box, _currentRunning.NominalWeight,
                    _currentRunning.IndividualPass, 999999);
            }
            else
            {
                InitPackingGroup(16, 0, 0.500, 0, 999999);
            }
            IncreaseIndividualFail(0);
            IncreaseIndividualPass(0);
            lblPacked.Text = _packingGroup.PackedQuantity.ToString("000");
        }

        private ComPortForm _barcode1;
        private ComPortForm _barcode2;
        private ComPortForm _weighing;
        private CodeSoftLabel _individualLabel;
        private CodeSoftLabel _groupLabel;
        private CodeSoftLabel _incompleteLabel;
        private XSetting _xSetting  = new XSetting();
        private Grouping _packingGroup;

        private Dictionary<string, string> _individualVariables = new Dictionary<string, string>();
        private Dictionary<string, string> _groupVariables = new Dictionary<string, string>();
        private Dictionary<string, string> _incompleteVariables = new Dictionary<string, string>();

        private ProcessType _currentRunning;

        public delegate void ObjectUpdateTextDelegate(object obj, string text);
        public delegate void ObjectUpdateForeColorDelegate(object obj, Color color);
        public delegate void ObjectUpdateVisibleDelegate(object obj, bool data);

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void InitBarcode1()
        {
            var com = _xSetting.GetComPort("Barcode1");
            _barcode1 = new ComPortForm(com) {ParserMethod = ParserMethodBarcode1};
            _barcode1.CompPortDataUpdated += Barcode1OnCompPortDataUpdated;
        }

        private void Barcode1OnCompPortDataUpdated(string data, bool parseResult)
        {
            UpdateTextWithInvoke(tbIndividual,
                  FormMessage(data, parseResult, parseResult ? "Printing ...." : "Not Printing"));
            if (parseResult)
            {
                _individualLabel?.Print();
            }
        
        }

        private bool ParserMethodBarcode1(string data)
        {
            if (data.Length < 24) return false;
            var getArt = data.Substring(0, 12);
            return getArt == _currentRunning?.Article;
        }

        private void InitBarcode2()
        {
            var com = _xSetting.GetComPort("Barcode2");
            _barcode2 = new ComPortForm(com) {ParserMethod = ParserMethodBarcode2};
            _barcode2.CompPortDataUpdated += Barcode2OnCompPortDataUpdated;
        }

        private void Barcode2OnCompPortDataUpdated(string data, bool parseResult)
        {
            UpdateTextWithInvoke(tbGrouping,
                  FormMessage(data, parseResult, ""));
            if (parseResult)
            {
                IncreaseIndividualPass(1);
            }
            else
            {
                IncreaseIndividualFail(1);
            }
        }

        private bool ParserMethodBarcode2(string data)
        {
            if (data.Length < 13) return false;
            var getArt = data.Substring(0, 13);
            return getArt == _currentRunning?.Ean13Code;
        }

        private void InitWeighing()
        {
            var com = _xSetting.GetComPort("Weighing");
            _weighing = new ComPortForm(com) {ParserMethod = ParserMethodWeighing};
            _weighing.CompPortDataUpdated += WeighingOnCompPortDataUpdated;
        }

        private void WeighingOnCompPortDataUpdated(string data, bool parseResult)
        {
            if (parseResult)
            _packingGroup?.UpdateActualWeight(Convert.ToDouble(data));
        }

        private bool ParserMethodWeighing(string data)
        {
            try
            {
                var d = Convert.ToDouble(data);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void InitIndividualLabel()
        {
            var label = _xSetting.GetLabelType("Individual1");
            _individualLabel = new CodeSoftLabel(label,_individualVariables);

        }
        public void InitGroupLabel()
        {
            var label = _xSetting.GetLabelType("Group1");
            _groupLabel = new CodeSoftLabel(label, _groupVariables);
        }
        public void InitIncompleteLabel()
        {
            var label = _xSetting.GetLabelType("Incomplete1");
            _incompleteLabel = new CodeSoftLabel(label, _incompleteVariables);
        }

        private void InitPackingGroup(int groupingSize, int initialBoxQty, double nominalWieght, int initialPackable, int target)
        {
            _packingGroup = new Grouping(groupingSize,initialBoxQty,nominalWieght,initialPackable,target);

            lblUpperLimit.Text = _packingGroup.WeighingUpperLimit.ToString("F3");
            lblLowerLimit.Text = _packingGroup.WeighingLowerLimit.ToString("F3");

            _packingGroup.GroupingWeightNominalValueChanged += PackingGroupOnGroupingWeightNominalValueChanged;
            _packingGroup.GroupingExceptionOccured += PackingGroupOnGroupingExceptionOccured;
            _packingGroup.GroupingPackedQuantityChanged += PackingGroupOnGroupingPackedQuantityChanged;
            _packingGroup.GroupingPackedQuantityReset += PackingGroupOnGroupingPackedQuantityReset;
            _packingGroup.GroupingPackedQuantitySizeAchieved += PackingGroupOnGroupingPackedQuantitySizeAchieved;
            _packingGroup.GroupingPackingCompleted += PackingGroupOnGroupingPackingCompleted;
            _packingGroup.GroupingWeightValueChanged += PackingGroupOnGroupingWeightValueChanged;
            _packingGroup.GroupingWeightValueEnteringRange += PackingGroupOnGroupingWeightValueEnteringRange;
            _packingGroup.GroupingWeightValueLeavingRange += PackingGroupOnGroupingWeightValueLeavingRange;
        }

        private void PackingGroupOnGroupingWeightValueLeavingRange(object sender, double data)
        {
            UpdateLabelForeColorWithInvoke(lblWeighing,Color.Black);
        }

        private void PackingGroupOnGroupingWeightValueEnteringRange(object sender, double data)
        {
            UpdateLabelForeColorWithInvoke(lblWeighing, Color.Green);
        }

        private void PackingGroupOnGroupingWeightValueChanged(object sender, double data)
        {
            UpdateLabelWithInvoke(lblWeighing,data.ToString("F3"));
        }

        private void PackingGroupOnGroupingPackingCompleted(object sender, int quantity, int deltaQuantity)
        {
            //Not Implemented 
        }

        private void PackingGroupOnGroupingPackedQuantitySizeAchieved(object sender, int quantity, int deltaQuantity)
        {
            _groupLabel.Print();
            UpdateLabelVisibleWithInvoke(lblBoxRemove, true);
            using (var t = new XSetting())
            {
                t.UpdateLastRunning(_currentRunning);
                t.Save();
            }
        }

        private void PackingGroupOnGroupingPackedQuantityReset(object sender, int quantity, int deltaQuantity)
        {
            UpdateLabelVisibleWithInvoke(lblBoxRemove, false);
        }

        private void PackingGroupOnGroupingPackedQuantityChanged(object sender, int quantity, int deltaQuantity)
        {
            var d = (Grouping) sender;
            UpdateLabelWithInvoke(lblPacked, d.PackedQuantity.ToString("000"));
            UpdateLabelWithInvoke(lblInbox,d.InBoxQuantity.ToString("000"));
        }

        private void PackingGroupOnGroupingExceptionOccured(object sender, string message)
        {
            UpdateLabelWithInvoke(lblMessage,message);
        }

        private void PackingGroupOnGroupingWeightNominalValueChanged(object sender, double data)
        {
            var d = (Grouping)sender;
            UpdateLabelWithInvoke(lblNominal,data.ToString("F3"));
            UpdateLabelWithInvoke(lblUpperLimit, d.WeighingUpperLimit.ToString("F3"));
            UpdateLabelWithInvoke(lblLowerLimit, d.WeighingLowerLimit.ToString("F3"));
        }

        private ProcessType ReloadLastRunning()
        {
            return _xSetting.GetLastRunning();
        }
       
        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void btnBarcode1_Click(object sender, EventArgs e)
        {
           _barcode1.Show();
           _barcode1.BringToFront();
        }

        private void btnBarcode2_Click(object sender, EventArgs e)
        {
            _barcode2.Show();
            _barcode2.BringToFront();
        }

        private void btnWeighing_Click(object sender, EventArgs e)
        {
            _weighing.Show();
            _weighing.BringToFront();
        }

        private void btnIndividual_Click(object sender, EventArgs e)
        {
            _individualLabel.Show();
            _individualLabel.BringToFront();
        }

        private void metroButton1_Click_1(object sender, EventArgs e)
        {
            _groupLabel.Show();
            _groupLabel.BringToFront();
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            _incompleteLabel.Show();
            _incompleteLabel.BringToFront();
        }

        private void btnPlc_Click(object sender, EventArgs e)
        {
            

        }

        private void metroButton6_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
               var p = new ProcessType();

               var j = LoadActiveReference(textBox4.Text, p);
            if (!j)
            {
                UpdateTextWithInvoke(tbLog, FormMessage($"Reference Not Found : {textBox4.Text} !!!", false, ""));
                return;
            }
                InitPackingGroup(_currentRunning.GroupSize, _currentRunning.Box, _currentRunning.NominalWeight,
                    _currentRunning.IndividualPass, 999999);
                IncreaseIndividualFail(0);
                IncreaseIndividualPass(0);
                lblPacked.Text = _packingGroup.PackedQuantity.ToString("000");
                UpdateTextWithInvoke(tbLog,FormMessage($"Reference Loaded : {_currentRunning.Article} {_currentRunning.Reference}!!!",true,""));


        }

        private bool LoadActiveReference(string article, ProcessType pp)
        {
            using (var xsetting = new XSetting())
            {
                var k = xsetting.GetDatabaseConnection();
                var kk = new Database(k.TableName, k.Provider, k.FileLocation);
                var kkk = kk.LoadByArticle(article);
                if (!kkk) return false;
                _currentRunning = ParseLoadedData(kk.Data, pp);
                UpdateActiveReferenceLabels(_currentRunning);
                _individualVariables = UpdateLabelVars("Individual1", kk.Data);
                _groupVariables = UpdateLabelVars("Group1", kk.Data);
                _incompleteVariables = UpdateLabelVars("Incomplete1", kk.Data);

                _individualLabel.Initiate(_individualVariables);
                _groupLabel.Initiate(_groupVariables);
                _incompleteLabel.Initiate(_incompleteVariables);

                xsetting.UpdateLastRunning(_currentRunning);
                xsetting.Save();
            }
            return true;
        }
        private Dictionary<string,string> UpdateLabelVars(string labelName, Dictionary<string, string> kkData)
        {
            var formLabel = _xSetting.GetLabelVariables(labelName);
            var data  = new Dictionary<string,string>();
            foreach (var key in formLabel)
            {
                try
                {
                    data.Add(key, kkData[key]);
                }
                catch
                {
                    // ignored
                }
            }
            return data;
        }

      
        private ProcessType ParseLoadedData(Dictionary<string, string> kkData, ProcessType process)
        {

            process.Article = kkData["Art_number"];
            process.Reference = kkData["Reference"];
            process.GroupSize = Convert.ToInt32(kkData["Qty_group"]);
            process.NominalWeight = Convert.ToDouble(kkData["upperweight"]);
            
            return process;
        }
        private void UpdateActiveReferenceLabels(ProcessType pp)
        {
            lblReference.Text = pp.Reference;
            lblArticle.Text = pp.Article;
            lblGroupsize.Text = pp.GroupSize.ToString();
            lblEan13.Text = pp.Ean13Code;
            lblNominal.Text = pp.NominalWeight.ToString("F3");
        }

        private void IncreaseIndividualPass(int delta)
        {
            if( _currentRunning.IndividualPass + delta <0) return;
            _currentRunning.IndividualPass += delta;

            UpdateLabelWithInvoke(lblPass, _currentRunning.IndividualPass.ToString("000"));
            _packingGroup.SetPackableQuantity(_currentRunning.IndividualPass);
            using (var t = new XSetting())
            {
                t.UpdateLastRunning(_currentRunning);
                t.Save();
            }
        }

        private void IncreaseIndividualFail(int delta)
        {
            if (_currentRunning.IndividualFail + delta < 0) return;
            _currentRunning.IndividualFail += delta;
            UpdateLabelWithInvoke(lblFail, _currentRunning.IndividualFail.ToString("000"));
            using (var t = new XSetting())
            {
                t.UpdateLastRunning(_currentRunning);
                t.Save();
            }
        }

        private string FormMessage(string data, bool result, string remark)
        {
            var passFail = result ? "PASS" : "FAIL";
            return $"{DateTime.Now:s}: {data} {passFail} {remark} \r\n";
        }
        #region Invokes
        private void UpdateTextWithInvoke(object obj, string data)
        {
            var tb = (TextBox) obj;
            if (tb.InvokeRequired)
            {
                ObjectUpdateTextDelegate d = UpdateTextWithInvoke;
                Invoke(d, tb, data);
            }
            else
            {
                try
                {
                    tb.Text += data;
                }
                catch
                {
                    //  MessageBox.Show(@"Convert Error");
                }
            }
        }
        private void UpdateLabelWithInvoke(object obj, string data)
        {
            var tb = (Label)obj;
            if (tb.InvokeRequired)
            {
                ObjectUpdateTextDelegate d = UpdateLabelWithInvoke;
                Invoke(d, tb, data);
            }
            else
            {
                try
                {
                    tb.Text = data;
                }
                catch
                {
                    //  MessageBox.Show(@"Convert Error");
                }
            }
        }
        private void UpdateLabelForeColorWithInvoke(object obj, Color data)
        {
            var tb = (Label)obj;
            if (tb.InvokeRequired)
            {
                ObjectUpdateForeColorDelegate d = UpdateLabelForeColorWithInvoke;
                Invoke(d, tb, data);
            }
            else
            {
                try
                {
                    tb.ForeColor = data;
                }
                catch
                {
                    //  MessageBox.Show(@"Convert Error");
                }
            }
        }
        private void UpdateLabelVisibleWithInvoke(object obj, bool data)
        {
            var tb = (Label)obj;
            if (tb.InvokeRequired)
            {
                ObjectUpdateVisibleDelegate d = UpdateLabelVisibleWithInvoke;
                Invoke(d, tb, data);
            }
            else
            {
                try
                {
                    tb.Visible = data;
                }
                catch
                {
                    //  MessageBox.Show(@"Convert Error");
                }
            }
        }
        #endregion

        private void btnClearIndividual_Click(object sender, EventArgs e)
        {
            tbIndividual.Clear();
        }

        private void btnClearGrouping_Click(object sender, EventArgs e)
        {
            tbGrouping.Clear();
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            tbLog.Clear();
        }

        private void btnTeach_Click(object sender, EventArgs e)
        {
            int pembagi;
            var d = MessageBox.Show(@"Letakkan beberapa Product ke dalam box di atas timbangan. Kemudian click [OK]",
                @"Teaching Weight", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (d != DialogResult.OK) return;
            using (var adj = new Adjust("Product Dalam Box","OK"))
            {
                adj.ShowDialog();
                var action = adj.ActionToTake;
                var delta = adj.AdjustmentQty;
                if (action != AdjusmentMode.Tambah)return;
                pembagi = delta <= 0 ? 1 : delta;
            }

            var weight = _packingGroup?.WeightTeach(_packingGroup.ActualWeight, pembagi);
            if (weight > 0.005)
            {
                using (var set = new XSetting())
                {
                    try
                    {
                        var k = set.GetDatabaseConnection();
                        var db = new Database(k.TableName, k.Provider, k.FileLocation);
                        db.Update("upperweight", weight.Value.ToString("F3"), "Art_number", _currentRunning.Article);
                        _packingGroup.SetNominalWeight(weight.Value);
                        UpdateTextWithInvoke(tbLog, FormMessage($"Weighing Nominal update {weight} for reference {_currentRunning.Article}", true, ""));
                        MessageBox.Show(@"Successfull");
                    }
                    catch(Exception mess)
                    {
                        UpdateTextWithInvoke(tbLog,FormMessage(mess.StackTrace,false,""));
                        MessageBox.Show(@"Failed");
                    }
                }
            }
        }

        private void metroButton4_Click(object sender, EventArgs e)
        {
            using (var adj = new Adjust("Adjust Pass","TAMBAH","KURANG"))
            {
                adj.ShowDialog();
                var action = adj.ActionToTake;
                var delta = adj.AdjustmentQty;
                switch (action)
                {
                    case AdjusmentMode.None:
                        break;
                    case AdjusmentMode.Kurangi:
                        IncreaseIndividualPass(-delta);
                        break;
                    case AdjusmentMode.Tambah:
                        IncreaseIndividualPass(delta);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
