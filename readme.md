This will look at how to hook up class interactions to bindings. The first thing is to set the `DataGridView` using `dataGridView.DataSource = childs`. If you did nothing else besides initialize it by overriding `MainForm.OnLoad` you'd already have a decent-looking view (but it would be missing the two-way interactions).

![prelim](https://github.com/IVSoftware/dgv-with-auto-total-class/blob/master/dgv-with-auto-total-class/ReadMe/prelim.png)

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
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
        private readonly BindingList<child> childs = new BindingList<child>();
        private readonly parent parent_object = new parent();
    }
***
**Bindable Properties**

In order to create a bound property that supports two-way communication, you need a way to detect and notify when the properties change. For example, to make the `total` property bindable in the `parent` class do this:

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
        public event PropertyChangedEventHandler PropertyChanged;
    }

The data binding shown in your code for numericUpDown will now respond to changes of total.

    numericUpDown.DataBindings.Add(
        nameof(numericUpDown.Value),
        parent_object, 
        nameof(parent_object.total), 
        false, 
        DataSourceUpdateMode.OnPropertyChanged);

***
**Responding to Changes _Internally_**

Once you make _all_ of your properties in the `child` class bindable in the same way by using the example above, consider taking the approach of handling certain changes internally in which case you would suppress the firing of the property change notification.

    public class child : INotifyPropertyChanged
    {
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
        ...
    }

***
**Connecting the `total` property in the `parent` class.**

The only thing missing now is having a way to tell the `parent_object` that a new global total for _all_ the rows is needed. The good news is that the bindings are already in place from the previous steps. To detect _any_ change to the DGV whether a new row is added or a a total is edited, subscribe to the `ListChanged` event of the `childs` collection by making this the first line in the `OnLoad` (before adding any items).

    childs.ListChanged += parent_object.onChildrenChanged;

This is a method that will need to be implemented in the `parent_object` class. Something like this:

    internal void onChildrenChanged(object sender, ListChangedEventArgs e)
    {
        var tmpTotal = 0m;
        foreach (var child in (IList<child>)sender)
        {
            tmpTotal += child.total;
        }
        total = tmpTotal;
    }

***
**READY TO TEST**

If you implement these steps, you'll have a fully-functional linked view where you can add, remove, and modify `child` records. 

![functional-bindings](https://github.com/IVSoftware/dgv-with-auto-total-class/blob/master/dgv-with-auto-total-class/ReadMe/functional-bindings.png)
