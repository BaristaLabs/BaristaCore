namespace BaristaLabs.BaristaCore.Utils
{
    // in .Net Core 1.0/1.1, ICustomMarshaler is internal; Saving this for .Net Core 1.2

    //using System;
    //using System.Runtime.InteropServices;
    //using System.Text;

    //public class Utf8Marshaler : ICustomMarshaler
    //{
    //    static Utf8Marshaler static_instance;

    //    public IntPtr MarshalManagedToNative(object managedObj)
    //    {
    //        if (managedObj == null)
    //            return IntPtr.Zero;
    //        if (!(managedObj is string))
    //            throw new MarshalDirectiveException(
    //                   "UTF8Marshaler must be used on a string.");

    //        // not null terminated
    //        byte[] strbuf = Encoding.UTF8.GetBytes((string)managedObj);
    //        IntPtr buffer = Marshal.AllocHGlobal(strbuf.Length + 1);
    //        Marshal.Copy(strbuf, 0, buffer, strbuf.Length);

    //        // write the terminating null
    //        Marshal.WriteByte(buffer + strbuf.Length, 0);
    //        return buffer;
    //    }

    //    public object MarshalNativeToManaged(IntPtr pNativeData)
    //    {
    //        var length = 0;

    //        // find the end of the string
    //        while (Marshal.ReadByte(pNativeData, length) != 0)
    //        {
    //            length++;
    //        }

    //        // should not be null terminated
    //        byte[] strbuf = new byte[length];
    //        // skip the trailing null
    //        Marshal.Copy(pNativeData, strbuf, 0, length);
    //        string data = Encoding.UTF8.GetString(strbuf);
    //        return data;
    //    }

    //    public void CleanUpNativeData(IntPtr pNativeData)
    //    {
    //        Marshal.FreeHGlobal(pNativeData);
    //    }

    //    public void CleanUpManagedData(object managedObj)
    //    {
    //    }

    //    public int GetNativeDataSize()
    //    {
    //        return -1;
    //    }

    //    public static ICustomMarshaler GetInstance(string cookie)
    //    {
    //        if (static_instance == null)
    //        {
    //            return static_instance = new Utf8Marshaler();
    //        }
    //        return static_instance;
    //    }
    //}
}
