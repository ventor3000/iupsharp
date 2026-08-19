// IUP - Portable User Interface Toolkit
// C# P/Invoke wrapper generated from iup.h (v3.32)
// All string parameters are marshalled as UTF-8.

using System;
using System.Runtime.InteropServices;

namespace IupSharp
{



    // Opaque handle type — IUP uses Ihandle* throughout.
    // We represent it as a plain IntPtr alias so the type system
    // keeps call-sites readable while remaining blittable.
    using Ihandle = System.IntPtr;

    // typedef int (*Icallback)(Ihandle*)
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int Icallback(Ihandle ih);

    // typedef int (*Iparamcb)(Ihandle* dialog, int param_index, void* user_data)
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int Iparamcb(Ihandle dialog, int paramIndex, IntPtr userData);

    public static class IupNative
    {

        // ------------------------------------------------------------------ //
        // Thread checking — thread safety utils.
        // ------------------------------------------------------------------ //
        internal static int UiThreadId=-1;   // set in Iup.Open()
        public static bool StrictChecks = true;   // let users turn it off in shipping builds

        // ------------------------------------------------------------------ //
        // Library name — change to match your platform / deployment layout.  //
        // ------------------------------------------------------------------ //
        private const string Lib = "iup";

        // ------------------------------------------------------------------ //
        // Version constants (from iup.h)                                      //
        // ------------------------------------------------------------------ //
        public const string IUP_NAME = "IUP - Portable User Interface";
        public const string IUP_DESCRIPTION = "Multi-platform Toolkit for Building Graphical User Interfaces";
        public const string IUP_COPYRIGHT = "Copyright (C) 1994-2025 Tecgraf/PUC-Rio";
        public const string IUP_VERSION = "3.32";
        public const int IUP_VERSION_NUMBER = 332000;
        public const string IUP_VERSION_DATE = "2025/01/06";

        // ------------------------------------------------------------------ //
        // Common return / flag values                                         //
        // ------------------------------------------------------------------ //
        public const int IUP_ERROR = 1;
        public const int IUP_NOERROR = 0;
        public const int IUP_OPENED = -1;
        public const int IUP_INVALID = -1;
        public const int IUP_INVALID_ID = -10;

        // Callback return values
        public const int IUP_IGNORE = -1;
        public const int IUP_DEFAULT = -2;
        public const int IUP_CLOSE = -3;
        public const int IUP_CONTINUE = -4;

        // IupPopup / IupShowXY position values
        public const int IUP_CENTER = 0xFFFF;
        public const int IUP_LEFT = 0xFFFE;
        public const int IUP_RIGHT = 0xFFFD;
        public const int IUP_MOUSEPOS = 0xFFFC;
        public const int IUP_CURRENT = 0xFFFB;
        public const int IUP_CENTERPARENT = 0xFFFA;
        public const int IUP_LEFTPARENT = 0xFFF9;
        public const int IUP_RIGHTPARENT = 0xFFF8;
        public const int IUP_TOP = IUP_LEFT;
        public const int IUP_BOTTOM = IUP_RIGHT;
        public const int IUP_TOPPARENT = IUP_LEFTPARENT;
        public const int IUP_BOTTOMPARENT = IUP_RIGHTPARENT;

        // Mouse button character codes
        public const int IUP_BUTTON1 = '1';
        public const int IUP_BUTTON2 = '2';
        public const int IUP_BUTTON3 = '3';
        public const int IUP_BUTTON4 = '4';
        public const int IUP_BUTTON5 = '5';

        // IupGetParam callback situations
        public const int IUP_GETPARAM_BUTTON1 = -1;
        public const int IUP_GETPARAM_INIT = -2;
        public const int IUP_GETPARAM_BUTTON2 = -3;
        public const int IUP_GETPARAM_BUTTON3 = -4;
        public const int IUP_GETPARAM_CLOSE = -5;
        public const int IUP_GETPARAM_MAP = -6;
        public const int IUP_GETPARAM_OK = IUP_GETPARAM_BUTTON1;
        public const int IUP_GETPARAM_CANCEL = IUP_GETPARAM_BUTTON2;
        public const int IUP_GETPARAM_HELP = IUP_GETPARAM_BUTTON3;

        // IupColorbar
        public const int IUP_PRIMARY = -1;
        public const int IUP_SECONDARY = -2;

        // Pre-defined mask strings
        public const string IUP_MASK_FLOAT = "[+/-]?(/d+/.?/d*|/./d+)";
        public const string IUP_MASK_UFLOAT = "(/d+/.?/d*|/./d+)";
        public const string IUP_MASK_EFLOAT = "[+/-]?(/d+/.?/d*|/./d+)([eE][+/-]?/d+)?";
        public const string IUP_MASK_UEFLOAT = "(/d+/.?/d*|/./d+)([eE][+/-]?/d+)?";
        public const string IUP_MASK_FLOATCOMMA = "[+/-]?(/d+/,?/d*|/,/d+)";
        public const string IUP_MASK_UFLOATCOMMA = "(/d+/,?/d*|/,/d+)";
        public const string IUP_MASK_INT = "[+/-]?/d+";
        public const string IUP_MASK_UINT = "/d+";

        // ------------------------------------------------------------------ //
        // SHOW_CB enum                                                         //
        // ------------------------------------------------------------------ //
        public enum ShowState { Show = 0, Restore, Minimize, Maximize, Hide }

        // ------------------------------------------------------------------ //
        // SCROLL_CB enum                                                       //
        // ------------------------------------------------------------------ //
        public enum ScrollPos
        {
            Up = 0, Down, PageUp, PageDown, PosV, DragV,
            Left, Right, PageLeft, PageRight, PosH, DragH
        }

        // ------------------------------------------------------------------ //
        // Record input modes                                                   //
        // ------------------------------------------------------------------ //
        public enum RecordMode { Binary = 0, Text }

        // ================================================================== //
        //  Helpers — native P/Invoke declarations (private)                   //
        //  All char* parameters use UnmanagedType.LPUTF8Str so .NET converts  //
        //  managed strings to/from UTF-8 automatically.                        //
        // ================================================================== //

        // ---------- Main API --------------------------------------------- //

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int IupOpen(ref int argc, ref IntPtr argv);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupClose")]
        private static extern void IupClose_native();

        public static void IupClose()
        {
            CheckThread();
            IupClose_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        public static extern int IupIsOpened();

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupImageLibOpen")]
        private static extern void IupImageLibOpen_native();

        public static void IupImageLibOpen()
        {
            CheckThread();
            IupImageLibOpen_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupMainLoop")]
        private static extern int IupMainLoop_native();

        public static int IupMainLoop()
        {
            CheckThread();
            return IupMainLoop_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupLoopStep")]
        private static extern int IupLoopStep_native();

        public static int IupLoopStep()
        {
            CheckThread();
            return IupLoopStep_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupLoopStepWait")]
        private static extern int IupLoopStepWait_native();

        public static int IupLoopStepWait()
        {
            CheckThread();
            return IupLoopStepWait_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupMainLoopLevel")]
        private static extern int IupMainLoopLevel_native();

        public static int IupMainLoopLevel()
        {
            CheckThread();
            return IupMainLoopLevel_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupFlush")]
        private static extern void IupFlush_native();

        public static void IupFlush()
        {
            CheckThread();
            IupFlush_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupExitLoop")]
        private static extern void IupExitLoop_native();

        public static void IupExitLoop()
        {
            CheckThread();
            IupExitLoop_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupPostMessage(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string s,
            int i, double d, IntPtr p);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int IupRecordInput(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string filename, int mode);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int IupPlayInput(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string filename);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupUpdate")]
        private static extern void IupUpdate_native(Ihandle ih);

        public static void IupUpdate(Ihandle ih)
        {
            CheckThread();
            IupUpdate_native(ih);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupUpdateChildren")]
        private static extern void IupUpdateChildren_native(Ihandle ih);

        public static void IupUpdateChildren(Ihandle ih)
        {
            CheckThread();
            IupUpdateChildren_native(ih);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupRedraw")]
        private static extern void IupRedraw_native(Ihandle ih, int children);

        public static void IupRedraw(Ihandle ih, int children)
        {
            CheckThread();
            IupRedraw_native(ih, children);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupRefresh")]
        private static extern void IupRefresh_native(Ihandle ih);

        public static void IupRefresh(Ihandle ih)
        {
            CheckThread();
            IupRefresh_native(ih);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupRefreshChildren")]
        private static extern void IupRefreshChildren_native(Ihandle ih);

        public static void IupRefreshChildren(Ihandle ih)
        {
            CheckThread();
            IupRefreshChildren_native(ih);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int IupExecute(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string filename,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string parameters);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int IupExecuteWait(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string filename,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string parameters);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int IupHelp(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string url);

        // IupLog uses varargs — expose a single-message overload via fixed format.
        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupLog(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string type,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string format);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr IupLoad(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string filename);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr IupLoadBuffer(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string buffer);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr IupVersion();

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr IupVersionDate();

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        public static extern int IupVersionNumber();

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupVersionShow")]
        private static extern void IupVersionShow_native();

        public static void IupVersionShow()
        {
            CheckThread();
            IupVersionShow_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupSetLanguage(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string lng);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr IupGetLanguage();

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupSetLanguageString(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string str);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupStoreLanguageString(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string str);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr IupGetLanguageString(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupSetLanguagePack")]
        private static extern void IupSetLanguagePack_native(Ihandle ih);

        public static void IupSetLanguagePack(Ihandle ih)
        {
            CheckThread();
            IupSetLanguagePack_native(ih);
        }

        // ---------- Handle tree ------------------------------------------ //

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupDestroy")]
        private static extern void IupDestroy_native(Ihandle ih);

        public static void IupDestroy(Ihandle ih)
        {
            CheckThread();
            IupDestroy_native(ih);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupDetach")]
        private static extern void IupDetach_native(Ihandle child);

        public static void IupDetach(Ihandle child)
        {
            CheckThread();
            IupDetach_native(child);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupAppend")]
        private static extern Ihandle IupAppend_native(Ihandle ih, Ihandle child);

        public static Ihandle IupAppend(Ihandle ih, Ihandle child)
        {
            CheckThread();
            return IupAppend_native(ih, child);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupInsert")]
        private static extern Ihandle IupInsert_native(Ihandle ih, Ihandle refChild, Ihandle child);

        public static Ihandle IupInsert(Ihandle ih, Ihandle refChild, Ihandle child)
        {
            CheckThread();
            return IupInsert_native(ih, refChild, child);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupGetChild")]
        private static extern Ihandle IupGetChild_native(Ihandle ih, int pos);

        public static Ihandle IupGetChild(Ihandle ih, int pos)
        {
            CheckThread();
            return IupGetChild_native(ih, pos);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupGetChildPos")]
        private static extern int IupGetChildPos_native(Ihandle ih, Ihandle child);

        public static int IupGetChildPos(Ihandle ih, Ihandle child)
        {
            CheckThread();
            return IupGetChildPos_native(ih, child);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupGetChildCount")]
        private static extern int IupGetChildCount_native(Ihandle ih);

        public static int IupGetChildCount(Ihandle ih)
        {
            CheckThread();
            return IupGetChildCount_native(ih);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupGetNextChild")]
        private static extern Ihandle IupGetNextChild_native(Ihandle ih, Ihandle child);

        public static Ihandle IupGetNextChild(Ihandle ih, Ihandle child)
        {
            CheckThread();
            return IupGetNextChild_native(ih, child);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupGetBrother")]
        private static extern Ihandle IupGetBrother_native(Ihandle ih);

        public static Ihandle IupGetBrother(Ihandle ih)
        {
            CheckThread();
            return IupGetBrother_native(ih);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupGetParent")]
        private static extern Ihandle IupGetParent_native(Ihandle ih);

        public static Ihandle IupGetParent(Ihandle ih)
        {
            CheckThread();
            return IupGetParent_native(ih);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupGetDialog")]
        private static extern Ihandle IupGetDialog_native(Ihandle ih);

        public static Ihandle IupGetDialog(Ihandle ih)
        {
            CheckThread();
            return IupGetDialog_native(ih);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern Ihandle IupGetDialogChild(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupReparent")]
        private static extern int IupReparent_native(Ihandle ih, Ihandle newParent, Ihandle refChild);

        public static int IupReparent(Ihandle ih, Ihandle newParent, Ihandle refChild)
        {
            CheckThread();
            return IupReparent_native(ih, newParent, refChild);
        }

        // ---------- Visibility ------------------------------------------- //

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupPopup")]
        private static extern int IupPopup_native(Ihandle ih, int x, int y);

        public static int IupPopup(Ihandle ih, int x, int y)
        {
            CheckThread();
            return IupPopup_native(ih, x, y);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupShow")]
        private static extern int IupShow_native(Ihandle ih);

        public static int IupShow(Ihandle ih)
        {
            CheckThread();
            return IupShow_native(ih);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupShowXY")]
        private static extern int IupShowXY_native(Ihandle ih, int x, int y);

        public static int IupShowXY(Ihandle ih, int x, int y)
        {
            CheckThread();
            return IupShowXY_native(ih, x, y);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupHide")]
        private static extern int IupHide_native(Ihandle ih);

        public static int IupHide(Ihandle ih)
        {
            CheckThread();
            return IupHide_native(ih);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupMap")]
        private static extern int IupMap_native(Ihandle ih);

        public static int IupMap(Ihandle ih)
        {
            CheckThread();
            return IupMap_native(ih);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupUnmap")]
        private static extern void IupUnmap_native(Ihandle ih);

        public static void IupUnmap(Ihandle ih)
        {
            CheckThread();
            IupUnmap_native(ih);
        }

        // ---------- Attributes ------------------------------------------- //

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupResetAttribute(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupGetAllAttributes")]
        private static extern int IupGetAllAttributes_native(Ihandle ih, IntPtr[] names, int n);

        public static int IupGetAllAttributes(Ihandle ih, IntPtr[] names, int n)
        {
            CheckThread();
            return IupGetAllAttributes_native(ih, names, n);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupCopyAttributes")]
        private static extern void IupCopyAttributes_native(Ihandle srcIh, Ihandle dstIh);

        public static void IupCopyAttributes(Ihandle srcIh, Ihandle dstIh)
        {
            CheckThread();
            IupCopyAttributes_native(srcIh, dstIh);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern Ihandle IupSetAttributes(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string str);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr IupGetAttributes(Ihandle ih);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupSetAttribute(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            IntPtr value);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupSetStrAttribute(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string value);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupSetInt(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int value);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupSetFloat(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            float value);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupSetDouble(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            double value);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupSetRGB(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            byte r, byte g, byte b);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupSetRGBA(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            byte r, byte g, byte b, byte a);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr IupGetAttribute(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int IupGetInt(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int IupGetInt2(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int IupGetIntInt(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            out int i1, out int i2);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern float IupGetFloat(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern double IupGetDouble(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupGetRGB(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            out byte r, out byte g, out byte b);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupGetRGBA(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            out byte r, out byte g, out byte b, out byte a);

        // ---------- Attributes by id ------------------------------------- //

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupSetAttributeId(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int id,
            IntPtr value);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupSetStrAttributeId(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int id,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string value);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupSetIntId(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int id, int value);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupSetFloatId(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int id, float value);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupSetDoubleId(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int id, double value);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupSetRGBId(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int id, byte r, byte g, byte b);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr IupGetAttributeId(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int id);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int IupGetIntId(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int id);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern float IupGetFloatId(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int id);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern double IupGetDoubleId(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int id);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupGetRGBId(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int id, out byte r, out byte g, out byte b);

        // ---------- Attributes by id2 (lin/col) -------------------------- //

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupSetAttributeId2(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int lin, int col,
            IntPtr value);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupSetStrAttributeId2(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int lin, int col,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string value);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupSetIntId2(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int lin, int col, int value);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupSetFloatId2(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int lin, int col, float value);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupSetDoubleId2(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int lin, int col, double value);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupSetRGBId2(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int lin, int col, byte r, byte g, byte b);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr IupGetAttributeId2(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int lin, int col);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int IupGetIntId2(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int lin, int col);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern float IupGetFloatId2(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int lin, int col);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern double IupGetDoubleId2(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int lin, int col);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupGetRGBId2(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int lin, int col, out byte r, out byte g, out byte b);

        // ---------- Global attributes ------------------------------------ //

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupSetGlobal(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            IntPtr value);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupSetStrGlobal(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string value);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr IupGetGlobal(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        // ---------- Focus ------------------------------------------------ //

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupSetFocus")]
        private static extern Ihandle IupSetFocus_native(Ihandle ih);

        public static Ihandle IupSetFocus(Ihandle ih)
        {
            CheckThread();
            return IupSetFocus_native(ih);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupGetFocus")]
        private static extern Ihandle IupGetFocus_native();

        public static Ihandle IupGetFocus()
        {
            CheckThread();
            return IupGetFocus_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupPreviousField")]
        private static extern Ihandle IupPreviousField_native(Ihandle ih);

        public static Ihandle IupPreviousField(Ihandle ih)
        {
            CheckThread();
            return IupPreviousField_native(ih);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupNextField")]
        private static extern Ihandle IupNextField_native(Ihandle ih);

        public static Ihandle IupNextField(Ihandle ih)
        {
            CheckThread();
            return IupNextField_native(ih);
        }

        // ---------- Callbacks -------------------------------------------- //

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr IupGetCallback(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr IupSetCallback(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            Icallback func);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr IupGetFunction(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr IupSetFunction(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            Icallback func);

        // ---------- Named handles ---------------------------------------- //

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern Ihandle IupGetHandle(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern Ihandle IupSetHandle(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            Ihandle ih);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupGetAllNames")]
        private static extern int IupGetAllNames_native(IntPtr[] names, int n);

        public static int IupGetAllNames(IntPtr[] names, int n)
        {
            CheckThread();
            return IupGetAllNames_native(names, n);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupGetAllDialogs")]
        private static extern int IupGetAllDialogs_native(IntPtr[] names, int n);

        public static int IupGetAllDialogs(IntPtr[] names, int n)
        {
            CheckThread();
            return IupGetAllDialogs_native(names, n);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr IupGetName(Ihandle ih);

        // ---------- Attribute handles ------------------------------------ //

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupSetAttributeHandle(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            Ihandle ihNamed);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern Ihandle IupGetAttributeHandle(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupSetAttributeHandleId(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int id, Ihandle ihNamed);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern Ihandle IupGetAttributeHandleId(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int id);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupSetAttributeHandleId2(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int lin, int col, Ihandle ihNamed);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern Ihandle IupGetAttributeHandleId2(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int lin, int col);

        // ---------- Class info ------------------------------------------- //

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr IupGetClassName(Ihandle ih);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr IupGetClassType(Ihandle ih);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupGetAllClasses")]
        private static extern int IupGetAllClasses_native(IntPtr[] names, int n);

        public static int IupGetAllClasses(IntPtr[] names, int n)
        {
            CheckThread();
            return IupGetAllClasses_native(names, n);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int IupGetClassAttributes(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string classname,
            IntPtr[] names, int n);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int IupGetClassCallbacks(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string classname,
            IntPtr[] names, int n);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupSaveClassAttributes")]
        private static extern void IupSaveClassAttributes_native(Ihandle ih);

        public static void IupSaveClassAttributes(Ihandle ih)
        {
            CheckThread();
            IupSaveClassAttributes_native(ih);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupCopyClassAttributes")]
        private static extern void IupCopyClassAttributes_native(Ihandle srcIh, Ihandle dstIh);

        public static void IupCopyClassAttributes(Ihandle srcIh, Ihandle dstIh)
        {
            CheckThread();
            IupCopyClassAttributes_native(srcIh, dstIh);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupSetClassDefaultAttribute(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string classname,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string value);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int IupClassMatch(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string classname);

        // ---------- Dynamic creation ------------------------------------- //

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern Ihandle IupCreate(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string classname);

        // IupCreatev / IupCreatep use void* varargs; expose only IupCreate.

        // ================================================================== //
        //  Elements                                                           //
        // ================================================================== //

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupFill")]
        private static extern Ihandle IupFill_native();

        public static Ihandle IupFill()
        {
            CheckThread();
            return IupFill_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupSpace")]
        private static extern Ihandle IupSpace_native();

        public static Ihandle IupSpace()
        {
            CheckThread();
            return IupSpace_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupRadio")]
        private static extern Ihandle IupRadio_native(Ihandle child);

        public static Ihandle IupRadio(Ihandle child)
        {
            CheckThread();
            return IupRadio_native(child);
        }

        // IupVbox / IupHbox / IupZbox etc. use C varargs terminated by NULL.
        // Expose the *v (array) variants which are more idiomatic in P/Invoke.

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupVboxv")]
        private static extern Ihandle IupVboxv_native(params Ihandle[] children);

        public static Ihandle IupVboxv(params Ihandle[] children)
        {
            CheckThread();
            return IupVboxv_native(children);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupZboxv")]
        private static extern Ihandle IupZboxv_native(params Ihandle[] children);

        public static Ihandle IupZboxv(params Ihandle[] children)
        {
            CheckThread();
            return IupZboxv_native(children);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupHboxv")]
        private static extern Ihandle IupHboxv_native(params Ihandle[] children);

        public static Ihandle IupHboxv(params Ihandle[] children)
        {
            CheckThread();
            return IupHboxv_native(children);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupNormalizerv")]
        private static extern Ihandle IupNormalizerv_native(params Ihandle[] ihList);

        public static Ihandle IupNormalizerv(params Ihandle[] ihList)
        {
            CheckThread();
            return IupNormalizerv_native(ihList);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupCboxv")]
        private static extern Ihandle IupCboxv_native(params Ihandle[] children);

        public static Ihandle IupCboxv(params Ihandle[] children)
        {
            CheckThread();
            return IupCboxv_native(children);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupSbox")]
        private static extern Ihandle IupSbox_native(Ihandle child);

        public static Ihandle IupSbox(Ihandle child)
        {
            CheckThread();
            return IupSbox_native(child);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupSplit")]
        private static extern Ihandle IupSplit_native(Ihandle child1, Ihandle child2);

        public static Ihandle IupSplit(Ihandle child1, Ihandle child2)
        {
            CheckThread();
            return IupSplit_native(child1, child2);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupScrollBox")]
        private static extern Ihandle IupScrollBox_native(Ihandle child);

        public static Ihandle IupScrollBox(Ihandle child)
        {
            CheckThread();
            return IupScrollBox_native(child);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupFlatScrollBox")]
        private static extern Ihandle IupFlatScrollBox_native(Ihandle child);

        public static Ihandle IupFlatScrollBox(Ihandle child)
        {
            CheckThread();
            return IupFlatScrollBox_native(child);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupGridBoxv")]
        private static extern Ihandle IupGridBoxv_native(Ihandle[] children);

        public static Ihandle IupGridBoxv(Ihandle[] children)
        {
            CheckThread();
            return IupGridBoxv_native(children);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupMultiBoxv")]
        private static extern Ihandle IupMultiBoxv_native(Ihandle[] children);

        public static Ihandle IupMultiBoxv(Ihandle[] children)
        {
            CheckThread();
            return IupMultiBoxv_native(children);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupExpander")]
        private static extern Ihandle IupExpander_native(Ihandle child);

        public static Ihandle IupExpander(Ihandle child)
        {
            CheckThread();
            return IupExpander_native(child);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupDetachBox")]
        private static extern Ihandle IupDetachBox_native(Ihandle child);

        public static Ihandle IupDetachBox(Ihandle child)
        {
            CheckThread();
            return IupDetachBox_native(child);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupBackgroundBox")]
        private static extern Ihandle IupBackgroundBox_native(Ihandle child);

        public static Ihandle IupBackgroundBox(Ihandle child)
        {
            CheckThread();
            return IupBackgroundBox_native(child);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupFrame")]
        private static extern Ihandle IupFrame_native(Ihandle child);

        public static Ihandle IupFrame(Ihandle child)
        {
            CheckThread();
            return IupFrame_native(child);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupFlatFrame")]
        private static extern Ihandle IupFlatFrame_native(Ihandle child);

        public static Ihandle IupFlatFrame(Ihandle child)
        {
            CheckThread();
            return IupFlatFrame_native(child);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupImage")]
        private static extern Ihandle IupImage_native(int width, int height, byte[] pixels);

        public static Ihandle IupImage(int width, int height, byte[] pixels)
        {
            CheckThread();
            return IupImage_native(width, height, pixels);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupImageRGB")]
        private static extern Ihandle IupImageRGB_native(int width, int height, byte[] pixels);

        public static Ihandle IupImageRGB(int width, int height, byte[] pixels)
        {
            CheckThread();
            return IupImageRGB_native(width, height, pixels);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupImageRGBA")]
        private static extern Ihandle IupImageRGBA_native(int width, int height, byte[] pixels);

        public static Ihandle IupImageRGBA(int width, int height, byte[] pixels)
        {
            CheckThread();
            return IupImageRGBA_native(width, height, pixels);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern Ihandle IupItem(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string title,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string action);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern Ihandle IupSubmenu(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string title,
            Ihandle child);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupSeparator")]
        private static extern Ihandle IupSeparator_native();

        public static Ihandle IupSeparator()
        {
            CheckThread();
            return IupSeparator_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupMenuv")]
        private static extern Ihandle IupMenuv_native(Ihandle[] children);

        public static Ihandle IupMenuv(Ihandle[] children)
        {
            CheckThread();
            return IupMenuv_native(children);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern Ihandle IupButton(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string title,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string action);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern Ihandle IupFlatButton(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string title);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern Ihandle IupFlatToggle(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string title);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupDropButton")]
        private static extern Ihandle IupDropButton_native(Ihandle dropChild);

        public static Ihandle IupDropButton(Ihandle dropChild)
        {
            CheckThread();
            return IupDropButton_native(dropChild);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern Ihandle IupFlatLabel(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string title);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupFlatSeparator")]
        private static extern Ihandle IupFlatSeparator_native();

        public static Ihandle IupFlatSeparator()
        {
            CheckThread();
            return IupFlatSeparator_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern Ihandle IupCanvas(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string action);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupDialog")]
        private static extern Ihandle IupDialog_native(Ihandle child);

        public static Ihandle IupDialog(Ihandle child)
        {
            CheckThread();
            return IupDialog_native(child);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupUser")]
        private static extern Ihandle IupUser_native();

        public static Ihandle IupUser()
        {
            CheckThread();
            return IupUser_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupThread")]
        private static extern Ihandle IupThread_native();

        public static Ihandle IupThread()
        {
            CheckThread();
            return IupThread_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern Ihandle IupLabel(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string title);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern Ihandle IupList(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string action);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupFlatList")]
        private static extern Ihandle IupFlatList_native();

        public static Ihandle IupFlatList()
        {
            CheckThread();
            return IupFlatList_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern Ihandle IupText(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string action);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern Ihandle IupMultiLine(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string action);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern Ihandle IupToggle(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string title,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string action);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupTimer")]
        private static extern Ihandle IupTimer_native();

        public static Ihandle IupTimer()
        {
            CheckThread();
            return IupTimer_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupClipboard")]
        private static extern Ihandle IupClipboard_native();

        public static Ihandle IupClipboard()
        {
            CheckThread();
            return IupClipboard_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupProgressBar")]
        private static extern Ihandle IupProgressBar_native();

        public static Ihandle IupProgressBar()
        {
            CheckThread();
            return IupProgressBar_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern Ihandle IupVal(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string type);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern Ihandle IupFlatVal(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string type);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupFlatTree")]
        private static extern Ihandle IupFlatTree_native();

        public static Ihandle IupFlatTree()
        {
            CheckThread();
            return IupFlatTree_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupTabsv")]
        private static extern Ihandle IupTabsv_native(Ihandle[] children);

        public static Ihandle IupTabsv(Ihandle[] children)
        {
            CheckThread();
            return IupTabsv_native(children);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupFlatTabsv")]
        private static extern Ihandle IupFlatTabsv_native(Ihandle[] children);

        public static Ihandle IupFlatTabsv(Ihandle[] children)
        {
            CheckThread();
            return IupFlatTabsv_native(children);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupTree")]
        private static extern Ihandle IupTree_native();

        public static Ihandle IupTree()
        {
            CheckThread();
            return IupTree_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern Ihandle IupLink(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string url,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string title);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupAnimatedLabel")]
        private static extern Ihandle IupAnimatedLabel_native(Ihandle animation);

        public static Ihandle IupAnimatedLabel(Ihandle animation)
        {
            CheckThread();
            return IupAnimatedLabel_native(animation);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupDatePick")]
        private static extern Ihandle IupDatePick_native();

        public static Ihandle IupDatePick()
        {
            CheckThread();
            return IupDatePick_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupCalendar")]
        private static extern Ihandle IupCalendar_native();

        public static Ihandle IupCalendar()
        {
            CheckThread();
            return IupCalendar_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupColorbar")]
        private static extern Ihandle IupColorbar_native();

        public static Ihandle IupColorbar()
        {
            CheckThread();
            return IupColorbar_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupGauge")]
        private static extern Ihandle IupGauge_native();

        public static Ihandle IupGauge()
        {
            CheckThread();
            return IupGauge_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern Ihandle IupDial(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string type);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupColorBrowser")]
        private static extern Ihandle IupColorBrowser_native();

        public static Ihandle IupColorBrowser()
        {
            CheckThread();
            return IupColorBrowser_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupSpin")]
        private static extern Ihandle IupSpin_native();

        public static Ihandle IupSpin()
        {
            CheckThread();
            return IupSpin_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupSpinbox")]
        private static extern Ihandle IupSpinbox_native(Ihandle child);

        public static Ihandle IupSpinbox(Ihandle child)
        {
            CheckThread();
            return IupSpinbox_native(child);
        }

        // ================================================================== //
        //  Utilities                                                          //
        // ================================================================== //

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int IupStringCompare(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string str1,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string str2,
            int caseSensitive, int lexicographic);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int IupSaveImageAsText(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string filename,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string format,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern Ihandle IupImageGetHandle(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupTextConvertLinColToPos")]
        private static extern void IupTextConvertLinColToPos_native(
            Ihandle ih, int lin, int col, out int pos);

        public static void IupTextConvertLinColToPos(Ihandle ih, int lin, int col, out int pos)
        {
            CheckThread();
            IupTextConvertLinColToPos_native(ih, lin, col, out pos);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupTextConvertPosToLinCol")]
        private static extern void IupTextConvertPosToLinCol_native(
            Ihandle ih, int pos, out int lin, out int col);

        public static void IupTextConvertPosToLinCol(Ihandle ih, int pos, out int lin, out int col)
        {
            CheckThread();
            IupTextConvertPosToLinCol_native(ih, pos, out lin, out col);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupConvertXYToPos")]
        private static extern int IupConvertXYToPos_native(Ihandle ih, int x, int y);

        public static int IupConvertXYToPos(Ihandle ih, int x, int y)
        {
            CheckThread();
            return IupConvertXYToPos_native(ih, x, y);
        }

        // Tree utilities
        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupTreeSetUserId")]
        private static extern int IupTreeSetUserId_native(Ihandle ih, int id, IntPtr userid);

        public static int IupTreeSetUserId(Ihandle ih, int id, IntPtr userid)
        {
            CheckThread();
            return IupTreeSetUserId_native(ih, id, userid);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupTreeGetUserId")]
        private static extern IntPtr IupTreeGetUserId_native(Ihandle ih, int id);

        public static IntPtr IupTreeGetUserId(Ihandle ih, int id)
        {
            CheckThread();
            return IupTreeGetUserId_native(ih, id);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupTreeGetId")]
        private static extern int IupTreeGetId_native(Ihandle ih, IntPtr userid);

        public static int IupTreeGetId(Ihandle ih, IntPtr userid)
        {
            CheckThread();
            return IupTreeGetId_native(ih, userid);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupTreeSetAttributeHandle(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int id, Ihandle ihNamed);

        // ================================================================== //
        //  Pre-defined dialogs                                                //
        // ================================================================== //

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupFileDlg")]
        private static extern Ihandle IupFileDlg_native();

        public static Ihandle IupFileDlg()
        {
            CheckThread();
            return IupFileDlg_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupMessageDlg")]
        private static extern Ihandle IupMessageDlg_native();

        public static Ihandle IupMessageDlg()
        {
            CheckThread();
            return IupMessageDlg_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupColorDlg")]
        private static extern Ihandle IupColorDlg_native();

        public static Ihandle IupColorDlg()
        {
            CheckThread();
            return IupColorDlg_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupFontDlg")]
        private static extern Ihandle IupFontDlg_native();

        public static Ihandle IupFontDlg()
        {
            CheckThread();
            return IupFontDlg_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupProgressDlg")]
        private static extern Ihandle IupProgressDlg_native();

        public static Ihandle IupProgressDlg()
        {
            CheckThread();
            return IupProgressDlg_native();
        }

        // IupGetFile — char* buffer, caller-provided
        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupGetFile")]
        private static extern int IupGetFile_native(
            [MarshalAs(UnmanagedType.LPArray)] byte[] arq);

        public static int IupGetFile(byte[] arq)
        {
            CheckThread();
            return IupGetFile_native(arq);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupMessage(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string title,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string msg);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupMessageError(
            Ihandle parent,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string message);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int IupMessageAlarm(
            Ihandle parent,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string title,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string message,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string buttons);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int IupAlarm(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string title,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string msg,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string b1,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string b2,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string b3);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int IupGetText(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string title,
            [MarshalAs(UnmanagedType.LPArray)] byte[] text,
            int maxsize);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupGetColor")]
        private static extern int IupGetColor_native(
            int x, int y, out byte r, out byte g, out byte b);

        public static int IupGetColor(int x, int y, out byte r, out byte g, out byte b)
        {
            CheckThread();
            return IupGetColor_native(x, y, out r, out g, out b);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int IupGetParamv(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string title,
            Iparamcb action, IntPtr userData,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string format,
            int paramCount, int paramExtra, IntPtr[] paramData);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern Ihandle IupParam(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string format);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupParamBoxv")]
        private static extern Ihandle IupParamBoxv_native(Ihandle[] paramArray);

        public static Ihandle IupParamBoxv(Ihandle[] paramArray)
        {
            CheckThread();
            return IupParamBoxv_native(paramArray);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupLayoutDialog")]
        private static extern Ihandle IupLayoutDialog_native(Ihandle dialog);

        public static Ihandle IupLayoutDialog(Ihandle dialog)
        {
            CheckThread();
            return IupLayoutDialog_native(dialog);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupElementPropertiesDialog")]
        private static extern Ihandle IupElementPropertiesDialog_native(
            Ihandle parent, Ihandle elem);

        public static Ihandle IupElementPropertiesDialog(Ihandle parent, Ihandle elem)
        {
            CheckThread();
            return IupElementPropertiesDialog_native(parent, elem);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupGlobalsDialog")]
        private static extern Ihandle IupGlobalsDialog_native();

        public static Ihandle IupGlobalsDialog()
        {
            CheckThread();
            return IupGlobalsDialog_native();
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupClassInfoDialog")]
        private static extern Ihandle IupClassInfoDialog_native(Ihandle parent);

        public static Ihandle IupClassInfoDialog(Ihandle parent)
        {
            CheckThread();
            return IupClassInfoDialog_native(parent);
        }

        // ================================================================== //
        //  Backward-compatibility aliases (kept per iup.h comments)          //
        // ================================================================== //

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupStoreGlobal(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string value);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupStoreAttribute(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string value);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupStoreAttributeId(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int id,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string value);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void IupStoreAttributeId2(
            Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            int lin, int col,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string value);

        // ================================================================== //
        //  Public managed façade                                              //
        //  Methods below convert between managed strings / types and the     //
        //  raw P/Invoke layer above.                                          //
        // ================================================================== //

        // Helper: IntPtr (char*) → managed UTF-8 string
        private static string PtrToStringUTF8(IntPtr ptr) =>
            ptr == IntPtr.Zero ? null : Marshal.PtrToStringUTF8(ptr);

        /// <summary>Initialises IUP. Call once before any other IUP function.</summary>
        public static int Open()
        {
            int argc = 0;
            IntPtr argv = IntPtr.Zero;
            int res=IupOpen(ref argc, ref argv);

            if (res!=(int)OpenResult.Error) 
                UiThreadId = Environment.CurrentManagedThreadId;
            return res;
            
        }

        public static void Close()
        {
            CheckThread();          // must be closed from the thread that opened it
            IupClose_native();      // bypass the guarded wrapper: UiThreadId is cleared below
            UiThreadId = -1;
        }

        public static void PostMessage(Ihandle ih, string s, int i, double d, IntPtr p)
            => IupPostMessage(ih, s, i, d, p);

        public static int RecordInput(string filename, RecordMode mode)
        {
            CheckThread();
            return IupRecordInput(filename, (int)mode);
        }

        public static int PlayInput(string filename)
        {
            CheckThread();
            return IupPlayInput(filename);
        }

        public static int Execute(string filename, string parameters)
        {
            CheckThread();
            return IupExecute(filename, parameters);
        }

        public static int ExecuteWait(string filename, string parameters)
        {
            CheckThread();
            return IupExecuteWait(filename, parameters);
        }

        public static int Help(string url)
        {
            CheckThread();
            return IupHelp(url);
        }

        /// <summary>Log a message. The format string is passed literally (no printf expansion).</summary>
        public static void Log(string type, string message)
        {
            CheckThread();
            IupLog(type, "%s", message);
        }

        // IupLog has varargs — declare a concrete two-arg overload for the native side.
        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupLog")]
        private static extern void IupLog(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string type,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string format,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string arg0);

        public static string Load(string filename)
        {
            CheckThread();
            return PtrToStringUTF8(IupLoad(filename));
        }

        public static string LoadBuffer(string buffer)
        {
            CheckThread();
            return PtrToStringUTF8(IupLoadBuffer(buffer));
        }

        public static string Version()
            => PtrToStringUTF8(IupVersion());

        public static string VersionDate()
            => PtrToStringUTF8(IupVersionDate());

        public static void SetLanguage(string lng)
        {
            CheckThread();
            IupSetLanguage(lng);
        }

        public static string GetLanguage()
        {
            CheckThread();
            return PtrToStringUTF8(IupGetLanguage());
        }

        public static void SetLanguageString(string name, string str)
        {
            CheckThread();
            IupSetLanguageString(name, str);
        }

        public static void StoreLanguageString(string name, string str)
        {
            CheckThread();
            IupStoreLanguageString(name, str);
        }

        public static string GetLanguageString(string name)
        {
            CheckThread();
            return PtrToStringUTF8(IupGetLanguageString(name));
        }

        public static Ihandle GetDialogChild(Ihandle ih, string name)
        {
            CheckThread();
            return IupGetDialogChild(ih, name);
        }

        // --- Attribute set/get façade ---

        public static void ResetAttribute(Ihandle ih, string name)
        {
            CheckThread();
            IupResetAttribute(ih, name);
        }

        public static Ihandle SetAttributes(Ihandle ih, string str)
        {
            CheckThread();
            return IupSetAttributes(ih, str);
        }

        public static string GetAttributes(Ihandle ih)
        {
            CheckThread();
            return PtrToStringUTF8(IupGetAttributes(ih));
        }

        public static void SetAttribute(Ihandle ih, string name, IntPtr value)
        {
            CheckThread();
            IupSetAttribute(ih, name, value);
        }

        public static void SetStrAttribute(Ihandle ih, string name, string value)
        {
            CheckThread();
            IupSetStrAttribute(ih, name, value);
        }

        public static void SetInt(Ihandle ih, string name, int value)
        {
            CheckThread();
            IupSetInt(ih, name, value);
        }

        public static void SetFloat(Ihandle ih, string name, float value)
        {
            CheckThread();
            IupSetFloat(ih, name, value);
        }

        public static void SetDouble(Ihandle ih, string name, double value)
        {
            CheckThread();
            IupSetDouble(ih, name, value);
        }

        public static void SetRGB(Ihandle ih, string name, byte r, byte g, byte b)
        {
            CheckThread();
            IupSetRGB(ih, name, r, g, b);
        }

        public static void SetRGBA(Ihandle ih, string name, byte r, byte g, byte b, byte a)
        {
            CheckThread();
            IupSetRGBA(ih, name, r, g, b, a);
        }

        public static string GetAttribute(Ihandle ih, string name)
        {
            CheckThread();
            return PtrToStringUTF8(IupGetAttribute(ih, name));
        }

        /// <summary>
        /// Gets an attribute's raw pointer value without interpreting it as a
        /// UTF-8 string. Needed for attributes that are documented as
        /// returning an opaque pointer rather than text, such as WID on
        /// IupImage/IupImageRGB/IupImageRGBA.
        /// </summary>
        public static IntPtr GetAttributePtr(Ihandle ih, string name)
        {
            CheckThread();
            return IupGetAttribute(ih, name);
        }

        public static int GetInt(Ihandle ih, string name)
        {
            CheckThread();
            return IupGetInt(ih, name);
        }

        public static int GetInt2(Ihandle ih, string name)
        {
            CheckThread();
            return IupGetInt2(ih, name);
        }

        public static int GetIntInt(Ihandle ih, string name, out int i1, out int i2)
        {
            CheckThread();
            return IupGetIntInt(ih, name, out i1, out i2);
        }

        public static float GetFloat(Ihandle ih, string name)
        {
            CheckThread();
            return IupGetFloat(ih, name);
        }

        public static double GetDouble(Ihandle ih, string name)
        {
            CheckThread();
            return IupGetDouble(ih, name);
        }

        public static void GetRGB(Ihandle ih, string name, out byte r, out byte g, out byte b)
        {
            CheckThread();
            IupGetRGB(ih, name, out r, out g, out b);
        }

        public static void GetRGBA(Ihandle ih, string name, out byte r, out byte g, out byte b, out byte a)
        {
            CheckThread();
            IupGetRGBA(ih, name, out r, out g, out b, out a);
        }

        // --- Id variants ---

        public static void SetAttributeId(Ihandle ih, string name, int id, IntPtr value)
        {
            CheckThread();
            IupSetAttributeId(ih, name, id, value);
        }

        public static void SetStrAttributeId(Ihandle ih, string name, int id, string value)
        {
            CheckThread();
            IupSetStrAttributeId(ih, name, id, value);
        }

        public static void SetIntId(Ihandle ih, string name, int id, int value)
        {
            CheckThread();
            IupSetIntId(ih, name, id, value);
        }

        public static void SetFloatId(Ihandle ih, string name, int id, float value)
        {
            CheckThread();
            IupSetFloatId(ih, name, id, value);
        }

        public static void SetDoubleId(Ihandle ih, string name, int id, double value)
        {
            CheckThread();
            IupSetDoubleId(ih, name, id, value);
        }

        public static void SetRGBId(Ihandle ih, string name, int id, byte r, byte g, byte b)
        {
            CheckThread();
            IupSetRGBId(ih, name, id, r, g, b);
        }

        public static string GetAttributeId(Ihandle ih, string name, int id)
        {
            CheckThread();
            return PtrToStringUTF8(IupGetAttributeId(ih, name, id));
        }

        public static int GetIntId(Ihandle ih, string name, int id)
        {
            CheckThread();
            return IupGetIntId(ih, name, id);
        }

        public static float GetFloatId(Ihandle ih, string name, int id)
        {
            CheckThread();
            return IupGetFloatId(ih, name, id);
        }

        public static double GetDoubleId(Ihandle ih, string name, int id)
        {
            CheckThread();
            return IupGetDoubleId(ih, name, id);
        }

        public static void GetRGBId(Ihandle ih, string name, int id, out byte r, out byte g, out byte b)
        {
            CheckThread();
            IupGetRGBId(ih, name, id, out r, out g, out b);
        }

        // --- Id2 (lin/col) variants ---

        public static void SetAttributeId2(Ihandle ih, string name, int lin, int col, IntPtr value)
        {
            CheckThread();
            IupSetAttributeId2(ih, name, lin, col, value);
        }

        public static void SetStrAttributeId2(Ihandle ih, string name, int lin, int col, string value)
        {
            CheckThread();
            IupSetStrAttributeId2(ih, name, lin, col, value);
        }

        public static void SetIntId2(Ihandle ih, string name, int lin, int col, int value)
        {
            CheckThread();
            IupSetIntId2(ih, name, lin, col, value);
        }

        public static void SetFloatId2(Ihandle ih, string name, int lin, int col, float value)
        {
            CheckThread();
            IupSetFloatId2(ih, name, lin, col, value);
        }

        public static void SetDoubleId2(Ihandle ih, string name, int lin, int col, double value)
        {
            CheckThread();
            IupSetDoubleId2(ih, name, lin, col, value);
        }

        public static void SetRGBId2(Ihandle ih, string name, int lin, int col, byte r, byte g, byte b)
        {
            CheckThread();
            IupSetRGBId2(ih, name, lin, col, r, g, b);
        }

        public static string GetAttributeId2(Ihandle ih, string name, int lin, int col)
        {
            CheckThread();
            return PtrToStringUTF8(IupGetAttributeId2(ih, name, lin, col));
        }

        public static int GetIntId2(Ihandle ih, string name, int lin, int col)
        {
            CheckThread();
            return IupGetIntId2(ih, name, lin, col);
        }

        public static float GetFloatId2(Ihandle ih, string name, int lin, int col)
        {
            CheckThread();
            return IupGetFloatId2(ih, name, lin, col);
        }

        public static double GetDoubleId2(Ihandle ih, string name, int lin, int col)
        {
            CheckThread();
            return IupGetDoubleId2(ih, name, lin, col);
        }

        public static void GetRGBId2(Ihandle ih, string name, int lin, int col, out byte r, out byte g, out byte b)
        {
            CheckThread();
            IupGetRGBId2(ih, name, lin, col, out r, out g, out b);
        }

        // --- Global attributes ---

        public static void SetGlobal(string name, IntPtr value)
        {
            CheckThread();
            IupSetGlobal(name, value);
        }

        public static void SetStrGlobal(string name, string value)
        {
            CheckThread();
            IupSetStrGlobal(name, value);
        }

        public static string GetGlobal(string name)
        {
            CheckThread();
            return PtrToStringUTF8(IupGetGlobal(name));
        }

        // --- Callbacks ---

        public static Icallback GetCallback(Ihandle ih, string name)
        {
            CheckThread();
            IntPtr ptr = IupGetCallback(ih, name);
            return ptr == IntPtr.Zero ? null
                : Marshal.GetDelegateForFunctionPointer<Icallback>(ptr);
        }

        public static Icallback SetCallback(Ihandle ih, string name, Icallback func)
        {
            CheckThread();
            IntPtr old = IupSetCallback(ih, name, func);
            return old == IntPtr.Zero ? null
                : Marshal.GetDelegateForFunctionPointer<Icallback>(old);
        }

        public static Icallback GetFunction(string name)
        {
            CheckThread();
            IntPtr ptr = IupGetFunction(name);
            return ptr == IntPtr.Zero ? null
                : Marshal.GetDelegateForFunctionPointer<Icallback>(ptr);
        }

        public static Icallback SetFunction(string name, Icallback func)
        {
            CheckThread();
            IntPtr old = IupSetFunction(name, func);
            return old == IntPtr.Zero ? null
                : Marshal.GetDelegateForFunctionPointer<Icallback>(old);
        }

        // --- Named handles ---

        public static Ihandle GetHandle(string name)
        {
            CheckThread();
            return IupGetHandle(name);
        }

        public static Ihandle SetHandle(string name, Ihandle ih)
        {
            CheckThread();
            return IupSetHandle(name, ih);
        }

        public static string GetName(Ihandle ih)
        {
            CheckThread();
            return PtrToStringUTF8(IupGetName(ih));
        }

        // --- Attribute handles ---

        public static void SetAttributeHandle(Ihandle ih, string name, Ihandle ihNamed)
        {
            CheckThread();
            IupSetAttributeHandle(ih, name, ihNamed);
        }

        public static Ihandle GetAttributeHandle(Ihandle ih, string name)
        {
            CheckThread();
            return IupGetAttributeHandle(ih, name);
        }

        public static void SetAttributeHandleId(Ihandle ih, string name, int id, Ihandle ihNamed)
        {
            CheckThread();
            IupSetAttributeHandleId(ih, name, id, ihNamed);
        }

        public static Ihandle GetAttributeHandleId(Ihandle ih, string name, int id)
        {
            CheckThread();
            return IupGetAttributeHandleId(ih, name, id);
        }

        public static void SetAttributeHandleId2(Ihandle ih, string name, int lin, int col, Ihandle ihNamed)
        {
            CheckThread();
            IupSetAttributeHandleId2(ih, name, lin, col, ihNamed);
        }

        public static Ihandle GetAttributeHandleId2(Ihandle ih, string name, int lin, int col)
        {
            CheckThread();
            return IupGetAttributeHandleId2(ih, name, lin, col);
        }

        // --- Class info ---

        public static string GetClassName(Ihandle ih)
        {
            CheckThread();
            return PtrToStringUTF8(IupGetClassName(ih));
        }

        public static string GetClassType(Ihandle ih)
        {
            CheckThread();
            return PtrToStringUTF8(IupGetClassType(ih));
        }

        public static int GetClassAttributes(string classname, IntPtr[] names, int n)
        {
            CheckThread();
            return IupGetClassAttributes(classname, names, n);
        }

        public static int GetClassCallbacks(string classname, IntPtr[] names, int n)
        {
            CheckThread();
            return IupGetClassCallbacks(classname, names, n);
        }

        public static void SetClassDefaultAttribute(string classname, string name, string value)
        {
            CheckThread();
            IupSetClassDefaultAttribute(classname, name, value);
        }

        public static int ClassMatch(Ihandle ih, string classname)
        {
            CheckThread();
            return IupClassMatch(ih, classname);
        }

        // --- Dynamic creation ---

        public static Ihandle Create(string classname)
        {
            CheckThread();
            return IupCreate(classname);
        }

        // --- Element constructors ---

        public static Ihandle Item(string title, string action)
        {
            CheckThread();
            return IupItem(title, action);
        }

        public static Ihandle Submenu(string title, Ihandle child)
        {
            CheckThread();
            return IupSubmenu(title, child);
        }

        public static Ihandle Button(string title, string action)
        {
            CheckThread();
            return IupButton(title, action);
        }

        public static Ihandle FlatButton(string title)
        {
            CheckThread();
            return IupFlatButton(title);
        }

        public static Ihandle FlatToggle(string title)
        {
            CheckThread();
            return IupFlatToggle(title);
        }

        public static Ihandle FlatLabel(string title)
        {
            CheckThread();
            return IupFlatLabel(title);
        }

        public static Ihandle Canvas(string action)
        {
            CheckThread();
            return IupCanvas(action);
        }

        public static Ihandle Label(string title)
        {
            CheckThread();
            return IupLabel(title);
        }

        public static Ihandle List(string action)
        {
            CheckThread();
            return IupList(action);
        }

        public static Ihandle Text(string action)
        {
            CheckThread();
            return IupText(action);
        }

        public static Ihandle MultiLine(string action)
        {
            CheckThread();
            return IupMultiLine(action);
        }

        public static Ihandle Toggle(string title, string action)
        {
            CheckThread();
            return IupToggle(title, action);
        }

        public static Ihandle Val(string type)
        {
            CheckThread();
            return IupVal(type);
        }

        public static Ihandle FlatVal(string type)
        {
            CheckThread();
            return IupFlatVal(type);
        }

        public static Ihandle Link(string url, string title)
        {
            CheckThread();
            return IupLink(url, title);
        }

        public static Ihandle Dial(string type)
        {
            CheckThread();
            return IupDial(type);
        }

        // --- Utilities ---

        public static int StringCompare(string str1, string str2, bool caseSensitive, bool lexicographic)
        {
            CheckThread();
            return IupStringCompare(str1, str2, caseSensitive ? 1 : 0, lexicographic ? 1 : 0);
        }

        public static int SaveImageAsText(Ihandle ih, string filename, string format, string name)
        {
            CheckThread();
            return IupSaveImageAsText(ih, filename, format, name);
        }

        public static Ihandle ImageGetHandle(string name)
        {
            CheckThread();
            return IupImageGetHandle(name);
        }

        public static void TreeSetAttributeHandle(Ihandle ih, string name, int id, Ihandle ihNamed)
        {
            CheckThread();
            IupTreeSetAttributeHandle(ih, name, id, ihNamed);
        }

        // --- Pre-defined dialogs ---

        /// <summary>
        /// Shows a message box with the given title and text.
        /// </summary>
        public static void Message(string title, string msg)
        {
            CheckThread();
            IupMessage(title, msg);
        }

        public static void MessageError(Ihandle parent, string message)
        {
            CheckThread();
            IupMessageError(parent, message);
        }

        public static int MessageAlarm(Ihandle parent, string title, string message, string buttons)
        {
            CheckThread();
            return IupMessageAlarm(parent, title, message, buttons);
        }

        public static int Alarm(string title, string msg, string b1, string b2, string b3)
        {
            CheckThread();
            return IupAlarm(title, msg, b1, b2, b3);
        }

        /// <summary>
        /// Shows the GetText dialog. Returns the result code; the text typed by the user
        /// is returned via <paramref name="text"/>.
        /// </summary>
        public static int GetText(string title, out string text, int maxsize = 4096)
        {
            CheckThread();
            byte[] buf = new byte[maxsize];
            int result = IupGetText(title, buf, maxsize);
            text = System.Text.Encoding.UTF8.GetString(buf).TrimEnd('\0');
            return result;
        }

        /// <summary>
        /// Opens the standard file selection dialog (IupGetFile).
        /// Pass a byte[] of at least IUP_MAX_FILENAME_SIZE bytes pre-filled with the
        /// initial path; on return it contains the selected path as UTF-8.
        /// </summary>
        public static int GetFile(byte[] arq)
        {
            CheckThread();
            return IupGetFile(arq);
        }

        public static int GetParamv(string title, Iparamcb action, IntPtr userData,
                                    string format, int paramCount, int paramExtra,
                                    IntPtr[] paramData)
            => IupGetParamv(title, action, userData, format, paramCount, paramExtra, paramData);

        public static Ihandle Param(string format)
        {
            CheckThread();
            return IupParam(format);
        }

        // --- Backward-compat aliases ---

        public static void StoreGlobal(string name, string value)
        {
            CheckThread();
            IupStoreGlobal(name, value);
        }

        public static void StoreAttribute(Ihandle ih, string name, string value)
        {
            CheckThread();
            IupStoreAttribute(ih, name, value);
        }

        public static void StoreAttributeId(Ihandle ih, string name, int id, string value)
        {
            CheckThread();
            IupStoreAttributeId(ih, name, id, value);
        }

        public static void StoreAttributeId2(Ihandle ih, string name, int lin, int col, string value)
        {
            CheckThread();
            IupStoreAttributeId2(ih, name, lin, col, value);
        }


        internal static void CheckThread()
        {
            if (!StrictChecks) return;
            if (UiThreadId == -1)
                throw new InvalidOperationException("Iup.Open() has not been called.");
            if (Environment.CurrentManagedThreadId != UiThreadId)
                throw new InvalidOperationException(
                    $"IUP called from thread {Environment.CurrentManagedThreadId}; " +
                    $"only thread {UiThreadId} (the one that called Iup.Open) may call IUP.");
        }


        #region IUPDRAW

        // ---- Control ----

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupDrawBegin")]
        private static extern void IupDrawBegin_native(Ihandle ih);

        public static void IupDrawBegin(Ihandle ih)
        {
            CheckThread();
            IupDrawBegin_native(ih);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupDrawEnd")]
        private static extern void IupDrawEnd_native(Ihandle ih);

        public static void IupDrawEnd(Ihandle ih)
        {
            CheckThread();
            IupDrawEnd_native(ih);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupDrawSetClipRect")]
        private static extern void IupDrawSetClipRect_native(Ihandle ih, int x1, int y1, int x2, int y2);

        public static void IupDrawSetClipRect(Ihandle ih, int x1, int y1, int x2, int y2)
        {
            CheckThread();
            IupDrawSetClipRect_native(ih, x1, y1, x2, y2);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupDrawGetClipRect")]
        private static extern void IupDrawGetClipRect_native(Ihandle ih, out int x1, out int y1, out int x2, out int y2);

        public static void IupDrawGetClipRect(Ihandle ih, out int x1, out int y1, out int x2, out int y2)
        {
            CheckThread();
            IupDrawGetClipRect_native(ih, out x1, out y1, out x2, out y2);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupDrawResetClip")]
        private static extern void IupDrawResetClip_native(Ihandle ih);

        public static void IupDrawResetClip(Ihandle ih)
        {
            CheckThread();
            IupDrawResetClip_native(ih);
        }

        // ---- Primitives ----

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupDrawParentBackground")]
        private static extern void IupDrawParentBackground_native(Ihandle ih);

        public static void IupDrawParentBackground(Ihandle ih)
        {
            CheckThread();
            IupDrawParentBackground_native(ih);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupDrawLine")]
        private static extern void IupDrawLine_native(Ihandle ih, int x1, int y1, int x2, int y2);

        public static void IupDrawLine(Ihandle ih, int x1, int y1, int x2, int y2)
        {
            CheckThread();
            IupDrawLine_native(ih, x1, y1, x2, y2);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupDrawRectangle")]
        private static extern void IupDrawRectangle_native(Ihandle ih, int x1, int y1, int x2, int y2);

        public static void IupDrawRectangle(Ihandle ih, int x1, int y1, int x2, int y2)
        {
            CheckThread();
            IupDrawRectangle_native(ih, x1, y1, x2, y2);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupDrawArc")]
        private static extern void IupDrawArc_native(Ihandle ih, int x1, int y1, int x2, int y2, double a1, double a2);

        public static void IupDrawArc(Ihandle ih, int x1, int y1, int x2, int y2, double a1, double a2)
        {
            CheckThread();
            IupDrawArc_native(ih, x1, y1, x2, y2, a1, a2);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupDrawPolygon")]
        private static extern void IupDrawPolygon_native(Ihandle ih, int[] points, int count);

        public static void IupDrawPolygon(Ihandle ih, int[] points, int count)
        {
            CheckThread();
            IupDrawPolygon_native(ih, points, count);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupDrawText")]
        private static extern void IupDrawText_native(Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string str, int len, int x, int y, int w, int h);

        public static void IupDrawText(Ihandle ih, string str, int len, int x, int y, int w, int h)
        {
            CheckThread();
            IupDrawText_native(ih, str, len, x, y, w, h);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupDrawImage")]
        private static extern void IupDrawImage_native(Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name, int x, int y, int w, int h);

        public static void IupDrawImage(Ihandle ih, string name, int x, int y, int w, int h)
        {
            CheckThread();
            IupDrawImage_native(ih, name, x, y, w, h);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupDrawSelectRect")]
        private static extern void IupDrawSelectRect_native(Ihandle ih, int x1, int y1, int x2, int y2);

        public static void IupDrawSelectRect(Ihandle ih, int x1, int y1, int x2, int y2)
        {
            CheckThread();
            IupDrawSelectRect_native(ih, x1, y1, x2, y2);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupDrawFocusRect")]
        private static extern void IupDrawFocusRect_native(Ihandle ih, int x1, int y1, int x2, int y2);

        public static void IupDrawFocusRect(Ihandle ih, int x1, int y1, int x2, int y2)
        {
            CheckThread();
            IupDrawFocusRect_native(ih, x1, y1, x2, y2);
        }

        // ---- Information ----

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupDrawGetSize")]
        private static extern void IupDrawGetSize_native(Ihandle ih, out int w, out int h);

        public static void IupDrawGetSize(Ihandle ih, out int w, out int h)
        {
            CheckThread();
            IupDrawGetSize_native(ih, out w, out h);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupDrawGetTextSize")]
        private static extern void IupDrawGetTextSize_native(Ihandle ih,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string str, int len, out int w, out int h);

        public static void IupDrawGetTextSize(Ihandle ih, string str, int len, out int w, out int h)
        {
            CheckThread();
            IupDrawGetTextSize_native(ih, str, len, out w, out h);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IupDrawGetImageInfo")]
        private static extern void IupDrawGetImageInfo_native(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name, out int w, out int h, out int bpp);

        public static void IupDrawGetImageInfo(string name, out int w, out int h, out int bpp)
        {
            CheckThread();
            IupDrawGetImageInfo_native(name, out w, out h, out bpp);
        }

        #endregion
    }
}
