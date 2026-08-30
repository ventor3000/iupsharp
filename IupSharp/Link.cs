using System;
using System.Drawing;

namespace IupSharp
{
    /// <summary>
    /// Creates a label showing underlined, clickable text. (since 3.8)
    /// </summary>
    /// <remarks>
    /// <para>This derives from Label because IUP does: a link inherits every Label
    /// attribute and callback, redefining only FgColor and adding Url. Alignment,
    /// Image, Padding, WordWrap and the rest all work as they do on a plain
    /// Label.</para>
    ///
    /// <para><b>Do not set ButtonCB, EnterWindowCB or LeaveWindowCB on a link.</b> IUP
    /// uses all three internally to implement the click and hover behaviour, so
    /// assigning them replaces that machinery and the link stops working. This is
    /// the one part of the Label surface that is unsafe here.</para>
    ///
    /// <para>If Action is not set, activating the link calls IupHelp with the Url,
    /// which opens it in the system browser. Set Action to handle it yourself; the
    /// default browser behaviour is then suppressed unless the callback data's Result
    /// is left at Default.</para>
    ///
    /// <para>The cursor changes to a hand while it is over the text.</para>
    ///
    /// <example>
    /// <code>
    /// // Opens in the system browser, no code needed:
    /// var site = new Link("https://www.tecgraf.puc-rio.br/iup/", "IUP home page");
    ///
    /// // Handled by the application instead:
    /// var custom = new Link("about:version", "About");
    /// custom.Action = d =&gt;
    /// {
    ///     ShowAboutDialog();
    ///     d.Result = CallbackResult.Close;   // suppress the IupHelp fallback
    /// };
    /// </code>
    /// </example>
    /// </remarks>
    public class Link : Label
    {
        /// <summary>
        /// Creates a new link.
        /// </summary>
        /// <param name="url">
        /// The destination address. It can be any text, but should be a valid URL if
        /// you rely on the default IupHelp behaviour. It can be null.
        /// </param>
        /// <param name="title">
        /// The text shown to the user. It can be null, in which case the link shows
        /// nothing until Title is set.
        /// </param>
        public Link(string url = null, string title = null)
            : base(IupNative.Link(url, title))
        {
        }

        /// <summary>
        /// Gets or sets the destination address. It can be any text; the default
        /// IupHelp behaviour expects a valid URL.
        /// </summary>
        public string Url
        {
            get => GetAttribute("URL");
            set => SetAttribute("URL", value);
        }

        /// <summary>
        /// Gets or sets the text colour. Default: the global attribute LinkFgColor
        /// rather than the usual label colour.
        /// </summary>
        public override Color FgColor { get => base.FgColor; set => base.FgColor = value; }

        #region CALLBACKS

        private LinkActionCallback _action;
        private IFns _actionInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated when the link is activated. The callback
        /// data carries the Url.
        /// </summary>
        /// <remarks>
        /// Leaving the callback data's Result at Default makes IUP call IupHelp with
        /// the Url afterwards, opening it in the system browser. Set the Result to
        /// Close to end the main loop instead. To handle the click entirely in the
        /// application without opening a browser, do the work and leave Result at
        /// Default only if the browser is also wanted.
        /// </remarks>
        public LinkActionCallback Action
        {
            get => _action;
            set
            {
                _action = value;
                _actionInternal = ActionInternal;
                SetCallback("ACTION", Utils.CastCallback<Icallback>(_actionInternal));
            }
        }
        private int ActionInternal(nint ih, string url)
        {
            try
            {
                var cb = new LinkActionData(this, url);
                _action?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[IupSharp] unhandled exception in Link Action callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        #endregion
    }
}