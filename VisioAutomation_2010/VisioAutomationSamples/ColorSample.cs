﻿using VA = VisioAutomation;
using IVisio = Microsoft.Office.Interop.Visio;
using VisioAutomation.Extensions;
using System.Linq;
using System.Collections.Generic;

namespace VisioAutomationSamples
{
    public static class ColorSample
    {
        public static void TEST()
        {
            var page = SampleEnvironment.Application.ActiveDocument.Pages.Add();
            page.SetSize(10, 10);
            var loader = new VA.Masters.MasterLoader();
            loader.Add("rectanglex","basic_u.vss");
            loader.Resolve(SampleEnvironment.Application.Documents);

        }

        public static void ColorGrid()
        {
            var fill_foregnd = VA.ShapeSheet.SRCConstants.FillForegnd;
            int[] colors = {
                    0x0A3B76, 0x4395D1, 0x99D9EA, 0x0D686B, 0x00A99D, 0x7ACCC8, 0x82CA9C,
                    0x74A402,
                    0xC4DF9B, 0xD9D56F, 0xFFF468, 0xFFF799, 0xFFC20E, 0xEB6119, 0xFBAF5D,
                    0xE57300, 0xC14000, 0xB82832, 0xD85171, 0xFEDFEC, 0x563F7F, 0xA186BE,
                    0xD9CFE5
                };

            const int num_cols = 5;
            const int num_rows = 5;

            var page = SampleEnvironment.Application.ActiveDocument.Pages.Add();
            page.SetSize(10, 10);

            var stencil = SampleEnvironment.Application.Documents.OpenStencil("basic_u.vss");
            var master = stencil.Masters["Rectangle"];

            var layout = new VA.Layout.Grid.GridLayout(num_cols, num_rows, new VA.Drawing.Size(1, 1), master);
            layout.Origin = new VA.Drawing.Point(0, 0);
            layout.CellSpacing = new VA.Drawing.Size(0, 0);
            layout.RowDirection = VA.Layout.Grid.RowDirection.BottomToTop;

            layout.PerformLayout();
            layout.Render(page);

            int i = 0;
            var update = new VA.ShapeSheet.Update.SIDSRCUpdate();
            foreach (var node in layout.Nodes)
            {
                var shapeid = node.ShapeID;
                int color_index = i%colors.Length;
                var color = colors[color_index];
                var formula = new VA.Drawing.ColorRGB(color).ToFormula();
                update.SetFormula(shapeid, fill_foregnd, formula);
                i++;
            }

            update.Execute(page);
            page.ResizeToFitContents(1,1);
        }
    }
}