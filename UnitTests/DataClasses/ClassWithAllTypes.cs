using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicSugarSharp_UnitTests {

    /// <summary>
    /// 
    /// </summary>
    public class ClassWithAllTypes {

        public byte    _byte;
        public Byte    _Byte;
        public sbyte   _sbyte;
        public SByte   _SByte;
        public decimal _decimal;
        public Decimal _Decimal;
        public double  _double;
        public Double  _Double;
        public float   _float;
        public Single  _Single;
        public int     _int;
        public Int32   _Int32;
        public uint    _uint;
        public UInt32  _UInt32;
        public long    _long;
        public Int64   _Int64;
        public ulong   _ulong;
        public UInt64  _UInt64;
        public short   _short;
        public Int16   _Int16;
        public ushort  _ushort;
        public UInt16  _UInt16;

        /// <summary>
        /// 
        /// </summary>
        public ClassWithAllTypes() {

            _byte    = 64;
            _Byte    = 64;
            _sbyte   = 64;
            _SByte   = 64;
            _decimal = 1.2M;
            _Decimal = 1.2M;
            _double  = 1.2;
            _Double  = 1.2;
            _float   = 1.2f;
            _Single  = System.Convert.ToSingle(1.2f);
            _int     = 64;
            _Int32   = 64;
            _uint    = 64;
            _UInt32  = 64;
            _long    = 64;
            _Int64   = 64;
            _ulong   = 64;
            _UInt64  = 64;
            _short   = 64;
            _Int16   = 64;
            _ushort  = 64;
            _UInt16  = 64;
        }
    }
}
