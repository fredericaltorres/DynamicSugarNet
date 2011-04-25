using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections;
using System.Dynamic;
using System.Reflection;

namespace DynamicSugar {
    
    /// <summary>
    /// Dynamic Sharp Helper Class, dedicated methods to work with text resource file
    /// </summary>
    public static partial class DS {

        public static class Resources {
            /// <summary>
            /// Return the fully qualified name of the resource file
            /// </summary>
            /// <param name="resourceFileName">File name of the resource</param>
            /// <returns></returns>
            private static string GetResourceFullName(string resourceFileName, Assembly assembly ) {
        
                foreach(var resource in assembly.GetManifestResourceNames())
                    if(resource.EndsWith("."+resourceFileName))
                        return resource;
                throw new System.ApplicationException("Resource '{0}' not find in assembly '{1}'".format(resourceFileName, Assembly.GetExecutingAssembly().FullName));
            }
            /// <summary>
            /// Return the content of a text file embed as a resource.
            /// The function takes care of finding the fully qualify name, in the current
            /// assembly.
            /// </summary>
            /// <param name="resourceFileName">The file name of the resource</param>
            /// <returns></returns>
            public static string GetTextResource(string resourceFileName, Assembly assembly) {

                var resourceFullName = GetResourceFullName(resourceFileName, assembly);
                    
                using (var _textStreamReader = new StreamReader(assembly.GetManifestResourceStream(resourceFullName)))
                    return _textStreamReader.ReadToEnd();
            }
        }
    }
}