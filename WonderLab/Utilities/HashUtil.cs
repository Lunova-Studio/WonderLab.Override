using SkiaSharp;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace WonderLab.Utilities;

public static class HashUtil {
    readonly static SHA1 _sha1 = SHA1.Create();
    public static async Task<string> GetFileSha1HashAsync(string filePath) {
        await using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        byte[] hash = _sha1.ComputeHash(fs);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public static async Task<uint> GetFileMurmurHash2Async(string filePath) {
        byte[] data = await File.ReadAllBytesAsync(filePath);
        ReadOnlySpan<byte> filteredData = FilterBytes(data.AsSpan());
        return ComputeMurmurHash2(filteredData);
    }

    private static ReadOnlySpan<byte> FilterBytes(Span<byte> data) {
        int writeIndex = 0;
        for (int i = 0; i < data.Length; i++) {
            byte b = data[i];
            if (b != 9 && b != 10 && b != 13 && b != 32)
                Unsafe.WriteUnaligned(ref data[writeIndex++], b);
        }

        return data[..writeIndex];
    }

    private static uint ComputeMurmurHash2(ReadOnlySpan<byte> data) {
        int length = data.Length;
        uint h = 1 ^ (uint)length;
        int i;

        for (i = 0; i <= length - 4; i += 4) {
            uint k = MemoryMarshal.Read<uint>(data.Slice(i, 4));
            k *= 0x5BD1E995;
            k ^= k >> 24;
            k *= 0x5BD1E995;
            h *= 0x5BD1E995;
            h ^= k;
        }

        if (length > i) {
            Span<byte> tail = stackalloc byte[4];
            data[i..].CopyTo(tail);
            uint remaining = MemoryMarshal.Read<uint>(tail);
            h ^= remaining;
            h *= 0x5BD1E995;
        }

        h ^= h >> 13;
        h *= 0x5BD1E995;
        h ^= h >> 15;

        return h;
    }
}