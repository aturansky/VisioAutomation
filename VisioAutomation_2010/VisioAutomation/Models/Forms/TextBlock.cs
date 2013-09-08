using System.Collections.Generic;
using VisioAutomation.Extensions;
using VA = VisioAutomation;
using IVisio = Microsoft.Office.Interop.Visio;

namespace VisioAutomation.Models.Forms
{
    public class TextBlock
    {
        public VA.Drawing.Size Size;
        public string Font = "SegoeUI";
        public VA.Text.TextCells Textcells;
        public VA.Text.ParagraphCells ParagraphCells;
        public VA.Shapes.FormatCells FormatCells;
        public VA.Text.CharacterCells CharacterCells;
        public string Text;
        public IVisio.Shape VisioShape;

        public TextBlock(VA.Drawing.Size size, string text)
        {
            this.Text = text;
            this.Size = size;
            this.Textcells = new VA.Text.TextCells();
            this.ParagraphCells = new VA.Text.ParagraphCells();
            this.FormatCells = new VA.Shapes.FormatCells();
            this.CharacterCells = new VA.Text.CharacterCells();

            this.Textcells.VerticalAlign = 0;
            this.ParagraphCells.HorizontalAlign = 0;

            this.FormatCells.LineWeight = 0;
            this.FormatCells.LinePattern = 0;

        }

        public void ApplyFormus(VA.ShapeSheet.Update update)
        {
            short titleshape_id = this.VisioShape.ID16;
            update.SetFormulas(titleshape_id, this.Textcells);
            update.SetFormulasForRow(titleshape_id, this.ParagraphCells, 0);
            update.SetFormulasForRow(titleshape_id, this.CharacterCells, 0);
            update.SetFormulas(titleshape_id, this.FormatCells);
        }
    }
}