using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ePicSearch.Views
{
    public partial class CodeEntryControl : ContentView
    {
        public ObservableCollection<DigitViewModel> Digits { get; private set; }

        public ICommand IncreaseDigitCommand { get; }
        public ICommand DecreaseDigitCommand { get; }
        public ICommand EnterCommand { get; }

        // Event to notify when code is entered
        public event EventHandler<string> CodeEntered;

        public CodeEntryControl()
        {
            InitializeComponent();

            Digits = new ObservableCollection<DigitViewModel>
            {
                new DigitViewModel { Value = 0, Index = 0 },
                new DigitViewModel { Value = 0, Index = 1 },
                new DigitViewModel { Value = 0, Index = 2 },
                new DigitViewModel { Value = 0, Index = 3 }
            };

            IncreaseDigitCommand = new Command<int>(IncreaseDigit);
            DecreaseDigitCommand = new Command<int>(DecreaseDigit);
            EnterCommand = new Command(OnEnter);

            BindingContext = this;
        }

        private void IncreaseDigit(int index)
        {
            var digit = Digits[index];
            digit.Value = (digit.Value + 1) % 10;
        }

        private void DecreaseDigit(int index)
        {
            var digit = Digits[index];
            digit.Value = (digit.Value + 9) % 10;
        }

        private void OnEnter()
        {
            string code = string.Join("", Digits.Select(d => d.Value));
            CodeEntered?.Invoke(this, code);
        }
    }
}
