using System.Windows.Forms.DataVisualization.Charting;

namespace Regulus.Network.Tests.TestTool
{
    partial class PeerProfile
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label1 = new System.Windows.Forms.Label();
            this.SendBytes = new System.Windows.Forms.Label();
            this.ReceiveBytes = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SRTT = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.RTO = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.SendPackages = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.SendLosts = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.ReceivePackages = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.ReceiveInvalids = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.LastRTT = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.LastRTO = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ReceiveBlock = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SendBlock = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.ReceiveNumber = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.SendNumber = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(0, -3);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "srtt";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Legend = "Legend1";
            series2.Name = "rto";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Legend = "Legend1";
            series3.Name = "last rtt";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series4.Legend = "Legend1";
            series4.Name = "last rto";
            this.chart1.Series.Add(series1);
            this.chart1.Series.Add(series2);
            this.chart1.Series.Add(series3);
            this.chart1.Series.Add(series4);
            this.chart1.Size = new System.Drawing.Size(928, 267);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            this.chart1.Click += new System.EventHandler(this.chart1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("新細明體", 14F);
            this.label1.Location = new System.Drawing.Point(-4, 698);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "Send Bytes";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // SendBytes
            // 
            this.SendBytes.AutoSize = true;
            this.SendBytes.Font = new System.Drawing.Font("新細明體", 14F);
            this.SendBytes.Location = new System.Drawing.Point(105, 698);
            this.SendBytes.Name = "SendBytes";
            this.SendBytes.Size = new System.Drawing.Size(99, 19);
            this.SendBytes.TabIndex = 2;
            this.SendBytes.Text = "1234567890";
            this.SendBytes.Click += new System.EventHandler(this.SendBytes_Click);
            // 
            // ReceiveBytes
            // 
            this.ReceiveBytes.AutoSize = true;
            this.ReceiveBytes.Font = new System.Drawing.Font("新細明體", 14F);
            this.ReceiveBytes.Location = new System.Drawing.Point(105, 717);
            this.ReceiveBytes.Name = "ReceiveBytes";
            this.ReceiveBytes.Size = new System.Drawing.Size(99, 19);
            this.ReceiveBytes.TabIndex = 4;
            this.ReceiveBytes.Text = "1234567890";
            this.ReceiveBytes.Click += new System.EventHandler(this.ReceiveBytes_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("新細明體", 14F);
            this.label4.Location = new System.Drawing.Point(-4, 717);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 19);
            this.label4.TabIndex = 3;
            this.label4.Text = "Receive Bytes";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // SRTT
            // 
            this.SRTT.AutoSize = true;
            this.SRTT.Font = new System.Drawing.Font("新細明體", 14F);
            this.SRTT.Location = new System.Drawing.Point(105, 736);
            this.SRTT.Name = "SRTT";
            this.SRTT.Size = new System.Drawing.Size(99, 19);
            this.SRTT.TabIndex = 6;
            this.SRTT.Text = "1234567890";
            this.SRTT.Click += new System.EventHandler(this.SRTT_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("新細明體", 14F);
            this.label6.Location = new System.Drawing.Point(-4, 736);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 19);
            this.label6.TabIndex = 5;
            this.label6.Text = "SRTT";
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // RTO
            // 
            this.RTO.AutoSize = true;
            this.RTO.Font = new System.Drawing.Font("新細明體", 14F);
            this.RTO.Location = new System.Drawing.Point(105, 755);
            this.RTO.Name = "RTO";
            this.RTO.Size = new System.Drawing.Size(99, 19);
            this.RTO.TabIndex = 8;
            this.RTO.Text = "1234567890";
            this.RTO.Click += new System.EventHandler(this.RTO_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("新細明體", 14F);
            this.label8.Location = new System.Drawing.Point(-4, 755);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(45, 19);
            this.label8.TabIndex = 7;
            this.label8.Text = "RTO";
            this.label8.Click += new System.EventHandler(this.label8_Click);
            // 
            // SendPackages
            // 
            this.SendPackages.AutoSize = true;
            this.SendPackages.Font = new System.Drawing.Font("新細明體", 14F);
            this.SendPackages.Location = new System.Drawing.Point(336, 698);
            this.SendPackages.Name = "SendPackages";
            this.SendPackages.Size = new System.Drawing.Size(99, 19);
            this.SendPackages.TabIndex = 10;
            this.SendPackages.Text = "1234567890";
            this.SendPackages.Click += new System.EventHandler(this.SendPackages_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("新細明體", 14F);
            this.label10.Location = new System.Drawing.Point(210, 698);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(85, 19);
            this.label10.TabIndex = 9;
            this.label10.Text = "Send Pkgs";
            this.label10.Click += new System.EventHandler(this.label10_Click);
            // 
            // SendLosts
            // 
            this.SendLosts.AutoSize = true;
            this.SendLosts.Font = new System.Drawing.Font("新細明體", 14F);
            this.SendLosts.Location = new System.Drawing.Point(336, 717);
            this.SendLosts.Name = "SendLosts";
            this.SendLosts.Size = new System.Drawing.Size(99, 19);
            this.SendLosts.TabIndex = 12;
            this.SendLosts.Text = "1234567890";
            this.SendLosts.Click += new System.EventHandler(this.label11_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("新細明體", 14F);
            this.label12.Location = new System.Drawing.Point(210, 717);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(82, 19);
            this.label12.TabIndex = 11;
            this.label12.Text = "Send Lost";
            this.label12.Click += new System.EventHandler(this.label12_Click);
            // 
            // ReceivePackages
            // 
            this.ReceivePackages.AutoSize = true;
            this.ReceivePackages.Font = new System.Drawing.Font("新細明體", 14F);
            this.ReceivePackages.Location = new System.Drawing.Point(336, 736);
            this.ReceivePackages.Name = "ReceivePackages";
            this.ReceivePackages.Size = new System.Drawing.Size(99, 19);
            this.ReceivePackages.TabIndex = 14;
            this.ReceivePackages.Text = "1234567890";
            this.ReceivePackages.Click += new System.EventHandler(this.ReceivePackages_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("新細明體", 14F);
            this.label14.Location = new System.Drawing.Point(210, 736);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(107, 19);
            this.label14.TabIndex = 13;
            this.label14.Text = "Receive Pkgs";
            this.label14.Click += new System.EventHandler(this.label14_Click);
            // 
            // ReceiveInvalids
            // 
            this.ReceiveInvalids.AutoSize = true;
            this.ReceiveInvalids.Font = new System.Drawing.Font("新細明體", 14F);
            this.ReceiveInvalids.Location = new System.Drawing.Point(336, 755);
            this.ReceiveInvalids.Name = "ReceiveInvalids";
            this.ReceiveInvalids.Size = new System.Drawing.Size(99, 19);
            this.ReceiveInvalids.TabIndex = 16;
            this.ReceiveInvalids.Text = "1234567890";
            this.ReceiveInvalids.Click += new System.EventHandler(this.ReceiveInvalids_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("新細明體", 14F);
            this.label16.Location = new System.Drawing.Point(210, 755);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(130, 19);
            this.label16.TabIndex = 15;
            this.label16.Text = "Receive Invalids";
            this.label16.Click += new System.EventHandler(this.label16_Click);
            // 
            // LastRTT
            // 
            this.LastRTT.AutoSize = true;
            this.LastRTT.Font = new System.Drawing.Font("新細明體", 14F);
            this.LastRTT.Location = new System.Drawing.Point(566, 698);
            this.LastRTT.Name = "LastRTT";
            this.LastRTT.Size = new System.Drawing.Size(99, 19);
            this.LastRTT.TabIndex = 18;
            this.LastRTT.Text = "1234567890";
            this.LastRTT.Click += new System.EventHandler(this.label17_Click);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("新細明體", 14F);
            this.label18.Location = new System.Drawing.Point(450, 698);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(79, 19);
            this.label18.TabIndex = 17;
            this.label18.Text = "Last RTT";
            this.label18.Click += new System.EventHandler(this.label18_Click);
            // 
            // LastRTO
            // 
            this.LastRTO.AutoSize = true;
            this.LastRTO.Font = new System.Drawing.Font("新細明體", 14F);
            this.LastRTO.Location = new System.Drawing.Point(566, 717);
            this.LastRTO.Name = "LastRTO";
            this.LastRTO.Size = new System.Drawing.Size(99, 19);
            this.LastRTO.TabIndex = 20;
            this.LastRTO.Text = "1234567890";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("新細明體", 14F);
            this.label3.Location = new System.Drawing.Point(450, 717);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 19);
            this.label3.TabIndex = 19;
            this.label3.Text = "Last RTO";
            // 
            // ReceiveBlock
            // 
            this.ReceiveBlock.AutoSize = true;
            this.ReceiveBlock.Font = new System.Drawing.Font("新細明體", 14F);
            this.ReceiveBlock.Location = new System.Drawing.Point(566, 736);
            this.ReceiveBlock.Name = "ReceiveBlock";
            this.ReceiveBlock.Size = new System.Drawing.Size(99, 19);
            this.ReceiveBlock.TabIndex = 22;
            this.ReceiveBlock.Text = "1234567890";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("新細明體", 14F);
            this.label5.Location = new System.Drawing.Point(450, 736);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 19);
            this.label5.TabIndex = 21;
            this.label5.Text = "ReceiveBlock";
            // 
            // SendBlock
            // 
            this.SendBlock.AutoSize = true;
            this.SendBlock.Font = new System.Drawing.Font("新細明體", 14F);
            this.SendBlock.Location = new System.Drawing.Point(566, 755);
            this.SendBlock.Name = "SendBlock";
            this.SendBlock.Size = new System.Drawing.Size(99, 19);
            this.SendBlock.TabIndex = 24;
            this.SendBlock.Text = "1234567890";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("新細明體", 14F);
            this.label9.Location = new System.Drawing.Point(450, 755);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(88, 19);
            this.label9.TabIndex = 23;
            this.label9.Text = "SendBlock";
            // 
            // ReceiveNumber
            // 
            this.ReceiveNumber.AutoSize = true;
            this.ReceiveNumber.Font = new System.Drawing.Font("新細明體", 14F);
            this.ReceiveNumber.Location = new System.Drawing.Point(821, 698);
            this.ReceiveNumber.Name = "ReceiveNumber";
            this.ReceiveNumber.Size = new System.Drawing.Size(99, 19);
            this.ReceiveNumber.TabIndex = 26;
            this.ReceiveNumber.Text = "1234567890";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("新細明體", 14F);
            this.label7.Location = new System.Drawing.Point(689, 698);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(126, 19);
            this.label7.TabIndex = 25;
            this.label7.Text = "ReceiveNumber";
            // 
            // SendNumber
            // 
            this.SendNumber.AutoSize = true;
            this.SendNumber.Font = new System.Drawing.Font("新細明體", 14F);
            this.SendNumber.Location = new System.Drawing.Point(821, 717);
            this.SendNumber.Name = "SendNumber";
            this.SendNumber.Size = new System.Drawing.Size(99, 19);
            this.SendNumber.TabIndex = 28;
            this.SendNumber.Text = "1234567890";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("新細明體", 14F);
            this.label13.Location = new System.Drawing.Point(689, 717);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(104, 19);
            this.label13.TabIndex = 27;
            this.label13.Text = "SendNumber";
            // 
            // PeerProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1113, 774);
            this.Controls.Add(this.SendNumber);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.ReceiveNumber);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.SendBlock);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.ReceiveBlock);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.LastRTO);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.LastRTT);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.ReceiveInvalids);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.ReceivePackages);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.SendLosts);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.SendPackages);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.RTO);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.SRTT);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.ReceiveBytes);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.SendBytes);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chart1);
            this.Name = "PeerProfile";
            this.Text = "PeerProfile";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PeerProfile_FormClosing);
            this.Load += new System.EventHandler(this.PeerProfile_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label SendBytes;
        private System.Windows.Forms.Label ReceiveBytes;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label SRTT;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label RTO;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label SendPackages;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label SendLosts;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label ReceivePackages;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label ReceiveInvalids;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label LastRTT;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label LastRTO;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label ReceiveBlock;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label SendBlock;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label ReceiveNumber;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label SendNumber;
        private System.Windows.Forms.Label label13;
    }
}