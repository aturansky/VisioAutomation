﻿using System;
using System.Collections.Generic;
using System.Linq;
using VA = VisioAutomation;
using IVisio = Microsoft.Office.Interop.Visio;
using VisioAutomation.Extensions;

namespace VisioAutomation.Layout
{
    public static class LayoutHelper
    {
        private static double GetPosition(VA.Layout.XFormCells xform, XFormPosition pos)
        {
            if (pos == XFormPosition.PinY)
            {
                return xform.PinY.Result;
            }
            else if (pos == XFormPosition.PinX)
            {
               return xform.PinX.Result;
            }
            else if (pos == XFormPosition.Left)
            {
                return xform.Rect.Left;
            }
            else if (pos == XFormPosition.Right)
            {
                return xform.Rect.Right;
            }
            else if (pos == XFormPosition.Top)
            {
                return xform.Rect.Top;
            }
            else if (pos == XFormPosition.Right)
            {
                return xform.Rect.Bottom;
            }
            else
            {
                throw new System.ArgumentOutOfRangeException("pos");
            }
        }

        public static IList<int> SortShapesByPosition(IVisio.Page page,
                                                 IList<int> shapeids,
                                                 XFormPosition pos)
        {
            if (page == null)
            {
                throw new System.ArgumentNullException("page");
            }

            if (shapeids == null)
            {
                throw new System.ArgumentNullException("shapeids");
            }

            // First get the transforms of the shapes on the given axis
            var xforms = VA.Layout.LayoutHelper.GetXForm(page, shapeids);

            // Then, sort the shapeids pased on the corresponding value in the results
            var sorted_shape_ids = VA.Internal.CollectionUtil.GetSortedItemsIndexed(shapeids, i=> GetPosition(xforms[i],pos), (a, b) => a.CompareTo(b)).ToList();

            return sorted_shape_ids;
        }

        public static IList<short> DrawGrid(
            IVisio.Page page,
            IVisio.Master masterobj,
            VA.Drawing.Size cell_size,
            int cols,
            int rows)
        {
            var total_grid_size = cell_size.Multiply(cols, rows);
            var grid_origin = new VA.Drawing.Point(0, total_grid_size.Height);
            var cellspacing = new VA.Drawing.Size(0, 0);

            //Create a new page to hold the grid

            var layout = new VA.Layout.Grid.GridLayout(cols, rows, cell_size, masterobj);
            layout.PerformLayout(grid_origin, cellspacing);
            layout.RowDirection = VA.Layout.Grid.RowDirection.TopToBottom;

            layout.Render(page);

            var shapeids = layout.Nodes.Select(n => n.ShapeID).ToList();
            return shapeids;
        }

        public static IList<IVisio.Shape> DrawPieSlices(IVisio.Page page, VA.Drawing.Point center,
                                                        double radius,
                                                        IList<double> values)
        {
            double sum = values.Sum();
            var shapes = new List<IVisio.Shape>();
            double start_angle = 0;

            foreach (int i in Enumerable.Range(0, values.Count))
            {
                double cur_val = values[i];
                double cur_val_norm = cur_val/sum;
                double cur_angle_size_deg = cur_val_norm*360;
                double end_angle = start_angle + cur_angle_size_deg;
                var shape = DrawPieSlice(page, center, radius, start_angle, end_angle);
                start_angle += cur_angle_size_deg;

                shapes.Add(shape);
            }

            return shapes;
        }

        public static IVisio.Shape DrawPieSlice(
            IVisio.Page page,
            VA.Drawing.Point center,
            double radius,
            double start_angle,
            double end_angle)
        {
            double total_angle = end_angle - start_angle;

            if (total_angle == 0.0)
            {
                return page.DrawLine(center, GetPointAtRadius(center, start_angle, radius));
            }
            else if (total_angle >= 360)
            {
                var A = center.Add(-radius, -radius);
                var B = center.Add(radius, radius);
                var rect = new VA.Drawing.Rectangle(A, B);
                var shape = page.DrawOval(rect);
                return shape;
            }
            else
            {
                int degree;
                var sub_arcs = VA.Drawing.BezierSegment.FromArc(
                    Convert.DegreesToRadians(start_angle),
                    Convert.DegreesToRadians(end_angle));

                var arc_bez_points = (from p in VA.Drawing.BezierSegment.Merge(sub_arcs, out degree)
                                      select p.Multiply(radius) + center).ToList();

                var pie_points = new List<VA.Drawing.Point>();
                pie_points.Add(center);
                pie_points.Add(center);
                pie_points.Add(arc_bez_points[0]);
                pie_points.AddRange(arc_bez_points);
                pie_points.Add(arc_bez_points[arc_bez_points.Count - 1]);
                pie_points.Add(center);
                pie_points.Add(center);

                var doubles_array = VA.Drawing.DrawingUtil.PointsToDoubles(pie_points).ToArray();
                var pie_slice = page.DrawBezier(doubles_array, (short) degree, 0);
                return pie_slice;
            }
        }

        private static VA.Drawing.Point GetPointAtRadius(VA.Drawing.Point origin, double angle, double radius)
        {
            var new_point = new VA.Drawing.Point(radius*System.Math.Cos(angle),
                                      radius*System.Math.Sin(angle));
            new_point = origin + new_point;
            return new_point;
        }

        public static void DistributeWithSpacing(IVisio.Page page,
                                     IList<int> shapeids,
                                     VA.Drawing.Axis axis,
                                     double spacing)
        {
            if (page == null)
            {
                throw new System.ArgumentNullException("page");
            }

            if (shapeids == null)
            {
                throw new System.ArgumentNullException("shapeids");
            }

            if (spacing < 0.0)
            {
                throw new System.ArgumentOutOfRangeException("spacing");
            }

            if (shapeids.Count < 2)
            {
                return;
            }

            var sortpos = axis == VA.Drawing.Axis.XAxis
                              ? VA.Layout.XFormPosition.PinX
                              : VA.Layout.XFormPosition.PinY;

            var delta = axis == VA.Drawing.Axis.XAxis
                            ? new VA.Drawing.Size(spacing, 0)
                            : new VA.Drawing.Size(0, spacing);


            var sorted_shape_ids = VA.Layout.LayoutHelper.SortShapesByPosition(page, shapeids, sortpos);
            var xfrms = VA.Layout.LayoutHelper.GetXForm(page, sorted_shape_ids); ;
            var bb = GetBoundingBox(xfrms);
            var cur_pos = new VA.Drawing.Point(bb.Left, bb.Bottom);

            var update = new VA.ShapeSheet.Update.SIDSRCUpdate();

            for (int i = 0; i < sorted_shape_ids.Count; i++)
            {
                var shape_id = sorted_shape_ids[i];
                var xfrm = xfrms[i];

                var new_pinpos = axis == VA.Drawing.Axis.XAxis
                                     ? new VA.Drawing.Point(cur_pos.X + xfrm.LocPinX.Result, xfrm.PinY.Result)
                                     : new VA.Drawing.Point(xfrm.PinX.Result, cur_pos.Y + xfrm.LocPinY.Result);

                update.SetFormula((short)shape_id, VA.ShapeSheet.SRCConstants.PinX, new_pinpos.X);
                update.SetFormula((short)shape_id, VA.ShapeSheet.SRCConstants.PinY, new_pinpos.Y);
                cur_pos = cur_pos.Add(xfrm.Size).Add(delta);
            }

            update.Execute(page);
        }

        public static VA.Drawing.Rectangle GetBoundingBox(IEnumerable<VA.Layout.XFormCells> xfrms)
        {
            var bb = VA.Drawing.DrawingUtil.TryGetBoundingBox(xfrms.Select(i => i.Rect));
            if (!bb.HasValue)
            {
                throw new System.ArgumentException("Could not calculate bounding box");
            }
            else
            {
                return bb.Value;
            }
        }

        public static void SnapCorner(IVisio.Page page,
                              IList<int> shapeids,
                              VA.Drawing.Size snapsize,
                              SnapCornerPosition corner)
        {
            var layout_info = VA.Layout.LayoutHelper.GetXForm(page, shapeids);
            var update = new VA.ShapeSheet.Update.SIDSRCUpdate();

            foreach (int i in Enumerable.Range(0, shapeids.Count))
            {
                var shapeid = shapeids[i];
                var old_layout = layout_info[i];
                var old_bb = old_layout.Rect;
                var new_corner_pos = VA.Drawing.DrawingUtil.Round(
                    old_bb.LowerLeft,
                    snapsize.Width,
                    snapsize.Height);
                var new_pin_position = GetPinPositionForCorner(
                    old_layout.Pin,
                    old_layout.Size,
                    old_layout.LocPin,
                    new_corner_pos,
                    corner);

                if (new_pin_position.X != old_layout.PinX.Result)
                {
                    update.SetFormula((short)shapeid, VA.ShapeSheet.SRCConstants.PinX, new_pin_position.X);
                }

                if (new_pin_position.Y != old_layout.PinY.Result)
                {
                    update.SetFormula((short)shapeid, VA.ShapeSheet.SRCConstants.PinY, new_pin_position.Y);
                }
            }

            update.Execute(page);
        }

        private static VA.Drawing.Point GetPinPositionForCorner(
            VA.Drawing.Point pinpos,
            VA.Drawing.Size size,
            VA.Drawing.Point locpin,
            VA.Drawing.Point new_corner_pos,
            SnapCornerPosition corner)
        {
            switch (corner)
            {
                case SnapCornerPosition.LowerLeft:
                    {
                        return new_corner_pos.Add(locpin.X, locpin.Y);
                    }
                case SnapCornerPosition.UpperRight:
                    {
                        return new_corner_pos.Subtract(size.Width, size.Height).Add(locpin.X, locpin.Y);
                    }
                case SnapCornerPosition.LowerRight:
                    {
                        return new_corner_pos.Subtract(size.Width, 0).Add(locpin.X, locpin.Y);
                    }
                case SnapCornerPosition.UpperLeft:
                    {
                        return new_corner_pos.Subtract(0, size.Height).Add(locpin.X, locpin.Y);
                    }
                default:
                    {
                        throw new System.ArgumentOutOfRangeException("corner", "Unsupported corner");
                    }
            }
        }

        public static void SnapSize(IVisio.Page page, IList<int> shapeids, VA.Drawing.Size snapsize, VA.Drawing.Size minsize)
        {
            var layout_info = VA.Layout.LayoutHelper.GetXForm(page, shapeids);
            var update = new VA.ShapeSheet.Update.SIDSRCUpdate();

            for (int i = 0; i < shapeids.Count; i++)
            {
                int shapeid = shapeids[i];
                var old_size = layout_info[i].Size;
                var new_size = VA.Drawing.DrawingUtil.Max(VA.Drawing.DrawingUtil.SnapToNearestValue(old_size, snapsize), minsize);

                update.SetFormula((short)shapeid, VA.ShapeSheet.SRCConstants.Width, new_size.Width);
                update.SetFormula((short)shapeid, VA.ShapeSheet.SRCConstants.Height, new_size.Height);
            }

            update.Execute(page);
        }

        public static void AlignTo(IVisio.Page page, IList<int> shapeids, VA.Drawing.AlignmentHorizontal align, double x)
        {
            var xfrms = VA.Layout.LayoutHelper.GetXForm(page, shapeids);
            var update = new VA.ShapeSheet.Update.SIDSRCUpdate();

            for (int i = 0; i < shapeids.Count; i++)
            {
                var info = xfrms[i];
                double nx = 0.0;
                if (align == VA.Drawing.AlignmentHorizontal.Left)
                {
                    nx = x + info.LocPinX.Result;
                }
                else if (align == VA.Drawing.AlignmentHorizontal.Center)
                {
                    nx = x + info.LocPinX.Result - (info.Size.Width / 2.0);
                }
                else if (align == VA.Drawing.AlignmentHorizontal.Right)
                {
                    nx = x + info.LocPinX.Result - info.Size.Width;
                }
                update.SetFormula((short)shapeids[i], VA.ShapeSheet.SRCConstants.PinX, nx);

            }

            update.Execute(page);
        }

        public static void AlignTo(IVisio.Page page, IList<int> shapeids, VA.Drawing.AlignmentVertical align, double y)
        {
            var xfrms = VA.Layout.LayoutHelper.GetXForm(page, shapeids);
            var update = new VA.ShapeSheet.Update.SIDSRCUpdate();

            for (int i = 0; i < shapeids.Count; i++)
            {
                var info = xfrms[i];
                double ny = 0.0;

                if (align == VA.Drawing.AlignmentVertical.Top)
                {
                    ny = y + info.LocPinY.Result - info.Size.Height;
                }
                else if (align == VA.Drawing.AlignmentVertical.Center)
                {
                    ny = y + info.LocPinY.Result - (info.Size.Height / 2.0);
                }
                else if (align == VA.Drawing.AlignmentVertical.Bottom)
                {
                    ny = y + info.LocPinY.Result;
                }

                update.SetFormula((short)shapeids[i], VA.ShapeSheet.SRCConstants.PinY, ny);
            }

            update.Execute(page);
        }


        public static VA.Layout.XFormCells GetXForm(IVisio.Shape shape)
        {
            if (shape == null)
            {
                throw new System.ArgumentNullException("shape");
            }

            var query = new VA.Layout.XFormQuery();
            var qds = query.GetFormulasAndResults<double>(shape);
            var xfrm = get_xformdata_from_row(query, qds, 0);

            return xfrm;
        }

        public static IList<VA.Layout.XFormCells> GetXForm(IVisio.Page page, IList<int> shapeids)
        {
            if (page == null)
            {
                throw new System.ArgumentNullException("page");
            }

            var query = new VA.Layout.XFormQuery();
            var qds = query.GetFormulasAndResults<double>(page, shapeids);
            var xfrms = new List<VA.Layout.XFormCells>(shapeids.Count);
            for (int i = 0; i < qds.RowCount; i++)
            {
                var xfrm = get_xformdata_from_row(query, qds, i);
                xfrms.Add(xfrm);
            }

            return xfrms;
        }

        private static VA.Layout.XFormCells get_xformdata_from_row(VA.Layout.XFormQuery query,
                                                              VA.ShapeSheet.Query.QueryDataSet<double> qds, int row)
        {
            var xfrm = new VA.Layout.XFormCells();
            xfrm.PinX = qds.GetItem(row, query.PinX);
            xfrm.PinY = qds.GetItem(row, query.PinY);
            xfrm.LocPinX = qds.GetItem(row, query.LocPinX);
            xfrm.LocPinY = qds.GetItem(row, query.LocPinY);
            xfrm.Width = qds.GetItem(row, query.Width);
            xfrm.Height = qds.GetItem(row, query.Height);
            xfrm.Angle = qds.GetItem(row, query.Angle);
            return xfrm;
        }
    }
}