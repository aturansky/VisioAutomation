using System.Collections.Generic;
using IVisio = Microsoft.Office.Interop.Visio;
using VA = VisioAutomation;
using VisioAutomation.Extensions;
using VisioAutomation.ShapeSheet.Query;

namespace VisioAutomation.Text
{
    public class CharacterCells : ShapeSheet.CellGroups.CellGroupMultiRow
    {
        public ShapeSheet.CellData<int> Color { get; set; }
        public ShapeSheet.CellData<int> Font { get; set; }
        public ShapeSheet.CellData<double> Size { get; set; }
        public ShapeSheet.CellData<int> Style { get; set; }
        public ShapeSheet.CellData<double> Transparency { get; set; }
        public ShapeSheet.CellData<int> AsianFont { get; set; }
        public ShapeSheet.CellData<int> Case { get; set; }
        public ShapeSheet.CellData<int> ComplexScriptFont { get; set; }
        public ShapeSheet.CellData<double> ComplexScriptSize { get; set; }
        public ShapeSheet.CellData<bool> DoubleStrikeThrough { get; set; }
        public ShapeSheet.CellData<bool> DoubleUnderline { get; set; }
        public ShapeSheet.CellData<int> LangID { get; set; }
        public ShapeSheet.CellData<int> Locale { get; set; }
        public ShapeSheet.CellData<int> LocalizeFont { get; set; }
        public ShapeSheet.CellData<bool> Overline { get; set; }
        public ShapeSheet.CellData<bool> Perpendicular { get; set; }
        public ShapeSheet.CellData<int> Pos { get; set; }
        public ShapeSheet.CellData<int> RTLText { get; set; }
        public ShapeSheet.CellData<double> FontScale { get; set; }
        public ShapeSheet.CellData<double> Letterspace { get; set; }
        public ShapeSheet.CellData<bool> Strikethru { get; set; }
        public ShapeSheet.CellData<int> UseVertical { get; set; }

        public override IEnumerable<SRCFormulaPair> Pairs
        {
            get
            {
                yield return this.newpair(ShapeSheet.SRCConstants.CharColor, this.Color.Formula);
                yield return this.newpair(ShapeSheet.SRCConstants.CharFont, this.Font.Formula);
                yield return this.newpair(ShapeSheet.SRCConstants.CharSize, this.Size.Formula);
                yield return this.newpair(ShapeSheet.SRCConstants.CharStyle, this.Style.Formula);
                yield return this.newpair(ShapeSheet.SRCConstants.CharColorTrans, this.Transparency.Formula);
                yield return this.newpair(ShapeSheet.SRCConstants.CharAsianFont, this.AsianFont.Formula);
                yield return this.newpair(ShapeSheet.SRCConstants.CharCase, this.Case.Formula);
                yield return this.newpair(ShapeSheet.SRCConstants.CharComplexScriptFont, this.ComplexScriptFont.Formula);
                yield return this.newpair(ShapeSheet.SRCConstants.CharComplexScriptSize, this.ComplexScriptSize.Formula);
                yield return this.newpair(ShapeSheet.SRCConstants.CharDblUnderline, this.DoubleUnderline.Formula);
                yield return this.newpair(ShapeSheet.SRCConstants.CharDoubleStrikethrough, this.DoubleStrikeThrough.Formula);
                yield return this.newpair(ShapeSheet.SRCConstants.CharLangID, this.LangID.Formula);
                yield return this.newpair(ShapeSheet.SRCConstants.CharFontScale, this.FontScale.Formula);
                yield return this.newpair(ShapeSheet.SRCConstants.CharLangID, this.LangID.Formula);
                yield return this.newpair(ShapeSheet.SRCConstants.CharLetterspace, this.Letterspace.Formula);
                yield return this.newpair(ShapeSheet.SRCConstants.CharLocale, this.Locale.Formula);
                yield return this.newpair(ShapeSheet.SRCConstants.CharLocalizeFont, this.LocalizeFont.Formula);
                yield return this.newpair(ShapeSheet.SRCConstants.CharOverline, this.Overline.Formula);
                yield return this.newpair(ShapeSheet.SRCConstants.CharPerpendicular, this.Perpendicular.Formula);
                yield return this.newpair(ShapeSheet.SRCConstants.CharPos, this.Pos.Formula);
                yield return this.newpair(ShapeSheet.SRCConstants.CharRTLText, this.RTLText.Formula);
                yield return this.newpair(ShapeSheet.SRCConstants.CharStrikethru, this.Strikethru.Formula);
                yield return this.newpair(ShapeSheet.SRCConstants.CharUseVertical, this.UseVertical.Formula);
            }
        }

        public static IList<List<CharacterCells>> GetCells(IVisio.Page page, IList<int> shapeids)
        {
            var query = get_query();
            return _GetCells<CharacterCells,double>(page, shapeids, query, query.GetCells);
        }

        public static IList<CharacterCells> GetCells(IVisio.Shape shape)
        {
            var query = get_query();
            return _GetCells<CharacterCells,double>(shape, query, query.GetCells);
        }

        private static CharacterFormatCellQuery _mCellQuery;

        private static CharacterFormatCellQuery get_query()
        {
            _mCellQuery = _mCellQuery ?? new CharacterFormatCellQuery();
            return _mCellQuery;
        }

        class CharacterFormatCellQuery : CellQuery
        {
            public CellColumn Font { get; set; }
            public CellColumn Style { get; set; }
            public CellColumn Color { get; set; }
            public CellColumn Size { get; set; }
            public CellColumn Trans { get; set; }
            public CellColumn AsianFont { get; set; }
            public CellColumn Case { get; set; }
            public CellColumn ComplexScriptFont { get; set; }
            public CellColumn ComplexScriptSize { get; set; }
            public CellColumn DoubleStrikethrough { get; set; }
            public CellColumn DoubleUnderline { get; set; }
            public CellColumn LangID { get; set; }
            public CellColumn Locale { get; set; }
            public CellColumn LocalizeFont { get; set; }
            public CellColumn Overline { get; set; }
            public CellColumn Perpendicular { get; set; }
            public CellColumn Pos { get; set; }
            public CellColumn RTLText { get; set; }
            public CellColumn FontScale { get; set; }
            public CellColumn Letterspace { get; set; }
            public CellColumn Strikethru { get; set; }
            public CellColumn UseVertical { get; set; }

            public CharacterFormatCellQuery() 
            {
                var sec = this.AddSection(IVisio.VisSectionIndices.visSectionCharacter);
                this.Color = sec.AddCell(ShapeSheet.SRCConstants.CharColor, "CharColor");
                this.Trans = sec.AddCell(ShapeSheet.SRCConstants.CharColorTrans, "CharColorTrans");
                this.Font = sec.AddCell(ShapeSheet.SRCConstants.CharFont, "CharFont");
                this.Size = sec.AddCell(ShapeSheet.SRCConstants.CharSize, "CharSize");
                this.Style = sec.AddCell(ShapeSheet.SRCConstants.CharStyle, "CharStyle");
                this.AsianFont = sec.AddCell(ShapeSheet.SRCConstants.CharAsianFont, "CharAsianFont");
                this.Case = sec.AddCell(ShapeSheet.SRCConstants.CharCase, "CharCase");
                this.ComplexScriptFont = sec.AddCell(ShapeSheet.SRCConstants.CharComplexScriptFont, "CharComplexScriptFont");
                this.ComplexScriptSize = sec.AddCell(ShapeSheet.SRCConstants.CharComplexScriptSize, "CharComplexScriptSize");
                this.DoubleStrikethrough = sec.AddCell(ShapeSheet.SRCConstants.CharDoubleStrikethrough, "CharDoubleStrikethrough");
                this.DoubleUnderline = sec.AddCell(ShapeSheet.SRCConstants.CharDblUnderline, "CharDblUnderline");
                this.LangID = sec.AddCell(ShapeSheet.SRCConstants.CharLangID, "CharLangID");
                this.Locale = sec.AddCell(ShapeSheet.SRCConstants.CharLocale, "CharLocale");
                this.LocalizeFont = sec.AddCell(ShapeSheet.SRCConstants.CharLocalizeFont, "CharLocalizeFont");
                this.Overline = sec.AddCell(ShapeSheet.SRCConstants.CharOverline, "CharOverline");
                this.Perpendicular = sec.AddCell(ShapeSheet.SRCConstants.CharPerpendicular, "CharPerpendicular");
                this.Pos = sec.AddCell(ShapeSheet.SRCConstants.CharPos, "CharPos");
                this.RTLText = sec.AddCell(ShapeSheet.SRCConstants.CharRTLText, "CharRTLText");
                this.FontScale = sec.AddCell(ShapeSheet.SRCConstants.CharFontScale, "CharFontScale");
                this.Letterspace = sec.AddCell(ShapeSheet.SRCConstants.CharLetterspace, "CharLetterspace");
                this.Strikethru = sec.AddCell(ShapeSheet.SRCConstants.CharStrikethru, "CharStrikethru");
                this.UseVertical = sec.AddCell(ShapeSheet.SRCConstants.CharUseVertical, "CharUseVertical");
            }

            public CharacterCells GetCells(IList<ShapeSheet.CellData<double>> row)
            {
                var cells = new CharacterCells();
                cells.Color = row[this.Color].ToInt();
                cells.Transparency = row[this.Trans];
                cells.Font = row[this.Font].ToInt();
                cells.Size = row[this.Size];
                cells.Style = row[this.Style].ToInt();
                cells.AsianFont = row[this.AsianFont].ToInt();
                cells.AsianFont = row[this.AsianFont].ToInt();
                cells.Case = row[this.Case].ToInt();
                cells.ComplexScriptFont = row[this.ComplexScriptFont].ToInt();
                cells.ComplexScriptSize = row[this.ComplexScriptSize];
                cells.DoubleStrikeThrough = row[this.DoubleStrikethrough].ToBool();
                cells.DoubleUnderline = row[this.DoubleUnderline].ToBool();
                cells.FontScale = row[this.FontScale];
                cells.LangID = row[this.LangID].ToInt();
                cells.Letterspace = row[this.Letterspace];
                cells.Locale = row[this.Locale].ToInt();
                cells.LocalizeFont = row[this.LocalizeFont].ToInt();
                cells.Overline = row[this.Overline].ToBool();
                cells.Perpendicular = row[this.Perpendicular].ToBool();
                cells.Pos = row[this.Pos].ToInt();
                cells.RTLText = row[this.RTLText].ToInt();
                cells.Strikethru = row[this.Strikethru].ToBool();
                cells.UseVertical = row[this.UseVertical].ToInt();

                return cells;
            }
        }
    }
}