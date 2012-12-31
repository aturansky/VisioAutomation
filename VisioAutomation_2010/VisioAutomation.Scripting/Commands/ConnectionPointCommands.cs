using System.Collections.Generic;
using System.Linq;
using VisioAutomation.Extensions;
using IVisio = Microsoft.Office.Interop.Visio;
using VA = VisioAutomation;

namespace VisioAutomation.Scripting.Commands
{
    public class ConnectionPointCommands : CommandSet
    {
        public ConnectionPointCommands(Session session) :
            base(session)
        {

        }
        /// <summary>
        /// Retrieves the connection points for elected shapes
        /// </summary>
        /// <returns></returns>
        public IDictionary<IVisio.Shape, IList<VA.Connections.ConnectionPointCells>> Get()
        {
            if (!this.Session.Selection.HasShapes())
            {
                return new Dictionary<IVisio.Shape, IList<VA.Connections.ConnectionPointCells>>();
            }

            var shapes = this.Session.Selection.GetShapes( VA.Selection.ShapesEnumeration.Flat);
            var dic = new Dictionary<IVisio.Shape, IList<VA.Connections.ConnectionPointCells>>();

            var application = this.Session.VisioApplication;
            foreach (var shape in shapes)
            {
                var cp = VA.Connections.ConnectionPointCells.GetCells(shape);
                dic[shape] = cp;
            }

            return dic;
        }

        /// <summary>
        /// Adds a connection point to the selected shapes
        /// </summary>
        /// <param name="fx"></param>
        /// <param name="fy"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IList<int> Add(
            string fx,
            string fy,
            VA.Connections.ConnectionPointType type)
        {
            if (!this.Session.Selection.HasShapes())
            {
                return new List<int>(0);
            }

            int dirx = 0;
            int diry = 0;

            var shapes = this.Session.Selection.GetShapes(VA.Selection.ShapesEnumeration.Flat);

            var indices = new List<int>(shapes.Count);

            using (var undoscope = new VA.Application.UndoScope(this.Session.VisioApplication,"Add Connection Point"))
            {
                var cp = new VA.Connections.ConnectionPointCells();
                cp.X = fx;
                cp.Y = fy;
                cp.DirX = dirx;
                cp.DirY = diry;
                cp.Type = (int) type;

                foreach (var shape in shapes)
                {

                    int index = VA.Connections.ConnectionPointHelper.Add(shape, cp);
                    indices.Add(index);
                }
            }

            return indices;
        }

        /// <summary>
        /// Deletes the connection point on the seleected shapes
        /// </summary>
        /// <param name="index"></param>
        public void Delete(int index)
        {
            if (!this.Session.Selection.HasShapes())
            {
                return;
            }

            var shapes = this.Session.Selection.GetShapes(VA.Selection.ShapesEnumeration.Flat);

            var target_shapes = from shape in shapes
                                where VA.Connections.ConnectionPointHelper.GetCount(shape) > index
                                select shape;

            using (var undoscope = new VA.Application.UndoScope(this.Session.VisioApplication, "Delete Connection Point"))
            {
                foreach (var shape in target_shapes)
                {
                    VA.Connections.ConnectionPointHelper.Delete(shape, index);
                }
            }
        }
    }
}