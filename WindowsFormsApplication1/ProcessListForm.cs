using FlowCytometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class ProcessListForm : Form
    {
        private string fileName;
        private string gatePath;
        private string culture;
            
        public ProcessListForm(string fileName, string gatePath, string channelCuture)
        {
            this.fileName = fileName;
            this.gatePath = gatePath;
            this.culture = channelCuture;

            InitializeComponent();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
        }

        private void ProcessListForm_Load(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy != true)
            {
                // Start the asynchronous operation.
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(gatePath) || string.IsNullOrEmpty(culture))
            {
                MessageBox.Show("Incorrect Parameters!");
                return;
            }
            FCMeasurement.FG_folder_analysis(fileName, gatePath, culture, false, sender as BackgroundWorker, e);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int percentage = e.ProgressPercentage;
            ProcessState processState = (ProcessState)e.UserState;

            progressBar.Value = percentage;
            txtProgress.Text = "Processing " + processState.nCurr + "/" + processState.nTotalCnt;

            txtMessage.AppendText("\n" + processState.strMsg);
            txtMessage.SelectionStart = txtMessage.Text.Length;
            txtMessage.ScrollToCaret();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string strMsg = "";
            if (e.Cancelled)
            {
                strMsg = "Process List was canceled";
            }
            else if (e.Error != null)
            {
                strMsg = "There was an error while processing list. \n Error Message : " + e.Error.Message;
            }
            else
            {
                strMsg = "Process List was completed";
                progressBar.Value = 100;
                txtProgress.Text = "Completed";
            }
            MessageBox.Show(strMsg);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.WorkerSupportsCancellation == true)
            {
                if (backgroundWorker1.IsBusy)
                {
                    txtProgress.Text = "Canceling...";
                    // Cancel the asynchronous operation.
                    backgroundWorker1.CancelAsync();
                }
                else
                {
                    this.Close();
                }
            }
        }
    }
}
