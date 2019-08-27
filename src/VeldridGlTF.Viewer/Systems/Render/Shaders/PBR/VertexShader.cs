﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using VeldridGlTF.Viewer.Systems.Render.Uniforms;

namespace VeldridGlTF.Viewer.Systems.Render.Shaders.PBR
{
    /// <summary>
    ///     Class to produce the template output
    /// </summary>
#line 1 "E:\MyWork\VeldridGlTF\src\VeldridGlTF.Viewer\Systems\Render\Shaders\PBR\VertexShader.tt"
    [GeneratedCode("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class VertexShader : VertexShaderBase
    {
        /// <summary>
        ///     Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            Write("#version 450\r\n\r\n");

#line 9 "E:\MyWork\VeldridGlTF\src\VeldridGlTF.Viewer\Systems\Render\Shaders\PBR\VertexShader.tt"

            WriteDefines();


#line default
#line hidden
            Write("\r\n");

#line 13 "E:\MyWork\VeldridGlTF\src\VeldridGlTF.Viewer\Systems\Render\Shaders\PBR\VertexShader.tt"

            for (var location = 0; location < Context.VertexElements.Count; ++location)
                WriteLine(string.Format("layout(location = {0}) in {1} {2};", location,
                    Glsl.NameOf(Context.VertexElements[location].Format), Context.VertexElements[location].Name));


#line default
#line hidden

#line 19 "E:\MyWork\VeldridGlTF\src\VeldridGlTF.Viewer\Systems\Render\Shaders\PBR\VertexShader.tt"

            for (var location = 0; location < Context.Varyings.Count; ++location)
                WriteLine(string.Format("layout(location = {0}) out {1} {2};", Context.Varyings[location].Location,
                    Glsl.NameOf(Context.Varyings[location].Format), Context.Varyings[location].Name));


#line default
#line hidden
            Write("\r\nlayout(set = 0, binding = 0) uniform EnvironmentProperties\r\n{\r\n");

#line 28 "E:\MyWork\VeldridGlTF\src\VeldridGlTF.Viewer\Systems\Render\Shaders\PBR\VertexShader.tt"

            WriteMembers<EnvironmentProperties>();


#line default
#line hidden
            Write("};\r\nlayout(set = 2, binding = 0) uniform ObjectProperties\r\n{\r\n");

#line 34 "E:\MyWork\VeldridGlTF\src\VeldridGlTF.Viewer\Systems\Render\Shaders\PBR\VertexShader.tt"

            WriteMembers<ObjectProperties>();


#line default
#line hidden
            Write("};\r\n\r\n#ifdef USE_SKINNING\r\nuniform mat4 u_jointMatrix[JOINT_COUNT];\r\nuniform mat4" +
                  " u_jointNormalMatrix[JOINT_COUNT];\r\n#endif\r\n\r\n#ifdef USE_SKINNING\r\nmat4 getSkinn" +
                  "ingMatrix()\r\n{\r\n    mat4 skin = mat4(0);\r\n\r\n    #if defined(HAS_WEIGHT_SET1) && " +
                  "defined(HAS_JOINT_SET1)\r\n    skin +=\r\n        a_Weight1.x * u_jointMatrix[int(a_" +
                  "Joint1.x)] +\r\n        a_Weight1.y * u_jointMatrix[int(a_Joint1.y)] +\r\n        a_" +
                  "Weight1.z * u_jointMatrix[int(a_Joint1.z)] +\r\n        a_Weight1.w * u_jointMatri" +
                  "x[int(a_Joint1.w)];\r\n    #endif\r\n\r\n    #if defined(HAS_WEIGHT_SET2) && defined(H" +
                  "AS_JOINT_SET2)\r\n    skin +=\r\n        a_Weight2.x * u_jointMatrix[int(a_Joint2.x)" +
                  "] +\r\n        a_Weight2.y * u_jointMatrix[int(a_Joint2.y)] +\r\n        a_Weight2.z" +
                  " * u_jointMatrix[int(a_Joint2.z)] +\r\n        a_Weight2.w * u_jointMatrix[int(a_J" +
                  "oint2.w)];\r\n    #endif\r\n\r\n    return skin;\r\n}\r\n\r\nmat4 getSkinningNormalMatrix()\r" +
                  "\n{\r\n    mat4 skin = mat4(0);\r\n\r\n    #if defined(HAS_WEIGHT_SET1) && defined(HAS_" +
                  "JOINT_SET1)\r\n    skin +=\r\n        a_Weight1.x * u_jointNormalMatrix[int(a_Joint1" +
                  ".x)] +\r\n        a_Weight1.y * u_jointNormalMatrix[int(a_Joint1.y)] +\r\n        a_" +
                  "Weight1.z * u_jointNormalMatrix[int(a_Joint1.z)] +\r\n        a_Weight1.w * u_join" +
                  "tNormalMatrix[int(a_Joint1.w)];\r\n    #endif\r\n\r\n    #if defined(HAS_WEIGHT_SET2) " +
                  "&& defined(HAS_JOINT_SET2)\r\n    skin +=\r\n        a_Weight2.x * u_jointNormalMatr" +
                  "ix[int(a_Joint2.x)] +\r\n        a_Weight2.y * u_jointNormalMatrix[int(a_Joint2.y)" +
                  "] +\r\n        a_Weight2.z * u_jointNormalMatrix[int(a_Joint2.z)] +\r\n        a_Wei" +
                  "ght2.w * u_jointNormalMatrix[int(a_Joint2.w)];\r\n    #endif\r\n\r\n    return skin;\r\n" +
                  "}\r\n#endif // !USE_SKINNING\r\n\r\n#ifdef USE_MORPHING\r\nvec4 getTargetPosition()\r\n{\r\n" +
                  "    vec4 pos = vec4(0);\r\n\r\n#ifdef HAS_TARGET_POSITION0\r\n    pos.xyz += MorphWeig" +
                  "hts[0] * TARGET_POSITION0;\r\n#endif\r\n\r\n#ifdef HAS_TARGET_POSITION1\r\n    pos.xyz +" +
                  "= MorphWeights[1] * TARGET_POSITION1;\r\n#endif\r\n\r\n#ifdef HAS_TARGET_POSITION2\r\n  " +
                  "  pos.xyz += MorphWeights[2] * TARGET_POSITION2;\r\n#endif\r\n\r\n#ifdef HAS_TARGET_PO" +
                  "SITION3\r\n    pos.xyz += MorphWeights[3] * TARGET_POSITION3;\r\n#endif\r\n\r\n#ifdef HA" +
                  "S_TARGET_POSITION4\r\n    pos.xyz += MorphWeights[4] * TARGET_POSITION4;\r\n#endif\r\n" +
                  "\r\n    return pos;\r\n}\r\n\r\nvec4 getTargetNormal()\r\n{\r\n    vec4 normal = vec4(0);\r\n\r" +
                  "\n#ifdef HAS_TARGET_NORMAL0\r\n    normal.xyz += MorphWeights[0] * TARGET_NORMAL0;\r" +
                  "\n#endif\r\n\r\n#ifdef HAS_TARGET_NORMAL1\r\n    normal.xyz += MorphWeights[1] * TARGET" +
                  "_NORMAL1;\r\n#endif\r\n\r\n#ifdef HAS_TARGET_NORMAL2\r\n    normal.xyz += MorphWeights[2" +
                  "] * TARGET_NORMAL2;\r\n#endif\r\n\r\n#ifdef HAS_TARGET_NORMAL3\r\n    normal.xyz += Morp" +
                  "hWeights[3] * TARGET_NORMAL3;\r\n#endif\r\n\r\n#ifdef HAS_TARGET_NORMAL4\r\n    normal.x" +
                  "yz += MorphWeights[4] * TARGET_NORMAL4;\r\n#endif\r\n\r\n    return normal;\r\n}\r\n\r\nvec4" +
                  " getTargetTangent()\r\n{\r\n    vec4 tangent = vec4(0);\r\n\r\n#ifdef HAS_TARGET_TANGENT" +
                  "0\r\n    tangent.xyz += MorphWeights[0] * TARGET_TANGENT0;\r\n#endif\r\n\r\n#ifdef HAS_T" +
                  "ARGET_TANGENT1\r\n    tangent.xyz += MorphWeights[1] * TARGET_TANGENT1;\r\n#endif\r\n\r" +
                  "\n#ifdef HAS_TARGET_TANGENT2\r\n    tangent.xyz += MorphWeights[2] * TARGET_TANGENT" +
                  "2;\r\n#endif\r\n\r\n#ifdef HAS_TARGET_TANGENT3\r\n    tangent.xyz += MorphWeights[3] * T" +
                  "ARGET_TANGENT3;\r\n#endif\r\n\r\n#ifdef HAS_TARGET_TANGENT4\r\n    tangent.xyz += MorphW" +
                  "eights[4] * TARGET_TANGENT4;\r\n#endif\r\n\r\n    return tangent;\r\n}\r\n\r\n#endif // !USE" +
                  "_MORPHING\r\n\r\n\r\nvec4 getPosition()\r\n{\r\n    vec4 pos = vec4(POSITION, 1.0);\r\n\r\n#if" +
                  "def USE_MORPHING\r\n    pos += getTargetPosition();\r\n#endif\r\n\r\n#ifdef USE_SKINNING" +
                  "\r\n    pos = getSkinningMatrix() * pos;\r\n#endif\r\n\r\n    return pos;\r\n}\r\n\r\n#ifdef H" +
                  "AS_NORMALS\r\nvec4 getNormal()\r\n{\r\n    vec4 normal = vec4(NORMAL.xyz,0);\r\n\r\n#ifdef" +
                  " USE_MORPHING\r\n    normal += getTargetNormal();\r\n#endif\r\n\r\n#ifdef USE_SKINNING\r\n" +
                  "    normal = getSkinningNormalMatrix() * normal;\r\n#endif\r\n\r\n    return normalize" +
                  "(normal);\r\n}\r\n#endif\r\n\r\n#ifdef HAS_TANGENTS\r\nvec4 getTangent()\r\n{\r\n    vec4 tang" +
                  "ent = TANGENT;\r\n\r\n#ifdef USE_MORPHING\r\n    tangent += getTargetTangent();\r\n#endi" +
                  "f\r\n\r\n#ifdef USE_SKINNING\r\n    tangent = getSkinningMatrix() * tangent;\r\n#endif\r\n" +
                  "\r\n    return normalize(tangent);\r\n}\r\n#endif\r\n\r\nvoid main()\r\n{\r\n    vec4 pos = Mo" +
                  "delMatrix * getPosition();\r\n    v_Position = vec3(pos.xyz) / pos.w;\r\n\r\n    #ifde" +
                  "f HAS_NORMALS\r\n    #ifdef HAS_TANGENTS\r\n    vec4 tangent = getTangent();\r\n    ve" +
                  "c3 normalW = normalize(vec3(NormalMatrix * vec4(getNormal().xyz, 0.0)));\r\n    ve" +
                  "c3 tangentW = normalize(vec3(ModelMatrix * vec4(tangent.xyz, 0.0)));\r\n    vec3 b" +
                  "itangentW = cross(normalW, tangentW) * tangent.w;\r\n    v_TBN = mat3(tangentW, bi" +
                  "tangentW, normalW);\r\n    #else // !HAS_TANGENTS\r\n    v_Normal = normalize(vec3(N" +
                  "ormalMatrix * vec4(getNormal().xyz, 0.0)));\r\n    #endif\r\n    #endif // !HAS_NORM" +
                  "ALS\r\n\r\n    v_UVCoord1 = vec2(0.0, 0.0);\r\n    v_UVCoord2 = vec2(0.0, 0.0);\r\n\r\n   " +
                  " #ifdef HAS_UV_SET1\r\n    v_UVCoord1 = TEXCOORD_0;\r\n    #endif\r\n\r\n    #ifdef HAS_" +
                  "UV_SET2\r\n    v_UVCoord2 = TEXCOORD_1;\r\n    #endif\r\n\r\n    #if defined(HAS_VERTEX_" +
                  "COLOR_VEC3)\r\n    v_Color = vec4(COLOR_0, 1.0f);\r\n    #endif\r\n    #if defined(HAS" +
                  "_VERTEX_COLOR_VEC4)\r\n    v_Color = COLOR_0;\r\n    #endif\r\n\r\n    gl_Position = u_V" +
                  "iewProjectionMatrix * pos;\r\n}\r\n");
            return GenerationEnvironment.ToString();
        }
    }

#line default
#line hidden

    #region Base class

    /// <summary>
    ///     Base class for this transformation
    /// </summary>
    [GeneratedCode("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public class VertexShaderBase
    {
        #region Fields

        private StringBuilder generationEnvironmentField;
        private CompilerErrorCollection errorsField;
        private List<int> indentLengthsField;
        private bool endsWithNewline;

        #endregion

        #region Properties

        /// <summary>
        ///     The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected StringBuilder GenerationEnvironment
        {
            get
            {
                if (generationEnvironmentField == null) generationEnvironmentField = new StringBuilder();
                return generationEnvironmentField;
            }
            set => generationEnvironmentField = value;
        }

        /// <summary>
        ///     The error collection for the generation process
        /// </summary>
        public CompilerErrorCollection Errors
        {
            get
            {
                if (errorsField == null) errorsField = new CompilerErrorCollection();
                return errorsField;
            }
        }

        /// <summary>
        ///     A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private List<int> indentLengths
        {
            get
            {
                if (indentLengthsField == null) indentLengthsField = new List<int>();
                return indentLengthsField;
            }
        }

        /// <summary>
        ///     Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent { get; private set; } = "";

        /// <summary>
        ///     Current transformation session
        /// </summary>
        public virtual IDictionary<string, object> Session { get; set; }

        #endregion

        #region Transform-time helpers

        /// <summary>
        ///     Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend)) return;
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (GenerationEnvironment.Length == 0
                || endsWithNewline)
            {
                GenerationEnvironment.Append(CurrentIndent);
                endsWithNewline = false;
            }

            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(Environment.NewLine, StringComparison.CurrentCulture)) endsWithNewline = true;
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if (CurrentIndent.Length == 0)
            {
                GenerationEnvironment.Append(textToAppend);
                return;
            }

            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(Environment.NewLine, Environment.NewLine + CurrentIndent);
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (endsWithNewline)
                GenerationEnvironment.Append(textToAppend, 0, textToAppend.Length - CurrentIndent.Length);
            else
                GenerationEnvironment.Append(textToAppend);
        }

        /// <summary>
        ///     Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            Write(textToAppend);
            GenerationEnvironment.AppendLine();
            endsWithNewline = true;
        }

        /// <summary>
        ///     Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            Write(string.Format(CultureInfo.CurrentCulture, format, args));
        }

        /// <summary>
        ///     Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(CultureInfo.CurrentCulture, format, args));
        }

        /// <summary>
        ///     Raise an error
        /// </summary>
        public void Error(string message)
        {
            var error = new CompilerError();
            error.ErrorText = message;
            Errors.Add(error);
        }

        /// <summary>
        ///     Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            var error = new CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            Errors.Add(error);
        }

        /// <summary>
        ///     Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if (indent == null) throw new ArgumentNullException("indent");
            CurrentIndent = CurrentIndent + indent;
            indentLengths.Add(indent.Length);
        }

        /// <summary>
        ///     Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            var returnValue = "";
            if (indentLengths.Count > 0)
            {
                var indentLength = indentLengths[indentLengths.Count - 1];
                indentLengths.RemoveAt(indentLengths.Count - 1);
                if (indentLength > 0)
                {
                    returnValue = CurrentIndent.Substring(CurrentIndent.Length - indentLength);
                    CurrentIndent = CurrentIndent.Remove(CurrentIndent.Length - indentLength);
                }
            }

            return returnValue;
        }

        /// <summary>
        ///     Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            indentLengths.Clear();
            CurrentIndent = "";
        }

        #endregion

        #region ToString Helpers

        /// <summary>
        ///     Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private IFormatProvider formatProviderField = CultureInfo.InvariantCulture;

            /// <summary>
            ///     Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public IFormatProvider FormatProvider
            {
                get => formatProviderField;
                set
                {
                    if (value != null) formatProviderField = value;
                }
            }

            /// <summary>
            ///     This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if (objectToConvert == null) throw new ArgumentNullException("objectToConvert");
                var t = objectToConvert.GetType();
                var method = t.GetMethod("ToString", new[]
                {
                    typeof(IFormatProvider)
                });
                if (method == null)
                    return objectToConvert.ToString();
                return (string) method.Invoke(objectToConvert, new object[]
                {
                    formatProviderField
                });
            }
        }

        /// <summary>
        ///     Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper { get; } = new ToStringInstanceHelper();

        #endregion
    }

    #endregion
}