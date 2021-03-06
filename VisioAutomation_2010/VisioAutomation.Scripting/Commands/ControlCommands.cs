using System.Collections.Generic;
using VACONTROL = VisioAutomation.Shapes.Controls;
using IVisio = Microsoft.Office.Interop.Visio;

namespace VisioAutomation.Scripting.Commands
{
    public class ControlCommands : CommandSet
    {
        internal ControlCommands(Client client) :
            base(client)
        {

        }

        public List<int> Add(TargetShapes targets, VACONTROL.ControlCells ctrl)
        {
            this._client.Application.AssertApplicationAvailable();
            this._client.Document.AssertDocumentAvailable();

            if (ctrl == null)
            {
                throw new System.ArgumentNullException(nameof(ctrl));
            }

            targets = targets.ResolveShapes(this._client);

            if (targets.Shapes.Count < 1)
            {
                return new List<int>(0);
            }


            var control_indices = new List<int>();

            using (var undoscope = this._client.Application.NewUndoScope("Add Control"))
            {
                foreach (var shape in targets.Shapes)
                {
                    int ci = VACONTROL.ControlHelper.Add(shape, ctrl);
                    control_indices.Add(ci);
                }
            }

            return control_indices;
        }

        public void Delete(TargetShapes targets, int n)
        {
            this._client.Application.AssertApplicationAvailable();
            this._client.Document.AssertDocumentAvailable();

            targets = targets.ResolveShapes(this._client);

            if (targets.Shapes.Count < 1)
            {
                return;
            }

            using (var undoscope = this._client.Application.NewUndoScope("Delete Control"))
            {
                foreach (var shape in targets.Shapes)
                {
                    VACONTROL.ControlHelper.Delete(shape, n);
                }
            }
        }

        public Dictionary<IVisio.Shape, IList<VACONTROL.ControlCells>> Get(TargetShapes targets)
        {
            this._client.Application.AssertApplicationAvailable();
            this._client.Document.AssertDocumentAvailable();

            targets = targets.ResolveShapes(this._client);

            if (targets.Shapes.Count < 1)
            {
                return new Dictionary<IVisio.Shape, IList<VACONTROL.ControlCells>>(0);
            }

            var dic = new Dictionary<IVisio.Shape, IList<VACONTROL.ControlCells>>();
            foreach (var shape in targets.Shapes)
            {
                var controls = VACONTROL.ControlCells.GetCells(shape);
                dic[shape] = controls;
            }
            return dic;
        }
    }
}