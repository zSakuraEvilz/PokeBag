using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace PokeBag
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:PokeBag"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:PokeBag;assembly=PokeBag"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:SortDataGid/>
    ///
    /// </summary>
    public class SortDataGid : DataGrid
    {
        private bool inWidthChange = false;
        private bool updatingColumnInfo = false;
        public static readonly DependencyProperty ColumnInfoProperty = DependencyProperty.Register("ColumnInfo",
        typeof(ObservableCollection<ColumnInfo>), typeof(SortDataGid),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColumnInfoChangedCallback)
        );

        private static void ColumnInfoChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var grid = (SortDataGid)dependencyObject;
            if (!grid.updatingColumnInfo) { grid.ColumnInfoChanged(); }
        }

        protected override void OnInitialized(EventArgs e)
        {
            EventHandler sortDirectionChangedHandler = (sender, x) => UpdateColumnInfo();
            EventHandler widthPropertyChangedHandler = (sender, x) => inWidthChange = true;
            var sortDirectionPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(DataGridColumn.SortDirectionProperty, typeof(DataGridColumn));
            var widthPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(DataGridColumn.WidthProperty, typeof(DataGridColumn));

            Loaded += (sender, x) =>
            {
                foreach (var column in Columns)
                {
                    sortDirectionPropertyDescriptor.AddValueChanged(column, sortDirectionChangedHandler);
                    widthPropertyDescriptor.AddValueChanged(column, widthPropertyChangedHandler);
                }
            };
            Unloaded += (sender, x) =>
            {
                foreach (var column in Columns)
                {
                    sortDirectionPropertyDescriptor.RemoveValueChanged(column, sortDirectionChangedHandler);
                    widthPropertyDescriptor.RemoveValueChanged(column, widthPropertyChangedHandler);
                }
            };

            base.OnInitialized(e);
        }
        public ObservableCollection<ColumnInfo> ColumnInfo
        {
            get { return (ObservableCollection<ColumnInfo>)GetValue(ColumnInfoProperty); }
            set { SetValue(ColumnInfoProperty, value); }
        }
        private void UpdateColumnInfo()
        {
            updatingColumnInfo = true;
            ColumnInfo = new ObservableCollection<ColumnInfo>(Columns.Select((x) => new ColumnInfo(x)));
            updatingColumnInfo = false;
        }
        protected override void OnColumnReordered(DataGridColumnEventArgs e)
        {
            UpdateColumnInfo();
            base.OnColumnReordered(e);
        }
        protected override void OnPreviewMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (inWidthChange)
            {
                inWidthChange = false;
                UpdateColumnInfo();
            }
            base.OnPreviewMouseLeftButtonUp(e);
        }
        private void ColumnInfoChanged()
        {
            Items.SortDescriptions.Clear();
            foreach (var column in ColumnInfo)
            {
                var realColumn = Columns.Where((x) => column.Header.Equals(x.Header)).FirstOrDefault();
                if (realColumn == null) { continue; }
                column.Apply(realColumn, Columns.Count, Items.SortDescriptions);
            }
        }
    }

    public struct ColumnInfo
    {
        public ColumnInfo(DataGridColumn column)
        {
            Header = column.Header;
            PropertyPath = ((Binding)((DataGridBoundColumn)column).Binding).Path.Path;
            WidthValue = column.Width.DisplayValue;
            WidthType = column.Width.UnitType;
            SortDirection = column.SortDirection;
            DisplayIndex = column.DisplayIndex;
        }
        public void Apply(DataGridColumn column, int gridColumnCount, SortDescriptionCollection sortDescriptions)
        {
            column.Width = new DataGridLength(WidthValue, WidthType);
            column.SortDirection = SortDirection;
            if (SortDirection != null)
            {
                sortDescriptions.Add(new SortDescription(PropertyPath, SortDirection.Value));
            }
            if (column.DisplayIndex != DisplayIndex)
            {
                var maxIndex = (gridColumnCount == 0) ? 0 : gridColumnCount - 1;
                column.DisplayIndex = (DisplayIndex <= maxIndex) ? DisplayIndex : maxIndex;
            }
        }
        public object Header;
        public string PropertyPath;
        public ListSortDirection? SortDirection;
        public int DisplayIndex;
        public double WidthValue;
        public DataGridLengthUnitType WidthType;
    }

}
