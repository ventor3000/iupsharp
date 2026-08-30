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
                NativeIup.IupAppend(Handle, child.Handle);
        }
    }
}
