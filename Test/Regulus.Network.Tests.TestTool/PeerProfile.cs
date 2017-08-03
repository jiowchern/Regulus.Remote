using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Regulus.Network.Tests.TestTool
{
    public partial class PeerProfile : Form
    {
        private readonly string _Ip;
        private readonly Regex _Regex;
        public PeerProfile(string ip)
        {
            _Ip = ip;
            _Regex = new Regex(
                @"\[RUDP\]\sRemoteEndPoint:([\d]+.[\d]+.[\d]+.[\d]+:[\d]+)\sSendBytes:([\d]+)\sReceiveBytes:([\d]+)\sSRTT:([\d]+)\sRTO:([\d]+)\sSendPackages:([\d]+)\sSendLost:([\d]+)\sReceivePackages:([\d]+)\sReceiveInvalidPackages:([\d]+)\sLastRTT:([\d]+)\sSendBlock:([\d]+)\sLastRTO:([\d]+)\sReceiveBlock:([\d]+)\sReceiveNumber:([\d]+)\sSendNumber:([\d]+)");
            InitializeComponent();
        }

        private void PeerProfile_Load(object sender, EventArgs e)
        {
            Regulus.Utility.Log.Instance.RecordEvent += _Record;
        }

        private void _Record(string message)
        {
            var match = _Regex.Match(message);
            if (match.Success )
            {
                var endPoint = match.Groups[1].ToString();
                if (endPoint != _Ip)
                {
                    return;
                }
                var propertys = new string[]
                {
                    match.Groups[1].ToString(),
                    match.Groups[2].ToString(),
                    match.Groups[3].ToString(),
                    match.Groups[4].ToString(),
                    match.Groups[5].ToString(),
                    match.Groups[6].ToString(),
                    match.Groups[7].ToString(),
                    match.Groups[8].ToString(),
                    match.Groups[9].ToString(),
                    match.Groups[10].ToString(),
                    match.Groups[11].ToString(),
                    match.Groups[12].ToString(),
                    match.Groups[13].ToString(),
                    match.Groups[14].ToString(),
                    match.Groups[15].ToString(),
                };
                

                if (this.InvokeRequired)
                {

                    try
                    {
                        this.Invoke(new UpdateRttCallabck(_UpdateRtt), propertys, chart1);
                    }
                    catch (Exception e)
                    {                        
                    }
                    
                }
                else
                {
                    _UpdateRtt(propertys, chart1);
                }
           }
        }

        delegate void UpdateRttCallabck(string[] propertys, Chart controll);
        void _UpdateRtt(string[] propertys, Chart controll)
        {

            var endPoint = propertys[0];
            var sendBytes = propertys[1];
            var receiveBytes = propertys[2];
            var srttTicks = propertys[3];
            var rtoTicks = propertys[4];
            var sendPackages = propertys[5];
            var sendLosts = propertys[6];
            var receivePackages = propertys[7];
            var receiveInvalids = propertys[8];
            var lastRttTicks = propertys[9];
            var sendBlock = propertys[10];
            var lastRTOTicks = propertys[11];
            var receiveBlock = propertys[12];
            var receiveNumber = propertys[13];
            var sendNumber = propertys[14];

            SendBytes.Text = sendBytes;
            ReceiveBytes.Text = receiveBytes;
            SendPackages.Text = sendPackages;
            SendLosts.Text = sendLosts;
            ReceivePackages.Text = receivePackages;
            ReceiveInvalids.Text = receiveInvalids;
            ReceiveBlock.Text = receiveBlock;
            SendBlock.Text = sendBlock;
            ReceiveNumber.Text = receiveNumber;
            SendNumber.Text = sendNumber;

            var srtt = double.Parse(srttTicks.ToString()) / Timestamp.OneSecondTicks;
            var rto = double.Parse(rtoTicks.ToString()) / Timestamp.OneSecondTicks;
            var lastRtt = double.Parse(lastRttTicks.ToString()) / Timestamp.OneSecondTicks;
            var lastRto = double.Parse(lastRTOTicks.ToString()) / Timestamp.OneSecondTicks;


            SRTT.Text = srtt.ToString();
            RTO.Text = rto.ToString();
            LastRTT.Text = lastRtt.ToString();

            var rttSerie = chart1.Series[0];
            rttSerie.Points.Add(srtt);

            var rtoSerie = chart1.Series[1];
            rtoSerie.Points.Add(rto);

            var lastRttSerie = chart1.Series[2];
            lastRttSerie.Points.Add(lastRtt);

            var lastRtoSerie = chart1.Series[3];
            lastRtoSerie.Points.Add(lastRto);

            

            if (lastRtoSerie.Points.Count > 100)
                lastRtoSerie.Points.RemoveAt(0);

            if (lastRttSerie.Points.Count > 100)
                lastRttSerie.Points.RemoveAt(0);

            if (rtoSerie.Points.Count > 100)
                rtoSerie.Points.RemoveAt(0);

            if (rttSerie.Points.Count > 100)
                rttSerie.Points.RemoveAt(0);
            

            controll.ResetAutoValues();
        }

        private void PeerProfile_FormClosing(object sender, FormClosingEventArgs e)
        {
            Regulus.Utility.Log.Instance.RecordEvent -= _Record;
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void ReceiveInvalids_Click(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void ReceivePackages_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void SendPackages_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void RTO_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void SRTT_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void ReceiveBytes_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void SendBytes_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void chart2_Click(object sender, EventArgs e)
        {

        }
    }
}
