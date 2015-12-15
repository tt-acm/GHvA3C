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

//using Spectacles.GrasshopperExporter.Properties;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using Newtonsoft.Json;
using Spectacles.GrasshopperExporter.Properties;


namespace Spectacles.GrasshopperExporter
{
    public class Spectacles_MeshBasicMaterial : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Spectacles_MeshBasicMaterial class.
        /// </summary>
        public Spectacles_MeshBasicMaterial()
            : base("Spectacles_MeshBasicMaterial", "Spectacles_MeshBasicMaterial",
                "Creates mesh material that will always be the same color in a THREE.js scene - it will not be effected by lighting.",
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
            pManager.AddColourParameter("Color", "C", "Diffuse color of the material", GH_ParamAccess.item);
            pManager.AddNumberParameter("[Opacity]", "[O]", "Number in the range of 0.0 - 1.0 indicating how transparent the material is. A value of 0.0 indicates fully transparent, 1.0 is fully opaque.", GH_ParamAccess.item, 1.0);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Mesh Material", "Mm", "Mesh Material.  Feed this into the Spectacles Mesh component.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //local varaibles
            GH_Colour inColor = null;
            Double inOpacity = 1;
            string outMaterial = null;

            //catch inputs and populate local variables
            if (!DA.GetData(0, ref inColor)) { return; }
            if (inColor == null) { return; }
            DA.GetData(1, ref inOpacity);
            if (inOpacity > 1 || inOpacity < 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The opacity input must be between 0 and 1, and has been defaulted back to 1.  Check your 'O' input.");
                inOpacity = 1.0;
            }

            outMaterial = CreateMaterial(inColor, inOpacity);
            Material material = new Material(outMaterial, SpectaclesMaterialType.Mesh);

            //set the output - build up a basic material json string
            DA.SetData(0, material);
        }

        private string CreateMaterial(GH_Colour inColor, double inOpacity)
        {
            dynamic jason = new ExpandoObject();
            jason.uuid = Guid.NewGuid();
            jason.type = "MeshBasicMaterial";
            jason.color = _Utilities.hexColor(inColor);
            jason.side = 2;
            
            jason.transparent = true;
            jason.opacity = inOpacity;
            
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
                return Resources.MESH_BASIC;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{54fcfed8-affa-4d93-8320-aa49d2ca7c6d}"); }
        }
    }
}