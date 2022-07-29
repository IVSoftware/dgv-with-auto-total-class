using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace dgv_with_auto_total_class
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            numericUpDown.Maximum = decimal.MaxValue;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            childs.ListChanged += parent_object.onChildrenChanged;

            dataGridView.DataSource = childs;

            // Add one or more child items to autogenerate rows.
            childs.Add(new child
            {
                product = "GEARWRENCH Pinch Off Pliers",
                price = 27.10m,
                amount = 1.0m
            });
            childs.Add(new child
            {
                product = "AXEMAX Bungee Cords",
                price = 25.48m,
                amount = 1.0m
            });

            // Format rows
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                switch (column.Name)
                {
                    case nameof(child.product):
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        break;
                    default:
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        column.DefaultCellStyle.Format = "F2";
                        break;
                }
            }

            numericUpDown.DataBindings.Add(
                nameof(numericUpDown.Value),
                parent_object, 
                nameof(parent_object.total), 
                false, 
                DataSourceUpdateMode.OnPropertyChanged);
        }
        private readonly BindingList<child> childs = new BindingList<child>();
        private readonly parent parent_object = new parent();
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
            // Read-only in DGV
            private set
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
                    // Perform an internal calculation.
                    recalcTotal();
                    break;
                default:
                    // Notify subscribers of important changes like product and total.
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                    break;
            }
        }
        private void recalcTotal()
        {
            total = price * amount;
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class parent : INotifyPropertyChanged
    {
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void onChildrenChanged(object sender, ListChangedEventArgs e)
        {
            var tmpTotal = 0m;
            foreach (var child in (IList<child>)sender)
            {
                tmpTotal += child.total;
            }
            total = tmpTotal;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
