namespace IupSharp
{
    public class ContainerControl:Control
    {
        public ContainerControl(nint handle) : base(handle)
        {
        }

        public virtual void Append(Control child)
        {
            CheckAlive();

            if (child != null)
                IupNative.IupAppend(Handle, child.Handle);
        }
    }
}
