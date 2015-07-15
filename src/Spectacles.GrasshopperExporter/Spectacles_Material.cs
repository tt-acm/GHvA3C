/*  HEADER
 * 
 * ELEMENT CLASS  
 *  A high level class to inherit from.  Provides fields that all model elements must have.
 *  
 *  3/3/15
 *  
 * Ana Garcia Puyol
 *  
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Spectacles.GrasshopperExporter.Properties;


using Newtonsoft.Json;


namespace Spectacles.GrasshopperExporter
{

    public class Material
    {
        //ATTRIBUTES

        private string myID;

        public SpectaclesMaterialType Type { get; set; }

        public string MaterialJson { get; set; }


        //PROPERTIES
        public string ID
        {
            get { return myID; }
            set
            {
                try
                {
                    //test for the empty string
                    if (value == "")
                    {
                        throw new ArgumentException("The input string cannot be empty");
                    }

                    myID = value;
                }

                catch (Exception e) //should catch the null case
                {
                    throw e;
                }
            }
        }

        //constructors

        public Material() { }

        public Material(string json, SpectaclesMaterialType type)
        {
            Type = type;
            MaterialJson = json;
        }

        
    }
    public enum SpectaclesMaterialType
    {
        Mesh,
        Line
    }
}
