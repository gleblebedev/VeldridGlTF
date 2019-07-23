namespace Veldrid.SPIRV.Instructions
{
    internal enum StorageClass
    {
        UniformConstant = 0,
        Input = 1,
        Uniform = 2,
        Output = 3,
        Workgroup = 4,
        CrossWorkgroup = 5,
        Private = 6,
        Function = 7,
        Generic = 8,
        PushConstant = 9,
        AtomicCounter = 10,
        Image = 11,
        StorageBuffer = 12
    }
}