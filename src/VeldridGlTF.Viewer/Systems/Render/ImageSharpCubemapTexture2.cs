using System;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using Veldrid;

namespace VeldridGlTF.Viewer.Systems.Render
{
    public class ImageSharpCubemapTexture2
    {
        /// <summary>
        ///     Provides standardized access to the cubemap texture array
        /// </summary>
        private const int PositiveXArrayLayer = 0;

        private const int NegativeXArrayLayer = 1;
        private const int PostitiveYArrayLayer = 2;
        private const int NegativeYArrayLayer = 3;
        private const int PositiveZArrayLayer = 4;
        private const int NegativeZArrayLayer = 5;

        public ImageSharpCubemapTexture2(
            Image<Rgba32>[] positiveX,
            Image<Rgba32>[] negativeX,
            Image<Rgba32>[] positiveY,
            Image<Rgba32>[] negativeY,
            Image<Rgba32>[] positiveZ,
            Image<Rgba32>[] negativeZ)
        {
            CubemapTextures = new Image<Rgba32>[6][];
            if (positiveX.Length == 0) throw new ArgumentException("Texture should have at least one mip level.");
            if (positiveX.Length != negativeX.Length ||
                positiveX.Length != positiveY.Length ||
                positiveX.Length != negativeY.Length ||
                positiveX.Length != positiveZ.Length ||
                positiveX.Length != negativeZ.Length)
                throw new ArgumentException("Mip count doesn't match.");
            CubemapTextures[0] = positiveX;
            CubemapTextures[1] = negativeX;
            CubemapTextures[2] = positiveY;
            CubemapTextures[3] = negativeY;
            CubemapTextures[4] = positiveZ;
            CubemapTextures[5] = negativeZ;
        }

        /// <summary>
        ///     An array of images, each face of a cubemap.
        ///     Access of CubemapTextures[2][3] means face 2 with mipmap level 3
        /// </summary>
        public Image<Rgba32>[][] CubemapTextures { get; }

        /// <summary>
        ///     The width of a cubemap texture.
        /// </summary>
        public uint Width => (uint) CubemapTextures[0][0].Width;

        /// <summary>
        ///     The height of a cubemap texture.
        /// </summary>
        public uint Height => (uint) CubemapTextures[0][0].Height;

        /// <summary>
        ///     The pixel format cubemap textures.
        /// </summary>
        public PixelFormat Format => PixelFormat.R8_G8_B8_A8_UNorm;

        /// <summary>
        ///     The size of each pixel, in bytes.
        /// </summary>
        public uint PixelSizeInBytes => sizeof(byte) * 4;

        /// <summary>
        ///     The number of levels in the mipmap chain. This is equal to the length of the Images array.
        /// </summary>
        public uint MipLevels => (uint) CubemapTextures[0].Length;

        public unsafe Texture CreateDeviceTexture(GraphicsDevice gd, ResourceFactory factory)
        {
            var cubemapTexture = factory.CreateTexture(TextureDescription.Texture2D(
                Width,
                Height,
                MipLevels,
                1,
                Format,
                TextureUsage.Sampled | TextureUsage.Cubemap));

            for (var level = 0; level < MipLevels; level++)
                fixed (Rgba32* positiveXPin =
                    &MemoryMarshal.GetReference(CubemapTextures[PositiveXArrayLayer][level].GetPixelSpan()))
                fixed (Rgba32* negativeXPin =
                    &MemoryMarshal.GetReference(CubemapTextures[NegativeXArrayLayer][level].GetPixelSpan()))
                fixed (Rgba32* positiveYPin =
                    &MemoryMarshal.GetReference(CubemapTextures[PostitiveYArrayLayer][level].GetPixelSpan()))
                fixed (Rgba32* negativeYPin =
                    &MemoryMarshal.GetReference(CubemapTextures[NegativeYArrayLayer][level].GetPixelSpan()))
                fixed (Rgba32* positiveZPin =
                    &MemoryMarshal.GetReference(CubemapTextures[PositiveZArrayLayer][level].GetPixelSpan()))
                fixed (Rgba32* negativeZPin =
                    &MemoryMarshal.GetReference(CubemapTextures[NegativeZArrayLayer][level].GetPixelSpan()))
                {
                    var image = CubemapTextures[0][level];
                    var width = (uint) image.Width;
                    var height = (uint) image.Height;
                    var faceSize = width * height * PixelSizeInBytes;
                    gd.UpdateTexture(cubemapTexture, (IntPtr) positiveXPin, faceSize, 0, 0, 0, width, height, 1,
                        (uint) level, PositiveXArrayLayer);
                    gd.UpdateTexture(cubemapTexture, (IntPtr) negativeXPin, faceSize, 0, 0, 0, width, height, 1,
                        (uint) level, NegativeXArrayLayer);
                    gd.UpdateTexture(cubemapTexture, (IntPtr) positiveYPin, faceSize, 0, 0, 0, width, height, 1,
                        (uint) level, PostitiveYArrayLayer);
                    gd.UpdateTexture(cubemapTexture, (IntPtr) negativeYPin, faceSize, 0, 0, 0, width, height, 1,
                        (uint) level, NegativeYArrayLayer);
                    gd.UpdateTexture(cubemapTexture, (IntPtr) positiveZPin, faceSize, 0, 0, 0, width, height, 1,
                        (uint) level, PositiveZArrayLayer);
                    gd.UpdateTexture(cubemapTexture, (IntPtr) negativeZPin, faceSize, 0, 0, 0, width, height, 1,
                        (uint) level, NegativeZArrayLayer);
                }

            return cubemapTexture;
        }
    }
}