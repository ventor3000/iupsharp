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
                return cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in MapCB callback: {ex}");
                return IupNative.IUP_DEFAULT;
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
                return cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in UnmapCB callback: {ex}");
                return IupNative.IUP_DEFAULT;
            }
        }


        #endregion
    }
}
