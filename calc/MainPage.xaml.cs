 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace calc
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        sbyte currentState = 1; //-128..127
        string mathOperator = null;
        double firstNumber, secondNumber, secondNumberTemp;
        bool firstOperator = true;
        bool specialOperation = false;
        bool percentPressed = false;
        bool oneXPressed = false;
        bool xSqr = false;

        private void OnClear(object sender, EventArgs e)
        {
            firstOperator = true;
            firstNumber = 0;
            secondNumber = 0;
            secondNumberTemp = 0;
            currentState = 1;
            this.resultText.Text = "0";
            this.resultHistory.Text = "";
            mathOperator = null;
            specialOperation = false;
            percentPressed = false;
            oneXPressed = false;
            xSqr = false;
        }

        /* ******************************************************************************************************************************
        ********************************************************   OnPercent   ********************************************************
        ****************************************************************************************************************************** */

        private void OnPercent(object sender, EventArgs e)
        {
            specialOperation = true;
            if (!percentPressed)
            {
                if (mathOperator == "x" || mathOperator == "/")
                {
                    this.resultHistory.Text = firstNumber.ToString() + mathOperator + (secondNumber / 100).ToString();
                    this.resultText.Text = (secondNumber / 100).ToString();
                }
                else
                {
                    this.resultHistory.Text = firstNumber.ToString() + mathOperator + (firstNumber * secondNumber / 100).ToString();
                    this.resultText.Text = (firstNumber * secondNumber / 100).ToString();
                }
                percentPressed = true;
                return;
            }

            double result = 0;
            switch (mathOperator)
            {
                case "+": result = firstNumber + (firstNumber * secondNumber / 100); break;
                case "-": result = firstNumber - (firstNumber * secondNumber / 100); break;
                case "x": result = firstNumber * secondNumber / 100; break;
                case "/": result = firstNumber / secondNumber / 100; break;
            }

            this.resultHistory.Text += "=";
            this.resultText.Text = result.ToString();
            firstNumber = double.Parse(this.resultText.Text);
            firstOperator = false;
        }

        /* ******************************************************************************************************************************
        *****************************************************  OnCancelEntry  *************************************************** 
        ****************************************************************************************************************************** */
        private void OnCancelEntry(object sender, EventArgs e)
        {
            this.resultText.Text = "0";
            secondNumber = 0;
            if (!firstOperator)
            {
                //currentState = -2 means you are typing the second number.
                currentState = -2;
            }
            
        }

        /* ******************************************************************************************************************************
        *****************************************************   OnDelButton   *************************************************** 
        ****************************************************************************************************************************** */

        private void OnDelButton(object sender, EventArgs e)
        {
            string numberDel = this.resultText.Text.Remove(this.resultText.Text.Length - 1);
            if(firstOperator)
            {
                firstNumber = double.Parse(numberDel);
                this.resultText.Text = numberDel;
            }
            else
            {
                secondNumber = double.Parse(numberDel);
                this.resultText.Text = numberDel;
            }
        }

        /* ******************************************************************************************************************************
        *********************************************************  On1x  ******************************************************** 
        ****************************************************************************************************************************** */
        private void On1x(object sender, EventArgs e)
        {
            specialOperation = true;
            oneXPressed = true;
            if (firstOperator)
            {
                this.resultHistory.Text = "1/(" + firstNumber.ToString() + ")";
                firstNumber = 1 / firstNumber;
                this.resultText.Text = firstNumber.ToString();
            }
            else
            {
                secondNumberTemp = secondNumber;
                this.resultHistory.Text = firstNumber.ToString() + mathOperator + "1/(" + secondNumber.ToString() + ")";
                secondNumber = 1 / secondNumber;
                this.resultText.Text = secondNumber.ToString();
            }
        }

        /* ******************************************************************************************************************************
        *********************************************************** xSqr ***************************************************************
        ****************************************************************************************************************************** */
        private void OnXSquare(object sender, EventArgs e)
        {
            xSqr = true;
            specialOperation = true;
            if (firstOperator)
            {
                this.resultHistory.Text = "sqr(" + firstNumber.ToString() + ")";
                firstNumber = Math.Pow(firstNumber, 2);
                this.resultText.Text = firstNumber.ToString();
            }
            else
            {
                this.resultHistory.Text = firstNumber.ToString() + "+srq(" + secondNumber.ToString() + ")";
                secondNumber = Math.Pow(secondNumber, 2);
                this.resultText.Text = secondNumber.ToString();
            }
        }

        /* ******************************************************************************************************************************
        *****************************************************   OnSelectNumber   *************************************************** 
        ****************************************************************************************************************************** */

        private void OnSelectNumber(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            if(this.resultText.Text == "0" || currentState < 0)
            {
                this.resultText.Text = "";
                if (currentState < 0) currentState *= -1;
            }

            this.resultText.Text += button.Text;

            double number;
            if(double.TryParse(this.resultText.Text, out number))
            {
                this.resultText.Text = number.ToString("N0");
                if (currentState == 1) firstNumber = number;
                else secondNumber = number;
            }

        }

        /* ******************************************************************************************************************************
            *****************************************************   OnSelectOperator   *************************************************** 
            ****************************************************************************************************************************** */

        private void OnSelectOperator(object sender, EventArgs e)
        {
            if (!firstOperator)
            {
                OnCalculate(new object(), new EventArgs());
                this.resultHistory.Text = this.resultHistory.Text.Remove(this.resultHistory.Text.Length - 1);
                this.resultHistory.Text += mathOperator;
            }

            currentState = -2;
            Button button = (Button)sender;
            if (mathOperator == button.Text) return;
            else if (mathOperator != null)
            {
                string pressed = this.resultText.Text;
                pressed = pressed.Remove(pressed.Length-1);
                this.resultText.Text = pressed;
            }
            mathOperator = button.Text;
            this.resultText.Text += mathOperator;

            if (this.resultHistory.Text.EndsWith("="))
            {
                string rHistory = this.resultHistory.Text.Remove(this.resultHistory.Text.Length - 1);
                this.resultHistory.Text = "";
                //this.resultHistory.Text = rHistory;
            }
            this.resultHistory.Text += this.resultText.Text;

            firstOperator = false;
        }

        /* ******************************************************************************************************************************
        *****************************************************   OnCalculate   *************************************************** 
        ****************************************************************************************************************************** */
        private void OnCalculate(object sender, EventArgs e)
        {
            if (specialOperation == true)
            {
                if (percentPressed)
                {
                    OnPercent(new object(), new EventArgs());
                    percentPressed = false;
                }

                if (oneXPressed)
                {
                    this.resultHistory.Text = firstNumber.ToString() + mathOperator + "1/(" + secondNumberTemp.ToString() + ")=";
                    double result = simple4Operations();
                    this.resultText.Text = result.ToString("N10");

                    removeTheZeros();
                    oneXPressed = false;
                }

                if (xSqr)
                {
                    this.resultHistory.Text = firstNumber.ToString() + "+sqr(" + secondNumber.ToString() + ")=";
                    double result = simple4Operations();
                    this.resultText.Text = result.ToString();

                    removeTheZeros();
                    xSqr = false;
                }
                
                firstNumber = double.Parse(this.resultText.Text);
                firstOperator = false;
                specialOperation = false;
                return;
            }

            if(currentState == 2)
            {
                double result = simple4Operations();                

                this.resultHistory.Text += secondNumber.ToString() + "=";

                string resultString = result.ToString("N10");

                //this while is being used to remove a lot of 0 from string
                while (resultString.EndsWith("0") || resultString.EndsWith(",")){
                    resultString = resultString.Remove(resultString.Length - 1);
                }

                if (resultString.EndsWith("."))
                {
                    resultString = resultString.Remove(resultString.Length - 1);
                }

                this.resultText.Text = resultString;
                
                firstNumber = result;
                currentState = -1;
            }
            else
            {
                this.resultHistory.Text = firstNumber.ToString() + "=";
            }
        }

        public MainPage()
        {
            InitializeComponent();
            OnClear(new object(), new EventArgs());
        }

        private double simple4Operations()
        {
            double result = 0;
            switch (mathOperator)
            {
                case "+": result = firstNumber + secondNumber; break;
                case "-": result = firstNumber - secondNumber; break;
                case "x": result = firstNumber * secondNumber; break;
                case "/": result = firstNumber / secondNumber; break;
            }
            return result;
        }

        private void removeTheZeros()
        {
            //this while is being used to remove a lot of 0 from string
            while (this.resultText.Text.EndsWith("0") || this.resultText.Text.EndsWith(","))
            {
                this.resultText.Text = this.resultText.Text.Remove(this.resultText.Text.Length - 1);
            }
        }
    }
}