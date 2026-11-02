using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CalculatorApp.ViewModels;

public class CalculatorViewModel : INotifyPropertyChanged
{
    private string _currentInput = "0";
    private string _expression = string.Empty;
    private double _firstOperand = 0;
    private string _pendingOp = string.Empty;
    private bool _isNewNumber = true;
    private bool _hasError = false;

    public string CurrentInput
    {
        get => _currentInput;
        set { _currentInput = value; OnPropertyChanged(); }
    }

    public string Expression
    {
        get => _expression;
        set { _expression = value; OnPropertyChanged(); }
    }

    public ICommand DigitCommand { get; }
    public ICommand OperatorCommand { get; }
    public ICommand CalculateCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand ClearEntryCommand { get; }
    public ICommand BackspaceCommand { get; }
    public ICommand DecimalCommand { get; }
    public ICommand NegateCommand { get; }
    public ICommand FuncCommand { get; }

    public CalculatorViewModel()
    {
        DigitCommand = new Command<string>(OnDigit);
        OperatorCommand = new Command<string>(OnOperator);
        CalculateCommand = new Command(OnCalculate);
        ClearCommand = new Command(OnClear);
        ClearEntryCommand = new Command(OnClearEntry);
        BackspaceCommand = new Command(OnBackspace);
        DecimalCommand = new Command(OnDecimal);
        NegateCommand = new Command(OnNegate);
        FuncCommand = new Command<string>(OnScientificFunc);
    }

    private void OnDigit(string d)
    {
        if (_hasError || _isNewNumber)
        {
            CurrentInput = d;
            _isNewNumber = false;
            _hasError = false;
        }
        else
        {
            CurrentInput = (CurrentInput == "0") ? d : CurrentInput + d;
        }
    }

    private void OnDecimal()
    {
        if (_hasError || _isNewNumber)
        {
            CurrentInput = "0,";
            _isNewNumber = false;
            _hasError = false;
            return;
        }
        if (!CurrentInput.Contains(','))
            CurrentInput += ",";
    }

    private void OnNegate()
    {
        if (_hasError || CurrentInput == "0") return;
        if (double.TryParse(CurrentInput, NumberStyles.Any, CultureInfo.GetCultureInfo("tr-TR"), out double val))
        {
            CurrentInput = (-val).ToString("G", CultureInfo.GetCultureInfo("tr-TR"));
        }
    }

    private void OnOperator(string op)
    {
        if (_hasError) return;

        if (double.TryParse(CurrentInput, NumberStyles.Any, CultureInfo.GetCultureInfo("tr-TR"), out double val))
        {
            if (!string.IsNullOrEmpty(_pendingOp) && !_isNewNumber)
            {
                val = ExecuteOp(_firstOperand, val, _pendingOp);
                if (_hasError) return;
                CurrentInput = val.ToString("G", CultureInfo.GetCultureInfo("tr-TR"));
            }

            _firstOperand = val;
            _pendingOp = op;
            Expression = $"{_firstOperand} {op}";
            _isNewNumber = true;
        }
    }

    private void OnCalculate()
    {
        if (_hasError || string.IsNullOrEmpty(_pendingOp)) return;

        if (double.TryParse(CurrentInput, NumberStyles.Any, CultureInfo.GetCultureInfo("tr-TR"), out double secondOperand))
        {
            Expression = $"{_firstOperand} {_pendingOp} {secondOperand} =";
            double res = ExecuteOp(_firstOperand, secondOperand, _pendingOp);

            if (!_hasError)
            {
                CurrentInput = res.ToString("G", CultureInfo.GetCultureInfo("tr-TR"));
                _firstOperand = res;
                _pendingOp = string.Empty;
                _isNewNumber = true;
            }
        }
    }

    private double ExecuteOp(double a, double b, string op)
    {
        switch (op)
        {
            case "+": return a + b;
            case "-": return a - b;
            case "×": return a * b;
            case "÷":
                if (Math.Abs(b) < double.Epsilon)
                {
                    SetError("Sıfıra bölünemez");
                    return 0;
                }
                return a / b;
            case "^": return Math.Pow(a, b);
            case "%": return (a * b) / 100.0;
            default: return b;
        }
    }

    private void OnScientificFunc(string fn)
    {
        if (_hasError) return;

        if (double.TryParse(CurrentInput, NumberStyles.Any, CultureInfo.GetCultureInfo("tr-TR"), out double val))
        {
            double res = 0;
            switch (fn)
            {
                case "sqrt":
                    if (val < 0) { SetError("Geçersiz giriş"); return; }
                    res = Math.Sqrt(val);
                    Expression = $"√({val})";
                    break;
                case "sqr":
                    res = Math.Pow(val, 2);
                    Expression = $"sqr({val})";
                    break;
                case "1/x":
                    if (Math.Abs(val) < double.Epsilon) { SetError("Sıfıra bölünemez"); return; }
                    res = 1 / val;
                    Expression = $"1/({val})";
                    break;
                case "sin":
                    res = Math.Sin(val * Math.PI / 180.0);
                    Expression = $"sin({val})";
                    break;
                case "cos":
                    res = Math.Cos(val * Math.PI / 180.0);
                    Expression = $"cos({val})";
                    break;
                case "tan":
                    if (Math.Abs(Math.Cos(val * Math.PI / 180.0)) < 1e-10) { SetError("Geçersiz giriş"); return; }
                    res = Math.Tan(val * Math.PI / 180.0);
                    Expression = $"tan({val})";
                    break;
                case "log":
                    if (val <= 0) { SetError("Geçersiz giriş"); return; }
                    res = Math.Log10(val);
                    Expression = $"log({val})";
                    break;
                case "ln":
                    if (val <= 0) { SetError("Geçersiz giriş"); return; }
                    res = Math.Log(val);
                    Expression = $"ln({val})";
                    break;
                case "fact":
                    if (val < 0 || val != Math.Floor(val) || val > 170) { SetError("Geçersiz giriş"); return; }
                    res = Factorial((int)val);
                    Expression = $"fact({val})";
                    break;
                case "pi": res = Math.PI; break;
                case "e": res = Math.E; break;
            }

            CurrentInput = res.ToString("G", CultureInfo.GetCultureInfo("tr-TR"));
            _isNewNumber = true;
        }
    }

    private double Factorial(int n)
    {
        if (n <= 1) return 1;
        double f = 1;
        for (int i = 2; i <= n; i++) f *= i;
        return f;
    }

    private void SetError(string message)
    {
        CurrentInput = message;
        Expression = string.Empty;
        _pendingOp = string.Empty;
        _hasError = true;
        _isNewNumber = true;
    }

    private void OnBackspace()
    {
        if (_hasError || _isNewNumber) return;
        CurrentInput = CurrentInput.Length > 1 ? CurrentInput[..^1] : "0";
    }

    private void OnClearEntry()
    {
        CurrentInput = "0";
        _isNewNumber = true;
        _hasError = false;
    }

    private void OnClear()
    {
        CurrentInput = "0";
        Expression = string.Empty;
        _firstOperand = 0;
        _pendingOp = string.Empty;
        _isNewNumber = true;
        _hasError = false;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}