using SixLabors.ImageSharp.PixelFormats;

namespace ImageSharpExtensions
{
	public static class ISEPosImageExtensions
	{
		#region FastDrawing Methods
		private static bool PreDraw<TPixel1, TPixel2>(PositionedImage<TPixel1> source, PositionedImage<TPixel2> image, bool expand)
			where TPixel1 : unmanaged, IPixel<TPixel1>
			where TPixel2 : unmanaged, IPixel<TPixel2>
		{
			if (image.Image is null) return false;
			if (expand) source.ExpandToContain(image.Pos, image.Size);
			else if (source.Image is null || !ISEUtils.Overlaps(source.Pos, source.Size, image.Pos, image.Size)) return false; // Image bounds do not overlap
			return true;
		}

		#region Draw Over
		public static void DrawOver(this PositionedImage<Argb32> source, PositionedImage<Argb32> image, bool expand = false)
		{
			if (PreDraw(source, image, expand))
				source.Image!.DrawOver(image.Image!, image.Pos - source.Pos);
		}

		public static void DrawOver(this PositionedImage<Argb32> source, PositionedImage<Rgb24> image, bool expand = false)
		{
			if (PreDraw(source, image, expand))
				source.Image!.DrawOver(image.Image!, image.Pos - source.Pos);
		}

		public static void DrawOver(this PositionedImage<Rgb24> source, PositionedImage<Argb32> image, bool expand = false)
		{
			if (PreDraw(source, image, expand))
				source.Image!.DrawOver(image.Image!, image.Pos - source.Pos);
		}

		public static void DrawOver(this PositionedImage<Rgb24> source, PositionedImage<Rgb24> image, bool expand = false)
		{
			if (PreDraw(source, image, expand))
				source.Image!.DrawOver(image.Image!, image.Pos - source.Pos);
		}
		#endregion

		#region Draw Replace
		public static void DrawReplace(this PositionedImage<Argb32> source, PositionedImage<Rgb24> image, bool expand = false)
		{
			if (PreDraw(source, image, expand))
				source.Image!.DrawReplace(image.Image!, image.Pos - source.Pos);
		}

		public static void DrawReplace(this PositionedImage<Rgb24> source, PositionedImage<Argb32> image, bool expand = false)
		{
			if (PreDraw(source, image, expand))
				source.Image!.DrawReplace(image.Image!, image.Pos - source.Pos);
		}
		#endregion

		#region Draw Mask
		public static void DrawMask(this PositionedImage<Argb32> source, PositionedImage<Argb32> image)
		{
			if (PreDraw(source, image, false))
				source.Image!.DrawMask(image.Image!, image.Pos - source.Pos);
		}

		public static void DrawMask(this PositionedImage<Argb32> source, PositionedImage<L8> image)
		{
			if (PreDraw(source, image, false))
				source.Image!.DrawMask(image.Image!, image.Pos - source.Pos);
		}

		public static void DrawMask(this PositionedImage<Argb32> source, PositionedImage<L16> image)
		{
			if (PreDraw(source, image, false))
				source.Image!.DrawMask(image.Image!, image.Pos - source.Pos);
		}

		public static void DrawMask(this PositionedImage<Argb32> source, PositionedImage<La16> image)
		{
			if (PreDraw(source, image, false))
				source.Image!.DrawMask(image.Image!, image.Pos - source.Pos);
		}

		public static void DrawMask(this PositionedImage<Argb32> source, PositionedImage<La32> image)
		{
			if (PreDraw(source, image, false))
				source.Image!.DrawMask(image.Image!, image.Pos - source.Pos);
		}

		public static void DrawMask(this PositionedImage<L8> source, PositionedImage<L8> image)
		{
			if (PreDraw(source, image, false))
				source.Image!.DrawMask(image.Image!, image.Pos - source.Pos);
		}

		public static void DrawMask(this PositionedImage<L8> source, PositionedImage<L16> image)
		{
			if (PreDraw(source, image, false))
				source.Image!.DrawMask(image.Image!, image.Pos - source.Pos);
		}

		public static void DrawMask(this PositionedImage<L16> source, PositionedImage<L8> image)
		{
			if (PreDraw(source, image, false))
				source.Image!.DrawMask(image.Image!, image.Pos - source.Pos);
		}

		public static void DrawMask(this PositionedImage<L16> source, PositionedImage<L16> image)
		{
			if (PreDraw(source, image, false))
				source.Image!.DrawMask(image.Image!, image.Pos - source.Pos);
		}

		public static void DrawMask(this PositionedImage<L8> source, PositionedImage<Argb32> image)
		{
			if (PreDraw(source, image, false))
				source.Image!.DrawMask(image.Image!, image.Pos - source.Pos);
		}

		public static void DrawMask(this PositionedImage<L16> source, PositionedImage<Argb32> image)
		{
			if (PreDraw(source, image, false))
				source.Image!.DrawMask(image.Image!, image.Pos - source.Pos);
		}
		#endregion
		#endregion
	}
}
