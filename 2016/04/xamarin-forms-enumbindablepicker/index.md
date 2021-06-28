
One of the controls missing from Xamarin Forms is a “BindablePicker” that allows you to dynamically bind an ItemsSource and SelectedItem from your view model. Fortunately, the Xamarin forums provide a solution to this problem found here:

https://forums.xamarin.com/discussion/30801/xamarin-forms-bindable-picker

The original implementation of this BindablePicker worked great as long as you had a list of strings as your ItemSource. But what if you have a list of objects and want to set a DisplayProperty that is shown to the user? Xamarin forums to the rescue again… scrolling down in that same post, there is a solution that allows you to do just that.

https://forums.xamarin.com/discussion/comment/110480/#Comment\_110480

# Simple Enums

Recently, however, I came across the need to have a bindable picker that would allow the user to select a value from an enum. Not finding just what I wanted, I used the BindablePicker in the Xamarin forums as a base, and built an EnumBindablePicker.

public class EnumBindablePicker<T> : Picker where T : struct
{
   public EnumBindablePicker()
   {
       SelectedIndexChanged += OnSelectedIndexChanged;
       //Fill the Items from the enum
       foreach (var value in Enum.GetValues(typeof(T)))
       {
           Items.Add(value.ToString());
       }
   }

   public static BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(T), typeof(EnumBindablePicker<T>), default(T), propertyChanged: OnSelectedItemChanged, defaultBindingMode: BindingMode.TwoWay);

   public T SelectedItem
   {
       get
       {
           return (T)GetValue(SelectedItemProperty);
       }
       set
       {
           SetValue(SelectedItemProperty, value);
       }
   }
   
   private void OnSelectedIndexChanged(object sender, EventArgs eventArgs)
   {
       if (SelectedIndex < 0 || SelectedIndex > Items.Count - 1)
       {
           SelectedItem = default(T);
       }
       else
       {
           SelectedItem = (T)Enum.Parse(typeof(T), Items\[SelectedIndex\]);
       }
   }
   
   private static void OnSelectedItemChanged(BindableObject bindable, object oldvalue, object newvalue)
   {
       var picker = bindable as EnumBindablePicker<T>;
       if (newvalue != null)
       {
           picker.SelectedIndex = picker.Items.IndexOf(newvalue.ToString());
       }
   }
}

This class uses C# generics to automatically generate the list of available items in the picker based on the generic enum type. The SelectedItem getter and OnSelectedIndexChanged use Enum.Parse to convert the string value back to a properly typed enum value.

In your XAML, you must specify the enum type as a parameter to the generic class like this:

<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
	xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
	xmlns:controls="clr-namespace:EnumBindablePickerSample.Controls;assembly=EnumBindablePickerSample"
	xmlns:enums="clr-namespace:EnumBindablePickerSample.Enumerations;assembly=EnumBindablePickerSample"
	x:Class="EnumBindablePickerSample.Views.PickerSamplePage"
	Title="Sample Page">
	<StackLayout>
		<controls:EnumBindablePicker x:TypeArguments="enums:SampleEnum" SelectedItem="{Binding SelectedEnum}"></controls:EnumBindablePicker>
		<Label Text="{Binding SelectedEnum}"></Label>
	</StackLayout>
</ContentPage>

I also added a Label below the EnumBindablePicker to verify that the user selected value is updating properly in the associated view model.

![EnumBindablePicker - Sample Page](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2016/04/xamarin-forms-enumbindablepicker/images/EnumBindablePicker-Sample-Page.png)

# Enums with user friendly descriptions

So this works great if your enum names are user friendly. Oftentimes, we want to display one value to the user, but the internal enum value is something different. One way to accomplish this is to use an attribute on the enum. In my project, using a PCL, System.ComponentModel.DescriptionAttribute is not available, but I was able to use System.ComponentModel.DataAnnotations.DisplayAttribute.

using System.ComponentModel.DataAnnotations;

namespace EnumBindablePickerSample.Enumerations
{
   public enum SampleEnumWithDescription
   {
       \[Display(Description = "First")\]
       One,
       \[Display(Description = "Second")\]
       Two,
       \[Display(Description = "Third")\]
       Three
   }
}

In order to support this, the EnumBindablePicker had to be updated to look for this DisplayAttribute:

public class EnumBindablePicker<T> : Picker where T : struct
{
   public EnumBindablePicker()
   {
       SelectedIndexChanged += OnSelectedIndexChanged;
       //Fill the Items from the enum
       foreach (var value in Enum.GetValues(typeof(T)))
       {
           Items.Add(GetEnumDescription(value));
       }
   }
 
   public static BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(T), typeof(EnumBindablePicker<T>), default(T), propertyChanged: OnSelectedItemChanged, defaultBindingMode: BindingMode.TwoWay);
 
   public T SelectedItem
   {
       get
       {
           return (T)GetValue(SelectedItemProperty)
       }
       set
       {
           SetValue(SelectedItemProperty, value);
       }
   }
 
   private void OnSelectedIndexChanged(object sender, EventArgs eventArgs)
   {
       if (SelectedIndex < 0 || SelectedIndex > Items.Count - 1)
       {
           SelectedItem = default(T);
       }
       else
       {
           //try parsing, if using description this will fail
           T match;
           if (!Enum.TryParse<T>(Items\[SelectedIndex\], out match))
           {
               //find enum by Description
               match = GetEnumByDescription(Items\[SelectedIndex\]);
           }
           SelectedItem = (T)Enum.Parse(typeof(T), match.ToString());
       }
   }
 
   private static void OnSelectedItemChanged(BindableObject bindable, object oldvalue, object newvalue)
   {
       var picker = bindable as EnumBindablePicker<T>;
       if (newvalue != null)
       {
           picker.SelectedIndex = picker.Items.IndexOf(GetEnumDescription(newvalue));
       }
   }
 
   private static string GetEnumDescription(object value)
   {
       string result = value.ToString();
       DisplayAttribute attribute = typeof(T).GetRuntimeField(value.ToString()).GetCustomAttributes<DisplayAttribute>(false).SingleOrDefault();
 
       if (attribute != null)
           result = attribute.Description;
 
       return result;
   }
 
   private T GetEnumByDescription(string description)
   {
       return Enum.GetValues(typeof(T)).Cast<T>().FirstOrDefault(x => string.Equals(GetEnumDescription(x), description));
   }
}

This now displays the proper DisplayAttribute information to the user in the picker control:

![EnumBindablePick - Sample Page 2](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2016/04/xamarin-forms-enumbindablepicker/images/EnumBindablePick-Sample-Page-2.png)

# Localized Enum Strings

Again, this is great if you only have one language to support. But what about an app that supports multiple languages?

There are several ways to solve the problem of retrieving the proper localized string. For purposes of this sample, I chose to put my localized strings in a resx file and use a convention {EnumType}\_{EnumValue} to find the correct value.

![EnumBindablePick - Enums](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2016/04/xamarin-forms-enumbindablepicker/images/EnumBindablePick-Enums.png)

With a slight modification to the EnumBindablePicker, we can support this functionality as well as looking for the DisplayAttribute.

private static string GetEnumDescription(object value)
{
   string result = value.ToString();
   DisplayAttribute attribute = typeof(T).GetRuntimeField(value.ToString()).GetCustomAttributes(typeof(DisplayAttribute), false).SingleOrDefault() as DisplayAttribute;

   if (attribute != null)
       result = attribute.Description;
   else
   {
       //is there a resource entry?
       var match = Resource1.ResourceManager.GetString($"{typeof(T).Name}\_{((int)value).ToString(CultureInfo.InvariantCulture)}");
       if (!string.IsNullOrWhiteSpace(match))
           result = match;
   }
   return result;
}

![EnumBindablePick - Sample Page 3](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2016/04/xamarin-forms-enumbindablepicker/images/EnumBindablePick-Sample-Page-3.png)

Now we have a Bindable Picker control that works natively with enums and can display a user-friendly description based on attributes or strings in a resource file.

Sample code can be found here: [https://github.com/creasewp/EnumBindablePickerSample](https://github.com/creasewp/EnumBindablePickerSample)
