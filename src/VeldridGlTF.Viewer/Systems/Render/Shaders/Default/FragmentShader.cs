﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace VeldridGlTF.Viewer.Systems.Render.Shaders.Default
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "E:\MyWork\VeldridGlTF\src\VeldridGlTF.Viewer\Systems\Render\Shaders\Default\FragmentShader.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class FragmentShader : FragmentShaderBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write(@"#version 450

struct MaterialPropertiesInfo
{
    vec4 BaseColor;
};

layout(set = 0, binding = 1) uniform texture2D BRDFTexture;
layout(set = 0, binding = 2) uniform sampler BRDFSampler;

layout(set = 1, binding = 0) uniform textureCube ReflectionTexture;
layout(set = 1, binding = 1) uniform sampler ReflectionSampler;

layout(set = 3, binding = 0) uniform MaterialProperties
{
    MaterialPropertiesInfo _MaterialProperties;
};
layout(set = 3, binding = 1) uniform texture2D SurfaceTexture;
layout(set = 3, binding = 2) uniform sampler SurfaceSampler;

");
            
            #line 26 "E:\MyWork\VeldridGlTF\src\VeldridGlTF.Viewer\Systems\Render\Shaders\Default\FragmentShader.tt"

	for (int location=0; location<Context.Varyings.Count; ++location)
	{
		WriteLine(string.Format("layout(location = {0}) in {1} {2};", Context.Varyings[location].Location, Glsl.NameOf(Context.Varyings[location].Format), Context.Varyings[location].Name));
	}

            
            #line default
            #line hidden
            this.Write("layout(location = 0) out vec4 fsout_color;\r\n\r\nvoid main()\r\n{\r\n    float light = 1" +
                    ".0;\r\n\tvec3 normal = vec3(0.0,1.0,0.0);\r\n");
            
            #line 38 "E:\MyWork\VeldridGlTF\src\VeldridGlTF.Viewer\Systems\Render\Shaders\Default\FragmentShader.tt"

	
    if (Context.Normal != null)
	{
		WriteLine("normal = normalize({0});", Context.Normal.Name);
	}
    else if (Context.TBN != null)
	{
		WriteLine("normal = normalize({0}[2]);", Context.TBN.Name);
	}
	WriteLine("light = normal.y*0.5+0.5;");
    if (Context.IsFlagSet(ShaderFlag.HAS_DIFFUSE_MAP) && Context.TexCoord0 != null)
	{

            
            #line default
            #line hidden
            this.Write("\tvec4 diffuse = texture(sampler2D(SurfaceTexture, SurfaceSampler), vec2(0.0, 0.0)" +
                    ");\r\n\tvec4 brdf = texture(sampler2D(BRDFTexture, BRDFSampler), vec2(0.0, 0.0));\r\n" +
                    "\tvec4 reflection = texture(samplerCube(ReflectionTexture, ReflectionSampler), re" +
                    "flect(");
            
            #line 54 "E:\MyWork\VeldridGlTF\src\VeldridGlTF.Viewer\Systems\Render\Shaders\Default\FragmentShader.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Context.CameraPosition));
            
            #line default
            #line hidden
            this.Write(" - ");
            
            #line 54 "E:\MyWork\VeldridGlTF\src\VeldridGlTF.Viewer\Systems\Render\Shaders\Default\FragmentShader.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Context.WorldPosition));
            
            #line default
            #line hidden
            this.Write(", normal));\r\n\tfsout_color = reflection+brdf+diffuse;\r\n    //fsout_color = texture" +
                    "(sampler2D(SurfaceTexture, SurfaceSampler), ");
            
            #line 56 "E:\MyWork\VeldridGlTF\src\VeldridGlTF.Viewer\Systems\Render\Shaders\Default\FragmentShader.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Context.TexCoord0));
            
            #line default
            #line hidden
            this.Write(") * vec4(light,light,light,light);\r\n");
            
            #line 57 "E:\MyWork\VeldridGlTF\src\VeldridGlTF.Viewer\Systems\Render\Shaders\Default\FragmentShader.tt"

	}
	else
	{

            
            #line default
            #line hidden
            this.Write("    fsout_color = _MaterialProperties.BaseColor * vec4(light,light,light,light);\r" +
                    "\n");
            
            #line 63 "E:\MyWork\VeldridGlTF\src\VeldridGlTF.Viewer\Systems\Render\Shaders\Default\FragmentShader.tt"

	}

            
            #line default
            #line hidden
            this.Write("\t\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public class FragmentShaderBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}
