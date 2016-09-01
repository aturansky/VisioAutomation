﻿using System.Linq;
using System.Management.Automation;
using IVisio = NetOffice.VisioApi;

namespace VisioPowerShell.Commands.New
{
    [Cmdlet(VerbsCommon.New, VisioPowerShell.Nouns.VisioShape)]
    public class New_VisioShape : VisioCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public IVisio.IVMaster[] Masters { get; set; }

        [Parameter(Position = 1, Mandatory = true)]
        public double [] Points { get; set; }

        [Parameter(Mandatory = false)]
        public string[] Names { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter NoSelect=false;

        protected override void ProcessRecord()
        {
            this.WriteVerbose("NoSelect: {0}", this.NoSelect);

            var points = VisioAutomation.Drawing.Point.FromDoubles(this.Points).ToList();
            var shape_ids = this.Client.Master.Drop(this.Masters, points);

            var page = this.Client.Page.Get();
            var shape_objects = VisioAutomation.Shapes.ShapeHelper.GetShapesFromIDs(page.Shapes, shape_ids);

          // If Names is not empty... assign it to the shape
             if (this.Names != null)
             {
                 int up_to = System.Math.Min(shape_objects.Count, this.Names.Length);
                 for (int i = 0; i < up_to; i++)
                 {
                     string cur_name = this.Names[i];
                     if (cur_name != null)
                     {
                         var cur_shape = shape_objects[i];
                         cur_shape.NameU = cur_name;                         
                     }
                 }
             }

            this.Client.Selection.SelectNone();

            if (!this.NoSelect)
            {
                // Select the Shapes
                ((Cmdlet) this).WriteVerbose("Selecting");
                this.Client.Selection.Select(shape_objects);
            }


            this.WriteObject(shape_objects, false);
        }
    }
}