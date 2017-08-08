namespace FrontLine600_23
{
    public class ProcessType
    {
        public string Reference { get; set; }
        public string Article { get; set; }
        public int IndividualPass { get; set; }
        public int IndividualFail { get; set; }
        public int Box { get; set; }
        public double NominalWeight { get; set; }
        public int GroupSize{ get; set; }
        public string Ean13Code => Ean13.CalculateChecksumDigit(Article);
    }
}
