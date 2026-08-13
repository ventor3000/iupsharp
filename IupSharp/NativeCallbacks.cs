using System;
using System.Runtime.InteropServices;

namespace IupSharp
{
    using Ihandle = IntPtr;

    // -----------------------------------------------------------------------
    // Global (non-Ihandle) callbacks
    // -----------------------------------------------------------------------

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFidle();                                          // idle

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void IFentry();                                         // entry

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void IFi(int i);                                        // globalentermodal_cb, globalleavemodal_cb

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void IFs([MarshalAs(UnmanagedType.LPUTF8Str)] string s); // openurl_cb

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void IFii(int i1, int i2);                              // globalkeypress_cb

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void IFiis(int i1, int i2,                              // globalmotion_cb, openfiles_cb
        [MarshalAs(UnmanagedType.LPUTF8Str)] string s);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void IFiiiis(int i1, int i2, int i3, int i4,           // globalbutton_cb
        [MarshalAs(UnmanagedType.LPUTF8Str)] string s);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void IFfiis(float f, int i1, int i2,                   // globalwheel_cb
        [MarshalAs(UnmanagedType.LPUTF8Str)] string s);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void IFvs(IntPtr ptr,                                   // handleadd_cb, handleremove_cb,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string s);                    // imagecreate_cb, imagedestroy_cb

    // -----------------------------------------------------------------------
    // Standard Ihandle callbacks  (return int)
    // -----------------------------------------------------------------------

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFn(Ihandle ih);                                    // default / Icallback

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFni(Ihandle ih, int i);                            // k_any, show_cb, toggle_action,
                                                                            // spin_cb, branchopen_cb, …

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFnii(Ihandle ih, int i1, int i2);                 // resize_cb, caret_cb, …

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFniii(Ihandle ih, int i1, int i2, int i3);        // trayclick_cb, edition_cb

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFniiii(Ihandle ih, int i1, int i2,                // dragdrop_cb
        int i3, int i4);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFniiiiiiC(Ihandle ih,                             // draw_cb  (cdCanvas*)
        int i1, int i2, int i3, int i4, int i5, int i6, IntPtr canvas);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFniiiiii(Ihandle ih,                              // OLD draw_cb
        int i1, int i2, int i3, int i4, int i5, int i6);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFnsidv(Ihandle ih,                                // postmessage_cb
        [MarshalAs(UnmanagedType.LPUTF8Str)] string s,
        int i, double d, IntPtr ptr);

    // -----------------------------------------------------------------------
    // Canvas / scroll / wheel
    // -----------------------------------------------------------------------

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFnff(Ihandle ih, float f1, float f2);             // canvas action

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFniff(Ihandle ih, int i, float f1, float f2);     // scroll_cb

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFnfiis(Ihandle ih, float f, int i1, int i2,       // wheel_cb
        [MarshalAs(UnmanagedType.LPUTF8Str)] string s);

    // -----------------------------------------------------------------------
    // Drag and drop
    // -----------------------------------------------------------------------

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFnsVi(Ihandle ih,                                 // dragdata_cb
        [MarshalAs(UnmanagedType.LPUTF8Str)] string s,
        IntPtr data, int i);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFnsViii(Ihandle ih,                               // dropdata_cb
        [MarshalAs(UnmanagedType.LPUTF8Str)] string s,
        IntPtr data, int i1, int i2, int i3);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFnsiii(Ihandle ih,                                // dropfiles_cb
        [MarshalAs(UnmanagedType.LPUTF8Str)] string s,
        int i1, int i2, int i3);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFnssi(Ihandle ih,                                 // dragfilecreatename_cb
        [MarshalAs(UnmanagedType.LPUTF8Str)] string s1,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string s2,
        int i);

    // -----------------------------------------------------------------------
    // Multi-handle and tab callbacks
    // -----------------------------------------------------------------------

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFnnii(Ihandle ih, Ihandle ih2, int i1, int i2);   // drop_cb

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFnn(Ihandle ih, Ihandle ih2);                     // savemarkers_cb, restoremarkers_cb

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFnnn(Ihandle ih, Ihandle ih2, Ihandle ih3);       // tabchange_cb

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFnni(Ihandle ih, Ihandle ih2, int i);

    // -----------------------------------------------------------------------
    // String-carrying callbacks
    // -----------------------------------------------------------------------

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFnss(Ihandle ih,                                  // file_cb
        [MarshalAs(UnmanagedType.LPUTF8Str)] string s1,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string s2);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFns(Ihandle ih,                                   // multiselect_cb
        [MarshalAs(UnmanagedType.LPUTF8Str)] string s);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFnsi(Ihandle ih,                                  // copydata_cb
        [MarshalAs(UnmanagedType.LPUTF8Str)] string s, int i);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFnis(Ihandle ih, int i,                           // text_action, edit_cb, rename_cb
        [MarshalAs(UnmanagedType.LPUTF8Str)] string s);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFnsii(Ihandle ih,                                 // list_action
        [MarshalAs(UnmanagedType.LPUTF8Str)] string s, int i1, int i2);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFniis(Ihandle ih, int i1, int i2,                 // motion_cb, click_cb, value_edit_cb
        [MarshalAs(UnmanagedType.LPUTF8Str)] string s);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFniiis(Ihandle ih, int i1, int i2, int i3,        // touch_cb, dblclick_cb
        [MarshalAs(UnmanagedType.LPUTF8Str)] string s);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFniiiis(Ihandle ih, int i1, int i2,               // button_cb, matrix_action,
        int i3, int i4,                                                    // mousemotion_cb
        [MarshalAs(UnmanagedType.LPUTF8Str)] string s);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFniiiiiis(Ihandle ih,                             // mouseclick_cb
        int i1, int i2, int i3, int i4, int i5, int i6,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string s);

    // -----------------------------------------------------------------------
    // Array / pointer int callbacks
    // -----------------------------------------------------------------------

    /// <summary>multiselection_cb, multiunselection_cb — int* is a caller-owned array of length n.</summary>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFnIi(Ihandle ih, IntPtr intArray, int n);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFnd(Ihandle ih, double d);                        // mousemove_cb, button_press_cb, …

    /// <summary>fgcolor_cb, bgcolor_cb — three out-int pointers for R, G, B.</summary>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFniiIII(Ihandle ih, int i1, int i2,
        out int r, out int g, out int b);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFniinsii(Ihandle ih, int i1, int i2,              // dropselect_cb
        Ihandle ih2,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string s,
        int i3, int i4);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFnccc(Ihandle ih, byte c1, byte c2, byte c3);     // drag_cb, change_cb

    /// <summary>multitouch_cb — two int* arrays (ids, px, py, pstate) of length count.</summary>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFniIIII(Ihandle ih, int count,
        IntPtr ids, IntPtr px, IntPtr py, IntPtr pstate);

    // -----------------------------------------------------------------------
    // cdCanvas callbacks
    // -----------------------------------------------------------------------

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFnC(Ihandle ih, IntPtr canvas);                   // postdraw_cb, predraw_cb

    // -----------------------------------------------------------------------
    // Plot callbacks
    // -----------------------------------------------------------------------

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFniidd(Ihandle ih, int i1, int i2,                // delete_cb
        double d1, double d2);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFniiddi(Ihandle ih, int i1, int i2,               // select_cb
        double d1, double d2, int i3);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFniiddiddi(Ihandle ih, int i1, int i2,            // clicksegment_cb
        double d1, double d2, int i3, double d4, double d5, int i4);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFniidds(Ihandle ih, int i1, int i2,               // plotbutton_cb
        double d1, double d2,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string s);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFndds(Ihandle ih, double d1, double d2,           // plotmotion_cb
        [MarshalAs(UnmanagedType.LPUTF8Str)] string s);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFnssds(Ihandle ih,                                // plottickformat_cb
        [MarshalAs(UnmanagedType.LPUTF8Str)] string s1,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string s2,
        double d,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string s3);

    // -----------------------------------------------------------------------
    // String-returning callbacks  (char* return value)
    // -----------------------------------------------------------------------

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr sIFnii(Ihandle ih, int i1, int i2);             // value_cb, font_cb

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr sIFni(Ihandle ih, int i);                       // cell_cb

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr sIFniis(Ihandle ih, int i1, int i2,             // translatevalue_cb
        [MarshalAs(UnmanagedType.LPUTF8Str)] string s);

    // -----------------------------------------------------------------------
    // Numeric value callbacks
    // -----------------------------------------------------------------------

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double dIFnii(Ihandle ih, int i1, int i2);             // numericgetvalue_cb

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IFniid(Ihandle ih, int i1, int i2, double d);      // numericsetvalue_cb

    // -----------------------------------------------------------------------
    // Android
    // -----------------------------------------------------------------------

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void IFniiv(Ihandle ih, int i1, int i2, IntPtr ptr);   // android_onactivityresult_cb

}
