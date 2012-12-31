using System.Collections.Generic;
using VA=VisioAutomation;
using IVisio = Microsoft.Office.Interop.Visio;
using VisioAutomation.Extensions;

namespace VisioAutomation.Controls
{
    public class ControlCells : VA.ShapeSheet.CellGroups.CellGroupMultiRow
    {
        public VA.ShapeSheet.CellData<int> CanGlue { get; set; }
        public VA.ShapeSheet.CellData<int> Tip { get; set; }
        public VA.ShapeSheet.CellData<double> X { get; set; }
        public VA.ShapeSheet.CellData<double> Y { get; set; }
        public VA.ShapeSheet.CellData<int> YBehavior { get; set; }
        public VA.ShapeSheet.CellData<int> XBehavior { get; set; }
        public VA.ShapeSheet.CellData<int> XDynamics { get; set; }
        public VA.ShapeSheet.CellData<int> YDynamics { get; set; }

        public override void ApplyFormulasForRow(ApplyFormula func, short row)
        {
            func(VA.ShapeSheet.SRCConstants.Controls_CanGlue.ForRow(row), this.CanGlue.Formula);
            func(VA.ShapeSheet.SRCConstants.Controls_Tip.ForRow(row), this.Tip.Formula);
            func(VA.ShapeSheet.SRCConstants.Controls_X.ForRow(row), this.X.Formula);
            func(VA.ShapeSheet.SRCConstants.Controls_Y.ForRow(row), this.Y.Formula);
            func(VA.ShapeSheet.SRCConstants.Controls_YCon.ForRow(row), this.YBehavior.Formula);
            func(VA.ShapeSheet.SRCConstants.Controls_XCon.ForRow(row), this.XBehavior.Formula);
            func(VA.ShapeSheet.SRCConstants.Controls_XDyn.ForRow(row), this.XDynamics.Formula);
            func(VA.ShapeSheet.SRCConstants.Controls_YDyn.ForRow(row), this.YDynamics.Formula);
        }

        private static ControlCells get_cells_from_row(ControlQuery query, VA.ShapeSheet.Data.Table<VA.ShapeSheet.CellData<double>> table, int row)
        {
            var cells = new ControlCells();
            cells.CanGlue = table[row,query.CanGlue].ToInt();
            cells.Tip = table[row,query.Tip].ToInt();
            cells.X = table[row,query.X];
            cells.Y = table[row,query.Y];
            cells.YBehavior = table[row,query.YBehavior].ToInt();
            cells.XBehavior = table[row,query.XBehavior].ToInt();
            cells.XDynamics = table[row,query.XDynamics].ToInt();
            cells.YDynamics = table[row,query.YDynamics].ToInt();
            return cells;
        }

        public static IList<List<ControlCells>> GetCells(IVisio.Page page, IList<int> shapeids)
        {
            var query = get_query();
            return VA.ShapeSheet.CellGroups.CellGroupMultiRow.CellsFromRowsGrouped(page, shapeids, query, get_cells_from_row);
        }

        public static IList<ControlCells> GetCells(IVisio.Shape shape)
        {
            var query = get_query();
            return VA.ShapeSheet.CellGroups.CellGroupMultiRow.CellsFromRows(shape, query, get_cells_from_row);
        }

        private static ControlQuery m_query;
        private static ControlQuery get_query()
        {
            if (m_query == null)
            {
                m_query = new ControlQuery();
            }
            return m_query;
        }

        class ControlQuery : VA.ShapeSheet.Query.SectionQuery
        {
            public VA.ShapeSheet.Query.QueryColumn CanGlue { get; set; }
            public VA.ShapeSheet.Query.QueryColumn Tip { get; set; }
            public VA.ShapeSheet.Query.QueryColumn X { get; set; }
            public VA.ShapeSheet.Query.QueryColumn Y { get; set; }
            public VA.ShapeSheet.Query.QueryColumn YBehavior { get; set; }
            public VA.ShapeSheet.Query.QueryColumn XBehavior { get; set; }
            public VA.ShapeSheet.Query.QueryColumn XDynamics { get; set; }
            public VA.ShapeSheet.Query.QueryColumn YDynamics { get; set; }

            public ControlQuery() :
                base(IVisio.VisSectionIndices.visSectionControls)
            {
                this.CanGlue = this.AddColumn(VA.ShapeSheet.SRCConstants.Controls_CanGlue.Cell, "CanGlue");
                this.Tip = this.AddColumn(VA.ShapeSheet.SRCConstants.Controls_Tip.Cell, "Tip");
                this.X = this.AddColumn(VA.ShapeSheet.SRCConstants.Controls_X.Cell, "X");
                this.Y = this.AddColumn(VA.ShapeSheet.SRCConstants.Controls_Y.Cell, "Y");
                this.YBehavior = this.AddColumn(VA.ShapeSheet.SRCConstants.Controls_YCon.Cell, "YBehavior");
                this.XBehavior = this.AddColumn(VA.ShapeSheet.SRCConstants.Controls_XCon.Cell, "XBehavior");
                this.XDynamics = this.AddColumn(VA.ShapeSheet.SRCConstants.Controls_XDyn.Cell, "XDynamics");
                this.YDynamics = this.AddColumn(VA.ShapeSheet.SRCConstants.Controls_YDyn.Cell, "YDynamics");
            }
        }
    }
}