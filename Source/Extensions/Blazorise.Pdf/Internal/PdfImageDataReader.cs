#region Using directives
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
#endregion

namespace Blazorise.Pdf;

internal static class PdfImageDataReader
{
    #region Methods

    internal static bool TryRead( PdfResourceContent resource, out PdfImageData imageData )
    {
        imageData = null;

        if ( resource?.Data is not { Length: > 0 } data )
            return false;

        if ( ( IsJpegMediaType( resource.MediaType ) || HasJpegSignature( data ) ) && TryReadJpegInfo( data, out int jpegWidth, out int jpegHeight, out int componentCount, out int bitsPerComponent ) )
        {
            imageData = new()
            {
                Data = data,
                Width = jpegWidth,
                Height = jpegHeight,
                ColorSpace = ResolveImageColorSpace( componentCount ),
                BitsPerComponent = bitsPerComponent,
                Filter = "/DCTDecode",
            };

            return true;
        }

        if ( ( IsPngMediaType( resource.MediaType ) || HasPngSignature( data ) ) && TryCreatePngImageData( data, out imageData ) )
            return true;

        return false;
    }

    private static bool IsJpegMediaType( string mediaType )
    {
        return string.Equals( mediaType, "image/jpeg", StringComparison.OrdinalIgnoreCase )
            || string.Equals( mediaType, "image/jpg", StringComparison.OrdinalIgnoreCase );
    }

    private static bool IsPngMediaType( string mediaType )
    {
        return string.Equals( mediaType, "image/png", StringComparison.OrdinalIgnoreCase );
    }

    private static bool HasJpegSignature( byte[] data )
    {
        return data.Length >= 2 && data[0] == 0xFF && data[1] == 0xD8;
    }

    private static string ResolveImageColorSpace( int componentCount )
    {
        return componentCount switch
        {
            1 => "/DeviceGray",
            4 => "/DeviceCMYK",
            _ => "/DeviceRGB",
        };
    }

    private static bool TryReadJpegInfo( byte[] data, out int width, out int height, out int componentCount, out int bitsPerComponent )
    {
        width = 0;
        height = 0;
        componentCount = 3;
        bitsPerComponent = 8;

        if ( data is null || data.Length < 4 || data[0] != 0xFF || data[1] != 0xD8 )
            return false;

        int index = 2;

        while ( index + 9 < data.Length )
        {
            if ( data[index] != 0xFF )
            {
                index++;
                continue;
            }

            while ( index < data.Length && data[index] == 0xFF )
            {
                index++;
            }

            if ( index >= data.Length )
                return false;

            byte marker = data[index++];

            if ( marker == 0xD9 || marker == 0xDA )
                return false;

            if ( index + 1 >= data.Length )
                return false;

            int length = ReadBigEndianUInt16( data, index );

            if ( length < 2 || index + length > data.Length )
                return false;

            if ( IsJpegStartOfFrameMarker( marker ) )
            {
                bitsPerComponent = data[index + 2];
                height = ReadBigEndianUInt16( data, index + 3 );
                width = ReadBigEndianUInt16( data, index + 5 );
                componentCount = data[index + 7];

                return width > 0 && height > 0 && bitsPerComponent > 0;
            }

            index += length;
        }

        return false;
    }

    private static bool IsJpegStartOfFrameMarker( byte marker )
    {
        return marker is 0xC0 or 0xC1 or 0xC2 or 0xC3 or 0xC5 or 0xC6 or 0xC7 or 0xC9 or 0xCA or 0xCB or 0xCD or 0xCE or 0xCF;
    }

    private static bool TryCreatePngImageData( byte[] data, out PdfImageData imageData )
    {
        imageData = null;

        if ( data is null || data.Length < 33 || !HasPngSignature( data ) )
            return false;

        int width = 0;
        int height = 0;
        int bitDepth = 0;
        int colorType = 0;
        byte[] palette = null;
        byte[] transparency = null;
        using MemoryStream compressedImageData = new();

        int index = 8;

        while ( index + 8 <= data.Length )
        {
            int length = ReadBigEndianInt32( data, index );
            index += 4;

            if ( length < 0 || index + 4 + length > data.Length )
                return false;

            string chunkType = Encoding.ASCII.GetString( data, index, 4 );
            index += 4;

            if ( chunkType == "IHDR" )
            {
                width = ReadBigEndianInt32( data, index );
                height = ReadBigEndianInt32( data, index + 4 );
                bitDepth = data[index + 8];
                colorType = data[index + 9];

                if ( data[index + 10] != 0 || data[index + 11] != 0 || data[index + 12] != 0 )
                    return false;
            }
            else if ( chunkType == "PLTE" )
            {
                palette = data.Skip( index ).Take( length ).ToArray();
            }
            else if ( chunkType == "tRNS" )
            {
                transparency = data.Skip( index ).Take( length ).ToArray();
            }
            else if ( chunkType == "IDAT" )
            {
                compressedImageData.Write( data, index, length );
            }
            else if ( chunkType == "IEND" )
            {
                break;
            }

            index += length + 4;
        }

        if ( width <= 0 || height <= 0 || bitDepth != 8 || compressedImageData.Length == 0 )
            return false;

        if ( !TryDecodePngImageBytes( compressedImageData.ToArray(), width, height, colorType, palette, transparency, out byte[] imageBytes, out byte[] alphaBytes, out string colorSpace ) )
            return false;

        byte[] encodedImageBytes = CompressZlib( imageBytes );

        imageData = new()
        {
            Data = encodedImageBytes,
            Width = width,
            Height = height,
            ColorSpace = colorSpace,
            BitsPerComponent = bitDepth,
            Filter = "/FlateDecode",
            AlphaData = alphaBytes is null ? null : CompressZlib( alphaBytes ),
        };

        return true;
    }

    private static bool HasPngSignature( byte[] data )
    {
        return data.Length >= 8
            && data[0] == 0x89
            && data[1] == 0x50
            && data[2] == 0x4E
            && data[3] == 0x47
            && data[4] == 0x0D
            && data[5] == 0x0A
            && data[6] == 0x1A
            && data[7] == 0x0A;
    }

    private static bool TryDecodePngImageBytes( byte[] compressedData, int width, int height, int colorType, byte[] palette, byte[] transparency, out byte[] imageBytes, out byte[] alphaBytes, out string colorSpace )
    {
        imageBytes = null;
        alphaBytes = null;
        colorSpace = null;

        int sourceComponents = GetPngSourceComponentCount( colorType );

        if ( sourceComponents <= 0 )
            return false;

        byte[] decodedBytes;

        try
        {
            decodedBytes = DecompressZlib( compressedData );
        }
        catch ( InvalidDataException )
        {
            return false;
        }

        int sourceStride = width * sourceComponents;
        int expectedLength = ( sourceStride + 1 ) * height;

        if ( decodedBytes.Length < expectedLength )
            return false;

        byte[] unfilteredBytes = UnfilterPngBytes( decodedBytes, width, height, sourceComponents );

        if ( colorType == 0 )
        {
            imageBytes = unfilteredBytes;
            alphaBytes = CreatePngColorKeyAlphaBytes( unfilteredBytes, 1, transparency );
            colorSpace = "/DeviceGray";
            return true;
        }

        if ( colorType == 2 )
        {
            imageBytes = unfilteredBytes;
            alphaBytes = CreatePngColorKeyAlphaBytes( unfilteredBytes, 3, transparency );
            colorSpace = "/DeviceRGB";
            return true;
        }

        if ( colorType == 3 )
        {
            if ( palette is null || palette.Length < 3 )
                return false;

            imageBytes = ExpandPngPaletteBytes( unfilteredBytes, palette );
            alphaBytes = ExpandPngPaletteAlphaBytes( unfilteredBytes, transparency );
            colorSpace = "/DeviceRGB";
            return true;
        }

        if ( colorType == 4 )
        {
            SplitPngAlphaBytes( unfilteredBytes, 2, 1, out imageBytes, out alphaBytes );
            colorSpace = "/DeviceGray";
            return true;
        }

        if ( colorType == 6 )
        {
            SplitPngAlphaBytes( unfilteredBytes, 4, 3, out imageBytes, out alphaBytes );
            colorSpace = "/DeviceRGB";
            return true;
        }

        return false;
    }

    private static int GetPngSourceComponentCount( int colorType )
    {
        return colorType switch
        {
            0 => 1,
            2 => 3,
            3 => 1,
            4 => 2,
            6 => 4,
            _ => 0,
        };
    }

    private static byte[] UnfilterPngBytes( byte[] decodedBytes, int width, int height, int components )
    {
        int stride = width * components;
        byte[] result = new byte[stride * height];
        int sourceIndex = 0;

        for ( int row = 0; row < height; row++ )
        {
            int filter = decodedBytes[sourceIndex++];

            if ( filter > 4 )
                throw new InvalidDataException( $"The PNG image uses unsupported scanline filter {filter}." );

            int rowStart = row * stride;
            int previousRowStart = rowStart - stride;

            for ( int column = 0; column < stride; column++ )
            {
                byte value = decodedBytes[sourceIndex++];
                int left = column >= components ? result[rowStart + column - components] : 0;
                int above = row > 0 ? result[previousRowStart + column] : 0;
                int upperLeft = row > 0 && column >= components ? result[previousRowStart + column - components] : 0;

                result[rowStart + column] = filter switch
                {
                    0 => value,
                    1 => unchecked((byte)( value + left )),
                    2 => unchecked((byte)( value + above )),
                    3 => unchecked((byte)( value + ( ( left + above ) / 2 ) )),
                    4 => unchecked((byte)( value + PaethPredictor( left, above, upperLeft ) )),
                    _ => value,
                };
            }
        }

        return result;
    }

    private static int PaethPredictor( int left, int above, int upperLeft )
    {
        int prediction = left + above - upperLeft;
        int distanceLeft = Math.Abs( prediction - left );
        int distanceAbove = Math.Abs( prediction - above );
        int distanceUpperLeft = Math.Abs( prediction - upperLeft );

        if ( distanceLeft <= distanceAbove && distanceLeft <= distanceUpperLeft )
            return left;

        if ( distanceAbove <= distanceUpperLeft )
            return above;

        return upperLeft;
    }

    private static byte[] ExpandPngPaletteBytes( byte[] indexBytes, byte[] palette )
    {
        byte[] result = new byte[indexBytes.Length * 3];
        int targetIndex = 0;

        foreach ( byte index in indexBytes )
        {
            int paletteIndex = index * 3;

            if ( paletteIndex + 2 >= palette.Length )
                throw new InvalidDataException( "The PNG image contains a palette index outside its palette." );

            result[targetIndex++] = palette[paletteIndex];
            result[targetIndex++] = palette[paletteIndex + 1];
            result[targetIndex++] = palette[paletteIndex + 2];
        }

        return result;
    }

    private static byte[] ExpandPngPaletteAlphaBytes( byte[] indexBytes, byte[] transparency )
    {
        if ( transparency is null || transparency.Length == 0 )
            return null;

        byte[] result = new byte[indexBytes.Length];

        for ( int i = 0; i < indexBytes.Length; i++ )
        {
            int paletteIndex = indexBytes[i];
            result[i] = paletteIndex < transparency.Length ? transparency[paletteIndex] : byte.MaxValue;
        }

        return HasTransparency( result ) ? result : null;
    }

    private static byte[] CreatePngColorKeyAlphaBytes( byte[] source, int components, byte[] transparency )
    {
        if ( transparency is null || transparency.Length < components * 2 )
            return null;

        int[] transparentComponents = new int[components];

        for ( int component = 0; component < components; component++ )
        {
            transparentComponents[component] = ReadBigEndianUInt16( transparency, component * 2 );
        }

        byte[] alpha = new byte[source.Length / components];

        for ( int pixel = 0; pixel < alpha.Length; pixel++ )
        {
            bool transparent = true;

            for ( int component = 0; component < components; component++ )
            {
                if ( source[pixel * components + component] != transparentComponents[component] )
                {
                    transparent = false;
                    break;
                }
            }

            alpha[pixel] = transparent ? (byte)0 : byte.MaxValue;
        }

        return HasTransparency( alpha ) ? alpha : null;
    }

    private static void SplitPngAlphaBytes( byte[] source, int sourceComponents, int targetComponents, out byte[] image, out byte[] alpha )
    {
        image = new byte[source.Length / sourceComponents * targetComponents];
        alpha = new byte[source.Length / sourceComponents];
        int imageIndex = 0;
        int alphaIndex = 0;

        for ( int sourceIndex = 0; sourceIndex < source.Length; sourceIndex += sourceComponents )
        {
            for ( int component = 0; component < targetComponents; component++ )
            {
                image[imageIndex++] = source[sourceIndex + component];
            }

            alpha[alphaIndex++] = source[sourceIndex + targetComponents];
        }

        if ( !HasTransparency( alpha ) )
            alpha = null;
    }

    private static bool HasTransparency( byte[] alpha )
    {
        return alpha?.Any( value => value < byte.MaxValue ) == true;
    }

    private static byte[] DecompressZlib( byte[] data )
    {
        using MemoryStream sourceStream = new( data );
        using ZLibStream zlibStream = new( sourceStream, CompressionMode.Decompress );
        using MemoryStream targetStream = new();

        zlibStream.CopyTo( targetStream );

        return targetStream.ToArray();
    }

    private static byte[] CompressZlib( byte[] data )
    {
        using MemoryStream targetStream = new();

        using ( ZLibStream zlibStream = new( targetStream, CompressionLevel.Optimal, leaveOpen: true ) )
        {
            zlibStream.Write( data, 0, data.Length );
        }

        return targetStream.ToArray();
    }

    private static int ReadBigEndianInt32( byte[] data, int index )
    {
        return data[index] << 24 | data[index + 1] << 16 | data[index + 2] << 8 | data[index + 3];
    }

    private static int ReadBigEndianUInt16( byte[] data, int index )
    {
        return data[index] << 8 | data[index + 1];
    }

    #endregion
}

internal sealed class PdfImageData
{
    #region Properties

    internal byte[] Data { get; set; }

    internal int Width { get; set; }

    internal int Height { get; set; }

    internal string ColorSpace { get; set; }

    internal int BitsPerComponent { get; set; }

    internal string Filter { get; set; }

    internal byte[] AlphaData { get; set; }

    #endregion
}