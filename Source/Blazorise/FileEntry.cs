#region Using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    public class FileEntry : IFileEntry
    {
        #region Methods

        public void Init( FileEdit fileEdit )
        {
            Owner = fileEdit;
        }

        public async Task WriteToStreamAsync( Stream stream )
        {
            await Owner.WriteToStreamAsync( this, stream );
        }

        #endregion

        #region Properties

        internal FileEdit Owner { get; set; }

        public int Id { get; set; }

        public DateTime LastModified { get; set; }

        public string Name { get; set; }

        public long Size { get; set; }

        public string Type { get; set; }

        #endregion
    }
}
