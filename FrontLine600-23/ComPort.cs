using System;
using System.IO.Ports;
using System.Timers;

namespace FrontLine600_23
{
    public delegate void ComPortString(object sender, string data);
    public class ComPort
    {
        public event ComPortString ComPortSuccessfulRead;
        private readonly SerialPort _serialPort = new SerialPort();
        private Timer _timerSimulation;
        public ComPort(ComPortType comPortType)
        {
            AssignConfig(comPortType);
        }
       
        private char _eof;
        private string _suffix, _prefix;
        private string _tempData;
        public void AssignConfig(ComPortType comPortType)
        { 
            _serialPort.BaudRate = comPortType.BaudRate;
            _serialPort.Parity = comPortType.Parity;
            _serialPort.StopBits = comPortType.StopBits;
            _serialPort.DataBits = comPortType.DataBits;
            _serialPort.PortName = comPortType.ComName;
            _prefix = comPortType.Prefix;
            _suffix = comPortType.Suffix;

            _eof = Convert.ToChar(comPortType.Eof);
            try
            {
                _serialPort.DataReceived -= SerialPortOnDataReceived;
            }
            finally
            {
                _serialPort.DataReceived += SerialPortOnDataReceived;
            }
        }

        private string _emulatorData;

        public void UpdateEmulatorData(string data)
        {
            _emulatorData = data;
            _timerSimulation?.Start();
        }
        public void StartEmulator()
        {
            _timerSimulation = new Timer {Interval = 300};
            _timerSimulation.Elapsed += TimerSimulationOnElapsed;
            
        }

        private void TimerSimulationOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _timerSimulation.Stop();
            if (_eof != '\0')
            {
                if (_emulatorData.Contains(_eof.ToString()))
                {
                    var parsed = Parse(_emulatorData);
                    ComPortSuccessfulRead?.Invoke(this, parsed);
                    
                }
            }
            else
            {
                var parsed = Parse(_emulatorData);
                ComPortSuccessfulRead?.Invoke(this, parsed);
            }
        }

        private string Parse(string data)
        {
            var temp = data.Replace(" ", "");
            temp = temp.Trim('\r', '\n');
            if (_eof != '\0')
            {
                temp = temp.Trim(_eof);
            }
            temp = _suffix==""? temp: temp.Replace(_suffix, "");
            temp = _prefix==""?temp: temp.Replace(_prefix, "");
            return temp;
        }
        private void SerialPortOnDataReceived(object sender, SerialDataReceivedEventArgs serialDataReceivedEventArgs)
        {
            _tempData += _serialPort.ReadExisting();
            if (_eof != '\0')
            {
                if (_tempData.Contains(_eof.ToString()))
                {
                    var parsed = Parse(_tempData);
                    ComPortSuccessfulRead?.Invoke(this, parsed);
                    _tempData = "";
                }
            }
            else
            {
                var parsed = Parse(_tempData);
                ComPortSuccessfulRead?.Invoke(this, parsed);
                _tempData = "";
            }
        }

        public bool Open()
        {
            try
            {
                _serialPort.Open();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool Close()
        {
            try
            {
                _serialPort.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool IsOpen => _serialPort.IsOpen;
    }
}
