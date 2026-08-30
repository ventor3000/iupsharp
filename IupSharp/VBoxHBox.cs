using System;
using System.Globalization;

namespace IupSharp
{
    /// <summary>
    /// Common base for VBox and HBox: void containers that arrange their children
    /// either vertically (VBox) or horizontally (HBox). Holds every attribute whose
    /// name and behavior is identical between the two - only ALIGNMENT differs in
    /// its semantics between orientations, so it is declared separately on VBox and
    /// HBox with an orientation-appropriate enum.
    /// </summary>
    /// <remarks>
    /// Neither VBox nor HBox has a native representation. Both can be created empty
    /// and filled dynamically using Append (inherited from ContainerControl).
    /// Note that a box will not reduce its children below their natural size in the
    /// box's main axis, even if the dialog has Shrink enabled.
    /// </remarks>
    public class VBoxHBox : ContainerControl
    {
        internal VBoxHBox(nint handle) : base(handle)
        {
        }

        /// <summary>
        /// Gets or sets the space, in pixels, between the children along the box's
        /// main axis (vertical for VBox, horizontal for HBox). Default: 0.
        /// </summary>
        public int Gap
        {
            get { int.TryParse(GetAttribute("GAP"), CultureInfo.InvariantCulture, out int gap); return gap; }
            set => SetAttribute("GAP", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets or sets the gap using the units of the SIZE attribute. It will
        /// actually set the Gap property.
        /// </summary>
        public int CGap
        {
            get { int.TryParse(GetAttribute("CGAP"), CultureInfo.InvariantCulture, out int gap); return gap; }
            set => SetAttribute("CGAP", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Same as Gap, but non-inheritable.
        /// </summary>
        public int NGap
        {
            get { int.TryParse(GetAttribute("NGAP"), CultureInfo.InvariantCulture, out int gap); return gap; }
            set => SetAttribute("NGAP", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Same as CGap, but non-inheritable.
        /// </summary>
        public int NCGap
        {
            get { int.TryParse(GetAttribute("NCGAP"), CultureInfo.InvariantCulture, out int gap); return gap; }
            set => SetAttribute("NCGAP", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets or sets the margin, in pixels, as (width, height) for the horizontal
        /// and vertical margins respectively. Default: (0, 0).
        /// </summary>
        public (int, int) Margin
        {
            get { NativeIup.GetIntInt(Handle, "MARGIN", out int x, out int y); return (x, y); }
            set => SetAttribute("MARGIN", Utils.FormatPadding(value));
        }

        /// <summary>
        /// Gets or sets the margin using the units of the SIZE attribute. It will
        /// actually set the Margin property.
        /// </summary>
        public (int, int) CMargin
        {
            get { NativeIup.GetIntInt(Handle, "CMARGIN", out int x, out int y); return (x, y); }
            set => SetAttribute("CMARGIN", Utils.FormatPadding(value));
        }

        /// <summary>
        /// Same as Margin, but non-inheritable.
        /// </summary>
        public (int, int) NMargin
        {
            get { NativeIup.GetIntInt(Handle, "NMARGIN", out int x, out int y); return (x, y); }
            set => SetAttribute("NMARGIN", Utils.FormatPadding(value));
        }

        /// <summary>
        /// Same as CMargin, but non-inheritable.
        /// </summary>
        public (int, int) NCMargin
        {
            get { NativeIup.GetIntInt(Handle, "NCMARGIN", out int x, out int y); return (x, y); }
            set => SetAttribute("NCMARGIN", Utils.FormatPadding(value));
        }

        /// <summary>
        /// Gets or sets whether all children are forced to get equal space along the
        /// box's main axis, based on the largest/highest child. This does not change
        /// the children's size, only the available space each has to expand into.
        /// Default: false.
        /// (non inheritable)
        /// </summary>
        public bool Homogeneous
        {
            get => GetAttribute("HOMOGENEOUS") == "YES";
            set => SetAttribute("HOMOGENEOUS", value ? "YES" : "NO");
        }

        /// <summary>
        /// Gets or sets whether all children are forced to expand along the box's
        /// cross axis (horizontally for VBox, vertically for HBox) and fully occupy
        /// the space available inside the box. This has the same effect as setting
        /// Expand on each child individually. Default: false.
        /// (non inheritable)
        /// </summary>
        public bool ExpandChildren
        {
            get => GetAttribute("EXPANDCHILDREN") == "YES";
            set => SetAttribute("EXPANDCHILDREN", value ? "YES" : "NO");
        }

        static readonly (string, NormalizeSize)[] normalizeSizes = new[]
        {
            ("NO", NormalizeSize.No),
            ("HORIZONTAL", NormalizeSize.Horizontal),
            ("VERTICAL", NormalizeSize.Vertical),
            ("BOTH", NormalizeSize.Both)
        };

        /// <summary>
        /// Gets or sets whether children's natural size is normalized to the
        /// biggest natural size among them - all natural widths set to the biggest
        /// width and/or all natural heights set to the biggest height, according to
        /// the chosen axis. Equivalent to using a Normalizer. Default: No.
        /// (non inheritable)
        /// </summary>
        public NormalizeSize NormalizeSize
        {
            get => Utils.MapAttrib(GetAttribute("NORMALIZESIZE"), normalizeSizes);
            set => SetAttribute("NORMALIZESIZE", Utils.MapEnum(value, normalizeSizes));
        }

        /// <summary>
        /// Gets the layout direction of this box: Vertical for VBox, Horizontal for
        /// HBox.
        /// (read-only) (non inheritable)
        /// </summary>
        public Orientation Orientation =>
            GetAttribute("ORIENTATION") == "HORIZONTAL" ? Orientation.Horizontal : Orientation.Vertical;
    }

    /// <summary>
    /// Creates a void container for composing elements vertically. It is a box
    /// that arranges the elements it contains from top to bottom.
    /// </summary>
    /// <remarks>
    /// Does not have a native representation.
    /// </remarks>
    public class VBox : VBoxHBox
    {
        /// <summary>
        /// Creates a new VBox containing the given children, appended in order.
        /// </summary>
        /// <param name="children">The controls to place in the box, top to bottom.</param>
        public VBox(params Control[] children) : base(NativeIup.IupVboxv(IntPtr.Zero))
        {
            foreach (var w in children)
                Append(w);
        }

        static readonly (string, HorizontalAlignment)[] alignments = new[]
        {
            ("ALEFT", HorizontalAlignment.Left),
            ("ACENTER", HorizontalAlignment.Center),
            ("ARIGHT", HorizontalAlignment.Right)
        };

        /// <summary>
        /// Gets or sets how the children are horizontally aligned within the box.
        /// Default: Left.
        /// (non inheritable)
        /// </summary>
        public HorizontalAlignment Alignment
        {
            get => Utils.MapAttrib(GetAttribute("ALIGNMENT"), alignments);
            set => SetAttribute("ALIGNMENT", Utils.MapEnum(value, alignments));
        }
    }

    /// <summary>
    /// Creates a void container for composing elements horizontally. It is a box
    /// that arranges the elements it contains from left to right.
    /// </summary>
    /// <remarks>
    /// Does not have a native representation.
    /// </remarks>
    public class HBox : VBoxHBox
    {
        /// <summary>
        /// Creates a new HBox containing the given children, appended in order.
        /// </summary>
        /// <param name="children">The controls to place in the box, left to right.</param>
        public HBox(params Control[] children) : base(NativeIup.IupHboxv(IntPtr.Zero))
        {
            foreach (var w in children)
                Append(w);
        }

        static readonly (string, VerticalAlignment)[] alignments = new[]
        {
            ("ATOP", VerticalAlignment.Top),
            ("ACENTER", VerticalAlignment.Center),
            ("ABOTTOM", VerticalAlignment.Bottom)
        };

        /// <summary>
        /// Gets or sets how the children are vertically aligned within the box.
        /// Default: Top.
        /// (non inheritable)
        /// </summary>
        public VerticalAlignment Alignment
        {
            get => Utils.MapAttrib(GetAttribute("ALIGNMENT"), alignments);
            set => SetAttribute("ALIGNMENT", Utils.MapEnum(value, alignments));
        }
    }
}