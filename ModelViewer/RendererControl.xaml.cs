using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.ListBox;

namespace ModelViewer
{
    public class DragArgs
    {
        public dynamic obj;
        public Vector3D position;
        public Vector3D StartPos;
    }

    public partial class RendererControl : UserControl
    {
        public Dictionary<string, Model3D> ImportedModels = new Dictionary<string, Model3D>();
        Dictionary<dynamic, ModelVisual3D> Models = new Dictionary<dynamic, ModelVisual3D>(); //Can't use LevelObj because this project is referenced into OdysseyEditor
		Dictionary<dynamic, ModelVisual3D> Paths = new Dictionary<dynamic, ModelVisual3D>();
		SortingVisual3D ModelViewer = new SortingVisual3D();
        Vector3D CameraTarget = new Vector3D(0, 0, 0);

        readonly Material defaultMaterial = new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(255,120, 120, 120)));

        public double CameraInertiaFactor
        {
            get { return ModelView.CameraInertiaFactor; }
            set { ModelView.CameraInertiaFactor = value; }
        }

        public bool ShowFps
        {
            get { return ModelView.ShowFrameRate; }
            set { ModelView.ShowFrameRate = value; }
        }

        public bool ShowTriangleCount
        {
            get { return ModelView.ShowTriangleCountInfo; }
            set { ModelView.ShowTriangleCountInfo = value; }
        }

        public bool ShowDebugInfo
        {
            get { return ModelView.ShowCameraInfo; }
            set { ModelView.ShowCameraInfo = value; }
        }

        public CameraMode CamMode
        {
            get { return ModelView.CameraMode; }
            set { ModelView.CameraMode = value; }
        }

        public double ZoomSensitivity
        {
            get { return ModelView.ZoomSensitivity; }
            set { ModelView.ZoomSensitivity = value; }
        }

        public double RotationSensitivity
        {
            get { return ModelView.RotationSensitivity; }
            set { ModelView.RotationSensitivity = value; }
        }

		public double FarPlaneDistance
		{
			get { return ModelView.Camera.FarPlaneDistance; }
			set { ModelView.Camera.FarPlaneDistance = value; }
		}

		public RendererControl()
        {
            InitializeComponent();
            ModelViewer.SortingFrequency = 0.5;
            ModelView.Children.Add(ModelViewer);
            ModelView.Camera.NearPlaneDistance = 1;
			ModelView.ClipToBounds = false;
        }
        
        public void Clear()
        {
            ImportedModels = null;
            Models = null;
			Paths = null;
			ModelView = null;
        }

        public void SetSortFrequency(double t)
        {
            ModelViewer.SortingFrequency = t;
        }
		
        bool HasModel(dynamic obj) => Models.ContainsKey(obj);

        public Model3DGroup ReadObj(string path)
        {
            var r = new ObjReader() { DefaultMaterial = defaultMaterial, IgnoreErrors = false };
            return r.Read(path);
        }

		bool HasPath(dynamic obj) => Paths.ContainsKey(obj);

		public void AddPath(dynamic reference, Point3D[] Points, int Thickness = 5) =>
			AddPath(reference, Points, Thickness, Colors.Red);

		public void AddPath(dynamic reference, Point3D[] Points, int thickness, Color color)
		{
			if (Points.Length < 2) return;
			LinesVisual3D l = null;
			if (HasPath(reference))
				l = Paths[reference];
			else
			{
				l = new LinesVisual3D();
				Paths.Add(reference, l);
				ModelViewer.Children.Add(l);
			}
			l.Thickness = thickness;
			l.Color = color;
			l.Children.Clear();
			for (int i = 0; i < Points.Length; i++)
			{
				l.Points.Add(Points[i]);
			}
			for (int i = 1; i < Points.Length; i++) //Fix for a weird glitch (?)
			{
				l.Points.Add(Points[i]);
			}
			ModelView.UpdateLayout();
		}

        public void AddModel(string path, dynamic obj, Vector3D pos, Vector3D scale, Vector3D rot)
        {
            if (HasModel(obj)) return;
            Models.Add(obj,new ModelVisual3D());
            ModelViewer.Children.Add(Models[obj]);
            Model3D Model;
            if (!ImportedModels.ContainsKey(path))
            {
                Model = ReadObj(path);
                ImportedModels.Add(path, Model);
            }
            else Model = ImportedModels[path];
            Model.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 90));
            Models[obj].Content = Model;
            Transform3DGroup t = new Transform3DGroup();
            t.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), rot.X)));
            t.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), rot.Y)));
            t.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), rot.Z)));
            t.Children.Add(new ScaleTransform3D(scale));
            t.Children.Add(new TranslateTransform3D(pos));
            Models[obj].Transform = t;
        }

        public void RemoveModel(dynamic obj)
        {
			if (HasModel(obj))
			{
				if (selectionBoxes.ContainsKey(obj))
				{
					ModelViewer.Children.Remove(selectionBoxes[obj]);
					selectionBoxes.Remove(obj);
				}
				ModelViewer.Children.Remove(Models[obj]);
				Models[obj].Content = null;
				Models.Remove(obj);
			}
			if (HasPath(obj))
			{
				ModelViewer.Children.Remove(Paths[obj]);
				Paths.Remove(obj);
				ModelView.UpdateLayout();
			}
            ModelView.UpdateLayout();
        }

        public void RemoveRailPoints(LinesVisual3D rail)
        {
            foreach (LinesVisual3D r in rail.Children) RemoveRailPoints(r);
            rail.Points.Clear();
        }
        
        public void LookAt(Vector3D p)
        {
            ModelView.Camera.LookAt(p.ToPoint3D(), 500, 1000);
            CameraTarget = p;
        }

        public void SetCameraDirection(int x, int y, int z)
        {
            ModelView.Camera.UpDirection = new Vector3D(x, y, z);
        }

        public Vector3D Drag(DragArgs args, System.Windows.Input.MouseEventArgs e, double roundTo)
        {
            Point p = e.GetPosition(ModelView);
            Vector3D v = args.position;
            Point3D? pos = ModelView.Viewport.UnProject(p, new Point3D(v.X,v.Y,v.Z), ModelView.Camera.LookDirection);
            if (pos.HasValue)
            {
                Vector3D vec = pos.Value.ToVector3D();
                if (roundTo != 0)
                {
                    vec.X = Math.Round(vec.X / roundTo, 0) * roundTo;
                    vec.Y = Math.Round(vec.Y / roundTo, 0) * roundTo;
                    vec.Z = Math.Round(vec.Z / roundTo, 0) * roundTo;
                    return vec;
                }
                else
                {
                    vec.X = Math.Round(vec.X, 3, MidpointRounding.AwayFromZero);
                    vec.Y = Math.Round(vec.Y, 3, MidpointRounding.AwayFromZero);
                    vec.Z = Math.Round(vec.Z, 3, MidpointRounding.AwayFromZero);
                    return vec;
                }
            }
            return pos.Value.ToVector3D();
        }

        public void ChangeTransform(dynamic obj, Vector3D pos, Vector3D scale, Vector3D Rot)
        {
            if (!HasModel(obj)) return;
            Transform3DGroup t = new Transform3DGroup();
            t.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), Rot.X)));
            t.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), Rot.Y)));
            t.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), Rot.Z)));
            t.Children.Add(new ScaleTransform3D(scale));
            t.Children.Add(new TranslateTransform3D(pos));
            Models[obj].Transform = t;
            if (selectionBoxes.ContainsKey(obj))
            {
                selectionBoxes[obj].BoundingBox = ((ModelVisual3D)Models[obj]).FindBounds(Transform3D.Identity);
            }
        }

        public void ChangeModel(dynamic obj, string path)
        {
            if (!HasModel(obj)) return;
            Model3D Model;
            if (!ImportedModels.ContainsKey(path))
            {
                Model = ReadObj(path);
                ImportedModels.Add(path, Model);
            }
            else Model = ImportedModels[path];
            Model.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 90));
            Models[obj].Content = Model;
            ModelView.UpdateLayout();
        }
        
        public dynamic GetOBJ(object sender, dynamic e)
        {
            Point p = e.GetPosition(ModelView);
            ModelVisual3D result = GetHitResult(p);
            if (result == null) return null;
            if (Models.ContainsValue(result)) return Models.FirstOrDefault(x => x.Value == result).Key;
            return null;
        }

        ModelVisual3D GetHitResult(Point location)
        {
            HitTestResult result = VisualTreeHelper.HitTest(ModelView, location);
            if (result != null && result.VisualHit is ModelVisual3D)
            {
                ModelVisual3D visual = (ModelVisual3D)result.VisualHit;
                return visual;
            }

            return null;
        }

        public double TooCloseCheck() {return Math.Abs(CameraTarget.X) - Math.Abs(ModelView.Camera.Position.X) ; }

        public Vector3D GetPositionInView()
        {
            FrameworkElement pnlClient = this.Content as FrameworkElement;
            Point3D p = (Point3D)ModelView.Viewport.UnProject(new Point(pnlClient.ActualWidth / 2, pnlClient.ActualHeight / 2), ModelView.Camera.Position, ModelView.Camera.LookDirection);
            return new Vector3D(Math.Truncate(p.X), Math.Truncate(p.Y), Math.Truncate(p.Z));
        }

        public const string C0ListName = "__C0EditingListObjs";

        public void UnloadLevel()
        {
            ModelView.Children.Remove(ModelViewer);
            ModelViewer.Children.Clear();
            ImportedModels = new Dictionary<string, Model3D>();
            Models = new Dictionary<dynamic, ModelVisual3D>();
			Paths = new Dictionary<dynamic, ModelVisual3D>();
            ModelViewer = new SortingVisual3D();
            ModelViewer.SortingFrequency = 0.5;
            ModelView.Children.Add(ModelViewer);
            ModelView.Camera.LookAt(new Point3D(0,0,0), 50, 1000);
            CameraTarget = new Vector3D(0, 0, 0);
        }

        Dictionary<dynamic, ModelVisual3D> selectionBoxes = new Dictionary<dynamic, ModelVisual3D>();
        public void SelectObjs(IList<dynamic> Objs)
        {
            ClearSelection();
            foreach (dynamic o in Objs)
            {
                if (!HasModel(o)) continue;
                BoundingBoxVisual3D box = new BoundingBoxVisual3D();
                selectionBoxes.Add(o,box);
                box.BoundingBox = ((ModelVisual3D)Models[o]).FindBounds(Transform3D.Identity);
                box.Diameter = box.BoundingBox.SizeX / 20;
                ModelViewer.Children.Add(selectionBoxes[o]);
            }
            ModelView.UpdateLayout();
        }

        public void ClearSelection()
        {
            foreach (var b in selectionBoxes.Values) ModelViewer.Children.Remove(b);
            selectionBoxes.Clear();
            ModelView.UpdateLayout();
        }
    }
}
