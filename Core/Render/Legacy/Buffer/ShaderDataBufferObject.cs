using Helion.Render.Legacy.Context;
using Helion.Render.Legacy.Util;
using static Helion.Util.Assertion.Assert;

namespace Helion.Render.Legacy.Buffer
{
    public abstract class ShaderDataBufferObject<T> : BufferObject<T> where T : struct
    {
        protected int BindIndex;
        
        protected ShaderDataBufferObject(GLCapabilities capabilities, IGLFunctions functions, BindingPoint bindPoint, string objectLabel = "") : 
            this(capabilities, functions, (int)bindPoint, objectLabel)
        {
        }
        
        protected ShaderDataBufferObject(GLCapabilities capabilities, IGLFunctions functions, int bindPoint, string objectLabel = "") : 
            base(capabilities, functions, objectLabel)
        {
            Precondition(bindPoint >= 0, "Cannot have a negative shader binding point");
            
            BindIndex = bindPoint;
        }
    }
}