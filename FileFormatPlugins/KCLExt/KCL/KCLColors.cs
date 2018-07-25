using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioKart.MK7
{
	class KCLColors
	{
		public enum CollisionType_MK8D : ushort
		{
			Road_Default = 0,
			Road_Bumpy = 2,
			Road_Sand = 4,
			Offroad_Sand = 6,
			Road_HeavySand = 8,
			Road_IcyRoad = 9,
			OrangeBooster = 10,
			AntiGravityPanel = 11,
			Latiku = 16,
			Wall5 = 17,
			Wall4 = 19,
			Wall = 23,
			Latiku2 = 28,
			Glider = 31,
			SidewalkSlope = 32,
			Road_Dirt = 33,
			Unsolid = 56,
			Water = 60,
			Road_Stone = 64,
			Wall1 = 81,
			Wall2 = 84,
			FinishLine = 93,
			RedFlowerEffect = 95,
			Wall3 = 113,
			WhiteFlowerEffect = 127,
			Road_Metal = 128,
			Road_3DS_MP_Piano = 129,
			Road_RoyalR_Grass = 134,
			TopPillar = 135,
			YoshiCuiruit_Grass = 144,
			YellowFlowerEffect = 159,

			Road_MetalGating = 160,
			Road_3DS_MP_Xylophone = 161,
			Road_3DS_MP_Vibraphone = 193,
			SNES_RR_road = 227,
			Offroad_Mud = 230,
			Trick = 4096,
			BoosterStunt = 4106,
			TrickEndOfRamp = 4108,
			Trick3 = 4130,
			Trick6 = 4160,
			Trick4 = 4224,
			Trick5 = 8192,
			BoostTrick = 8202,
		}

		public static Color GetMaterialColor(ushort coll)
		{
			switch (coll)
			{
				case (ushort)CollisionType_MK8D.Road_Default:
					return Color.DarkGray;
				case (ushort)CollisionType_MK8D.Glider:
					return Color.Orange;
				case (ushort)CollisionType_MK8D.Road_Sand:
					return Color.LightYellow;
				case (ushort)CollisionType_MK8D.Offroad_Sand:
					return Color.SandyBrown;
				case (ushort)CollisionType_MK8D.Water:
					return Color.Blue;
				case (ushort)CollisionType_MK8D.Wall1:
					return Color.LightSlateGray;
				case (ushort)CollisionType_MK8D.Wall2:
					return Color.OrangeRed;
				case (ushort)CollisionType_MK8D.Wall3:
					return Color.IndianRed;
				case (ushort)CollisionType_MK8D.Unsolid:
					return Color.Beige;
				case (ushort)CollisionType_MK8D.Road_3DS_MP_Piano:
					return Color.RosyBrown;
				case (ushort)CollisionType_MK8D.Road_3DS_MP_Vibraphone:
					return Color.BurlyWood;
				case (ushort)CollisionType_MK8D.Road_3DS_MP_Xylophone:
					return Color.DarkSalmon;
				case (ushort)CollisionType_MK8D.Latiku:
					return Color.GhostWhite;
				case (ushort)CollisionType_MK8D.Road_Bumpy:
					return Color.GreenYellow;
				case (ushort)CollisionType_MK8D.Road_RoyalR_Grass:
					return Color.Green;
				case (ushort)CollisionType_MK8D.YoshiCuiruit_Grass:
					return Color.Green;
				case (ushort)CollisionType_MK8D.Wall:
					return Color.LightCyan;
				case (ushort)CollisionType_MK8D.Wall4:
					return Color.LightSlateGray;
				case (ushort)CollisionType_MK8D.Wall5:
					return Color.DarkSlateGray;
				case (ushort)CollisionType_MK8D.AntiGravityPanel:
					return Color.Purple;
				case (ushort)CollisionType_MK8D.SidewalkSlope:
					return Color.FromArgb(153, 153, 102);
				case (ushort)CollisionType_MK8D.BoostTrick:
					return Color.DarkOrange;
				case (ushort)CollisionType_MK8D.Offroad_Mud:
					return Color.FromArgb(77, 26, 0);
				case (ushort)CollisionType_MK8D.Road_Metal:
					return Color.FromArgb(80, 80, 80);
				case (ushort)CollisionType_MK8D.Road_MetalGating:
					return Color.FromArgb(64, 64, 64);
				case (ushort)CollisionType_MK8D.Road_Dirt:
					return Color.Sienna;
				case (ushort)CollisionType_MK8D.Road_Stone:
					return Color.FromArgb(50, 50, 50);
				case (ushort)CollisionType_MK8D.Latiku2:
					return Color.WhiteSmoke;
				case (ushort)CollisionType_MK8D.RedFlowerEffect:
					return Color.MediumVioletRed;
				case (ushort)CollisionType_MK8D.WhiteFlowerEffect:
					return Color.FloralWhite;
				case (ushort)CollisionType_MK8D.YellowFlowerEffect:
					return Color.Yellow;
				case (ushort)CollisionType_MK8D.TopPillar:
					return Color.Gray;
				default:
					return Color.FromArgb(20, 20, 20);
			}
		}
	}
}
