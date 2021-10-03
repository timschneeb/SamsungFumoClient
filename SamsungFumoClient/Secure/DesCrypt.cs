using System;
using System.Diagnostics;
using System.Text;
using SamsungFumoClient.Utils;

namespace SamsungFumoClient.Secure
{
    public class DesCrypt
    {
        private const int MaxCryptBitsSize = 64;
        private static byte[]? _c;
        private static byte[]? _d;
        private static byte[]? _e;

        private static readonly byte[] Fp =
        {
            40, 8, 48, 16, 56, 24, 64, 32, 39, 7, 47, 15, 55, 23, 63, 31, 38, 6, 46, 14, 54, 22, 62, 30, 37, 5, 45, 13,
            53, 21, 61, 29, 36, 4, 44, 12, 52, 20, 60, 28, 35, 3, 43, 11, 51, 19, 59, 27, 34, 2, 42, 10, 50, 18, 58, 26,
            33, 1, 41, 9, 49, 17, 57, 25
        };

        private static readonly byte[] Ip =
        {
            58, 50, 42, 34, 26, 18, 10, 2, 60, 52, 44, 36, 28, 20, 12, 4, 62, 54, 46, 38, 30, 22, 14, 6, 64, 56, 48, 40,
            32, 24, 16, 8, 57, 49, 41, 33, 25, 17, 9, 1, 59, 51, 43, 35, 27, 19, 11, 3, 61, 53, 45, 37, 29, 21, 13, 5,
            63, 55, 47, 39, 31, 23, 15, 7
        };

        private static byte[][]? _ks;

        private static readonly byte[] P =
        {
            16, 7, 20, 21, 29, 12, 28, 17, 1, 15, 23, 26, 5, 18, 31, 10, 2, 8, 24, 14, 32, 27, 3, 9, 19, 13, 30, 6, 22,
            11, 4, 25
        };

        private static readonly byte[] Pc1C =
        {
            57, 49, 41, 33, 25, 17, 9, 1, 58, 50, 42, 34, 26, 18, 10, 2, 59, 51, 43, 35, 27, 19, 11, 3, 60, 52, 44, 36
        };

        private static readonly byte[] Pc1D =
        {
            63, 55, 47, 39, 31, 23, 15, 7, 62, 54, 46, 38, 30, 22, 14, 6, 61, 53, 45, 37, 29, 21, 13, 5, 28, 20, 12, 4
        };

        private static readonly byte[] Pc2C =
            {14, 17, 11, 24, 1, 5, 3, 28, 15, 6, 21, 10, 23, 19, 12, 4, 26, 8, 16, 7, 27, 20, 13, 2};

        private static readonly byte[] Pc2D =
            {41, 52, 31, 37, 47, 55, 30, 40, 51, 45, 33, 48, 44, 49, 39, 56, 34, 53, 46, 42, 50, 36, 29, 32};

        private static readonly byte[][] S =
        {
            new byte[]
            {
                14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7, 0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3,
                8, 4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0, 15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0,
                6, 13
            },
            new byte[]
            {
                15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10, 3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11,
                5, 0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15, 13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5,
                14, 9
            },
            new byte[]
            {
                10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8, 13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15,
                1, 13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7, 1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5,
                2, 12
            },
            new byte[]
            {
                7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15, 13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14,
                9, 10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4, 3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7,
                2, 14
            },
            new byte[]
            {
                2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9, 14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8,
                6, 4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14, 11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4,
                5, 3
            },
            new byte[]
            {
                12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11, 10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3,
                8, 9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6, 4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0,
                8, 13
            },
            new byte[]
            {
                4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1, 13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8,
                6, 1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2, 6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2,
                3, 12
            },
            new byte[]
            {
                13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7, 1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9,
                2, 7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8, 2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5,
                6, 11
            }
        };

        private static readonly byte[] e2 =
        {
            32, 1, 2, 3, 4, 5, 4, 5, 6, 7, 8, 9, 8, 9, 10, 11, 12, 13, 12, 13, 14, 15, 16, 17, 16, 17, 18, 19, 20, 21,
            20, 21, 22, 23, 24, 25, 24, 25, 26, 27, 28, 29, 28, 29, 30, 31, 32, 1
        };

        private static byte[]? _cryptCryptByte;
        private static byte[]? _preS;
        private static readonly byte[] Shifts = {1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1};

        public DesCrypt()
        {
            _c = new byte[28];
            _d = new byte[28];
            _ks = new byte[16][];
            for (var index = 0; index < _ks.Length; index++)
            {
                _ks[index] = new byte[48];
            }

            _e = new byte[48];
            _preS = new byte[48];
            _cryptCryptByte = new byte[16];
        }

        private static byte[]? InitPassword(byte[]? bArr, byte[]? bArr2)
        {
            if (bArr == null || bArr2 == null)
            {
                return null;
            }

            var i = 0;
            var i2 = 0;
            while (i < bArr.Length && bArr[i] != 0 && i2 < MaxCryptBitsSize)
            {
                for (var i3 = 6; i3 >= 0; i3--)
                {
                    bArr2[i2] = (byte) ((bArr[i] >> i3) & 1);
                    i2++;
                }

                i++;
                bArr2[i2] = 0;
                i2++;
            }

            while (i2 < MaxCryptBitsSize + 2)
            {
                bArr2[i2] = 0;
                i2++;
            }

            return bArr2;
        }

        private static byte[] ZeroPassword(byte[] bArr)
        {
            for (var i = 0; i < MaxCryptBitsSize + 2; i++)
            {
                bArr[i] = 0;
            }

            return bArr;
        }

        private static void SetKey(byte[] bArr)
        {
            Debug.Assert(_c != null);
            Debug.Assert(_d != null);
            Debug.Assert(_e != null);
            Debug.Assert(_ks != null);

            for (var i = 0; i < 28; i++)
            {
                _c[i] = bArr[Pc1C[i] - 1];
                _d[i] = bArr[Pc1D[i] - 1];
            }

            for (var i2 = 0; i2 < 16; i2++)
            {
                for (var i3 = 0; i3 < Shifts[i2]; i3++)
                {
                    var b = _c[0];
                    var i4 = 0;
                    while (i4 < 27)
                    {
                        byte[] bArr2 = _c;
                        var i5 = i4 + 1;
                        bArr2[i4] = bArr2[i5];
                        i4 = i5;
                    }

                    _c[27] = b;
                    var b2 = _d[0];
                    var i6 = 0;
                    while (i6 < 27)
                    {
                        byte[] bArr3 = _d;
                        var i7 = i6 + 1;
                        bArr3[i6] = bArr3[i7];
                        i6 = i7;
                    }

                    _d[27] = b2;
                }

                for (var i8 = 0; i8 < 24; i8++)
                {
                    byte[][] bArr4 = _ks;
                    bArr4[i2][i8] = _c[Pc2C[i8] - 1];
                    bArr4[i2][i8 + 24] = _d[(Pc2D[i8] - 28) - 1];
                }
            }

            for (var i9 = 0; i9 < 48; i9++)
            {
                _e[i9] = e2[i9];
            }
        }

        private static void EExpandsion(byte[]? bArr)
        {
            if (bArr == null)
            {
                return;
            }

            Debug.Assert(_cryptCryptByte != null);
            Debug.Assert(_e != null);

            var i = 0;
            var i2 = 0;
            while (i < 2)
            {
                var i3 = i2 + 1;
                var b = bArr[i2];
                _cryptCryptByte[i] = b;
                var b2 = (byte) (b > 90 ? b - 59 : b > 57 ? b - 53 : b - 46);
                for (var i4 = 0; i4 < 6; i4++)
                {
                    if ((byte) ((b2 >> i4) & 1) != 0)
                    {
                        byte[] bArr2 = _e;
                        var i5 = (i * 6) + i4;
                        var b3 = bArr2[i5];
                        var i6 = i5 + 24;
                        bArr2[i5] = bArr2[i6];
                        bArr2[i6] = b3;
                    }
                }

                i++;
                i2 = i3;
            }
        }

        private static byte[] DesEncrypt(byte[] bArr)
        {
            Debug.Assert(_preS != null);
            Debug.Assert(_e != null);
            Debug.Assert(_ks != null);

            byte[] bArr2 = new byte[32];
            byte[] bArr3 = new byte[32];
            byte[] bArr4 = new byte[32];
            byte[] bArr5 = new byte[32];
            var i = 0;
            while (i < 32)
            {
                bArr2[i] = bArr[Ip[i] - 1];
                i++;
            }

            while (i < 64)
            {
                bArr3[i - 32] = bArr[Ip[i] - 1];
                i++;
            }

            for (var i2 = 0; i2 < 16; i2++)
            {
                for (var i3 = 0; i3 < 32; i3++)
                {
                    bArr4[i3] = bArr3[i3];
                }

                for (var i4 = 0; i4 < 48; i4++)
                {
                    _preS[i4] = (byte) (bArr3[_e[i4] - 1] ^ _ks[i2][i4]);
                }

                for (var i5 = 0; i5 < 8; i5++)
                {
                    var b = (byte) (i5 * 6);
                    byte[] bArr6 = _preS;
                    var b2 = S[i5][
                        (bArr6[b] << 5) + (bArr6[b + 1] << 3) + (bArr6[b + 2] << 2) + (bArr6[b + 3] << 1) +
                        (bArr6[b + 4] << 0) + (bArr6[b + 5] << 4)];
                    var b3 = (byte) (i5 * 4);
                    bArr5[b3] = (byte) ((b2 >> 3) & 1);
                    bArr5[b3 + 1] = (byte) ((b2 >> 2) & 1);
                    bArr5[b3 + 2] = (byte) ((b2 >> 1) & 1);
                    bArr5[b3 + 3] = (byte) ((b2 >> 0) & 1);
                }

                for (var i6 = 0; i6 < 32; i6++)
                {
                    bArr3[i6] = (byte) (bArr2[i6] ^ bArr5[P[i6] - 1]);
                }

                for (var i7 = 0; i7 < 32; i7++)
                {
                    bArr2[i7] = bArr4[i7];
                }
            }

            for (var i8 = 0; i8 < 32; i8++)
            {
                var b4 = bArr2[i8];
                bArr2[i8] = bArr3[i8];
                bArr3[i8] = b4;
            }

            for (var i9 = 0; i9 < 64; i9++)
            {
                byte[] bArr7 = Fp;
                if (bArr7[i9] < 33)
                {
                    bArr[i9] = bArr2[bArr7[i9] - 1];
                }
                else
                {
                    bArr[i9] = bArr3[bArr7[i9] - 33];
                }
            }

            return bArr;
        }

        private static void Encrypt(byte[] bArr)
        {
            Debug.Assert(_cryptCryptByte != null);

            for (var i = 0; i < 25; i++)
            {
                bArr = DesEncrypt(bArr);
            }

            var i2 = 0;
            while (i2 < 11)
            {
                byte b = 0;
                for (var i3 = 0; i3 < 6; i3++)
                {
                    b = (byte) (((byte) (b << 1)) | bArr[(i2 * 6) + i3]);
                }

                var b2 = (byte) (b + 46);
                if (b2 > 57)
                {
                    b2 = (byte) (b2 + 7);
                }

                if (b2 > 90)
                {
                    b2 = (byte) (b2 + 6);
                }

                _cryptCryptByte[i2 + 2] = b2;
                i2++;
            }

            byte[] bArr2 = _cryptCryptByte;
            bArr2[i2 + 2] = 0;
            if (bArr2[1] == 0)
            {
                bArr2[1] = bArr2[0];
            }
        }

        public string Generate(string str, byte[] bArr)
        {
            Debug.Assert(_cryptCryptByte != null);

            var initPwd = InitPassword(Encoding.UTF8.GetBytes(str), new byte[(MaxCryptBitsSize + 2)]);
            if (initPwd != null)
            {
                SetKey(initPwd);
                byte[] zeroPwd = ZeroPassword(initPwd);
                EExpandsion(bArr);
                Encrypt(zeroPwd);
            }

            return Encoding.UTF8.GetString(_cryptCryptByte);
        }
    }
}