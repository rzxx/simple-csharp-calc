using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool recordHistory = true;
        int state = 0;
        int lastoperation = 0;
        string input = "0";
        double memory = 0;
        double memoryRepeat = 0;
        bool removeAfterFirstInput = false;

        List<string> operationHistory = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            VersionDefault(null, null); //just to apply default settings

            foreach (UIElement el in CalcFather.Children)
            {
                if (el is Button)
                {
                    ((Button)el).Click += ButtonClick;
                }
            }
            CheckBoxRecordOperations.Checked += CheckBoxChecked;
            CheckBoxRecordOperations.Unchecked += CheckBoxUnchecked;
            RadioDefault.Checked += VersionDefault;
            RadioPremium.Checked += VersionPremium;
        }
        
        private void VersionDefault(object sender, RoutedEventArgs e)
        {
            ButtonDeleteAll.Visibility = Visibility.Hidden;
            ButtonDeleteLast.Visibility = Visibility.Hidden;
            CheckBoxRecordOperations.Visibility = Visibility.Hidden;
            OperationsList.Visibility = Visibility.Hidden;
        }

        private void VersionPremium(object sender, RoutedEventArgs e)
        {
            ButtonDeleteAll.Visibility = Visibility.Visible;
            ButtonDeleteLast.Visibility = Visibility.Visible;
            CheckBoxRecordOperations.Visibility = Visibility.Visible;
            OperationsList.Visibility = Visibility.Visible;
        }
        private void CheckBoxChecked(object sender, RoutedEventArgs e)
        {
            recordHistory = true;
        }

        private void CheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            recordHistory = false;
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            string str = (string)((Button)e.OriginalSource).Content;
            if (sender == ButtonDeleteAll)
            {
                HistoryRemove(true);
            }
            else if (sender == ButtonDeleteLast)
            {
                HistoryRemove(false);
            }
            else
            {
                switch (str)
                {
                    case "Купить доп. кнопки":
                        Numero.FontSize = 72;
                        Numero.Text = "Стереть?";
                        return;
                    case "+":
                        HandleOperator(1);
                        break;
                    case "-":
                        HandleOperator(2);
                        break;
                    case "×":
                        HandleOperator(3);
                        break;
                    case "÷":
                        HandleOperator(4);
                        break;
                    case "=":
                        HandleOperator(5);
                        break;
                    case "+/-":
                        PlusMinus();
                        break;
                    default:
                        CheckForRemovingInput();
                        if (input == "0" && str != ",") input = "";
                        if (str == "," && !CanAddPoint()) break;
                        input += str;
                        break;
                }
                NumeroDraw();
            }
        }

        void HistoryRemove(bool everything)
        {
            if (!operationHistory.Any()) return;

            if (everything) operationHistory.Clear();
            else operationHistory.RemoveAt(0);

            HistoryDraw();
        }

        void HistoryDraw()
        {
            OperationsList.ItemsSource = null;
            OperationsList.ItemsSource = operationHistory;
        }

        void CheckForRemovingInput()
        {
            if (removeAfterFirstInput)
            {
                input = "0";
                removeAfterFirstInput = false;
            }
        }

        bool CanAddPoint()
        {
            if (input.Contains(",")) return false;
            else return true;
        }

        void PlusMinus()
        {
            if (input[0] == '-') input = input.Remove(0,1);
            else input = input.Insert(0, "-");
        }

        void HandleOperator(int operation)
        {
            string operationString = "";

            if (state != 0)
            {
                switch (state)
                {
                    case 1:
                        if (removeAfterFirstInput && operation == 5)
                        {
                            operationString = $"{memory} + {memoryRepeat} = {memory + memoryRepeat}";
                            memory += memoryRepeat;
                        }
                        else if (lastoperation != 5)
                        {
                            operationString = $"{memory} + {input} = {memory + Convert.ToDouble(input)}";
                            memory += Convert.ToDouble(input);
                            memoryRepeat = Convert.ToDouble(input);

                        }
                        break;
                    case 2:
                        if (removeAfterFirstInput && operation == 5)
                        {
                            operationString = $"{memory} - {memoryRepeat} = {memory - memoryRepeat}";
                            memory -= memoryRepeat;
                        }
                        else if (lastoperation != 5)
                        {
                            operationString = $"{memory} - {input} = {memory - Convert.ToDouble(input)}";
                            memory -= Convert.ToDouble(input);
                            memoryRepeat = Convert.ToDouble(input);
                        }
                        break;
                    case 3:
                        if (removeAfterFirstInput && operation == 5)
                        {
                            operationString = $"{memory} × {memoryRepeat} = {memory * memoryRepeat}";
                            memory *= memoryRepeat;
                        }
                        else if (lastoperation != 5)
                        {
                            operationString = $"{memory} × {input} = {memory * Convert.ToDouble(input)}";
                            memory *= Convert.ToDouble(input);
                            memoryRepeat = Convert.ToDouble(input);
                        }
                        break;
                    case 4:
                        if (removeAfterFirstInput && operation == 5)
                        {
                            operationString = $"{memory} ÷ {memoryRepeat} = {memory / memoryRepeat}";
                            memory /= memoryRepeat;
                        }
                        else if (lastoperation != 5)
                        {
                            operationString = $"{memory} ÷ {input} = {memory / Convert.ToDouble(input)}";
                            memory /= Convert.ToDouble(input);
                            memoryRepeat = Convert.ToDouble(input);
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                memory = Convert.ToDouble(input);
            }

            if (recordHistory)
            {
                if (operationString != "") operationHistory.Insert(0, operationString);
                HistoryDraw();
            }

            lastoperation = operation;

            removeAfterFirstInput = true;
            input = memory.ToString();
            if (operation == 5)
            {
            }
            else state = operation;
        }

        private void NumeroDraw()
        {
            switch (input.Length)
            {
                case int n when (n > 20):
                    Numero.FontSize = 32;
                    Numero.Text = "Длина превышена!";
                    return;
                case int n when (n > 16):
                    Numero.FontSize = 28;
                    break;
                case int n when (n > 12):
                    Numero.FontSize = 35;
                    break;
                case int n when (n > 9):
                    Numero.FontSize = 48;
                    break;
                case int n when (n > 7):
                    Numero.FontSize = 64;
                    break;
                default:
                    Numero.FontSize = 76;
                    break;
            }
            Numero.Text = input;
        }
    }
}