using EditorCore;
using EditorCore.Interfaces;
using ExtensionMethods;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using static EditorCore.PropertyGridTypes;

namespace MK8DExt
{
    public class LevelObj : ILevelObj
    {
		[Browsable(false)]
		public bool CanDrag { get; set; } = true;

		public const string N_Translate = "Translate";
        public const string N_Rotate = "Rotate";
        public const string N_Scale = "Scale";
        public const string N_Id = "UnitIdNum";
        public const string N_ObjectID = "ObjId";
		public static readonly string[] CantRemoveNames = { N_Translate, N_Rotate, N_Scale, N_Id , N_ObjectID };
		public static readonly string[] ModelFieldNames = { N_ObjectID, "ObjectID" };

		[Browsable(false)]
		public Dictionary<string, dynamic> Prop { get; set; } = new Dictionary<string, dynamic>();

        public LevelObj(dynamic bymlNode)
        {
            if (bymlNode is Dictionary<string, dynamic>) Prop = (Dictionary<string, dynamic>)bymlNode;
            else throw new Exception("Not a dictionary");
        }

        public LevelObj(bool empty = false, bool _CanDrag = true)
        {
			CanDrag = _CanDrag;
			if (empty) return;
            Prop.Add(N_Translate, new Dictionary<string, dynamic>());
            Prop[N_Translate].Add("X", (Single)0);
            Prop[N_Translate].Add("Y", (Single)0);
            Prop[N_Translate].Add("Z", (Single)0);
            Prop.Add(N_Rotate, new Dictionary<string, dynamic>());
            Prop[N_Rotate].Add("X", (Single)0);
            Prop[N_Rotate].Add("Y", (Single)0);
            Prop[N_Rotate].Add("Z", (Single)0);
            Prop.Add(N_Scale, new Dictionary<string, dynamic>());
            Prop[N_Scale].Add("X", (Single)1);
            Prop[N_Scale].Add("Y", (Single)1);
            Prop[N_Scale].Add("Z", (Single)1);
			this[N_Id] = 0;
			this[N_ObjectID] = 0;
		}

        public dynamic this [string name]
        {
            get
            {
                if (Prop.ContainsKey(name)) return Prop[name];
                else return null;
            }
            set
            {
                if (Prop.ContainsKey(name)) Prop[name] = value;
                else Prop.Add(name,value);
            }
        }

		public bool ContainsKey(string name) => Prop.ContainsKey(name);

		public Vector3D Pos
        {
            get { return new Vector3D(this[N_Translate]["X"], this[N_Translate]["Y"], this[N_Translate]["Z"]); }
            set {
                this[N_Translate]["X"] = (Single)value.X;
                this[N_Translate]["Y"] = (Single)value.Y;
                this[N_Translate]["Z"] = (Single)value.Z;
            }
        }

        public Vector3D Rot
        {
            get { return new Vector3D(this[N_Rotate]["X"], this[N_Rotate]["Y"], this[N_Rotate]["Z"]); }
            set
            {
                this[N_Rotate]["X"] = (Single)value.X;
                this[N_Rotate]["Y"] = (Single)value.Y;
                this[N_Rotate]["Z"] = (Single)value.Z;
            }
        }

		[Browsable(false)]
        public Vector3D ModelView_Pos
        {
            get { return new Vector3D(this[N_Translate]["X"], -this[N_Translate]["Z"], this[N_Translate]["Y"]); }
            set //set when dragging
            {
                this[N_Translate]["X"] = (Single)value.X;
                this[N_Translate]["Y"] = (Single)value.Z;
                this[N_Translate]["Z"] = -(Single)value.Y;
            }
        }

        [Browsable(false)]
        public Vector3D ModelView_Rot
        {
            get { return new Vector3D(this[N_Rotate]["X"], -this[N_Rotate]["Z"], this[N_Rotate]["Y"]); } //TODO: check if it matches in-game
        }

        public Vector3D Scale
        {
            get { return new Vector3D(this[N_Scale]["X"], this[N_Scale]["Y"], this[N_Scale]["Z"]); }
            set
            {
                this[N_Scale]["X"] = (Single)value.X;
                this[N_Scale]["Y"] = (Single)value.Y;
                this[N_Scale]["Z"] = (Single)value.Z;
            }
        }

        [Browsable(false)]
        public Vector3D ModelView_Scale
        {
            get { return new Vector3D(this[N_Scale]["X"], this[N_Scale]["Z"], this[N_Scale]["Y"]); }           
        }

        [Browsable(false)]
        public Transform transform
        {
            get => new Transform() { Pos = Pos, Rot = Rot, Scale = Scale };
            set
            {
                Pos = value.Pos;
                Rot = value.Rot;
                Scale = value.Scale;
            }
        }

		[Browsable(false)]
		public int ID_int
		{
			get => this[N_Id];
			set => this[N_Id] = value;
		}

		public string ID
        {
			get => ID_int.ToString();
			set => ID_int = int.Parse(value);
        }

		public int ObjectID
		{
			get => this[N_ObjectID];
			set => this[N_ObjectID] = value;
		}

        public string Name
        {
            get { return ModelName; }
            set { ObjectID = MK8DModule.GetId(value); }
        }

        public string ModelName
        {
            get => MK8DModule.GetName(ObjectID);
        }

        public override string ToString()
        {
			string name = Name;
            if (name == null) name = "LevelObj id: " + this[N_Id];
            if (name == null) name = "LevelObj";
            return name;
        }

        public LevelObj Clone()
        {
            return new LevelObj(DeepCloneDictArr.DeepClone(Prop));
        }        

        object ICloneable.Clone()
        {
            return Clone();
        }

        //[Editor(typeof(LevelObjEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(DictionaryConverter))]
        [Description("This contains every property of this object")]
        public Dictionary<string, dynamic> Properties
        {
            get { return Prop; }
            set { Prop = value; }
        }
    }
}
