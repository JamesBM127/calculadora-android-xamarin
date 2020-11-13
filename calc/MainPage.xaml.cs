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
        double firstNumber, secondNumber;
        bool firstOperator = true;
        bool percentPressed = false;

        private void OnClear(object sender, EventArgs e)
        {
            firstOperator = true;
            firstNumber = 0;
            secondNumber = 0;
            currentState = 1;
            this.resultText.Text = "0";
            this.resultHistory.Text = "";
            mathOperator = null;
            percentPressed = false;
        }

        /* ******************************************************************************************************************************
        *****************************************************  OnCancelEntry  *************************************************** 
        ****************************************************************************************************************************** */
        private void OnCancelEntry(object sender, EventArgs e)
        {
            this.resultText.Text = "0";
            secondNumber = 0;
            currentState = -2;
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
            if (percentPressed)
            {
                OnPercent(new object(), new EventArgs());
                return;
            }

            if(currentState == 2)
            {
                double result = 0;
                switch (mathOperator)
                {
                    case "+": result = firstNumber + secondNumber; break;
                    case "-": result = firstNumber - secondNumber; break;
                    case "x": result = firstNumber * secondNumber; break;
                    case "/": result = firstNumber / secondNumber; break;
                }

                this.resultHistory.Text += secondNumber.ToString() + "=";

                string resultString = result.ToString("N10");
                
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

        /* ******************************************************************************************************************************
        ********************************************************   OnPercent   ********************************************************
        ****************************************************************************************************************************** */

        private void OnPercent(object sender, EventArgs e)
        {
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

        }

        public MainPage()
        {
            InitializeComponent();
            OnClear(new object(), new EventArgs());
        }
    }
}