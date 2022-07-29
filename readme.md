Your code so far looks reasonable. Perhaps I can help with the big picture of getting everything hooked up. So the first thing is to simplify the data binding for the DGV where all you need is `dataGridView.DataSource = childs`. If you did nothing else besides initialize it by overriding `MainForm.OnLoad` you'd already have a decent-looking view but it would be missing interactions.

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
    }
***
**BINDABLE PROPERTIES**

In order to create a bound property that supports two-way communication, you need a way to detect and notify when the properties change. For the `parent` class it would look something like this:

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

This class will support the data binding to the `numericUpDown` which could be added at the bottom of `OnLoad`.

    numericUpDown.DataBindings.Add(
        nameof(numericUpDown.Value),
        parent_object, 
        nameof(parent_object.total), 
        false, 
        DataSourceUpdateMode.OnPropertyChanged);

***
**RESPONDING TO CHANGES INTERNALLY**

Once you make of your properties in the `child` class bindable by using the example above, you might take the approach of handling certain changes internally in which case you would suppress the firing of the property change notification.

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
**Connect the `total` property in the `parent` class.**

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

If you implement these steps, you'll have a fully-functional linked view where you can add, remove, and modify `child` records. 

![functional-bindings](https://github.com/IVSoftware/dgv-with-auto-total-class/blob/master/dgv-with-auto-total-class/ReadMe/functional-bindings.png)
