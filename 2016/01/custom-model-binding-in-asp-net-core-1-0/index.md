

Early on in my career I was an accountant, which meant a lot of dollar signs and commas in the numbers I worked with in my daily tasks. The typical website or application I used, if built according to normal expectations, gave a nice message telling me what I’d done wrong when I accidentally entered a currency formatted value in a text input expecting a decimal. That was frustrating when I might be copy/pasting a value into the input from another system, or simply entering a value and absent-mindedly adding a dollar sign and comma. How smart is that software if it can’t understand that $1,340.12 == 1340.12?

For web applications, the first place most developers go when solving this issue is JavaScript. Possibly a CSS class added to the input identifying it as a currency value, with an event handler attached to the form submit that scrubs the input. Or when generating an Ajax call you can scrub it before adding it to the post data. But what if you want to stick with ASP.Net Core MVC  functionality and aren’t planning to use JavaScript for standard user input validation? Or if you are using a 3rd-party control to generate your user interface which doesn’t allow easily hooking into the form submit or Ajax calls? The solution we’re going to look at here is a custom model binder which will add currency input scrubbing by simply decorating a model property with an attribute.

The code in this article was previously found in a public GitHub repository at , and consists mostly of an MVC app - ASP.NET Core Web Application (.NET Framework) - with no authentication. The following items were also added to facilitate demoing our custom model binder:

- Session state and in-memory caching in project.json and Startup.cs so we can use a session variable to store list entries.
- Account model with two properties - Name and Balance.
- AccountController and two views - Index and Create.

Our Account model will start out with Required attributes for each property.

```
public class Account
{
    \[Required\]
    public string Name { get; set; }
    
    \[Required\]
    public decimal Balance { get; set; }
}
```

At this point if you attempt to enter $1,340.12 in the Balance field you will receive this message, which is returned when the SimpleTypeModelBinder fails to convert the string "$1,340.12" to a decimal.

 "Custom Model Binding in ASP.Net Core"

The result we are looking for is to validate the input as currency rather than a decimal, but we may have other needs for scrubbing input before model binding takes place so we’ll use an interface that describes our Scrub action.

```
public interface IScrubberAttribute
{
    object Scrub(string modelValue, out bool success);
}
```

Next we’ll add our CurrencyScrubberAttribute that will do the work of parsing the user input to see if it is a valid currency format. C#’s decimal.TryParse has an overload that takes a NumberStyle and CultureInfo, which is how we’ll do our currency validation. You’ll notice this only works with US currency ($) at this time, but would just require setting CultureInfo to handle other currencies. Our Scrub method has an out parameter that sends back a boolean value indicating success or failure. The model binder will use this parameter to indicate whether or not the binding for this property has succeeded.

```
\[AttributeUsage(AttributeTargets.Property)\]
public class CurrencyScrubberAttribute : Attribute, IScrubberAttribute
{
    private static NumberStyles \_currencyStyle = NumberStyles.Currency;
    private CultureInfo \_culture = new CultureInfo("en-US");

    public object Scrub(string modelValue, out bool success)
    {
        var modelDecimal = 0M;
        success = decimal.TryParse(
            modelValue,
            \_currencyStyle,
            \_culture,
            out modelDecimal
        );

        return modelDecimal;
    }
}
```

Using our new CurrencyScrubberAttribute, the Balance property now looks like this:

```
\[Required\]
\[CurrencyScrubber\]
public decimal Balance { get; set; }
```

Next we'll need to add a model binder.  In .Net Core 1.0 model binding is achieved by sending the model to a list of IModelBindingProviders until it finds one that will provide an IModelBinder to handle the binding.  In the case of a strongly typed model like Account, the ComplexTypeModelBinderProvider accepts the challenge, then creates a binder for each property.  In our case, both Name and Balance will be handled by the SimpleTypeModelBinder.  Pre-RTM, the binders were called sequentially until one of the binders dealt with the binding.  In the RTM release the IModelBindingProvider needs to make a decision up front whether it will handle the value, and if it does then it handles all binding work.

For our input scrubber our IModelBindingProvider looks like the following.  We will be looking for non-complex types which have an IScrubberAttribute.  If those conditions aren't met we return null and the framework moves on to the next provider.

```
public class ScrubbingModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        if (!context.Metadata.IsComplexType)
        {
            // Look for scrubber attributes
            var propName = context.Metadata.PropertyName;
            var propInfo = context.Metadata.ContainerType.GetProperty(propName);

            // Only one scrubber attribute can be applied to each property
            var attribute = propInfo.GetCustomAttributes(typeof(IScrubberAttribute), false).FirstOrDefault();
            if (attribute != null) return new ScrubbingModelBinder(context.Metadata.ModelType, attribute as IScrubberAttribute);
        }

        return null;
    }
}
```

Our model binder will be handling simple types that have an IScrubberAttribute, but if for any reason we aren't going to deal with the binding we will pass it to a SimpleTypeModelBinder to handle it.  If we do handle the model binding and the call to Scrub is successful then we'll pass back the new value and indicate the Task has completed.

```
public class ScrubbingModelBinder : IModelBinder
{
    IScrubberAttribute \_attribute;
    SimpleTypeModelBinder \_baseBinder;

    public ScrubbingModelBinder(Type type, IScrubberAttribute attribute)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));

        \_attribute = attribute as IScrubberAttribute;
        \_baseBinder = new SimpleTypeModelBinder(type);
    }

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

        // Check the value sent in
        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult != ValueProviderResult.None)
        {
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            // Attempt to scrub the input value
            var valueAsString = valueProviderResult.FirstValue;
            var success = true;
            var result = \_attribute.Scrub(valueAsString, out success);
            if (success)
            {
                bindingContext.Result = ModelBindingResult.Success(result);
                return Task.CompletedTask;
            }
        }

        // If we haven't handled it, then we'll let the base SimpleTypeModelBinder handle it
        return \_baseBinder.BindModelAsync(bindingContext);
    }
}
﻿
```

In Startup.cs we'll need to tell the framework about our IModelBinderProvider, and to make sure we get first choice on binding the model we'll put ours first.

```
services.AddMvc(config =>
{
    config.ModelBinderProviders.Insert(0, new ScrubbingModelBinderProvider());
});
```

While there are many ways on the client to scrub user input, it’s not as easy on the server. Custom model binders give you this power and allow you to handle for yourself the conversion from a string input to a model property. With the above approach, you can deal with any custom model binding needs your business rules may require and continue to use the latest MVC functionality without mixing in JavaScript.

_Written by Dan Haley_
