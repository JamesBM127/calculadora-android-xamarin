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
        int currentState = 1;
        string mathOperator;
        double firstNumber, secondNumber;

        private void OnClear(object sender, EventArgs e)
        {
            firstNumber = 0;
            secondNumber = 0;
            currentState = 1;
            this.resultText.Text = "0";
            mathOperator = null;
        }

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

        private void OnSelectOperator(object sender, EventArgs e)
        {
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
        }

        private void OnCalculate(object sender, EventArgs e)
        {
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

                string resultString = result.ToString("N10");

                while (resultString.EndsWith("0")){
                    resultString = resultString.Remove(resultString.Length - 1);
                }

                this.resultText.Text = resultString;
                
                firstNumber = result;
                currentState = -1;
            }
        }

        public MainPage()
        {
            InitializeComponent();
            OnClear(new object(), new EventArgs());
        }
    }
}