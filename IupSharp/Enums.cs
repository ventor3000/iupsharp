using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IupSharp
{

    [Flags]
    public enum Alignment
    {
        Left = 1,
        Center = 2,
        Right = 4,

        Top = 8,
        Middle= 16,
        Bottom = 32,

        TopLeft = Top | Left,
        TopCenter = Top | Center,
        TopRight = Top | Right,
        MiddleLeft = Middle | Left,
        MiddleCenter = Middle | Center,
        MiddleRight = Middle | Right,
        BottomLeft = Bottom | Left,
        BottomCenter = Bottom | Center,
        BottomRight = Bottom | Right,
    }

    public enum Expand
    {
        No,
        Yes,
        Horizontal,
        Vertical,
        HorizontalFree,
        VerticalFree,
    }

    public enum ImagePosition
    {
        Left,
        Right,
        Top,
        Bottom
    }

    public enum SeparatorOrientation
    {
        No,
        Horizontal,
        Vertical,
    }

   
    /// <summary>Position of an element in the Z order relative to its siblings.</summary>
    public enum ZOrder
    {
        Top,
        Bottom
    }

    /// <summary>Pre-defined icon shown in the title area of a balloon tip.</summary>
    public enum TipIcon
    {
        None = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }

    /// <summary>
    /// Horizontal alignment of the children of a VBox (which arranges its
    /// children vertically, so aligns them along the horizontal axis).
    /// </summary>
    public enum HorizontalAlignment
    {
        Left,
        Center,
        Right
    }

    /// <summary>
    /// Vertical alignment of the children of an HBox (which arranges its
    /// children horizontally, so aligns them along the vertical axis).
    /// </summary>
    public enum VerticalAlignment
    {
        Top,
        Center,
        Bottom
    }

    /// <summary>
    /// Controls how a VBox/HBox normalizes the natural size of its children,
    /// via the NORMALIZESIZE attribute.
    /// </summary>
    public enum NormalizeSize
    {
        No,
        Horizontal,
        Vertical,
        Both
    }

    /// <summary>
    /// The layout direction of a box container, as reported by its read-only
    /// ORIENTATION attribute.
    /// </summary>
    public enum BoxOrientation
    {
        Horizontal,
        Vertical
    }

    public enum OpenResult
    {
        NoError=0,
        Error=-1,
        AlreadyOpen=1
    }

    /// <summary>Which scrollbars a canvas has.</summary>
    public enum ScrollBars
    {
        /// <summary>No scrollbars.</summary>
        No,
        /// <summary>Horizontal scrollbar only.</summary>
        Horizontal,
        /// <summary>Vertical scrollbar only.</summary>
        Vertical,
        /// <summary>Both scrollbars.</summary>
        Both
    }

    /// <summary>
    /// The operation performed on a scrollbar, as reported by ScrollCB. Values match
    /// the IUP_SB* enumeration in iup.h.
    /// </summary>
    public enum ScrollOperation
    {
        /// <summary>Vertical: line up.</summary>
        Up = 0,
        /// <summary>Vertical: line down.</summary>
        Down = 1,
        /// <summary>Vertical: page up.</summary>
        PageUp = 2,
        /// <summary>Vertical: page down.</summary>
        PageDown = 3,
        /// <summary>Vertical: position changed.</summary>
        PosV = 4,
        /// <summary>Vertical: thumb dragged.</summary>
        DragV = 5,
        /// <summary>Horizontal: column left.</summary>
        Left = 6,
        /// <summary>Horizontal: column right.</summary>
        Right = 7,
        /// <summary>Horizontal: page left.</summary>
        PageLeft = 8,
        /// <summary>Horizontal: page right.</summary>
        PageRight = 9,
        /// <summary>Horizontal: position changed.</summary>
        PosH = 10,
        /// <summary>Horizontal: thumb dragged.</summary>
        DragH = 11
    }

    /// <summary>State of a single touch point.</summary>
    public enum TouchState
    {
        /// <summary>Unrecognised state.</summary>
        Unknown,
        /// <summary>The point was pressed.</summary>
        Down,
        /// <summary>The point moved.</summary>
        Move,
        /// <summary>The point was released.</summary>
        Up
    }

    /// <summary>
    /// Value returned from a callback to tell IUP how to proceed. Values match the
    /// callback return constants in iup.h.
    ///
    /// <para>Not every value is meaningful in every callback. Default is always
    /// accepted; the others are only honoured where the specific callback documents
    /// them, and are ignored elsewhere. Check the callback's documentation before
    /// using anything other than Default.</para>
    /// </summary>
    public enum CallbackResult
    {
        /// <summary>
        /// Proceed normally. This is the default for every callback and is always a
        /// valid answer.
        /// </summary>
        Default = -2,

        /// <summary>
        /// Discard the event, so the control does not process it.
        ///
        /// <para>In KAny and KeyPressCB this stops the key reaching the control - the
        /// documented way to keep the arrow keys from moving focus away from a canvas.
        /// In CloseCB it cancels the close and keeps the dialog open. In ResizeCB it
        /// prevents the layout being recalculated. In a Text or List ACTION it rejects
        /// the character or selection.</para>
        /// </summary>
        Ignore = -1,

        /// <summary>
        /// Close the current dialog and end the main loop, as though ExitLoop had been
        /// called. Honoured by most callbacks, including button actions, CloseCB,
        /// HelpCB, TrayClickCB, TouchCB and MultiTouchCB.
        /// </summary>
        Close = -3,

        /// <summary>
        /// Pass the event on to the next element, letting it be processed further up
        /// the hierarchy. Used mainly by KAny, where it forwards the key to the parent
        /// so a dialog can handle keys on behalf of its children.
        ///
        /// <para>Not valid in CloseCB - use Ignore there to cancel a close.</para>
        /// </summary>
        Continue = -4
    }

    /// <summary>Horizontal text alignment inside a Text control.</summary>
    public enum TextAlignment
    {
        /// <summary>Left aligned. This is the default, and the only option in Motif.</summary>
        Left,
        /// <summary>Centred.</summary>
        Center,
        /// <summary>Right aligned.</summary>
        Right
    }

    /// <summary>Case conversion applied by Text.ChangeCase.</summary>
    public enum TextCase
    {
        /// <summary>Convert everything to upper case.</summary>
        Upper,
        /// <summary>Convert everything to lower case.</summary>
        Lower,
        /// <summary>Invert the case of each character.</summary>
        Toggle,
        /// <summary>
        /// Title case: the first letter of each space-separated word becomes upper case
        /// and the rest lower case, but only for words longer than three characters.
        /// For example "Best of the World".
        /// </summary>
        Title
    }

    /// <summary>Filter applied to characters typed into a Text control.</summary>
    public enum TextFilter
    {
        /// <summary>No filtering.</summary>
        None,
        /// <summary>Force typed characters to lower case.</summary>
        Lowercase,
        /// <summary>Force typed characters to upper case.</summary>
        Uppercase,
        /// <summary>Allow digits only.</summary>
        Number
    }

    /// <summary>
    /// Where the drop child appears relative to the drop button. The named corner of
    /// the drop child is aligned with the opposite corner of the button.
    /// </summary>
    public enum DropPosition
    {
        /// <summary>
        /// The top-left corner of the drop child aligns with the bottom-left corner of
        /// the button. This is the default.
        /// </summary>
        BottomLeft,
        /// <summary>
        /// The bottom-left corner of the drop child aligns with the top-left corner of
        /// the button.
        /// </summary>
        TopLeft,
        /// <summary>
        /// The top-right corner of the drop child aligns with the bottom-right corner
        /// of the button.
        /// </summary>
        BottomRight,
        /// <summary>
        /// The bottom-right corner of the drop child aligns with the top-right corner
        /// of the button.
        /// </summary>
        TopRight
    }

    /// <summary>
    /// State reported by the ShowCB callback.
    /// </summary>
    public enum ShowState
    {
        /// <summary>The dialog was shown.</summary>
        Show = 0,
        /// <summary>The dialog was restored from minimized or maximized.</summary>
        Restore = 1,
        /// <summary>The dialog was minimized.</summary>
        Minimize = 2,
        /// <summary>The dialog was maximized. Not received in Motif.</summary>
        Maximize = 3,
        /// <summary>The dialog was hidden.</summary>
        Hide = 4
    }

    /// <summary>
    /// IUP keyboard codes, as reported by the KAny callback and used by the Key
    /// attribute of menu items. Generated from iupkey.h of IUP 3.32.
    ///
    /// <para>Modifiers occupy the top four bits and are independent flags, so they
    /// combine freely with the | operator and with each other:
    /// <c>Key.Ctrl | Key.S</c>, <c>Key.Ctrl | Key.Shift | Key.F1</c>. Since IUP 3.9
    /// any modifier can be combined with any key and modifiers can be mixed.</para>
    ///
    /// <para>Printable keys use the ASCII character code, so <c>Key.A</c> is 65.
    /// Extended keys use the same values as X11 and GDK, which means any X11 or GDK
    /// keysym may also be used. Note that Shift on a printable key is reported as the
    /// shifted character rather than as the Shift flag: pressing Shift+a gives
    /// <c>Key.A</c>, not <c>Key.Shift | Key.a</c>.</para>
    ///
    /// <para>The enum is deliberately NOT marked [Flags]: base keys are ordinary
    /// values rather than bits, so a flags-style ToString would be nonsense for them.
    /// The | operator works on any C# enum regardless.</para>
    ///
    /// <para>Sys sets bit 31, so combinations including it are negative when viewed
    /// as a signed int. The header notes this too.</para>
    /// </summary>
    public enum Key
    {
        /// <summary>No key.</summary>
        None = 0,

        // ---- Modifiers: independent flags, freely combinable ----

        /// <summary>Shift modifier (0x10000000).</summary>
        Shift = 0x10000000,
        /// <summary>Control modifier (0x20000000).</summary>
        Ctrl = 0x20000000,
        /// <summary>Alt modifier (0x40000000).</summary>
        Alt = 0x40000000,
        /// <summary>System modifier: the Windows key, or the Apple key on Mac (0x80000000).</summary>
        Sys = unchecked((int)0x80000000),

        // ---- Control characters ----

        /// <summary>K_BS (0x8 = 8).</summary>
        Backspace = 0x8,
        /// <summary>K_TAB (0x9 = 9).</summary>
        Tab = 0x9,
        /// <summary>K_LF (0xA = 10). Not a real key; it is Enter with a modifier, present for documentation.</summary>
        LineFeed = 0xA,
        /// <summary>K_CR (0xD = 13).</summary>
        Enter = 0xD,

        // ---- Printable ASCII: the code is the character code ----

        /// <summary>K_SP (0x20 = 32).</summary>
        Space = 0x20,
        /// <summary>K_exclam (0x21 = 33).</summary>
        Exclam = 0x21,
        /// <summary>K_quotedbl (0x22 = 34).</summary>
        QuoteDbl = 0x22,
        /// <summary>K_numbersign (0x23 = 35).</summary>
        NumberSign = 0x23,
        /// <summary>K_dollar (0x24 = 36).</summary>
        Dollar = 0x24,
        /// <summary>K_percent (0x25 = 37).</summary>
        Percent = 0x25,
        /// <summary>K_ampersand (0x26 = 38).</summary>
        Ampersand = 0x26,
        /// <summary>K_apostrophe (0x27 = 39).</summary>
        Apostrophe = 0x27,
        /// <summary>K_parentleft (0x28 = 40).</summary>
        ParenLeft = 0x28,
        /// <summary>K_parentright (0x29 = 41).</summary>
        ParenRight = 0x29,
        /// <summary>K_asterisk (0x2A = 42).</summary>
        Asterisk = 0x2A,
        /// <summary>K_plus (0x2B = 43).</summary>
        Plus = 0x2B,
        /// <summary>K_comma (0x2C = 44).</summary>
        Comma = 0x2C,
        /// <summary>K_minus (0x2D = 45).</summary>
        Minus = 0x2D,
        /// <summary>K_period (0x2E = 46).</summary>
        Period = 0x2E,
        /// <summary>K_slash (0x2F = 47).</summary>
        Slash = 0x2F,
        /// <summary>K_0 (0x30 = 48).</summary>
        D0 = 0x30,
        /// <summary>K_1 (0x31 = 49).</summary>
        D1 = 0x31,
        /// <summary>K_2 (0x32 = 50).</summary>
        D2 = 0x32,
        /// <summary>K_3 (0x33 = 51).</summary>
        D3 = 0x33,
        /// <summary>K_4 (0x34 = 52).</summary>
        D4 = 0x34,
        /// <summary>K_5 (0x35 = 53).</summary>
        D5 = 0x35,
        /// <summary>K_6 (0x36 = 54).</summary>
        D6 = 0x36,
        /// <summary>K_7 (0x37 = 55).</summary>
        D7 = 0x37,
        /// <summary>K_8 (0x38 = 56).</summary>
        D8 = 0x38,
        /// <summary>K_9 (0x39 = 57).</summary>
        D9 = 0x39,
        /// <summary>K_colon (0x3A = 58).</summary>
        Colon = 0x3A,
        /// <summary>K_semicolon (0x3B = 59).</summary>
        Semicolon = 0x3B,
        /// <summary>K_less (0x3C = 60).</summary>
        Less = 0x3C,
        /// <summary>K_equal (0x3D = 61).</summary>
        Equal = 0x3D,
        /// <summary>K_greater (0x3E = 62).</summary>
        Greater = 0x3E,
        /// <summary>K_question (0x3F = 63).</summary>
        Question = 0x3F,
        /// <summary>K_at (0x40 = 64).</summary>
        At = 0x40,
        /// <summary>K_A (0x41 = 65).</summary>
        A = 0x41,
        /// <summary>K_B (0x42 = 66).</summary>
        B = 0x42,
        /// <summary>K_C (0x43 = 67).</summary>
        C = 0x43,
        /// <summary>K_D (0x44 = 68).</summary>
        D = 0x44,
        /// <summary>K_E (0x45 = 69).</summary>
        E = 0x45,
        /// <summary>K_F (0x46 = 70).</summary>
        F = 0x46,
        /// <summary>K_G (0x47 = 71).</summary>
        G = 0x47,
        /// <summary>K_H (0x48 = 72).</summary>
        H = 0x48,
        /// <summary>K_I (0x49 = 73).</summary>
        I = 0x49,
        /// <summary>K_J (0x4A = 74).</summary>
        J = 0x4A,
        /// <summary>K_K (0x4B = 75).</summary>
        K = 0x4B,
        /// <summary>K_L (0x4C = 76).</summary>
        L = 0x4C,
        /// <summary>K_M (0x4D = 77).</summary>
        M = 0x4D,
        /// <summary>K_N (0x4E = 78).</summary>
        N = 0x4E,
        /// <summary>K_O (0x4F = 79).</summary>
        O = 0x4F,
        /// <summary>K_P (0x50 = 80).</summary>
        P = 0x50,
        /// <summary>K_Q (0x51 = 81).</summary>
        Q = 0x51,
        /// <summary>K_R (0x52 = 82).</summary>
        R = 0x52,
        /// <summary>K_S (0x53 = 83).</summary>
        S = 0x53,
        /// <summary>K_T (0x54 = 84).</summary>
        T = 0x54,
        /// <summary>K_U (0x55 = 85).</summary>
        U = 0x55,
        /// <summary>K_V (0x56 = 86).</summary>
        V = 0x56,
        /// <summary>K_W (0x57 = 87).</summary>
        W = 0x57,
        /// <summary>K_X (0x58 = 88).</summary>
        X = 0x58,
        /// <summary>K_Y (0x59 = 89).</summary>
        Y = 0x59,
        /// <summary>K_Z (0x5A = 90).</summary>
        Z = 0x5A,
        /// <summary>K_bracketleft (0x5B = 91).</summary>
        BracketLeft = 0x5B,
        /// <summary>K_backslash (0x5C = 92).</summary>
        Backslash = 0x5C,
        /// <summary>K_bracketright (0x5D = 93).</summary>
        BracketRight = 0x5D,
        /// <summary>K_circum (0x5E = 94).</summary>
        Circum = 0x5E,
        /// <summary>K_underscore (0x5F = 95).</summary>
        Underscore = 0x5F,
        /// <summary>K_grave (0x60 = 96).</summary>
        Grave = 0x60,
        /// <summary>K_a (0x61 = 97).</summary>
        a = 0x61,
        /// <summary>K_b (0x62 = 98).</summary>
        b = 0x62,
        /// <summary>K_c (0x63 = 99).</summary>
        c = 0x63,
        /// <summary>K_d (0x64 = 100).</summary>
        d = 0x64,
        /// <summary>K_e (0x65 = 101).</summary>
        e = 0x65,
        /// <summary>K_f (0x66 = 102).</summary>
        f = 0x66,
        /// <summary>K_g (0x67 = 103).</summary>
        g = 0x67,
        /// <summary>K_h (0x68 = 104).</summary>
        h = 0x68,
        /// <summary>K_i (0x69 = 105).</summary>
        i = 0x69,
        /// <summary>K_j (0x6A = 106).</summary>
        j = 0x6A,
        /// <summary>K_k (0x6B = 107).</summary>
        k = 0x6B,
        /// <summary>K_l (0x6C = 108).</summary>
        l = 0x6C,
        /// <summary>K_m (0x6D = 109).</summary>
        m = 0x6D,
        /// <summary>K_n (0x6E = 110).</summary>
        n = 0x6E,
        /// <summary>K_o (0x6F = 111).</summary>
        o = 0x6F,
        /// <summary>K_p (0x70 = 112).</summary>
        p = 0x70,
        /// <summary>K_q (0x71 = 113).</summary>
        q = 0x71,
        /// <summary>K_r (0x72 = 114).</summary>
        r = 0x72,
        /// <summary>K_s (0x73 = 115).</summary>
        s = 0x73,
        /// <summary>K_t (0x74 = 116).</summary>
        t = 0x74,
        /// <summary>K_u (0x75 = 117).</summary>
        u = 0x75,
        /// <summary>K_v (0x76 = 118).</summary>
        v = 0x76,
        /// <summary>K_w (0x77 = 119).</summary>
        w = 0x77,
        /// <summary>K_x (0x78 = 120).</summary>
        x = 0x78,
        /// <summary>K_y (0x79 = 121).</summary>
        y = 0x79,
        /// <summary>K_z (0x7A = 122).</summary>
        z = 0x7A,
        /// <summary>K_braceleft (0x7B = 123).</summary>
        BraceLeft = 0x7B,
        /// <summary>K_bar (0x7C = 124).</summary>
        Bar = 0x7C,
        /// <summary>K_braceright (0x7D = 125).</summary>
        BraceRight = 0x7D,
        /// <summary>K_tilde (0x7E = 126).</summary>
        Tilde = 0x7E,

        // ---- Latin-1 keys ----

        /// <summary>K_diaeresis (0xA8 = 168).</summary>
        Diaeresis = 0xA8,
        /// <summary>K_acute (0xB4 = 180). Has no Shift, Ctrl or Alt variant.</summary>
        Acute = 0xB4,
        /// <summary>K_Ccedilla (0xC7 = 199).</summary>
        CCedillaUpper = 0xC7,
        /// <summary>K_ccedilla (0xE7 = 231).</summary>
        CCedilla = 0xE7,

        // ---- Extended keys: same values as X11 and GDK ----

        /// <summary>K_MIDDLE (0xFF0B = 65291). Numeric keypad 5 with NumLock off.</summary>
        Middle = 0xFF0B,
        /// <summary>K_PAUSE (0xFF13 = 65299).</summary>
        Pause = 0xFF13,
        /// <summary>K_SCROLL (0xFF14 = 65300).</summary>
        ScrollLock = 0xFF14,
        /// <summary>K_ESC (0xFF1B = 65307).</summary>
        Escape = 0xFF1B,
        /// <summary>K_HOME (0xFF50 = 65360).</summary>
        Home = 0xFF50,
        /// <summary>K_LEFT (0xFF51 = 65361).</summary>
        Left = 0xFF51,
        /// <summary>K_UP (0xFF52 = 65362).</summary>
        Up = 0xFF52,
        /// <summary>K_RIGHT (0xFF53 = 65363).</summary>
        Right = 0xFF53,
        /// <summary>K_DOWN (0xFF54 = 65364).</summary>
        Down = 0xFF54,
        /// <summary>K_PGUP (0xFF55 = 65365).</summary>
        PageUp = 0xFF55,
        /// <summary>K_PGDN (0xFF56 = 65366).</summary>
        PageDown = 0xFF56,
        /// <summary>K_END (0xFF57 = 65367).</summary>
        End = 0xFF57,
        /// <summary>K_Print (0xFF61 = 65377).</summary>
        PrintScreen = 0xFF61,
        /// <summary>K_INS (0xFF63 = 65379).</summary>
        Insert = 0xFF63,
        /// <summary>K_Menu (0xFF67 = 65383).</summary>
        Menu = 0xFF67,
        /// <summary>K_NUM (0xFF7F = 65407).</summary>
        NumLock = 0xFF7F,
        /// <summary>K_F1 (0xFFBE = 65470).</summary>
        F1 = 0xFFBE,
        /// <summary>K_F2 (0xFFBF = 65471).</summary>
        F2 = 0xFFBF,
        /// <summary>K_F3 (0xFFC0 = 65472).</summary>
        F3 = 0xFFC0,
        /// <summary>K_F4 (0xFFC1 = 65473).</summary>
        F4 = 0xFFC1,
        /// <summary>K_F5 (0xFFC2 = 65474).</summary>
        F5 = 0xFFC2,
        /// <summary>K_F6 (0xFFC3 = 65475).</summary>
        F6 = 0xFFC3,
        /// <summary>K_F7 (0xFFC4 = 65476).</summary>
        F7 = 0xFFC4,
        /// <summary>K_F8 (0xFFC5 = 65477).</summary>
        F8 = 0xFFC5,
        /// <summary>K_F9 (0xFFC6 = 65478).</summary>
        F9 = 0xFFC6,
        /// <summary>K_F10 (0xFFC7 = 65479).</summary>
        F10 = 0xFFC7,
        /// <summary>K_F11 (0xFFC8 = 65480).</summary>
        F11 = 0xFFC8,
        /// <summary>K_F12 (0xFFC9 = 65481).</summary>
        F12 = 0xFFC9,
        /// <summary>K_F13 (0xFFCA = 65482).</summary>
        F13 = 0xFFCA,
        /// <summary>K_F14 (0xFFCB = 65483).</summary>
        F14 = 0xFFCB,
        /// <summary>K_F15 (0xFFCC = 65484).</summary>
        F15 = 0xFFCC,
        /// <summary>K_F16 (0xFFCD = 65485).</summary>
        F16 = 0xFFCD,
        /// <summary>K_F17 (0xFFCE = 65486).</summary>
        F17 = 0xFFCE,
        /// <summary>K_F18 (0xFFCF = 65487).</summary>
        F18 = 0xFFCF,
        /// <summary>K_F19 (0xFFD0 = 65488).</summary>
        F19 = 0xFFD0,
        /// <summary>K_F20 (0xFFD1 = 65489).</summary>
        F20 = 0xFFD1,
        /// <summary>K_CLEAR (0xFFD2 = 65490). Mac clear button.</summary>
        Clear = 0xFFD2,
        /// <summary>K_HELP (0xFFD3 = 65491).</summary>
        Help = 0xFFD3,
        /// <summary>K_LSHIFT (0xFFE1 = 65505). Has no Shift, Ctrl or Alt variant.</summary>
        LeftShift = 0xFFE1,
        /// <summary>K_RSHIFT (0xFFE2 = 65506). Has no Shift, Ctrl or Alt variant.</summary>
        RightShift = 0xFFE2,
        /// <summary>K_LCTRL (0xFFE3 = 65507). Has no Shift, Ctrl or Alt variant.</summary>
        LeftCtrl = 0xFFE3,
        /// <summary>K_RCTRL (0xFFE4 = 65508). Has no Shift, Ctrl or Alt variant.</summary>
        RightCtrl = 0xFFE4,
        /// <summary>K_CAPS (0xFFE5 = 65509).</summary>
        CapsLock = 0xFFE5,
        /// <summary>K_LALT (0xFFE9 = 65513). Has no Shift, Ctrl or Alt variant.</summary>
        LeftAlt = 0xFFE9,
        /// <summary>K_RALT (0xFFEA = 65514). Has no Shift, Ctrl or Alt variant.</summary>
        RightAlt = 0xFFEA,
        /// <summary>K_DEL (0xFFFF = 65535).</summary>
        Delete = 0xFFFF,

    }

}
