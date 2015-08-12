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
 *  Custom GH Parameter : Element
 *  This register the custom object as a GH parameter
 *  //http://www.grasshopper3d.com/forum/topics/custom-data-and-parameter-no-implicit-reference-conversion
 * 
 *  03/03/15 
 *  Ana Garcia Puyol
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Spectacles.GrasshopperExporter
{
    class ElementParameter:GH_Goo<Element>
    {
        public override IGH_Goo Duplicate()
        {
            ElementParameter element = new ElementParameter();
            element.Value = Value;
            return element;
        }

        public override string TypeDescription
        {
            get { return "Element: A high level Spectacles class to inherit from"; }
        }

        public override string TypeName
        {
            get { return "Element"; }
        }

        public override bool IsValid
        {
            get { return Value.ID != null; }
        }

        public override string ToString()
        {
            return Value.ID;
        }
    }
}
