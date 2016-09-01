using IVisio = NetOffice.VisioApi;
using VisioAutomation.Drawing.Layout;
using VisioAutomation.Scripting.Utilities;
using VisioAutomation.Extensions;

namespace VisioAutomation.Scripting.Commands
{
    public class DistributeCommands : CommandSet
    {
        internal DistributeCommands(Client client) :
            base(client)
        {

        }

        public void DistributeOnAxis(TargetShapes targets, Axis axis, double spacing)
        {
            if (!this._client.Document.HasActiveDocument)
            {
                return;
            }
            var page = this._client.Page.Get();
            var shapes = targets.ResolveShapes(this._client);
            var targets2 = new VisioAutomation.Scripting.TargetShapes(shapes);
            var targetids = targets2.ToShapeIDs(page);
            using (var undoscope = this._client.Application.NewUndoScope("Distribute on Axis"))
            {
                ArrangeHelper.DistributeWithSpacing(targetids, axis, spacing);
            }
        }

        public void DistributeOnAxis(TargetShapes targets, Axis axis)
        {
            this._client.Application.AssertApplicationAvailable();
            this._client.Document.AssertDocumentAvailable();

            int shape_count = targets.SetSelectionGetSelectedCount(this._client);
            if (shape_count < 1)
            {
                return;
            }


            IVisio.Enums.VisUICmds cmd;

            switch (axis)
            {
                case Axis.XAxis:
                    cmd = IVisio.Enums.VisUICmds.visCmdDistributeHSpace;
                    break;
                case Axis.YAxis:
                    cmd = IVisio.Enums.VisUICmds.visCmdDistributeVSpace;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }

            var application = this._client.Application.Get();
            using (var undoscope = this._client.Application.NewUndoScope("Distribute Shapes"))
            {
                application.DoCmd((short)cmd);
            }
        }

        public void DistributeHorizontal(TargetShapes targets, AlignmentHorizontal halign)
        {
            this._client.Application.AssertApplicationAvailable();
            this._client.Document.AssertDocumentAvailable();

            int shape_count = targets.SetSelectionGetSelectedCount(this._client);
            if (shape_count < 1)
            {
                return;
            }

            IVisio.Enums.VisUICmds cmd;

            switch (halign)
            {
                case AlignmentHorizontal.Left:
                    cmd = IVisio.Enums.VisUICmds.visCmdDistributeLeft;
                    break;
                case AlignmentHorizontal.Center:
                    cmd = IVisio.Enums.VisUICmds.visCmdDistributeCenter;
                    break;
                case AlignmentHorizontal.Right:
                    cmd = IVisio.Enums.VisUICmds.visCmdDistributeRight;
                    break;
                default: throw new System.ArgumentOutOfRangeException();
            }

            var application = this._client.Application.Get();
            application.DoCmd((short)cmd);
        }

        public void DistributeVertical(TargetShapes targets, AlignmentVertical valign)
        {
            this._client.Application.AssertApplicationAvailable();
            this._client.Document.AssertDocumentAvailable();

            int shape_count = targets.SetSelectionGetSelectedCount(this._client);
            if (shape_count < 1)
            {
                return;
            }

            IVisio.Enums.VisUICmds cmd;
            switch (valign)
            {
                case AlignmentVertical.Top:
                    cmd = IVisio.Enums.VisUICmds.visCmdDistributeTop;
                    break;
                case AlignmentVertical.Center: cmd = IVisio.Enums.VisUICmds.visCmdDistributeMiddle; break;
                case AlignmentVertical.Bottom: cmd = IVisio.Enums.VisUICmds.visCmdDistributeBottom; break;
                default: throw new System.ArgumentOutOfRangeException();
            }

            var application = this._client.Application.Get();
            application.DoCmd((short)cmd);
        }
    }
}