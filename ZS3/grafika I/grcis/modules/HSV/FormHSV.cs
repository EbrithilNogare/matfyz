﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Modules
{
    public partial class FormHSV : Form
    {
        /// <summary>
        /// Associated raster module (to be notified under various conditions).
        /// </summary>
        protected IRasterModule module;

        /// <summary>
        /// If true, any of the values was changed and needs to by send to the module.
        /// </summary>
        protected bool dirty = true;

        public FormHSV(IRasterModule hModule)
        {
            module = hModule;

            InitializeComponent();
        }

        private void buttonRecompute_Click(object sender, EventArgs e)
        {
            if (module == null)
                return;

            if (dirty)
            {
                module.OnGuiWindowChanged();
                dirty = false;
            }

            module.UpdateRequest?.Invoke(module);
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            numericHue.Value = Convert.ToDecimal(0);
            textSaturation.Text = "1.0";
            textValue.Text = "1.0";
            textGamma.Text = "1.0";
            checkParallel.Checked = true;
            checkSlow.Checked = false;


            minHue.Text = "-30";
            maxHue.Text = "40";

            minSaturation.Text = "0.0";
            maxSaturation.Text = "1.0";

            minValue.Text = "0.0";
            maxValue.Text = "1.0";

            module?.OnGuiWindowChanged();
            dirty = false;
        }

        private void buttonDeactivate_Click(object sender, EventArgs e)
        {
            if (module != null)
            {
                if (dirty)
                    module.OnGuiWindowChanged();

                module.DeactivateRequest?.Invoke(module);
            }
        }

        private void FormHSV_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (module != null)
            {
                if (dirty)
                    module.OnGuiWindowChanged();

                module.OnGuiWindowClose();
            }
        }

        private void numericHue_ValueChanged(object sender, EventArgs e) => dirty = true;

        private void textSaturation_TextChanged(object sender, EventArgs e) => dirty = true;

        private void checkParallel_CheckedChanged(object sender, EventArgs e) => dirty = true;

        private void textValue_TextChanged(object sender, EventArgs e) => dirty = true;

        private void checkSlow_CheckedChanged(object sender, EventArgs e) => dirty = true;

        private void textGamma_TextChanged(object sender, EventArgs e) => dirty = true;

        private void something_changed(object sender, EventArgs e) => dirty = true;

    }
}
