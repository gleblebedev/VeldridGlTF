#version 450

layout (set=0, binding=0) uniform ObjectProperties
{
	mat4 ModelMatrix;
	mat4 NormalMatrix;
	float MorphWeights[5];
};

void main()
{
}