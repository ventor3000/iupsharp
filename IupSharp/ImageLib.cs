// IUP - Portable User Interface Toolkit
// Managed wrapper for the IupImageLib add-on library (v3.32)
//
// Requires the native iupimglib library: iupimglib.dll on Windows,
// libiupimglib.so on Unix. See IupImageLibNative.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace IupSharp
{
    /// <summary>
    /// A library of pre-defined stock images for buttons, labels and toolbars.
    /// </summary>
    /// <remarks>
    /// <para>Call <see cref="Open"/> once, after <c>Iup.Open</c>, before using any
    /// of the names below. That registers the names; the images themselves are
    /// loaded lazily when a name is first assigned to a control, and released by
    /// <c>Iup.Close</c>.</para>
    ///
    /// <para>The names are NOT registered through IupSetHandle, so
    /// <c>IupGetHandle</c> will not find them. They are only resolved when set as a
    /// control's image attribute value.</para>
    ///
    /// <para><b>Appearance varies by platform.</b> On Windows these are 32x32 32bpp
    /// bitmaps; on Motif 16x16 8bpp; on GTK they come from the theme in several
    /// sizes. Since GTK 3.10 direct access to stock images is deprecated, so from
    /// IUP 3.23 the GTK3 build carries its own copies that the current theme does
    /// not affect.</para>
    ///
    /// <para>Use <see cref="StockSize"/> to force a specific height.</para>
    /// </remarks>
    public static class ImageLib
    {
        /// <summary>
        /// Registers the stock image names with IUP. Call once, after
        /// <c>Iup.Open</c> and before any of the names are used. Calling it more
        /// than once is harmless.
        /// </summary>
        /// <exception cref="DllNotFoundException">
        /// The native iupimglib library could not be found. It must be deployed
        /// alongside the iup library.
        /// </exception>
        public static void Open()
        {
            if (IupNative.IupIsOpened() == 0)
                throw new InvalidOperationException(
                    "Iup.Open() must be called before IupImageLib.Open().");

            IupImageLibNative.IupImageLibOpen();
        }

        /// <summary>
        /// Gets or sets the height forced for stock images, in pixels. Valid values
        /// are 24, 32 and 48; anything else is ignored by IUP. Set it before the
        /// images are first used.
        /// </summary>
        /// <remarks>
        /// <para>This is the IMAGESTOCKSIZE global attribute (since 3.16). When not
        /// set, the size follows the screen resolution: 24 at 144 DPI or less, 32 at
        /// 192 DPI, 48 at 288 DPI. Returns 0 when it has not been set.</para>
        ///
        /// <para>On GTK an unavailable size is scaled from one that exists; on
        /// Windows anything other than 32x32 is resized. The minimum resulting height
        /// is 24 pixels (since 3.29).</para>
        /// </remarks>
        public static int StockSize
        {
            get
            {
                string v = IupNative.GetGlobal("IMAGESTOCKSIZE");
                return int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out int i)
                    ? i
                    : 0;
            }
            set => IupNative.SetStrGlobal("IMAGESTOCKSIZE",
                value.ToString(CultureInfo.InvariantCulture));
        }

        #region BASE LIBRARY GROUP

        // Toolbar and button icons. On Windows 32x32 32bpp, on Motif 16x16 8bpp,
        // on GTK theme-dependent sizes from 16x16 up to 48x48.

        /// <summary>Cancel an action.</summary>
        public const string ActionCancel = "IUP_ActionCancel";

        /// <summary>Confirm an action.</summary>
        public const string ActionOk = "IUP_ActionOk";

        /// <summary>Downward arrow.</summary>
        public const string ArrowDown = "IUP_ArrowDown";

        /// <summary>Left-pointing arrow.</summary>
        public const string ArrowLeft = "IUP_ArrowLeft";

        /// <summary>Right-pointing arrow.</summary>
        public const string ArrowRight = "IUP_ArrowRight";

        /// <summary>Upward arrow.</summary>
        public const string ArrowUp = "IUP_ArrowUp";

        /// <summary>Copy to the clipboard.</summary>
        public const string EditCopy = "IUP_EditCopy";

        /// <summary>Cut to the clipboard.</summary>
        public const string EditCut = "IUP_EditCut";

        /// <summary>Delete the selection.</summary>
        public const string EditErase = "IUP_EditErase";

        /// <summary>Find or search.</summary>
        public const string EditFind = "IUP_EditFind";

        /// <summary>Paste from the clipboard.</summary>
        public const string EditPaste = "IUP_EditPaste";

        /// <summary>Redo the last undone action.</summary>
        public const string EditRedo = "IUP_EditRedo";

        /// <summary>Undo the last action.</summary>
        public const string EditUndo = "IUP_EditUndo";

        /// <summary>Close the current file.</summary>
        public const string FileClose = "IUP_FileClose";

        /// <summary>Create a new file.</summary>
        public const string FileNew = "IUP_FileNew";

        /// <summary>Open an existing file.</summary>
        public const string FileOpen = "IUP_FileOpen";

        /// <summary>Show file properties.</summary>
        public const string FileProperties = "IUP_FileProperties";

        /// <summary>Save the current file.</summary>
        public const string FileSave = "IUP_FileSave";

        /// <summary>Seek forward.</summary>
        public const string MediaForward = "IUP_MediaForward";

        /// <summary>Skip to the beginning.</summary>
        public const string MediaGoToBegin = "IUP_MediaGoToBegin";

        /// <summary>Skip to the end.</summary>
        public const string MediaGoToEnd = "IUP_MediaGoToEnd";

        /// <summary>Pause playback.</summary>
        public const string MediaPause = "IUP_MediaPause";

        /// <summary>Start playback.</summary>
        public const string MediaPlay = "IUP_MediaPlay";

        /// <summary>Start recording.</summary>
        public const string MediaRecord = "IUP_MediaRecord";

        /// <summary>Play in reverse.</summary>
        public const string MediaReverse = "IUP_MediaReverse";

        /// <summary>Seek backward.</summary>
        public const string MediaRewind = "IUP_MediaRewind";

        /// <summary>Stop playback.</summary>
        public const string MediaStop = "IUP_MediaStop";

        /// <summary>Error indication.</summary>
        public const string MessageError = "IUP_MessageError";

        /// <summary>Help contents.</summary>
        public const string MessageHelp = "IUP_MessageHelp";

        /// <summary>Information indication.</summary>
        public const string MessageInfo = "IUP_MessageInfo";

        /// <summary>Go to the home location.</summary>
        public const string NavigateHome = "IUP_NavigateHome";

        /// <summary>Reload or refresh.</summary>
        public const string NavigateRefresh = "IUP_NavigateRefresh";

        /// <summary>Print the document.</summary>
        public const string Print = "IUP_Print";

        /// <summary>Preview before printing.</summary>
        public const string PrintPreview = "IUP_PrintPreview";

        /// <summary>Choose a colour.</summary>
        public const string ToolsColor = "IUP_ToolsColor";

        /// <summary>Open preferences or settings.</summary>
        public const string ToolsSettings = "IUP_ToolsSettings";

        /// <summary>Sort in ascending order.</summary>
        public const string ToolsSortAscend = "IUP_ToolsSortAscend";

        /// <summary>Sort in descending order.</summary>
        public const string ToolsSortDescend = "IUP_ToolsSortDescend";

        /// <summary>Switch to full screen.</summary>
        public const string ViewFullScreen = "IUP_ViewFullScreen";

        /// <summary>Webcam or camera.</summary>
        public const string Webcam = "IUP_Webcam";

        #endregion

        #region ANIMATIONS

        /// <summary>
        /// An indefinite-progress spinner, for use with AnimatedLabel. Twelve 32x32
        /// 32bpp frames with FRAMETIME set to 83 ms, about one second per turn.
        /// (since 3.17)
        /// </summary>
        public const string CircleProgressAnimation = "IUP_CircleProgressAnimation";

        #endregion

        #region ALL NAMES

        /// <summary>
        /// Every stock image name declared here, in declaration order. Useful for
        /// building an icon browser or verifying that the library opened.
        /// </summary>
        public static IReadOnlyList<string> All { get; } = new[]
        {
            ActionCancel,
            ActionOk,
            ArrowDown,
            ArrowLeft,
            ArrowRight,
            ArrowUp,
            EditCopy,
            EditCut,
            EditErase,
            EditFind,
            EditPaste,
            EditRedo,
            EditUndo,
            FileClose,
            FileNew,
            FileOpen,
            FileProperties,
            FileSave,
            MediaForward,
            MediaGoToBegin,
            MediaGoToEnd,
            MediaPause,
            MediaPlay,
            MediaRecord,
            MediaReverse,
            MediaRewind,
            MediaStop,
            MessageError,
            MessageHelp,
            MessageInfo,
            NavigateHome,
            NavigateRefresh,
            Print,
            PrintPreview,
            ToolsColor,
            ToolsSettings,
            ToolsSortAscend,
            ToolsSortDescend,
            ViewFullScreen,
            Webcam,
            CircleProgressAnimation
        };

        #endregion

        #region REMOVED NAMES

        /// <summary>
        /// Names removed from the pre-compiled library in IUP 3.16. Their C source
        /// is still shipped, so they can be rebuilt into a custom library, but they
        /// will not resolve against the stock iupimglib and are listed only so that
        /// older code can be recognised.
        /// </summary>
        public static IReadOnlyList<string> RemovedIn316 { get; } = new[]
        {
            "IUP_FileCloseAll",
            "IUP_FileSaveAll",
            "IUP_FileText",
            "IUP_FontBold",
            "IUP_FontDialog",
            "IUP_FontItalic",
            "IUP_WindowsCascade",
            "IUP_WindowsTile",
            "IUP_Zoom",
        };

        #endregion
    }
}