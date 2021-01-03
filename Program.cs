using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;

namespace lab4
{
    class Program
    {
        private static double UTCToJulianDate(int year, int mounth, int day, int hour = 0, int min = 0, int sec = 0)
        {
            return 367 * year - (int)(7 * (year + (int)((mounth + 9) / 12.0)) / 4) + (int)(275 * mounth / 9.0) +
                day + 1721013.5 + ((sec / 60.0 + min) / 60.0 + hour) / 24.0;
        }

        private static double JulianDatetoLST(double julianDate, double longitude)
        {
            double omega_E = 0.000072921158553;
            double Tut1 = (julianDate - 2451545) / 36525;
            double UT1 = 86400 * (julianDate - (int)julianDate);
            double GST0 = 100.4606184 + 36000.77005361 * Tut1 + 0.00038793 * Math.Pow(Tut1, 2) - 2.6e-8 * Math.Pow(Tut1, 3);
            double GST = GST0 + omega_E * UT1;
            return GST + longitude;
        }

        private static Matrix Rotate1(double alpha)
        {
            double[,] temp = new double[3, 3]
            {
                {1, 0, 0 },
                {0, Math.Cos(alpha), Math.Sin(alpha)},
                {0, -Math.Sin(alpha),  Math.Cos(alpha)}
            };
            Matrix tempMatrix = new Matrix(temp);
            return tempMatrix;
        }

        private static Matrix Rotate2(double alpha)
        {
            double[,] temp = new double[3, 3]
            {
                {Math.Cos(alpha), 0, -Math.Sin(alpha) },
                {0, 1, 0},
                {Math.Sin(alpha), 0,  Math.Cos(alpha)}
            };
            Matrix tempMatrix = new Matrix(temp);
            return tempMatrix;
        }

        private static Matrix Rotate3(double alpha)
        {
            double[,] temp = new double[3, 3]
            {
                {Math.Cos(alpha), Math.Sin(alpha), 0 },
                {-Math.Sin(alpha), Math.Cos(alpha), 0 },
                {0, 0, 1 }
            };
            Matrix tempMatrix = new Matrix(temp);
            return tempMatrix;
        }

        public static void SaveData<T>(T[] array, string title)
        {
            string path = $"{Environment.CurrentDirectory}\\{title}.txt";
            using (StreamWriter writer = File.CreateText(path))
            {
                string output = $"-----------------------{title}----------------------\n";

                for (int k = 0; k < array.Length; k++)
                {
                    output += array[k] + "\n";
                }
                writer.Write(output);
            }
        }

        public static void PrintArray<T>(T[] array, string title)
        {
            Console.WriteLine($"-----------------------{title}----------------------");
            for (int k = 0; k < array.Length; k++)
            {
                Console.WriteLine(array[k]);
            }
            Console.Write("\n\n\n\n\n\n");
        }

        static void Main(string[] args)
        {
            double eE = 0.081819221456;
            double RE = 6378.137;
            double omega_E = 0.000072921158553;
            double M_PI = 3.1415926;

            double t0 = UTCToJulianDate(2020, 9, 22, 15, 15, 0);

            double longitude_deg = 27 + 33 / 60.0 + 52 / 3600.0;
            double latitude_deg = 53 + 54 / 60.0 + 27 / 3600.0;
            double longitude_rad = longitude_deg / 180 * M_PI;
            double H = 0;        

            double ro = 1734;
            double el = 9 + 52 / 60.0 + 3 / 3600.0;
            double N = 85 + 13 / 60.0 + 50 / 3600.0;

            el = el / 180 * M_PI;
            N = N / 180 * M_PI;
            double phi = latitude_deg / 180 * M_PI;

            double LST = JulianDatetoLST(t0, longitude_rad);

            double Ce = RE / Math.Sqrt(1 - Math.Pow(eE, 2) * Math.Pow(Math.Sin(phi), 2));
            double Se = Ce * (1 - Math.Pow(eE, 2));

            double r_delta = (Ce + H) * Math.Cos(phi);
            double r_k = (Se + H) * Math.Sin(phi);

            Vector3 r_tsr = new Vector3(r_delta * Math.Cos(LST), r_delta * Math.Sin(LST), r_k);
            Vector3 temp = new Vector3(0, 0, omega_E);
            Vector3 v_tsr = Vector3.Cross(temp, r_tsr);

            Vector3 ro_seu = new Vector3(-ro * Math.Cos(el) * Math.Cos(N), ro * Math.Cos(el) * Math.Sin(N), ro * Math.Sin(el));

            Vector3 ro_xyz = new Vector3(Rotate3(-LST) * Rotate2(phi - M_PI / 2) * ro_seu);
            Vector3 r_eci = ro_xyz + r_tsr;

            Console.WriteLine("ro_seu: \n" + ro_seu);

            Console.WriteLine("r_eci: \n" + r_eci);

            Console.ReadKey();
        }
    }
}
