// IUP - Portable User Interface Toolkit
// C# P/Invoke wrapper for the IupControls add-on library (v3.32)
//
// This is a SEPARATE native library from iup itself: iupcontrols.dll on
// Windows, libiupcontrols.so on Unix. It must be deployed alongside iup for
// anything here to work.
//
// The DllImport is resolved lazily, at the first call rather than at load
// time, so an application that never touches IupControls never needs the
// library present.
//
// SCOPE (as of IUP 3.32):
//   IupMatrix, IupMatrixEx, IupMatrixList and IupCells.
//
// Note that IupDial, IupGauge, IupColorbar and IupColorBrowser were moved
// OUT of this library and into the main one in IUP 3.24, when they were
// rewritten to use IupDraw and stopped depending on CD. They live in
// NativeIup, not here. Likewise IupMatrixEx moved IN, from its own separate
// library, in the same release - IupMatrixExOpen and IupMatrixExInit were
// removed at that point and no longer exist.

using System;
using System.Runtime.InteropServices;

namespace IupSharp
{
    /// <summary>
    /// P/Invoke declarations for the IupControls add-on library, which provides the
    /// matrix family and the cells control.
    /// </summary>
    /// <remarks>
    /// <para>Mirrors the conventions of <see cref="NativeIup"/>: each native entry
    /// point is a private extern with an explicit EntryPoint, fronted by a public
    /// wrapper that calls CheckThread first.</para>
    ///
    /// <para><see cref="IupControlsOpen"/> must be called after Iup.Open and before
    /// any of these controls are created.</para>
    /// </remarks>
    public static class NativeIupControls
    {
        // ------------------------------------------------------------------ //
        // Library name — change to match your platform / deployment layout.  //
        // ------------------------------------------------------------------ //
        private const string Lib = "iupcontrols";

        // ------------------------------------------------------------------ //
        // Constants                                                           //
        // ------------------------------------------------------------------ //

        /// <summary>Colorbar index for the primary colour cell.</summary>
        /// <remarks>
        /// Declared in iupcontrols.h, but IupColorbar itself now lives in the main
        /// library. Kept here because that is where IUP declares it.
        /// </remarks>
        public const int IUP_PRIMARY = -1;

        /// <summary>Colorbar index for the secondary colour cell.</summary>
        public const int IUP_SECONDARY = -2;

        // ------------------------------------------------------------------ //
        // Library initialisation                                              //
        // ------------------------------------------------------------------ //

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupControlsOpen")]
        private static extern int IupControlsOpen_native();

        /// <summary>
        /// Initialises the IupControls library. Must be called after Iup.Open and
        /// before any control from this library is created.
        /// </summary>
        /// <returns>IUP_NOERROR on success.</returns>
        /// <exception cref="DllNotFoundException">
        /// The iupcontrols native library could not be found.
        /// </exception>
        public static int IupControlsOpen()
        {
            NativeIup.CheckThread();
            return IupControlsOpen_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupControlsClose")]
        private static extern void IupControlsClose_native();

        /// <summary>
        /// Present for backward compatibility only. It has done nothing since IUP 3;
        /// the library is released by Iup.Close.
        /// </summary>
        public static void IupControlsClose()
        {
            NativeIup.CheckThread();
            IupControlsClose_native();
        }

        // ------------------------------------------------------------------ //
        // Controls                                                            //
        // ------------------------------------------------------------------ //

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupMatrix")]
        private static extern IntPtr IupMatrix_native(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string action);

        /// <summary>
        /// Creates a matrix of alphanumeric fields, which can be edited by the user.
        /// </summary>
        /// <param name="action">
        /// Name of the ACTION_CB callback, or null. Prefer null and set the callback
        /// through SetCallback afterwards.
        /// </param>
        public static IntPtr IupMatrix(string action)
        {
            NativeIup.CheckThread();
            return IupMatrix_native(action);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupMatrixEx")]
        private static extern IntPtr IupMatrixEx_native();

        /// <summary>
        /// Creates a matrix with the extended editing features: copy and paste, undo,
        /// sorting, filtering, find, import and export.
        /// </summary>
        /// <remarks>
        /// Merged into this library in IUP 3.24. The old IupMatrixExOpen and
        /// IupMatrixExInit functions were removed at the same time and no longer
        /// exist.
        /// </remarks>
        public static IntPtr IupMatrixEx()
        {
            NativeIup.CheckThread();
            return IupMatrixEx_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupMatrixList")]
        private static extern IntPtr IupMatrixList_native();

        /// <summary>
        /// Creates a list built on IupMatrix, with optional check boxes, images and
        /// per-item editing.
        /// </summary>
        public static IntPtr IupMatrixList()
        {
            NativeIup.CheckThread();
            return IupMatrixList_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupCells")]
        private static extern IntPtr IupCells_native();

        /// <summary>
        /// Creates a grid of cells whose contents are drawn by the application,
        /// through callbacks.
        /// </summary>
        public static IntPtr IupCells()
        {
            NativeIup.CheckThread();
            return IupCells_native();
        }

        // ------------------------------------------------------------------ //
        // IupMatrix cell utilities                                            //
        //                                                                     //
        // These are line/column variants of the ordinary attribute functions. //
        // Note they use the SAME storage as SetAttributeId2, so either API can //
        // be used against a matrix; these simply read better at the call site. //
        // ------------------------------------------------------------------ //

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupMatSetAttribute")]
        private static extern void IupMatSetAttribute_native(
            IntPtr ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int lin, int col,
            IntPtr value);

        /// <summary>
        /// Sets a cell attribute WITHOUT copying the value, so IUP keeps the pointer
        /// rather than the text. The caller owns the memory and must keep it valid for
        /// as long as IUP might read it.
        /// </summary>
        /// <remarks>
        /// Takes an IntPtr rather than a string deliberately: a marshalled string is
        /// freed when the call returns, which would leave IUP holding a dangling
        /// pointer. Use <see cref="IupMatStoreAttribute"/> for text.
        /// </remarks>
        public static void IupMatSetAttribute(IntPtr ih, string name, int lin, int col, IntPtr value)
        {
            NativeIup.CheckThread();
            IupMatSetAttribute_native(ih, name, lin, col, value);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupMatStoreAttribute")]
        private static extern void IupMatStoreAttribute_native(
            IntPtr ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int lin, int col,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string value);

        /// <summary>
        /// Sets a cell attribute, copying the value. This is the safe form for text.
        /// </summary>
        public static void IupMatStoreAttribute(IntPtr ih, string name, int lin, int col, string value)
        {
            NativeIup.CheckThread();
            IupMatStoreAttribute_native(ih, name, lin, col, value);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupMatGetAttribute")]
        private static extern IntPtr IupMatGetAttribute_native(
            IntPtr ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int lin, int col);

        /// <summary>
        /// Gets a cell attribute as text, or null when it has no value. The returned
        /// buffer belongs to IUP and must not be freed, which is why this converts
        /// rather than letting the marshaller own the string.
        /// </summary>
        public static string IupMatGetAttribute(IntPtr ih, string name, int lin, int col)
        {
            NativeIup.CheckThread();
            return Marshal.PtrToStringUTF8(IupMatGetAttribute_native(ih, name, lin, col));
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupMatGetInt")]
        private static extern int IupMatGetInt_native(
            IntPtr ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int lin, int col);

        /// <summary>Gets a cell attribute as an integer.</summary>
        public static int IupMatGetInt(IntPtr ih, string name, int lin, int col)
        {
            NativeIup.CheckThread();
            return IupMatGetInt_native(ih, name, lin, col);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupMatGetFloat")]
        private static extern float IupMatGetFloat_native(
            IntPtr ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int lin, int col);

        /// <summary>Gets a cell attribute as a float.</summary>
        public static float IupMatGetFloat(IntPtr ih, string name, int lin, int col)
        {
            NativeIup.CheckThread();
            return IupMatGetFloat_native(ih, name, lin, col);
        }

        // IupMatSetfAttribute is deliberately not declared: it takes C varargs, which
        // cannot be expressed safely in a P/Invoke signature. Format the string in C#
        // and call IupMatStoreAttribute instead.
    }
}