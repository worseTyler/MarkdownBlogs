

In my work for an electric utility, we have a WPF-based map control used to show the location of power lines, along with other devices such as transformers and points of service. We wanted to show a tooltip when a user clicks on one of the power lines to identify what is called the “feeder”, which essentially defines the source of power for the line. Although WPF controls support the use of a tooltip, in this case if we used the built-in tooltip it would display any time the mouse cursor is moved over the map. We only want to see it when the user clicks on a particular power line. Then we want to display a custom value in the tooltip-like popup at that point. The solution for this was two-fold.

First, mouse clicks are captured by the third-party map control. When we determine that it was a power line that was clicked on the map, we perform an upstream trace of the power line to find the feeder. The name of the feeder is what we want to see in a tooltip near the mouse position. We want the tooltip to remain until the user moves the mouse a short distance from where it was clicked. To know whether the mouse has been moved, and how far, I used a class called MouseTrackerDecorator, written by [Grigory](https://stackoverflow.com/users/435828/grigory) for [StackOverflow.com](https://stackoverflow.com/questions/6714663/wpf-how-do-i-bind-a-controls-position-to-the-current-mouse-position), which I will present later.

Second, I needed a way to display the custom tooltip which I did in XAML.

```csharp
<Grid>
  <Esri:MapView x:Name="_MyMapView" Map="{Binding Map}" Editor="{Binding Editor}"
          Map:MapViewService.MapView="{Binding MapViewService}" Cursor ="{Binding MapCursor}" >
    <!-- Remainder of Esri XAML removed -->
  </Esri:MapView>
  <Canvas Visibility="{Binding FeederTooltipVisibility}">
    <Border BorderBrush="Black" BorderThickness="1" CornerRadius="5" Background="LightGray" Opacity=".75"
        Canvas.Left="{Binding FeederTooltipLocation.X}"
        Canvas.Top="{Binding FeederTooltipLocation.Y}">
      <TextBlock Margin="5" FontWeight="Bold" Text="{Binding FeederTooltip, StringFormat='Feeder is {0}'}" />
    </Border>
  </Canvas>
</Grid>
```

I first wrapped the map control in a Grid control. Doing so places both the map and the tooltip at the same place on the screen. The tooltip is declared after the Esri map which gives it a lower Z order and allows it to show on top. Then, by adjusting the position and size of the tooltip, I can place it anywhere over the map. The tooltip itself is a Canvas. I also gave it a border with rounded corners and a slight bit of transparency. The TextBlock FeederTooltip value is set once we know the name of the feeder. In addition to the tooltip XAML, there’s one more thing needed to support the use of MouseTrackerDecorator. The entire outer Grid (the top level of the XAML) is wrapped with the MouseTrackerDecorator tag below to catch mouse movement events.

```csharp
<UserControl:MouseTrackerDecorator x:Name="_MouseTracker" >
  ...
</UserControl:MouseTrackerDecorator>
```

The code behind is fairly simple. When we get a click event and determine it’s a power line, we trace to the feeder and set up the tooltip values.

```csharp
FeederTooltip = electricLineId;
FeederTooltipLocation = parameters.Position;
FeederTooltipVisibility = Visibility.Visible;
MouseTrackerDecorator.OnMovement(SetFeederTooltipCollapsed, 50);
```

MouseTrackerDecorator.OnMovement sets up a call-back to SetFeederTooltipCollapsed when the mouse moves 50 pixels away from its original location. Here is SetFeederTooltipCollapsed and the bindable properties.

```csharp
private void SetFeederTooltipCollapsed()
{
  FeederTooltipVisibility = Visibility.Collapsed;
}

private string _FeederTooltip;
public string FeederTooltip
{
  get { return _FeederTooltip; }
  set { Set( () => FeederTooltip, ref _FeederTooltip, value ); }
}

private Point _FeederTooltipLocation;
public Point FeederTooltipLocation
{
  get { return _FeederTooltipLocation; }
  set
  {
    var x = value.X + 5;
    var y = value.Y - 30;
    var bounds = new Rectangle( 0, 0, (int) MyMap.ActualWidth, (int) MyMap.ActualHeight );
    if ( x > bounds.Right - 150 )
    {
      x = bounds.Right - 150;
    }
    if ( y < bounds.Top )
    {
      y = bounds.Top;
    }
    Set( () => FeederTooltipLocation, ref _FeederTooltipLocation, new Point( x, y ) );
  }
}

private Visibility _FeederTooltipVisibility;
public Visibility FeederTooltipVisibility
{
  get { return _FeederTooltipVisibility; }
  set { Set( () => FeederTooltipVisibility, ref _FeederTooltipVisibility, value ); }
}
```

Here is the result.

[![custom_tooltip_mouse_position_WPF](https://intellitect.com/wp-content/uploads/2015/12/custom_tooltip_mouse_position_WPF.png)](https://intellitect.com/wp-content/uploads/2015/12/custom_tooltip_mouse_position_WPF.png "Display Custom Mouse Tooltip on Large WPF Control")

And here is the MouseTrackerDecorator class.

```csharp
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

// Based on:
// https://stackoverflow.com/questions/6714663/wpf-how-do-i-bind-a-controls-position-to-the-current-mouse-position
// https://stackoverflow.com/users/435828/grigory

namespace MyApp.UserInterface.MVVM.View.UserControls
{
  public class MouseTrackerDecorator : Decorator
  {
    public MouseTrackerDecorator()
    {
      Instance = this;
    }

    static readonly DependencyProperty MousePositionProperty;
    static MouseTrackerDecorator()
    {
      MousePositionProperty = DependencyProperty.Register( "MousePosition", typeof( Point ), typeof( MouseTrackerDecorator ) );
    }

    private static MouseTrackerDecorator Instance { get; set; }

    public override UIElement Child
    {
      get
      {
        return base.Child;
      }
      set
      {
        if ( base.Child != null )
        {
          base.Child.MouseMove -= ControlledObjectMouseMove;
        }
        base.Child = value;
        base.Child.MouseMove += ControlledObjectMouseMove;
      }
    }

    public Point MousePosition
    {
      get
      {
        return (Point)GetValue( MouseTrackerDecorator.MousePositionProperty );
      }
      set
      {
        SetValue( MousePositionProperty, value );
      }
    }

    void ControlledObjectMouseMove( object sender, MouseEventArgs e )
    {
      Point p = e.GetPosition( base.Child );
      if ( _ClickAction != null && _ClickPosition.HasValue )
      {
        if ( Math.Abs( p.X - _ClickPosition.Value.X ) > _Distance ||
        Math.Abs( p.Y - _ClickPosition.Value.Y ) > _Distance )
        {
          _ClickAction();
          _ClickPosition = null;
          _ClickAction = null;
        }
      }
      MousePosition = p;
    }

    private static Point? _ClickPosition;
    private static Action _ClickAction;
    private static int _Distance;
    public static void OnMovement( Action action, int distance = 10 )
    {
      _ClickPosition = Instance.MousePosition;
      _ClickAction = action;
      _Distance = distance;
    }
  }
}
```

This class is “based on” Grigory’s version from StackOverflow because I added the ability to track the distance from the initial mouse click and make the one-time call-back. This is done by saving the click position and comparing that to the location of the mouse during movement events. Once the distance surpasses the distance requested, the Action method is called and the action is cancelled so it only occurs once.

In summary, I was able to display a custom tooltip and keep it visible until the mouse moves the desired distance from the original position by leveraging MouseTrackerDecorator.
