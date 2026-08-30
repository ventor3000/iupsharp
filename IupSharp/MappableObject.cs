using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IupSharp
{
    public class MappableObject : IupObject
    {
        public MappableObject(nint handle) : base(handle)
        {
        }

        /// <summary>
        /// Gets whether the element has a native representation yet. Non-native
        /// containers such as VBox and HBox report a fake WID of -1 when mapped, so
        /// this is true for them too.
        /// </summary>
        public bool IsMapped => GetAttribute("WID") != null;

        /// <summary>
        /// Creates the native element and, recursively, those of its children. Called
        /// automatically before a dialog is shown, so this is only needed when an
        /// attribute must be read before the dialog is displayed, or when adding
        /// elements to an already mapped container.
        /// </summary>
        /// <remarks>
        /// The element must already be attached to a mapped container, except for a
        /// dialog. Mapping an already mapped non-dialog element does nothing. After
        /// adding elements to a live dialog, call Refresh on it to update the layout.
        /// </remarks>
        /// <exception cref="IupException">The native creation failed.</exception>
        public void Map()
        {
            CheckAlive();
            if (IupNative.IupMap(Handle) != IupNative.IUP_NOERROR)
                throw new IupException("Failed to map the element.");
        }

        /// <summary>
        /// Destroys the native element but keeps the IUP element. Its attributes are
        /// saved first, so a later Map restores them.
        /// </summary>
        public void Unmap()
        {
            CheckAlive();
            IupNative.IupUnmap(Handle);
        }

        #region CALLBACKS

        private Callback _mapCB;
        private IFn _mapCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated right after the element is mapped and its
        /// attributes updated.
        /// </summary>
        public Callback MapCB
        {
            get => _mapCB;
            set
            {
                _mapCB = value;
                _mapCBInternal = MapCBInternal;
                SetCallback("MAP_CB", Utils.CastCallback<Icallback>(_mapCBInternal));
            }
        }
        private int MapCBInternal(nint ih)
        {
            try
            {
                var cb = new CallbackData(this);
                _mapCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in MapCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }

        private Callback _unmapCB;
        private IFn _unmapCBInternal; // need reference to keep alive in GC
        /// <summary>
        /// Gets or sets the action generated right before the element is unmapped.
        /// </summary>
        public Callback UnmapCB
        {
            get => _unmapCB;
            set
            {
                _unmapCB = value;
                _unmapCBInternal = UnmapCBInternal;
                SetCallback("UNMAP_CB", Utils.CastCallback<Icallback>(_unmapCBInternal));
            }
        }
        private int UnmapCBInternal(nint ih)
        {
            try
            {
                var cb = new CallbackData(this);
                _unmapCB?.Invoke(cb);
                return (int)cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in UnmapCB callback: {ex}");
                return (int)CallbackResult.Default;
            }
        }


        #endregion

        #region IMAGE ATTRIBUTE HELPERS

        /// <summary>
        /// Sets an image attribute from an Image object, keeping the managed reference
        /// so the image stays reachable, and clearing any name previously set for the
        /// same attribute.
        /// </summary>
        /// <remarks>
        /// An image attribute holds one value, so the object form and the name form
        /// are mutually exclusive. These two helpers keep the cached fields consistent
        /// with whichever was assigned last.
        /// </remarks>
        protected void SetImageHandle(string attribute, Image image,
                                      ref Image imageField, ref string nameField)
        {
            CheckAlive();

            imageField = image;
            nameField = null;

            IupNative.SetAttributeHandle(Handle, attribute,
                image == null ? IntPtr.Zero : image.Handle);
        }

        /// <summary>
        /// Sets an image attribute from a name - a stock image, a name registered with
        /// IupSetHandle, a system resource name, or a path to an image file - and
        /// clears any Image object previously set for the same attribute.
        /// </summary>
        protected void SetImageName(string attribute, string imageName,
                                    ref Image imageField, ref string nameField)
        {
            CheckAlive();

            nameField = imageName;
            imageField = null;

            // SetAttribute copies the string, which IUP needs since it keeps the name.
            SetAttribute(attribute, imageName);
        }

        #endregion
    }
}