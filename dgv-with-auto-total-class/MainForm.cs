using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dgv_with_auto_total_class
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

    }
    public class child : INotifyPropertyChanged
    {
        string _product = string.Empty;

        public string product
        {
            get => _product;
            // set => SetProperty(ref _product, value);
            set
            {
                if (!Equals(_product, value))
                {
                    _product = value;
                    OnPropertyChanged();
                }
            }
        }
        decimal _amount = 0;
        public decimal amount
        {
            get => _amount;
            set
            {
                if (!Equals(_amount, value))
                {
                    _amount = value;
                    OnPropertyChanged();
                }
            }
        }
        decimal _price = 0;
        public decimal price
        {
            get => _price;
            set
            {
                if (!Equals(_price, value))
                {
                    _price = value;
                    OnPropertyChanged();
                }
            }
        }
        decimal _total = 0;
        public decimal total
        {
            get => _total;
            set
            {
                if (!Equals(_total, value))
                {
                    _total = value;
                    OnPropertyChanged();
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            switch (propertyName)
            {
                case nameof(price):
                case nameof(amount):
                    recalcTotal();
                    break;
                default:
                    break;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void recalcTotal()
        {
            total = price * amount;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class parent
    {
        public decimal total { get; set; }
        public BindingList<child> childs { get; set; }
    }
}
