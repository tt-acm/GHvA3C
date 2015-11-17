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
using System.Collections.Generic;
using System.Dynamic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using Newtonsoft.Json;
using Spectacles.GrasshopperExporter.Properties;

namespace Spectacles.GrasshopperExporter
{
    public class Spectacles_LineBasicMaterial : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Spectacles_LineBasicMaterial class.
        /// </summary>
        public Spectacles_LineBasicMaterial()
            : base("Spectacles_LineBasicMaterial", "Spectacles_LineBasicMaterial",
                "Creates a THREE.js Basic Line Material to use with line geometries",
                "TT Toolbox", "Spectacles")
        {
        }

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
            pManager.AddColourParameter("Color", "C", "Material Color", GH_ParamAccess.item);
            pManager.AddNumberParameter("LineWeight", "LW", "The thickness, in pixels, of the line material.  Not supported yet.", GH_ParamAccess.item, 1.0);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Line Material", "Lm", "Line Material.  Feed this into the Spectacles Line component.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //loacl varaibles
            GH_Colour inColor = null;
            GH_Number inNumber = new GH_Number(1.0);

            //get user data
            if (!DA.GetData(0, ref inColor))
            {
                return;
            }
            DA.GetData(1, ref inNumber);

            //spin up a JSON material from the inputs
            string outJSON = ConstructMaterial(inColor, inNumber);

            Material material = new Material(outJSON, SpectaclesMaterialType.Line);
            
            //output
            DA.SetData(0, material);
            

        }

        private string ConstructMaterial(GH_Colour inColor, GH_Number inNumber)
        {
            //json object to populate
            dynamic jason = new ExpandoObject();

            //JSON properties
            jason.uuid = Guid.NewGuid();
            jason.type = "LineBasicMaterial";
            jason.color = _Utilities.hexColor(inColor);
            jason.linewidth = inNumber.Value;
            jason.opacity = 1;


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
                return Resources.LINE_MAT;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{08689df0-81f6-4946-b067-122a12bb3a78}"); }
        }
    }
}