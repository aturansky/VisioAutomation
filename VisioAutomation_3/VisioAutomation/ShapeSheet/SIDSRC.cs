using IVisio = Microsoft.Office.Interop.Visio;
using VA = VisioAutomation;

namespace VisioAutomation.ShapeSheet
{
    public struct SIDSRC
    {
        public short ID { get; private set; }
        public short Section { get; private set; }
        public short Row { get; private set; }
        public short Cell { get; private set; }

        public SIDSRC(
            short id,
            IVisio.VisSectionIndices section,
            IVisio.VisRowIndices row,
            IVisio.VisCellIndices cell) : this(id,(short)section,(short)row,(short)cell)
        {
        }

        public SIDSRC(
            short id,
            short section,
            short row,
            short cell) : this()
        {
            this.ID = id;
            this.Section = section;
            this.Row = row;
            this.Cell = cell;
        }

        public SIDSRC(
            short id,
            SRC src) : this(id,src.Section,src.Row,src.Cell)
        {
        }  
        
        public override string ToString()
        {
            return System.String.Format("({0},{1},{2},{3})", this.ID, this.Section, this.Row, this.Cell);
        }
    }
}