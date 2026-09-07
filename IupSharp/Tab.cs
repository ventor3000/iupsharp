using System;

namespace IupSharp
{
    /// <summary>
    /// A tab title and image paired with the control that fills the tab, so a whole
    /// notebook can be written as a single expression.
    /// </summary>
    /// <remarks>
    /// <para>This is a description used at construction time, not a live object. Once
    /// the tab has been added, changing a Tab has no effect - use
    /// <c>tabs.TabItems[n]</c> to change a title or image afterwards.</para>
    ///
    /// <para>That restriction comes from IUP: the child-side TABTITLE and TABIMAGE
    /// attributes are read when the child joins the container and ignored from then
    /// on.</para>
    ///
    /// <example>
    /// <code>
    /// var d = new Dialog(
    ///     new Tabs(
    ///         new Tab("&amp;General",  new VBox(nameField, emailField)),
    ///         new Tab("&amp;Advanced", new VBox(logLevel, cachePath))))
    /// {
    ///     Title = "Preferences"
    /// };
    /// </code>
    /// With a stock icon on each tab:
    /// <code>
    /// ImageLib.Open();
    ///
    /// var tabs = new Tabs(
    ///     new Tab("&amp;Open", openPage) { ImageName = ImageLib.FileOpen },
    ///     new Tab("&amp;Save", savePage) { ImageName = ImageLib.FileSave });
    /// </code>
    /// </example>
    /// </remarks>
    public sealed class Tab
    {
        /// <summary>
        /// Creates a tab description.
        /// </summary>
        /// <param name="title">
        /// The text shown on the tab button. The "&amp;" character defines a mnemonic,
        /// activated with Alt from anywhere in the dialog; use "&amp;&amp;" for a
        /// literal "&amp;". It can be null for an untitled tab.
        /// </param>
        /// <param name="child">The control that fills the tab.</param>
        /// <exception cref="ArgumentNullException">The child is null.</exception>
        public Tab(string title, Control child)
        {
            Title = title;
            Child = child ?? throw new ArgumentNullException(nameof(child));
        }

        /// <summary>
        /// Creates an untitled tab description. A title can be set through the Title
        /// property before the tab is added.
        /// </summary>
        /// <exception cref="ArgumentNullException">The child is null.</exception>
        public Tab(Control child) : this(null, child)
        {
        }

        /// <summary>The control that fills the tab.</summary>
        public Control Child { get; }

        /// <summary>
        /// The text shown on the tab button, or null for an untitled tab. Changing
        /// this after the tab has been added has no effect.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// An image for the tab button, by name: a stock image name (after
        /// ImageLib.Open), a name registered with IupSetHandle, a system resource
        /// name, or a path to an image file.
        /// </summary>
        /// <remarks>
        /// If both this and <see cref="Image"/> are set, Image wins - it is applied
        /// after the child is added and so overwrites the name.
        /// </remarks>
        public string ImageName { get; set; }

        /// <summary>
        /// An image for the tab button, as an object rather than a name.
        /// </summary>
        /// <remarks>
        /// <para>The container does not keep a reference to it, so the caller must
        /// keep the Image alive for as long as the tab uses it - the same caveat that
        /// applies to <c>TabItems[n].SetImage</c>.</para>
        ///
        /// <para>On Motif a tab image is shown only when the title is null. On Windows
        /// and Motif, set the container's BgColor before the image.</para>
        /// </remarks>
        public Image Image { get; set; }
    }
}