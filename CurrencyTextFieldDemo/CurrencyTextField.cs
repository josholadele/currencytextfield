using System;
using System.ComponentModel;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace CurrencyTextFieldDemo
{

    [Register("CurrencyTextField"), DesignTimeVisible(true)]
    public class CurrencyTextField : UITextField, IUITextFieldDelegate
    {
        private int maxDigits = 12;
        private double defaultValue = 0.00;
        private NSNumberFormatter currencyFormattor = new NSNumberFormatter();
        private string previousValue = "";
        private string currencySymbol = "₦";



        private void setAmount(double amount)
        {
            string textFieldStringValue = currencyFormattor.StringFromNumber(amount);
            this.Text = textFieldStringValue;
            if (true)
            {
                previousValue = textFieldStringValue;
            }
        }

        private void initTextField()
        {
            this.KeyboardType = UIKeyboardType.DecimalPad;
            currencyFormattor.NumberStyle = NSNumberFormatterStyle.Currency;
            currencyFormattor.CurrencySymbol = currencySymbol;
            currencyFormattor.MinimumFractionDigits = 2;
            currencyFormattor.MaximumFractionDigits = 2;
            setAmount(defaultValue);
        }

        public CurrencyTextField(IntPtr p)
            : base(p)
        {

        }

        public CurrencyTextField()
        {

        }

        protected void Setup()
        {
            this.Delegate = this;
            this.BorderStyle = UITextBorderStyle.None;
            this.BackgroundColor = UIColor.Clear;

        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);
            /*
             * if newSuperview != nil {
            NotificationCenter.default.addObserver(self, selector: #selector(UITextInputDelegate.textDidChange(_:)), name:NSNotification.Name.UITextFieldTextDidChange, object: self)
        } else {
            NotificationCenter.default.removeObserver(self)
        }
            */
            if (newsuper != null)
            {
                NSNotificationCenter.DefaultCenter.AddObserver(UITextField.TextFieldTextDidChangeNotification, HandleAction, this);
            }
            else
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(this);
            }
        }

        void HandleAction(NSNotification obj)
        {
            var cursorOffset = getOriginalCursorPosition();
            var cleanNumericString = getCleanNumberString();
            var textFieldLength = this.Text.Length;
            if (cleanNumericString.Length > maxDigits)
            {
                this.Text = previousValue;
            }
            else
            {
                var textFieldNumber = double.Parse(cleanNumericString);
                if (textFieldNumber != null)
                {
                    var textFieldNewValue = textFieldNumber / 100;
                    setAmount(textFieldNewValue);
                }
                else
                {
                    this.Text = previousValue;
                }
            }
        }

        private int getOriginalCursorPosition()
        {
            var cursorOffset = 0;
            var startPosition = this.BeginningOfDocument;
            if (this.SelectedTextRange != null)
            {
                cursorOffset = (int)this.GetOffsetFromPosition(startPosition, SelectedTextRange.Start);
            }
            return cursorOffset;
        }

        private string getCleanNumberString()
        {
            var cleanNumericString = "";
            var textFieldString = this.Text;
            if (!string.IsNullOrWhiteSpace(textFieldString))
            {
                var toArray = textFieldString.Split(currencySymbol.ToCharArray());//₦

                cleanNumericString = String.Join("", toArray);
                var splitArray = new char[] { ' ', ',', '.' };

                toArray = cleanNumericString.Split(splitArray);
                cleanNumericString = String.Join("", toArray);

            }
            return cleanNumericString;
        }

        private void setCursorOriginalPosition(int cursorOffset, int oldTextFieldLength)
        {
            var newLength = this.Text.Length;
            var startPosition = this.BeginningOfDocument;
            if (oldTextFieldLength > cursorOffset)
            {
                var newOffset = newLength - oldTextFieldLength + cursorOffset;
                var newCursorPosition = this.GetPosition(startPosition, newOffset);
                if (newCursorPosition != null)
                {
                    var newSelectedRange = this.GetTextRange(newCursorPosition, newCursorPosition);
                    this.SelectedTextRange = newSelectedRange;
                }
            }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            initTextField();
            Setup();
        }

    }
}
