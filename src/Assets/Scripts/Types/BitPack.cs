﻿using System;

/// <summary>
/// BitPack is a class that treats an array of bytes as a stream of single bits.
///
/// From this, values of variable bit length can be extracted.
/// 
/// </summary>
public class BitPack
{
    protected byte[] Bytes;
    
    protected int CurrentBitNumber;
    protected int CurrentByteIndex;
    protected int CurrentBitIndex;

    public int Count { get; protected set; }

    /// <summary>
    /// Creates a BitPack given an array of bytes.
    ///
    /// </summary>
    /// <param name="buffer"></param>
    public BitPack(byte[] buffer)
    {
        Bytes = buffer;
        Count = buffer.Length * 8;
    }

    #region bool
    public bool GetBool()
    {
        bool v = (Bytes[CurrentByteIndex] & (1 << (7 - CurrentBitIndex))) != 0;
        Step (1);
        return v;
    }
    #endregion bool

    #region UInt64
    public UInt64 GetUInt64_Be(int nBits = 64)
    {
        if (nBits < 0 || nBits > 64)
        {
            throw new ArgumentException();
        }

        UInt64 v = 0;
        while (nBits > 0)
        {
            v |= (UInt64)((GetBool() ? 1 : 0) << --nBits);
        }
        return v;
    }

    public UInt64 GetUInt64_Le(int nBits = 64)
    {
        UInt64 be = GetUInt64_Be (nBits);
        return (UInt64)((((be >>  0) & 0xff) << 56)
                      + (((be >>  8) & 0xff) << 48)
                      + (((be >> 16) & 0xff) << 40)
                      + (((be >> 24) & 0xff) << 32)
                      + (((be >> 32) & 0xff) << 24)
                      + (((be >> 40) & 0xff) << 16)
                      + (((be >> 48) & 0xff) <<  8)
                      + (((be >> 56) & 0xff) <<  0)
            );
    }
    #endregion UInt64

    #region UInt32
    public UInt32 GetUInt32_Be (int nBits = 32)
    {
        if (nBits < 0 || nBits > 32)
        {
            throw new ArgumentException();
        }
        return (UInt32)GetUInt64_Be(nBits);
    }

    public UInt32 GetUInt32_Le (int nBits = 32)
    {
        if (nBits < 0 || nBits > 32)
        {
            throw new ArgumentException();
        }
        UInt64 be = GetUInt64_Be(nBits);
        return (UInt32)((((be >>  0) & 0xff) << 24)
                      + (((be >>  8) & 0xff) << 16)
                      + (((be >> 16) & 0xff) << 8)
                      + (((be >> 24) & 0xff) << 0)
                      );
    }
    #endregion UInt32

    #region UInt16
    public UInt16 GetUInt16_Be(int nBits = 16)
    {
        if (nBits < 0 || nBits > 16)
        {
            throw new ArgumentException();
        }
        return (UInt16)GetUInt64_Be(nBits);
    }

    public UInt16 GetUInt16_Le(int nBits = 16)
    {
        if (nBits < 0 || nBits > 16)
        {
            throw new ArgumentException();
        }
        UInt64 be = GetUInt64_Be(nBits);
        return (UInt16)((((be >> 0) & 0xff) << 8)
                      + (((be >> 8) & 0xff) << 0)
                      );
    }
    #endregion UInt16

    #region UInt8
    public byte GetUInt8 (int nBits = 8)
    {
        if (nBits < 0 || nBits > 8)
        {
            throw new ArgumentException();
        }

        return (byte)GetUInt64_Be(nBits);
    }
    #endregion UInt8

    #region Int8
    public sbyte GetInt8()
    {
        return (sbyte)GetUInt8();
    }
    #endregion Int8

    #region float
    public float GetFloat_Le()
    {
        byte[] buf = new byte[4];
        buf[0] = GetUInt8();
        buf[1] = GetUInt8();
        buf[2] = GetUInt8();
        buf[3] = GetUInt8();
        return BitConverter.ToSingle(buf, 0);
    }
    public float GetFloat_Be()
    {
        byte[] buf = new byte[4];
        buf[3] = GetUInt8();
        buf[2] = GetUInt8();
        buf[1] = GetUInt8();
        buf[0] = GetUInt8();
        return BitConverter.ToSingle(buf, 0);
    }
    #endregion float

    protected void Step(int bitCount)
    {
        CurrentBitNumber += bitCount;
        CurrentByteIndex = CurrentBitNumber / 8;
        CurrentBitIndex = CurrentBitNumber % 8;
    }

}

