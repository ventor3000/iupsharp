using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IupSharp
{
    /// <summary>
    /// Creates an empty element that expands to take up the spare room in a box,
    /// pushing its siblings apart or to one end.
    /// </summary>
    /// <remarks>
    /// <para>It has no visual representation at all. The parent must be an HBox, VBox
    /// or GridBox, or the expansion does nothing.</para>
    ///
    /// <para><b>If any other child of the box has Expand set, the fill is
    /// ignored.</b> A fill only claims room that nothing else wants.</para>
    ///
    /// <para>Giving it a Size turns it into a fixed spacer instead: with a user size
    /// set, its Expand becomes No.</para>
    ///
    /// <example>
    /// Push a pair of buttons to the right-hand end of a row:
    /// <code>
    /// var buttons = new HBox(new Fill(), okButton, cancelButton);
    /// </code>
    /// Centre a label by putting a fill on each side:
    /// <code>
    /// var centred = new HBox(new Fill(), new Label("Centred"), new Fill());
    /// </code>
    /// A fixed 20 pixel gap:
    /// <code>
    /// var spacer = new Fill(20);
    /// </code>
    /// </example>
    /// </remarks>
    public class Fill : Control
    {
        /// <summary>
        /// Creates a new fill that expands to occupy the spare room in its box.
        /// </summary>
        public Fill() : base(NativeIup.IupFill())
        {
        }

        /// <summary>
        /// Creates a new fill of a fixed size, acting as a spacer rather than a
        /// spring.
        /// </summary>
        /// <param name="size">
        /// The width in pixels when inside an HBox, or the height when inside a VBox.
        /// </param>
        public Fill(int size) : this()
        {
            FillSize = size;
        }

        /// <summary>
        /// Gets or sets the fill's fixed size in pixels: its width inside an HBox, its
        /// height inside a VBox. Setting this stops the fill expanding, since a fill
        /// with a user size has Expand of No.
        /// (non inheritable)
        /// </summary>
        /// <remarks>
        /// Writes RASTERSIZE, which for a fill takes a single number rather than the
        /// usual "wxh". The "wxh" form is also accepted, but the irrelevant dimension
        /// is ignored, so a single value states the intent more clearly.
        /// </remarks>
        public int FillSize
        {
            get
            {
                CheckAlive();
                NativeIup.GetIntInt(Handle, "RASTERSIZE", out int w, out int h);
                return w != 0 ? w : h;
            }
            set => SetAttribute("RASTERSIZE",
                value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets how the fill expands, which IUP decides from its surroundings:
        /// Horizontal inside an HBox or GridBox, Vertical inside a VBox, and No once a
        /// user size has been set.
        /// (read only) (non inheritable)
        /// </summary>
        /// <remarks>
        /// Read only on a fill, unlike other controls. Assigning it throws, because
        /// IUP computes the value and would silently discard anything set here - use
        /// <see cref="FillSize"/> to stop a fill expanding.
        /// </remarks>
        /// <exception cref="InvalidOperationException">On any attempt to set it.</exception>
        public override Expand Expand
        {
            get => base.Expand;
            set => throw new InvalidOperationException(
                "Expand is read-only on a Fill; IUP computes it from the parent box. " +
                "Set FillSize to make the fill a fixed spacer instead.");
        }
    }
}
