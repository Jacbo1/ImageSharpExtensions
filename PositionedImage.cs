using NewMath;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp;

namespace ImageSharpExtensions
{
	public class PositionedImage<TPixel> : IDisposable where TPixel : unmanaged, IPixel<TPixel>
	{
		#region Fields
		public Image<TPixel>? Image;
		public Action<int2>? ExpandAction;
		public Action? ImageCreatedAction;
		public int2 RealPos;
		#endregion

		#region Properties
		public int Width { get => Image?.Width ?? 0; }
		public int Height { get => Image?.Height ?? 0; }

		public int X
		{
			get => Pos.x;
			set => RealPos.x = value;
		}

		public int Y
		{
			get => Pos.y;
			set => RealPos.y = value;
		}

		public int2 Pos
		{
			get => Image is null ? int2.Zero : RealPos;
			set => RealPos = value;
		}

		public int2 Size
		{
			get
			{
				if (Image is null) return int2.Zero;
				return new int2(Image.Width, Image.Height);
			}
			set
			{
				if (Image is null)
				{
					// No image
					Image = new(value.x, value.y);
					if (ImageCreatedAction is not null) ImageCreatedAction();
					return;
				}

				// Valid image
				if (value.x == Image.Width && value.y == Image.Height) return; // Same size

				// Different size
				var temp = Image.GetSubimage(int2.Zero, value);
				Image.Dispose();
				Image = temp;
			}
		}
		#endregion

		#region Constructors
		public PositionedImage() { }

		public PositionedImage(int2 pos)
		{
			Pos = pos;
		}

		public PositionedImage(int2 pos, int2 size)
		{
			Pos = pos;
			Size = size;
		}

		public PositionedImage(Image<TPixel>? image)
		{
			Image = image;
		}

		public PositionedImage(int2 pos, Image<TPixel>? image)
		{
			Pos = pos;
			Image = image;
		}
		#endregion

		#region Public Methods
		public bool ExpandToContain(int2 pos, int2 size, out int2 deltaPos)
		{
			if (Image is null)
			{
				// Image is null
				Pos = pos;
				Size = size;
				deltaPos = int2.Zero;
				if (ExpandAction is not null) ExpandAction(deltaPos);
				return true;
			}

			int2 mySize = new int2(Image.Width, Image.Height);
			if (ISEUtils.RectContains(Pos, mySize, pos, size))
			{
				// Region within bounds
				deltaPos = int2.Zero;
				return false;
			}

			// Extend Image
			mySize = Math2.Max(pos + size, Pos + mySize) - Math2.Min(pos, Pos);

			deltaPos = Math2.Max(Pos - pos, 0);
			var img = Image.GetSubimage(-deltaPos, mySize);
			Image.Dispose();
			Image = img;
			Pos -= deltaPos;

			if (ExpandAction is not null) ExpandAction(deltaPos);
			return true;
		}

		public bool ExpandToContain(int2 pos, int2 size) => ExpandToContain(pos, size, out int2 _);

		public bool ExpandToContain(Rectangle rect) => ExpandToContain(new int2(rect.X, rect.Y), new int2(rect.Width, rect.Height), out int2 _);

		public PositionedImage<TPixel> Clone()
		{
			return new PositionedImage<TPixel>(RealPos, Image?.Clone());
		}

		public PositionedImage<TPixel2> ClonseAs<TPixel2>() where TPixel2 : unmanaged, IPixel<TPixel2>
		{
			return new PositionedImage<TPixel2>(RealPos, Image?.CloneAs<TPixel2>());
		}

		public void Mutate(Action<IImageProcessingContext> action) => Image?.Mutate(action);

		public void ProcessPixelRows(PixelAccessorAction<TPixel> processPixels) => Image?.ProcessPixelRows(processPixels);

		public void DrawImage<TPixel2>(PositionedImage<TPixel2> image, bool expand = false) where TPixel2 : unmanaged, IPixel<TPixel2>
		{
			if (expand) ExpandToContain(image.Pos, image.Size);
			else if (Image is null || !ISEUtils.Overlaps(Pos, Size, image.Pos, image.Size)) return; // Image bounds do not overlap

			int2 offset = image.Pos - Pos;
			Image.Mutate(op => op.DrawImage(image.Image, new Point(offset.x, offset.y), 1));
		}

		public void DrawImage<TPixel2>(PositionedImage<TPixel2> image, PixelAlphaCompositionMode alphaCompositeMode, bool expand = false) where TPixel2 : unmanaged, IPixel<TPixel2>
		{
			if (expand) ExpandToContain(image.Pos, image.Size);
			else if (Image is null || !ISEUtils.Overlaps(Pos, Size, image.Pos, image.Size)) return; // Image bounds do not overlap

			int2 offset = image.Pos - Pos;
			Image.Mutate(op => op.DrawImage(image.Image, new Point(offset.x, offset.y), PixelColorBlendingMode.Normal, alphaCompositeMode, 1));
		}

		public void DrawReplace(PositionedImage<TPixel> image, bool expand = false)
		{
			if (image.Image is null) return;
			if (expand) ExpandToContain(image.Pos, image.Size);
			else if (Image is null || !ISEUtils.Overlaps(Pos, Size, image.Pos, image.Size)) return; // Image bounds do not overlap
			Image!.DrawReplace(image.Image, image.Pos - Pos);
		}

		public void Crop(int2 pos, int2 size, bool expand = false)
		{
			if (Image is null)
			{
				if (expand) ExpandToContain(pos, size);
				return;
			}

			if (!ISEUtils.Overlaps(Pos, Size, pos, size))
			{
				// Target rect does not overlap image
				if (expand)
				{
					Size = size;
					int2 deltaPos_ = Pos - pos;
					Pos = pos;
					return;
				}

				Pos = pos;
				Image.Dispose();
				Image = null;
				return;
			}

			if (expand) ExpandToContain(pos, size); // Expand image

			Rectangle target = new(pos.x, pos.y, size.x, size.y);
			Rectangle imageBounds = new(Pos.x, Pos.y, Image.Width, Image.Height);
			var clampedRect = ISEUtils.Clamp(target, imageBounds);
			Rectangle region = new(clampedRect.X - imageBounds.X, clampedRect.Y - imageBounds.Y, clampedRect.Width, clampedRect.Height);
			Image.Mutate(op => op.Crop(region));
			int2 deltaPos = Pos - new int2(clampedRect.X, clampedRect.Y);
			RealPos.x = clampedRect.X;
			RealPos.y = clampedRect.Y;
		}

		public PositionedImage<TPixel> GetPositionedSubimage(int2 pos, int2 size)
		{
			return new PositionedImage<TPixel>(pos, Image?.GetSubimage(pos - Pos, size));
		}

		public void Dispose()
		{
			Image?.Dispose();
			Image = null;
		}

		public static implicit operator Image<TPixel>?(PositionedImage<TPixel> image) { return image.Image; }

		public static implicit operator PositionedImage<TPixel>(Image<TPixel>? image) { return new(image); }
		#endregion
	}
}
