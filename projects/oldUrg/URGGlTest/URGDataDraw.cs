
using System;
using System.Collections.Generic;
using System.Drawing;
using Tao.OpenGl;
using Tao.FreeGlut;
using MBF.Sensors;
using MBF.HMIDevices;
using MBF.CWiidSharp;
using System.IO;

namespace URGGlTest
{


	public class URGDataDraw
	{
		private bool isInitialized;
		private int glutWnd;
		private int last_button, last_state;

		//public int AliveMsec = 3000;            // [msec]
    	public int AliveMsec = 3; //20;              // [msec]
		
		//vector<long> data_;
		public List<int> data_ = new List<int>();
		//vector<long> intensity_data_;
		public List<int> intensity_data_ = new List<int>();

		//size_t data_max_;
		public double data_max_;
		public int length_min_;
  		public int length_max_;
		
		Lines saved_lines_data_ = new Lines();
		Lines normal_lines_data_ = new Lines();
  		Line recent_line_data_ = new Line();
		public Point last_pos_ = new Point(0 ,0);
		public int x_rot_ = 0;
  		public int y_rot_ = 0;
  		public int z_rot_ = 0;

  		public bool h_type_ = false;
  		public bool front_only_ = false;
  		public bool pre_record_ = false;

  		public int magnify_ = 50;
  		public bool no_plot_ = false ;
  		public bool intensity_mode_ = false ;

		//vector<Point3d<double> > color_table_;
		List<Point3d<double>> color_table_ = new List<Point3d<double>>();

		private int ticks;
		
		private double startTicks;
		
		
		URG sensor;
		
		
		Wiimote wii;
		
		
		public URGDataDraw ()
		{
			data_max_ = 0;
      		length_min_ = 0;
			length_max_ = 0;
      		x_rot_ =0;
			y_rot_ = 0;
			z_rot_ = 0;
            h_type_ = false;
			front_only_ = true; //true; //TODO: should be true by default?, since UrgScannerWindow have that one as default on then UI
			pre_record_ = false;
      		magnify_ = 50;
      		no_plot_ = false;
			intensity_mode_= false;
			
			for (int mm_height = 0; mm_height < 1000; ++mm_height) {
      			//QColor color;
      			//color.setHsv(static_cast<int>(360.0 * mm_height / 1000.0), 255, 255);

      			int r, g, b;
      			HsvToRgb(360.0 * mm_height / 1000.0, 1, 1, out r, out g, out b); //Supose to generate a HSV color 
																				 //and return r,g,b values ... :-S ...
      			color_table_.Add(new Point3d<double>(r / 255.0, g / 255.0, b / 255.0));
    		}
		}
		
		public void Show()
		{
			SensorInitialization();
			
			WiiInitialization();
			
			AmbientInitialization();
		}

		private void WiiInitialization()
		{
			wii = new Wiimote();
			
			Console.WriteLine("Put Wiimote in discoverable mode now (press 1+2) ...");
		
			wii.Connect();
			wii.SetReportMode(CWiid.RPT_FLAGS.ACC | CWiid.RPT_FLAGS.BTN);

			//Asociate to the desired event handlers...
			wii.ButtonPressed += new ButtonPressedEventHandler(ButtonPress); 
			wii.AccelerometerChanged += new AccelerometerChangedEventHandler(AccChange);

		}
		
		private void ButtonPress(object sender,ButtonPressedEventArgs e)
		{
			if(CWiid.BTN_FLAGS.BTN_A == e.Button)
				this.saveVrml("test.wrl");
		}

		double accx, accy, accz;
		private void AccChange(object sender, AccelerometerChangedEventArgs e)
		{		
			//Console.WriteLine(e.Accelerometer.ToString());
			//Console.WriteLine("Roll:{0}, Pitch:{1}, Global Acceleration:{2}", e.Roll.ToString(), e.Pitch.ToString(), e.Acceleration.ToString());
			
			accx = e.Roll;  		
			accy = e.Pitch; 		
			accz = e.Acceleration; 	
		}
		
		private void SensorInitialization()
		{
			try {
				
				sensor = new URG("/dev/ttyACM0");
				sensor.Connect();
				Console.WriteLine(sensor.ToString());
			} 
			catch (Exception e) {
				Console.WriteLine("URG Error: {0}", e.Message);
			}
		}
		
		private void AmbientInitialization()
		{
		
			try 
			{
				

				Glut.glutInit();
				
				Glut.glutInitDisplayMode(Glut.GLUT_RGBA | Glut.GLUT_DOUBLE);
				Glut.glutInitWindowSize(600, 600);
				
				Gl.glClearColor(0.0f, 0.0f, 0.0f, 1f);
				Gl.glEnable(Gl.GL_DEPTH_TEST);
				Gl.glEnable(Gl.GL_CULL_FACE);
				Gl.glEnable(Gl.GL_TEXTURE_2D);
				
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
			catch (Exception e) 
			{
				Console.WriteLine("Gl/Glut Init Error: {0}", e.Message);
			}
		}
		
		private void TimerHandler(int value)
		{
		
			ticks +=1 ;// value;
			//Console.WriteLine("Ticks {0}", ticks);
			Glut.glutPostRedisplay();
			Glut.glutTimerFunc(1, new Glut.TimerCallback(TimerHandler), value);

		}
		
		private void IdleHandler()
		{
			redrawCap();
		}
		
		private void KeyboardHandler(byte key, int x, int y)
		{
			switch (key)
			{
				case 109: //key "m"
					this.magnify_ += 10;
					break;
				case 110: //key "n"
					this.magnify_ -= 10;
					break;
			}
		}
		
		private void MouseHandler(int button, int state, int x, int y)
		{
			//Console.WriteLine("Mx = {0}, My = {1}, B = {2}, State = {3}", x, y, button, state);
			last_pos_.X = x;
			last_pos_.Y = y;
			last_button = button; //0 left, 1 middle, 2 right
			last_state = state; //0 pressed, 1 unpressed
		}
		
		private void MotionHandler(int x, int y)
		{
			//Console.WriteLine("Mx = {0}, My = {1}", x, y);
			
			int dx = x - last_pos_.X;
  			int dy = y - last_pos_.Y;

  			if ((last_button == 0)&&(last_state == 0)) {
    			setXRotation(x_rot_ + 8 * dy);
    			setZRotation(z_rot_ + 8 * dx);

  			} else if ((last_button == 2)&&(last_state == 0)) {
    			setXRotation(x_rot_ + 8 * dy);
    			setYRotation(y_rot_ + 8 * dx);
  			}
  			
			last_pos_.X = x;
			last_pos_.Y = y;

		}
		
		private void setXRotation(int angle)
  		{
    		angle = normalizeAngle(angle);
    		if (angle != x_rot_) {
      			x_rot_ = angle;
      			//parent->updateGL(); //TODO: what exaclty do this??
				//Glut.glutSwapBuffers(); //??
				PaintHandler(); //??
    		}
  		}

		private void setYRotation(int angle)
  		{
    		angle = normalizeAngle(angle);
    		if (angle != y_rot_) {
      			y_rot_ = angle;
      			//parent->updateGL();
				//Glut.glutSwapBuffers(); //??
				PaintHandler(); //??
    		}
  		}

		private void setZRotation(int angle)
  		{
    		angle = normalizeAngle(angle);
    		if (angle != z_rot_) {
      			z_rot_ = angle;
      			//parent->updateGL();
				//Glut.glutSwapBuffers(); //??
				PaintHandler(); //??
    		}
  		}

		private int normalizeAngle(int angle)
  		{
    		while (angle < 0) {
      			angle += 360 * 16;
    		}

    		while (angle > 360 * 16) {
      			angle -= 360 * 16;
    		}
			return angle;
  		}
		
		private void ReshapeHandler(int width, int height)
		{
//			Gl.glClearColor(0.0f, 0.0f, 0.0f, 1f);
//			Gl.glEnable(Gl.GL_DEPTH_TEST);
//			Gl.glEnable(Gl.GL_CULL_FACE);
//			Gl.glEnable(Gl.GL_TEXTURE_2D);
		
			Gl.glViewport(0, 0, width, height);

  			Gl.glMatrixMode(Gl.GL_PROJECTION);
  			Gl.glLoadIdentity();

  			double aspect = 1.0 * width / height;
  			Gl.glOrtho(-5000 * aspect, 5000 * aspect, -5000, 5000, -100000, 100000);

  			Gl.glMatrixMode(Gl.GL_MODELVIEW);

//			Gl.glEnable(Gl.GL_DITHER);
//			Gl.glDepthFunc(Gl.GL_LESS);
//			Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);
//
//			Gl.glClearDepth(1);
//			Gl.glClearColor(0.7f, 0.7f, 0.7f, 1f);
//			Gl.glMatrixMode(Gl.GL_PROJECTION);
//			Gl.glLoadIdentity();
//	            
//			double _Aspect = (float)weith / (float)height; 
//	            
//			Glu.gluPerspective(60, _Aspect, 1, 2000);
//
//			Gl.glViewport(0, 0, weith, height); 
//			Gl.glMatrixMode(Gl.GL_MODELVIEW);
//			Gl.glLoadIdentity();
			
			//TODO: Remove !!! Test only!!...
			//Gl.glEnable(Gl.GL_LIGHTING);
			//Gl.glEnable(Gl.GL_LIGHT0);

//			Gl.glShadeModel(Gl.GL_SMOOTH);
//			Gl.glFrontFace(Gl.GL_CCW);
//			Gl.glLightModelf(Gl.GL_LIGHT_MODEL_TWO_SIDE, 1.0F);
//			Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
//			Gl.glEnable(Gl.GL_TEXTURE_2D);
////			Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_AMBIENT, _ModelAmbientLight);
////			Gl.glMateriali(Gl.GL_FRONT, Gl.GL_SHININESS, _MaterialShininess);
//
//			Gl.glEnable(Gl.GL_COLOR_MATERIAL);
//
//			Gl.glLoadIdentity();

		}
		
		private void PaintHandler()
		{
			//TODO: check...qrobosdk seems to use by default SDL_GetTicks() which it seems to return a milisecond value
			//since the SDL lib was initialized. So "startTicks" hold the "initial" tick value when this app start, then here
			//we substract the current ticks to have the value of how many tick we have from the start (divided by 10000 since MSDN says
			//that 1 milisecond contain 10000 ticks)
			//double ticks = (DateTime.Now.Ticks - startTicks) / 10000;
			//Console.WriteLine("Painthandler ticks {0}", ticks);
			
			Gl.glClearColor(0.0f, 0.0f, 0.0f, 1f);
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
			Gl.glLoadIdentity();

			Gl.glRotated(x_rot_ / 16.0, 1.0, 0.0, 0.0);
		    Gl.glRotated(y_rot_ / 16.0, 0.0, 1.0, 0.0);
    		Gl.glRotated((z_rot_ / 16.0) + 90, 0.0, 0.0, 1.0);

			while ((normal_lines_data_.Count > 0) &&
           			((normal_lines_data_[0].timestamp + AliveMsec) < ticks)) {
      				//normal_lines_data_.pop_front();
				normal_lines_data_.Remove(normal_lines_data_[0]);
				
    		}
			
		    Gl.glBegin(Gl.GL_POINTS);

			double ratio = (1.0 / 2.0) + (5.0 * magnify_ / 100.0);
			
   			if (! no_plot_) {
				
   				//for (Lines::iterator line_it = normal_lines_data_.begin();
				//     					line_it != normal_lines_data_.end(); ++line_it)
				foreach (Line line_it in normal_lines_data_) {
        			// !!! false をマクロに置き換える
        			drawLine(line_it, false, ratio); 
   				}
   			}

			
			
			//for (Lines::iterator line_it = saved_lines_data_.begin();
         	//		line_it != saved_lines_data_.end(); ++line_it) {
			foreach (Line line_it in saved_lines_data_) {
     			drawLine(line_it, true, ratio);
   			}


   			if (! no_plot_) {
      			drawLaser(recent_line_data_, ratio);
   			}

			Gl.glEnd();
			
			Glut.glutSwapBuffers();

		}
		
		private void WindowsCloseHandler()
		{
		}
		
		public void drawLine(Line line, bool record, double ratio)
		{
			//record false
			
			//TODO: check...qrobosdk seems to use by default SDL_GetTicks() which it seems to return a milisecond value
			//since the SDL lib was initialized. So "startTicks" hold the "initial" tick value when this app start, then here
			//we substract the current ticks to have the value of how many tick we have from the start (divided by 10000 since MSDN says
			//that 1 milisecond contain 10000 ticks)
			//double ticks = (DateTime.Now.Ticks - startTicks) / 10000;
			
			if (! record) {
		    	// 時間に応じた色分け
		      	double gradation =
		        	1.0 * (AliveMsec - (ticks - line.timestamp)) / AliveMsec;
		      		Gl.glColor3d(1.0 * gradation, 1.0 * gradation, 1.0 * gradation);
				
				//entra aqui
		//Glut.getTick();
			} else {
		    	// 通常時の色
		      	Gl.glColor3d(0.0, 1.0, 0.0);
		    }
		
		    int index = 0;
		    //for (Points::iterator it = line->points.begin();
			//    it != line->points.end(); ++it, ++index) {
		    foreach (Point3d<int> it in line.points) {
		      	if (record) {
		        	if (intensity_mode_) {
		          		// 強度情報に応じた色分け
		          		//int ratio = static_cast<int>((line->intensity[index] % 4000) / 4.0);
		          		int ratioN = (int)(line.intensity[index] + 1000) % 1000;
		          		Point3d<double> color = color_table_[ratioN];
		          		Gl.glColor3d(color.x, color.y, color.z);
		
		        	} else {
		
		        	  	// 記録中は、奥行きによって色を変える
		          		int mm = ((it.x % 1000) + 1000) % 1000;
		          		Point3d<double> color = color_table_[mm];
		          		Gl.glColor3d(color.x, color.y, color.z);
		        	}
		      	}
				//TODO: check if this "ratio" supose to be the argument one or the "ratioN" that was forced to define inside previous "if"...
		     	Gl.glVertex3d(it.x * ratio, it.y * ratio, it.z * ratio);
				
				index++;
		    }
		}

		public void drawLaser(Line line, double ratio)
  		{
    		Gl.glColor3d(0.6, 0.0, 0.0);

    		int index = 0;
    		//for (Points::iterator it = line.points.begin();
         	//	it != line.points.end(); ++it, ++index) {
			foreach (Point3d<int> it in line.points) {

			    if ((it.x == 0) && (it.y == 0) && (it.z == 0)) {
        			continue;
      			}

      			if ((index & 0x3) == 0x00) {
        			Gl.glBegin(Gl.GL_LINE_STRIP);
        			Gl.glVertex3d(0.0, 0.0, 0.0);
        			Gl.glVertex3d(it.x * ratio, it.y * ratio, it.z * ratio);
        			Gl.glEnd();
      			}
    		}
  		}

		public void ResetView()
		{
  			x_rot_ = 0;
  			y_rot_ = 0;
  			z_rot_ = 0;
		}
		
		//TODO: this should be called by idle handler or something like that, also it can include the "how" to take the values
		//from Wiimote and URG laser sensor (on qrobosdk UrgScanner pass this values from the UI window...
		private void redraw(Point3d<int> wii_rotate, bool record, bool no_plot)
		{
  			no_plot_ = no_plot;

		  	Line line = new Line();
		  	line.rotate = wii_rotate;
		  	convertScanData(line);
		
		  	if (record) {
		    	if (pre_record_ == false) {
		      		// 新しい記録が開始されたら、前回の記録データを削除
		      		saved_lines_data_.Clear();
		    	}
		    	// 描画データとして登録
		    	addSaveLine(line);
		
		  	} else {
		    	// 一時データとして登録
		    	addTemporaryLine(line);
		  	}
		
		  	// 最新データをレーザ表示用に登録
		  	//if (! line.points.empty()) {
			if (line.points.Count > 0) {
				//TODO: check this!!, suposly the "swap" function just exchanche data from b to a...check if the qrobosdk do the same...
		    	//swap(recent_line_data_, line);
				Line tmpl = new Line();
				tmpl.intensity = recent_line_data_.intensity;
				tmpl.points = recent_line_data_.points;
				tmpl.rotate = recent_line_data_.rotate;
				tmpl.timestamp = recent_line_data_.timestamp;
				
				recent_line_data_.intensity = line.intensity;
				recent_line_data_.points = line.points;
				recent_line_data_.rotate = line.rotate;
				recent_line_data_.timestamp = line.timestamp;
				
				line.intensity = tmpl.intensity;
				line.points = tmpl.points;
				line.rotate = tmpl.rotate;
				line.timestamp = tmpl.timestamp;

				
				//recent_line_data_ = line;
		  	}
		
		  	pre_record_ = record;
		
		  	//updateGL();
			//Glut.glutSwapBuffers(); //??
			PaintHandler(); //??
		}

		//private void convertScanData(Line line, RangeSensor sensor)
		private void convertScanData(Line line)
  		{
    		data_max_ = sensor.MaxScanLines;
    		length_min_ = sensor.MinDistance;
    		length_max_ = sensor.MaxDistance;

			
			//TODO: check...qrobosdk seems to use by default SDL_GetTicks() which it seems to return a milisecond value
			//since the SDL lib was initialized. So "startTicks" hold the "initial" tick value when this app start, then here
			//we substract the current ticks to have the value of how many tick we have from the start (divided by 10000 since MSDN says
			//that 1 milisecond contain 10000 ticks)
			//double ticks = (DateTime.Now.Ticks - startTicks) / 10000;
			
    		line.timestamp = ticks;
			
    		//int n = sensor.capture(data_, NULL); 
			int n;
			data_ = sensor.GetFullGDData();
			if (data_ != null)
				n = data_.Count;
			else
				n = 0;
		
    		if (n <= 0) {
      			return;
    		}

    		// 強度データの取得     //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    		int intensity_n = 0; //sensor.captureWithIntensity(data_, intensity_data_, null); //!!!!!!!!!!!!!1
    		if (intensity_n == 0) {
      			// 強度データが返されないならば、モードを無効とみなす
      			intensity_mode_ = false;
    		}

    		// 測距データを２次元展開してから、Wii の向きに応じてさらに回転させる
    		for (int i = 0; i < n; ++i) {
      			int length = data_[i];
      			if ((length <= length_min_) || (length >= length_max_)) {
        			continue;
      			}

      			int index = i;     
      			double radian = sensor.Index2Rad(index); 
      			if (front_only_ && (System.Math.Abs(radian) > System.Math.PI / 2.0)) {
					// 前面以外のデータは無視する
					continue;	
      			}

      			Point3d<int> p = new Point3d<int>();
      			if (! h_type_) {
        			p.x = (int)(length * System.Math.Cos(radian));
        			p.y = (int)(length * System.Math.Sin(radian));
        			p.z = 0;
      			} else {
        			p.x = 0;
        			p.y = -(int)(length * System.Math.Cos(radian));
        			p.z = -(int)(length * System.Math.Sin(radian));
      			}

      			if (h_type_) { 
        			adjustTypeH(p, (int)(180.0 * radian / System.Math.PI)); 
      			}

      			// wiimote の角度に基づいた回転演算を行う
      			rotateX(p, line.rotate.x);
      			rotateY(p, line.rotate.y);
      			rotateY(p, line.rotate.z);

      			line.points.Add(p);
      			if (intensity_n > index) {
        			line.intensity.Add(intensity_data_[index]);
      			}	
    		}
  		}

		bool save_ok_;
		private void redrawCap()
		{
  			// Wii 姿勢の取得
  			Point3d<int> wii_rotate = new Point3d<int>();
			wii_rotate = getWiiRotate();
			//Console.WriteLine("WiiRotate Points: \n{0}\n{1}\n{2}\n ",wii_rotate.x,wii_rotate.y,wii_rotate.z);
			
  			// 記録するかどうか、の取得
			bool record = wii.Buttons == CWiid.BTN_FLAGS.BTN_B;
  			if (record && (!save_ok_)) {
    			save_ok_ = true;
    			//save_button_->setEnabled(true); //TODO: this only enable the UI ave button
  			}

  			// 現在のデータを表示するかどうか
  			bool no_plot = (wii.Buttons == CWiid.BTN_FLAGS.BTN_1) || (wii.Buttons == CWiid.BTN_FLAGS.BTN_2); 

  			// データの再描画
  			redraw(wii_rotate, record, no_plot);
		}

		private Point3d<int> getWiiRotate()
  		{
			Point3d<int> wii_rotate = new Point3d<int>();
			
    		if (wii.IsConnected) {

      			// 加速度情報の取得
      			Point3d<double> acc;

		      	// 回転角度の計算
				double length = System.Math.Sqrt((this.accx * this.accx) + (this.accz * this.accz));
				
		      	double x_rad = System.Math.Atan2(-this.accy, length);
		      	double z_rad = System.Math.Atan2(this.accz, this.accx);
				
		      	wii_rotate.x = -(int)(180 * z_rad / System.Math.PI) + 90;
		      	wii_rotate.y = -(int)(180 * x_rad / System.Math.PI);
		      	wii_rotate.z = 0;

    		} else {
      			wii_rotate.x = 0;
    		}
			
			return wii_rotate;
  		}
		
		private void rotateX(Point3d<int> point, int rotate_degree)
  		{
    		double radian = rotate_degree * System.Math.PI / 180.0;
    		double z2 = (point.z * System.Math.Cos(-radian)) - (point.y * System.Math.Sin(-radian));
    		double y2 = (point.z * System.Math.Sin(-radian)) + (point.y * System.Math.Cos(-radian));

    		point.z = (int)(z2);
    		point.y = (int)(y2);
  		}

  		private void rotateY(Point3d<int> point, int rotate_degree)
  		{
    		double radian = -rotate_degree * System.Math.PI / 180.0;
    		double z2 = (point.z * System.Math.Cos(-radian)) - (point.x * System.Math.Sin(-radian));
    		double x2 = (point.z * System.Math.Sin(-radian)) + (point.x * System.Math.Cos(-radian));

    		point.z = (int)(z2);
    		point.x = (int)(x2);
  		}

  		private void rotateZ(Point3d<int> point, int rotate_degree)
  		{
    		double radian = rotate_degree * System.Math.PI / 180.0;
    		double x2 = (point.x * System.Math.Cos(-radian)) - (point.y * System.Math.Sin(-radian));
    		double y2 = (point.x * System.Math.Sin(-radian)) + (point.y * System.Math.Cos(-radian));

    		point.x = (int)(x2);
    		point.y = (int)(y2);
  		}

		private  void adjustTypeH(Point3d<int> p, int degree)
  		{
    		if ((degree > -20) && (degree < 20)) {
      			// 前方は、Z 軸に対して、90 度ほど回転
      			//rotateZ(p, +90);
      			// !!! なぜか、-90 度にすると、適切に動作する？
      			rotateZ(p, -90);

      			// さらに、Y 軸の負方向に移動して、X 軸の負方向に移動させる
      			// !!!

      			//p = Point3d<int>(0, 0, 0);

    		} else if (degree < -45) {
      			// 右側は、Y 軸に対して、+60 度ほど回転
      			rotateY(p, +(60 + 15 + 0));

      			// さらに、Z 軸の正方向に移動して、X 軸の負方向に移動させる(誤差無視)
      			// !!!

    		} else if (degree > +45) {
      			// 左側は、Y 軸に対して、-60 度ほど回転
      			rotateY(p, -(60 + 15 + 0));

      			// さらに、Z 軸の負方向に移動して、X 軸の負方向に移動させる(誤差無視)
      			// !!!
    		} else {
      			// それ以外は、今回は利用しない
      			p = new Point3d<int>(0, 0, 0);
    		}
  		}

		private void addSaveLine(Line line)
  		{
    		saved_lines_data_.Add(line);
  		}

  		public void addTemporaryLine(Line line)
  		{
    		normal_lines_data_.Add(line);
  		}

		/// <summary>
		/// Convert HSV to RGB
		/// h is from 0-360
		/// s,v values are 0-1
		/// r,g,b values are 0-255
		/// Based upon http://ilab.usc.edu/wiki/index.php/HSV_And_H2SV_Color_Space#HSV_Transformation_C_.2F_C.2B.2B_Code_2
		/// </summary>
		public void HsvToRgb(double h, double S, double V, out int r, out int g, out int b)
		{
  			// ######################################################################
  			// T. Nathan Mundhenk
  			// mundhenk@usc.edu
  			// C/C++ Macro HSV to RGB

  			double H = h;
  			while (H < 0) { H += 360; };
  			while (H >= 360) { H -= 360; };
  			double R, G, B;
  			if (V <= 0)
    			{ R = G = B = 0; }
  			else if (S <= 0)
  			{
    			R = G = B = V;
  			}
  			else
  			{
    			double hf = H / 60.0;
    			int i = (int)Math.Floor(hf);
    			double f = hf - i;
    			double pv = V * (1 - S);
    			double qv = V * (1 - S * f);
    			double tv = V * (1 - S * (1 - f));
    			switch (i)
    			{

      			// Red is the dominant color

				case 0:
        			R = V;
        			G = tv;
        			B = pv;
        			break;

      			// Green is the dominant color

				case 1:
        			R = qv;
        			G = V;
        			B = pv;
        			break;
      			case 2:
        			R = pv;
        			G = V;
        			B = tv;
        			break;

      			// Blue is the dominant color

      			case 3:
        			R = pv;
        			G = qv;
        			B = V;
        			break;
      			case 4:
        			R = tv;
        			G = pv;
        			B = V;
        			break;	

      			// Red is the dominant color

				case 5:
        			R = V;
        			G = pv;
        			B = qv;
        			break;

      			// Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

      			case 6:
        			R = V;
        			G = tv;
        			B = pv;
        			break;
      			case -1:
        			R = V;
        			G = pv;
        			B = qv;
        			break;

      			// The color is not defined, we should throw an error.

      			default:
        		//LFATAL("i Value error in Pixel conversion, Value is %d", i);
        			R = G = B = V; // Just pretend its black/white
        			break;
    			}
  			}
  			r = Clamp((int)(R * 255.0));
  			g = Clamp((int)(G * 255.0));
  			b = Clamp((int)(B * 255.0));
		}

		private string fileName;
		private StreamWriter sw;
		private string directory = "Saved";
		public void saveVrml(string filename)
		{
			string header = 		"#VRML V2.0 utf8\n" +
    								"Shape\n" +
    								"{\n" +
    								"  geometry PointSet\n" +
    								"  {\n";
  			string coord_header =  "    coord Coordinate\n" +
    								"    {\n";
  			string point_header =  "      point\n" +
									"      [\n";
  			string point_footer =  "      ]\n";
  			string coord_footer =  "    }\n";
  			string color_header =  "    color Color\n" +
    								"    {\n" +
									"      color\n" +
    								"      [\n";
  			string color_footer = 	"      ]\n" +
    								"    }\n";
  			string footer = 		"  }\n" +
    								"}\n";
			
			//Create the Directory
			Directory.CreateDirectory (this.directory);
			this.fileName = filename;
			//Create the File
			//Create the File
			if (File.Exists(this.directory+"/"+this.fileName)) 
			{
				Console.WriteLine("{0} already exists.", this.fileName);
				File.Delete(this.directory+"/"+this.fileName);
			}
			Console.WriteLine("Creating File...\n");
			try{
				sw = File.CreateText(this.directory+"/"+this.fileName);// FileMode.Create,FileAccess.Write,FileShare.None);
			
				//string str = header + coord_header + point_header + "        # begin points.\n";
				sw.WriteLine(header);
				sw.WriteLine(coord_header);
				sw.WriteLine(point_header);
				sw.WriteLine("        # begin points.\n");
				//TRANSLATE THIS!
				//for (Lines::iterator line_it = pimpl->saved_lines_data_.begin();
       			//line_it != pimpl->saved_lines_data_.end(); ++line_it) {
    			//for (Points::iterator it = line_it->points.begin();
         		//it != line_it->points.end(); ++it) {
			
				foreach (Line l in this.saved_lines_data_)
					foreach(Point3d<int> p in l.points){
						//Console.WriteLine("x: "+p.x+"y: "+p.y+"z: "+p.z);
						sw.WriteLine("        "+ p.x+" "+p.y+" "+p.z+"\n");
						sw.Flush();
					}
			
				sw.WriteLine("        # end points.\n"+point_footer+coord_footer+color_header);
							
				//TRANSLATE ALSO THIS!
				//for (Lines::iterator line_it = pimpl->saved_lines_data_.begin();
       			//line_it != pimpl->saved_lines_data_.end(); ++line_it) {
    			//for (Points::iterator it = line_it->points.begin();
         		//it != line_it->points.end(); ++it) {
			
				//AND THIS!
      			//int mm = ((it->z % 1000) + 1000) % 1000;
      			//Point3d<double>& color = pimpl->color_table_[mm];
      			//fprintf(fd, "        %f %f %f,\n", color.x, color.y, color.z);
		    	//}
  				//}
				//Console.WriteLine("Writing File...\n");
				//str += color_footer + footer;
				sw.WriteLine(color_footer);
				sw.WriteLine(footer);
				//sw.WriteLine(str);
				sw.Close();
				Console.WriteLine("TAMOS LISTOS");
			}catch (Exception e){
				Console.WriteLine(e.Message);
			}
		}
		
		/// <summary>
		/// Clamp a value to 0-255
		/// </summary>
		public int Clamp(int i)
		{
	  		if (i < 0) return 0;
	  		if (i > 255) return 255;
	  		return i;
		}

				
	}
	
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
	
	//On this link there is info about the C++ "vector" and "deque" types (mostly like "List" C# one..)
	//http://www.cplusplus.com/reference/stl/deque/deque/
	
	//typedef vector<Point3d<int> > Points;
	public class Points: List<Point3d<int>>
	{
	}
	
	public class Line
  	{
    	public Points points = new Points();
    	public Point3d<int> rotate = new Point3d<int>();
    	public double timestamp;
    	//vector<long> intensity;
		public List<int> intensity = new List<int>();
  	}
	
	//typedef deque<Line> Lines;
	public class Lines: List<Line>
	{
	}
}
