![Icon](https://raw.githubusercontent.com/tomlm/Iciclecreek.Avalonia.Controls/main/icon.png)

# Iciclecreek.Avalonia.Controls
This is a package of UI controls for Avalonia UI.
* **ColumnsPanel** - A panel which lays out arranges children into dynamic columns

# Installation
Add the **Iciclecreek.Avalonia.Controls** package to your project

Update your xmlns 
```xaml
	xmlns:ice="using:Iciclecreek.Avalonia.Controls"
```

## ColumnsPanel Control
![ColumnsPanel](https://user-images.githubusercontent.com/17789481/284078002-ec829cbc-3bbb-4cdd-a4a4-e0cdb70df718.gif)
The ColumnsPanel control is a panel which dynamically lays out its children in left-to right order into columns.  

There are 2 modes of usage:
* **Dynamic Columns** - The **ColumnWidth** property defines the width of every column, and the number of columns is 
  dynamically set to the number of columns which fit the width of the control. The **MinColumns** and **MaxColumns** 
  properties can be used to control the number of columns.
* **Static Columns** - The **ColumnDefinitions** property defines column definitions (ex: ```'*, 2*, 500'```) 
  The number of columns is determined by the number of column definitions. **MinColumns**, **MaxColumns** are ignored. 
 

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
| **ColumnDefinitions** | null | A ColumnDefinitions string like ```'300,2*,*'``` which can be used to specify the width of each column. When ColumnDefinitions is set ColumnWidth, MinColumns, MaxColumns are ignored. |


## TextBlockSpinner Control
The TextBlockSpinner control is a loading/busy spinner which uses Text characters to create the spinner.
![spinners](https://github.com/user-attachments/assets/35750276-15e2-4941-b6b6-e2ea8fb87872)


### Usage
```xaml
<ice:TextBlockSpinner AnimationType="Arcs" IsActive="{Binding ...}" />
```

### Properties

| Property | Default | Description |
| --- | --- | --- |
| **IsActive** | true/false | When it's active it's visible and animating |
| **AnimationType** | enum | name of animationtype |

## BigTextBlockSpinner Control
The BigTextBlockSpinner control is a loading/busy spinner which uses Text characters to create the spinner but it can be arbitrary large.
![bigspinner](https://github.com/user-attachments/assets/e2ea1cf0-70a5-4b0a-b74d-238f0418e611)

### Usage
```xaml
<ice:BigTextBlockSpinner AnimationType="Dots" Rows="5" Columns="5" IsActive="{Binding ...}" />
```
| Property | Default | Description |
| --- | --- | --- |
| **IsActive** | true/false | When it's active it's visible and animating |
| **Rows** | number | defines number of characters in height |
| **Colunms** | number | defines number of characters in width |
| **Length** | number | defines how "long" the tail is in characters |
| **AnimationChar** | char| char to use to render the animation |


