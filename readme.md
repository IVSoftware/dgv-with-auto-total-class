Your code so far looks reasonable. Perhaps I can help with the big picture of getting everything hooked up. So the first thing is to simplify the data binding for the DGV where all you need is `dataGridView.DataSource = childs`. If you did nothing else besides initialize it by overriding `MainForm.OnLoad` you'd already have a decent-looking view but it would be missing interactions.

![screenshot]()

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

 ***
 **HOW TO RESPOND INTERNALLY**





