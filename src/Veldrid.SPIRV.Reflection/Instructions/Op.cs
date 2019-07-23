namespace Veldrid.SPIRV.Instructions
{
    internal enum Op
    {
        OpName = 5,
        OpMemberName = 6,

        OpTypeVoid = 19,
        OpTypeBool = 20,
        OpTypeInt = 21,
        OpTypeFloat = 22,
        OpTypeVector = 23,
        OpTypeMatrix = 24,
        OpTypeImage = 25,
        OpTypeSampler = 26,
        OpTypeSampledImage = 27,
        OpTypeArray = 28,
        OpTypeRuntimeArray = 29,
        OpTypeStruct = 30,
        OpTypeOpaque = 31,
        OpTypePointer = 32,
        OpTypeFunction = 33,
        OpTypeEvent = 34,
        OpTypeDeviceEvent = 35,
        OpTypeReserveId = 36,
        OpTypeQueue = 37,
        OpTypePipe = 38,
        OpTypeForwardPointer = 39,

        OpVariable = 59,

        OpDecorate = 71,
        OpMemberDecorate = 72,
        OpDecorationGroup = 73,
        OpGroupDecorate = 74,
        OpGroupMemberDecorate = 75
    }
}