// #region Header
//     Window.cs created with MonoDevelop
//     User: Patricio Palma S. as ppalma at 5:55 PM 12/22/2010
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
//     UNIT                : Window.cs
//     SUMMARY            :     
//     PRINCIPLE AUTHOR     : Patricio Palma S. <mail@ppalma.cl>
// 
// #endregion Header
// #region Revisions
//  --------------------------------------------------------------------------
//     REVISIONS/NOTES
//         dd-mm-yyyy    By          Revision Summary
// 
//  --------------------------------------------------------------------------
// #endregion Revisions

using System;
using System.Collections.Generic;
using System.Drawing;
using MBF.Sensors;
using Tao.FreeGlut;

using Tao;

namespace URG.Gl
{


	public abstract class Window
	{

//		URG sensor;
		public Window ()
		{
		}
		protected virtual void HotKeys(){}
		private int glutWnd;
		private bool isInitialized;
		
		protected abstract void PaintHandler();
		protected abstract void ReshapeHandler(int weith, int height);
		
		private void AmbientInitialization()
		{
		
				Glut.glutInit();
				
				Glut.glutInitDisplayMode(Glut.GLUT_RGBA | Glut.GLUT_DOUBLE);
				Glut.glutInitWindowSize(600, 600);
				Tao.OpenGl.Gl.glClearColor(0.0f, 0.0f, 0.0f, 1f);
				Tao.OpenGl.Gl.glEnable(Tao.OpenGl.Gl.GL_DEPTH_TEST);
				Tao.OpenGl.Gl.glEnable(Tao.OpenGl.Gl.GL_CULL_FACE);
				Tao.OpenGl.Gl.glEnable(Tao.OpenGl.Gl.GL_TEXTURE_2D);
				
	        	glutWnd = Glut.glutCreateWindow("URG 3D Scanner");

				//signal handlers
				Glut.glutKeyboardFunc(new Glut.KeyboardCallback(KeyboardHandler));
				Glut.glutMouseFunc(new Glut.MouseCallback(MouseHandler));
				Glut.glutMotionFunc(new Glut.MotionCallback(MotionHandler));
				Glut.glutIdleFunc(new Glut.IdleCallback(IdleHandler));
				Glut.glutReshapeFunc(new Glut.ReshapeCallback(ReshapeHandler)); 
				Glut.glutDisplayFunc(new Glut.DisplayCallback(PaintHandler)); 
				Glut.glutWMCloseFunc(new Glut.WindowCloseCallback(WindowsCloseHandler));
				Glut.glutTimerFunc(1, new Glut.TimerCallback(TimerHandler), 0);
				
				//Gl/Glut initialized correclty
				isInitialized = true;
				Glut.glutMainLoop();
	        	
		}
		protected virtual void KeyboardHandler(byte key, int x, int y)
		{
			Console.WriteLine("Keyboar Handler: key = {0}. x ={1}. y = {2}",key, x,y); 
		}
		protected virtual void MouseHandler(int button, int state, int x, int y)
		{
			Console.WriteLine("Mouse 2D position ({0},{1})",x,y); 
		}
		protected virtual void MotionHandler(int x, int y)
		{
			Console.WriteLine("Motion Handler: x = {0}. y = {1}",x,y);
		}
		protected virtual void TimerHandler(int val)
		{
			Console.WriteLine("Timer Handler: val = {0}",val); 
		}
		protected virtual void IdleHandler(){}
		protected virtual void WindowsCloseHandler(){}
		
		public void Show()
		{
			HotKeys();
			AmbientInitialization();
		}
	}
}
