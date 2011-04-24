using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;

namespace DynamicSugar.Experimental {

    /// <summary>
    /// Exception raised by the class DynamicTextFileManager
    /// </summary>
    public class DynamicTextFileManagerException : System.Exception {

        public DynamicTextFileManagerException(string message) : base(message) { }
        public DynamicTextFileManagerException(string message, System.Exception innerEx) : base(message, innerEx) { }
    }        
    /// <summary>
    /// Class allowing to read and write text file, just by setting or getting property.
    /// The property name is the filename.
    /// </summary>
    public class DynamicTextFileManager : DynamicObject {
     
        private string _path;
        private string _extension;
                
        public static dynamic Create(string path, string extension){

            dynamic o = new DynamicTextFileManager(path,extension);
            return o;
        }            
        public DynamicTextFileManager(string path, string extension){

            if(!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            
            foreach(char c in extension)
                if(c.In(DS.List('\\','?','*',':','|')))
                    throw new DynamicTextFileManagerException("Invalid extension file '{0}'".format(extension));

            this._path      = path;
            this._extension = extension;
        }        
        public virtual object GetMember(string name) {

            return null;
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result) {
            
            var fileName = @"{0}\{1}.{2}".format(this._path, binder.Name, this._extension);
            if(System.IO.File.Exists(fileName)){
                result = System.IO.File.ReadAllText(fileName);
                return true;
            }
            else {
                throw new DynamicTextFileManagerException("".format("Property/file '{0}' not find as file '{1}'", binder.Name, fileName));
            }
        }      
        public override bool TrySetMember(SetMemberBinder binder, object value) {         

            var fileName = @"{0}\{1}.{2}".format(this._path, binder.Name, this._extension);
            try{
                if(System.IO.File.Exists(fileName)){
                    System.IO.File.Delete(fileName);
                }
                System.IO.File.WriteAllText( fileName, value.ToString()); 
                return true;
            }
            catch(System.Exception ex){

                throw new DynamicTextFileManagerException("".format("Property/file '{0}' cannot be written as file '{1}'", binder.Name, fileName), ex);
            }            
        }        
    }
}
