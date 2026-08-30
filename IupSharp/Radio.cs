using System;

namespace IupSharp
{
    /// <summary>
    /// A void container that groups toggles so that only one of them is selected at
    /// a time.
    /// </summary>
    /// <remarks>
    /// <para>It has no visual representation of its own and does not lay anything
    /// out. Its single child is normally a VBox or HBox holding the toggles; the
    /// toggles may sit anywhere in the subtree below it, at any depth.</para>
    ///
    /// <para>Every Toggle under this container joins the exclusive group unless it
    /// sets <c>IgnoreRadio</c>. Once a toggle is selected, selecting another clears
    /// the first automatically.</para>
    ///
    /// <para><b>The first toggle in the group is selected automatically when the
    /// dialog is mapped</b>, so a radio group is never left with nothing
    /// selected.</para>
    ///
    /// <example>
    /// <code>
    /// var portrait  = new Toggle("Portrait");
    /// var landscape = new Toggle("Landscape");
    ///
    /// var mode = new Radio(new VBox(portrait, landscape));
    /// mode.SelectedToggle = landscape;
    ///
    /// // later
    /// if (mode.SelectedToggle == portrait) { ... }
    /// </code>
    /// </example>
    /// </remarks>
    public class Radio : ContainerControl
    {
        private Control _child;

        /// <summary>
        /// Creates a new radio group around the given element.
        /// </summary>
        /// <param name="child">
        /// The element holding the toggles, normally a VBox or HBox. It can be null,
        /// in which case a child can be added later with Append.
        /// </param>
        public Radio(Control child)
            : base(NativeIup.IupRadio(child == null ? IntPtr.Zero : child.Handle))
        {
            _child = child;
        }

        /// <summary>
        /// Creates a new empty radio group. A child can be added later with Append.
        /// </summary>
        public Radio() : this(null)
        {
        }

        /// <summary>
        /// Gets the single child of the radio group, or null if it has none.
        /// </summary>
        public Control Child => _child;

        /// <summary>
        /// Sets the single child of the radio group. A radio accepts only one child;
        /// put several toggles in a VBox or HBox and pass that.
        /// </summary>
        /// <exception cref="InvalidOperationException">The radio already has a child.</exception>
        public override void Append(Control child)
        {
            if (_child != null)
                throw new InvalidOperationException(
                    "A Radio accepts only one child. Wrap the toggles in a VBox or HBox.");

            base.Append(child);
            _child = child;
        }

        #region SELECTION

        /// <summary>
        /// Gets or sets the selected toggle.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// <para>Returns null before the toggles have been mapped, since IUP does not
        /// track the selection until then. It also returns null if the selected
        /// element was not created through IupSharp, or is not a Toggle - a
        /// FlatToggle, for instance, participates in the group but is a different
        /// type.</para>
        ///
        /// <para>Setting this selects the given toggle and clears whichever was
        /// selected before. The toggle must be a descendant of this radio group;
        /// IUP does not check that, and selecting an unrelated toggle simply has no
        /// visible effect.</para>
        /// </remarks>
        public virtual Toggle SelectedToggle
        {
            get
            {
                CheckAlive();
                return Find<Toggle>(NativeIup.GetAttributePtr(Handle, "VALUE_HANDLE"));


            }
            set
            {
                CheckAlive();
                NativeIup.SetAttributeHandle(Handle, "VALUE_HANDLE",
                    value == null ? IntPtr.Zero : value.Handle);
            }
        }

        /// <summary>
        /// Gets or sets the selected toggle by the name it was given with
        /// IupSetHandle, rather than by object.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// <para>Prefer <see cref="SelectedToggle"/>, which needs no name
        /// registration. This is here for elements named through IupSetHandle, and for
        /// the toggles inside a radio that IUP names automatically when they are
        /// mapped.</para>
        ///
        /// <para>Reading this before the toggles are mapped may return null or a stale
        /// value, which is IUP's behaviour rather than the wrapper's.</para>
        /// </remarks>
        public virtual string SelectedName
        {
            get => GetAttribute("VALUE");
            set => SetAttribute("VALUE", value);
        }

        #endregion

        protected override void OnDestroying()
        {
            _child = null;
            base.OnDestroying();
        }
    }
}