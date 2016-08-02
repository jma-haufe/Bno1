using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace transmate.DataService
{
    /// <summary>
    /// This interface must be implemented for each Platform to be able to do the persistence operations
    /// See http://developer.xamarin.com/guides/cross-platform/xamarin-forms/dependency-service/
    /// It is used by MockStore
    /// </summary>
    public interface IPersistenceWriter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">Name of the file without path</param>
        /// <returns>true if file already exists</returns>
        bool HasFile(string fileName);

        /// <summary>
        /// Write content to a existing file (overwrite)
        /// </summary>
        /// <param name="fileName">Name of the file without path</param>
        /// <param name="xmlContent">content</param>
        void WriteFile(string fileName, string xmlContent);

        /// <summary>
        /// Write content to a new file
        /// </summary>
        /// <param name="fileName">Name of the file without path</param>
        /// <param name="xmlContent">content</param>
        void CreateAndWriteFile(string fileName, string xmlContent);

        /// <summary>
        /// Read all content of a file
        /// </summary>
        /// <param name="fileName">Name of the file without path</param>
        /// <returns></returns>
        string ReadFile(string fileName);
    }
}
