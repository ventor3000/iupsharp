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

    public enum Separator
    {
        No,
        Horizontal,
        Vertical,
    }

    public enum OpenResult
    {
        NoError,
        Error,
        AlreadyOpen
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
    /// IUP keyboard codes, as reported by the KAny callback and used by the Key
    /// attribute of menu items.
    ///
    /// <para>A single modifier may be combined with a base key using the | operator,
    /// for example <c>Key.Ctrl | Key.S</c>. This works because every base code is
    /// below 256 and each modifier is a multiple of 256.</para>
    ///
    /// <para><b>Only one modifier can be encoded.</b> IUP stores the modifier as an
    /// additive offset rather than as independent bits, and Alt (768) happens to
    /// equal Shift (256) + Ctrl (512). Combining two modifiers therefore produces a
    /// different, valid-looking key rather than an error:
    /// <c>Key.Ctrl | Key.Shift | Key.S</c> silently means <c>Alt+S</c>. Use
    /// <see cref="KeyExtensions.IsValid"/> to check, or read the MODKEYSTATE global
    /// attribute when you need to detect two or more modifiers at once.</para>
    ///
    /// <para>Note this enum is deliberately NOT marked [Flags]: the values are not
    /// independent bits, and a flags-style ToString would decompose Alt into
    /// "Shift, Ctrl".</para>
    ///
    /// <para>Shift on a printable key is not a modifier. Shift+a arrives as
    /// <c>Key.A</c> (65), not as <c>Key.Shift | Key.a</c>. The Shift modifier only
    /// applies to extended keys such as the function and navigation keys.</para>
    ///
    /// <para>IUP uses the US keyboard codification, so on other layouts a small
    /// group of keys will report codes that do not match the physical key. Some
    /// combinations are also reserved by the system and never arrive at all, among
    /// them Shift+Insert, Shift+Delete, Alt+Space, Ctrl+Enter and Alt+Enter.</para>
    /// </summary>
    public enum Key
    {
        /// <summary>No key.</summary>
        None = 0,

        // ---- Modifiers (combine ONE of these with a base key using |) ----

        /// <summary>Shift modifier. Applies to extended keys only.</summary>
        Shift = 256,
        /// <summary>Control modifier.</summary>
        Ctrl = 512,
        /// <summary>Alt modifier. Note this equals Shift | Ctrl numerically.</summary>
        Alt = 768,
        /// <summary>System modifier: the Windows key, or the Apple key on Mac.</summary>
        Sys = 1024,

        // ---- Control characters ----

        /// <summary>K_BS (8).</summary>
        Backspace = 8,
        /// <summary>K_TAB (9).</summary>
        Tab = 9,
        /// <summary>K_LF (10). Not a real key; it is Enter with a modifier, present for documentation.</summary>
        LineFeed = 10,
        /// <summary>K_CR (13).</summary>
        Enter = 13,

        // ---- Printable ASCII: the code is the character code ----

        /// <summary>K_SP (32).</summary>
        Space = 32,
        /// <summary>K_exclam (33).</summary>
        Exclam = 33,
        /// <summary>K_quotedbl (34).</summary>
        QuoteDbl = 34,
        /// <summary>K_numbersign (35).</summary>
        NumberSign = 35,
        /// <summary>K_dollar (36).</summary>
        Dollar = 36,
        /// <summary>K_percent (37).</summary>
        Percent = 37,
        /// <summary>K_ampersand (38).</summary>
        Ampersand = 38,
        /// <summary>K_apostrophe (39).</summary>
        Apostrophe = 39,
        /// <summary>K_parentleft (40).</summary>
        ParenLeft = 40,
        /// <summary>K_parentright (41).</summary>
        ParenRight = 41,
        /// <summary>K_asterisk (42).</summary>
        Asterisk = 42,
        /// <summary>K_plus (43).</summary>
        Plus = 43,
        /// <summary>K_comma (44).</summary>
        Comma = 44,
        /// <summary>K_minus (45).</summary>
        Minus = 45,
        /// <summary>K_period (46).</summary>
        Period = 46,
        /// <summary>K_slash (47).</summary>
        Slash = 47,

        /// <summary>K_0 (48).</summary>
        D0 = 48,
        /// <summary>K_1 (49).</summary>
        D1 = 49,
        /// <summary>K_2 (50).</summary>
        D2 = 50,
        /// <summary>K_3 (51).</summary>
        D3 = 51,
        /// <summary>K_4 (52).</summary>
        D4 = 52,
        /// <summary>K_5 (53).</summary>
        D5 = 53,
        /// <summary>K_6 (54).</summary>
        D6 = 54,
        /// <summary>K_7 (55).</summary>
        D7 = 55,
        /// <summary>K_8 (56).</summary>
        D8 = 56,
        /// <summary>K_9 (57).</summary>
        D9 = 57,

        /// <summary>K_colon (58).</summary>
        Colon = 58,
        /// <summary>K_semicolon (59).</summary>
        Semicolon = 59,
        /// <summary>K_less (60).</summary>
        Less = 60,
        /// <summary>K_equal (61).</summary>
        Equal = 61,
        /// <summary>K_greater (62).</summary>
        Greater = 62,
        /// <summary>K_question (63).</summary>
        Question = 63,
        /// <summary>K_at (64).</summary>
        At = 64,

        /// <summary>K_A (65).</summary>
        A = 65,
        /// <summary>K_B (66).</summary>
        B = 66,
        /// <summary>K_C (67).</summary>
        C = 67,
        /// <summary>K_D (68).</summary>
        D = 68,
        /// <summary>K_E (69).</summary>
        E = 69,
        /// <summary>K_F (70).</summary>
        F = 70,
        /// <summary>K_G (71).</summary>
        G = 71,
        /// <summary>K_H (72).</summary>
        H = 72,
        /// <summary>K_I (73).</summary>
        I = 73,
        /// <summary>K_J (74).</summary>
        J = 74,
        /// <summary>K_K (75).</summary>
        K = 75,
        /// <summary>K_L (76).</summary>
        L = 76,
        /// <summary>K_M (77).</summary>
        M = 77,
        /// <summary>K_N (78).</summary>
        N = 78,
        /// <summary>K_O (79).</summary>
        O = 79,
        /// <summary>K_P (80).</summary>
        P = 80,
        /// <summary>K_Q (81).</summary>
        Q = 81,
        /// <summary>K_R (82).</summary>
        R = 82,
        /// <summary>K_S (83).</summary>
        S = 83,
        /// <summary>K_T (84).</summary>
        T = 84,
        /// <summary>K_U (85).</summary>
        U = 85,
        /// <summary>K_V (86).</summary>
        V = 86,
        /// <summary>K_W (87).</summary>
        W = 87,
        /// <summary>K_X (88).</summary>
        X = 88,
        /// <summary>K_Y (89).</summary>
        Y = 89,
        /// <summary>K_Z (90).</summary>
        Z = 90,

        /// <summary>K_bracketleft (91).</summary>
        BracketLeft = 91,
        /// <summary>K_backslash (92).</summary>
        Backslash = 92,
        /// <summary>K_bracketright (93).</summary>
        BracketRight = 93,
        /// <summary>K_circum (94).</summary>
        Circum = 94,
        /// <summary>K_underscore (95).</summary>
        Underscore = 95,
        /// <summary>K_grave (96).</summary>
        Grave = 96,

        /// <summary>K_a (97).</summary>
        a = 97,
        /// <summary>K_b (98).</summary>
        b = 98,
        /// <summary>K_c (99).</summary>
        c = 99,
        /// <summary>K_d (100).</summary>
        d = 100,
        /// <summary>K_e (101).</summary>
        e = 101,
        /// <summary>K_f (102).</summary>
        f = 102,
        /// <summary>K_g (103).</summary>
        g = 103,
        /// <summary>K_h (104).</summary>
        h = 104,
        /// <summary>K_i (105).</summary>
        i = 105,
        /// <summary>K_j (106).</summary>
        j = 106,
        /// <summary>K_k (107).</summary>
        k = 107,
        /// <summary>K_l (108).</summary>
        l = 108,
        /// <summary>K_m (109).</summary>
        m = 109,
        /// <summary>K_n (110).</summary>
        n = 110,
        /// <summary>K_o (111).</summary>
        o = 111,
        /// <summary>K_p (112).</summary>
        p = 112,
        /// <summary>K_q (113).</summary>
        q = 113,
        /// <summary>K_r (114).</summary>
        r = 114,
        /// <summary>K_s (115).</summary>
        s = 115,
        /// <summary>K_t (116).</summary>
        t = 116,
        /// <summary>K_u (117).</summary>
        u = 117,
        /// <summary>K_v (118).</summary>
        v = 118,
        /// <summary>K_w (119).</summary>
        w = 119,
        /// <summary>K_x (120).</summary>
        x = 120,
        /// <summary>K_y (121).</summary>
        y = 121,
        /// <summary>K_z (122).</summary>
        z = 122,

        /// <summary>K_braceleft (123).</summary>
        BraceLeft = 123,
        /// <summary>K_bar (124).</summary>
        Bar = 124,
        /// <summary>K_braceright (125).</summary>
        BraceRight = 125,
        /// <summary>K_tilde (126).</summary>
        Tilde = 126,

        // ---- Extended keys: base 128 + n ----

        /// <summary>K_HOME (129).</summary>
        Home = 129,
        /// <summary>K_UP (130).</summary>
        Up = 130,
        /// <summary>K_PGUP (131).</summary>
        PageUp = 131,
        /// <summary>K_LEFT (132).</summary>
        Left = 132,
        /// <summary>K_MIDDLE (133).</summary>
        Middle = 133,
        /// <summary>K_RIGHT (134).</summary>
        Right = 134,
        /// <summary>K_END (135).</summary>
        End = 135,
        /// <summary>K_DOWN (136).</summary>
        Down = 136,
        /// <summary>K_PGDN (137).</summary>
        PageDown = 137,
        /// <summary>K_INS (138).</summary>
        Insert = 138,
        /// <summary>K_DEL (139).</summary>
        Delete = 139,
        /// <summary>K_PAUSE (140).</summary>
        Pause = 140,
        /// <summary>K_ESC (141).</summary>
        Escape = 141,
        /// <summary>K_ccedilla (142). Shift gives the uppercase form.</summary>
        CCedilla = 142,
        /// <summary>K_F1 (143).</summary>
        F1 = 143,
        /// <summary>K_F2 (144).</summary>
        F2 = 144,
        /// <summary>K_F3 (145).</summary>
        F3 = 145,
        /// <summary>K_F4 (146).</summary>
        F4 = 146,
        /// <summary>K_F5 (147).</summary>
        F5 = 147,
        /// <summary>K_F6 (148).</summary>
        F6 = 148,
        /// <summary>K_F7 (149).</summary>
        F7 = 149,
        /// <summary>K_F8 (150).</summary>
        F8 = 150,
        /// <summary>K_F9 (151).</summary>
        F9 = 151,
        /// <summary>K_F10 (152).</summary>
        F10 = 152,
        /// <summary>K_F11 (153).</summary>
        F11 = 153,
        /// <summary>K_F12 (154).</summary>
        F12 = 154,
        /// <summary>K_Print (155).</summary>
        PrintScreen = 155,
        /// <summary>K_Menu (156).</summary>
        Menu = 156,
        /// <summary>K_acute (157). Has no Shift, Ctrl or Alt variant.</summary>
        Acute = 157,
    }

    
}
