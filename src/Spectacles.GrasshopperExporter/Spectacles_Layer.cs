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

    public class Layer
    {
        //ATTRIBUTES

        private string myID;

       // public SpectaclesElementType Type { get; set; }

        public string Name { get; set; }

        //PROPERTIES
        //[DataMember]
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

        public Layer() { }
        public Layer(string name)
        {
            Name = name;
        }


    }

   
}
