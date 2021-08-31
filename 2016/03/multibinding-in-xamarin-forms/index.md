

## Multi Binding in Xamarin.Forms
#
The current release of Xamarin.Forms does not contain an implementation for a MultiBinding object. For those of us who have a strong WPF background, this is a feature that would be very beneficial. With a little work, it is possible for us to implement our own MultiBinding class using the current Xamarin.Forms framework.

**Creating a basic MultiBinding**

The existing Binding class  is the glue that links any property on the binding’s source to a BindableProperty on the target [BindableObject](https://developer.xamarin.com/api/type/Xamarin.Forms.BindableObject/). Typically, this will be a property on a VisualElement. A simple binding might look like this:

```
<Label Text="{Binding Title}" />
```

This is great when we only want to bind a single value, but what about when we have multiple values we want to use. This is the problem that a MultiBinding is designed to solve when combined with a string format or a converter.  
This is where we will start with our own MultiBinding class.

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Proxy;
using Xamarin.Forms.Xaml;

[ContentProperty(nameof(Bindings))]
public class MultiBinding : IMarkupExtension
{
    public IList Bindings { get; } = new List();
 
    public string StringFormat { get; set; }
 
    public Binding ProvideValue(IServiceProvider serviceProvider)
    {
        return null;
    }
 
    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
    {
        return ProvideValue(serviceProvider);
    }
}
```

Since we can’t derive from the Xamarin.Forms Binding (sealed class) nor BindingBase (constructor is declared internal) classes, we will declare our MultiBinding class as a [IMarkupExtension<Binding>](https://docs.microsoft.com/en-us/dotnet/api/xamarin.forms.xaml.imarkupextension-1?view=xamarin-forms). It [is important](https://bugzilla.xamarin.com/37/37622/bug.html) to use IMarkupExtenion<T> rather than [IMarkupExtension](https://developer.xamarin.com/api/type/Xamarin.Forms.Xaml.IMarkupExtension/) if you are going to use the [XAML Compile](https://developer.xamarin.com/guides/xamarin-forms/user-interface/xaml-basics/xamlc/) feature.

In the ProvideValue method we will create the bindings and links that cause the MultiBinding to work. We will then monitor the dynamic BindableProperties we create for each binding in the Bindings collection and update our own internal value when any of them change. We will create a Binding using this internal value as its source and return it as the MultiBinding.

```csharp
private BindableObject _target;
private readonly InternalValue _internalValue = new InternalValue();
private readonly IList _properties = new List();
       
public Binding ProvideValue(IServiceProvider serviceProvider)
{
    if (string.IsNullOrWhiteSpace(StringFormat)) throw new InvalidOperationException($"{nameof(MultiBinding)} requires a {nameof(StringFormat)}");
 
    //Get the object that the markup extension is being applied to
    var provideValueTarget = (IProvideValueTarget)serviceProvider?.GetService(typeof(IProvideValueTarget));
    _target = provideValueTarget?.TargetObject as BindableObject;
 
    if (_target == null) return null;
 
    foreach (Binding b in Bindings)
    {
        var property = BindableProperty.Create($"Property-{Guid.NewGuid().ToString("N")}", typeof (object),
            typeof (MultiBinding), default(object), propertyChanged: (_, o, n) => SetValue());
        _properties.Add(property);
        _target.SetBinding(property, b);
    }
    SetValue();
 
    var binding = new Binding
    {
        Path = nameof(InternalValue.Value),
        Source = _internalValue
    };
 
    return binding;
}
 
private void SetValue()
{
    if (_target == null) return;
    var values = _properties.Select(_target.GetValue).ToArray();
    if (!string.IsNullOrWhiteSpace(StringFormat))
    {
        _internalValue.Value = string.Format(StringFormat, values);
        return;
    }
    _internalValue.Value = values;
}
 
private sealed class InternalValue : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
 
    private object _value;
    public object Value
    {
        get { return _value; }
        set
        {
            if (!Equals(_value, value))
            {
                _value = value;
                OnPropertyChanged();
            }
        }
    }
 
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

Looking a bit deeper at the ProvideValue method, we first get access to the object that the MultiBinding is getting applied to by using the IProvideValueTarget service interface. We use this object to make calls to SetBinding and GetValue. The advantage of using this object is that our bindings will get the same BindingContext as the object that the MultiBinding is applied to.

For each of our bindings we create a bindable property as their target. For our internal value there is a private inner class that implements INPC. This property could have been left on the MultiBinding itself, but since it needs to be public, I find that hiding it away in an internal class keeps the API of the MultiBinding cleaner.

**Adding IMultiValueConverter**

If we run the app, we can see the MultiBinding works with string formats, but this is only a fraction of the functionality that the [WPF MultiBinding](https://msdn.microsoft.com/en-us/library/system.windows.data.multibinding(v=vs.110).aspx) provides. In WPF we can specify an [IMultiValueConverter](https://msdn.microsoft.com/en-us/library/system.windows.data.imultivalueconverter(v=vs.110).aspx) to synthesize the bindings’ values into a single value. Let’s add similar functionality to our own MultiBinding class.

First we declare the converter interface. Most of the time MultiBindings are only used to convert from the source to the target, so we will only concern ourselves with one-way conversions.

```csharp
public interface IMultiValueConverter
{
    object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);
}

```

Next, we will add properties for the converter and its parameter to the MultiBinding, and update the relevant portions of the ProvideValue method.

```csharp
public IMultiValueConverter Converter { get; set; }
 
public object ConverterParameter { get; set; }
 
public Binding ProvideValue(IServiceProvider serviceProvider)
{
    if (string.IsNullOrWhiteSpace(StringFormat) && Converter == null)
        throw new InvalidOperationException($"{nameof(MultiBinding)} requires a {nameof(Converter)} or {nameof(StringFormat)}");
 
    ...
 
    var binding = new Binding
    {
        Path = nameof(InternalValue.Value),
        Converter = new MultiValueConverterWrapper(Converter, StringFormat),
        ConverterParameter = ConverterParameter,
        Source = _internalValue
    };
 
    return binding;
}
```

In an effort to mimic [WPF behavior](https://msdn.microsoft.com/en-us/library/system.windows.data.bindingbase.stringformat(v=vs.110).aspx), the converter is applied prior to the string format. We need to modify our SetValue method. Rather than applying the string format there, we will do it in our new MultiValueConverterWrapper.

```csharp
private void SetValue()
{
    if (_target == null) return;
    _internalValue.Value = _properties.Select(_target.GetValue).ToArray();
}
```

In this new structure we will always pass an array of values from our bound properties to the internal value. We then apply the string format and converter inside of the MultiValueConverterWrapper.

```csharp
private sealed class MultiValueConverterWrapper : IValueConverter
{
    private readonly IMultiValueConverter _multiValueConverter;
    private readonly string _stringFormat;
 
    public MultiValueConverterWrapper(IMultiValueConverter multiValueConverter, string stringFormat)
    {
        _multiValueConverter = multiValueConverter;
        _stringFormat = stringFormat;
    }
 
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (_multiValueConverter != null)
        {
            value = _multiValueConverter.Convert(value as object[], targetType, parameter, culture);
        }
        if (!string.IsNullOrWhiteSpace(_stringFormat))
        {
            var array = value as object[];
            // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
            if (array != null)
            {
                value = string.Format(_stringFormat, array);
            }
            else
            {
                value = string.Format(_stringFormat, value);
            }
        }
        return value;
    }
 
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
```

This allows us to create our own multi value converters that can translate an array of objects into our desired type. In the following example, our value converter simply selects the first non-null value from the array or the converter’s parameter if there are no non-null values. It then takes the value and applies a string format to it.

```csharp
<VisualElement.Resources>
 <ResourceDictionary>
   <local:FirstNotNullConverter x:Key="FirstNotNullConverter" />
 </ResourceDictionary>
</VisualElement.Resources>

...

<Label>
 <Label.Text>
   <local:MultiBinding StringFormat="Hello {0}" Converter="{StaticResource FirstNotNullConverter}" ConverterParameter="(No Name)">
     <Binding Path="Person1" />
     <Binding Path="Person2" />
   </local:MultiBinding>
 </Label.Text>
</Label>
```

**Handling triggers, styles, and setters**

It is worth pointing out that our MultiBinding, in its current state, will fail if it is used inside of a [setter](https://developer.xamarin.com/api/type/Xamarin.Forms.Setter/) (applied either [directly on a VisualElement's](https://developer.xamarin.com/api/property/Xamarin.Forms.VisualElement.Triggers/) trigger or [as part of a Style](https://developer.xamarin.com/api/property/Xamarin.Forms.Style.Triggers/)). This is because in we are using the TargetObject from the IProvideValueTarget service. Because a setter is not a [BindableObject](https://developer.xamarin.com/api/type/Xamarin.Forms.BindableObject/) the cast fails and we have no BindableObject to use to set the bindings on. If all you care about is having a MultiBinding that you can apply directly to elements, you can stop here.

**The following options are very hacky, kittens will be killed, and** [**you may be devoured by a raptor**](https://www.xkcd.com/292/)**.**

With that warning out of the way, let’s continue.

Handling the case where the MultiBinding is used directly inside of a VisualElement’s trigger can be solved by accessing some internal members. The two classes that currently exist in Xamarin.Forms that implement the IProvideValueTarget interface also implement another internal interface IProvideParentValues interface.

```csharp
namespace Xamarin.Forms.Xaml
{
    internal interface IProvideParentValues : IProvideValueTarget
    {
        IEnumerable ParentObjects { get; }    
    }
}
```

Using reflection, we could retrieve the parent objects from our IProvideValueTarget. By doing this we can simply grab the first BindableObject that we find in the ParentObjects. This will effectively handle the case where the MultiBinding is used within a Setter that is a child of the target element:

```csharp
<Label> 
  <Label.Triggers>
    <DataTrigger Binding="{Binding HasFullName}" Value="True" TargetType="Label">
      <Setter Property="Text">
        <Setter.Value>
          <local:MultiBinding StringFormat="{}{0}, {1}">
            <Binding Path="LastName" />
            <Binding Path="FirstName" />
          </local:MultiBinding>
        </Setter.Value>
      </Setter>
    </DataTrigger>
  </Label.Triggers>
</Label>
```

This does **not** solve the case where the setter is not a child of the element (such is the case when used inside of a style):

```csharp
<Style TargetType="Label" x:Key="LabelStyle">
  <Setter Property="Text" TargetType="Label">
    <Setter.Value>
      <local:MultiBinding StringFormat="{}{0}, {1}">
        <Binding Path="LastName" />
        <Binding Path="FirstName" />
      </local:MultiBinding>
    </Setter.Value>
  </Setter>
</Style>
...
<Label Style="{StaticResource LabelStyle}" />
```

This case is much more complicated. Because multiple elements can share a single style, when a setter’s value derives from [BindingBase](https://developer.xamarin.com/api/type/Xamarin.Forms.BindingBase/) it creates a _shallow clone_ of the binding and applies the cloned binding to the target element. This is a problem for our current implementation because it would apply a shallow clone of the binding returned from the ProvideValue method. However, there is a solution.[Examining the manifest](https://msdn.microsoft.com/en-us/library/ceats605(v=vs.110).aspx) of the Xamarin.Forms.Core assembly we can see that contains several [InternalsVisibleToAttributes](https://msdn.microsoft.com/en-us/library/system.runtime.compilerservices.internalsvisibletoattribute(v=vs.110).aspx). Specifically we are going to pick on this one:

```csharp
[assembly: InternalsVisibleTo("Xamarin.Forms.Core.UnitTests")]
```

We can create our own proxy PCL project that has an Assembly name of “Xamarin.Forms.Core.UnitTests”. This project can be referenced by our main PCL project, and will have access to all of the internal members of the Xamarin.Forms.Core assembly.

![Screen Shot 2016-03-15 at 11.40.37 AM](https://intellitect.com/wp-content/uploads/2016/03/Screen-Shot-2016-03-15-at-11.40.37-AM.png "MultiBinding in Xamarin.Forms")

This now allows us to derive from [BindingBase](https://developer.xamarin.com/api/type/Xamarin.Forms.BindingBase/) and implement a true MultiBindings class.

```csharp
[ContentProperty(nameof(Bindings))]
public class MultiBinding : BindingBase
{
    private readonly BindingExpression _bindingExpression;
    private readonly InternalValue _internalValue = new InternalValue();
    private readonly IList _properties = new List();
    private bool _isApplying;
    private IMultiValueConverter _converter;
    private object _converterParameter;
    public IList Bindings { get; } = new List();
    
    public IMultiValueConverter Converter
    {
        get { return _converter; }
        set
        {
            ThrowIfApplied();
            _converter = value;
        }
    }
    
    public object ConverterParameter
    {
        get { return _converterParameter; }
        set
        {
            ThrowIfApplied();
            _converterParameter = value;
        }
    }
    ...
}
```

Rather than using a markup extension we can simply derive from BindingBase directly. Because we are deriving from BindingBase we no longer need our own StringFormat property since it is declared on the base class. The Converter and ConverterParameter properties are now implemented with a backing field so that we can throw if they get modified after the binding is applied.

```csharp
public MultiBinding()
{
    Mode = BindingMode.OneWay;
    _bindingExpression = new BindingExpression(this, nameof(InternalValue.Value));
}
```

We also have access to the internal BindingExpression class. We will use it in a similar manner as Xamarin’s [Binding](https://developer.xamarin.com/api/type/Xamarin.Forms.Binding/) class to get similar behavior.

```csharp
internal override void Apply(object context, BindableObject bindObj, BindableProperty targetProperty,
    bool fromBindingContextChanged = false)
{
    if (Mode != BindingMode.OneWay)
        throw new InvalidOperationException($"{nameof(MultiBinding)} only supports {nameof(Mode)}.{nameof(BindingMode.OneWay)}");

    base.Apply(context, bindObj, targetProperty, fromBindingContextChanged);

    _isApplying = true;
    Properties = new BindableProperty[Bindings.Count];
    int i = 0;
    foreach (BindingBase binding in Bindings)
    {
        var property = BindableProperty.Create($"{nameof(MultiBinding)}Property-{Guid.NewGuid():N}", typeof(object),
            typeof(MultiBinding), default(object), propertyChanged: (bindableObj, o, n) =>
            {
                SetInternalValue(bindableObj);
            });
        Properties[i++] = property;
        bindObj.SetBinding(property, binding);
    }
    _isApplying = false;
    SetInternalValue(bindObj);

    _bindingExpression.Apply(_internalValue, bindObj, targetProperty);
}

internal override void Apply(bool fromTarget)
{
    base.Apply(fromTarget);
    foreach (BindingBase binding in Bindings)
    {
        binding.Apply(fromTarget);
    }
    _bindingExpression.Apply(fromTarget);
}

internal override void Unapply(bool fromBindingContextChanged = false)
{
    base.Unapply(fromBindingContextChanged);
    foreach (BindingBase binding in Bindings)
    {
        binding.Unapply(fromBindingContextChanged);
    }
    Properties = null;
    _bindingExpression?.Unapply();
}

internal override object GetSourceValue(object value, Type targetPropertyType)
{
    if (Converter != null)
        value = Converter.Convert(value as object[], targetPropertyType, ConverterParameter, CultureInfo.CurrentUICulture);
    if (StringFormat != null && value != null)
    {
        // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
        if (value is object[] array)
        {
            value = string.Format(StringFormat, array);
        }
        else
        {
            value = string.Format(StringFormat, value);
        }
    }
    return value;
}

internal override object GetTargetValue(object value, Type sourcePropertyType)
{
    throw new InvalidOperationException($"{nameof(MultiBinding)} only supports {nameof(Mode)}.{nameof(BindingMode.OneWay)}");
}

private void SetInternalValue(BindableObject source)
{
    if (source == null || _isApplying) return;
    _internalValue.Value = source.GetValues(Properties);
}
```

The apply and unapply methods are invoked as the MultiBinding is applied to an object, or when its binding context changes. Because we now have knowledge of when we are being applied to an element, we can properly create our child properties and bindings. The child bindings use the actual context object, while the MultiBinding itself uses the InternalValue class as its source. Our previous MultiValueConverterWrapper is replaced by similar logic inside of the GetSourceValue method that is used to provide a value from the MultiBinding to the target property.

```csharp
internal override BindingBase Clone()
{
    var rv = new MultiBinding
    {
        Converter = Converter,
        ConverterParameter = ConverterParameter,
        StringFormat = StringFormat
    };
    rv._internalValue.Value = _internalValue.Value;

    foreach (var binding in Bindings.Select(x => x.Clone()))
    {
        rv.Bindings.Add(binding);
    }
    return rv;
}
```

Finally, the clone method solves the problem of the MultiBinding being used inside of a style’s setter. Because bindings are cloned when a style is applied to an element, deriving from BindingBase and implementing the Clone method allows the MultiBinding to work correctly.

Though Xamarin.Forms is certainly maturing, it still lacks some of the functionality that WPF developers would expect. It would be ideal if the library exposed more functionality and opportunities for developers to extend and implement these types of features. Hopefully this will improve in the future.Full code can be found on [Github](https://github.com/Keboo/Xamarin.Forms.Proxy).The code for the simple binding (the one that won’t work inside of setters) can be found [here](https://gist.github.com/Keboo/0d6e42028ea9e4256715).
