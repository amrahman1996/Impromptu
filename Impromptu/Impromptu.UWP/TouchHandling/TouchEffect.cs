﻿using System;
using System.Linq;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Impromptu.TouchHandling;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using TouchEffect = Impromptu.UWP.TouchHandling.TouchEffect;

[assembly: ResolutionGroupName("XamarinDocs")]
[assembly: ExportEffect(typeof(TouchEffect), "TouchEffect")]

namespace Impromptu.UWP.TouchHandling {
    public class TouchEffect : PlatformEffect {
        FrameworkElement frameworkElement;
        Impromptu.TouchHandling.TouchEffect effect;
        Action<Element, TouchActionEventArgs> onTouchAction;

        protected override void OnAttached() {
            // Get the Windows FrameworkElement corresponding to the Element that the effect is attached to
            frameworkElement = Control == null ? Container : Control;

            // Get access to the TouchEffect class in the PCL
            effect = (Impromptu.TouchHandling.TouchEffect)Element.Effects.FirstOrDefault(e => e is Impromptu.TouchHandling.TouchEffect);

            if(effect != null && frameworkElement != null) {
                // Save the method to call on touch events
                onTouchAction = effect.OnTouchAction;

                // Set event handlers on FrameworkElement
                frameworkElement.PointerEntered += OnPointerEntered;
                frameworkElement.PointerPressed += OnPointerPressed;
                frameworkElement.PointerMoved += OnPointerMoved;
                frameworkElement.PointerReleased += OnPointerReleased;
                frameworkElement.PointerExited += OnPointerExited;
                frameworkElement.PointerCanceled += OnPointerCancelled;
            }
        }

        protected override void OnDetached() {
            if(onTouchAction != null) {
                // Release event handlers on FrameworkElement
                frameworkElement.PointerEntered -= OnPointerEntered;
                frameworkElement.PointerPressed -= OnPointerPressed;
                frameworkElement.PointerMoved -= OnPointerMoved;
                frameworkElement.PointerReleased -= OnPointerReleased;
                frameworkElement.PointerExited -= OnPointerEntered;
                frameworkElement.PointerCanceled -= OnPointerCancelled;
            }
        }

        void OnPointerEntered(object sender, PointerRoutedEventArgs args) { CommonHandler(sender, TouchActionType.Entered, args); }

        void OnPointerPressed(object sender, PointerRoutedEventArgs args) {
            CommonHandler(sender, TouchActionType.Pressed, args);

            // Check setting of Capture property
            if(effect.Capture) {
                (sender as FrameworkElement).CapturePointer(args.Pointer);
            }
        }

        void OnPointerMoved(object sender, PointerRoutedEventArgs args) { CommonHandler(sender, TouchActionType.Moved, args); }

        void OnPointerReleased(object sender, PointerRoutedEventArgs args) { CommonHandler(sender, TouchActionType.Released, args); }

        void OnPointerExited(object sender, PointerRoutedEventArgs args) { CommonHandler(sender, TouchActionType.Exited, args); }

        void OnPointerCancelled(object sender, PointerRoutedEventArgs args) { CommonHandler(sender, TouchActionType.Cancelled, args); }

        void CommonHandler(object sender, TouchActionType touchActionType, PointerRoutedEventArgs args) {
            PointerPoint pointerPoint = args.GetCurrentPoint(sender as UIElement);
            Windows.Foundation.Point windowsPoint = pointerPoint.Position;

            onTouchAction(Element, new TouchActionEventArgs(args.Pointer.PointerId,
                touchActionType,
                new Point(windowsPoint.X, windowsPoint.Y),
                args.Pointer.IsInContact));
        }
    }
}