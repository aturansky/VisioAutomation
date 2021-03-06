using System.IO;
using System.Management.Automation;

namespace VisioPowerShell.Commands
{
    [Cmdlet(VerbsCommon.Open, VisioPowerShell.Nouns.VisioDocument)]
    public class Open_VisioDocument : VisioCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string Filename { get; set; }

        protected override void ProcessRecord()
        {
            if (this.Client.Application.HasApplication == false)
            {
                // no app - let's create one
                this.Client.Application.New();
            }

            if (this.filename_is_stencil(this.Filename))
            {
                var doc = this.Client.Document.OpenStencil(this.Filename);
                this.WriteObject(doc);                
            }
            else
            {
                var doc = this.Client.Document.Open(this.Filename);
                this.WriteObject(doc);                
            }
        }

        public bool filename_is_stencil(string fname)
        {
            var ext = Path.GetExtension(fname).ToLowerInvariant();
            return (ext == ".vss" || ext == ".vst" || ext == ".vssx" || ext == ".vstx");
        }
    }
}