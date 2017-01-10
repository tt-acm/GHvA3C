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
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Spectacles.GrasshopperExporter.Properties;

using Newtonsoft.Json;

namespace Spectacles.GrasshopperExporter
{
    /// <summary>
    /// Contains utility functions to be called from GH component classes
    /// </summary>
    public class _Utilities
    {
        /// <summary>
        /// Returns a string representation of a hex color given a GH_Colour object
        /// </summary>
        /// <param name="ghColor">the grasshopper color to convert</param>
        /// <returns>a hex color string</returns>
        public static string hexColor(GH_Colour ghColor)
        {
            string hexStr = "0x" + ghColor.Value.R.ToString("X2") +
                ghColor.Value.G.ToString("X2") +
                ghColor.Value.B.ToString("X2");

            return hexStr;
        }

        /// <summary>
        /// Returns a css string representation of a .net color
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        public static string cssColor(Color col)
        {
            string cssStr = "#" + col.R.ToString("X2") +
                col.G.ToString("X2") +
                col.B.ToString("X2");

            return cssStr;
        }

        /// <summary>
        /// Returns a JSON string representing a rhino mesh, and containing any attributes as user data
        /// </summary>
        /// <param name="mesh">The rhino mesh to serialize.  Can contain quads and tris.</param>
        /// <param name="attDict">The attribute dictionary to serialize.  Objects should all be reference types.</param>
        /// <returns>a JSON string representing a rhino mes</returns>
        public static string geoJSON(Mesh mesh, Dictionary<string, object> attDict)
        {
            //create a dynamic object to populate
            dynamic jason = new ExpandoObject();


            jason.uuid = Guid.NewGuid();
            jason.type = "Geometry";
            jason.data = new ExpandoObject();
            jason.userData = new ExpandoObject();

            //populate data object properties

            //fisrt, figure out how many faces we need based on the tri/quad count
            var quads = from q in mesh.Faces
                        where q.IsQuad
                        select q;

            jason.data.vertices = new object[mesh.Vertices.Count * 3];
            jason.data.faces = new object[(mesh.Faces.Count + quads.Count()) * 4];
            jason.data.normals = new object[0];
            jason.data.uvs = new object[0];
            jason.data.scale = 1;
            jason.data.visible = true;
            jason.data.castShadow = true;
            jason.data.receiveShadow = false;
            jason.data.doubleSided = true;

            //populate vertices
            int counter = 0;
            int i = 0;
            foreach (var v in mesh.Vertices)
            {
                jason.data.vertices[counter++] = Math.Round(mesh.Vertices[i].X * -1.0, 5);
                jason.data.vertices[counter++] = Math.Round(mesh.Vertices[i].Z, 5);
                jason.data.vertices[counter++] = Math.Round(mesh.Vertices[i].Y, 5);
                i++;
            }

            //populate faces
            counter = 0;
            i = 0;
            foreach (var f in mesh.Faces)
            {
                if (f.IsTriangle)
                {
                    jason.data.faces[counter++] = 0;
                    jason.data.faces[counter++] = mesh.Faces[i].A;
                    jason.data.faces[counter++] = mesh.Faces[i].B;
                    jason.data.faces[counter++] = mesh.Faces[i].C;
                    i++;
                }
                if (f.IsQuad)
                {
                    jason.data.faces[counter++] = 0;
                    jason.data.faces[counter++] = mesh.Faces[i].A;
                    jason.data.faces[counter++] = mesh.Faces[i].B;
                    jason.data.faces[counter++] = mesh.Faces[i].C;
                    jason.data.faces[counter++] = 0;
                    jason.data.faces[counter++] = mesh.Faces[i].C;
                    jason.data.faces[counter++] = mesh.Faces[i].D;
                    jason.data.faces[counter++] = mesh.Faces[i].A;
                    i++;
                }
            }

            //populate vertex colors
            if (mesh.VertexColors.Count != 0)
            {
                jason.data.colors = new object[mesh.Vertices.Count];
                i = 0;
                foreach (var c in mesh.VertexColors)
                {
                    jason.data.colors[i] = _Utilities.hexColor(new GH_Colour(c));
                    i++;
                }
            }


            //populate userData objects
            var attributeCollection = (ICollection<KeyValuePair<string, object>>)jason.userData;
            foreach (var kvp in attDict)
            {
                attributeCollection.Add(kvp);
            }


            return JsonConvert.SerializeObject(jason);
        }

        /// <summary>
        /// Returns a JSON string representing a rhino mesh, and containing any attributes as user data
        /// </summary>
        /// <param name="mesh">The rhino mesh to serialize.  Can contain quads and tris.</param>
        /// <param name="attDict">The attribute dictionary to serialize.  Objects should all be reference types.</param>
        /// <returns>a JSON string representing a rhino mes</returns>
        public static string geoJSONColoredVertices(Mesh mesh, Dictionary<string, object> attDict)
        {
            //create a dynamic object to populate
            dynamic jason = new ExpandoObject();


            jason.uuid = Guid.NewGuid();
            jason.type = "Geometry";
            jason.data = new ExpandoObject();
            jason.userData = new ExpandoObject();

            //populate data object properties

            //fisrt, figure out how many faces we need based on the tri/quad count
            var quads = from q in mesh.Faces
                        where q.IsQuad
                        select q;

            jason.data.vertices = new object[mesh.Vertices.Count * 3];
            jason.data.faces = new object[(mesh.Faces.Count + quads.Count()) * 7];
            jason.data.normals = new object[0];
            jason.data.uvs = new object[0];
            jason.data.scale = 1;
            jason.data.visible = true;
            jason.data.castShadow = true;
            jason.data.receiveShadow = false;
            jason.data.doubleSided = true;

            //populate vertices
            int counter = 0;
            int i = 0;
            foreach (var v in mesh.Vertices)
            {
                jason.data.vertices[counter++] = System.Math.Round(mesh.Vertices[i].X * -1.0, 5);
                jason.data.vertices[counter++] = System.Math.Round(mesh.Vertices[i].Z, 5);
                jason.data.vertices[counter++] = System.Math.Round(mesh.Vertices[i].Y, 5);
                i++;
            }

            //populate faces
            counter = 0;
            i = 0;
            foreach (var f in mesh.Faces)
            {
                if (f.IsTriangle)
                {
                    jason.data.faces[counter++] = 128;
                    jason.data.faces[counter++] = mesh.Faces[i].A;
                    jason.data.faces[counter++] = mesh.Faces[i].B;
                    jason.data.faces[counter++] = mesh.Faces[i].C;
                    jason.data.faces[counter++] = mesh.Faces[i].A;
                    jason.data.faces[counter++] = mesh.Faces[i].B;
                    jason.data.faces[counter++] = mesh.Faces[i].C;
                    i++;
                }
                if (f.IsQuad)
                {
                    jason.data.faces[counter++] = 128;
                    jason.data.faces[counter++] = mesh.Faces[i].A;
                    jason.data.faces[counter++] = mesh.Faces[i].B;
                    jason.data.faces[counter++] = mesh.Faces[i].C;
                    jason.data.faces[counter++] = mesh.Faces[i].A;
                    jason.data.faces[counter++] = mesh.Faces[i].B;
                    jason.data.faces[counter++] = mesh.Faces[i].C;
                    jason.data.faces[counter++] = 128;
                    jason.data.faces[counter++] = mesh.Faces[i].C;
                    jason.data.faces[counter++] = mesh.Faces[i].D;
                    jason.data.faces[counter++] = mesh.Faces[i].A;
                    jason.data.faces[counter++] = mesh.Faces[i].C;
                    jason.data.faces[counter++] = mesh.Faces[i].D;
                    jason.data.faces[counter++] = mesh.Faces[i].A;
                    i++;
                }
            }

            //populate vertex colors
            if (mesh.VertexColors.Count != 0)
            {
                jason.data.colors = new object[mesh.Vertices.Count];
                i = 0;
                foreach (var c in mesh.VertexColors)
                {
                    jason.data.colors[i] = _Utilities.cssColor(c);
                    i++;
                }
            }


            //populate userData objects
            var attributeCollection = (ICollection<KeyValuePair<string, object>>)jason.userData;
            foreach (var kvp in attDict)
            {
                attributeCollection.Add(kvp);
            }


            return JsonConvert.SerializeObject(jason);
        }
    }


    //below are a number of Catcher classes which are used to deserialize JSON objects
    //mostly called from the Spectacles_CompileScene component


    public class SpectaclesGeometryCatcher
    {
        public string uuid;
        public string type;
        public object data;
    }

    public class SpectaclesBaseMaterialCatcher
    {
        public string type;
    }

    //mesh phong materials
    public class SpectaclesMeshPhongMaterialCatcher
    {
        public string uuid;
        public string type;
        public string color;
        public string ambient;
        public string emissive;
        public string specular;
        public double shininess;
        public double opacity;
        public bool transparent;
        public bool wireframe;
        public int side;
    }

    //mesh basic materials
    public class SpectaclesMeshBasicMaterialCatcher
    {
        public string uuid;
        public string type;
        public string color;
        public int side;
        public double opacity;
        public bool transparent;
    }

    //mesh basic materials with face colors
    public class SpectaclesMeshFaceMaterialCatcher
    {
        public string uuid;
        public string type;
        //public string color;
        //public bool transparent;
        //public bool wireframe;
        //public int side;
        public object[] materials;
    }

    //mesh lambert materials - for use with vertex colors
    public class SpectaclesMeshLambertMaterialCatcher
    {
        public string uuid;
        public string type;
        public string color;
        public string ambient;
        public string emissive;
        public int side;
        public double opacity;
        public bool transparent;
        public int shading;
        public int vertexColors;

    }


    public class SpectaclesAttributesCatcher
    {
        public object userData;
    }

    public class SpectaclesLineCatcher
    {
        public string uuid;
        public string type;
        public object data;
    }

    public class SpectaclesLineBasicMaterialCatcher
    {
        public string uuid;
        public string type;
        public string color;
        public double linewidth;
        public double opacity;
    }

    public class SpectaclesCameraCatcher
    {
        public string name;
        public object eye;
        public object target;
    }
}
