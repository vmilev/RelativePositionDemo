#if WPF
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#elif NETFX_CORE
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

namespace Panels
{
    public class RelativePositionPanel : Panel
    {
        public static readonly DependencyProperty RelativeXProperty =
            DependencyProperty.RegisterAttached("RelativeX", typeof(double), typeof(RelativePositionPanel), new PropertyMetadata(0.5d, InvalidateLayoutCallback));

        public static readonly DependencyProperty RelativeYProperty =
            DependencyProperty.RegisterAttached("RelativeY", typeof(double), typeof(RelativePositionPanel), new PropertyMetadata(0.5d, InvalidateLayoutCallback));

        public static readonly DependencyProperty ItemHorizontalPositionAlignmentProperty =
            DependencyProperty.RegisterAttached("ItemHorizontalPositionAlignment", typeof(HorizontalPositionAlignment), typeof(RelativePositionPanel), new PropertyMetadata(HorizontalPositionAlignment.Center, InvalidateLayoutCallback));

        public static readonly DependencyProperty ItemVerticalPositionAlignmentProperty =
            DependencyProperty.RegisterAttached("ItemVerticalPositionAlignment", typeof(VerticalPositionAlignment), typeof(RelativePositionPanel), new PropertyMetadata(VerticalPositionAlignment.Center, InvalidateLayoutCallback));

        public static double GetRelativeX(DependencyObject obj)
        {
            return (double)obj.GetValue(RelativeXProperty);
        }

        public static void SetRelativeX(DependencyObject obj, double value)
        {
            obj.SetValue(RelativeXProperty, value);
        }

        public static double GetRelativeY(DependencyObject obj)
        {
            return (double)obj.GetValue(RelativeYProperty);
        }

        public static void SetRelativeY(DependencyObject obj, double value)
        {
            obj.SetValue(RelativeYProperty, value);
        }

        public static HorizontalPositionAlignment GetItemHorizontalPositionAlignment(DependencyObject obj)
        {
            return (HorizontalPositionAlignment)obj.GetValue(ItemHorizontalPositionAlignmentProperty);
        }

        public static void SetItemHorizontalPositionAlignment(DependencyObject obj, HorizontalPositionAlignment value)
        {
            obj.SetValue(ItemHorizontalPositionAlignmentProperty, value);
        }

        public static VerticalPositionAlignment GetItemVerticalPositionAlignment(DependencyObject obj)
        {
            return (VerticalPositionAlignment)obj.GetValue(ItemVerticalPositionAlignmentProperty);
        }

        public static void SetItemVerticalPositionAlignment(DependencyObject obj, VerticalPositionAlignment value)
        {
            obj.SetValue(ItemVerticalPositionAlignmentProperty, value);
        }

        private static void InvalidateLayoutCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var child = d as UIElement;

            if(child != null)
            {
                var panel = VisualTreeHelper.GetParent(child) as RelativePositionPanel;

                if(panel != null)
                {
                    panel.InvalidateArrange();
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var coalescedAvailableSize = new Size(this.Coalesce(availableSize.Width), this.Coalesce(availableSize.Height));

            foreach (UIElement child in this.Children)
            {
                child.Measure(coalescedAvailableSize);
            }

            return coalescedAvailableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement child in this.Children)
            {
                var childRelativeX = GetRelativeX(child);
                var childRelativeY = GetRelativeY(child);
                var childHorizontalPositionAlignment = GetItemHorizontalPositionAlignment(child);
                var childVerticalPositionAlignment = GetItemVerticalPositionAlignment(child);

                var posX = childRelativeX * finalSize.Width;
                var posY = childRelativeY * finalSize.Height;

                if (childHorizontalPositionAlignment == HorizontalPositionAlignment.Center)
                    posX -= child.DesiredSize.Width / 2;

                if (childHorizontalPositionAlignment == HorizontalPositionAlignment.Right)
                    posX -= child.DesiredSize.Width;

                if (childVerticalPositionAlignment == VerticalPositionAlignment.Center)
                    posY -= child.DesiredSize.Height / 2;

                if (childVerticalPositionAlignment == VerticalPositionAlignment.Bottom)
                    posY -= child.DesiredSize.Height;

                child.Arrange(new Rect(new Point(posX, posY), child.DesiredSize));
            }

            return finalSize;
        }

        private double Coalesce(double dimension)
        {
            if (double.IsInfinity(dimension))
            {
                return 0;
            }

            return dimension;
        }
    }
}
