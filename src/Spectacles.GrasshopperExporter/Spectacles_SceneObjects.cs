using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using Newtonsoft.Json;

namespace Spectacles.GrasshopperExporter
{
    public class Spectacles_SceneObjects : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Spectacles_SceneObjects class.
        /// </summary>
        public Spectacles_SceneObjects()
          : base("Spectacles_SceneObjects", "SceneObjects",
              "Compiles Spectacles objects into a JSON representation of a THREE.js scene, which can be opened using the Spectacles viewer.",
                "TT Toolbox", "Spectacles")
        {
        }

        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.quarternary;
            }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Elements", "E", "Spectacles Elements to add to the scene.", GH_ParamAccess.list);
            pManager[0].DataMapping = GH_DataMapping.Flatten;
            pManager[0].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("3D Objects", "3DObjects", "3d element output to feed into Aggregator component", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            List<Element> inElements = new List<Element>();
            DA.GetDataList(0, inElements);
            

            #region Input management
            
            List<GH_String> inMeshGeometry = new List<GH_String>();
            List<GH_String> inLineGeometry = new List<GH_String>();
            List<GH_String> inViews = new List<GH_String>();

            List<GH_String> inMeshMaterial = new List<GH_String>();
            List<GH_String> inLineMaterial = new List<GH_String>();

            List<GH_String> inMeshLayer = new List<GH_String>();
            List<GH_String> inLineLayer = new List<GH_String>();

            Dictionary<string, List<Element>> definitionLayers = new Dictionary<string, List<Element>>();

            foreach (Element e in inElements)
            {
                if (e == null) continue;
                GH_String g = new GH_String();
                g.Value = e.GeometryJson;

                if (e.Type != SpectaclesElementType.Camera)
                {
                    GH_String m = new GH_String();
                    m.Value = e.Material.MaterialJson;

                    string layerName = "";
                    if (e.Layer == null) layerName = "Default";
                    else layerName = e.Layer.Name;

                    GH_String l = new GH_String();
                    l.Value = layerName;


                    if (e.Type == SpectaclesElementType.Mesh)
                    {
                        inMeshGeometry.Add(g);
                        inMeshMaterial.Add(m);
                        inMeshLayer.Add(l);
                    }

                    if (e.Type == SpectaclesElementType.Line)
                    {
                        inLineGeometry.Add(g);
                        inLineMaterial.Add(m);
                        inLineLayer.Add(l);
                    }



                    if (!definitionLayers.Keys.Contains(layerName))
                    {
                        List<Element> layerElements = new List<Element>();
                        definitionLayers.Add(layerName, layerElements);
                    }

                    definitionLayers[layerName].Add(e);


                }
                else
                {
                    inViews.Add(g);
                }
            }

            #endregion

            
            if (inElements.Any())
            {
                var outJSONObjects = sceneJSONObjects(inMeshGeometry, inMeshMaterial, inMeshLayer, inLineGeometry, inLineMaterial, inLineLayer, inViews, definitionLayers);
                //var outParam = new threeDParam(outJSONObjects);
                DA.SetData(0, outJSONObjects);
            }
            else
            {
                DA.SetData(0, new threeDParam());
            }
            
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
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{d4e3c4eb-d082-4c4d-a9be-cd7dda9d56f2}"); }
        }

        private object sceneJSONObjects(List<GH_String> meshList, List<GH_String> meshMaterialList, List<GH_String> meshLayerList, List<GH_String> linesList, List<GH_String> linesMaterialList, List<GH_String> lineLayerList, List<GH_String> viewList, Dictionary<string, List<Element>> defLayers)
        {

            //create a dynamic object to populate
            dynamic jason = new ExpandoObject();

            //populate metadata object
            jason.metadata = new ExpandoObject();
            jason.metadata.version = 4.3;
            jason.metadata.type = "Object";
            jason.metadata.generator = "Spectacles_Grasshopper_Exporter";

            int size = meshList.Count + linesList.Count;

            //populate mesh geometries:
            jason.geometries = new object[size];   //array for geometry - both lines and meshes
            jason.materials = new object[size];  //array for materials - both lines and meshes


            #region Mesh management
            int meshCounter = 0;
            Dictionary<string, object> MeshDict = new Dictionary<string, object>();
            Dictionary<string, SpectaclesAttributesCatcher> attrDict = new Dictionary<string, SpectaclesAttributesCatcher>();


            foreach (GH_String m in meshList)
            {
                bool alreadyExists = false;
                //deserialize the geometry and attributes, and add them to our object
                SpectaclesGeometryCatcher c = JsonConvert.DeserializeObject<SpectaclesGeometryCatcher>(m.Value);
                SpectaclesAttributesCatcher ac = JsonConvert.DeserializeObject<SpectaclesAttributesCatcher>(m.Value);
                jason.geometries[meshCounter] = c;
                attrDict.Add(c.uuid, ac);


                //now that we have different types of materials, we need to know which catcher to call
                //use the SpectaclesBaseMaterialCatcher class to determine a material's type, then call the appropriate catcher
                //object mc;
                SpectaclesBaseMaterialCatcher baseCatcher = JsonConvert.DeserializeObject<SpectaclesBaseMaterialCatcher>(meshMaterialList[meshCounter].Value);
                if (baseCatcher.type == "MeshFaceMaterial")
                {
                    SpectaclesMeshFaceMaterialCatcher mc = JsonConvert.DeserializeObject<SpectaclesMeshFaceMaterialCatcher>(meshMaterialList[meshCounter].Value);

                    foreach (var existingMaterial in jason.materials)
                    {
                        try
                        {
                            if (existingMaterial.type == "MeshFaceMaterial")
                            {
                                //check if all the properties match a material that already exists
                                if (mc.materials == existingMaterial.materials)
                                {
                                    mc.uuid = existingMaterial.uuid;
                                    alreadyExists = true;
                                    break;
                                }
                            }
                        }
                        catch { }
                    }
                    //only add it if it does not exist
                    if (!alreadyExists) jason.materials[meshCounter] = mc;
                    MeshDict.Add(c.uuid, mc.uuid);
                }
                if (baseCatcher.type == "MeshPhongMaterial")
                {
                    SpectaclesMeshPhongMaterialCatcher mc = JsonConvert.DeserializeObject<SpectaclesMeshPhongMaterialCatcher>(meshMaterialList[meshCounter].Value);

                    foreach (var existingMaterial in jason.materials)
                    {
                        try
                        {
                            if (existingMaterial.type == "MeshPhongMaterial")
                            {
                                //check if all the properties match a material that already exists
                                if (mc.color == existingMaterial.color && mc.ambient == existingMaterial.ambient && mc.emissive == existingMaterial.emissive
                                     && mc.side == existingMaterial.side && mc.opacity == existingMaterial.opacity && mc.shininess == existingMaterial.shininess
                                    && mc.specular == existingMaterial.specular && mc.transparent == existingMaterial.transparent && mc.wireframe == existingMaterial.wireframe)
                                {
                                    mc.uuid = existingMaterial.uuid;
                                    alreadyExists = true;
                                    break;
                                }
                            }
                        }
                        catch { }
                    }
                    //only add it if it does not exist
                    if (!alreadyExists) jason.materials[meshCounter] = mc;


                    MeshDict.Add(c.uuid, mc.uuid);
                }
                if (baseCatcher.type == "MeshLambertMaterial")
                {
                    SpectaclesMeshLambertMaterialCatcher mc = JsonConvert.DeserializeObject<SpectaclesMeshLambertMaterialCatcher>(meshMaterialList[meshCounter].Value);

                    foreach (var existingMaterial in jason.materials)
                    {
                        try
                        {
                            if (existingMaterial.type == "MeshLambertMaterial")
                            {
                                //check if all the properties match a material that already exists
                                if (mc.color == existingMaterial.color && mc.ambient == existingMaterial.ambient && mc.emissive == existingMaterial.emissive
                                    && mc.side == existingMaterial.side && mc.opacity == existingMaterial.opacity && mc.shading == existingMaterial.shading)
                                {
                                    mc.uuid = existingMaterial.uuid;
                                    alreadyExists = true;
                                    break;
                                }
                            }
                        }
                        catch
                        { }
                    }
                    //only add it if it does not exist
                    if (!alreadyExists) jason.materials[meshCounter] = mc;
                    MeshDict.Add(c.uuid, mc.uuid);
                }
                if (baseCatcher.type == "MeshBasicMaterial")
                {
                    SpectaclesMeshBasicMaterialCatcher mc = JsonConvert.DeserializeObject<SpectaclesMeshBasicMaterialCatcher>(meshMaterialList[meshCounter].Value);

                    foreach (var existingMaterial in jason.materials)
                    {
                        try
                        {
                            if (existingMaterial.type == "MeshBasicMaterial")
                            {
                                //check if all the properties match a material that already exists
                                if (
                                    mc.color == existingMaterial.color && mc.transparent == existingMaterial.transparent
                                    && mc.side == existingMaterial.side && mc.opacity == existingMaterial.opacity)
                                {
                                    mc.uuid = existingMaterial.uuid;
                                    alreadyExists = true;
                                    break;
                                }
                            }
                        }
                        catch
                        { }
                    }
                    //only add it if it does not exist
                    if (!alreadyExists) jason.materials[meshCounter] = mc;
                    MeshDict.Add(c.uuid, mc.uuid);
                }
                meshCounter++;

            }



            #endregion

            #region Line management
            //populate line geometries
            int lineCounter = meshCounter;
            int lineMaterialCounter = 0;
            Dictionary<string, object> LineDict = new Dictionary<string, object>();
            foreach (GH_String l in linesList)
            {
                bool alreadyExists = false;
                //deserialize the line and the material
                SpectaclesLineCatcher lc = JsonConvert.DeserializeObject<SpectaclesLineCatcher>(l.Value);
                SpectaclesLineBasicMaterialCatcher lmc = JsonConvert.DeserializeObject<SpectaclesLineBasicMaterialCatcher>(linesMaterialList[lineMaterialCounter].Value);
                //add the deserialized values to the jason object
                jason.geometries[lineCounter] = lc;


                foreach (var existingMaterial in jason.materials)
                {
                    try
                    {
                        if (existingMaterial.type == "LineBasicMaterial")
                        {
                            //check if all the properties match a material that already exists
                            if (
                                lmc.color == existingMaterial.color && lmc.linewidth == existingMaterial.linewidth
                                 && lmc.opacity == existingMaterial.opacity)
                            {
                                lmc.uuid = existingMaterial.uuid;
                                alreadyExists = true;
                                break;
                            }
                        }
                    }
                    catch
                    { }
                }
                //only add it if it does not exist
                if (!alreadyExists) jason.materials[meshCounter + lineMaterialCounter] = lmc;

                //populate dict to match up materials and lines
                LineDict.Add(lc.uuid, lmc.uuid);

                //increment counters
                lineCounter++;
                lineMaterialCounter++;
            }
            #endregion


            //make a new array that has the correct size according to the number of materials in the scene
            object[] myMaterials = jason.materials;
            myMaterials = myMaterials.Where(mat => mat != null).ToArray();
            jason.materials = myMaterials;

            #region Camera management
            //populate line geometries
            int viewCounter = 0;

            Dictionary<string, List<object>> viewDict = new Dictionary<string, List<object>>();
            foreach (GH_String l in viewList)
            {
                //deserialize the line and the material
                SpectaclesCameraCatcher lc = JsonConvert.DeserializeObject<SpectaclesCameraCatcher>(l.Value);

                List<object> viewSettings = new List<object>();
                viewSettings.Add(lc.eye);
                viewSettings.Add(lc.target);

                viewDict.Add(lc.name, viewSettings);

                //increment counters
                viewCounter++;

            }
            #endregion

            jason.OOO = new ExpandoObject();
            //create scene:
            jason.OOO.uuid = System.Guid.NewGuid();
            jason.OOO.type = "Scene";
            int[] numbers = new int[16] { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 };
            jason.OOO.matrix = numbers;
            jason.OOO.children = new object[meshList.Count + linesList.Count];
            jason.OOO.userData = new ExpandoObject();



            //create childern
            //loop over meshes and lines
            int i = 0;
            foreach (var g in MeshDict.Keys) //meshes
            {
                jason.OOO.children[i] = new ExpandoObject();
                jason.OOO.children[i].uuid = Guid.NewGuid();
                jason.OOO.children[i].name = "mesh" + i.ToString();
                jason.OOO.children[i].type = "Mesh";
                jason.OOO.children[i].geometry = g;
                jason.OOO.children[i].material = MeshDict[g];
                jason.OOO.children[i].matrix = numbers;
                jason.OOO.children[i].userData = attrDict[g].userData;
                i++;
            }
            int lineCount = 0;
            foreach (var l in LineDict.Keys)
            {
                jason.OOO.children[i] = new ExpandoObject();
                jason.OOO.children[i].uuid = Guid.NewGuid();
                jason.OOO.children[i].name = "line " + i.ToString();
                jason.OOO.children[i].type = "Line";
                jason.OOO.children[i].geometry = l;
                jason.OOO.children[i].material = LineDict[l];
                jason.OOO.children[i].matrix = numbers;
                jason.OOO.children[i].userData = new ExpandoObject();
                jason.OOO.children[i].userData.layer = lineLayerList[lineCount].Value;
                i++;
                lineCount++;
            }

            jason.OOO.userData.views = new object[viewList.Count];
            int j = 0;
            foreach (var n in viewDict.Keys)
            {
                jason.OOO.userData.views[j] = new ExpandoObject();
                jason.OOO.userData.views[j].name = n;
                jason.OOO.userData.views[j].eye = viewDict[n][0];
                jason.OOO.userData.views[j].target = viewDict[n][1];

                j++;
            }

            jason.OOO.userData.layers = new object[defLayers.Keys.Count];
            int li = 0;
            foreach (var n in defLayers.Keys)
            {
                jason.OOO.userData.layers[li] = new ExpandoObject();
                jason.OOO.userData.layers[li].name = n;
                li++;
            }
            

            return jason;
        }

    }
    
    public class threeDParam
    {
        public bool IsDefined { get; set; }
        public string JsonSting { get; set; }
        public threeDParam()
        {
            this.IsDefined = false;
        }
        public threeDParam(object JsonObj)
        {
            this.IsDefined = true;
            this.JsonSting = JsonConvert.SerializeObject(JsonObj);
            this.JsonSting = JsonSting.Replace("OOO", "object");

        }

        public override string ToString()
        {
            string outputString = "3D Model for Design Explorer.\nPlease connect this output to Colibri Aggregator's 3DParam";
            return outputString;
        }
        
    }
}