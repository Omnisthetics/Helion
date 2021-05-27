using Helion.Render.OpenGL.Legacy.Context.Types;

namespace Helion.Render.OpenGL.Legacy.Vertex.Attribute
{
    public class VertexPointerUnsignedByteAttribute : VertexPointerAttribute
    {
        public VertexPointerUnsignedByteAttribute(string name, int index, int size, bool normalized = false) : 
            base(name, index, size, normalized)
        {
        }

        public override int ByteLength() => 1 * Size;

        protected override VertexAttributePointerType GetAttributePointerType()
        {
            return VertexAttributePointerType.UnsignedByte;
        } 
    }
}