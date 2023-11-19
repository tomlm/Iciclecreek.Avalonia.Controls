![Icon](https://github.com/tomlm/Iciclecreek.Avalonia.Controls/raw/main/icon.png)

# Iciclecreek.Avalonia.Controls
This is a package of UI controls for Avalonia UI.

# Installation
Add the **Iciclecreek.Avalonia.Controls** package to your project

Update your xmlns 
```xaml
	xmlns:ice="using:Iciclecreek.Avalonia.Controls"
```

## ColumnsPanel Control
The ColumnsPanel control is a panel which dynamically lays out its children in left-to right order into fixed width columns.  Columns will be added or removed to fill the horizontal space of the control. If there is not enough space for MinColumns then the item wills be scaled down to fit proportionally.
![ColumnsPanel](https://github.com/tomlm/Iciclecreek.Avalonia.Controls/assets/17789481/ec829cbc-3bbb-4cdd-a4a4-e0cdb70df718)


### Usage
```xaml
<ice:ColumnsPanel ColumnWidth="100">
	<TextBlock Text="Column 1" />
	<TextBlock Text="Column 2" />
	<TextBlock Text="Column 3" />
    ...
</ice:ColumnsPanel>
```

Example of a columns panel in a items control
```xaml
<ItemsControl Items="{Binding Items}">
	<ItemsControl.ItemsPanel>
		<ItemsPanelTemplate>
			<ice:ColumnsPanel ColumnWidth="300" />
		</ItemsPanelTemplate>
	</ItemsControl.ItemsPanel>
	<ItemsControl.ItemTemplate>
		<DataTemplate>
			<Image Source="{Binding Source}" />
		</DataTemplate>
	</ItemsControl.ItemTemplate>
</ItemsControl>
```
### Properties

| Property | Default | Description |
| --- | --- | --- |
| **ColumnWidth** | 300 | The width of each column. |
| **MinColumns** | 1 | The minimum number of columns. If ColumnWidth*MinColumns is not possible the column width will scale to fit.|
| **MaxColumns** | Int.MaxValue | The maximum number of columns. |
| **Gap** | 0 | The vertical gap between each item in a column |
| **ColumnGap** | 0 | The horizontal gap between each column|

