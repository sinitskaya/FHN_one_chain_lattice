using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
//using System.Windows.Media;

namespace FHN_Chain
{
    //delegate double Del(int flag_first_elem_exist, int rank, int eqNumber, double t, List<double> parameter, List<double> value, double resh_start_value);
    class Program
    {
        static void Main(string[] args)
        {
            int slychai = 3;
            int flag_first_elem_exist = 0;
            String inFileName, outFileNameDat, outFileNameOsc;
            inFileName = "inputFHN.txt";
            if (args.Length == 1)
                inFileName = args[0];

            List<double> value = new List<double>(), parameter = new List<double>(), data = new List<double>();

            bool fullLog;
            int rank;
            double t, dt, tEst, tMin, tInterval, tMax, tFin;
            string boundaryConditions;//граничные условия

            StreamReader sr = new StreamReader(inFileName);
            String str = sr.ReadToEnd();//считывает вплоть до конца файла в одной операции.
            String[] strs;
            String[] tmpStrs;

            strs = str.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);//удалить пустые записи

            outFileNameDat = strs[0];
            outFileNameOsc = strs[1];

            tmpStrs = strs[2].Split(new Char[] { ' ' });
            fullLog = Convert.ToBoolean(tmpStrs[2]);
            tmpStrs = strs[3].Split(new Char[] { ' ' });
            rank = Convert.ToInt32(tmpStrs[2]);

            tmpStrs = strs[4].Split(new Char[] { ' ' });
            t = Convert.ToDouble(tmpStrs[2]);
            tmpStrs = strs[5].Split(new Char[] { ' ' });
            dt = Convert.ToDouble(tmpStrs[2]);
            tmpStrs = strs[6].Split(new Char[] { ' ' });
            tEst = Convert.ToDouble(tmpStrs[2]);//время установления
            tmpStrs = strs[7].Split(new Char[] { ' ' });
            tMin = Convert.ToDouble(tmpStrs[2]);
            tmpStrs = strs[8].Split(new Char[] { ' ' });
            tInterval = Convert.ToDouble(tmpStrs[2]);
            tmpStrs = strs[9].Split(new Char[] { ' ' });
            tMax = Convert.ToDouble(tmpStrs[2]);
            tmpStrs = strs[10].Split(new Char[] { ' ' });
            tFin = Convert.ToDouble(tmpStrs[2]);
            //////////////////////////////////////////////
            List<double> value3 = new List<double>(); double parameter3; // хаос
            parameter3 = Convert.ToDouble(strs[18]); // хаос
            double NU = 0; // НУ
            for (int r = 0; r < 2; r++)
            {
                if (slychai != 3)
                    value3.Add(NU);
            }
            if (slychai == 3)
            {//1 0 0
                value3.Add(0);//1
                value3.Add(0);
                value3.Add(0);//0
            }
            for (int i = 0; i < 2; i++)
            {
                parameter.Add(Convert.ToDouble(strs[12 + i]));
            }
            for (int i = 0; i < (int)(rank / 2)-2-1; i++) // параметры у 2х цепочек одинаковые
            {
                parameter.Add(Convert.ToDouble(strs[12+2]));
            }
            for (int i = 0; i < 3; i++)
            {
                parameter.Add(Convert.ToDouble(strs[15 + i]));
            }
            boundaryConditions = Convert.ToString(strs[19]);
            Console.WriteLine("Boundary: {0}", boundaryConditions);

            /////////////////////////////////////////////////
            int k1 = 0;//кол НУ
            for (k1 = 0; k1 < rank; k1++)
                value.Add(0);

            int size_line, size_column;
            tmpStrs = strs[20].Split(new Char[] { ' ','x' });
            size_line = Convert.ToInt16(tmpStrs[2]);
            size_column = Convert.ToInt16(tmpStrs[3]);

            double param_resh/*.Add*/= 1.003;
            

            double [,] valueX = new double[size_line, size_column];
            for (int k3 = 0; k3 < size_line; k3++)
                for (int kk = 0; kk < size_column; kk++)
                {
                    valueX/*.Add*/[k3, kk] = 0;
                }

            double[,] valueY = new double[size_line, size_column];
            for (int k4 = 0; k4 < size_line; k4++)
                for (int kk = 0; kk < size_column; kk++)
                {
                    valueY/*.Add*/[k4, kk] = 0;
                }
            ///////////////////////
            List<double> x = new List<double>();
            //value[0] = x[0];
            /////////////////
            double t_ = t;
            int ijk = 0; double d = 0;///12412
            //double omega1 = 0, omega2 = 0;
            //начальные значения
            File.WriteAllText(outFileNameOsc, string.Empty);//очистка файла
            File.WriteAllText("output_osc_resh.txt", string.Empty);//очистка файла
            File.WriteAllText("y.txt", string.Empty);//очистка файла

            /*if (t_ == 0)
            {
                StreamWriter swOsc = new StreamWriter(outFileNameOsc, true);
                StreamWriter swOsc_resh = new StreamWriter("output_osc_resh.txt", true);
                swOsc.Write("{0:F1}", t_); swOsc_resh.Write("{0:F1}", t_);
                //swOsc.Write(t_);//:F2
                for (int l = 0; l < rank; l += 2)
                {
                    swOsc.Write("  ");
                    swOsc.Write("{0:F7}", value[l]);
                }

                for (int l = 0; l < size_line; l++)
                    for (int l1 = 0; l1 < size_column; l1++)
                    {
                        swOsc_resh.Write(" ");
                        swOsc_resh.Write("{0:F2}", valueX[l, l1]);
                    }

                swOsc.WriteLine();
                swOsc.Close();
                swOsc_resh.WriteLine();
                swOsc_resh.Close();
            } 
             * */
            while (t_ < tFin)//счет
            {
                if (t_ >= tMin && t_ <= tMax)//вывод
                {
                    if (fullLog && Convert.ToInt64(t_ / dt) % Convert.ToInt32(tInterval / dt) == 0)
                    {
                        StreamWriter swOsc = new StreamWriter(outFileNameOsc, true);
                        swOsc.Write(t_);
                        for (int l = 0; l < rank; l += 2)
                            swOsc.Write("\t{0}\t{1}", value[l], value[l + 1]);
                        swOsc.WriteLine();
                        swOsc.Close();
                        dataoutput(rank, ref t_, ref dt, tEst, tMin, tMax, tFin, boundaryConditions, value, data, parameter, outFileNameDat);
                    }
                    else if (Convert.ToInt64(t_ / dt) % Convert.ToInt32(tInterval / dt) == 0)
                    {                        
                        StreamWriter swOsc = new StreamWriter(outFileNameOsc, true);
                        StreamWriter swOsc_resh = new StreamWriter("output_osc_resh.txt", true);
                        StreamWriter swOsc_y = new StreamWriter("y.txt", true);

                        swOsc.Write("{0:F1}", t_); swOsc_resh.Write("{0:F1}", t_); swOsc_y.Write("{0:F1}", t_); 
                        for (int l = 0; l < rank; l += 2)
                        {
                            swOsc.Write("  ");
                            swOsc.Write("{0:F7}", value[l]);
                            swOsc_y.Write("  ");
                            swOsc_y.Write("{0:F7}", value[l+1]);
                        }
                        //resh
                        for (int l = 0; l < size_line; l ++)
                            for(int l1 = 0; l1 < size_column; l1++)
                            {
                               swOsc_resh.Write(" ");
                               swOsc_resh.Write("{0:F2}", valueX[l,l1]);
                            }
                        swOsc.WriteLine();
                        swOsc.Close();
                        swOsc_resh.WriteLine();
                        swOsc_resh.Close();
                        swOsc_y.WriteLine();
                        swOsc_y.Close();
                        //if (t_ > tEst && t + dt> tMax)
                        //dataoutput(rank, ref t_, ref dt, tEst, tMin, tMax, tFin, boundaryConditions, value, data, parameter, outFileNameDat);
                    }
                }
                //////////////////////////////////////
                double xx = 0;///12412 ijk=0
                if (flag_first_elem_exist == 1)
                {
                    double alpha = 0.5;
                    xx = x[0];
                    if (t_ > d && t_ < d + 2*tInterval)
                    {
                        if (ijk >= x.Count-1)
                            xx = x[x.Count - 1];
                        else
                            xx = (x[ijk] + x[ijk + 1]) / 2;
                        if (t_ + dt > d + 2*tInterval)
                        {
                            d = d + 2*tInterval;
                            ijk++;
                        }
                    }
                    if (t_ == d + 2*tInterval)//маловероятно
                    {
                        xx = x[(int) (ijk*alpha)];
                        if (t_ + dt > d + 2*tInterval)
                        {
                            d = d + 2*tInterval;
                            ijk++;
                        }
                    }
                }
                else { xx = 0; }
                /////////////////////////////////////
                //calculate(rank, ref t_, ref dt, tEst, tMin, tMax, tFin, boundaryConditions, value, data, parameter, outFileNameDat);
                rk4(slychai, flag_first_elem_exist, rank, ref t_, ref dt, value, parameter, param_resh, valueX, valueY, size_column, size_line, xx, parameter3, value3);////////////////////
                t_ += dt;
                Console.SetCursorPosition(2,1);
                Console.Write("{0}",(int)(((double)t_ / tMax)*100));
            }
             System.Media.SystemSounds.Exclamation.Play();           
        }

        static void rk4(int slychai, int flag_first_elem_exist, int rank, ref double t, ref double dt, List<double> value, List<double> parameter, double param_a, double[,] valueX, double[,] valueY, int size_column, int size_line, double x_alph, double parameter3, List<double> value3)
        {
            int i;
            double[,] k = new double[4, rank + 2*size_line*size_column+3];
            List<double> value_ = new List<double>();
            double[,] valueX_ = new double[size_line, size_column];
            double[,] valueY_ = new double[size_line, size_column];
            List<double> value3_ = new List<double>();
            double elem = x_alph;
            for (i = 0; i < rank; i++)
                k[0, i] = rightPartFree(slychai, flag_first_elem_exist, rank, i, t, parameter, value, valueX[0, 0], value3[0]) * dt;////size_column, не используется в rightPartFree убрать Del//valueX[0,0]
            for (int x = 0; x < size_line; x++)                                                         //value2[0]
                for(int y = 0; y < size_column; y++)                                                    //elem
                {
                    k[0, i] = rightPartFree_lattice(x, y, i, t, parameter, param_a, valueX, valueY, size_column, size_line, value[rank-2]) * dt;
                    i++;                                                                                                 //лев верхний с пер // нижний правый со второй цепочкой
                    k[0, i] = rightPartFree_lattice(x, y, i, t, parameter, param_a, valueX, valueY, size_column, size_line, value[rank - 2]) * dt;
                    i++;
                }
            ///////
            if (slychai == 2) //  ФХН
            {
                for (int r = 0; r < 2; r++) //                           
                {
                    k[0, i] = chaos_elem(r, parameter, parameter3, value3) * dt;////size_column, не используется в rightPartFree убрать Del//valueX[0,0]
                    i++;
                }
            }
            if (slychai == 3)// хаос Лоренц
            {
                for (int r = 0; r < 3; r++) //                                
                {
                    k[0, i] = attraction_Resslera_elem(r, value3) * dt;////size_column, не используется в rightPartFree убрать Del//valueX[0,0]
                    i++;
                }
            }///////хаос // t
            ////////////////////////////////////////////////////////////////////////////////////
            for (i = 0; i < rank; i++)
                value_.Add(value[i] + k[0, i] * 0.5);
            for (int x = 0; x < size_line; x++)
                for (int y = 0; y < size_column; y++)
                {
                    valueX_[x, y] = valueX[x, y] + k[0, i]*0.5;
                    i++;
                    valueY_[x, y] = valueY[x, y] + k[0, i] * 0.5;
                    i++;
                }
            double hrank = 0; //хаос
            if ((slychai == 2) || (slychai == 3))
            {
                if (slychai == 2) hrank = 2; if (slychai == 3) hrank = 3;
                for (int r = 0; r < hrank; r++)
                {
                    value3_.Add(value3[r] + k[0, i] * 0.5);
                    i++;
                }
            }
            if (slychai == 1) // чтобы не выдавал ошибок
            { value3_.Add(0); }
            ///////////////////////////////////////////////////////////////////////////////////////
            for (i = 0; i < rank; i++)
                k[1, i] = rightPartFree(slychai, flag_first_elem_exist, rank, i, t + dt * 0.5, parameter, value_, valueX_[0, 0], value3_[0]) * dt;//value2_[0]
            for (int x = 0; x < size_line; x++)                                                                     //elem
                for (int y = 0; y < size_column; y++)
                {
                    k[1, i] = rightPartFree_lattice(x, y, i, t + dt * 0.5, parameter, param_a, valueX_, valueY_, size_column, size_line, value_[rank - 2]) *dt;
                    i++;
                    k[1, i] = rightPartFree_lattice(x, y, i, t + dt * 0.5, parameter, param_a, valueX_, valueY_, size_column, size_line, value_[rank - 2]) * dt;
                    i++;
                }
            /////// хаос
            if (slychai == 2) //  ФХН
            {
                for (int r = 0; r < 2; r++)
                {
                    k[1, i] = chaos_elem(r, parameter, parameter3, value3_) * dt;
                    i++;
                }
            }
            if (slychai == 3)// хаос Лоренц
            {
                for (int r = 0; r < 3; r++)
                {
                    k[1, i] = attraction_Resslera_elem(r, value3_) * dt;
                    i++;
                }
            }///////хаос// t + dt * 0.5
            /////////////////////////////////////////////////////////////////////////////////
            for (i = 0; i < rank; i++)
                value_[i] = value[i] + k[1, i] * 0.5;
            for (int x = 0; x < size_line; x++)
                for (int y = 0; y < size_column; y++)
                {
                    valueX_[x, y] = valueX[x, y] + k[1, i] * 0.5;
                    i++;//
                    valueY_[x, y] = valueY[x, y] + k[1, i] * 0.5;
                    i++;
                }
            if ((slychai == 2) || (slychai == 3)) // хаос
            {
                if (slychai == 2) hrank = 2; if (slychai == 3) hrank = 3;
                for (int r = 0; r < hrank; r++) //хаос
                {
                    value3_[r] = value3[r] + k[1, i] * 0.5;
                    i++;
                }
            } 
            ////////////////////////////////////////////////////////////////////////////////
            for (i = 0; i < rank; i++)
                k[2, i] = rightPartFree(slychai, flag_first_elem_exist, rank, i, t + dt * 0.5, parameter, value_, valueX_[0, 0], value3_[0]) * dt;
            for (int x = 0; x < size_line; x++)                                                                     //elem
                for (int y = 0; y < size_column; y++)
                {
                    k[2, i] = rightPartFree_lattice(x, y, i, t + dt * 0.5, parameter, param_a, valueX_, valueY_, size_column, size_line, value_[rank - 2]) * dt;
                    i++;
                    k[2, i] = rightPartFree_lattice(x, y, i, t + dt * 0.5, parameter, param_a, valueX_, valueY_, size_column, size_line, value_[rank - 2]) * dt;
                    i++;
                }
            /////// 
            if (slychai == 2) //  ФХН
            {
                for (int r = 0; r < 2; r++)
                {
                    k[2, i] = chaos_elem(r, parameter, parameter3, value3_) * dt;////size_column, не используется в rightPartFree убрать Del//valueX[0,0]
                    i++;
                }
            }
            if (slychai == 3)// хаос Лоренц
            {
                for (int r = 0; r < 3; r++) //                                
                {
                    k[2, i] = attraction_Resslera_elem(r, value3_) * dt;////size_column, не используется в rightPartFree убрать Del//valueX[0,0]
                    i++;
                }
            }///////хаос//// t + dt * 0.5
            //////////////////////////////////////////////////////////////////////////////////
            for (i = 0; i < rank; i++)
                value_[i] = value[i] + k[2, i];
            for (int x = 0; x < size_line; x++)
                for (int y = 0; y < size_column; y++)
                {
                    valueX_[x, y] = valueX[x, y] + k[2, i];
                    i++;//
                    valueY_[x, y] = valueY[x, y] + k[2, i];
                    i++;
                }
            if ((slychai == 2) || (slychai == 3)) // хаос
            {
                if (slychai == 2) hrank = 2; if (slychai == 3) hrank = 3;
                for (int r = 0; r < hrank; r++) //хаос
                {
                    value3_[r] = value3[r] + k[2, i];
                    i++;
                }
            }
            ///////////////////////////////////////
            for (i = 0; i < rank; i++)
                k[3, i] = rightPartFree(slychai, flag_first_elem_exist, rank, i, t + dt, parameter, value_, valueX_[0, 0], value3_[0]) * dt;//size_column, value[rank - 2] не используется в rightPartFree
            for (int x = 0; x < size_line; x++)
                for (int y = 0; y < size_column; y++)
                {
                    k[3, i] = rightPartFree_lattice(x, y, i, t + dt, parameter, param_a, valueX_, valueY_, size_column, size_line, value_[rank - 2]) * dt;
                    i++;
                    k[3, i] = rightPartFree_lattice(x, y, i, t + dt, parameter, param_a, valueX_, valueY_, size_column, size_line, value_[rank - 2]) * dt;
                    i++;
                }
            /////// хаос
            if (slychai == 2) //  ФХН
            {
                for (int r = 0; r < 2; r++) 
                {
                    k[3, i] = chaos_elem(r, parameter, parameter3, value3_) * dt;
                    i++;
                }
            }
            if (slychai == 3)// хаос Лоренц
            {
                for (int r = 0; r < 3; r++) //                                
                {
                    k[3, i] = attraction_Resslera_elem(r, value3_) * dt;
                    i++;
                }
            }///////хаос// // t + dt
            ///////////////////////////////////////////////////////////////////////////////////
            for (i = 0; i < rank; i++)
                value[i] = value[i] + 1.0 / 6 * (k[0, i] + 2 * k[1, i] + 2 * k[2, i] + k[3, i]);
            for (int x = 0; x < size_line; x++)
                for (int y = 0; y < size_column; y++)
                {
                    valueX[x, y] = valueX[x, y] + 1.0 / 6 * (k[0, i] + 2 * k[1, i] + 2 * k[2, i] + k[3, i]);
                    i++;//
                    valueY[x, y] = valueY[x, y] + 1.0 / 6 * (k[0, i] + 2 * k[1, i] + 2 * k[2, i] + k[3, i]);
                    i++;
                }
            if ((slychai == 2) || (slychai == 3)) // хаос
            {
                if (slychai == 2) hrank = 2; if (slychai == 3) hrank = 3;
                for (int r = 0; r < hrank; r++) //хаос
                {
                    value3[r] = value3[r] + 1.0 / 6 * (k[0, i] + 2 * k[1, i] + 2 * k[2, i] + k[3, i]);
                    i++;
                }
            }
            value3_.Clear();
            value3_.Capacity = 0;
            value_.Clear();
            value_.Capacity = 0;
        }

        static void calculate(int rank, ref double t, ref double dt, double tEst, double tMin, double tMax, double tFin, String boundaryConditions,
            List<double> value, List<double> data, List<double> parameter, String outFileNameDat)
        {
            if (data.Count < rank/2 )
            {
                for (int i = 0; i < rank / 2 + 7; i++)//rank/2 + 1
                {
                    data.Add(0.0);
                }
                for (int i = 0; i <  rank; i++)
                {
                    value.Add(0.0); 
                }
            }
            else if (t > tEst && t < tFin)
            {
                double T1_average, TN_average, omega1=0, omega2=0;
                int Flag = 0;
                int j = 1;//индекс для data
                for (int i = 0; i < rank; i += 2)//для каждого элемента           
                {
                    if (value[i] > 0.0 && value[rank + i] <= 0.0 ) //происходит оборот    
                    {
                        if (data[0] == 0)
                        { data[rank/2 + 1] = t;}//t1 первого элемента

                        if((data[rank / 2 + 3] == 0) && (j == rank/2))// t1 для последнего элемента
                        { data[rank / 2 + 3] = t; }//j == rank/2 - последний элемент
                     
                        if (j == 1)//если изменяется значение частоты первого элемента
                        { data[rank / 2 + 2] = t; }//tn первого элемента

                        if (j == rank / 2)//tn для последнего элемента
                        { data[rank / 2 + 4] = t; }

                        data[0] = t;
                        data[j] += 1.0;          //подсч средн част //частота jго элемента     
                        Flag = 1;

                        T1_average = 1.0 * (data[rank / 2 + 2] - data[rank / 2 + 1]) / (data[1]-1);//средний период первого элемента
                        TN_average = 1.0 * (data[rank / 2 + 4] - data[rank / 2 + 3]) / (data[rank/2]-1);//средний период последнего элемента
                        omega1 = 2 * Math.PI / T1_average;
                        omega2 = 2 * Math.PI / TN_average;
                        data[rank / 2 + 5] = omega1;
                        data[rank / 2 + 6] = omega2;
                        
                        /*if ((t + dt > tMax) && (f==0))//вывод 
                        {
                            StreamWriter swData = new StreamWriter(outFileNameDat, true);
                            swData.Write("{0:F8}", omega1);
                            swData.Write("    ");
                            swData.Write("{0:F8}", omega2);
                            swData.Write("    ");
                            swData.WriteLine();
                            swData.Close();
                            f++;
                        }*/
                    }
                    j++;
                }

                if ((t >= tMin && t <= tMax))//выввод  //(Flag == 1) &&   
                    dataoutput(rank, ref t, ref dt, tEst, tMin, tMax, tFin, boundaryConditions, value, data, parameter, outFileNameDat);             
            }
            for (int i = 0; i < rank; i++)
            {
                value[i + rank] = value[i];
            }
        }
        
        static void dataoutput(int rank, ref double t, ref double dt, double tEst, double tMin, double tMax, double tFin, String boundaryConditions,
            List<double> value, List<double> data, List<double> parameter, String outFileNameDat)
        {
            if (t + dt > tMax)//вывод только последней строки "omega1 omega2"
            {
                StreamWriter swData = new StreamWriter(outFileNameDat, true);
                //swData.Write("{0:F2}", t);
                swData.Write(parameter[0]);
                swData.Write("    ");
                for (int i = data.Count - 2; i < data.Count; i++)//i=0  //data.Count - 3
                {
                    //swData.Write("    ");
                    swData.Write("{0:F8}", data[i]);
                    swData.Write("    ");
                }
                swData.WriteLine();
                swData.Close();
            }
        }

        static double rightPart1(int flag_first_elem_exist, int rank, int eqNumber, double t, List<double> parameter, List<double> value, double resh_start_value) // 1 element
        {//этот метод не исполюзую
            double a1 = parameter[0], da = parameter[1], betta = parameter[2], eps = parameter[3];
            double result = 0;
            if (eqNumber == 0)
            {
                result = value[0] - Math.Pow(value[0], 3.0) / 3.0 - value[1] + betta*(resh_start_value - value[0]);
            }
            else
            {
                result = eps * (value[0] + a1);
            }
            return result;
        }

        static double chaos_elem(int eqNumber/*i*/, List<double> parameter, double parameter_chaos, List<double> value)
        {       //ФХН   атвоколеб                               //цепочки               
            double eps = parameter[parameter.Count - 1];
            double result = 0;
            if (eqNumber / 2 * 2 != eqNumber)//четный т е y(нечетн i)
            {
                result = eps * (value[eqNumber - 1] + parameter_chaos);
            }
            else if (eqNumber == 0) // левый и правый автоколебат
            {
                result = value[eqNumber] - Math.Pow(value[eqNumber], 3.0) / 3.0 - value[eqNumber + 1];//соединение со второй цепочкой
            }
            return result;
        }

        static double attraction_Resslera_elem(int eqNumber/*i*/, List<double> value)
        {   //элемент Лоренца
            double a = 0.38, b = 0.2, c = 5.7;//a = 0.38,
            double result = 0;
            if (eqNumber == 0) //x'= sigma(y-x)
            {
                result = -value[1] - value[2]; //x'= -y-z
            }
            else if (eqNumber == 1) // y' = x(r - z) - y
            {
                result = value[0] + a * value[1]; //x+ay
            }
            else if (eqNumber == 2)  // z' = xy - bz       
            {
                result = b + value[2] * (value[0] - c);//b+z(x-c)
            }
            return result;
        }

        static double rightPartFree(int slychai, int flag_first_elem_exist, int rank, int eqNumber/*i*/, double t, List<double> parameter, List<double> value, double elem_resh_value, double elem3)
        {
            int elem_chaos = (int)(rank / 2)- 3 - 12;
            if (slychai != 1)
            {
                if (elem_chaos / 2 * 2 != elem_chaos) //( т е если не четн, нам надо x)
                {
                    elem_chaos++;
                }
            }
            double betta = parameter[parameter.Count - 2], eps = parameter[parameter.Count - 1];
            double[] a = new double[rank/2];
            for (int i = 0; i < a.Count(); i++)
            { a[i] = parameter[i]; }
            double result = 0;
            if (eqNumber / 2 * 2 != eqNumber)//четный т е y(нечетн i)
            {
                result = eps * (value[eqNumber - 1] + a[eqNumber/2] );
            }
            else if (eqNumber == 0)
            {
                result = value[eqNumber] - Math.Pow(value[eqNumber], 3.0) / 3.0 - value[eqNumber + 1] + betta * (value[eqNumber + 2] - value[eqNumber]);//соединение со второй цепочкой
                if (flag_first_elem_exist==1)
                    result = value[eqNumber] - Math.Pow(value[eqNumber], 3.0) / 3.0 - value[eqNumber + 1] + betta * (value[eqNumber + 2] - value[eqNumber]);//для одной цепочки у которой первый не интегр-ся
                //result = value[eqNumber] - Math.Pow(value[eqNumber], 3.0) / 3.0 - value[eqNumber + 1] + betta * (value[eqNumber + 2] - value[eqNumber]);//для одной цепочки
            }
            else if (eqNumber == rank - 2)
            {
                //result = value[eqNumber] - Math.Pow(value[eqNumber], 3.0) / 3.0 - value[eqNumber + 1] + betta * (value[eqNumber - 2] - value[eqNumber]);//для одной цепочки
                result = value[eqNumber] - Math.Pow(value[eqNumber], 3.0) / 3.0 - value[eqNumber + 1] + betta * (elem_resh_value - 2*value[eqNumber] + value[eqNumber - 2]);//соединение с решеткой
            }
            else if ((eqNumber == elem_chaos) && (slychai == 3)) // две цепочки с хаотич сигналом
            {//  хаотич сигнал Лоренца                                                                                                                          2
                result = value[eqNumber] - Math.Pow(value[eqNumber], 3.0) / 3.0 - value[eqNumber + 1] + betta * (value[eqNumber + 2] + value[eqNumber - 2] - 2 * value[eqNumber]) + betta / 1.4 * elem3;
            }
            else if ((eqNumber == elem_chaos) && (slychai == 2)) // две цепочки с хаотич сигналом
            {//  сигнал ФХН
                result = value[eqNumber] - Math.Pow(value[eqNumber], 3.0) / 3.0 - value[eqNumber + 1] + betta * (value[eqNumber + 2] + value[eqNumber - 2] + elem3 - 3 * value[eqNumber]);
            }
            else
            {
                result = value[eqNumber] - Math.Pow(value[eqNumber], 3.0) / 3.0 - value[eqNumber + 1] + betta * (value[eqNumber + 2] - 2 * value[eqNumber] + value[eqNumber - 2]);
            }           
            return result;
        }

        static double rightPartFree_lattice(int i, int j, int k, double t, List<double> parameter, double param_a, double [,] valueX, double[,] valueY, int size_column, int size_line, double chain1_value)
        {
            double betta = parameter[parameter.Count - 2], eps = parameter[parameter.Count - 1];
            double result = 0;
            if (k / 2 * 2 != k)//четный т е y(нечетн k)
            {
                result = eps * (valueX[i, j] + param_a);// было valueY исправили
            }
            else if (i == 0 && j == 0)//верхний левый
            {
                result = valueX[0, 0] - Math.Pow(valueX[0, 0], 3.0) / 3.0 - valueY[0, 0] + betta * (chain1_value + valueX[0, 1] + valueX[1, 0] - 3 * valueX[0, 0]);
            }
            else if (i == size_line - 1 && j == size_column - 1)//нижний правый
            {
                result = valueX[i, j] - Math.Pow(valueX[i, j], 3.0) / 3.0 - valueY[i, j] + betta * (valueX[i, j - 1] + valueX[i - 1, j] - 2 * valueX[i, j]);
            }//3elem
            ////////////////////////2 elem
            else if (i == 0 && j == size_column - 1)//верхний правый
            {
                result = valueX[i, j] - Math.Pow(valueX[i, j], 3.0) / 3.0 - valueY[i, j] + betta * (valueX[i, j - 1] + valueX[i + 1, j] - 2 * valueX[i, j]);
            }
            else if (i == size_line - 1 && j == 0)//нижний левый
            {
                result = valueX[i, j] - Math.Pow(valueX[i, j], 3.0) / 3.0 - valueY[i, j] + betta * (valueX[i, j + 1] + valueX[i - 1, j] - 2 * valueX[i, j]);
            }            
            ////////////////////////2 elem
            ////////////////////////3 elem
            else if (i == 0 && 0 < j && j < size_column - 1)//все элементы в первой строке между первым и последним в строке эл-ми
            {
                result = valueX[i, j] - Math.Pow(valueX[i, j], 3.0) / 3.0 - valueY[i, j] + betta * (valueX[i, j - 1] + valueX[i, j + 1] + valueX[i + 1, j] - 3 * valueX[i, j]);
            }
            else if (j == 0 && 0 < i && i < size_line - 1)//все элементы в первом столбце между первым и последнем в столбце элем-ми
            {
                result = valueX[i, j] - Math.Pow(valueX[i, j], 3.0) / 3.0 - valueY[i, j] + betta * (valueX[i - 1, j] + valueX[i, j + 1] + valueX[i + 1, j] - 3 * valueX[i, j]);
            }
            else if (j == size_column - 1 && 0 < i && i < size_line - 1)//все элемнты в последнем столбце между первым и последним в этом столбце эл-ми
            {
                result = valueX[i, j] - Math.Pow(valueX[i, j], 3.0) / 3.0 - valueY[i, j] + betta * (valueX[i - 1, j] + valueX[i, j - 1] + valueX[i + 1, j] - 3 * valueX[i, j]);
            }
            else if (i == size_line - 1 && 0 < j && j < size_column - 1)//все элем в последней строке между первым и последним эл в этой строке
            {
                result = valueX[i, j] - Math.Pow(valueX[i, j], 3.0) / 3.0 - valueY[i, j] + betta * (valueX[i, j - 1] + valueX[i, j + 1] + valueX[i - 1, j] - 3 * valueX[i, j]);
            }
            ////////////////////////3 elem
            ////////////////////////4 elem
            else
            {
                result = valueX[i, j] - Math.Pow(valueX[i, j], 3.0) / 3.0 - valueY[i, j] + betta * (valueX[i-1, j] + valueX[i, j + 1] + valueX[i + 1, j] + valueX[i,j-1] - 4 * valueX[i, j]);
            }
            ////////////////////////4 elem
            return result;
        }
    }
}