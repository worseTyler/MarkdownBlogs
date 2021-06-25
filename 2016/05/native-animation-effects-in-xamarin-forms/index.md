---
title: "Native animation effects in Xamarin.Forms"
date: "2016-05-12"
categories: 
  - "net"
  - "blog"
  - "xamarin"
---

Xamarin.Forms (XF) provides a [cross platform API](https://developer.xamarin.com/api/type/Xamarin.Forms.ViewExtensions/) for doing animations. This makes it very easy to do simple animations of XF elements. There is, however, a downside. The XF animations do not make use of native platform APIs, rather they animate properties on the XF elements. This can cause significant overhead, since each change to the property will trigger a property changed event that the [native render](https://developer.xamarin.com/guides/xamarin-forms/custom-renderer/) will update the corresponding property on the native UI element. For small simple animations this is fine, but it does not take much before you will notice a significant degradation in performance between the XF animations and animations done with native APIs.

[Xamarin.Forms 2.1](https://developer.xamarin.com/releases/xamarin-forms/xamarin-forms-2.1/2.1.0/) introduced [effects](https://developer.xamarin.com/guides/xamarin-forms/effects/) that can easily be attached to the elements we want to animate. Because this can be done using XAML, this approach also works well if you are following the [MVVM pattern](https://developer.xamarin.com/guides/xamarin-forms/user-interface/xaml-basics/data_bindings_to_mvvm/). We can use these effects as a bridge to implement our own animations that leverage native platform animations to get true native performance.

# Xamarin.Forms PCL project

To begin, we will create an empty page with some content to animate up and down when a button is clicked. In this case the Overlay is half the size of the page, and starts off the bottom of the screen.

<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:XfAnimationEffects;assembly=XfAnimationEffects"
             x:Class="XfAnimationEffects.MainPage">
  <RelativeLayout BackgroundColor="White">
    <Button RelativeLayout.WidthConstraint="{ConstraintExpression Type=RelativeToParent, Property=Width}"
            RelativeLayout.XConstraint="{ConstraintExpression Type=Constant}"
            RelativeLayout.YConstraint="{ConstraintExpression Type=Constant}" Text="Show Overlay" Clicked="ShowOverlayClicked"/>
    <RelativeLayout RelativeLayout.WidthConstraint="{ConstraintExpression Type=RelativeToParent, Property=Width}"
                    RelativeLayout.HeightConstraint="{ConstraintExpression Type=RelativeToParent, Property=Height, Factor=0.5}"
                    RelativeLayout.XConstraint="{ConstraintExpression Type=Constant}"
                    RelativeLayout.YConstraint="{ConstraintExpression Type=RelativeToParent, Property=Height}" x:Name="Overlay" BackgroundColor="Pink">
      <Label Text="Some super awesome content goes here" />
    </RelativeLayout>
  </RelativeLayout>
</ContentPage>

Next we need to [create the effect](https://developer.xamarin.com/guides/xamarin-forms/effects/creating/) to reference in the XAML.

namespace XfAnimationEffects 
{ 
    public class VerticalSlideEffect : RoutingEffect 
    { 
        public const string ID = nameof(XfAnimationEffects) + "." + nameof(VerticalSlideEffect);  
        public VerticalSlideEffect()  
            : base(ID) 
        {
    
        } 
    } 
}

Next we need a property that we can toggle when the button is clicked to initiate the animation. To keep things simple, we will add an attached property to the existing VerticalSlideEffect. This attached property could be defined anywhere, but doing it here keeps all of the related code together.

public static readonly BindableProperty IsShownProperty = BindableProperty.CreateAttached(ID + ".IsShown", typeof(bool), typeof(VerticalSlideEffect), default(bool)); 

public static bool GetIsShown(BindableObject obj) 
{ 
    return (bool)obj.GetValue(IsShownProperty); 
} 

public static void SetIsShown(BindableObject obj, bool value) 
{ 
    obj.SetValue(IsShownProperty, value); 
}

The click handler for the button simply toggles the attached property on the Overlay.

private void ShowOverlayClicked(object sender, EventArgs e)
{ 
    VerticalSlideEffect.SetIsShown(Overlay, !VerticalSlideEffect.GetIsShown(Overlay)); 
}

Finally, we attach the effect to the Overlay.

<RelativeLayout ... x:Name="Overlay"> 
  <RelativeLayout.Effects> 
    <local:VerticalSlideEffect /> 
  </RelativeLayout.Effects> 
  ... 
</RelativeLayout>

# Xamarin.iOS

The effect on iOS is very straightforward. The effect simply watches for the IsShown attached property to change and then starts the appropriate animation.

\[assembly: ResolutionGroupName(nameof(XfAnimationEffects))\] 
\[assembly: ExportEffect(typeof(VerticalSlideEffect), nameof(VerticalSlideEffect))\] 

namespace XfAnimationEffects.iOS 
{ 
  public class VerticalSlideEffect : PlatformEffect 
  { 
    protected override void OnAttached() 
    { 
      Element.PropertyChanged += OnPropertyChanged; 
    } 

    protected override void OnDetached() 
    { 
      Element.PropertyChanged -= OnPropertyChanged; 
    } 

    private async void OnPropertyChanged(object sender, PropertyChangedEventArgs e) 
    { 
      if (e.PropertyName == XfAnimationEffects.VerticalSlideEffect.IsShownProperty.PropertyName) 
      { 
        var visualElement = Element as VisualElement; 

        if (visualElement != null) 
        { 
          if (XfAnimationEffects.VerticalSlideEffect.GetIsShown(visualElement)) 
          { 
            await AnimateIn(); 
          } 
          else 
          { 
            await AnimateOut(); 
          } 
        } 
      } 
    }  
  }
}

The AnimateIn and AnimateOut methods move the native container and then set the corresponding XF property. The built-in renderers on iOS use the VisualElementTracker to update many of the common properties. For the XF TranslationY property, [it updates the transform on the UIView’s layer](https://github.com/xamarin/Xamarin.Forms/blob/2d9288eee6e6f197364a64308183725e7bd561f9/Xamarin.Forms.Platform.iOS/VisualElementTracker.cs). To match that behavior, the effect animates the same property. After the animation completes, the XF property is set. This will trigger a property changed event and the native renderer will update the native property, but because this is the same property that just finished animating there is no visible change.

private async Task AnimateIn() 

{ 
    var target = -Container.Frame.Height; 
    await UIView.AnimateNotifyAsync(2, 
        () => 
        { 
            CATransform3D transform = Container.Layer.Transform; 
            transform.m42 = target; 
            Container.Layer.Transform = transform; 
        }); 
    var visualElement = Element as VisualElement; 
    if (visualElement != null) 
    { 
        visualElement.TranslationY = target; 
    } 
} 
private async Task AnimateOut() 
{ 
    await UIView.AnimateNotifyAsync(2, 
        () => 
        { 
            CATransform3D transform = Container.Layer.Transform; 
            transform.m42 = 0; 
            Container.Layer.Transform = transform; 
        }); 
    var visualElement = Element as VisualElement; 
    if (visualElement != null) 
    { 
        visualElement.TranslationY = 0; 
    } 
}

# Xamarin.Android

The animation effect on Android is very similar to the one for iOS; duplicate parts have been omitted for brevity.  
Android requires a bit more setup to perform the same slide animation. The [value animator](https://developer.android.com/reference/android/animation/ValueAnimator.html) will produce a stream of values that can be used to animate a view. Once again looking at the visual element tracker for Android we see that it [updates the TranslationY property on the ViewGroup](https://github.com/xamarin/Xamarin.Forms/blob/2d9288eee6e6f197364a64308183725e7bd561f9/Xamarin.Forms.Platform.Android/VisualElementTracker.cs). This is the property that we will update during the animation. Then, after it completes, update the TranslationY on the XF element.  
There is one final difference worth noting. We have to convert the screen pixels to logical pixels when setting the correct value back into the XF element. This is done using the [FromPixels](https://github.com/xamarin/Xamarin.Forms/blob/2d9288eee6e6f197364a64308183725e7bd561f9/Xamarin.Forms.Platform.Android/ContextExtensions.cs) method.

\[assembly: ResolutionGroupName(nameof(XfAnimationEffects))\] 
\[assembly: ExportEffect(typeof(VerticalSlideEffect), nameof(VerticalSlideEffect))\] 

namespace XfAnimationEffects.Droid 
{ 
    public class VerticalSlideEffect : PlatformEffect 
    { 
        private readonly ValueAnimator \_animator; 

        public VerticalSlideEffect() 
        { 
            \_animator = ValueAnimator.OfFloat(0); 
            \_animator.SetInterpolator(new Android.Views.Animations.LinearInterpolator()); 
            \_animator.SetDuration(2000); 
            \_animator.Update += OnAnimationUpdate; 
            \_animator.AnimationEnd += OnAnimationEnd; 
        } 

        protected override void OnAttached() { ... } 

        protected override void OnDetached() { ... } 

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) { ... } 

        private void OnAnimationUpdate(object sender, ValueAnimator.AnimatorUpdateEventArgs e) 
        { 
            if (!IsAttached) return; 

            Container.TranslationY = (float) e.Animation.AnimatedValue; 
        } 

        private void OnAnimationEnd(object sender, EventArgs e) 
        { 
            var visualElement = Element as VisualElement; 

            if (visualElement != null) 
            { 
                visualElement.TranslationY = Forms.Context.FromPixels(Container.TranslationY); 
            } 
        } 

        private void AnimateIn() 
        { 
            if (\_animator.IsRunning) 
                \_animator.Cancel(); 

            \_animator.SetFloatValues(Container.TranslationY, -Container.Height); 
            \_animator.Start(); 
        } 

        private void AnimateOut() 
        { 
            if (\_animator.IsRunning) 
                \_animator.Cancel();

            \_animator.SetFloatValues(Container.TranslationY, 0); 
            \_animator.Start(); 
        } 
    } 
}

Using these effects allows us to easily animate views in and achieve native performance. Encapsulating the animations in the effects allows for the code to be re-used and applied to other elements in the future.

The complete solution can be found [here](https://github.com/Keboo/XfAnimationEffects).
