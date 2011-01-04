#region Header
//     Main.cs created with MonoDevelop
//     User: Patricio Palma S. as ppalma at 12:45 PM 1/1/2011
//     --------------------------------------------------------------------------
//     URG.GL.LaserSpatialDataDraw
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
//     UNIT                : Main.cs
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

namespace URG.GL
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello URG Gl  Laser spatial data draw Test!");
			LaserSpatialDataDraw win = new LaserSpatialDataDraw();
			win.Show();
		}
	}
}
