using System.Collections.Generic;
using System.Linq;
using VisioAutomation.Extensions;
using VisioAutomation.Scripting.Utilities;
using VisioAutomation.ShapeSheet.Writers;
using IVisio = NetOffice.VisioApi;

namespace VisioAutomation.Scripting.Commands
{
    public class TextCommands: CommandSet
    {
        internal TextCommands(Client client) :
            base(client)
        {

        }

       public void Set(TargetShapes targets, IList<string> texts)
       {
           this._client.Application.AssertApplicationAvailable();
           this._client.Document.AssertDocumentAvailable();

           if (texts == null || texts.Count < 1)
           {
               // do nothing
               return;
           }

            var shapes = targets.ResolveShapes(this._client);

            if (shapes.Count < 1)
           {
               return;
           }

           var application = this._client.Application.Get();
           using (var undoscope = this._client.Application.NewUndoScope("Set Shape Text"))
           {
               int numtexts = texts.Count;

               int up_to = System.Math.Min(numtexts, shapes.Count);

               for (int i = 0; i < up_to; i++)
               {
                   var text = texts[i % numtexts];
                   if (text != null)
                   {
                       var shape = shapes[i];
                       shape.Text = text;
                   }
               }
           }
       }

        public IList<string> Get(TargetShapes targets)
        {
            this._client.Application.AssertApplicationAvailable();
            this._client.Document.AssertDocumentAvailable();

            var shapes = targets.ResolveShapes(this._client);

            if (shapes.Count < 1)
            {
                return new List<string>(0);
            }

            var texts = shapes.Select(s => s.Text).ToList();
            return texts;
        }

        public void ToogleCase(TargetShapes targets)
        {
            this._client.Application.AssertApplicationAvailable();
            this._client.Document.AssertDocumentAvailable();

            var shapes = targets.ResolveShapes(this._client);


            if (shapes.Count < 1)
            {
                return;
            }

            var application = this._client.Application.Get();
            using (var undoscope = this._client.Application.NewUndoScope("Toggle Shape Text Case"))
            {
                var shapeids = shapes.Select(s => s.ID).ToList();

                var page = application.ActivePage;
                // Store all the formatting
                var formats = Text.TextFormat.GetFormat(page, shapeids);

                // Change the text - this will wipe out all the character and paragraph formatting
                foreach (var shape in shapes)
                {
                    string t = shape.Text;
                    if (t.Length < 1)
                    {
                        continue;
                    }
                    shape.Text = TextHelper.toggle_case(t);
                }

                // Now restore all the formatting - based on any initial formatting from the text

                var writer = new FormulaWriterSIDSRC();
                for (int i = 0; i < shapes.Count; i++)
                {
                    var format = formats[i];

                    if (format.CharacterFormats.Count>0)
                    {
                        var fmt = format.CharacterFormats[0];
                        fmt.SetFormulas((short) shapeids[i], writer, 0);
                    }

                    if (format.ParagraphFormats.Count > 0)
                    {
                        var fmt = format.ParagraphFormats[0];
                        fmt.SetFormulas((short)shapeids[i], writer, 0);
                    }
                }

                writer.Commit(page);
            }
        }

        public void SetFont(TargetShapes targets, string fontname)
        {
            this._client.Application.AssertApplicationAvailable();
            this._client.Document.AssertDocumentAvailable();

            var shapes = targets.ResolveShapes(this._client);

            if (shapes.Count < 1)
            {
                return;
            }
            var application = this._client.Application.Get();
            var active_document = application.ActiveDocument;
            var active_doc_fonts = active_document.Fonts;
            var font = active_doc_fonts[fontname];
            IVisio.Enums.VisGetSetArgs flags=0;
            var srcs = new[] {ShapeSheet.SRCConstants.CharFont};
            var formulas = new[] { font.ID.ToString() };
            this._client.ShapeSheet.SetFormula(targets, srcs, formulas, flags);
        }

        public IList<Text.TextFormat> GetFormat(TargetShapes targets)
        {
            this._client.Application.AssertApplicationAvailable();
            this._client.Document.AssertDocumentAvailable();

            var shapes = targets.ResolveShapes(this._client);

            if (shapes.Count < 1)
            {
                return new List<Text.TextFormat>(0);
            }

            var selection = this._client.Selection.Get();
            var shapeids = selection.GetIDs();
            var application = this._client.Application.Get();
            var formats = Text.TextFormat.GetFormat(application.ActivePage, shapeids);
            return formats;
        }

        public void MoveTextToBottom(TargetShapes targets)
        {
            this._client.Application.AssertApplicationAvailable();
            this._client.Document.AssertDocumentAvailable();

            var shapes = targets.ResolveShapes(this._client);

            if (shapes.Count < 1)
            {
                return ;
            }

            var writer = new FormulaWriterSIDSRC();
            foreach (var shape in shapes)
            {
                if (0 ==
                    shape.get_RowExists(
                        (short) IVisio.Enums.VisSectionIndices.visSectionObject, (short) IVisio.Enums.VisRowIndices.visRowTextXForm,
                        (short) IVisio.Enums.VisExistsFlags.visExistsAnywhere))
                {
                    shape.AddRow((short)IVisio.Enums.VisSectionIndices.visSectionObject, (short)IVisio.Enums.VisRowIndices.visRowTextXForm, (short)IVisio.Enums.VisRowTags.visTagDefault); 
                    
                }
            }

            var application = this._client.Application.Get();
            var shapeids = shapes.Select(s=>s.ID);
            foreach (int shapeid in shapeids)
            {
                writer.SetFormula((short)shapeid, ShapeSheet.SRCConstants.TxtHeight, "Height*0"); 
                writer.SetFormula((short)shapeid, ShapeSheet.SRCConstants.TxtPinY, "Height*0"); 
                writer.SetFormula((short)shapeid, ShapeSheet.SRCConstants.VerticalAlign, "0");
            } 
            var active_page = application.ActivePage; 
            writer.Commit(active_page);
        }

        public void SetTextWrapping(TargetShapes targets,bool wrap)
        {
            this._client.Application.AssertApplicationAvailable();
            this._client.Document.AssertDocumentAvailable();

            var shapes = targets.ResolveShapes2DOnly(this._client);

            if (shapes.Count < 1)
            {
                return;
            }

            var shapeids = shapes.Select(s => s.ID).ToList();
            var application = this._client.Application.Get();
            using (var undoscope = this._client.Application.NewUndoScope("SetTextWrapping"))
            {
                var active_page = application.ActivePage;
                TextHelper.set_text_wrapping(active_page, shapeids, wrap);
            }
        }

        public void FitShapeToText(TargetShapes targets)
        {
            this._client.Application.AssertApplicationAvailable();
            this._client.Document.AssertDocumentAvailable();

            var shapes = targets.ResolveShapes2DOnly(this._client);

            if (shapes.Count < 1)
            {
                return;
            }

            var application = this._client.Application.Get();
            var active_page = application.ActivePage;
            var shapeids = shapes.Select(s => s.ID).ToList();

            using (var undoscope = this._client.Application.NewUndoScope("FitShapeToText"))
            {
                // Calculate the new sizes for each shape
                var new_sizes = new List<Drawing.Size>(shapeids.Count);
                foreach (var shape in shapes)
                {
                    var text_bounding_box = shape.GetBoundingBox(IVisio.Enums.VisBoundingBoxArgs.visBBoxUprightText).Size;
                    var wh_bounding_box = shape.GetBoundingBox(IVisio.Enums.VisBoundingBoxArgs.visBBoxUprightWH).Size;

                    double max_w = System.Math.Max(text_bounding_box.Width, wh_bounding_box.Width);
                    double max_h = System.Math.Max(text_bounding_box.Height, wh_bounding_box.Height);
                    var max_size = new Drawing.Size(max_w, max_h);
                    new_sizes.Add(max_size);
                }

                var src_width = ShapeSheet.SRCConstants.Width;
                var src_height = ShapeSheet.SRCConstants.Height;

                var writer = new FormulaWriterSIDSRC();
                for (int i = 0; i < new_sizes.Count; i++)
                {
                    var shapeid = shapeids[i];
                    var new_size = new_sizes[i];
                    writer.SetFormula((short)shapeid, src_width, new_size.Width);
                    writer.SetFormula((short)shapeid, src_height, new_size.Height);
                }

                writer.Commit(active_page);
            }
        }
    }
}


