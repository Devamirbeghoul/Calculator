using Guna.UI2.WinForms;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calculator
{
    public partial class fmCalculator : Form
    {
        public fmCalculator()
        {
            InitializeComponent();
            CustomWindow(Color.FromArgb(68, 109, 255), Color.White, Color.FromArgb(68, 109, 255), Handle);

        }

        private string ToBgr(Color c) => $"{c.B:X2}{c.G:X2}{c.R:X2}";

        [DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);

        const int DWWMA_CAPTION_COLOR = 35;
        const int DWWMA_BORDER_COLOR = 34;
        const int DWMWA_TEXT_COLOR = 36;
        public void CustomWindow(Color captionColor, Color fontColor, Color borderColor, IntPtr handle)
        {
            IntPtr hWnd = handle;
            //Change caption color
            int[] caption = new int[] { int.Parse(ToBgr(captionColor), System.Globalization.NumberStyles.HexNumber) };
            DwmSetWindowAttribute(hWnd, DWWMA_CAPTION_COLOR, caption, 4);
            //Change font color
            int[] font = new int[] { int.Parse(ToBgr(fontColor), System.Globalization.NumberStyles.HexNumber) };
            DwmSetWindowAttribute(hWnd, DWMWA_TEXT_COLOR, font, 4);
            //Change border color
            int[] border = new int[] { int.Parse(ToBgr(borderColor), System.Globalization.NumberStyles.HexNumber) };
            DwmSetWindowAttribute(hWnd, DWWMA_BORDER_COLOR, border, 4);

        }


        bool IsLastbtnOperation = false;
        bool IsLastbtnEqualtion = false;
        bool Allow = false;
        bool DivError = false;
        char LastOpertion = 'n';
        float Num1;
        float Num2;
        bool Clear = true;

        void Reset()
        {
            IsLastbtnOperation = false;
            IsLastbtnEqualtion = false;
            LastOpertion = 'n';
            Num1 = 0;
            Num2 = 0;
            tbResult.Text = "0";
            lbPrev.Text = string.Empty;
            Allow = false;
            DivError = false;
            Clear = true;
        }
        void PerformLastOperation()
        {
            if (LastOpertion == '+')
            {
                Num1 += Num2;
                tbResult.Text = Num1.ToString();
                return;
            }

            if (LastOpertion == '-')
            {
                Num1 -= Num2;
                tbResult.Text = Num1.ToString();
                return;
            }

            if (LastOpertion == '×')
            {
                Num1 *= Num2;
                tbResult.Text = Num1.ToString();
                return;
            }

            if (LastOpertion == '÷')
            {
                if (Num2 == 0)
                {
                    tbResult.Text = "Cannot divide by zero";
                    DivError = true;

                }
                else
                {
                    Num1 /= Num2;
                   tbResult.Text = Num1.ToString();
                }
                    
                return;
            }

            if (LastOpertion == '%')
            {
                Num1 %= Num2;
                tbResult.Text = Num1.ToString();
                return;
            }

        }
        void PerformBs()
        {
            if (tbResult.Text != "0")
            {
                if (tbResult.Text != string.Empty)
                    tbResult.Text = tbResult.Text.Substring(0, tbResult.Text.Length - 1);

                if (tbResult.Text == string.Empty)
                {
                    tbResult.Text = "0";
                    Clear = true;
                }
                    

            }



            
        }

        private void btn_Click(object sender, EventArgs e)
        {
            if (DivError)
                Reset();

            if (Clear)
            {
                Clear = false;
                tbResult.Clear();
            }
                

            foreach (var Control in pnButtonsContainer.Controls)
            {
                if (Control is Guna2Button)
                {
                    Guna2Button btn = (Guna2Button)Control;
                    
                    if (sender == btn)
                    {
                        if (btn.Text == "0" && tbResult.Text == "0")
                            return;

                        if (btn.Text == "." && tbResult.Text == string.Empty)
                        {
                            tbResult.Text = "0" + btn.Text;
                            return;
                        }

                        if (btn.Text == "-" && IsLastbtnOperation)
                        {
                            tbResult.Text = "-";
                            return;
                        }


                        if (btn.Text == "C")
                        {
                            Reset();
                            return;
                        }

                        if (btn.Text == "CE")
                        {
                            tbResult.Text = "0";
                            Clear = true;
                            return;
                        }

                        if (btn.Text == "DEL")
                        {
                            PerformBs();
                            return;
                        }

                        

                        if (btn.Text == "+" || btn.Text == "-" || btn.Text == "×" || btn.Text == "÷" || btn.Text == "%")
                        {
                            if (tbResult.Text == string.Empty)
                                tbResult.Text = "0";

                            Clear = true;

                            LastOpertion = Convert.ToChar(btn.Text);
                            
                            if (!IsLastbtnOperation)
                            {
                                
                                IsLastbtnOperation = true;

                                if (IsLastbtnEqualtion)
                                {
                                    lbPrev.Text = " " + Num1.ToString() + " " + btn.Text;
                                    IsLastbtnEqualtion = false;
                                    return;

                                }

                                Num1 = Convert.ToSingle(tbResult.Text);


                                lbPrev.Text = " " + tbResult.Text + " " + btn.Text;

                                return;
                            }

                            if (IsLastbtnOperation)
                            {
                                Num2 = Convert.ToSingle(tbResult.Text);

                                PerformLastOperation();
                                LastOpertion = 'n';
                                Num1 = Convert.ToSingle(tbResult.Text);
                                IsLastbtnOperation = true;
                                lbPrev.Text = " " + tbResult.Text + " " + btn.Text;
                                return;

                            }


                        }

                        if (btn.Text == "=")
                        {
                            if (!IsLastbtnOperation)
                                return;

                
                            Num2 = Convert.ToSingle(tbResult.Text);

                            lbPrev.Text += " " + tbResult.Text + " =";
                            PerformLastOperation();
                            
                            
                            if (DivError)
                                return;

   
                            IsLastbtnEqualtion = true;
                            IsLastbtnOperation = false;
                            return;
                        }


                        tbResult.Text += btn.Text;

                    }

                }
            }
        }
    }
}
