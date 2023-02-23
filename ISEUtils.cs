using NewMath;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Runtime.CompilerServices;

namespace ImageSharpExtensions
{
	public static class ISEUtils
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool RectContains(int2 pos1, int2 size1, int2 pos2, int2 size2)
		{
			return pos2 >= pos1 && pos2 + size2 < pos1 + size1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Overlaps(int2 pos1, int2 size1, int2 pos2, int2 size2)
		{
			return pos1 < pos2 + size2 && pos2 < pos1 + size1;
		}

		public static Rectangle Clamp(Rectangle rect, Rectangle bounds)
		{
			int x = Math.Max(rect.X, bounds.X);
			int y = Math.Max(rect.Y, bounds.Y);
			return new Rectangle(
				x, y,
				Math.Min(rect.X + rect.Width, bounds.X + bounds.Width) - x,
				Math.Min(rect.Y + rect.Height, bounds.Y + bounds.Height) - y
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 ToDouble4(this Argb32 argb32) => new double4(argb32.R, argb32.G, argb32.B, argb32.A);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 ToInt4(this Argb32 argb32) => new int4(argb32.R, argb32.G, argb32.B, argb32.A);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Argb32 ToArgb32(this int4 color) => new Argb32(color.x, color.y, color.z, color.w);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Argb32 ToArgb32(this double4 color) => new Argb32(
			(byte)Math.Clamp(Math.Round(color.x, MidpointRounding.AwayFromZero), 0, 255),
			(byte)Math.Clamp(Math.Round(color.y, MidpointRounding.AwayFromZero), 0, 255),
			(byte)Math.Clamp(Math.Round(color.z, MidpointRounding.AwayFromZero), 0, 255),
			(byte)Math.Clamp(Math.Round(color.w, MidpointRounding.AwayFromZero), 0, 255));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 Clamp(this int4 n, int min, int max) => new int4(Math.Clamp(n.x, min, max), Math.Clamp(n.y, min, max), Math.Clamp(n.z, min, max), Math.Clamp(n.w, min, max));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 Clamp(this double4 n, int min, int max) => new double4(Math.Clamp(n.x, min, max), Math.Clamp(n.y, min, max), Math.Clamp(n.z, min, max), Math.Clamp(n.w, min, max));
	}
}
