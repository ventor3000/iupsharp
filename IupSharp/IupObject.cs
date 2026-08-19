using System;
using System.Collections.Generic;

namespace IupSharp
{
    public class IupObject:IDisposable
    {

        private static readonly Dictionary<nint, IupObject> _live = new();
        public nint Handle { get; private set; }
        public IupObject(nint handle)
        {

            if (handle == IntPtr.Zero) throw new IupException("Cannot create a null handle IUP object");
            Handle = handle;
            _live[handle] = this;                 // strong: keeps the LDESTROY thunk alive

            LDestroyCB = (data) =>
            {
                OnDestroying(); // notify derived classes that the object is being destroyed
                _live.Remove(Handle);                     // now collectable
                Handle = IntPtr.Zero;
            };
            
        }

        protected virtual void OnDestroying()
        {
            
        }

        private bool _disposed;

        /// <summary>
        /// Destroys the underlying native element, equivalent to calling Destroy().
        /// Safe to call more than once. Deliberately does NOT expose a finalizer:
        /// IUP is single-threaded and IupDestroy may only be called from the thread
        /// that called Iup.Open (see IupNative.CheckThread). A finalizer runs on the
        /// GC thread, so it can never call IupDestroy safely - attempting to do so
        /// previously caused exactly this bug. If Dispose is never called, the
        /// native element is only reclaimed by an explicit Destroy() call, by its
        /// parent being destroyed, or by Iup.Close(), exactly as today.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            Destroy();
            GC.SuppressFinalize(this); // no-op today, but future-proofs against a subclass ever adding a finalizer
        }

        public void SetAttribute(string name, string value)
        {
            CheckAlive();
            IupNative.StoreAttribute(Handle, name, value);
        }

        public void SetAttributeId(string name, int id, string value)
        {
            CheckAlive();
            IupNative.StoreAttributeId(Handle, name, id, value);
        }

        public void SetAttributeId2(string name, int lin, int col, string value)
        {
            CheckAlive();
            IupNative.StoreAttributeId2(Handle, name, lin, col, value);
        }

        public string GetAttribute(string name)
        {
            CheckAlive();
            return IupNative.GetAttribute(Handle, name);
        }

        /// <summary>
        /// Gets an attribute's raw pointer value without interpreting it as a
        /// UTF-8 string. Needed for attributes documented as returning an
        /// opaque pointer, such as WID on image elements.
        /// </summary>
        protected IntPtr GetAttributePtr(string name)
        {
            CheckAlive();
            return IupNative.GetAttributePtr(Handle, name);
        }

        public string GetAttributeId(string name, int id)
        {
            CheckAlive();
            return IupNative.GetAttributeId(Handle, name, id);
        }
        public string GetAttributeId2(string name, int lin, int col)
        {
            CheckAlive();
            return IupNative.GetAttributeId2(Handle, name, lin, col);
        }

        

        protected void CheckAlive()
        {
            if (Handle == IntPtr.Zero) throw new ObjectDisposedException(GetType().Name);
        }


        public void Destroy()
        {
            if (Handle != IntPtr.Zero)
                IupNative.IupDestroy(Handle);
        }

        protected void SetCallback(string cbname, Icallback cb)
        {
            CheckAlive();
            IupNative.SetCallback(Handle, cbname, cb);
        }

        protected Icallback GetCallback(string name, Icallback func) => IupNative.GetCallback(Handle, name);


        #region CALLBACKS

        private Callback _destroyCB; // users callback function for ButtonCB
        private IFn _destroyCBInternal; // need reference to keep alive in GC
        public Callback DestroyCB
        {
            get => _destroyCB;
            set
            {
                _destroyCB = value;
                _destroyCBInternal = DestroyCBInternal;
                SetCallback( "DESTROY_CB", Utils.CastCallback<Icallback>(_destroyCBInternal));
            }
        }
        private int DestroyCBInternal(nint ih)
        {
            try
            {
                var cb = new CallbackData(this);
                _destroyCB?.Invoke(cb);
                return cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in DestroyCB callback: {ex}");
                return IupNative.IUP_DEFAULT;
            }

        }

        private Callback _ldestroyCB; // users callback function for 
        private IFn _ldestroyCBInternal; // need reference to keep alive in GC
        

        private Callback LDestroyCB // note only used for internal object management, not usable for end application
        {
            get => _ldestroyCB;
            set
            {
                _ldestroyCB = value;
                _ldestroyCBInternal = LDestroyCBInternal;
                SetCallback( "LDESTROY_CB", Utils.CastCallback<Icallback>(_ldestroyCBInternal));
            }
        }
        private int LDestroyCBInternal(nint ih)
        {
            try
            {
                var cb = new CallbackData(this);
                _ldestroyCB?.Invoke(cb);
                return cb.Result;
            }
            catch (Exception ex)
            {
                // drop ourselves in this specific case:
                _live.Remove(Handle);
                Handle = IntPtr.Zero;

                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in LDestroyCB callback: {ex}");
                return IupNative.IUP_DEFAULT;
            }

        }

        
        #endregion



        /// <summary>
        /// only to be called when IUP is closed to drop obejct tracking.
        /// </summary>
        internal static void InvalidateAll()
        {
            foreach (var kv in _live)
            {
                kv.Value.Handle = IntPtr.Zero;
            }
            _live.Clear();
        }

    }
}
