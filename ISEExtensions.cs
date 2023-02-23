using NewMath;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageSharpExtensions
{
	public static class ISEExtensions
	{
		public static Image<TPixel> GetSubimage<TPixel>(this Image<TPixel> image, int2 pos, int2 size) where TPixel : unmanaged, IPixel<TPixel>
		{
			if (size.x <= 0 || size.y <= 0) return new Image<TPixel>(1, 1);
			var subimage = new Image<TPixel>(size.x, size.y);

			int2 subimageOffset = Math2.Max(-pos, 0);
			int2 srcOffset = Math2.Max(pos, 0);
			int2 overlapMax = Math2.Min(pos + size, new int2(image.Width, image.Height));
			int2 overlapSize = overlapMax - srcOffset;

			if (overlapSize.x <= 0 || overlapSize.y <= 0) return subimage; // No overlap

			for (int y = 0; y < overlapSize.y; y++)
			{
				var srcRow = image.DangerousGetPixelRowMemory(y + srcOffset.y).Span;
				var subRow = subimage.DangerousGetPixelRowMemory(y + subimageOffset.y).Span;

				Span<TPixel> srcSlice = srcRow.Slice(srcOffset.x, overlapSize.x);
				Span<TPixel> destSlice = subRow.Slice(subimageOffset.x, overlapSize.x);
				srcSlice.CopyTo(destSlice);
			}

			return subimage;
		}
	}
}
