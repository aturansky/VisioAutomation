using System;
using System.Collections.Generic;
using System.Linq;
using VisioAutomation.Extensions;
using VisioAutomation.UserDefinedCells;
using IVisio = Microsoft.Office.Interop.Visio;
using VA = VisioAutomation;

namespace VisioAutomation.Scripting.Commands
{
    public class UserDefinedCellCommands : SessionCommands
    {
        public UserDefinedCellCommands(Session session) :
            base(session)
        {

        }
        
        public IDictionary<IVisio.Shape, IList<VA.UserDefinedCells.UserDefinedCell>> GetUserDefinedCells()
        {
            var prop_dic = new Dictionary<IVisio.Shape, IList<VA.UserDefinedCells.UserDefinedCell>>();
            if (!HasSelectedShapes())
            {
                return prop_dic;
            }

            var shapes = this.Session.Selection.EnumSelectedShapes().ToList();
            var application = Application;
            var page = application.ActivePage;
            var list_user_props = UserDefinedCellsHelper.GetUserDefinedCells(page, shapes);

            for (int i = 0; i < shapes.Count; i++)
            {
                var shape = shapes[i];
                var props = list_user_props[i];
                prop_dic[shape] = props;
            }

            return prop_dic;
        }

        public IList<bool> HasUserDefinedCell(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (!HasSelectedShapes())
            {
                return new List<bool>();
            }

            var results = (from s in this.Session.Selection.EnumSelectedShapes().ToList()
                           select UserDefinedCellsHelper.HasUserDefinedCell(s, name))
                .ToList();

            return results;
        }

        public void DeleteUserDefinedCell(string name)
        {
            if (!HasSelectedShapes())
            {
                return;
            }

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (name.Length < 1)
            {
                throw new ArgumentException("name");
            }

            var shapes = this.Session.Selection.EnumSelectedShapes().ToList();

            var application = Application;
            using (var undoscope = application.CreateUndoScope())
            {
                foreach (var shape in shapes)
                {
                    UserDefinedCellsHelper.DeleteUserDefinedCell(shape, name);
                }
            }
        }

        public void SetUserDefinedCell(VA.UserDefinedCells.UserDefinedCell userprop)
        {
            if (!HasSelectedShapes())
            {
                return;
            }

            if (userprop == null)
            {
                throw new ArgumentNullException("userprop");
            }

            var shapes = this.Session.Selection.EnumSelectedShapes().ToList();

            var application = Application;
            using (var undoscope = application.CreateUndoScope())
            {
                foreach (var shape in shapes)
                {
                    UserDefinedCellsHelper.SetUserDefinedCell(shape, userprop.Name, userprop.Value, userprop.Prompt);
                }
            }
        }

    }
}