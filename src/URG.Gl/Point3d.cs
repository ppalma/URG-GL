#region Header
//     Point3d.cs created with MonoDevelop
//     User: Patricio Palma S. as ppalma at 5:25 PM 12/22/2010
//     --------------------------------------------------------------------------
//     URG.Gl
//     Copyright (C) 2006-2008  Patricio Palma S. All Rights Reserved.
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
//     --------------------------------------------------------------------------
// 
//     UNIT                : Point3d.cs
//     SUMMARY            :     
//     PRINCIPLE AUTHOR     : Patricio Palma S. <mail@ppalma.cl>
// 
#endregion Header
#region Revisions
//  --------------------------------------------------------------------------
//     REVISIONS/NOTES
//         dd-mm-yyyy    By          Revision Summary
// 
//  --------------------------------------------------------------------------
 #endregion Revisions

using System;
using System.Collections.Generic;
namespace URG.Gl
{


	public class Point3d<T>
	{
		public T x;                        
        public T y;                        
        public T z;      
		
		public Point3d() 
		{
			x = default(T);
			y = default(T);
			z = default(T);
		}
		
		public Point3d(T x_, T y_, T z_) 
        {
			x = x_;
			y = y_;
			z = z_;
        }
	
		//TODO: note than in original code "rhs" is received as "&" supose to be "content" of variable
		//mmm, may be don't need it, "=" operator is allready "byref" by default...
		public Point3d(Point3d<T> rhs): this(rhs.x, rhs.y, rhs.z) { }
		
		 //public static Matrix operator +(Matrix A, Matrix B)
//        public static Point3d<T> operator =(Point3d<T> rhs)
//        {
//			Point3d<T> p = new Point3d<T>();
//            p = rhs;
//			//p.x = rhs.x;
//            //p.y = rhs.y;
//            //p.z = rhs.z;
//
//            return p;
//        }

		
	}
}
