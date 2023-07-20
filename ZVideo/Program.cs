using System.Drawing;
using System.Runtime.CompilerServices;
using FFMediaToolkit;
using FFMediaToolkit.Decoding;
using FFMediaToolkit.Graphics;
using FFmpeg.AutoGen;

// using OpenCvSharp;
using static System.Runtime.InteropServices.JavaScript.JSType;
using uint8_t = System.Byte;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using Color = System.Drawing.Color;
using Size = SixLabors.ImageSharp.Size;

// using Color = SixLabors.ImageSharp.Color;

namespace ZVideo;

public static class Util
{
	public static Image<T> ToBitmap<T>(this ImageData imageData) where T : unmanaged, IPixel, IPixel<T>
	{
		return Image.LoadPixelData<T>(imageData.Data, imageData.ImageSize.Width, imageData.ImageSize.Height);
	}
}

internal class Program
{

	public static T GetMostUsedColor<T>(Image<T> image) where T : unmanaged, IPixel, IPixel<T>
	{
		image.Mutate(
			x => x
				// Scale the image down preserving the aspect ratio. This will speed up quantization.
				// We use nearest neighbor as it will be the fastest approach.
				.Resize(new ResizeOptions() { Sampler = KnownResamplers.NearestNeighbor, Size = new Size(100, 0) })
				// Reduce the color palette to 1 color without dithering.
				.Quantize(new OctreeQuantizer(new QuantizerOptions() { Dither = null, MaxColors = 1 })));

		T dominant = image[0, 0];
		return dominant;
	}

	static unsafe int Main(string[] args)
	{
		var l = @"C:\Users\Deci\scoop\apps\ffmpeg-shared\current\bin\";

		var f  = @"C:\Users\Deci\Downloads\halo epic.mp4";
		FFmpegLoader.FFmpegPath = l;
		var mf = MediaFile.Open(f, new MediaOptions() { });

		while (mf.Video.TryGetNextFrame(out var imageData)) {
			var bm = imageData.ToBitmap<Rgb24>();

			// See the #Usage details for example .ToBitmap() implementation
			// The .Save() method may be different depending on your graphics library
			Console.WriteLine(GetMostUsedColor(bm));
		
		}

		return 0;
	}
}