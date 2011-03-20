using VAS = VisioAutomation.Scripting;
using SMA = System.Management.Automation;

namespace VisioPS.Commands
{
    [SMA.Cmdlet("Draw", "Oval")]
    public class Draw_Oval : VisioPS.VisioPSCmdlet
    {
        [SMA.Parameter(Position = 0, Mandatory = true)]
        public double X0 { get; set; }

        [SMA.Parameter(Position = 1, Mandatory = true)]
        public double Y0 { get; set; }

        [SMA.Parameter(Position = 2, Mandatory = true)]
        public double X1 { get; set; }

        [SMA.Parameter(Position = 3, Mandatory = true)]
        public double Y1 { get; set; }

        protected override void ProcessRecord()
        {
            var scriptingsession = this.ScriptingSession;
            var shape = scriptingsession.Draw.DrawOval(X0, Y0, X1, Y1);
            this.WriteObject(shape);
        }
    }
}