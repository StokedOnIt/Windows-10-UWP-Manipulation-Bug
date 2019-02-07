using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Manipulation_Test
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        double CurrentZoomScale;
        double PaperRotationOffset;
        CompositeTransform _transform = new CompositeTransform();


        private Rect BoundsRelativeTo(FrameworkElement element, FrameworkElement relativeTo)
        {
            return element.TransformToVisual(relativeTo).TransformBounds(new Rect(0, 0, element.ActualWidth, element.ActualHeight));
        }


        public MainPage()
        {
            this.InitializeComponent();

            Area.Width = 3000;
            Area.Height = 3000;
            Outer.Width = 3000;
            Outer.Height = 3000;
            PageControler.Height = 3000;
            PageControler.Height = 3000;
            Inner.Width = 1000;
            Inner.Height = 1000;
            PageControler.Width = 3000;
            PageControler.Height = 3000;

            _transform.ScaleX = .8;
            _transform.ScaleY = .8;

            Area.RenderTransform = _transform;

            PageControler.ManipulationStarting += new ManipulationStartingEventHandler(ManipulateMe_ManipulationStarting);
            PageControler.ManipulationStarted += new ManipulationStartedEventHandler(ManipulateMe_ManipulationStarted);
            PageControler.ManipulationDelta += new ManipulationDeltaEventHandler(ManipulateMe_ManipulationDelta);
            PageControler.ManipulationCompleted += new ManipulationCompletedEventHandler(ManipulateMe_ManipulationCompleted);

            PageControler.DoubleTapped += PageControler_DoubleTapped;

        }

        private void PageControler_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {

            //this simulates a large amount of processing that happens in the background
                var count = 0;
                for(var i = 0; i < 10000000;i++)
                {
                    for (var r = 0; r < 100; r++)
                    {
                        count++;
                    }
                    count++;
                }
        }

        async void ManipulateMe_ManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
        {

        }

        async void ManipulateMe_ManipulationStarted(object sender, Windows.UI.Xaml.Input.ManipulationStartedRoutedEventArgs e)
        {
            try
            {
                ((MainPage)sender).CancelDirectManipulations();


                Point point = new Point(
                    e.Position.X,
                    e.Position.Y);

                TransformGroup group = new TransformGroup();
                TranslateTransform tempTranslateTransform = new TranslateTransform();
                tempTranslateTransform.X = _transform.CenterX;
                tempTranslateTransform.Y = _transform.CenterY;

                RotateTransform tempRotateTransform = new RotateTransform();
                tempRotateTransform.CenterX = _transform.CenterX;
                tempRotateTransform.CenterY = _transform.CenterY;
                tempRotateTransform.Angle = _transform.Rotation;

                group.Children.Add(tempTranslateTransform);
                group.Children.Add(tempRotateTransform);

                Point center = tempRotateTransform.TransformPoint(e.Position);
                Point localPoint = group.Inverse.TransformPoint(center);

                localPoint.X *= _transform.ScaleX;
                localPoint.Y *= _transform.ScaleY;

                Point worldPoint = group.TransformPoint(localPoint);

                Point distance = new Point(
                    worldPoint.X - point.X,
                    worldPoint.Y - point.Y);

                _transform.TranslateX += distance.X;
                _transform.TranslateY += distance.Y;

                _transform.CenterX = point.X;
                _transform.CenterY = point.Y;


            }
            catch (Exception ex)
            {
            }
        }

        async void ManipulateMe_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            try
            {


                Point point = new Point(
                 e.Position.X,
                 e.Position.Y);

                TransformGroup group = new TransformGroup();
                TranslateTransform tempTranslateTransform = new TranslateTransform();
                tempTranslateTransform.X = _transform.CenterX;
                tempTranslateTransform.Y = _transform.CenterY;

                RotateTransform tempRotateTransform = new RotateTransform();
                tempRotateTransform.CenterX = _transform.CenterX;
                tempRotateTransform.CenterY = _transform.CenterY;
                    tempRotateTransform.Angle = _transform.Rotation;

                group.Children.Add(tempTranslateTransform);
                group.Children.Add(tempRotateTransform);

                Point center = tempRotateTransform.TransformPoint(e.Position);
                Point localPoint = group.Inverse.TransformPoint(center);

                localPoint.X *= _transform.ScaleX;
                localPoint.Y *= _transform.ScaleY;

                Point worldPoint = group.TransformPoint(localPoint);

                Point distance = new Point(
                    worldPoint.X - point.X,
                    worldPoint.Y - point.Y);


                _transform.TranslateX += distance.X;
                _transform.TranslateY += distance.Y;

                    CurrentZoomScale = _transform.ScaleY = _transform.ScaleX *= e.Delta.Scale;


                if (_transform.Rotation > 360)
                {
                    _transform.Rotation -= 360;
                }
                else if (_transform.Rotation < -360)
                {
                    _transform.Rotation += 360;
                }
                    PaperRotationOffset = _transform.Rotation += e.Delta.Rotation;


                _transform.CenterX = point.X;
                _transform.CenterY = point.Y;

                DebugDot.RenderTransform = new TranslateTransform()
                {
                    X = point.X,
                    Y = point.Y
                };



                Rect desktopBounds = BoundsRelativeTo(Outer, LayoutRoot);

                double zoomRatio = Outer.ActualWidth / desktopBounds.Width;
                double zoomRatioInvert = desktopBounds.Width / Outer.ActualWidth;

                double edgeBufferYPadding = 0;



                if (desktopBounds.X < LayoutRoot.ActualWidth / 2 && desktopBounds.Right > LayoutRoot.ActualWidth / 2)
                {
                    _transform.TranslateX += e.Delta.Translation.X;
                }
                if (desktopBounds.X > LayoutRoot.ActualWidth / 2)
                {
                    _transform.TranslateX += e.Delta.Translation.X - (desktopBounds.X - (LayoutRoot.ActualWidth / 2) - 1);
                }
                if (desktopBounds.Right < LayoutRoot.ActualWidth / 2)
                {
                    _transform.TranslateX += e.Delta.Translation.X - (desktopBounds.Right - (LayoutRoot.ActualWidth / 2) - 1);
                }

                if (desktopBounds.Y + edgeBufferYPadding < LayoutRoot.ActualHeight / 2 && desktopBounds.Bottom - edgeBufferYPadding * zoomRatio > LayoutRoot.ActualHeight / 2)
                {
                    _transform.TranslateY += e.Delta.Translation.Y;
                }
                if (desktopBounds.Y + edgeBufferYPadding * zoomRatio > LayoutRoot.ActualHeight / 2)
                {
                    _transform.TranslateY += e.Delta.Translation.Y - (desktopBounds.Y + edgeBufferYPadding - (LayoutRoot.ActualHeight / 2) - 1);
                }
                if (desktopBounds.Bottom - edgeBufferYPadding < LayoutRoot.ActualHeight / 2)
                {
                    _transform.TranslateY += e.Delta.Translation.Y - (desktopBounds.Bottom - edgeBufferYPadding - (LayoutRoot.ActualHeight / 2) - 1);
                }


            }
            catch (Exception ex)
            {
            }
        }

        async void ManipulateMe_ManipulationCompleted(object sender, Windows.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)
        {
            try
            {
                Point point = new Point(e.Position.X, e.Position.Y);
                Point transformedPoint = _transform.TransformPoint(point);

                    if (_transform.Rotation > 360)
                    {
                        _transform.Rotation -= 360;
                    }
                    else if (_transform.Rotation < -360)
                    {
                        _transform.Rotation += 360;
                    }




                PageControler.CancelDirectManipulations();
            }
            catch (Exception ex)
            {
            }
        }
    }
}
