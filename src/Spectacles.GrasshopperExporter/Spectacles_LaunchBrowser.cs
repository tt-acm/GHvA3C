//The MIT License (MIT)

//Copyright (c) 2015 Thornton Tomasetti

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

/*  HEADER
 * 
 *  
 *  9/10/15
 *  
 * Jonatan Schumacher
 *  
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Parameters;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Rhino.Geometry;
using Spectacles.GrasshopperExporter.Properties;

namespace Spectacles.GrasshopperExporter
{
    public class Spectacles_LaunchBrowser : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Spectacles_geometry class.
        /// </summary>
        public Spectacles_LaunchBrowser()
            : base("Launch Spectacles Viewer", "Spectacles_Viewer",
                "Launches the Spectacles Viewer in your web browser.",
                "Spectacles", "Spectacles")
        { }

        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.secondary;
            }
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("URL", "[U]", "URL to open", GH_ParamAccess.item,"http://tt-acm.github.io/Spectacles.WebViewer/");
            pManager.AddBooleanParameter("Launch Boolean", "B", "Set to True to Launch Default Web Browser.", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
           pManager.AddTextParameter("output message", "out", "Output Message", GH_ParamAccess.item);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //local variables
            string url = "";
            Boolean openBrowser = false;
            string outString = "";

            if (!DA.GetData(0, ref url)) return;
            if (!DA.GetData(1, ref openBrowser)) return;

            if (openBrowser)
            {
                try
                {
                    System.Diagnostics.Process.Start(url);
                    outString = "Spectacles Viewer has been launched in the browser.";
                }
                catch (Exception e)
                {
                    outString = e.Message.ToString();
                }
            }
            else
            {
                outString = "set the 'B' input to true!";
            }

            DA.SetData(0, outString);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Resources.SPECTACLES_browser_32px;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("EEE33D45-D2A5-4C8A-B82E-AD873F000B4A"); }
        }
    }
}
