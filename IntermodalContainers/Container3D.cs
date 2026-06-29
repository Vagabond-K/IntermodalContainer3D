using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace IntermodalContainers
{
    public class Container3D : UIElement3D
    {
        private static readonly DiffuseMaterial emptyMaterial = new();

        public Container3D()
        {
            var materialGroup = new MaterialGroup();

            materialGroup.Children.Add(new DiffuseMaterial());  // Background 브러시를 적용할 재질
            materialGroup.Children.Add(emptyMaterial);  // BaseMaterial이 위치할 곳
            materialGroup.Children.Add(textureMaterial);    // 컨테이너 줄무늬 텍스처를 적용할 재질
            materialGroup.Children.Add(emptyMaterial);  // OverlayMaterial이 위치할 곳

            Visual3DModel = new GeometryModel3D { Material = materialGroup };

            // 컨테이너의 가장 깊은 재질에 Background 브러시 바인딩
            BindingOperations.SetBinding(materialGroup.Children[0], DiffuseMaterial.BrushProperty, new Binding(nameof(Background)) { Source = this });
        }

        private readonly DiffuseMaterial textureMaterial = new();

        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(Container3D), new PropertyMetadata(new SolidColorBrush(Colors.SteelBlue)));
        public static readonly DependencyProperty BaseMaterialProperty = DependencyProperty.Register(nameof(BaseMaterial), typeof(Material), typeof(Container3D), new PropertyMetadata(null, (obj, e) => (obj as Container3D)?.OnUpdateMaterial(1, e.NewValue as Material)));
        public static readonly DependencyProperty OverlayMaterialProperty = DependencyProperty.Register(nameof(OverlayMaterial), typeof(Material), typeof(Container3D), new PropertyMetadata(null, (obj, e) => (obj as Container3D)?.OnUpdateMaterial(3, e.NewValue as Material)));
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(nameof(Size), typeof(Size3D), typeof(Container3D), new PropertyMetadata(new Size3D(12.192, 2.438, 2.591), (obj, e) => (obj as Container3D)?.OnUpdateModel()));

        public Brush Background { get => (Brush)GetValue(BackgroundProperty); set => SetValue(BackgroundProperty, value); }
        public Material BaseMaterial { get => (Material)GetValue(BaseMaterialProperty); set => SetValue(BaseMaterialProperty, value); }
        public Material OverlayMaterial { get => (Material)GetValue(OverlayMaterialProperty); set => SetValue(OverlayMaterialProperty, value); }
        public Size3D Size { get => (Size3D)GetValue(SizeProperty); set => SetValue(SizeProperty, value); }

        private static readonly Dictionary<Size3D, WeakReference<ImageBrush>> textureCache = [];

        protected override void OnUpdateModel()
        {
            var model = (GeometryModel3D)Visual3DModel;
            var mesh = model.Geometry as MeshGeometry3D;
            if (mesh?.IsFrozen != false)
                model.Geometry = mesh = new MeshGeometry3D();

            var size = Size;

            // 1. Size에 따른 직육면체 정점 생성 시작
            double hl = size.X / 2;
            double hw = size.Y / 2;

            var lth = new Point3D(-hl, -hw, size.Z);
            var trh = new Point3D(hl, -hw, size.Z);
            var rbh = new Point3D(hl, hw, size.Z);
            var blh = new Point3D(-hl, hw, size.Z);

            var lt0 = new Point3D(-hl, -hw, 0);
            var tr0 = new Point3D(hl, -hw, 0);
            var rb0 = new Point3D(hl, hw, 0);
            var bl0 = new Point3D(-hl, hw, 0);

            var positions = new Point3DCollection
        {
            lth, trh, rbh, blh,
            bl0, rb0, tr0, lt0,
            blh, rbh, rb0, bl0,
            trh, lth, lt0, tr0,
            rbh, trh, tr0, rb0,
            lth, blh, bl0, lt0,
        };

            // 2. 직육면체 정점들을 이용한 삼각형 표면 생성 시작
            var triangleIndices = new Int32Collection();
            for (int i = 0; i < positions.Count; i += 4)
            {
                // 시계방향 삼각형 두 개씩 생성
                triangleIndices.Add(i + 0);
                triangleIndices.Add(i + 1);
                triangleIndices.Add(i + 2);

                triangleIndices.Add(i + 0);
                triangleIndices.Add(i + 2);
                triangleIndices.Add(i + 3);
            }

            // 3. 컨테이너 사이즈와 텍스처 너비와 높이를 이용한 텍스처 매핑 좌표 생성 시작
            var texureWidth = size.X + size.Y;
            var texureHeight = (size.Y + size.Z) * 2;

            var col1 = size.X / texureWidth;
            var row1 = size.Y / texureHeight;
            var row3 = (texureHeight - size.Z) / texureHeight;

            var textureCoordinates = new PointCollection();

            void AddTextureCoords(params Point[] points)
            {
                foreach (var point in points)
                    textureCoordinates.Add(point);
            }

            AddTextureCoords(new Point(col1, 0), new Point(0, 0), new Point(0, row1), new Point(col1, row1));
            AddTextureCoords(new Point(col1, 0.5), new Point(0, 0.5), new Point(0, row3), new Point(col1, row3));
            AddTextureCoords(new Point(col1, row1), new Point(0, row1), new Point(0, 0.5), new Point(col1, 0.5));
            AddTextureCoords(new Point(col1, row3), new Point(0, row3), new Point(0, 1), new Point(col1, 1));
            AddTextureCoords(new Point(1, row1), new Point(col1, row1), new Point(col1, 0.5), new Point(1, 0.5));
            AddTextureCoords(new Point(1, row3), new Point(col1, row3), new Point(col1, 1), new Point(1, 1));

            mesh.Positions = positions;
            mesh.TriangleIndices = triangleIndices;
            mesh.TextureCoordinates = textureCoordinates;

            // 4. 노멀 벡터 생성 시작
            IEnumerable<Vector3D> GetNormals()
            {
                yield return new Vector3D(0, 0, 1);
                yield return new Vector3D(0, 0, -1);
                yield return new Vector3D(0, 1, 0);
                yield return new Vector3D(0, -1, 0);
                yield return new Vector3D(1, 0, 0);
                yield return new Vector3D(-1, 0, 0);
            }
            mesh.Normals = [.. GetNormals().SelectMany(normal => Enumerable.Repeat(normal, 4))];

            // 5. 컨테이너 표면 주름 및 기타 요소 표시용 텍스처 이미지 브러시 생성 시작
            if (!textureCache.TryGetValue(size, out var reference) || !reference.TryGetTarget(out var imageBrush))
            {
                var texture = new ContainerTexture { Size = size };
                var viewbox = new Viewbox { Child = texture };
                viewbox.Arrange(new Rect());
                viewbox.UpdateLayout();
                var image = new RenderTargetBitmap((int)Math.Ceiling(texture.ActualWidth), (int)Math.Ceiling(texture.ActualHeight), 96, 96, PixelFormats.Default);
                image.Render(texture);
                imageBrush = new ImageBrush { ImageSource = image };
                imageBrush.Freeze();
                textureCache[size] = new WeakReference<ImageBrush>(imageBrush);
            }
            textureMaterial.Brush = imageBrush;

            foreach (var key in textureCache.Where(item => !item.Value.TryGetTarget(out _)).Select(item => item.Key).ToHashSet())
                textureCache.Remove(key);

            if (!DesignerProperties.GetIsInDesignMode(mesh))
                mesh.Freeze();
        }

        private void OnUpdateMaterial(int index, Material? material)
            => ((MaterialGroup)((GeometryModel3D)Visual3DModel).Material).Children[index] = material ?? emptyMaterial;
    }
}
