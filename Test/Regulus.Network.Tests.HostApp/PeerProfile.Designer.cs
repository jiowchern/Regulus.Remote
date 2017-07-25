using System.Windows.Forms.DataVisualization.Charting;

namespace Regulus.Network.Tests.HostApp
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
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
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // chart1
            // 
            chartArea2.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart1.Legends.Add(legend2);
            this.chart1.Location = new System.Drawing.Point(-1, -3);
            this.chart1.Name = "chart1";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series4.Legend = "Legend1";
            series4.Name = "srtt";
            series5.ChartArea = "ChartArea1";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series5.Legend = "Legend1";
            series5.Name = "rto";
            series6.ChartArea = "ChartArea1";
            series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series6.Legend = "Legend1";
            series6.Name = "last rtt";
            this.chart1.Series.Add(series4);
            this.chart1.Series.Add(series5);
            this.chart1.Series.Add(series6);
            this.chart1.Size = new System.Drawing.Size(928, 267);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("新細明體", 14F);
            this.label1.Location = new System.Drawing.Point(-5, 267);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "Send Bytes";
            // 
            // SendBytes
            // 
            this.SendBytes.AutoSize = true;
            this.SendBytes.Font = new System.Drawing.Font("新細明體", 14F);
            this.SendBytes.Location = new System.Drawing.Point(104, 267);
            this.SendBytes.Name = "SendBytes";
            this.SendBytes.Size = new System.Drawing.Size(99, 19);
            this.SendBytes.TabIndex = 2;
            this.SendBytes.Text = "1234567890";
            // 
            // ReceiveBytes
            // 
            this.ReceiveBytes.AutoSize = true;
            this.ReceiveBytes.Font = new System.Drawing.Font("新細明體", 14F);
            this.ReceiveBytes.Location = new System.Drawing.Point(104, 286);
            this.ReceiveBytes.Name = "ReceiveBytes";
            this.ReceiveBytes.Size = new System.Drawing.Size(99, 19);
            this.ReceiveBytes.TabIndex = 4;
            this.ReceiveBytes.Text = "1234567890";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("新細明體", 14F);
            this.label4.Location = new System.Drawing.Point(-5, 286);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 19);
            this.label4.TabIndex = 3;
            this.label4.Text = "Receive Bytes";
            // 
            // SRTT
            // 
            this.SRTT.AutoSize = true;
            this.SRTT.Font = new System.Drawing.Font("新細明體", 14F);
            this.SRTT.Location = new System.Drawing.Point(104, 305);
            this.SRTT.Name = "SRTT";
            this.SRTT.Size = new System.Drawing.Size(99, 19);
            this.SRTT.TabIndex = 6;
            this.SRTT.Text = "1234567890";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("新細明體", 14F);
            this.label6.Location = new System.Drawing.Point(-5, 305);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 19);
            this.label6.TabIndex = 5;
            this.label6.Text = "SRTT";
            // 
            // RTO
            // 
            this.RTO.AutoSize = true;
            this.RTO.Font = new System.Drawing.Font("新細明體", 14F);
            this.RTO.Location = new System.Drawing.Point(104, 324);
            this.RTO.Name = "RTO";
            this.RTO.Size = new System.Drawing.Size(99, 19);
            this.RTO.TabIndex = 8;
            this.RTO.Text = "1234567890";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("新細明體", 14F);
            this.label8.Location = new System.Drawing.Point(-5, 324);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(45, 19);
            this.label8.TabIndex = 7;
            this.label8.Text = "RTO";
            // 
            // SendPackages
            // 
            this.SendPackages.AutoSize = true;
            this.SendPackages.Font = new System.Drawing.Font("新細明體", 14F);
            this.SendPackages.Location = new System.Drawing.Point(335, 267);
            this.SendPackages.Name = "SendPackages";
            this.SendPackages.Size = new System.Drawing.Size(99, 19);
            this.SendPackages.TabIndex = 10;
            this.SendPackages.Text = "1234567890";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("新細明體", 14F);
            this.label10.Location = new System.Drawing.Point(209, 267);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(85, 19);
            this.label10.TabIndex = 9;
            this.label10.Text = "Send Pkgs";
            // 
            // SendLosts
            // 
            this.SendLosts.AutoSize = true;
            this.SendLosts.Font = new System.Drawing.Font("新細明體", 14F);
            this.SendLosts.Location = new System.Drawing.Point(335, 286);
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
            this.label12.Location = new System.Drawing.Point(209, 286);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(82, 19);
            this.label12.TabIndex = 11;
            this.label12.Text = "Send Lost";
            // 
            // ReceivePackages
            // 
            this.ReceivePackages.AutoSize = true;
            this.ReceivePackages.Font = new System.Drawing.Font("新細明體", 14F);
            this.ReceivePackages.Location = new System.Drawing.Point(335, 305);
            this.ReceivePackages.Name = "ReceivePackages";
            this.ReceivePackages.Size = new System.Drawing.Size(99, 19);
            this.ReceivePackages.TabIndex = 14;
            this.ReceivePackages.Text = "1234567890";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("新細明體", 14F);
            this.label14.Location = new System.Drawing.Point(209, 305);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(107, 19);
            this.label14.TabIndex = 13;
            this.label14.Text = "Receive Pkgs";
            // 
            // ReceiveInvalids
            // 
            this.ReceiveInvalids.AutoSize = true;
            this.ReceiveInvalids.Font = new System.Drawing.Font("新細明體", 14F);
            this.ReceiveInvalids.Location = new System.Drawing.Point(335, 324);
            this.ReceiveInvalids.Name = "ReceiveInvalids";
            this.ReceiveInvalids.Size = new System.Drawing.Size(99, 19);
            this.ReceiveInvalids.TabIndex = 16;
            this.ReceiveInvalids.Text = "1234567890";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("新細明體", 14F);
            this.label16.Location = new System.Drawing.Point(209, 324);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(130, 19);
            this.label16.TabIndex = 15;
            this.label16.Text = "Receive Invalids";
            // 
            // LastRTT
            // 
            this.LastRTT.AutoSize = true;
            this.LastRTT.Font = new System.Drawing.Font("新細明體", 14F);
            this.LastRTT.Location = new System.Drawing.Point(534, 267);
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
            this.label18.Location = new System.Drawing.Point(449, 267);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(79, 19);
            this.label18.TabIndex = 17;
            this.label18.Text = "Last RTT";
            // 
            // PeerProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(928, 368);
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
    }
}