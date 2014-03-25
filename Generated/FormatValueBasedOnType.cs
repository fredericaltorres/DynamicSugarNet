/*
 * DO NOT CHANGE THIS FILE.
 * THIS FILE WAS GENERATED.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions; 

namespace DynamicSugar {

public static class Generated {

 

		 public static string FormatValueBasedOnType(object v, string format){

			if(v==null) return null;
			if(v==System.DBNull.Value) return null;

			var t = v.GetType();			

			 
			if(t == typeof(DateTime)) return ((DateTime)v).ToString(format);
			 
			if(t == typeof(byte)) return ((byte)v).ToString(format);
			 
			if(t == typeof(Byte)) return ((Byte)v).ToString(format);
			 
			if(t == typeof(sbyte)) return ((sbyte)v).ToString(format);
			 
			if(t == typeof(SByte)) return ((SByte)v).ToString(format);
			 
			if(t == typeof(decimal)) return ((decimal)v).ToString(format);
			 
			if(t == typeof(Decimal)) return ((Decimal)v).ToString(format);
			 
			if(t == typeof(double)) return ((double)v).ToString(format);
			 
			if(t == typeof(Double)) return ((Double)v).ToString(format);
			 
			if(t == typeof(float)) return ((float)v).ToString(format);
			 
			if(t == typeof(Single)) return ((Single)v).ToString(format);
			 
			if(t == typeof(int)) return ((int)v).ToString(format);
			 
			if(t == typeof(Int32)) return ((Int32)v).ToString(format);
			 
			if(t == typeof(uint)) return ((uint)v).ToString(format);
			 
			if(t == typeof(UInt32)) return ((UInt32)v).ToString(format);
			 
			if(t == typeof(long)) return ((long)v).ToString(format);
			 
			if(t == typeof(Int64)) return ((Int64)v).ToString(format);
			 
			if(t == typeof(ulong)) return ((ulong)v).ToString(format);
			 
			if(t == typeof(UInt64)) return ((UInt64)v).ToString(format);
			 
			if(t == typeof(short)) return ((short)v).ToString(format);
			 
			if(t == typeof(Int16)) return ((Int16)v).ToString(format);
			 
			if(t == typeof(ushort)) return ((ushort)v).ToString(format);
			 
			if(t == typeof(UInt16)) return ((UInt16)v).ToString(format);
			
return v.ToString();            
		}
	}
}