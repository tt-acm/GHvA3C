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
    public class Spectacles_Material_ARCHIVE_20141116 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Spectacles_material class.
        /// </summary>
        public Spectacles_Material_ARCHIVE_20141116()
            : base("Spectacles_Material", "Spectacles_Material", "Create a Spectacles mesh material to apply to Spectacles meshes.", "Spectacles", "materials")
        {
        }

        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.hidden;
            }
        }

        
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddColourParameter("Color", "C", "Material Color", GH_ParamAccess.item);
            pManager.AddNumberParameter("[Opacity]", "[O]", "Material Opacity", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddTextParameter("[Name]", "[N]", "Material Name", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }


        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Mesh Material", "Mm", "Mesh Material JSON representation.  Feed this into the Scene Compiler component.", GH_ParamAccess.item);
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //System.Drawing.Color inColor = System.Drawing.Color.White;
            GH_Colour inColor = null;
            Double inOpacity = 1;
            String inName = String.Empty;
            String outMaterial = null;
            String outName = null;

            //if (!DA.GetData(0, ref inColor)) { return; }
            if (!DA.GetData(0, ref inColor)) { return; }
            if (inColor == null) { return; }
            DA.GetData(1, ref inOpacity);
            DA.GetData(2, ref inName);

            if (inName == string.Empty) { inName = DateTime.Now.ToShortDateString(); }      //autogenerate name
            outName = inName;
            outMaterial = ConstructMaterial(inColor, inOpacity, inName);
            //call json conversion function
            
            DA.SetData(0, outMaterial);
        }


        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:

                //return  Resources.MatIcon;
                //return Resources.Spectacles_yellow;
                return null;
            }
        }


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{afb20e86-13f1-4083-89e4-282299763f4c}"); }
        }


        public string ConstructMaterial(GH_Colour Col, Double Opp, String Name)
        {
            dynamic JsonMat = new ExpandoObject();
            //JsonMat.metadata = new ExpandoObject();
            //JsonMat.metadata.version = 4.2;
            //JsonMat.metadata.type = "material";
            //JsonMat.metadata.generator = "MaterialExporter";

            JsonMat.uuid = Guid.NewGuid();
            JsonMat.type = "MeshPhongMaterial";
            JsonMat.color = _Utilities.hexColor(Col);
            JsonMat.ambient = _Utilities.hexColor(Col);
            JsonMat.emissive = _Utilities.hexColor(new GH_Colour(System.Drawing.Color.Black));
            JsonMat.specular = _Utilities.hexColor(new GH_Colour(System.Drawing.Color.Gray)); 
            JsonMat.shininess = 50;
            JsonMat.opacity = Opp;
            JsonMat.transparent = false;
            JsonMat.wireframe = false;
            JsonMat.side = 2;
            return JsonConvert.SerializeObject(JsonMat);
        }
    }
}