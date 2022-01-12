﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PKHeX.Core;

/// <summary>
/// Self-modifying RNG structure that implements xorshift.
/// </summary>
/// <remarks>https://en.wikipedia.org/wiki/Xorshift</remarks>
/// <seealso cref="Xoroshiro128Plus"/>
[StructLayout(LayoutKind.Explicit)]
public ref struct XorShift128
{
    [FieldOffset(0x0)] private uint x;
    [FieldOffset(0x4)] private uint y;
    [FieldOffset(0x8)] private uint z;
    [FieldOffset(0xC)] private uint w;

    // not really readonly! just prevents future updates from touching this.
    [FieldOffset(0x0)] private readonly ulong s0;
    [FieldOffset(0x8)] private readonly ulong s1;

    /// <summary>
    /// Uses the <see cref="RNG.ARNG"/> to advance the seed for each 32-bit input.
    /// </summary>
    /// <param name="seed">32 bit seed</param>
    /// <remarks>sub_E0F5E0 in v1.1.3</remarks>
    public XorShift128(uint seed) : this()
    {
        x = seed;
        y = (0x6C078965 * x) + 1;
        z = (0x6C078965 * y) + 1;
        w = (0x6C078965 * z) + 1;
    }

    public XorShift128(ulong s0, ulong s1) : this()
    {
        this.s0 = s0;
        this.s1 = s1;
    }

    public XorShift128(uint x, uint y, uint z, uint w) : this()
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public (uint x, uint y, uint z, uint w) GetState32() => (x, y, z, w);
    public (ulong s0, ulong s1) GetState64() => (s0, s1);
    public string FullState => $"{s1:X16}{s0:X16}";

    /// <summary>
    /// Gets the next random <see cref="ulong"/>.
    /// </summary>
    public uint Next()
    {
        var t = x ^ (x << 11);
        x = y;
        y = z;
        z = w;
        return w = w ^ (w >> 19) ^ t ^ (t >> 8);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint NextUInt() => (uint)NextInt();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint NextUInt(uint max) => NextUInt() % max;

    /// <summary>
    /// Gets the next random <see cref="ulong"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int NextInt(int start = int.MinValue, int end = int.MaxValue)
    {
        var next = Next();
        var delta = unchecked((uint)(end - start));
        return start + (int)(next % delta);
    }
}