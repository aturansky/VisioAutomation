﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;

namespace VisioPowerTools2010
{
    public partial class VPTRibbon
    {
        private void VPTRibbon_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void buttonHelp_Click_1(object sender, RibbonControlEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Hello World");

        }
    }
}
