// MIT License

// Copyright (c) 2017 Susumu Tanaka

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ForDAS
{
  public partial class ForDAS : Form
  {
    public ForDAS()
    {
      InitializeComponent();
    }

    private void ForDAS_Load(object sender, EventArgs e)
    {
      calc_mass();
    }

    private void checkBox_mass_direct_Click(object sender, EventArgs e)
    {
      if (checkBox_mass_direct.Checked == false)
      {
        textBox_ms.ReadOnly = true;
        textBox_mox.ReadOnly = true;
        textBox_mf.ReadOnly = true;
        textBox_mox_dot.ReadOnly = true;
        textBox_mf_dot.ReadOnly = true;

        textBox_ms_rate.ReadOnly = false;
        textBox_Isp.ReadOnly = false;
        textBox_O_F.ReadOnly = false;
        textBox_tb.ReadOnly = false;
      }
      if (checkBox_mass_direct.Checked == true)
      {
        textBox_ms.ReadOnly = false;
        textBox_mox.ReadOnly = false;
        textBox_mf.ReadOnly = false;
        textBox_mox_dot.ReadOnly = false;
        textBox_mf_dot.ReadOnly = false;

        textBox_ms_rate.ReadOnly = true;
        textBox_Isp.ReadOnly = true;
        textBox_O_F.ReadOnly = true;
        textBox_tb.ReadOnly = true;
      }

      calc_mass();
    }

    private void textBox_ms_rate_Leave(object sender, EventArgs e)
    {
      calc_mass();

    }

    private void textBox_thrust_Leave(object sender, EventArgs e)
    {
      calc_mass();

    }

    private void textBox_tb_Leave(object sender, EventArgs e)
    {
      calc_mass();

    }

    private void textBox_Isp_Leave(object sender, EventArgs e)
    {
      calc_mass();

    }

    private void textBox_O_F_Leave(object sender, EventArgs e)
    {
      calc_mass();

    }

    private void textBox_sliver_Leave(object sender, EventArgs e)
    {
      calc_mass();

    }

    private void textBox_mox_dot_Leave(object sender, EventArgs e)
    {
      if (checkBox_mass_direct.Checked == true)
      {
        calc_mass();

      }
    }

    private void textBox_mf_dot_Leave(object sender, EventArgs e)
    {
      if (checkBox_mass_direct.Checked == true)
      {
        calc_mass();

      }
    }

    private void calc_mass()
    {
      double mp_dot, mox_dot, mf_dot;
      double mox, mf, ms, m;

      if (checkBox_mass_direct.Checked == false)
      {
        mp_dot = double.Parse(textBox_thrust.Text) / (double.Parse(textBox_Isp.Text) * 9.80665);
        mox_dot = mp_dot * double.Parse(textBox_O_F.Text) / (double.Parse(textBox_O_F.Text) + 1.0);
        mf_dot = mp_dot - mox_dot;
        mox = mox_dot * double.Parse(textBox_tb.Text);
        mf = mf_dot * double.Parse(textBox_tb.Text) / (1.0 - double.Parse(textBox_sliver.Text) / 100.0);
        m = (mox + mf) / (100.0 - double.Parse(textBox_ms_rate.Text)) * 100.0;
        ms = m * double.Parse(textBox_ms_rate.Text) / 100.0;

        textBox_mox_dot.Text = mox_dot.ToString("F3");
        textBox_mf_dot.Text = mf_dot.ToString("F3");
        textBox_ms.Text = ms.ToString("F3");
        textBox_mox.Text = mox.ToString("F3");
        textBox_mf.Text = mf.ToString("F3");
        textBox_m.Text = m.ToString("F3");
      }
      else // true
      {
        ms = double.Parse(textBox_ms.Text);
        mox = double.Parse(textBox_mox.Text);
        mf = double.Parse(textBox_mf.Text);
        mox_dot = double.Parse(textBox_mox_dot.Text);
        mf_dot = double.Parse(textBox_mf_dot.Text);

        m = ms + mox + mf;
        mp_dot = mox_dot + mf_dot;

        textBox_m.Text = m.ToString("F3");
        textBox_ms_rate.Text = (100.0 * ms / m).ToString("F3");
        textBox_Isp.Text = (double.Parse(textBox_thrust.Text) / (mp_dot * 9.80665)).ToString("F3");
        textBox_O_F.Text = (mox_dot / mf_dot).ToString("F3");
        textBox_tb.Text = (Math.Min(mox / mox_dot, mf / mf_dot)).ToString("F3");
      }
           
      
    }

    private void button_simulation_Click(object sender, EventArgs e)
    {
      Object mainform = this;
      calc_mass();

      System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.AppStarting;

      Rocket Rocket = new Rocket();
      Rocket.d = double.Parse(textBox_diameter.Text);
      Rocket.Cd = double.Parse(textBox_Cd.Text);
      Rocket.thrust = double.Parse(textBox_thrust.Text);
      Rocket.tb = double.Parse(textBox_tb.Text);
      Rocket.de = double.Parse(textBox_de.Text);
      Rocket.ms = double.Parse(textBox_ms.Text);
      Rocket.mox = double.Parse(textBox_mox.Text);
      Rocket.mf = double.Parse(textBox_mf.Text);

      Solver Solver = new Solver(mainform);
      Solver.calculation(Rocket, double.Parse(textBox_theta.Text));

      
      textBox_Vol_ox.Text = (double.Parse(textBox_mox.Text) / double.Parse(textBox_rho_ox.Text)).ToString("F2");
      textBox_Rm.Text = (double.Parse(textBox_m.Text) / (double.Parse(textBox_ms.Text) + double.Parse(textBox_mf.Text))).ToString("F2");
      textBox_deltaV.Text = (double.Parse(textBox_Isp.Text) * 9.80665 * Math.Log(double.Parse(textBox_Rm.Text))).ToString("F2");
      textBox_Acclc_5m.Text = Solver.Acc_lc_5.ToString("F2");
      textBox_Vlc_5m.Text = Solver.V_lc_5.ToString("F2");
      textBox_Acclc10m.Text = Solver.Acc_lc_10.ToString("F2");
      textBox_Vlc_10m.Text = Solver.V_lc_10.ToString("F2");
      textBox_Vmax.Text = Solver.Va_max.ToString("F2");
      textBox_Machmax.Text = Solver.Mach_max.ToString("F2");
      textBox_Altitude.Text = Solver.Z_top.ToString("F2");
      textBox_t_flight.Text = (Solver.t_flight * 2.0).ToString("F2");
    }

    private void textBox_rho_ox_Leave(object sender, EventArgs e)
    {
      textBox_Vol_ox.Text = (double.Parse(textBox_mox.Text) / double.Parse(textBox_rho_ox.Text)).ToString("F2");
    }
  }

  public class Rocket
  {
    public double m { get; set; } //全備質量 [kg]    
    public double ms { get; set; }
    public double d { get; set; }
    public double A { get; set; } //[m^2]
    public double Cd { get; set; }

    public double mox { get; set; }
    public double mf { get; set; }
    public double thrust { get; set; }
    public double tb { get; set; }
    public double de { get; set; }
    public double Ae { get; set; } //nozzle exit

    public Rocket()
    {

    }
  }

  ////////////////////////////////////
  //シミュレータクラス
  //2次元平面内の質点並進運動のみ
  public class Solver
  {
    Object MainObject = null;
    standard_atmosphere Atmosphere = new standard_atmosphere();

    const double PI = 3.14159216;
    const double R = 287.1; //気体定数(空気)
    const double gamma = 1.4; //比熱比
    const double g0 = 9.80665; //重力加速度
    const double Re = 6356766.0;
    double g;
    double P_air; //大気圧[kPa]
    double T_air; //気温[K]
    double rho_air; //地上空気密度[kg/m^3]

    double t,dt;

    double theta;
    double Drag; //抗力[N]
    double Force; //荷重[N]
    double[] Acc = new double[2]; //加速度[m/s^2]
    double Acc_abs;
    double[] Va = new double[2]; //対気速度 [m/s]
    double Va_abs;
    double Mach, Cs;
    double[] Position = new double[2]; //[Downrange,Altitude] [m]

    public double V_lc_5 { get; set; }
    public double Acc_lc_5 { get; set; }
    public double V_lc_10 { get; set; }
    public double Acc_lc_10 { get; set; }
    public double Z_top { get; set; }
    public double X_top { get; set; }
    public double Va_max { get; set; }
    public double Mach_max { get; set; }
    public double t_flight { get; set; }
    double Altitude_pre;


    public Solver(Object mainform)
    {
      MainObject = mainform;
    }
    

    public void Initialize(Rocket Rocket, double theta_0)
    {
      dt = 1.0 / 300.0;
      t = 0.0;

      Rocket.A = utility.d2A(Rocket.d / 1000.0);
      Rocket.Ae = utility.d2A(Rocket.de / 1000.0);
      
      theta = utility.deg2rad(-theta_0);

      Drag = 0.0;
      Force = 0.0;
      Acc[0] = 0.0; Acc[1] = 0.0;
      Acc_abs = 0.0;
      Va[0] = 0.0; Va[1] = 0.0;
      Va_abs = 0.0;
      Position[0] = 0.0; Position[1] = 0.5;
      Z_top = 0.0; Va_max = 0.0; Mach_max = 0.0;
      Altitude_pre = 0.0;
    }
        
    /////////////////////
    //計算メソッド
    //呼び出しはこれ
    //引数：Rocketクラスのオブジェクト,射角[deg]
    public void calculation(Rocket Rocket, double theta_0)
    {
      Initialize(Rocket, theta_0);
      
      //////////////////////
      //飛しょうループ
      //頂点まで。降下を始めたら終了 Position > Altitude_pre
      /////////////////////
      for (t = 0; Position[1] > Altitude_pre; t = t + dt)
      {
        P_air = Atmosphere.getPressure(Position[1]);
        T_air = Atmosphere.getTemperature(Position[1]);
        rho_air = P_air / (R * T_air);

        g = g0 * Math.Pow(Re / (Re + Position[1]), 2);
        Cs = Atmosphere.getSoundofSpeed(Position[1]);

        //質量計算
        if (t < Rocket.tb)
        {          
          Rocket.m = Rocket.ms + (Rocket.mf - (Rocket.mf / Rocket.tb * t)) + (Rocket.mox - (Rocket.mox / Rocket.tb * t));
        }

        //荷重計算
        Drag = 0.5 * rho_air * Va_abs * Va_abs * Rocket.Cd * Rocket.A;
        if (t > Rocket.tb)
        {
          Force = -1.0 * Drag;
        }
        else
        {
          Force = Rocket.thrust + Rocket.Ae * (Atmosphere.getPressure(0.0) - P_air) - Drag;
        }
        
        //微分方程式
        Acc[0] = Force * Math.Cos(Math.Abs(theta)) / Rocket.m;
        Acc[1] = Force * Math.Sin(Math.Abs(theta)) / Rocket.m - g;
        Acc_abs = utility.AxisComposite(Acc);

        Va[0] = Va[0] + Acc[0] * dt;
        Va[1] = Va[1] + Acc[1] * dt;
        Va_abs = utility.AxisComposite(Va);
        Mach = Va_abs / Cs;

        Altitude_pre = Position[1];  
        Position[0] = Position[0] + Va[0] * dt;
        Position[1] = Position[1] + Va[1] * dt;
        
        theta = -1.0 * Math.Atan2(Va[1], Va[0]);

        if (Position[1]/Math.Sin(Math.Abs(utility.deg2rad(theta_0))) <= 5.0)
        {
          V_lc_5 = Va_abs; Acc_lc_5 = Acc_abs / g;
        }
        if (Position[1] / Math.Sin(Math.Abs(utility.deg2rad(theta_0))) <= 10.0)
        {
          V_lc_10 = Va_abs; Acc_lc_10 = Acc_abs / g;
        }
        if (Va_max < Va_abs)
        {
          Va_max = Va_abs;
        }
        if (Mach_max < Mach)
        {
          Mach_max = Mach;
        }
        if (Z_top < Position[1])
        {
          X_top = Position[0];
          Z_top = Position[1];
          t_flight = t;
        }
        
      }

    }

  }

  public class standard_atmosphere
  {
    double[,] table = new double[861, 5]{ { 0 , 15 , 101325 , 1.225 , 340.294 } ,
                                          { 100 , 14.35 , 100129 , 1.21328 , 339.910 } ,
                                          { 200 , 13.7 , 98945.3 , 1.20165 , 339.526 } ,
                                          { 300 , 13.05 , 97772.6 , 1.19011 , 339.141 } ,
                                          { 400 , 12.4 , 96611.1 , 1.17864 , 338.755 } ,
                                          { 500 , 11.75 , 95460.8 , 1.16727 , 338.370 } ,
                                          { 600 , 11.1 , 94321.7 , 1.15598 , 337.983 } ,
                                          { 700 , 10.45 , 93193.6 , 1.14477 , 337.597 } ,
                                          { 800 , 9.8 , 92076.4 , 1.13364 , 337.210 } ,
                                          { 900 , 9.15 , 90970.1 , 1.1226 , 336.822 } ,
                                          { 1000 , 8.5 , 89874.6 , 1.11164 , 336.434 } ,
                                          { 1100 , 7.85 , 88789.8 , 1.10077 , 336.046 } ,
                                          { 1200 , 7.2 , 87715.6 , 1.08997 , 335.657 } ,
                                          { 1300 , 6.55 , 86651.9 , 1.07925 , 335.267 } ,
                                          { 1400 , 5.9 , 85598.8 , 1.06862 , 334.878 } ,
                                          { 1500 , 5.25 , 84556 , 1.05807 , 334.487 } ,
                                          { 1600 , 4.6 , 83523.5 , 1.04759 , 334.097 } ,
                                          { 1700 , 3.95 , 82501.3 , 1.0372 , 333.705 } ,
                                          { 1800 , 3.3 , 81489.2 , 1.02688 , 333.314 } ,
                                          { 1900 , 2.65 , 80487.2 , 1.01665 , 332.922 } ,
                                          { 2000 , 2 , 79495.2 , 1.00649 , 332.529 } ,
                                          { 2100 , 1.35 , 78513.1 , 0.99641 , 332.136 } ,
                                          { 2200 , 0.7 , 77540.9 , 0.986407 , 331.743 } ,
                                          { 2300 , 0.05 , 76578.4 , 0.976481 , 331.349 } ,
                                          { 2400 , -0.6 , 75625.7 , 0.966632 , 330.954 } ,
                                          { 2500 , -1.25 , 74682.5 , 0.956859 , 330.560 } ,
                                          { 2600 , -1.9 , 73748.9 , 0.947162 , 330.164 } ,
                                          { 2700 , -2.55 , 72824.8 , 0.93754 , 329.768 } ,
                                          { 2800 , -3.2 , 71910.1 , 0.927993 , 329.372 } ,
                                          { 2900 , -3.85 , 71004.7 , 0.91852 , 328.975 } ,
                                          { 3000 , -4.5 , 70108.5 , 0.909122 , 328.578 } ,
                                          { 3100 , -5.15 , 69221.6 , 0.899798 , 328.180 } ,
                                          { 3200 , -5.8 , 68343.7 , 0.890546 , 327.782 } ,
                                          { 3300 , -6.45 , 67474.9 , 0.881368 , 327.383 } ,
                                          { 3400 , -7.1 , 66615 , 0.872262 , 326.984 } ,
                                          { 3500 , -7.75 , 65764.1 , 0.863229 , 326.584 } ,
                                          { 3600 , -8.4 , 64921.9 , 0.854267 , 326.184 } ,
                                          { 3700 , -9.05 , 64088.6 , 0.845377 , 325.784 } ,
                                          { 3800 , -9.7 , 63263.9 , 0.836557 , 325.382 } ,
                                          { 3900 , -10.35 , 62447.8 , 0.827808 , 324.981 } ,
                                          { 4000 , -11 , 61640.2 , 0.819129 , 324.579 } ,
                                          { 4100 , -11.65 , 60841.2 , 0.81052 , 324.176 } ,
                                          { 4200 , -12.3 , 60050.5 , 0.801981 , 323.773 } ,
                                          { 4300 , -12.95 , 59268.2 , 0.79351 , 323.369 } ,
                                          { 4400 , -13.6 , 58494.2 , 0.785108 , 322.965 } ,
                                          { 4500 , -14.25 , 57728.3 , 0.776775 , 322.560 } ,
                                          { 4600 , -14.9 , 56970.6 , 0.768509 , 322.155 } ,
                                          { 4700 , -15.55 , 56221 , 0.76031 , 321.750 } ,
                                          { 4800 , -16.2 , 55479.4 , 0.752179 , 321.343 } ,
                                          { 4900 , -16.85 , 54745.7 , 0.744114 , 320.937 } ,
                                          { 5000 , -17.5 , 54019.9 , 0.736116 , 320.529 } ,
                                          { 5100 , -18.15 , 53301.9 , 0.728183 , 320.122 } ,
                                          { 5200 , -18.8 , 52591.7 , 0.720317 , 319.713 } ,
                                          { 5300 , -19.45 , 51889.1 , 0.712515 , 319.305 } ,
                                          { 5400 , -20.1 , 51194.2 , 0.704778 , 318.895 } ,
                                          { 5500 , -20.75 , 50506.8 , 0.697106 , 318.486 } ,
                                          { 5600 , -21.4 , 49826.9 , 0.689497 , 318.075 } ,
                                          { 5700 , -22.05 , 49154.5 , 0.681953 , 317.664 } ,
                                          { 5800 , -22.7 , 48489.4 , 0.674471 , 317.253 } ,
                                          { 5900 , -23.35 , 47831.6 , 0.667053 , 316.841 } ,
                                          { 6000 , -24 , 47181 , 0.659697 , 316.428 } ,
                                          { 6100 , -24.65 , 46537.7 , 0.652404 , 316.015 } ,
                                          { 6200 , -25.3 , 45901.4 , 0.645172 , 315.602 } ,
                                          { 6300 , -25.95 , 45272.3 , 0.638002 , 315.188 } ,
                                          { 6400 , -26.6 , 44650.1 , 0.630892 , 314.773 } ,
                                          { 6500 , -27.25 , 44034.8 , 0.623844 , 314.358 } ,
                                          { 6600 , -27.9 , 43426.5 , 0.616856 , 313.942 } ,
                                          { 6700 , -28.55 , 42825 , 0.609928 , 313.526 } ,
                                          { 6800 , -29.2 , 42230.2 , 0.60306 , 313.109 } ,
                                          { 6900 , -29.85 , 41642.2 , 0.596251 , 312.692 } ,
                                          { 7000 , -30.5 , 41060.7 , 0.589501 , 312.274 } ,
                                          { 7100 , -31.15 , 40485.9 , 0.58281 , 311.855 } ,
                                          { 7200 , -31.8 , 39917.6 , 0.576177 , 311.436 } ,
                                          { 7300 , -32.45 , 39355.8 , 0.569602 , 311.016 } ,
                                          { 7400 , -33.1 , 38800.4 , 0.563084 , 310.596 } ,
                                          { 7500 , -33.75 , 38251.4 , 0.556624 , 310.175 } ,
                                          { 7600 , -34.4 , 37708.7 , 0.55022 , 309.754 } ,
                                          { 7700 , -35.05 , 37172.2 , 0.543873 , 309.332 } ,
                                          { 7800 , -35.7 , 36642 , 0.537582 , 308.909 } ,
                                          { 7900 , -36.35 , 36117.8 , 0.531347 , 308.486 } ,
                                          { 8000 , -37 , 35599.8 , 0.525168 , 308.063 } ,
                                          { 8100 , -37.65 , 35087.8 , 0.519043 , 307.638 } ,
                                          { 8200 , -38.3 , 34581.8 , 0.512973 , 307.214 } ,
                                          { 8300 , -38.95 , 34081.7 , 0.506958 , 306.788 } ,
                                          { 8400 , -39.6 , 33587.5 , 0.500997 , 306.362 } ,
                                          { 8500 , -40.25 , 33099 , 0.49509 , 305.935 } ,
                                          { 8600 , -40.9 , 32616.4 , 0.489236 , 305.508 } ,
                                          { 8700 , -41.55 , 32139.5 , 0.483435 , 305.080 } ,
                                          { 8800 , -42.2 , 31668.2 , 0.477687 , 304.652 } ,
                                          { 8900 , -42.85 , 31202.6 , 0.471992 , 304.223 } ,
                                          { 9000 , -43.5 , 30742.5 , 0.466348 , 303.793 } ,
                                          { 9100 , -44.15 , 30287.9 , 0.460756 , 303.363 } ,
                                          { 9200 , -44.8 , 29838.7 , 0.455216 , 302.932 } ,
                                          { 9300 , -45.45 , 29395 , 0.449727 , 302.501 } ,
                                          { 9400 , -46.1 , 28956.7 , 0.444289 , 302.069 } ,
                                          { 9500 , -46.75 , 28523.6 , 0.438901 , 301.636 } ,
                                          { 9600 , -47.4 , 28095.8 , 0.433563 , 301.203 } ,
                                          { 9700 , -48.05 , 27673.2 , 0.428275 , 300.769 } ,
                                          { 9800 , -48.7 , 27255.8 , 0.423036 , 300.334 } ,
                                          { 9900 , -49.35 , 26843.5 , 0.417847 , 299.899 } ,
                                          { 10000 , -50 , 26436.3 , 0.412707 , 299.463 } ,
                                          { 10100 , -50.65 , 26034 , 0.407615 , 299.027 } ,
                                          { 10200 , -51.3 , 25636.8 , 0.402571 , 298.590 } ,
                                          { 10300 , -51.95 , 25244.5 , 0.397575 , 298.152 } ,
                                          { 10400 , -52.6 , 24857 , 0.392627 , 297.714 } ,
                                          { 10500 , -53.25 , 24474.4 , 0.387725 , 297.275 } ,
                                          { 10600 , -53.9 , 24096.5 , 0.382871 , 296.835 } ,
                                          { 10700 , -54.55 , 23723.4 , 0.378064 , 296.395 } ,
                                          { 10800 , -55.2 , 23355 , 0.373303 , 295.954 } ,
                                          { 10900 , -55.85 , 22991.2 , 0.368588 , 295.512 } ,
                                          { 11000 , -56.5 , 22632.1 , 0.363918 , 295.070 } ,
                                          { 11100 , -56.5 , 22278 , 0.358224 , 295.070 } ,
                                          { 11200 , -56.5 , 21929.4 , 0.35262 , 295.070 } ,
                                          { 11300 , -56.5 , 21586.3 , 0.347103 , 295.070 } ,
                                          { 11400 , -56.5 , 21248.6 , 0.341673 , 295.070 } ,
                                          { 11500 , -56.5 , 20916.2 , 0.336327 , 295.070 } ,
                                          { 11600 , -56.5 , 20589 , 0.331065 , 295.070 } ,
                                          { 11700 , -56.5 , 20266.8 , 0.325886 , 295.070 } ,
                                          { 11800 , -56.5 , 19949.8 , 0.320787 , 295.070 } ,
                                          { 11900 , -56.5 , 19637.6 , 0.315768 , 295.070 } ,
                                          { 12000 , -56.5 , 19330.4 , 0.310828 , 295.070 } ,
                                          { 12100 , -56.5 , 19028 , 0.305965 , 295.070 } ,
                                          { 12200 , -56.5 , 18730.3 , 0.301178 , 295.070 } ,
                                          { 12300 , -56.5 , 18437.2 , 0.296466 , 295.070 } ,
                                          { 12400 , -56.5 , 18148.8 , 0.291828 , 295.070 } ,
                                          { 12500 , -56.5 , 17864.8 , 0.287262 , 295.070 } ,
                                          { 12600 , -56.5 , 17585.4 , 0.282768 , 295.070 } ,
                                          { 12700 , -56.5 , 17310.2 , 0.278344 , 295.070 } ,
                                          { 12800 , -56.5 , 17039.4 , 0.273989 , 295.070 } ,
                                          { 12900 , -56.5 , 16772.8 , 0.269703 , 295.070 } ,
                                          { 13000 , -56.5 , 16510.4 , 0.265483 , 295.070 } ,
                                          { 13100 , -56.5 , 16252.1 , 0.26133 , 295.070 } ,
                                          { 13200 , -56.5 , 15997.8 , 0.257241 , 295.070 } ,
                                          { 13300 , -56.5 , 15747.5 , 0.253217 , 295.070 } ,
                                          { 13400 , -56.5 , 15501.2 , 0.249255 , 295.070 } ,
                                          { 13500 , -56.5 , 15258.7 , 0.245355 , 295.070 } ,
                                          { 13600 , -56.5 , 15019.9 , 0.241517 , 295.070 } ,
                                          { 13700 , -56.5 , 14784.9 , 0.237738 , 295.070 } ,
                                          { 13800 , -56.5 , 14553.6 , 0.234019 , 295.070 } ,
                                          { 13900 , -56.5 , 14325.9 , 0.230357 , 295.070 } ,
                                          { 14000 , -56.5 , 14101.8 , 0.226753 , 295.070 } ,
                                          { 14100 , -56.5 , 13881.2 , 0.223206 , 295.070 } ,
                                          { 14200 , -56.5 , 13664 , 0.219714 , 295.070 } ,
                                          { 14300 , -56.5 , 13450.2 , 0.216276 , 295.070 } ,
                                          { 14400 , -56.5 , 13239.8 , 0.212893 , 295.070 } ,
                                          { 14500 , -56.5 , 13032.7 , 0.209562 , 295.070 } ,
                                          { 14600 , -56.5 , 12828.8 , 0.206283 , 295.070 } ,
                                          { 14700 , -56.5 , 12628.1 , 0.203056 , 295.070 } ,
                                          { 14800 , -56.5 , 12430.5 , 0.199879 , 295.070 } ,
                                          { 14900 , -56.5 , 12236 , 0.196752 , 295.070 } ,
                                          { 15000 , -56.5 , 12044.6 , 0.193674 , 295.070 } ,
                                          { 15100 , -56.5 , 11856.1 , 0.190644 , 295.070 } ,
                                          { 15200 , -56.5 , 11670.6 , 0.187661 , 295.070 } ,
                                          { 15300 , -56.5 , 11488.1 , 0.184725 , 295.070 } ,
                                          { 15400 , -56.5 , 11308.3 , 0.181835 , 295.070 } ,
                                          { 15500 , -56.5 , 11131.4 , 0.17899 , 295.070 } ,
                                          { 15600 , -56.5 , 10957.2 , 0.17619 , 295.070 } ,
                                          { 15700 , -56.5 , 10785.8 , 0.173433 , 295.070 } ,
                                          { 15800 , -56.5 , 10617.1 , 0.17072 , 295.070 } ,
                                          { 15900 , -56.5 , 10451 , 0.168049 , 295.070 } ,
                                          { 16000 , -56.5 , 10287.5 , 0.16542 , 295.070 } ,
                                          { 16100 , -56.5 , 10126.5 , 0.162832 , 295.070 } ,
                                          { 16200 , -56.5 , 9968.08 , 0.160284 , 295.070 } ,
                                          { 16300 , -56.5 , 9812.13 , 0.157777 , 295.070 } ,
                                          { 16400 , -56.5 , 9658.61 , 0.155308 , 295.070 } ,
                                          { 16500 , -56.5 , 9507.5 , 0.152878 , 295.070 } ,
                                          { 16600 , -56.5 , 9358.76 , 0.150487 , 295.070 } ,
                                          { 16700 , -56.5 , 9212.34 , 0.148132 , 295.070 } ,
                                          { 16800 , -56.5 , 9068.21 , 0.145815 , 295.070 } ,
                                          { 16900 , -56.5 , 8926.34 , 0.143533 , 295.070 } ,
                                          { 17000 , -56.5 , 8786.68 , 0.141288 , 295.070 } ,
                                          { 17100 , -56.5 , 8649.21 , 0.139077 , 295.070 } ,
                                          { 17200 , -56.5 , 8513.89 , 0.136901 , 295.070 } ,
                                          { 17300 , -56.5 , 8380.69 , 0.134759 , 295.070 } ,
                                          { 17400 , -56.5 , 8249.58 , 0.132651 , 295.070 } ,
                                          { 17500 , -56.5 , 8120.51 , 0.130576 , 295.070 } ,
                                          { 17600 , -56.5 , 7993.46 , 0.128533 , 295.070 } ,
                                          { 17700 , -56.5 , 7868.4 , 0.126522 , 295.070 } ,
                                          { 17800 , -56.5 , 7745.3 , 0.124543 , 295.070 } ,
                                          { 17900 , -56.5 , 7624.13 , 0.122594 , 295.070 } ,
                                          { 18000 , -56.5 , 7504.84 , 0.120676 , 295.070 } ,
                                          { 18100 , -56.5 , 7387.43 , 0.118788 , 295.070 } ,
                                          { 18200 , -56.5 , 7271.85 , 0.11693 , 295.070 } ,
                                          { 18300 , -56.5 , 7158.08 , 0.1151 , 295.070 } ,
                                          { 18400 , -56.5 , 7046.09 , 0.113299 , 295.070 } ,
                                          { 18500 , -56.5 , 6935.86 , 0.111527 , 295.070 } ,
                                          { 18600 , -56.5 , 6827.34 , 0.109782 , 295.070 } ,
                                          { 18700 , -56.5 , 6720.53 , 0.108064 , 295.070 } ,
                                          { 18800 , -56.5 , 6615.39 , 0.106374 , 295.070 } ,
                                          { 18900 , -56.5 , 6511.89 , 0.10471 , 295.070 } ,
                                          { 19000 , -56.5 , 6410.01 , 0.103071 , 295.070 } ,
                                          { 19100 , -56.5 , 6309.72 , 0.101459 , 295.070 } ,
                                          { 19200 , -56.5 , 6211 , 0.0998714 , 295.070 } ,
                                          { 19300 , -56.5 , 6113.83 , 0.0983089 , 295.070 } ,
                                          { 19400 , -56.5 , 6018.18 , 0.0967709 , 295.070 } ,
                                          { 19500 , -56.5 , 5924.03 , 0.0952569 , 295.070 } ,
                                          { 19600 , -56.5 , 5831.34 , 0.0937666 , 295.070 } ,
                                          { 19700 , -56.5 , 5740.11 , 0.0922996 , 295.070 } ,
                                          { 19800 , -56.5 , 5650.31 , 0.0908555 , 295.070 } ,
                                          { 19900 , -56.5 , 5561.91 , 0.0894341 , 295.070 } ,
                                          { 20000 , -56.5 , 5474.89 , 0.0880349 , 295.070 } ,
                                          { 20100 , -56.4 , 5389.25 , 0.0866179 , 295.138 } ,
                                          { 20200 , -56.3 , 5304.99 , 0.0852243 , 295.206 } ,
                                          { 20300 , -56.2 , 5222.09 , 0.0838538 , 295.274 } ,
                                          { 20400 , -56.1 , 5140.52 , 0.082506 , 295.342 } ,
                                          { 20500 , -56 , 5060.26 , 0.0811804 , 295.410 } ,
                                          { 20600 , -55.9 , 4981.29 , 0.0798768 , 295.478 } ,
                                          { 20700 , -55.8 , 4903.59 , 0.0785946 , 295.546 } ,
                                          { 20800 , -55.7 , 4827.14 , 0.0773336 , 295.614 } ,
                                          { 20900 , -55.6 , 4751.91 , 0.0760934 , 295.682 } ,
                                          { 21000 , -55.5 , 4677.89 , 0.0748737 , 295.750 } ,
                                          { 21100 , -55.4 , 4605.05 , 0.073674 , 295.818 } ,
                                          { 21200 , -55.3 , 4533.38 , 0.0724941 , 295.886 } ,
                                          { 21300 , -55.2 , 4462.86 , 0.0713336 , 295.954 } ,
                                          { 21400 , -55.1 , 4393.47 , 0.0701923 , 296.021 } ,
                                          { 21500 , -55 , 4325.18 , 0.0690697 , 296.089 } ,
                                          { 21600 , -54.9 , 4257.99 , 0.0679655 , 296.157 } ,
                                          { 21700 , -54.8 , 4191.87 , 0.0668795 , 296.225 } ,
                                          { 21800 , -54.7 , 4126.81 , 0.0658114 , 296.293 } ,
                                          { 21900 , -54.6 , 4062.79 , 0.0647607 , 296.361 } ,
                                          { 22000 , -54.5 , 3999.79 , 0.0637273 , 296.428 } ,
                                          { 22100 , -54.4 , 3937.79 , 0.0627109 , 296.496 } ,
                                          { 22200 , -54.3 , 3876.79 , 0.0617111 , 296.564 } ,
                                          { 22300 , -54.2 , 3816.75 , 0.0607278 , 296.632 } ,
                                          { 22400 , -54.1 , 3757.68 , 0.0597605 , 296.699 } ,
                                          { 22500 , -54 , 3699.54 , 0.0588091 , 296.767 } ,
                                          { 22600 , -53.9 , 3642.33 , 0.0578732 , 296.835 } ,
                                          { 22700 , -53.8 , 3586.03 , 0.0569526 , 296.903 } ,
                                          { 22800 , -53.7 , 3530.62 , 0.0560471 , 296.970 } ,
                                          { 22900 , -53.6 , 3476.09 , 0.0551564 , 297.038 } ,
                                          { 23000 , -53.5 , 3422.43 , 0.0542803 , 297.105 } ,
                                          { 23100 , -53.4 , 3369.63 , 0.0534184 , 297.173 } ,
                                          { 23200 , -53.3 , 3317.66 , 0.0525706 , 297.241 } ,
                                          { 23300 , -53.2 , 3266.51 , 0.0517367 , 297.308 } ,
                                          { 23400 , -53.1 , 3216.18 , 0.0509164 , 297.376 } ,
                                          { 23500 , -53 , 3166.65 , 0.0501094 , 297.443 } ,
                                          { 23600 , -52.9 , 3117.9 , 0.0493155 , 297.511 } ,
                                          { 23700 , -52.8 , 3069.92 , 0.0485346 , 297.579 } ,
                                          { 23800 , -52.7 , 3022.7 , 0.0477665 , 297.646 } ,
                                          { 23900 , -52.6 , 2976.23 , 0.0470108 , 297.714 } ,
                                          { 24000 , -52.5 , 2930.49 , 0.0462674 , 297.781 } ,
                                          { 24100 , -52.4 , 2885.48 , 0.045536 , 297.849 } ,
                                          { 24200 , -52.3 , 2841.18 , 0.0448166 , 297.916 } ,
                                          { 24300 , -52.2 , 2797.58 , 0.0441089 , 297.983 } ,
                                          { 24400 , -52.1 , 2754.66 , 0.0434126 , 298.051 } ,
                                          { 24500 , -52 , 2712.42 , 0.0427276 , 298.118 } ,
                                          { 24600 , -51.9 , 2670.85 , 0.0420538 , 298.186 } ,
                                          { 24700 , -51.8 , 2629.94 , 0.0413909 , 298.253 } ,
                                          { 24800 , -51.7 , 2589.67 , 0.0407387 , 298.320 } ,
                                          { 24900 , -51.6 , 2550.03 , 0.0400971 , 298.388 } ,
                                          { 25000 , -51.5 , 2511.02 , 0.0394658 , 298.455 } ,
                                          { 25100 , -51.4 , 2472.63 , 0.0388448 , 298.522 } ,
                                          { 25200 , -51.3 , 2434.83 , 0.0382338 , 298.590 } ,
                                          { 25300 , -51.2 , 2397.63 , 0.0376327 , 298.657 } ,
                                          { 25400 , -51.1 , 2361.02 , 0.0370414 , 298.724 } ,
                                          { 25500 , -51 , 2324.98 , 0.0364595 , 298.791 } ,
                                          { 25600 , -50.9 , 2289.51 , 0.0358871 , 298.859 } ,
                                          { 25700 , -50.8 , 2254.59 , 0.0353239 , 298.926 } ,
                                          { 25800 , -50.7 , 2220.22 , 0.0347698 , 298.993 } ,
                                          { 25900 , -50.6 , 2186.39 , 0.0342246 , 299.060 } ,
                                          { 26000 , -50.5 , 2153.09 , 0.0336882 , 299.128 } ,
                                          { 26100 , -50.4 , 2120.32 , 0.0331605 , 299.195 } ,
                                          { 26200 , -50.3 , 2088.05 , 0.0326413 , 299.262 } ,
                                          { 26300 , -50.2 , 2056.29 , 0.0321304 , 299.329 } ,
                                          { 26400 , -50.1 , 2025.03 , 0.0316277 , 299.396 } ,
                                          { 26500 , -50 , 1994.26 , 0.0311331 , 299.463 } ,
                                          { 26600 , -49.9 , 1963.97 , 0.0306465 , 299.530 } ,
                                          { 26700 , -49.8 , 1934.15 , 0.0301677 , 299.597 } ,
                                          { 26800 , -49.7 , 1904.8 , 0.0296966 , 299.664 } ,
                                          { 26900 , -49.6 , 1875.9 , 0.029233 , 299.732 } ,
                                          { 27000 , -49.5 , 1847.46 , 0.0287769 , 299.799 } ,
                                          { 27100 , -49.4 , 1819.46 , 0.0283281 , 299.866 } ,
                                          { 27200 , -49.3 , 1791.89 , 0.0278865 , 299.933 } ,
                                          { 27300 , -49.2 , 1764.76 , 0.0274519 , 300.000 } ,
                                          { 27400 , -49.1 , 1738.05 , 0.0270244 , 300.067 } ,
                                          { 27500 , -49 , 1711.75 , 0.0266036 , 300.133 } ,
                                          { 27600 , -48.9 , 1685.87 , 0.0261896 , 300.200 } ,
                                          { 27700 , -48.8 , 1660.39 , 0.0257823 , 300.267 } ,
                                          { 27800 , -48.7 , 1635.3 , 0.0253814 , 300.334 } ,
                                          { 27900 , -48.6 , 1610.6 , 0.024987 , 300.401 } ,
                                          { 28000 , -48.5 , 1586.29 , 0.0245988 , 300.468 } ,
                                          { 28100 , -48.4 , 1562.35 , 0.0242169 , 300.535 } ,
                                          { 28200 , -48.3 , 1538.79 , 0.023841 , 300.602 } ,
                                          { 28300 , -48.2 , 1515.59 , 0.0234712 , 300.669 } ,
                                          { 28400 , -48.1 , 1492.75 , 0.0231072 , 300.735 } ,
                                          { 28500 , -48 , 1470.27 , 0.022749 , 300.802 } ,
                                          { 28600 , -47.9 , 1448.13 , 0.0223966 , 300.869 } ,
                                          { 28700 , -47.8 , 1426.34 , 0.0220498 , 300.936 } ,
                                          { 28800 , -47.7 , 1404.89 , 0.0217084 , 301.003 } ,
                                          { 28900 , -47.6 , 1383.76 , 0.0213726 , 301.069 } ,
                                          { 29000 , -47.5 , 1362.96 , 0.021042 , 301.136 } ,
                                          { 29100 , -47.4 , 1342.49 , 0.0207167 , 301.203 } ,
                                          { 29200 , -47.3 , 1322.33 , 0.0203966 , 301.269 } ,
                                          { 29300 , -47.2 , 1302.48 , 0.0200816 , 301.336 } ,
                                          { 29400 , -47.1 , 1282.94 , 0.0197716 , 301.403 } ,
                                          { 29500 , -47 , 1263.7 , 0.0194664 , 301.469 } ,
                                          { 29600 , -46.9 , 1244.76 , 0.0191662 , 301.536 } ,
                                          { 29700 , -46.8 , 1226.11 , 0.0188707 , 301.603 } ,
                                          { 29800 , -46.7 , 1207.75 , 0.0185798 , 301.669 } ,
                                          { 29900 , -46.6 , 1189.67 , 0.0182936 , 301.736 } ,
                                          { 30000 , -46.5 , 1171.87 , 0.0180119 , 301.803 } ,
                                          { 30100 , -46.4 , 1154.34 , 0.0177347 , 301.869 } ,
                                          { 30200 , -46.3 , 1137.08 , 0.0174619 , 301.936 } ,
                                          { 30300 , -46.2 , 1120.09 , 0.0171934 , 302.002 } ,
                                          { 30400 , -46.1 , 1103.36 , 0.0169291 , 302.069 } ,
                                          { 30500 , -46 , 1086.88 , 0.016669 , 302.135 } ,
                                          { 30600 , -45.9 , 1070.66 , 0.016413 , 302.202 } ,
                                          { 30700 , -45.8 , 1054.69 , 0.016161 , 302.268 } ,
                                          { 30800 , -45.7 , 1038.97 , 0.015913 , 302.335 } ,
                                          { 30900 , -45.6 , 1023.48 , 0.015669 , 302.401 } ,
                                          { 31000 , -45.5 , 1008.23 , 0.0154288 , 302.468 } ,
                                          { 31100 , -45.4 , 993.218 , 0.0151923 , 302.534 } ,
                                          { 31200 , -45.3 , 978.434 , 0.0149596 , 302.600 } ,
                                          { 31300 , -45.2 , 963.876 , 0.0147306 , 302.667 } ,
                                          { 31400 , -45.1 , 949.541 , 0.0145051 , 302.733 } ,
                                          { 31500 , -45 , 935.425 , 0.0142832 , 302.800 } ,
                                          { 31600 , -44.9 , 921.526 , 0.0140648 , 302.866 } ,
                                          { 31700 , -44.8 , 907.838 , 0.0138499 , 302.932 } ,
                                          { 31800 , -44.7 , 894.36 , 0.0136383 , 302.999 } ,
                                          { 31900 , -44.6 , 881.088 , 0.01343 , 303.065 } ,
                                          { 32000 , -44.5 , 868.019 , 0.013225 , 303.131 } ,
                                          { 32100 , -44.22 , 855.154 , 0.0130131 , 303.317 } ,
                                          { 32200 , -43.94 , 842.495 , 0.0128048 , 303.502 } ,
                                          { 32300 , -43.66 , 830.038 , 0.0126001 , 303.688 } ,
                                          { 32400 , -43.38 , 817.781 , 0.0123989 , 303.873 } ,
                                          { 32500 , -43.1 , 805.719 , 0.0122011 , 304.058 } ,
                                          { 32600 , -42.82 , 793.849 , 0.0120068 , 304.243 } ,
                                          { 32700 , -42.54 , 782.168 , 0.0118157 , 304.428 } ,
                                          { 32800 , -42.26 , 770.674 , 0.011628 , 304.612 } ,
                                          { 32900 , -41.98 , 759.361 , 0.0114434 , 304.797 } ,
                                          { 33000 , -41.7 , 748.228 , 0.011262 , 304.982 } ,
                                          { 33100 , -41.42 , 737.272 , 0.0110837 , 305.166 } ,
                                          { 33200 , -41.14 , 726.489 , 0.0109084 , 305.350 } ,
                                          { 33300 , -40.86 , 715.876 , 0.0107361 , 305.535 } ,
                                          { 33400 , -40.58 , 705.431 , 0.0105667 , 305.719 } ,
                                          { 33500 , -40.3 , 695.15 , 0.0104002 , 305.903 } ,
                                          { 33600 , -40.02 , 685.032 , 0.0102365 , 306.086 } ,
                                          { 33700 , -39.74 , 675.072 , 0.0100756 , 306.270 } ,
                                          { 33800 , -39.46 , 665.269 , 0.00991734 , 306.454 } ,
                                          { 33900 , -39.18 , 655.62 , 0.00976181 , 306.637 } ,
                                          { 34000 , -38.9 , 646.122 , 0.00960889 , 306.821 } ,
                                          { 34100 , -38.62 , 636.773 , 0.00945855 , 307.004 } ,
                                          { 34200 , -38.34 , 627.57 , 0.00931073 , 307.187 } ,
                                          { 34300 , -38.06 , 618.511 , 0.0091654 , 307.370 } ,
                                          { 34400 , -37.78 , 609.593 , 0.0090225 , 307.553 } ,
                                          { 34500 , -37.5 , 600.814 , 0.008882 , 307.736 } ,
                                          { 34600 , -37.22 , 592.172 , 0.00874385 , 307.919 } ,
                                          { 34700 , -36.94 , 583.664 , 0.008608 , 308.102 } ,
                                          { 34800 , -36.66 , 575.288 , 0.00847443 , 308.284 } ,
                                          { 34900 , -36.38 , 567.042 , 0.00834308 , 308.467 } ,
                                          { 35000 , -36.1 , 558.924 , 0.00821392 , 308.649 } ,
                                          { 35100 , -35.82 , 550.931 , 0.00808691 , 308.831 } ,
                                          { 35200 , -35.54 , 543.062 , 0.00796201 , 309.013 } ,
                                          { 35300 , -35.26 , 535.314 , 0.00783918 , 309.195 } ,
                                          { 35400 , -34.98 , 527.686 , 0.00771839 , 309.377 } ,
                                          { 35500 , -34.7 , 520.175 , 0.00759959 , 309.559 } ,
                                          { 35600 , -34.42 , 512.78 , 0.00748277 , 309.741 } ,
                                          { 35700 , -34.14 , 505.498 , 0.00736787 , 309.922 } ,
                                          { 35800 , -33.86 , 498.329 , 0.00725486 , 310.104 } ,
                                          { 35900 , -33.58 , 491.269 , 0.00714372 , 310.285 } ,
                                          { 36000 , -33.3 , 484.317 , 0.00703441 , 310.467 } ,
                                          { 36100 , -33.02 , 477.471 , 0.0069269 , 310.648 } ,
                                          { 36200 , -32.74 , 470.73 , 0.00682115 , 310.829 } ,
                                          { 36300 , -32.46 , 464.092 , 0.00671714 , 311.010 } ,
                                          { 36400 , -32.18 , 457.555 , 0.00661483 , 311.191 } ,
                                          { 36500 , -31.9 , 451.118 , 0.00651419 , 311.371 } ,
                                          { 36600 , -31.62 , 444.778 , 0.0064152 , 311.552 } ,
                                          { 36700 , -31.34 , 438.535 , 0.00631783 , 311.733 } ,
                                          { 36800 , -31.06 , 432.386 , 0.00622205 , 311.913 } ,
                                          { 36900 , -30.78 , 426.331 , 0.00612782 , 312.093 } ,
                                          { 37000 , -30.5 , 420.367 , 0.00603513 , 312.274 } ,
                                          { 37100 , -30.22 , 414.494 , 0.00594394 , 312.454 } ,
                                          { 37200 , -29.94 , 408.709 , 0.00585424 , 312.634 } ,
                                          { 37300 , -29.66 , 403.011 , 0.00576599 , 312.814 } ,
                                          { 37400 , -29.38 , 397.399 , 0.00567917 , 312.993 } ,
                                          { 37500 , -29.1 , 391.872 , 0.00559375 , 313.173 } ,
                                          { 37600 , -28.82 , 386.427 , 0.00550972 , 313.353 } ,
                                          { 37700 , -28.54 , 381.065 , 0.00542704 , 313.532 } ,
                                          { 37800 , -28.26 , 375.783 , 0.00534569 , 313.712 } ,
                                          { 37900 , -27.98 , 370.58 , 0.00526566 , 313.891 } ,
                                          { 38000 , -27.7 , 365.455 , 0.00518691 , 314.070 } ,
                                          { 38100 , -27.42 , 360.406 , 0.00510943 , 314.249 } ,
                                          { 38200 , -27.14 , 355.433 , 0.00503319 , 314.428 } ,
                                          { 38300 , -26.86 , 350.534 , 0.00495817 , 314.607 } ,
                                          { 38400 , -26.58 , 345.708 , 0.00488436 , 314.786 } ,
                                          { 38500 , -26.3 , 340.954 , 0.00481172 , 314.965 } ,
                                          { 38600 , -26.02 , 336.27 , 0.00474025 , 315.143 } ,
                                          { 38700 , -25.74 , 331.656 , 0.00466992 , 315.322 } ,
                                          { 38800 , -25.46 , 327.111 , 0.00460071 , 315.500 } ,
                                          { 38900 , -25.18 , 322.632 , 0.0045326 , 315.678 } ,
                                          { 39000 , -24.9 , 318.22 , 0.00446557 , 315.856 } ,
                                          { 39100 , -24.62 , 313.874 , 0.00439961 , 316.034 } ,
                                          { 39200 , -24.34 , 309.591 , 0.0043347 , 316.212 } ,
                                          { 39300 , -24.06 , 305.372 , 0.00427081 , 316.390 } ,
                                          { 39400 , -23.78 , 301.214 , 0.00420794 , 316.568 } ,
                                          { 39500 , -23.5 , 297.118 , 0.00414606 , 316.746 } ,
                                          { 39600 , -23.22 , 293.082 , 0.00408516 , 316.923 } ,
                                          { 39700 , -22.94 , 289.105 , 0.00402522 , 317.101 } ,
                                          { 39800 , -22.66 , 285.187 , 0.00396623 , 317.278 } ,
                                          { 39900 , -22.38 , 281.326 , 0.00390816 , 317.455 } ,
                                          { 40000 , -22.1 , 277.522 , 0.00385101 , 317.633 } ,
                                          { 40100 , -21.82 , 273.773 , 0.00379476 , 317.810 } ,
                                          { 40200 , -21.54 , 270.079 , 0.00373939 , 317.987 } ,
                                          { 40300 , -21.26 , 266.438 , 0.00368488 , 318.164 } ,
                                          { 40400 , -20.98 , 262.851 , 0.00363123 , 318.340 } ,
                                          { 40500 , -20.7 , 259.316 , 0.00357842 , 318.517 } ,
                                          { 40600 , -20.42 , 255.832 , 0.00352644 , 318.694 } ,
                                          { 40700 , -20.14 , 252.399 , 0.00347527 , 318.870 } ,
                                          { 40800 , -19.86 , 249.016 , 0.00342489 , 319.047 } ,
                                          { 40900 , -19.58 , 245.682 , 0.0033753 , 319.223 } ,
                                          { 41000 , -19.3 , 242.395 , 0.00332648 , 319.399 } ,
                                          { 41100 , -19.02 , 239.157 , 0.00327842 , 319.575 } ,
                                          { 41200 , -18.74 , 235.965 , 0.00323111 , 319.751 } ,
                                          { 41300 , -18.46 , 232.819 , 0.00318453 , 319.927 } ,
                                          { 41400 , -18.18 , 229.719 , 0.00313867 , 320.103 } ,
                                          { 41500 , -17.9 , 226.663 , 0.00309352 , 320.279 } ,
                                          { 41600 , -17.62 , 223.651 , 0.00304907 , 320.454 } ,
                                          { 41700 , -17.34 , 220.683 , 0.00300531 , 320.630 } ,
                                          { 41800 , -17.06 , 217.757 , 0.00296222 , 320.805 } ,
                                          { 41900 , -16.78 , 214.873 , 0.00291979 , 320.981 } ,
                                          { 42000 , -16.5 , 212.03 , 0.00287802 , 321.156 } ,
                                          { 42100 , -16.22 , 209.228 , 0.00283689 , 321.331 } ,
                                          { 42200 , -15.94 , 206.466 , 0.00279639 , 321.506 } ,
                                          { 42300 , -15.66 , 203.743 , 0.00275651 , 321.681 } ,
                                          { 42400 , -15.38 , 201.059 , 0.00271725 , 321.856 } ,
                                          { 42500 , -15.1 , 198.413 , 0.00267858 , 322.030 } ,
                                          { 42600 , -14.82 , 195.805 , 0.00264051 , 322.205 } ,
                                          { 42700 , -14.54 , 193.234 , 0.00260302 , 322.380 } ,
                                          { 42800 , -14.26 , 190.7 , 0.00256609 , 322.554 } ,
                                          { 42900 , -13.98 , 188.201 , 0.00252974 , 322.729 } ,
                                          { 43000 , -13.7 , 185.738 , 0.00249393 , 322.903 } ,
                                          { 43100 , -13.42 , 183.309 , 0.00245867 , 323.077 } ,
                                          { 43200 , -13.14 , 180.915 , 0.00242395 , 323.251 } ,
                                          { 43300 , -12.86 , 178.555 , 0.00238975 , 323.425 } ,
                                          { 43400 , -12.58 , 176.228 , 0.00235607 , 323.599 } ,
                                          { 43500 , -12.3 , 173.934 , 0.00232291 , 323.773 } ,
                                          { 43600 , -12.02 , 171.672 , 0.00229024 , 323.947 } ,
                                          { 43700 , -11.74 , 169.442 , 0.00225807 , 324.120 } ,
                                          { 43800 , -11.46 , 167.243 , 0.00222638 , 324.294 } ,
                                          { 43900 , -11.18 , 165.075 , 0.00219517 , 324.467 } ,
                                          { 44000 , -10.9 , 162.937 , 0.00216443 , 324.641 } ,
                                          { 44100 , -10.62 , 160.83 , 0.00213415 , 324.814 } ,
                                          { 44200 , -10.34 , 158.751 , 0.00210433 , 324.987 } ,
                                          { 44300 , -10.06 , 156.702 , 0.00207496 , 325.160 } ,
                                          { 44400 , -9.78 , 154.682 , 0.00204602 , 325.333 } ,
                                          { 44500 , -9.5 , 152.689 , 0.00201752 , 325.506 } ,
                                          { 44600 , -9.22 , 150.724 , 0.00198945 , 325.679 } ,
                                          { 44700 , -8.94 , 148.787 , 0.0019618 , 325.851 } ,
                                          { 44800 , -8.66 , 146.877 , 0.00193456 , 326.024 } ,
                                          { 44900 , -8.38 , 144.993 , 0.00190772 , 326.197 } ,
                                          { 45000 , -8.1 , 143.135 , 0.00188129 , 326.369 } ,
                                          { 45100 , -7.82 , 141.303 , 0.00185525 , 326.541 } ,
                                          { 45200 , -7.54 , 139.496 , 0.0018296 , 326.714 } ,
                                          { 45300 , -7.26 , 137.714 , 0.00180432 , 326.886 } ,
                                          { 45400 , -6.98 , 135.957 , 0.00177943 , 327.058 } ,
                                          { 45500 , -6.7 , 134.224 , 0.0017549 , 327.230 } ,
                                          { 45600 , -6.42 , 132.515 , 0.00173074 , 327.402 } ,
                                          { 45700 , -6.14 , 130.829 , 0.00170693 , 327.574 } ,
                                          { 45800 , -5.86 , 129.167 , 0.00168348 , 327.745 } ,
                                          { 45900 , -5.58 , 127.527 , 0.00166037 , 327.917 } ,
                                          { 46000 , -5.3 , 125.91 , 0.0016376 , 328.088 } ,
                                          { 46100 , -5.02 , 124.315 , 0.00161517 , 328.260 } ,
                                          { 46200 , -4.74 , 122.742 , 0.00159307 , 328.431 } ,
                                          { 46300 , -4.46 , 121.191 , 0.00157129 , 328.602 } ,
                                          { 46400 , -4.18 , 119.66 , 0.00154983 , 328.774 } ,
                                          { 46500 , -3.9 , 118.151 , 0.00152869 , 328.945 } ,
                                          { 46600 , -3.62 , 116.662 , 0.00150786 , 329.116 } ,
                                          { 46700 , -3.34 , 115.193 , 0.00148733 , 329.287 } ,
                                          { 46800 , -3.06 , 113.745 , 0.0014671 , 329.457 } ,
                                          { 46900 , -2.78 , 112.316 , 0.00144717 , 329.628 } ,
                                          { 47000 , -2.5 , 110.906 , 0.00142753 , 329.799 } ,
                                          { 47100 , -2.5 , 109.515 , 0.00140963 , 329.799 } ,
                                          { 47200 , -2.5 , 108.141 , 0.00139195 , 329.799 } ,
                                          { 47300 , -2.5 , 106.785 , 0.00137449 , 329.799 } ,
                                          { 47400 , -2.5 , 105.446 , 0.00135725 , 329.799 } ,
                                          { 47500 , -2.5 , 104.123 , 0.00134022 , 329.799 } ,
                                          { 47600 , -2.5 , 102.817 , 0.00132341 , 329.799 } ,
                                          { 47700 , -2.5 , 101.527 , 0.00130681 , 329.799 } ,
                                          { 47800 , -2.5 , 100.254 , 0.00129042 , 329.799 } ,
                                          { 47900 , -2.5 , 98.9962 , 0.00127423 , 329.799 } ,
                                          { 48000 , -2.5 , 97.7545 , 0.00125825 , 329.799 } ,
                                          { 48100 , -2.5 , 96.5283 , 0.00124247 , 329.799 } ,
                                          { 48200 , -2.5 , 95.3176 , 0.00122688 , 329.799 } ,
                                          { 48300 , -2.5 , 94.122 , 0.00121149 , 329.799 } ,
                                          { 48400 , -2.5 , 92.9414 , 0.0011963 , 329.799 } ,
                                          { 48500 , -2.5 , 91.7756 , 0.00118129 , 329.799 } ,
                                          { 48600 , -2.5 , 90.6244 , 0.00116647 , 329.799 } ,
                                          { 48700 , -2.5 , 89.4877 , 0.00115184 , 329.799 } ,
                                          { 48800 , -2.5 , 88.3652 , 0.00113739 , 329.799 } ,
                                          { 48900 , -2.5 , 87.2568 , 0.00112313 , 329.799 } ,
                                          { 49000 , -2.5 , 86.1623 , 0.00110904 , 329.799 } ,
                                          { 49100 , -2.5 , 85.0815 , 0.00109513 , 329.799 } ,
                                          { 49200 , -2.5 , 84.0143 , 0.00108139 , 329.799 } ,
                                          { 49300 , -2.5 , 82.9605 , 0.00106783 , 329.799 } ,
                                          { 49400 , -2.5 , 81.9199 , 0.00105443 , 329.799 } ,
                                          { 49500 , -2.5 , 80.8924 , 0.00104121 , 329.799 } ,
                                          { 49600 , -2.5 , 79.8777 , 0.00102815 , 329.799 } ,
                                          { 49700 , -2.5 , 78.8758 , 0.00101525 , 329.799 } ,
                                          { 49800 , -2.5 , 77.8864 , 0.00100252 , 329.799 } ,
                                          { 49900 , -2.5 , 76.9095 , 0.000989942 , 329.799 } ,
                                          { 50000 , -2.5 , 75.9448 , 0.000977525 , 329.799 } ,
                                          { 50100 , -2.5 , 74.9922 , 0.000965264 , 329.799 } ,
                                          { 50200 , -2.5 , 74.0515 , 0.000953156 , 329.799 } ,
                                          { 50300 , -2.5 , 73.1227 , 0.0009412 , 329.799 } ,
                                          { 50400 , -2.5 , 72.2055 , 0.000929395 , 329.799 } ,
                                          { 50500 , -2.5 , 71.2998 , 0.000917737 , 329.799 } ,
                                          { 50600 , -2.5 , 70.4054 , 0.000906225 , 329.799 } ,
                                          { 50700 , -2.5 , 69.5223 , 0.000894858 , 329.799 } ,
                                          { 50800 , -2.5 , 68.6503 , 0.000883634 , 329.799 } ,
                                          { 50900 , -2.5 , 67.7892 , 0.00087255 , 329.799 } ,
                                          { 51000 , -2.5 , 66.9389 , 0.000861606 , 329.799 } ,
                                          { 51100 , -2.78 , 66.0988 , 0.000851674 , 329.628 } ,
                                          { 51200 , -3.06 , 65.2684 , 0.000841846 , 329.457 } ,
                                          { 51300 , -3.34 , 64.4476 , 0.000832122 , 329.287 } ,
                                          { 51400 , -3.62 , 63.6363 , 0.0008225 , 329.116 } ,
                                          { 51500 , -3.9 , 62.8344 , 0.00081298 , 328.945 } ,
                                          { 51600 , -4.18 , 62.0418 , 0.00080356 , 328.774 } ,
                                          { 51700 , -4.46 , 61.2583 , 0.00079424 , 328.602 } ,
                                          { 51800 , -4.74 , 60.484 , 0.000785018 , 328.431 } ,
                                          { 51900 , -5.02 , 59.7186 , 0.000775894 , 328.260 } ,
                                          { 52000 , -5.3 , 58.9622 , 0.000766867 , 328.088 } ,
                                          { 52100 , -5.58 , 58.2145 , 0.000757935 , 327.917 } ,
                                          { 52200 , -5.86 , 57.4756 , 0.000749098 , 327.745 } ,
                                          { 52300 , -6.14 , 56.7453 , 0.000740355 , 327.574 } ,
                                          { 52400 , -6.42 , 56.0235 , 0.000731705 , 327.402 } ,
                                          { 52500 , -6.7 , 55.3101 , 0.000723147 , 327.230 } ,
                                          { 52600 , -6.98 , 54.6051 , 0.000714681 , 327.058 } ,
                                          { 52700 , -7.26 , 53.9084 , 0.000706305 , 326.886 } ,
                                          { 52800 , -7.54 , 53.2198 , 0.000698018 , 326.714 } ,
                                          { 52900 , -7.82 , 52.5393 , 0.00068982 , 326.541 } ,
                                          { 53000 , -8.1 , 51.8668 , 0.00068171 , 326.369 } ,
                                          { 53100 , -8.38 , 51.2022 , 0.000673687 , 326.197 } ,
                                          { 53200 , -8.66 , 50.5454 , 0.000665749 , 326.024 } ,
                                          { 53300 , -8.94 , 49.8964 , 0.000657897 , 325.851 } ,
                                          { 53400 , -9.22 , 49.2551 , 0.00065013 , 325.679 } ,
                                          { 53500 , -9.5 , 48.6213 , 0.000642446 , 325.506 } ,
                                          { 53600 , -9.78 , 47.995 , 0.000634845 , 325.333 } ,
                                          { 53700 , -10.06 , 47.3761 , 0.000627326 , 325.160 } ,
                                          { 53800 , -10.34 , 46.7646 , 0.000619888 , 324.987 } ,
                                          { 53900 , -10.62 , 46.1603 , 0.00061253 , 324.814 } ,
                                          { 54000 , -10.9 , 45.5632 , 0.000605252 , 324.641 } ,
                                          { 54100 , -11.18 , 44.9731 , 0.000598053 , 324.467 } ,
                                          { 54200 , -11.46 , 44.3902 , 0.000590932 , 324.294 } ,
                                          { 54300 , -11.74 , 43.8141 , 0.000583888 , 324.120 } ,
                                          { 54400 , -12.02 , 43.2449 , 0.000576921 , 323.947 } ,
                                          { 54500 , -12.3 , 42.6826 , 0.00057003 , 323.773 } ,
                                          { 54600 , -12.58 , 42.1269 , 0.000563214 , 323.599 } ,
                                          { 54700 , -12.86 , 41.5779 , 0.000556472 , 323.425 } ,
                                          { 54800 , -13.14 , 41.0354 , 0.000549803 , 323.251 } ,
                                          { 54900 , -13.42 , 40.4995 , 0.000543208 , 323.077 } ,
                                          { 55000 , -13.7 , 39.97 , 0.000536684 , 322.903 } ,
                                          { 55100 , -13.98 , 39.4469 , 0.000530232 , 322.729 } ,
                                          { 55200 , -14.26 , 38.93 , 0.000523851 , 322.554 } ,
                                          { 55300 , -14.54 , 38.4194 , 0.000517539 , 322.380 } ,
                                          { 55400 , -14.82 , 37.9149 , 0.000511298 , 322.205 } ,
                                          { 55500 , -15.1 , 37.4166 , 0.000505124 , 322.030 } ,
                                          { 55600 , -15.38 , 36.9242 , 0.000499019 , 321.856 } ,
                                          { 55700 , -15.66 , 36.4378 , 0.000492981 , 321.681 } ,
                                          { 55800 , -15.94 , 35.9573 , 0.000487009 , 321.506 } ,
                                          { 55900 , -16.22 , 35.4826 , 0.000481104 , 321.331 } ,
                                          { 56000 , -16.5 , 35.0137 , 0.000475263 , 321.156 } ,
                                          { 56100 , -16.78 , 34.5504 , 0.000469488 , 320.981 } ,
                                          { 56200 , -17.06 , 34.0928 , 0.000463776 , 320.805 } ,
                                          { 56300 , -17.34 , 33.6408 , 0.000458128 , 320.630 } ,
                                          { 56400 , -17.62 , 33.1943 , 0.000452542 , 320.454 } ,
                                          { 56500 , -17.9 , 32.7532 , 0.000447019 , 320.279 } ,
                                          { 56600 , -18.18 , 32.3175 , 0.000441557 , 320.103 } ,
                                          { 56700 , -18.46 , 31.8871 , 0.000436156 , 319.927 } ,
                                          { 56800 , -18.74 , 31.462 , 0.000430815 , 319.751 } ,
                                          { 56900 , -19.02 , 31.0421 , 0.000425534 , 319.575 } ,
                                          { 57000 , -19.3 , 30.6274 , 0.000420311 , 319.399 } ,
                                          { 57100 , -19.58 , 30.2178 , 0.000415147 , 319.223 } ,
                                          { 57200 , -19.86 , 29.8131 , 0.000410041 , 319.047 } ,
                                          { 57300 , -20.14 , 29.4135 , 0.000404993 , 318.870 } ,
                                          { 57400 , -20.42 , 29.0188 , 0.000400001 , 318.694 } ,
                                          { 57500 , -20.7 , 28.629 , 0.000395065 , 318.517 } ,
                                          { 57600 , -20.98 , 28.2439 , 0.000390184 , 318.340 } ,
                                          { 57700 , -21.26 , 27.8637 , 0.000385359 , 318.164 } ,
                                          { 57800 , -21.54 , 27.4881 , 0.000380588 , 317.987 } ,
                                          { 57900 , -21.82 , 27.1172 , 0.000375871 , 317.810 } ,
                                          { 58000 , -22.1 , 26.7509 , 0.000371207 , 317.633 } ,
                                          { 58100 , -22.38 , 26.3891 , 0.000366596 , 317.455 } ,
                                          { 58200 , -22.66 , 26.0318 , 0.000362037 , 317.278 } ,
                                          { 58300 , -22.94 , 25.679 , 0.000357529 , 317.101 } ,
                                          { 58400 , -23.22 , 25.3306 , 0.000353073 , 316.923 } ,
                                          { 58500 , -23.5 , 24.9865 , 0.000348668 , 316.746 } ,
                                          { 58600 , -23.78 , 24.6467 , 0.000344313 , 316.568 } ,
                                          { 58700 , -24.06 , 24.3112 , 0.000340007 , 316.390 } ,
                                          { 58800 , -24.34 , 23.9798 , 0.00033575 , 316.212 } ,
                                          { 58900 , -24.62 , 23.6526 , 0.000331542 , 316.034 } ,
                                          { 59000 , -24.9 , 23.3296 , 0.000327382 , 315.856 } ,
                                          { 59100 , -25.18 , 23.0105 , 0.00032327 , 315.678 } ,
                                          { 59200 , -25.46 , 22.6955 , 0.000319205 , 315.500 } ,
                                          { 59300 , -25.74 , 22.3844 , 0.000315186 , 315.322 } ,
                                          { 59400 , -26.02 , 22.0773 , 0.000311214 , 315.143 } ,
                                          { 59500 , -26.3 , 21.774 , 0.000307287 , 314.965 } ,
                                          { 59600 , -26.58 , 21.4746 , 0.000303405 , 314.786 } ,
                                          { 59700 , -26.86 , 21.1789 , 0.000299568 , 314.607 } ,
                                          { 59800 , -27.14 , 20.887 , 0.000295775 , 314.428 } ,
                                          { 59900 , -27.42 , 20.5988 , 0.000292027 , 314.249 } ,
                                          { 60000 , -27.7 , 20.3143 , 0.000288321 , 314.070 } ,
                                          { 60100 , -27.98 , 20.0333 , 0.000284658 , 313.891 } ,
                                          { 60200 , -28.26 , 19.7559 , 0.000281038 , 313.712 } ,
                                          { 60300 , -28.54 , 19.4821 , 0.000277459 , 313.532 } ,
                                          { 60400 , -28.82 , 19.2117 , 0.000273923 , 313.353 } ,
                                          { 60500 , -29.1 , 18.9448 , 0.000270427 , 313.173 } ,
                                          { 60600 , -29.38 , 18.6813 , 0.000266972 , 312.993 } ,
                                          { 60700 , -29.66 , 18.4212 , 0.000263557 , 312.814 } ,
                                          { 60800 , -29.94 , 18.1644 , 0.000260182 , 312.634 } ,
                                          { 60900 , -30.22 , 17.9109 , 0.000256847 , 312.454 } ,
                                          { 61000 , -30.5 , 17.6606 , 0.00025355 , 312.274 } ,
                                          { 61100 , -30.78 , 17.4136 , 0.000250292 , 312.093 } ,
                                          { 61200 , -31.06 , 17.1697 , 0.000247072 , 311.913 } ,
                                          { 61300 , -31.34 , 16.929 , 0.00024389 , 311.733 } ,
                                          { 61400 , -31.62 , 16.6913 , 0.000240746 , 311.552 } ,
                                          { 61500 , -31.9 , 16.4568 , 0.000237638 , 311.371 } ,
                                          { 61600 , -32.18 , 16.2252 , 0.000234567 , 311.191 } ,
                                          { 61700 , -32.46 , 15.9967 , 0.000231532 , 311.010 } ,
                                          { 61800 , -32.74 , 15.7711 , 0.000228533 , 310.829 } ,
                                          { 61900 , -33.02 , 15.5485 , 0.000225569 , 310.648 } ,
                                          { 62000 , -33.3 , 15.3287 , 0.00022264 , 310.467 } ,
                                          { 62100 , -33.58 , 15.1118 , 0.000219746 , 310.285 } ,
                                          { 62200 , -33.86 , 14.8977 , 0.000216886 , 310.104 } ,
                                          { 62300 , -34.14 , 14.6864 , 0.000214061 , 309.922 } ,
                                          { 62400 , -34.42 , 14.4778 , 0.000211268 , 309.741 } ,
                                          { 62500 , -34.7 , 14.272 , 0.000208509 , 309.559 } ,
                                          { 62600 , -34.98 , 14.0689 , 0.000205783 , 309.377 } ,
                                          { 62700 , -35.26 , 13.8684 , 0.00020309 , 309.195 } ,
                                          { 62800 , -35.54 , 13.6705 , 0.000200428 , 309.013 } ,
                                          { 62900 , -35.82 , 13.4753 , 0.000197798 , 308.831 } ,
                                          { 63000 , -36.1 , 13.2826 , 0.0001952 , 308.649 } ,
                                          { 63100 , -36.38 , 13.0924 , 0.000192633 , 308.467 } ,
                                          { 63200 , -36.66 , 12.9047 , 0.000190097 , 308.284 } ,
                                          { 63300 , -36.94 , 12.7196 , 0.000187591 , 308.102 } ,
                                          { 63400 , -37.22 , 12.5368 , 0.000185115 , 307.919 } ,
                                          { 63500 , -37.5 , 12.3565 , 0.000182669 , 307.736 } ,
                                          { 63600 , -37.78 , 12.1785 , 0.000180253 , 307.553 } ,
                                          { 63700 , -38.06 , 12.0029 , 0.000177865 , 307.370 } ,
                                          { 63800 , -38.34 , 11.8297 , 0.000175507 , 307.187 } ,
                                          { 63900 , -38.62 , 11.6587 , 0.000173177 , 307.004 } ,
                                          { 64000 , -38.9 , 11.49 , 0.000170875 , 306.821 } ,
                                          { 64100 , -39.18 , 11.3235 , 0.000168601 , 306.637 } ,
                                          { 64200 , -39.46 , 11.1593 , 0.000166355 , 306.454 } ,
                                          { 64300 , -39.74 , 10.9973 , 0.000164136 , 306.270 } ,
                                          { 64400 , -40.02 , 10.8374 , 0.000161944 , 306.086 } ,
                                          { 64500 , -40.3 , 10.6796 , 0.000159778 , 305.903 } ,
                                          { 64600 , -40.58 , 10.524 , 0.000157639 , 305.719 } ,
                                          { 64700 , -40.86 , 10.3704 , 0.000155527 , 305.535 } ,
                                          { 64800 , -41.14 , 10.2189 , 0.00015344 , 305.350 } ,
                                          { 64900 , -41.42 , 10.0695 , 0.000151378 , 305.166 } ,
                                          { 65000 , -41.7 , 9.92203 , 0.000149342 , 304.982 } ,
                                          { 65100 , -41.98 , 9.77656 , 0.000147331 , 304.797 } ,
                                          { 65200 , -42.26 , 9.63306 , 0.000145344 , 304.612 } ,
                                          { 65300 , -42.54 , 9.49149 , 0.000143382 , 304.428 } ,
                                          { 65400 , -42.82 , 9.35183 , 0.000141444 , 304.243 } ,
                                          { 65500 , -43.1 , 9.21406 , 0.00013953 , 304.058 } ,
                                          { 65600 , -43.38 , 9.07816 , 0.000137639 , 303.873 } ,
                                          { 65700 , -43.66 , 8.9441 , 0.000135772 , 303.688 } ,
                                          { 65800 , -43.94 , 8.81186 , 0.000133928 , 303.502 } ,
                                          { 65900 , -44.22 , 8.68141 , 0.000132107 , 303.317 } ,
                                          { 66000 , -44.5 , 8.55275 , 0.000130308 , 303.131 } ,
                                          { 66100 , -44.78 , 8.42583 , 0.000128532 , 302.946 } ,
                                          { 66200 , -45.06 , 8.30064 , 0.000126778 , 302.760 } ,
                                          { 66300 , -45.34 , 8.17717 , 0.000125046 , 302.574 } ,
                                          { 66400 , -45.62 , 8.05538 , 0.000123335 , 302.388 } ,
                                          { 66500 , -45.9 , 7.93526 , 0.000121645 , 302.202 } ,
                                          { 66600 , -46.18 , 7.81679 , 0.000119977 , 302.016 } ,
                                          { 66700 , -46.46 , 7.69994 , 0.00011833 , 301.829 } ,
                                          { 66800 , -46.74 , 7.5847 , 0.000116703 , 301.643 } ,
                                          { 66900 , -47.02 , 7.47104 , 0.000115096 , 301.456 } ,
                                          { 67000 , -47.3 , 7.35895 , 0.00011351 , 301.269 } ,
                                          { 67100 , -47.58 , 7.24841 , 0.000111944 , 301.083 } ,
                                          { 67200 , -47.86 , 7.13939 , 0.000110397 , 300.896 } ,
                                          { 67300 , -48.14 , 7.03187 , 0.00010887 , 300.709 } ,
                                          { 67400 , -48.42 , 6.92585 , 0.000107362 , 300.522 } ,
                                          { 67500 , -48.7 , 6.8213 , 0.000105873 , 300.334 } ,
                                          { 67600 , -48.98 , 6.71819 , 0.000104403 , 300.147 } ,
                                          { 67700 , -49.26 , 6.61652 , 0.000102952 , 299.959 } ,
                                          { 67800 , -49.54 , 6.51626 , 0.000101519 , 299.772 } ,
                                          { 67900 , -49.82 , 6.4174 , 0.000100104 , 299.584 } ,
                                          { 68000 , -50.1 , 6.31992 , 0.0000987069 , 299.396 } ,
                                          { 68100 , -50.38 , 6.2238 , 0.0000973279 , 299.208 } ,
                                          { 68200 , -50.66 , 6.12902 , 0.0000959663 , 299.020 } ,
                                          { 68300 , -50.94 , 6.03557 , 0.0000946222 , 298.832 } ,
                                          { 68400 , -51.22 , 5.94343 , 0.0000932952 , 298.644 } ,
                                          { 68500 , -51.5 , 5.85259 , 0.0000919852 , 298.455 } ,
                                          { 68600 , -51.78 , 5.76302 , 0.000090692 , 298.266 } ,
                                          { 68700 , -52.06 , 5.6747 , 0.0000894154 , 298.078 } ,
                                          { 68800 , -52.34 , 5.58764 , 0.0000881551 , 297.889 } ,
                                          { 68900 , -52.62 , 5.5018 , 0.0000869111 , 297.700 } ,
                                          { 69000 , -52.9 , 5.41717 , 0.000085683 , 297.511 } ,
                                          { 69100 , -53.18 , 5.33374 , 0.0000844708 , 297.322 } ,
                                          { 69200 , -53.46 , 5.25149 , 0.0000832742 , 297.133 } ,
                                          { 69300 , -53.74 , 5.17041 , 0.000082093 , 296.943 } ,
                                          { 69400 , -54.02 , 5.09047 , 0.0000809272 , 296.754 } ,
                                          { 69500 , -54.3 , 5.01168 , 0.0000797764 , 296.564 } ,
                                          { 69600 , -54.58 , 4.934 , 0.0000786406 , 296.374 } ,
                                          { 69700 , -54.86 , 4.85743 , 0.0000775195 , 296.184 } ,
                                          { 69800 , -55.14 , 4.78196 , 0.000076413 , 295.994 } ,
                                          { 69900 , -55.42 , 4.70756 , 0.0000753209 , 295.804 } ,
                                          { 70000 , -55.7 , 4.63422 , 0.000074243 , 295.614 } ,
                                          { 70100 , -55.98 , 4.56194 , 0.0000731792 , 295.423 } ,
                                          { 70200 , -56.26 , 4.49069 , 0.0000721293 , 295.233 } ,
                                          { 70300 , -56.54 , 4.42046 , 0.0000710931 , 295.042 } ,
                                          { 70400 , -56.82 , 4.35125 , 0.0000700705 , 294.852 } ,
                                          { 70500 , -57.1 , 4.28303 , 0.0000690613 , 294.661 } ,
                                          { 70600 , -57.38 , 4.21579 , 0.0000680654 , 294.470 } ,
                                          { 70700 , -57.66 , 4.14952 , 0.0000670825 , 294.279 } ,
                                          { 70800 , -57.94 , 4.08422 , 0.0000661126 , 294.087 } ,
                                          { 70900 , -58.22 , 4.01985 , 0.0000651555 , 293.896 } ,
                                          { 71000 , -58.5 , 3.95642 , 0.000064211 , 293.704 } ,
                                          { 71100 , -58.7 , 3.89392 , 0.0000632556 , 293.568 } ,
                                          { 71200 , -58.9 , 3.83235 , 0.0000623136 , 293.431 } ,
                                          { 71300 , -59.1 , 3.7717 , 0.0000613847 , 293.294 } ,
                                          { 71400 , -59.3 , 3.71195 , 0.0000604688 , 293.157 } ,
                                          { 71500 , -59.5 , 3.6531 , 0.0000595657 , 293.019 } ,
                                          { 71600 , -59.7 , 3.59512 , 0.0000586753 , 292.882 } ,
                                          { 71700 , -59.9 , 3.53801 , 0.0000577974 , 292.745 } ,
                                          { 71800 , -60.1 , 3.48176 , 0.0000569318 , 292.608 } ,
                                          { 71900 , -60.3 , 3.42634 , 0.0000560784 , 292.470 } ,
                                          { 72000 , -60.5 , 3.37176 , 0.000055237 , 292.333 } ,
                                          { 72100 , -60.7 , 3.318 , 0.0000544074 , 292.195 } ,
                                          { 72200 , -60.9 , 3.26505 , 0.0000535895 , 292.058 } ,
                                          { 72300 , -61.1 , 3.21289 , 0.0000527832 , 291.920 } ,
                                          { 72400 , -61.3 , 3.16152 , 0.0000519883 , 291.783 } ,
                                          { 72500 , -61.5 , 3.11092 , 0.0000512046 , 291.645 } ,
                                          { 72600 , -61.7 , 3.06109 , 0.000050432 , 291.507 } ,
                                          { 72700 , -61.9 , 3.012 , 0.0000496703 , 291.369 } ,
                                          { 72800 , -62.1 , 2.96366 , 0.0000489195 , 291.231 } ,
                                          { 72900 , -62.3 , 2.91605 , 0.0000481793 , 291.093 } ,
                                          { 73000 , -62.5 , 2.86917 , 0.0000474496 , 290.955 } ,
                                          { 73100 , -62.7 , 2.82299 , 0.0000467302 , 290.817 } ,
                                          { 73200 , -62.9 , 2.77751 , 0.0000460212 , 290.679 } ,
                                          { 73300 , -63.1 , 2.73272 , 0.0000453222 , 290.540 } ,
                                          { 73400 , -63.3 , 2.68861 , 0.0000446331 , 290.402 } ,
                                          { 73500 , -63.5 , 2.64518 , 0.000043954 , 290.264 } ,
                                          { 73600 , -63.7 , 2.6024 , 0.0000432845 , 290.125 } ,
                                          { 73700 , -63.9 , 2.56028 , 0.0000426246 , 289.987 } ,
                                          { 73800 , -64.1 , 2.5188 , 0.0000419741 , 289.848 } ,
                                          { 73900 , -64.3 , 2.47795 , 0.0000413329 , 289.709 } ,
                                          { 74000 , -64.5 , 2.43773 , 0.000040701 , 289.570 } ,
                                          { 74100 , -64.7 , 2.39812 , 0.0000400781 , 289.432 } ,
                                          { 74200 , -64.9 , 2.35912 , 0.0000394642 , 289.293 } ,
                                          { 74300 , -65.1 , 2.32071 , 0.000038859 , 289.154 } ,
                                          { 74400 , -65.3 , 2.2829 , 0.0000382626 , 289.015 } ,
                                          { 74500 , -65.5 , 2.24567 , 0.0000376748 , 288.876 } ,
                                          { 74600 , -65.7 , 2.209 , 0.0000370955 , 288.737 } ,
                                          { 74700 , -65.9 , 2.17291 , 0.0000365245 , 288.597 } ,
                                          { 74800 , -66.1 , 2.13737 , 0.0000359618 , 288.458 } ,
                                          { 74900 , -66.3 , 2.10237 , 0.0000354072 , 288.319 } ,
                                          { 75000 , -66.5 , 2.06792 , 0.0000348607 , 288.179 } ,
                                          { 75100 , -66.7 , 2.034 , 0.0000343221 , 288.040 } ,
                                          { 75200 , -66.9 , 2.0006 , 0.0000337912 , 287.900 } ,
                                          { 75300 , -67.1 , 1.96772 , 0.0000332681 , 287.761 } ,
                                          { 75400 , -67.3 , 1.93535 , 0.0000327526 , 287.621 } ,
                                          { 75500 , -67.5 , 1.90348 , 0.0000322446 , 287.481 } ,
                                          { 75600 , -67.7 , 1.8721 , 0.000031744 , 287.341 } ,
                                          { 75700 , -67.9 , 1.84121 , 0.0000312507 , 287.201 } ,
                                          { 75800 , -68.1 , 1.81081 , 0.0000307645 , 287.061 } ,
                                          { 75900 , -68.3 , 1.78087 , 0.0000302855 , 286.921 } ,
                                          { 76000 , -68.5 , 1.7514 , 0.0000298135 , 286.781 } ,
                                          { 76100 , -68.7 , 1.7224 , 0.0000293484 , 286.641 } ,
                                          { 76200 , -68.9 , 1.69384 , 0.0000288901 , 286.501 } ,
                                          { 76300 , -69.1 , 1.66573 , 0.0000284385 , 286.361 } ,
                                          { 76400 , -69.3 , 1.63806 , 0.0000279935 , 286.220 } ,
                                          { 76500 , -69.5 , 1.61082 , 0.0000275551 , 286.080 } ,
                                          { 76600 , -69.7 , 1.58401 , 0.0000271231 , 285.939 } ,
                                          { 76700 , -69.9 , 1.55762 , 0.0000266975 , 285.799 } ,
                                          { 76800 , -70.1 , 1.53165 , 0.0000262781 , 285.658 } ,
                                          { 76900 , -70.3 , 1.50608 , 0.000025865 , 285.517 } ,
                                          { 77000 , -70.5 , 1.48092 , 0.0000254579 , 285.377 } ,
                                          { 77100 , -70.7 , 1.45615 , 0.0000250568 , 285.236 } ,
                                          { 77200 , -70.9 , 1.43177 , 0.0000246617 , 285.095 } ,
                                          { 77300 , -71.1 , 1.40778 , 0.0000242724 , 284.954 } ,
                                          { 77400 , -71.3 , 1.38416 , 0.0000238889 , 284.813 } ,
                                          { 77500 , -71.5 , 1.36092 , 0.0000235111 , 284.672 } ,
                                          { 77600 , -71.7 , 1.33805 , 0.0000231389 , 284.530 } ,
                                          { 77700 , -71.9 , 1.31554 , 0.0000227722 , 284.389 } ,
                                          { 77800 , -72.1 , 1.29338 , 0.000022411 , 284.248 } ,
                                          { 77900 , -72.3 , 1.27158 , 0.0000220551 , 284.106 } ,
                                          { 78000 , -72.5 , 1.25012 , 0.0000217046 , 283.965 } ,
                                          { 78100 , -72.7 , 1.22901 , 0.0000213593 , 283.823 } ,
                                          { 78200 , -72.9 , 1.20823 , 0.0000210191 , 283.682 } ,
                                          { 78300 , -73.1 , 1.18778 , 0.0000206841 , 283.540 } ,
                                          { 78400 , -73.3 , 1.16766 , 0.000020354 , 283.398 } ,
                                          { 78500 , -73.5 , 1.14786 , 0.0000200289 , 283.256 } ,
                                          { 78600 , -73.7 , 1.12837 , 0.0000197087 , 283.114 } ,
                                          { 78700 , -73.9 , 1.1092 , 0.0000193932 , 282.972 } ,
                                          { 78800 , -74.1 , 1.09034 , 0.0000190826 , 282.830 } ,
                                          { 78900 , -74.3 , 1.07177 , 0.0000187765 , 282.688 } ,
                                          { 79000 , -74.5 , 1.05351 , 0.0000184751 , 282.546 } ,
                                          { 79100 , -74.7 , 1.03554 , 0.0000181783 , 282.404 } ,
                                          { 79200 , -74.9 , 1.01785 , 0.0000178859 , 282.262 } ,
                                          { 79300 , -75.1 , 1.00045 , 0.0000175979 , 282.119 } ,
                                          { 79400 , -75.3 , 0.983336 , 0.0000173143 , 281.977 } ,
                                          { 79500 , -75.5 , 0.966494 , 0.0000170349 , 281.834 } ,
                                          { 79600 , -75.7 , 0.949924 , 0.0000167598 , 281.691 } ,
                                          { 79700 , -75.9 , 0.933621 , 0.0000164889 , 281.549 } ,
                                          { 79800 , -76.1 , 0.917582 , 0.0000162221 , 281.406 } ,
                                          { 79900 , -76.3 , 0.901803 , 0.0000159593 , 281.263 } ,
                                          { 80000 , -76.5 , 0.88628 , 0.0000157005 , 281.120 } ,
                                          { 80100 , -76.7 , 0.871008 , 0.0000154457 , 280.977 } ,
                                          { 80200 , -76.9 , 0.855984 , 0.0000151948 , 280.834 } ,
                                          { 80300 , -77.1 , 0.841205 , 0.0000149476 , 280.691 } ,
                                          { 80400 , -77.3 , 0.826666 , 0.0000147043 , 280.548 } ,
                                          { 80500 , -77.5 , 0.812363 , 0.0000144647 , 280.405 } ,
                                          { 80600 , -77.7 , 0.798294 , 0.0000142287 , 280.261 } ,
                                          { 80700 , -77.9 , 0.784455 , 0.0000139964 , 280.118 } ,
                                          { 80800 , -78.1 , 0.770842 , 0.0000137676 , 279.974 } ,
                                          { 80900 , -78.3 , 0.757451 , 0.0000135423 , 279.831 } ,
                                          { 81000 , -78.5 , 0.74428 , 0.0000133205 , 279.687 } ,
                                          { 81100 , -78.7 , 0.731324 , 0.0000131021 , 279.543 } ,
                                          { 81200 , -78.9 , 0.718581 , 0.000012887 , 279.399 } ,
                                          { 81300 , -79.1 , 0.706047 , 0.0000126753 , 279.256 } ,
                                          { 81400 , -79.3 , 0.69372 , 0.0000124668 , 279.112 } ,
                                          { 81500 , -79.5 , 0.681595 , 0.0000122616 , 278.968 } ,
                                          { 81600 , -79.7 , 0.66967 , 0.0000120595 , 278.824 } ,
                                          { 81700 , -79.9 , 0.657941 , 0.0000118606 , 278.679 } ,
                                          { 81800 , -80.1 , 0.646406 , 0.0000116647 , 278.535 } ,
                                          { 81900 , -80.3 , 0.635062 , 0.0000114719 , 278.391 } ,
                                          { 82000 , -80.5 , 0.623905 , 0.000011282 , 278.246 } ,
                                          { 82100 , -80.7 , 0.612933 , 0.0000110952 , 278.102 } ,
                                          { 82200 , -80.9 , 0.602143 , 0.0000109112 , 277.957 } ,
                                          { 82300 , -81.1 , 0.591532 , 0.0000107301 , 277.813 } ,
                                          { 82400 , -81.3 , 0.581097 , 0.0000105518 , 277.668 } ,
                                          { 82500 , -81.5 , 0.570835 , 0.0000103762 , 277.523 } ,
                                          { 82600 , -81.7 , 0.560745 , 0.0000102035 , 277.378 } ,
                                          { 82700 , -81.9 , 0.550822 , 0.0000100334 , 277.234 } ,
                                          { 82800 , -82.1 , 0.541065 , 0.00000986599 , 277.089 } ,
                                          { 82900 , -82.3 , 0.531471 , 0.0000097012 , 276.943 } ,
                                          { 83000 , -82.5 , 0.522037 , 0.00000953899 , 276.798 } ,
                                          { 83100 , -82.7 , 0.512761 , 0.00000937933 , 276.653 } ,
                                          { 83200 , -82.9 , 0.50364 , 0.00000922218 , 276.508 } ,
                                          { 83300 , -83.1 , 0.494672 , 0.00000906751 , 276.362 } ,
                                          { 83400 , -83.3 , 0.485855 , 0.00000891526 , 276.217 } ,
                                          { 83500 , -83.5 , 0.477186 , 0.00000876542 , 276.071 } ,
                                          { 83600 , -83.7 , 0.468662 , 0.00000861794 , 275.926 } ,
                                          { 83700 , -83.9 , 0.460282 , 0.00000847279 , 275.780 } ,
                                          { 83800 , -84.1 , 0.452044 , 0.00000832994 , 275.634 } ,
                                          { 83900 , -84.3 , 0.443944 , 0.00000818935 , 275.489 } ,
                                          { 84000 , -84.5 , 0.435981 , 0.00000805098 , 275.343 } ,
                                          { 84100 , -84.7 , 0.428153 , 0.00000791481 , 275.197 } ,
                                          { 84200 , -84.9 , 0.420457 , 0.0000077808 , 275.051 } ,
                                          { 84300 , -85.1 , 0.412891 , 0.00000764892 , 274.904 } ,
                                          { 84400 , -85.3 , 0.405454 , 0.00000751914 , 274.758 } ,
                                          { 84500 , -85.5 , 0.398143 , 0.00000739143 , 274.612 } ,
                                          { 84600 , -85.7 , 0.390956 , 0.00000726576 , 274.466 } ,
                                          { 84700 , -85.9 , 0.383892 , 0.00000714209 , 274.319 } ,
                                          { 84800 , -86.1 , 0.376948 , 0.00000702039 , 274.173 } ,
                                          { 84900 , -86.204 , 0.370123 , 0.00000689712 , 274.096 } ,
                                          { 85000 , -86.204 , 0.36342 , 0.00000677222 , 274.096 } ,
                                          { 85100 , -86.204 , 0.356839 , 0.00000664959 , 274.096 } ,
                                          { 85200 , -86.204 , 0.350378 , 0.00000652917 , 274.096 } ,
                                          { 85300 , -86.204 , 0.344033 , 0.00000641094 , 274.096 } ,
                                          { 85400 , -86.204 , 0.337803 , 0.00000629485 , 274.096 } ,
                                          { 85500 , -86.204 , 0.331686 , 0.00000618086 , 274.096 } ,
                                          { 85600 , -86.204 , 0.32568 , 0.00000606893 , 274.096 } ,
                                          { 85700 , -86.204 , 0.319782 , 0.00000595904 , 274.096 } ,
                                          { 85800 , -86.204 , 0.313991 , 0.00000585113 , 274.096 } ,
                                          { 85900 , -86.204 , 0.308305 , 0.00000574517 , 274.096 } ,
                                          { 86000 , -86.204 , 0.302723 , 0.00000564114 , 274.096 } };

    public double getPressure(double Altitude)
    {
      int i;
      double delta;
      double Pressure = 0.0;

      for (i = 0; i <= 860; i++)
      {
        delta = Altitude - table[i, 0];
        if (delta == 0.0)
        {
          Pressure = table[i, 2];
        }
        else if (delta < 0.0)
        {
          Pressure = table[i - 1, 2] + (table[i, 2] - table[i - 1, 2]) * (Altitude - table[i - 1, 0]) / (table[i, 0] - table[i - 1, 0]);
        }
      }

      return Pressure;
    }
    public double getTemperature(double Altitude)
    {
      int i;
      double delta;
      double Temperature = 0.0;

      for (i = 0; i <= 860; i++)
      {
        delta = Altitude - table[i, 0];
        if (delta == 0.0)
        {
          Temperature = table[i, 1];
        }
        else if (delta < 0.0)
        {
          Temperature = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * (Altitude - table[i - 1, 0]) / (table[i, 0] - table[i - 1, 0]);
        }
      }

      return Temperature + 273.15;
    }

    public double getSoundofSpeed(double Altitude)
    {
      int i;
      double delta;
      double Cs = 0.0;

      for (i = 0; i <= 860; i++)
      {
        delta = Altitude - table[i, 0];
        if (delta == 0.0)
        {
          Cs = table[i, 4];
        }
        else if (delta < 0.0)
        {
          Cs = table[i - 1, 4] + (table[i, 4] - table[i - 1, 4]) * (Altitude - table[i - 1, 0]) / (table[i, 0] - table[i - 1, 0]);
        }
      }

      return Cs;
    }

  }

  public static class utility
  {
    public static double sum_range(double[] Array, int k1, int k2)
    {
      int n;
      double sum = 0.0;

      for (n = k1; n <= k2; n++)
      {
        sum = sum + Array[n];
      }

      return sum;
    }

    public static double average_range(double[] Array, int k1, int k2)
    {
      return sum_range(Array, k1, k2) / (k2 - k1);
    }

    public static double d2A(double d)
    {
      return 0.25 * d * d * Math.PI;
    }
    public static double deg2rad(double deg)
    {
      return deg * Math.PI / 180.0;
    }

    public static double max(double[] Array, int k1, int k2)
    {
      int i;
      double max;
      max = Array[k1];
      for (i = k1 + 1; i < k2; i++)
      {
        if (Array[i] > max) max = Array[i];
      }
      return max;
    }
    public static double min(double[] Array, int k1, int k2)
    {
      int i;
      double min;
      min = Array[k1];
      for (i = k1 + 1; i < k2; i++)
      {
        if (Array[i] < min) min = Array[i];
      }
      return min;
    }

    public static void matcopy(double[] array_ori, double[] array_to)
    {
      int i;
      for (i = 0; i <= 1; i++)
      {
        array_to[i] = array_ori[i];
      }

      return;
    }
    public static double AxisComposite(double[] array)
    {
      int i;
      double ans = 0.0;

      for (i = 0; i <= 1; i++)
      {
        ans = ans + array[i] * array[i];
      }
      ans = Math.Sqrt(ans);
      return ans;
    }
  }
}
