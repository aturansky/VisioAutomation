using System.Collections.Generic;
using System.Linq;

using VisioPS.Extensions;
using VA = VisioAutomation;
using SMA = System.Management.Automation;

namespace VisioPS.Commands
{
    [SMA.Cmdlet(SMA.VerbsLifecycle.Invoke, "VisioDraw")]
    public class Invoke_VisioDraw : VisioPS.VisioPSCmdlet
    {
        [SMA.Parameter(ParameterSetName="orgchcart",Position=0,Mandatory = true)]
        public VA.Layout.Models.OrgChart.Drawing OrgChart { get; set; }

        [SMA.Parameter(ParameterSetName = "grid", Position = 0, Mandatory = true)]
        public VA.Layout.Models.Grid.GridLayout GridLayout { get; set; }

        [SMA.Parameter(ParameterSetName = "directedgraph", Position = 0, Mandatory = true)]
        public List<VA.Layout.Models.DirectedGraph.Drawing> DirectedGraphs { get; set; }

        [SMA.Parameter(ParameterSetName = "datatable", Position = 0, Mandatory = true)]
        public System.Data.DataTable DataTable { get; set; }

        [SMA.Parameter(ParameterSetName = "datatable", Position = 1, Mandatory = true)]
        public double CellWidth { get; set; }

        [SMA.Parameter(ParameterSetName = "datatable", Position = 2, Mandatory = true)]
        public double CellHeight { get; set; }

        [SMA.Parameter(ParameterSetName = "datatable", Position = 3, Mandatory = true)]
        public double CellSpacing { get; set; }

        protected override void ProcessRecord()
        {
            if (this.OrgChart != null)
            {
                var scriptingsession = this.ScriptingSession;
                scriptingsession.Draw.OrgChart(this.OrgChart);                
            }
            else if (this.GridLayout != null)
            {
                var scriptingsession = this.ScriptingSession;
                scriptingsession.Draw.Grid(this.GridLayout);
            }
            else if (this.DirectedGraphs != null)
            {
                var scriptingsession = this.ScriptingSession;
                VA.Scripting.DirectedGraph.DirectedGraphBuilder.RenderDiagrams(scriptingsession, this.DirectedGraphs);
            }
            else if (this.DataTable != null)
            {
                var scriptingsession = this.ScriptingSession;

                var widths = Enumerable.Repeat<double>(CellWidth, DataTable.Columns.Count).ToList();
                var heights = Enumerable.Repeat<double>(CellHeight, DataTable.Rows.Count).ToList();
                var spacing = new VA.Drawing.Size(CellSpacing, CellSpacing);
                var shapes = scriptingsession.Draw.Table(DataTable, widths, heights, spacing);
                this.WriteObject(shapes);
            }
            else
            {
                
            }
        }
    }
}