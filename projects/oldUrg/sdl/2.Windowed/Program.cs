using System;
using System.Collections.Generic;
using System.Text;
using ISE;
using Tao.Sdl;
using Tao.OpenGl;

namespace ISE.Samples
{
    class Program
    {
        static ISE.Decoder thisDecoder;
		static StreamOpenGLVideoHandler OpenGLHandler;
		
        static bool spin = true;
		static int TexHandle;		
		
        static float angle = 0;
        static float wratio = 0;
        static float hratio = 0;

        static void DrawGL()
        {
			thisDecoder.VideoStreams[thisDecoder.videoStream].ReadFrame();
            
			//thisDecoder = new ISE.Decoder("http://3.3.3.2/cgi-bin/video.jpg",
			//                              new StreamVideoHandler[1] {OpenGLHandler}, null);				
			
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);        // Clear The Screen And The Depth Buffer
            Gl.glLoadIdentity(); // Reset The View                       
            
            if (spin) angle = angle + 0.5f;

            Gl.glLoadIdentity();
			

            if (spin) Gl.glRotatef(angle, 0, 1, 0);
            if (spin) Gl.glRotatef(angle/10, 0, 0, 1);
			//Gl.glBindTexture(Gl.GL_TEXTURE_2D,TexHandle);
			//Gl.glEnable(Gl.GL_TEXTURE_2D);
            //Gl.glColor3f(1, 0.5f, 0.3f);

			Gl.glBegin(Gl.GL_QUADS);
			// Draw top half of video flat
			Gl.glTexCoord2d(0,hratio); Gl.glVertex3d(-1,-1,0);
			Gl.glTexCoord2d(wratio,hratio); Gl.glVertex3d(1,-1,0);
			Gl.glTexCoord2d(wratio,0); Gl.glVertex3d(1,1,0);
			Gl.glTexCoord2d(0,0); Gl.glVertex3d(-1,1,0);
			
			Gl.glEnd();

        }

        static void Main(string[] args)
        {        
			// Set the console to output to the command line automatically.
			Global.argsSYSCONSOLE=true;
			//ISE.Engine.InitIcarus(); //Or manually set the proper framework used by the following code...
			int p = (int)Environment.OSVersion.Platform;
            if ((p == 4) || (p == 6) || (p == 128))
            {
                Global.MONO = true;
                IConsole.Write("MONO");

            }
            else
            {
                Global.MONO = false;
                IConsole.Write(".NET");
            }
			
            Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
			
			IConsole.Write("Press ESC to exit!");
			IConsole.Write("------------------");
            IConsole.Write("SDL Initialised");
            IConsole.Write("SDL Major Version: " + Sdl.SDL_MAJOR_VERSION.ToString());
            IConsole.Write("SDL Minor Version: " + Sdl.SDL_MINOR_VERSION.ToString());
			
			IConsole.Write("Initialising Screen..");

            int flags = (Sdl.SDL_HWSURFACE | Sdl.SDL_DOUBLEBUF | Sdl.SDL_OPENGL | Sdl.SDL_RESIZABLE /*| Sdl.SDL_FULLSCREEN */);

            Sdl.SDL_WM_SetCaption("ISE.Video example", "");            

            /*{IntPtr surfacePtr = */	

            int Errors = 0;
			
			// FFMpeg wrapper code
			ISE.Decoder.InitVideo();

			
			// For FireWire
			//thisDecoder.Source = ISE.DecoderSource.Source_IEEE1394;
			//thisDecoder.IEEEdev = "dv1394";
			//thisDecoder.Filename = "/dev/dv1394";
			
			// Generate the OpenGL texture
			int[] textures = new int[2];
			try
			{
			//Gl.glGenTextures(1,textures);
			}
			catch
			{
			}
			TexHandle = textures[1];
			
			// For a file
			//thisDecoder.Source = ISE.DecoderSource.Source_File;
			//StreamOpenGLVideoHandler OpenGLHandler = new StreamOpenGLVideoHandler(TexHandle);			
			OpenGLHandler = new StreamOpenGLVideoHandler(TexHandle);			

			//Setting values are supported for the media resource, like: width, height, timebase_num, timebase_den, samplerate, standard, pixfmt, etc...
			//the following can be used as setting values examples
			//thisDecoder = new ISE.Decoder("../../../../../../../Assets/Movies/startrek1.flv",
			//thisDecoder = new ISE.Decoder("../../../../../../../Assets/Movies/startrek1.flv",
            //thisDecoder = new ISE.Decoder("http://www.pointscape.com.sg/logo.jpg",
			//thisDecoder = new ISE.Decoder("http://3.3.3.2/cgi-bin/video.jpg?FileInputFormat=mjpeg", 
            //thisDecoder = new ISE.Decoder("http://ve3dmovies.ign.com/videos/04/57/45701_TerminatorSalvationMovieTrailer3.flv",
            //thisDecoder = new ISE.Decoder("file://../../Assets/Movies/2.mpeg",
			thisDecoder = new ISE.Decoder("file://../../Assets/Movies/War3-Movie-Trailer.avi?SourceWidth=320&SourceHeight=240&SourceSampleRate=32",			                          
			//thisDecoder = new ISE.Decoder("v4l2:///dev/video0",
			//thisDecoder = new ISE.Decoder("v4l:///dev/video4?SourceWidth=320&SourceHeight=240&SourceSampleRate=32",
			//thisDecoder = new ISE.Decoder("v4l2:///dev/video0?SourceWidth=640&SourceHeight=480&SourceTimeBaseNum=1&SourceTimeBaseDen=28&SourceSampleRate=28&SourceChannel=0&SourcePixFmt=NONE",
			//thisDecoder = new ISE.Decoder("v4l2:///dev/video2?SourceWidth=640&SourceHeight=480&SourceTimeBaseNum=1&SourceTimeBaseDen=28&SourceSampleRate=28&SourceChannel=0&SourcePixFmt=none&SourceStandard=ntsc",
            //thisDecoder = new ISE.Decoder("v4l2:///dev/video0?SourceWidth=640&SourceHeight=480&SourceTimeBaseNum=1&SourceTimeBaseDen=28&SourceSampleRate=28&SourceChannel=0&SourcePixFmt=YUV420P",
			//thisDecoder = new ISE.Decoder("v4l2:///dev/video3?SourceWidth=640&SourceHeight=480&SourceTimeBaseNum=1&SourceTimeBaseDen=28&SourceSampleRate=28",
			                              new StreamVideoHandler[1] {OpenGLHandler},
			                              null);				
			
		    DecoderVideoStream videoStream = thisDecoder.VideoStreams[thisDecoder.videoStream];
			
			IConsole.Write("Using Stream " + thisDecoder.videoStream.ToString() + " for Video");			
						
			
            Sdl.SDL_SetVideoMode(videoStream.OutputWidth, videoStream.OutputHeight, 32, flags);

            Sdl.SDL_WM_GrabInput(Sdl.SDL_GRAB_OFF);		
			
			// Now initialise OpenGL rendering			
			Gl.glBindTexture(Gl.GL_TEXTURE_2D,TexHandle);			
			Gl.glEnable(Gl.GL_TEXTURE_2D);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE);
			
                int thiswidth = videoStream.OutputWidth;
                int thisheight = videoStream.OutputHeight;

                wratio = videoStream.OutputWidth / (float)thiswidth;
                hratio = videoStream.OutputHeight / (float)thisheight;
				Gl.glTexImage2D(Gl.GL_TEXTURE_2D,0,3,thiswidth,
			                thisheight,
			                0,Gl.GL_RGB,Gl.GL_UNSIGNED_BYTE,IntPtr.Zero); 
				Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
				Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR); // MUST TURN OFF MIPMAPS for DecodeTo_Play to work without mipmaps.
			//}
			//else
			//{
				// For better image quality on 3D video, use Gl.GL_LINEAR_MIPMAP_LINEAR) for the MIN filter
			//	Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
			//	Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR_MIPMAP_LINEAR);
			//}			
			

            Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);
            Gl.glMatrixMode(Gl.GL_PROJECTION);  // Change Matrix Mode to Projection
            Gl.glLoadIdentity();             // Reset View
            Glu.gluPerspective(45, 480/640,
                               40, -40);

						
			
			//IConsole.Write("{0}, {1}, {2}, {3} ",offset.left, offset.top, offset.width, offset.height);

            Gl.glClearColor(0.5f, 0, 0, 1);

            if (Errors == 0)
            {
                bool isRunning = true;

                /*int numevents = 10;
                Sdl.SDL_Event[] events = new Sdl.SDL_Event[numevents];
                events[0].type = Sdl.SDL_KEYDOWN;
                events[0].key.keysym.sym = (int)Sdl.SDLK_p;
                events[1].type = Sdl.SDL_KEYDOWN;
                events[1].key.keysym.sym = (int)Sdl.SDLK_z;
                int result2 = Sdl.SDL_PeepEvents(events, numevents, Sdl.SDL_ADDEVENT, Sdl.SDL_KEYDOWNMASK);
                */
                Sdl.SDL_Event evt;

                while (isRunning)
                {                    

					while (Sdl.SDL_PollEvent(out evt)!=0)
					{
						if (evt.type == Sdl.SDL_QUIT)
	                    {
	                        isRunning = false;
	                    }
	                    else if (evt.type == Sdl.SDL_KEYDOWN)
	                    {
	                        if ((evt.key.keysym.sym == (int)Sdl.SDLK_ESCAPE) ||
	                            (evt.key.keysym.sym == (int)Sdl.SDLK_q))
	                        {
	                            isRunning = false;
	                        }

	                        else if (evt.key.keysym.sym == (int)Sdl.SDLK_SPACE)
	                        {
	                            spin = !spin;
	                        }
	                    }
					}

                    DrawGL();
					
					//if (Global.ReportRun>0)
					//	Global.ReportRun -= 1;
					
                    Sdl.SDL_GL_SwapBuffers();

											
				}
            }

			thisDecoder.Clear();
            Sdl.SDL_Quit();    
        }
    }
}
