using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace IntermodalContainers
{
    public class ContainerTexture : Control
    {
        public ContainerTexture()
        {
            UpdateTextureSize(this, default);
        }

        static ContainerTexture() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ContainerTexture), new FrameworkPropertyMetadata(typeof(ContainerTexture), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty RoofTemplateProperty = DependencyProperty.Register(nameof(RoofTemplate), typeof(DataTemplate), typeof(ContainerTexture), new PropertyMetadata(null));
        public static readonly DependencyProperty FloorTemplateProperty = DependencyProperty.Register(nameof(FloorTemplate), typeof(DataTemplate), typeof(ContainerTexture), new PropertyMetadata(null));
        public static readonly DependencyProperty DoorTemplateProperty = DependencyProperty.Register(nameof(DoorTemplate), typeof(DataTemplate), typeof(ContainerTexture), new PropertyMetadata(null));
        public static readonly DependencyProperty FrontTemplateProperty = DependencyProperty.Register(nameof(FrontTemplate), typeof(DataTemplate), typeof(ContainerTexture), new PropertyMetadata(null));
        public static readonly DependencyProperty LeftSideTemplateProperty = DependencyProperty.Register(nameof(LeftSideTemplate), typeof(DataTemplate), typeof(ContainerTexture), new PropertyMetadata(null));
        public static readonly DependencyProperty RightSideTemplateProperty = DependencyProperty.Register(nameof(RightSideTemplate), typeof(DataTemplate), typeof(ContainerTexture), new PropertyMetadata(null));
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(nameof(Size), typeof(Size3D), typeof(ContainerTexture), new PropertyMetadata(new Size3D(12.192, 2.438, 2.591), UpdateTextureSize));
        public static readonly DependencyProperty MeterToPixelScaleProperty = DependencyProperty.Register(nameof(MeterToPixelScale), typeof(double), typeof(ContainerTexture), new PropertyMetadata(100d, UpdateTextureSize));
        public static readonly DependencyProperty TextureSizeProperty = DependencyProperty.Register(nameof(TextureSize), typeof(Size3D), typeof(ContainerTexture), new PropertyMetadata(Size3D.Empty));

        public DataTemplate RoofTemplate { get => (DataTemplate)GetValue(RoofTemplateProperty); set => SetValue(RoofTemplateProperty, value); }
        public DataTemplate RightSideTemplate { get => (DataTemplate)GetValue(RightSideTemplateProperty); set => SetValue(RightSideTemplateProperty, value); }
        public DataTemplate FloorTemplate { get => (DataTemplate)GetValue(FloorTemplateProperty); set => SetValue(FloorTemplateProperty, value); }
        public DataTemplate LeftSideTemplate { get => (DataTemplate)GetValue(LeftSideTemplateProperty); set => SetValue(LeftSideTemplateProperty, value); }
        public DataTemplate DoorTemplate { get => (DataTemplate)GetValue(DoorTemplateProperty); set => SetValue(DoorTemplateProperty, value); }
        public DataTemplate FrontTemplate { get => (DataTemplate)GetValue(FrontTemplateProperty); set => SetValue(FrontTemplateProperty, value); }
        public Size3D Size { get => (Size3D)GetValue(SizeProperty); set => SetValue(SizeProperty, value); }
        public double MeterToPixelScale { get => (double)GetValue(MeterToPixelScaleProperty); set => SetValue(MeterToPixelScaleProperty, value); }
        public Size3D TextureSize { get => (Size3D)GetValue(TextureSizeProperty); private set => SetValue(TextureSizeProperty, value); }

        private static void UpdateTextureSize(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ContainerTexture texture)
            {
                var size = texture.Size;
                texture.TextureSize = new Size3D(
                    size.X * texture.MeterToPixelScale,
                    size.Y * texture.MeterToPixelScale,
                    size.Z * texture.MeterToPixelScale);
            }
        }
    }
}
