using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace IupSharp
{

    public enum ToggleValue
    {
        On,
        Off,
        NotDef // only in 3-state mode
    }
    public class Toggle:Control
    {
        public Toggle(string title):base(IupNative.Toggle(title, null))
        {
            
        }


        static (string, ToggleValue)[] toggleValues = new[] { ("OFF", ToggleValue.Off), ("ON", ToggleValue.On),("NOTDEF", ToggleValue.NotDef) };
        public ToggleValue Value
        {
            get
            {
                return Utils.MapAttrib(GetAttribute("VALUE"), toggleValues);
            }

            set
            {
                SetAttribute("VALUE", Utils.MapEnum(value, toggleValues));
            }
        }

        public bool Checked
        {
            get => Value == ToggleValue.On;
            set => Value = (value ? ToggleValue.On : ToggleValue.Off);
        }



        #region CALLBACKS

        private ToggleActionCallback _action; // users callback function for ButtonCB
        private IFni _actionInternal; // need reference to keep alive in GC
        public ToggleActionCallback Action
        {
            get => _action;
            set
            {
                _action = value;
                _actionInternal = ActionInternal;
                SetCallback( "ACTION", Utils.CastCallback<Icallback>(_actionInternal));
            }
        }

        private int ActionInternal(nint ih,int status)
        {
            try
            {
                var cb = new ToggleActionData(this, status);
                _action?.Invoke(cb);
                return cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in Action callback: {ex}");
                return IupNative.IUP_DEFAULT;
            }

        }

        private Callback _valueChangedCB; // users callback function for ButtonCB
        private IFn _valueChangedCBInternal; // need reference to keep alive in GC
        public Callback ValueChangedCB
        {
            get => _valueChangedCB;
            set
            {
                _valueChangedCB = value;
                _valueChangedCBInternal = ValueChangedCBInternal;
                SetCallback( "VALUECHANGED_CB", Utils.CastCallback<Icallback>(_valueChangedCBInternal));
            }
        }

        private int ValueChangedCBInternal(nint ih)
        {
            try
            {
                var cb = new CallbackData(this);
                _valueChangedCB?.Invoke(cb);
                return cb.Result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IupSharp] unhandled exception in ValueChangedCB callback: {ex}");
                return IupNative.IUP_DEFAULT;
            }
}

        #endregion
    }
}
