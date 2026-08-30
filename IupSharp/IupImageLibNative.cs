// IUP - Portable User Interface Toolkit
// C# P/Invoke wrapper for the IupImageLib add-on library (v3.32)
//
// This is a SEPARATE native library from iup itself: iupimglib.dll on
// Windows, libiupimglib.so on Unix. It must be deployed alongside iup for
// anything in this namespace to work.
//
// The DllImport is resolved lazily, at the first call rather than at load
// time, so an application that never touches IupImageLib never needs the
// library present.

using System;
using System.Runtime.InteropServices;

namespace IupSharp
{
    /// <summary>
    /// P/Invoke declarations for the IupImageLib add-on library, which provides a
    /// set of pre-defined stock images for buttons, labels and toolbars.
    /// </summary>
    /// <remarks>
    /// <para>Mirrors the conventions of <see cref="IupSharp.IupNative"/>: each
    /// native entry point is a private extern with an explicit EntryPoint, fronted
    /// by a public wrapper that calls CheckThread first.</para>
    ///
    /// <para>Prefer the managed <c>ImageLib</c> class over calling these directly.</para>
    /// </remarks>
    public static class IupImageLibNative
    {
        // ------------------------------------------------------------------ //
        // Library name — change to match your platform / deployment layout.  //
        // ------------------------------------------------------------------ //
        private const string Lib = "iupimglib";

        // ------------------------------------------------------------------ //
        // Functions                                                           //
        // ------------------------------------------------------------------ //

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupImageLibOpen")]
        private static extern void IupImageLibOpen_native();

        /// <summary>
        /// Registers the stock image names with IUP. Must be called after
        /// <c>Iup.Open</c> and before any stock image name is used.
        /// </summary>
        /// <remarks>
        /// <para>This only registers the names; the images themselves are loaded
        /// lazily, when a name is first associated with a control. The names are NOT
        /// registered through IupSetHandle, so they will not appear in
        /// <c>IupGetHandle</c>.</para>
        ///
        /// <para>There is no matching close function in IUP 3. The loaded images are
        /// released by <c>Iup.Close</c>.</para>
        ///
        /// <para>Calling this is what creates the dependency on the native
        /// <c>iupimglib</c> library. An application that never calls it does not need
        /// the library deployed.</para>
        /// </remarks>
        /// <exception cref="DllNotFoundException">
        /// The iupimglib native library could not be found.
        /// </exception>
        public static void IupImageLibOpen()
        {
            IupNative.CheckThread();
            IupImageLibOpen_native();
        }
    }
}