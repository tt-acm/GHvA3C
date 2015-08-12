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

using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Timers;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Spectacles.GrasshopperExporter.Properties;

using Newtonsoft.Json;

namespace Spectacles.GrasshopperExporter
{
    public class Spectacles_Line : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Spectacles_Line class.
        /// </summary>
        public Spectacles_Line()
            : base("Spectacles_Line", "Spectacles_Line",
                "Creates a Spectacles line",
                "Spectacles", "geometry")
        {
        }

        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.primary;
            }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddLineParameter("Line", "L", "A line to convert into a Spectacles JSON representation of the line", GH_ParamAccess.item);
            pManager.AddGenericParameter("Line Material", "Lm", "Line Material", GH_ParamAccess.item);
            pManager.AddTextParameter("Layer", "[L]", "Layer", GH_ParamAccess.item);
            pManager[2].Optional = true;

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Line Element", "Le", "Line element output to feed into the scene compiler component", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //local variables
            GH_Line line = null;
            Material material = null;
            Layer layer = null;
            string layerName = "Default";
            //catch inputs and populate local variables
            if (!DA.GetData(0, ref line))
            {
                return;
            }

            if (!DA.GetData(1, ref material))
            {
                return;
            }

            if (material.Type != SpectaclesMaterialType.Line)
            {
                throw new Exception("Please use a LINE Material");
            }

            DA.GetData(2, ref layerName);

            layer = new Layer(layerName);


            //create JSON from line
            string outJSON = lineJSON(line.Value);


            Element e = new Element(outJSON, SpectaclesElementType.Line, material, layer);

            //output results
            DA.SetData(0, e);

        }

        private string lineJSON(Line line)
        {
            //create a dynamic object to populate
            dynamic jason = new ExpandoObject();

            //top level properties
            jason.uuid = Guid.NewGuid();
            jason.type = "Geometry";
            jason.data = new ExpandoObject();

            //populate data object properties
            jason.data.vertices = new object[6];
            jason.data.vertices[0] = Math.Round(line.FromX * -1.0, 5);
            jason.data.vertices[1] = Math.Round(line.FromZ, 5);
            jason.data.vertices[2] = Math.Round(line.FromY, 5);
            jason.data.vertices[3] = Math.Round(line.ToX * -1.0, 5);
            jason.data.vertices[4] = Math.Round(line.ToZ, 5);
            jason.data.vertices[5] = Math.Round(line.ToY, 5);
            jason.data.normals = new object[0];
            jason.data.uvs = new object[0];
            jason.data.faces = new object[0];
            jason.data.scale = 1;
            jason.data.visible = true;
            jason.data.castShadow = true;
            jason.data.receiveShadow = false;


            //return
            return JsonConvert.SerializeObject(jason);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                return Resources.LINE;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{7705c130-3743-4243-b421-3e709531f189}"); }
        }
    }
}