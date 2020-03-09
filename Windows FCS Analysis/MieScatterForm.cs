using FlowCytometry.Mie_Scatter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Windows_FCS_Analysis
{
    public partial class MieScatterForm : Form
    {

        List<Record> arrRecords = new List<Record>();
        double record_min_s1 = 1000;
        double record_max_s1 = 0;
        double record_min_s2 = 1000;
        double record_max_s2 = 0;

        string str_n0 = "";
        string str_alpha = "";
        string str_n_s = "";
        string str_HC_min = "";
        string str_HC_max = "";
        string str_M_mass = "";
        string str_lambda = "";
        string str_eps = "";

        string str_V_min = "";
        string str_V_max = "";

        string str_Theta_1 = "";
        string str_delta_Theta_1 = "";
        string str_Theta_2 = "";
        string str_delta_Theta_2 = "";

        string str_Phi_1 = "";
        string str_delta_Phi_1 = "";
        string str_Phi_2 = "";
        string str_delta_Phi_2 = "";

        double V_min = 0;
        double V_max = 0;

        double HC_min = 0;
        double HC_max = 0;

        List<double> arrIDX = new List<double>();
        List<double> arrRes1 = new List<double>();
        List<double> arrRes2 = new List<double>();

        public MieScatterForm()
        {
            InitializeComponent();
            ReadRecords();
        }

        private bool validateInputs()
        {
            string message, caption;
            MessageBoxButtons button;

            str_n0 = txt_n0.Text.ToString();
            str_alpha = txt_alpha.Text.ToString();
            str_n_s = txt_n_s.Text.ToString();
            str_HC_min = txt_HC_min.Text.ToString();
            str_HC_max = txt_HC_max.Text.ToString();
            str_M_mass = txt_M_mass.Text.ToString();
            str_lambda = txt_lambda.Text.ToString();
            str_eps = txt_eps.Text.ToString();

            str_V_min = txt_V_min.Text.ToString();
            str_V_max = txt_V_max.Text.ToString();

            str_Theta_1 = txt_Theta_1.Text.ToString();
            str_delta_Theta_1 = txt_delta_Theta_1.Text.ToString();
            str_Theta_2 = txt_Theta_2.Text.ToString();
            str_delta_Theta_2 = txt_delta_Theta_2.Text.ToString();

            str_Phi_1 = txt_phi_1.Text.ToString();
            str_delta_Phi_1 = txt_delta_phi_1.Text.ToString();
            str_Phi_2 = txt_phi_2.Text.ToString();
            str_delta_Phi_2 = txt_delta_phi_2.Text.ToString();

            // Validation of Parameters - Complex Refractive Index
            if (
                !isValidate(str_n0) || !isValidate(str_alpha) || !isValidate(str_n_s) ||
                !isValidate(str_HC_min) || !isValidate(str_HC_max) ||
                !isValidate(str_M_mass) || !isValidate(str_lambda) || !isValidate(str_eps)
            )
            {
                message = "Parameters of Complex Refractive Index are incorrect." + Environment.NewLine + "Please input them correctly.";
                caption = "Input Error";
                button = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, button);
                return false;
            }

            HC_min = Convert.ToDouble(str_HC_min);
            HC_max = Convert.ToDouble(str_HC_max);

            if (HC_min < 0 || HC_max < 0 || HC_max <= HC_min)
            {
                message = "HC values of Complex Refractive Index are incorrect." + Environment.NewLine + "Please input them correctly.";
                caption = "Input Error";
                button = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, button);
                return false;
            }

            // Validation of Parameters - Size Parameter
            if (!isValidate(str_V_min) || !isValidate(str_V_max))
            {
                message = "Size parameters are incorrect." + Environment.NewLine + "Please input them correctly.";
                caption = "Input Error";
                button = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, button);
                return false;
            }

            V_min = Convert.ToDouble(str_V_min);
            V_max = Convert.ToDouble(str_V_max);

            if (V_min < 0 || V_max < 0 || V_max <= V_min)
            {
                message = "V values of Size Parameter are incorrect." + Environment.NewLine + "Please input them correctly.";
                caption = "Input Error";
                button = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, button);
                return false;
            }

            // Validation of Parameters - Scattering Angle
            if (
                !isValidate(str_Theta_1) || !isValidate(str_delta_Theta_1) ||
                !isValidate(str_Theta_2) || !isValidate(str_delta_Theta_2) ||
                !isValidate(str_Phi_1) || !isValidate(str_delta_Phi_1) ||
                !isValidate(str_Phi_2) || !isValidate(str_delta_Phi_2)
            )
            {
                message = "Parameters of Scattering Angle are incorrect." + Environment.NewLine + "Please input them correctly.";
                caption = "Input Error";
                button = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, button);
                return false;
            }

            return true;
        }

        private void btnEstimate_Click(object sender, EventArgs e)
        {
            if (!validateInputs())
            {
                return;
            }
            CalculateS12FromV_HC();
            ReadRecords();
            ResGraph.Refresh();
        }

        public bool isValidate(string str)
        {
            return !String.IsNullOrEmpty(str);
        }

        private void ResGraph_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            // Clear picture box with blue color
            g.Clear(Color.White);

            if (arrRecords.Count < 1 || record_min_s2 > record_max_s2 || record_min_s1 > record_max_s1)
                return;

            float w = ResGraph.Width;
            float h = ResGraph.Height;
            float padding = 10;

            // Create a pen to draw Ellipse
            Pen pen = new Pen(Color.Black);
            Pen pen_r = new Pen(Color.Red);
            Pen pen_b = new Pen(Color.Blue);
            Pen DrawPen;
            //Draw Coordinate
            g.DrawLine(pen, padding, h - padding, w - 2 * padding, h - padding);
            g.DrawLine(pen, padding, h - padding, padding, padding);

            float nXstep = (w - 2 * padding) / (float)Math.Max(0.1, (record_max_s2 - record_min_s2));
            float nYstep = (h - 2 * padding) / (float)Math.Max(0.1, (record_max_s1 - record_min_s1));
            int i = 0;
            bool emphasizeV = radioBtn_V.Checked;

            for (i = 0; i < arrRecords.Count(); i++)
            {
                float x = padding + nXstep * (float)(arrRecords[i].S2 - record_min_s2);
                float y = h - padding - nYstep * (float)(arrRecords[i].S1 - record_min_s1);
                if (emphasizeV)
                {
                    if (arrRecords[i].V % 20 < 10)
                        DrawPen = pen_r;
                    else
                        DrawPen = pen_b;
                }
                else
                {
                    if (arrRecords[i].HC % 6 < 3)
                        DrawPen = pen_r;
                    else
                        DrawPen = pen_b;
                }
                g.DrawEllipse(DrawPen, x, y, 1, 1);
            }
        }

        private void CalculateS12FromV_HC()
        {
            double _ln10 = Math.Log(10);
            double _PI = Math.PI;

            try
            {
                ///////   Init parameters  //////////
                double n0 = Math.Abs(Convert.ToDouble(str_n0));
                double alpha = Math.Abs(Convert.ToDouble(str_alpha));
                double n_s = Math.Abs(Convert.ToDouble(str_n_s));
                double M_mass = Math.Abs(Convert.ToDouble(str_M_mass));
                double lambda = Math.Abs(Convert.ToDouble(str_lambda));
                double eps = Math.Abs(Convert.ToDouble(str_eps));

                double Theta_1 = Math.Abs(Convert.ToDouble(str_Theta_1));
                double delta_Theta_1 = Math.Abs(Convert.ToDouble(str_delta_Theta_1));
                double Theta_2 = Math.Abs(Convert.ToDouble(str_Theta_2));
                double delta_Theta_2 = Math.Abs(Convert.ToDouble(str_delta_Theta_2));

                double Phi_1 = Math.Abs(Convert.ToDouble(str_Phi_1)) * _PI / 180.0f;
                double delta_Phi_1 = Math.Abs(Convert.ToDouble(str_delta_Phi_1)) * _PI / 180.0f;
                double Phi_2 = Math.Abs(Convert.ToDouble(str_Phi_2)) * _PI / 180.0f;
                double delta_Phi_2 = Math.Abs(Convert.ToDouble(str_delta_Phi_2)) * _PI / 180.0f;

                double m1 = 0.0, m2 = 0.0;
                Complex m = new Complex(0, 0);
                double k0 = 0.0, r = 0.0;

                k0 = 2 * _PI * n_s / lambda;
                Mie_si12_result si12_res;

                double v = 0;
                double theta_step = 0.1;
                double s1 = 0, s2 = 0;
                double hc = 0;
                double i1, i2;

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@".\result.txt", false))
                {
                    for (hc = HC_min; hc <= HC_max; hc++)
                    {

                        m1 = (n0 + alpha * hc) / n_s;
                        m2 = _ln10 / (_PI * M_mass * n_s) * lambda * eps * hc;

                        m = new Complex(m1, -m2);

                        for (v = V_min; v <= V_max; v++)
                        {
                            r = Math.Pow(3 * v / (4 * _PI), 1.0f / 3);
                            /////////    Calculate Integral of S2     ////////////
                            s1 = 0;
                            for (double theta = Theta_1; theta <= Theta_1 + delta_Theta_1; theta += theta_step)
                            {

                                double theta1_rad = _PI * theta / 180;

                                si12_res = Mie_si12.calc_mie_si12(m, k0, r, theta1_rad);
                                if (!si12_res.isSuccess)
                                {
                                    MessageBox.Show(si12_res.errStr, "Error", MessageBoxButtons.OK);
                                    return;
                                }
                                i1 = Math.Pow(Complex.Abs(si12_res.si1), 2);
                                i2 = Math.Pow(Complex.Abs(si12_res.si2), 2);

                                s1 += ((i1 + i2) * delta_Phi_1 / 2.0f
                                        - (i1 - i2) * (
                                            Math.Sin(2.0f * (Phi_1 + delta_Phi_1))
                                            - Math.Sin(2.0f * Phi_1))
                                        / 4.0f) * Math.Sin(theta1_rad);
                            }
                            s1 *= theta_step / (2 * 180 * k0 * k0);

                            /////////    Calculate Integral of S2     ////////////
                            s2 = 0;
                            for (double theta = Theta_2; theta <= Theta_2 + delta_Theta_2; theta += theta_step)
                            {

                                double theta1_rad = _PI * theta / 180;

                                si12_res = Mie_si12.calc_mie_si12(m, k0, r, theta1_rad);
                                if (!si12_res.isSuccess)
                                {
                                    MessageBox.Show(si12_res.errStr, "Error", MessageBoxButtons.OK);
                                    return;
                                }
                                i1 = Math.Pow(Complex.Abs(si12_res.si1), 2);
                                i2 = Math.Pow(Complex.Abs(si12_res.si2), 2);

                                s2 += ((i1 + i2) * delta_Phi_2 / 2.0f
                                        - (i1 - i2) * (
                                            Math.Sin(2.0f * (Phi_2 + delta_Phi_2))
                                            - Math.Sin(2.0f * Phi_2))
                                        / 4.0f) * Math.Sin(theta1_rad);
                            }
                            s2 *= theta_step / (2 * 180 * k0 * k0);

                            //save results on text file
                            file.WriteLine("{0},{1},{2},{3}", v.ToString(), hc.ToString(), s1.ToString(), s2.ToString());
                        }

                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK);
                return;
            }
        }

        private void BtnS12ToVHC_Click(object sender, EventArgs e)
        {
            double s1 = Convert.ToDouble(txt_S1.Text.ToString());
            double s2 = Convert.ToDouble(txt_S2.Text.ToString());

            Record record = arrRecords.Aggregate((e1, e2) =>
                Math.Pow(Math.Abs(e1.S1 - s1), 2) + Math.Pow(Math.Abs(e1.S2 - s2), 2) <
                Math.Pow(Math.Abs(e2.S1 - s1), 2) + Math.Pow(Math.Abs(e2.S2 - s2), 2) ?
                e1 : e2);
            txt_HC.Text = record.HC + "";
            txt_V.Text = record.V + "";
        }

        private void ReadRecords()
        {
            arrRecords.Clear();
            record_min_s1 = 1000;
            record_max_s1 = 0;
            record_min_s2 = 1000;
            record_max_s2 = 0;
            try
            {
                using (System.IO.StreamReader file = new System.IO.StreamReader(@".\result.txt"))
                {
                    string line;
                    string[] vals;
                    while ((line = file.ReadLine()) != null)
                    {
                        vals = line.Split(',');
                        try
                        {
                            Record record = new Record
                            {
                                V = Convert.ToInt32(vals[0]),
                                HC = Convert.ToInt32(vals[1]),
                                S1 = Convert.ToDouble(vals[2]),
                                S2 = Convert.ToDouble(vals[3])
                            };
                            record_min_s1 = Math.Min(record_min_s1, record.S1);
                            record_max_s1 = Math.Max(record_max_s1, record.S1);
                            record_min_s2 = Math.Min(record_min_s2, record.S2);
                            record_max_s2 = Math.Max(record_max_s2, record.S2);

                            arrRecords.Add(record);
                        }
                        catch
                        {
                            return;
                        }
                    }
                }

            }
            catch
            {

            }
        }

        private void radioBtn_HC_CheckedChanged(object sender, EventArgs e)
        {
            ResGraph.Refresh();
        }

        private void radioBtn_V_CheckedChanged(object sender, EventArgs e)
        {
            ResGraph.Refresh();
        }
    }
}
