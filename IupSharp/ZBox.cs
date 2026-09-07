using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace IupSharp
{
    /// <summary>
    /// Where the visible child of a <see cref="Zbox"/> sits within the box, when the
    /// box is larger than the child.
    /// </summary>
    /// <remarks>
    /// Compass points rather than the usual left/right/top/bottom pair, because IUP
    /// spells them that way for this control.
    /// </remarks>
    public enum ZboxAlignment
    {
        /// <summary>Top left. This is the default.</summary>
        NorthWest,
        /// <summary>Top centre.</summary>
        North,
        /// <summary>Top right.</summary>
        NorthEast,
        /// <summary>Left, vertically centred.</summary>
        West,
        /// <summary>Centred both ways.</summary>
        Center,
        /// <summary>Right, vertically centred.</summary>
        East,
        /// <summary>Bottom left.</summary>
        SouthWest,
        /// <summary>Bottom centre.</summary>
        South,
        /// <summary>Bottom right.</summary>
        SouthEast
    }

    /// <summary>
    /// A void container holding several children stacked on top of one another, with
    /// exactly one visible at a time.
    /// </summary>
    /// <remarks>
    /// <para>Useful for wizard pages, mode switching, or anywhere a region of the
    /// dialog should show one of several alternative layouts. Unlike
    /// <see cref="Tabs"/> there is no interactive way to change the visible child -
    /// the application chooses it.</para>
    ///
    /// <para><b>Changing the visible child overrides the children's own Visible
    /// state.</b> The chosen child is made visible and every other child is made
    /// invisible, whatever they were set to before.</para>
    ///
    /// <para>The natural size is the smallest that fits the largest child, so the box
    /// does not resize as the visible child changes. Set
    /// <see cref="ChildSizeAll"/> to false to size it from the visible child
    /// only.</para>
    ///
    /// <example>
    /// <code>
    /// var pages = new Zbox(welcomePage, optionsPage, summaryPage);
    /// pages.SelectedIndex = 0;
    ///
    /// nextButton.Action = d =&gt; pages.SelectedIndex++;
    /// </code>
    /// </example>
    /// </remarks>
    public class Zbox : ContainerControl, IEnumerable<Control>
    {
        private readonly System.Collections.Generic.List<Control> _children = new();

        /// <summary>
        /// Creates a new zbox holding the given children. The first one is visible to
        /// begin with. It can also be created empty and filled later with Append.
        /// </summary>
        public Zbox(params Control[] children)
            : base(NativeIup.IupZboxv(new nint[] { IntPtr.Zero }))
        {
            if (children == null)
                return;

            foreach (Control child in children)
                Append(child);
        }

        #region CHILDREN

        /// <summary>
        /// Adds a child at the end. The first child added becomes the visible one.
        /// </summary>
        public override void Append(Control child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));

            base.Append(child);
            _children.Add(child);
        }

        /// <summary>Gets the number of children.</summary>
        public int Count => _children.Count;

        /// <summary>Gets the child at the given zero based position.</summary>
        public Control this[int index] => _children[index];

        public IEnumerator<Control> GetEnumerator() => _children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _children.GetEnumerator();

        #endregion

        #region VISIBLE CHILD

        /// <summary>
        /// Gets or sets the visible child by its zero based position. The first child
        /// added is visible when the box is created.
        /// (non inheritable) (since 3.0)
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                string v = GetAttribute("VALUEPOS");
                return int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out int i)
                    ? i
                    : 0;
            }
            set => SetAttribute("VALUEPOS", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets or sets the visible child by element.
        /// (non inheritable) (since 3.0)
        /// </summary>
        /// <remarks>
        /// Returns null if the visible child was not created through IupSharp. Setting
        /// it requires the element to be one of this box's children.
        /// </remarks>
        public virtual Control SelectedChild
        {
            get
            {
                CheckAlive();
                return Find<Control>(NativeIup.GetAttributePtr(Handle, "VALUE_HANDLE"));
            }
            set
            {
                CheckAlive();
                NativeIup.SetAttributeHandle(Handle, "VALUE_HANDLE",
                    value == null ? IntPtr.Zero : value.Handle);
            }
        }

        #endregion

        #region APPEARANCE

        static readonly (string, ZboxAlignment)[] _alignments = new[]
        {
            ("NW", ZboxAlignment.NorthWest),
            ("N", ZboxAlignment.North),
            ("NE", ZboxAlignment.NorthEast),
            ("W", ZboxAlignment.West),
            ("ACENTER", ZboxAlignment.Center),
            ("E", ZboxAlignment.East),
            ("SW", ZboxAlignment.SouthWest),
            ("S", ZboxAlignment.South),
            ("SE", ZboxAlignment.SouthEast)
        };

        /// <summary>
        /// Gets or sets where the visible child sits when the box is larger than it.
        /// Default: NorthWest.
        /// (non inheritable)
        /// </summary>
        public virtual ZboxAlignment Alignment
        {
            get => Utils.MapAttrib(GetAttribute("ALIGNMENT"), _alignments);
            set => SetAttribute("ALIGNMENT", Utils.MapEnum(value, _alignments));
        }

        /// <summary>
        /// Gets or sets whether the natural size is computed from every child rather
        /// than only the visible one. Default: true, so the box does not change size
        /// as the visible child changes.
        /// (non inheritable) (since 3.27)
        /// </summary>
        public virtual bool ChildSizeAll
        {
            get => GetAttribute("CHILDSIZEALL") != "NO";
            set => SetAttribute("CHILDSIZEALL", value ? "YES" : "NO");
        }

        #endregion

        protected override void OnDestroying()
        {
            _children.Clear();
            base.OnDestroying();
        }
    }
}