using NewMath;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageSharpExtensions
{
	public static class ISEFastDrawing
	{
		#region B over A
		public static void DrawOver(this Image<Argb32> source, Image<Argb32> overlay, int2 pos)
		{
			int2 overlayOffset = Math2.Max(-pos, 0);
			int2 srcOffset = Math2.Max(pos, 0);
			int2 overlapMax = new int2(Math.Min(pos.x + overlay.Width, source.Width), Math.Min(pos.y + overlay.Height, source.Height));
			int2 overlapSize = overlapMax - srcOffset;

			if (overlapSize.x <= 0 || overlapSize.y <= 0) return; // No overlap

			Parallel.For(0, overlapSize.y, y =>
			{
				var srcRow = source.DangerousGetPixelRowMemory(y + srcOffset.y).Span;
				var overlayRow = overlay.DangerousGetPixelRowMemory(y + overlayOffset.y).Span;

				var srcSlice = srcRow.Slice(srcOffset.x, overlapSize.x);
				var overlaySlice = overlayRow.Slice(overlayOffset.x, overlapSize.x);

				for (int x = 0; x < overlapSize.x; x++)
				{
					ref var srcPixel = ref srcSlice[x];
					var overlayPixel = overlaySlice[x];

					if (overlayPixel.A == byte.MinValue) continue; // Overlay pixel is fully transparent

					// Blend colors
					byte iOverlayAlpha = (byte)(byte.MaxValue - overlayPixel.A);
					byte alpha = (byte)(overlayPixel.A + srcPixel.A * iOverlayAlpha / byte.MaxValue);
					srcPixel.R = (byte)((overlayPixel.R * overlayPixel.A + srcPixel.R * srcPixel.A * iOverlayAlpha / byte.MaxValue) / alpha);
					srcPixel.G = (byte)((overlayPixel.G * overlayPixel.A + srcPixel.G * srcPixel.A * iOverlayAlpha / byte.MaxValue) / alpha);
					srcPixel.B = (byte)((overlayPixel.B * overlayPixel.A + srcPixel.B * srcPixel.A * iOverlayAlpha / byte.MaxValue) / alpha);
					srcPixel.A = alpha;
				}
			});
		}

		public static void DrawOver(this Image<Rgb24> source, Image<Argb32> overlay, int2 pos)
		{
			int2 overlayOffset = Math2.Max(-pos, 0);
			int2 srcOffset = Math2.Max(pos, 0);
			int2 overlapMax = new int2(Math.Min(pos.x + overlay.Width, source.Width), Math.Min(pos.y + overlay.Height, source.Height));
			int2 overlapSize = overlapMax - srcOffset;

			if (overlapSize.x <= 0 || overlapSize.y <= 0) return; // No overlap

			Parallel.For(0, overlapSize.y, y =>
			{
				var srcRow = source.DangerousGetPixelRowMemory(y + srcOffset.y).Span;
				var overlayRow = overlay.DangerousGetPixelRowMemory(y + overlayOffset.y).Span;

				var srcSlice = srcRow.Slice(srcOffset.x, overlapSize.x);
				var overlaySlice = overlayRow.Slice(overlayOffset.x, overlapSize.x);

				for (int x = 0; x < overlapSize.x; x++)
				{
					ref var srcPixel = ref srcSlice[x];
					var overlayPixel = overlaySlice[x];

					if (overlayPixel.A == byte.MinValue) continue; // Overlay pixel is fully transparent

					// Blend colors
					byte iOverlayAlpha = (byte)(byte.MaxValue - overlayPixel.A);
					srcPixel.R = (byte)((overlayPixel.R * overlayPixel.A + srcPixel.R * iOverlayAlpha) / byte.MaxValue);
					srcPixel.G = (byte)((overlayPixel.G * overlayPixel.A + srcPixel.G * iOverlayAlpha) / byte.MaxValue);
					srcPixel.B = (byte)((overlayPixel.B * overlayPixel.A + srcPixel.B * iOverlayAlpha) / byte.MaxValue);
				}
			});
		}

		public static void DrawOver(this Image<Argb32> source, Image<Rgb24> overlay, int2 pos)
		{
			int2 overlayOffset = Math2.Max(-pos, 0);
			int2 srcOffset = Math2.Max(pos, 0);
			int2 overlapMax = new int2(Math.Min(pos.x + overlay.Width, source.Width), Math.Min(pos.y + overlay.Height, source.Height));
			int2 overlapSize = overlapMax - srcOffset;

			if (overlapSize.x <= 0 || overlapSize.y <= 0) return; // No overlap

			Parallel.For(0, overlapSize.y, y =>
			{
				var srcRow = source.DangerousGetPixelRowMemory(y + srcOffset.y).Span;
				var overlayRow = overlay.DangerousGetPixelRowMemory(y + overlayOffset.y).Span;

				var srcSlice = srcRow.Slice(srcOffset.x, overlapSize.x);
				var overlaySlice = overlayRow.Slice(overlayOffset.x, overlapSize.x);

				for (int x = 0; x < overlapSize.x; x++)
				{
					ref var srcPixel = ref srcSlice[x];
					var overlayPixel = overlaySlice[x];

					srcPixel.R = overlayPixel.R;
					srcPixel.G = overlayPixel.G;
					srcPixel.B = overlayPixel.B;
					srcPixel.A = byte.MaxValue;
				}
			});
		}

		public static void DrawOver(this Image<Rgb24> source, Image<Rgb24> overlay, int2 pos)
		{
			int2 overlayOffset = Math2.Max(-pos, 0);
			int2 srcOffset = Math2.Max(pos, 0);
			int2 overlapMax = new int2(Math.Min(pos.x + overlay.Width, source.Width), Math.Min(pos.y + overlay.Height, source.Height));
			int2 overlapSize = overlapMax - srcOffset;

			if (overlapSize.x <= 0 || overlapSize.y <= 0) return; // No overlap

			for (int y = 0; y < overlapSize.y; y++)
			{
				var srcRow = source.DangerousGetPixelRowMemory(y + srcOffset.y).Span;
				var overlayRow = overlay.DangerousGetPixelRowMemory(y + overlayOffset.y).Span;

				var srcSlice = srcRow.Slice(srcOffset.x, overlapSize.x);
				var overlaySlice = overlayRow.Slice(overlayOffset.x, overlapSize.x);
				overlaySlice.CopyTo(srcSlice);
			}
		}
		#endregion

		#region B replace A
		public static void DrawReplace(this Image<Rgb24> source, Image<Argb32> overlay, int2 pos)
		{
			int2 overlayOffset = Math2.Max(-pos, 0);
			int2 srcOffset = Math2.Max(pos, 0);
			int2 overlapMax = new int2(Math.Min(pos.x + overlay.Width, source.Width), Math.Min(pos.y + overlay.Height, source.Height));
			int2 overlapSize = overlapMax - srcOffset;

			if (overlapSize.x <= 0 || overlapSize.y <= 0) return; // No overlap

			Parallel.For(0, overlapSize.y, y =>
			{
				var srcRow = source.DangerousGetPixelRowMemory(y + srcOffset.y).Span;
				var overlayRow = overlay.DangerousGetPixelRowMemory(y + overlayOffset.y).Span;

				var srcSlice = srcRow.Slice(srcOffset.x, overlapSize.x);
				var overlaySlice = overlayRow.Slice(overlayOffset.x, overlapSize.x);

				for (int x = 0; x < overlapSize.x; x++)
				{
					ref var srcPixel = ref srcSlice[x];
					var overlayPixel = overlaySlice[x];

					srcPixel.R = overlayPixel.R;
					srcPixel.G = overlayPixel.G;
					srcPixel.B = overlayPixel.B;
				}
			});
		}

		public static void DrawReplace(this Image<Argb32> source, Image<Rgb24> overlay, int2 pos)
		{
			int2 overlayOffset = Math2.Max(-pos, 0);
			int2 srcOffset = Math2.Max(pos, 0);
			int2 overlapMax = new int2(Math.Min(pos.x + overlay.Width, source.Width), Math.Min(pos.y + overlay.Height, source.Height));
			int2 overlapSize = overlapMax - srcOffset;

			if (overlapSize.x <= 0 || overlapSize.y <= 0) return; // No overlap

			Parallel.For(0, overlapSize.y, y =>
			{
				var srcRow = source.DangerousGetPixelRowMemory(y + srcOffset.y).Span;
				var overlayRow = overlay.DangerousGetPixelRowMemory(y + overlayOffset.y).Span;

				var srcSlice = srcRow.Slice(srcOffset.x, overlapSize.x);
				var overlaySlice = overlayRow.Slice(overlayOffset.x, overlapSize.x);

				for (int x = 0; x < overlapSize.x; x++)
				{
					ref var srcPixel = ref srcSlice[x];
					var overlayPixel = overlaySlice[x];

					srcPixel.R = overlayPixel.R;
					srcPixel.G = overlayPixel.G;
					srcPixel.B = overlayPixel.B;
					srcPixel.A = byte.MaxValue;
				}
			});
		}

		public static void DrawReplace<TPixel>(this Image<TPixel> source, Image<TPixel> overlay, int2 pos) where TPixel : unmanaged, IPixel<TPixel>
		{
			int2 overlayOffset = Math2.Max(-pos, 0);
			int2 srcOffset = Math2.Max(pos, 0);
			int2 overlapMax = new int2(Math.Min(pos.x + overlay.Width, source.Width), Math.Min(pos.y + overlay.Height, source.Height));
			int2 overlapSize = overlapMax - srcOffset;

			if (overlapSize.x <= 0 || overlapSize.y <= 0) return; // No overlap

			for (int y = 0; y < overlapSize.y; y++)
			{
				var srcRow = source.DangerousGetPixelRowMemory(y + srcOffset.y).Span;
				var overlayRow = overlay.DangerousGetPixelRowMemory(y + overlayOffset.y).Span;

				var srcSlice = srcRow.Slice(srcOffset.x, overlapSize.x);
				var overlaySlice = overlayRow.Slice(overlayOffset.x, overlapSize.x);
				overlaySlice.CopyTo(srcSlice);
			}
		}
		#endregion

		#region B mask A
		public static void DrawMask(this Image<Argb32> source, Image<Argb32> mask, int2 pos)
		{
			int2 overlayOffset = Math2.Max(-pos, 0);
			int2 srcOffset = Math2.Max(pos, 0);
			int2 overlapMax = new int2(Math.Min(pos.x + mask.Width, source.Width), Math.Min(pos.y + mask.Height, source.Height));
			int2 overlapSize = overlapMax - srcOffset;

			if (overlapSize.x <= 0 || overlapSize.y <= 0) return; // No overlap

			Parallel.For(0, overlapSize.y, y =>
			{
				var srcRow = source.DangerousGetPixelRowMemory(y + srcOffset.y).Span;
				var overlayRow = mask.DangerousGetPixelRowMemory(y + overlayOffset.y).Span;

				var srcSlice = srcRow.Slice(srcOffset.x, overlapSize.x);
				var overlaySlice = overlayRow.Slice(overlayOffset.x, overlapSize.x);

				for (int x = 0; x < overlapSize.x; x++)
				{
					ref var srcPixel = ref srcSlice[x];
					srcPixel.A = (byte)(srcPixel.A * overlaySlice[x].A / byte.MaxValue);
				}
			});
		}

		public static void DrawMask(this Image<Argb32> source, Image<L8> mask, int2 pos)
		{
			int2 overlayOffset = Math2.Max(-pos, 0);
			int2 srcOffset = Math2.Max(pos, 0);
			int2 overlapMax = new int2(Math.Min(pos.x + mask.Width, source.Width), Math.Min(pos.y + mask.Height, source.Height));
			int2 overlapSize = overlapMax - srcOffset;

			if (overlapSize.x <= 0 || overlapSize.y <= 0) return; // No overlap

			Parallel.For(0, overlapSize.y, y =>
			{
				var srcRow = source.DangerousGetPixelRowMemory(y + srcOffset.y).Span;
				var overlayRow = mask.DangerousGetPixelRowMemory(y + overlayOffset.y).Span;

				var srcSlice = srcRow.Slice(srcOffset.x, overlapSize.x);
				var overlaySlice = overlayRow.Slice(overlayOffset.x, overlapSize.x);

				for (int x = 0; x < overlapSize.x; x++)
				{
					ref var srcPixel = ref srcSlice[x];
					srcPixel.A = (byte)(srcPixel.A * overlaySlice[x].PackedValue / byte.MaxValue);
				}
			});
		}

		public static void DrawMask(this Image<Argb32> source, Image<L16> mask, int2 pos)
		{
			int2 overlayOffset = Math2.Max(-pos, 0);
			int2 srcOffset = Math2.Max(pos, 0);
			int2 overlapMax = new int2(Math.Min(pos.x + mask.Width, source.Width), Math.Min(pos.y + mask.Height, source.Height));
			int2 overlapSize = overlapMax - srcOffset;

			if (overlapSize.x <= 0 || overlapSize.y <= 0) return; // No overlap

			Parallel.For(0, overlapSize.y, y =>
			{
				var srcRow = source.DangerousGetPixelRowMemory(y + srcOffset.y).Span;
				var overlayRow = mask.DangerousGetPixelRowMemory(y + overlayOffset.y).Span;

				var srcSlice = srcRow.Slice(srcOffset.x, overlapSize.x);
				var overlaySlice = overlayRow.Slice(overlayOffset.x, overlapSize.x);

				for (int x = 0; x < overlapSize.x; x++)
				{
					ref var srcPixel = ref srcSlice[x];
					srcPixel.A = (byte)(srcPixel.A * overlaySlice[x].PackedValue / ushort.MaxValue);
				}
			});
		}

		public static void DrawMask(this Image<Argb32> source, Image<La16> mask, int2 pos)
		{
			int2 overlayOffset = Math2.Max(-pos, 0);
			int2 srcOffset = Math2.Max(pos, 0);
			int2 overlapMax = new int2(Math.Min(pos.x + mask.Width, source.Width), Math.Min(pos.y + mask.Height, source.Height));
			int2 overlapSize = overlapMax - srcOffset;

			if (overlapSize.x <= 0 || overlapSize.y <= 0) return; // No overlap

			Parallel.For(0, overlapSize.y, y =>
			{
				var srcRow = source.DangerousGetPixelRowMemory(y + srcOffset.y).Span;
				var overlayRow = mask.DangerousGetPixelRowMemory(y + overlayOffset.y).Span;

				var srcSlice = srcRow.Slice(srcOffset.x, overlapSize.x);
				var overlaySlice = overlayRow.Slice(overlayOffset.x, overlapSize.x);

				for (int x = 0; x < overlapSize.x; x++)
				{
					ref var srcPixel = ref srcSlice[x];
					srcPixel.A = (byte)(srcPixel.A * overlaySlice[x].A / byte.MaxValue);
				}
			});
		}

		public static void DrawMask(this Image<Argb32> source, Image<La32> mask, int2 pos)
		{
			int2 overlayOffset = Math2.Max(-pos, 0);
			int2 srcOffset = Math2.Max(pos, 0);
			int2 overlapMax = new int2(Math.Min(pos.x + mask.Width, source.Width), Math.Min(pos.y + mask.Height, source.Height));
			int2 overlapSize = overlapMax - srcOffset;

			if (overlapSize.x <= 0 || overlapSize.y <= 0) return; // No overlap

			Parallel.For(0, overlapSize.y, y =>
			{
				var srcRow = source.DangerousGetPixelRowMemory(y + srcOffset.y).Span;
				var overlayRow = mask.DangerousGetPixelRowMemory(y + overlayOffset.y).Span;

				var srcSlice = srcRow.Slice(srcOffset.x, overlapSize.x);
				var overlaySlice = overlayRow.Slice(overlayOffset.x, overlapSize.x);

				for (int x = 0; x < overlapSize.x; x++)
				{
					ref var srcPixel = ref srcSlice[x];
					srcPixel.A = (byte)(srcPixel.A * overlaySlice[x].A / ushort.MaxValue);
				}
			});
		}

		public static void DrawMask(this Image<L8> source, Image<L8> mask, int2 pos)
		{
			int2 overlayOffset = Math2.Max(-pos, 0);
			int2 srcOffset = Math2.Max(pos, 0);
			int2 overlapMax = new int2(Math.Min(pos.x + mask.Width, source.Width), Math.Min(pos.y + mask.Height, source.Height));
			int2 overlapSize = overlapMax - srcOffset;

			if (overlapSize.x <= 0 || overlapSize.y <= 0) return; // No overlap

			Parallel.For(0, overlapSize.y, y =>
			{
				var srcRow = source.DangerousGetPixelRowMemory(y + srcOffset.y).Span;
				var overlayRow = mask.DangerousGetPixelRowMemory(y + overlayOffset.y).Span;

				var srcSlice = srcRow.Slice(srcOffset.x, overlapSize.x);
				var overlaySlice = overlayRow.Slice(overlayOffset.x, overlapSize.x);

				for (int x = 0; x < overlapSize.x; x++)
				{
					ref var srcPixel = ref srcSlice[x];
					srcPixel.PackedValue = (byte)(srcPixel.PackedValue * overlaySlice[x].PackedValue / byte.MaxValue);
				}
			});
		}

		public static void DrawMask(this Image<L16> source, Image<L16> mask, int2 pos)
		{
			int2 overlayOffset = Math2.Max(-pos, 0);
			int2 srcOffset = Math2.Max(pos, 0);
			int2 overlapMax = new int2(Math.Min(pos.x + mask.Width, source.Width), Math.Min(pos.y + mask.Height, source.Height));
			int2 overlapSize = overlapMax - srcOffset;

			if (overlapSize.x <= 0 || overlapSize.y <= 0) return; // No overlap

			Parallel.For(0, overlapSize.y, y =>
			{
				var srcRow = source.DangerousGetPixelRowMemory(y + srcOffset.y).Span;
				var overlayRow = mask.DangerousGetPixelRowMemory(y + overlayOffset.y).Span;

				var srcSlice = srcRow.Slice(srcOffset.x, overlapSize.x);
				var overlaySlice = overlayRow.Slice(overlayOffset.x, overlapSize.x);

				for (int x = 0; x < overlapSize.x; x++)
				{
					ref var srcPixel = ref srcSlice[x];
					srcPixel.PackedValue = (ushort)(srcPixel.PackedValue * overlaySlice[x].PackedValue / ushort.MaxValue);
				}
			});
		}

		public static void DrawMask(this Image<L16> source, Image<L8> mask, int2 pos)
		{
			int2 overlayOffset = Math2.Max(-pos, 0);
			int2 srcOffset = Math2.Max(pos, 0);
			int2 overlapMax = new int2(Math.Min(pos.x + mask.Width, source.Width), Math.Min(pos.y + mask.Height, source.Height));
			int2 overlapSize = overlapMax - srcOffset;

			if (overlapSize.x <= 0 || overlapSize.y <= 0) return; // No overlap

			Parallel.For(0, overlapSize.y, y =>
			{
				var srcRow = source.DangerousGetPixelRowMemory(y + srcOffset.y).Span;
				var overlayRow = mask.DangerousGetPixelRowMemory(y + overlayOffset.y).Span;

				var srcSlice = srcRow.Slice(srcOffset.x, overlapSize.x);
				var overlaySlice = overlayRow.Slice(overlayOffset.x, overlapSize.x);

				for (int x = 0; x < overlapSize.x; x++)
				{
					ref var srcPixel = ref srcSlice[x];
					srcPixel.PackedValue = (ushort)(srcPixel.PackedValue * overlaySlice[x].PackedValue / byte.MaxValue);
				}
			});
		}

		public static void DrawMask(this Image<L8> source, Image<L16> mask, int2 pos)
		{
			int2 overlayOffset = Math2.Max(-pos, 0);
			int2 srcOffset = Math2.Max(pos, 0);
			int2 overlapMax = new int2(Math.Min(pos.x + mask.Width, source.Width), Math.Min(pos.y + mask.Height, source.Height));
			int2 overlapSize = overlapMax - srcOffset;

			if (overlapSize.x <= 0 || overlapSize.y <= 0) return; // No overlap

			Parallel.For(0, overlapSize.y, y =>
			{
				var srcRow = source.DangerousGetPixelRowMemory(y + srcOffset.y).Span;
				var overlayRow = mask.DangerousGetPixelRowMemory(y + overlayOffset.y).Span;

				var srcSlice = srcRow.Slice(srcOffset.x, overlapSize.x);
				var overlaySlice = overlayRow.Slice(overlayOffset.x, overlapSize.x);

				for (int x = 0; x < overlapSize.x; x++)
				{
					ref var srcPixel = ref srcSlice[x];
					srcPixel.PackedValue = (byte)(srcPixel.PackedValue * overlaySlice[x].PackedValue / ushort.MaxValue);
				}
			});
		}

		public static void DrawMask(this Image<L8> source, Image<Argb32> mask, int2 pos)
		{
			int2 overlayOffset = Math2.Max(-pos, 0);
			int2 srcOffset = Math2.Max(pos, 0);
			int2 overlapMax = new int2(Math.Min(pos.x + mask.Width, source.Width), Math.Min(pos.y + mask.Height, source.Height));
			int2 overlapSize = overlapMax - srcOffset;

			if (overlapSize.x <= 0 || overlapSize.y <= 0) return; // No overlap

			Parallel.For(0, overlapSize.y, y =>
			{
				var srcRow = source.DangerousGetPixelRowMemory(y + srcOffset.y).Span;
				var overlayRow = mask.DangerousGetPixelRowMemory(y + overlayOffset.y).Span;

				var srcSlice = srcRow.Slice(srcOffset.x, overlapSize.x);
				var overlaySlice = overlayRow.Slice(overlayOffset.x, overlapSize.x);

				for (int x = 0; x < overlapSize.x; x++)
				{
					ref var srcPixel = ref srcSlice[x];
					srcPixel.PackedValue = (byte)(srcPixel.PackedValue * overlaySlice[x].A / byte.MaxValue);
				}
			});
		}

		public static void DrawMask(this Image<L16> source, Image<Argb32> mask, int2 pos)
		{
			int2 overlayOffset = Math2.Max(-pos, 0);
			int2 srcOffset = Math2.Max(pos, 0);
			int2 overlapMax = new int2(Math.Min(pos.x + mask.Width, source.Width), Math.Min(pos.y + mask.Height, source.Height));
			int2 overlapSize = overlapMax - srcOffset;

			if (overlapSize.x <= 0 || overlapSize.y <= 0) return; // No overlap

			Parallel.For(0, overlapSize.y, y =>
			{
				var srcRow = source.DangerousGetPixelRowMemory(y + srcOffset.y).Span;
				var overlayRow = mask.DangerousGetPixelRowMemory(y + overlayOffset.y).Span;

				var srcSlice = srcRow.Slice(srcOffset.x, overlapSize.x);
				var overlaySlice = overlayRow.Slice(overlayOffset.x, overlapSize.x);

				for (int x = 0; x < overlapSize.x; x++)
				{
					ref var srcPixel = ref srcSlice[x];
					srcPixel.PackedValue = (ushort)(srcPixel.PackedValue * overlaySlice[x].A / byte.MaxValue);
				}
			});
		}
		#endregion
	}
}