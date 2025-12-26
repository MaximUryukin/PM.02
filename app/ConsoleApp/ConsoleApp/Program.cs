// Формат входного файла «Kolchugino.net»:
//  <кол-во-вершин>
//  <from> <to> <длина_км>

using System.Globalization;

namespace ConsoleApp
{
    public class Program
    {
        static int n;                          // кол-во вершин
        static double[] speed;                 // speed[i] км/ч для рёбра i
        static int[,] edgeId;                  // для быстрого доступа к id рёбер
        static double[,] adjMetres;            // веса в метрах
        static (int from, int to, double km)[] edges; // описания рёбер

        static void Main()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            if (!LoadGraph("Kolchugino.txt"))
            {
                Console.WriteLine("Не удалось загрузить файл Kolchugino.txt");
                return;
            }

            GenerateSpeeds();
            BuildTimeMatrix();

            Console.WriteLine("Готовы к расчёту. Введите пункты 1–{0} (Q-выход)", n);
            while (true)
            {
                Console.Write("\n\nОткуда: ");
                string sFrom = Console.ReadLine().Trim();
                if (sFrom.Equals("q", StringComparison.OrdinalIgnoreCase)) break;
                Console.Write("Куда:   ");
                string sTo = Console.ReadLine().Trim();
                if (sTo.Equals("q", StringComparison.OrdinalIgnoreCase)) break;

                if (!int.TryParse(sFrom, out int v0) || !int.TryParse(sTo, out int v1) ||
                    v0 < 1 || v0 > n || v1 < 1 || v1 > n)
                {
                    Console.WriteLine("Номер(-а) вне диапазона 1–{0}", n);
                    continue;
                }
                v0--; v1--;

                PrintSpeeds();
                double timeMin = ComputeTimeAndPath(v0, v1, out var path);
                Console.WriteLine("\nМинимальное время: {0:F1} мин", timeMin);
                Console.WriteLine("Путь: {0}", string.Join(" → ", path.ConvertAll(x => x + 1)));
            }
        }
        public static bool LoadGraph(string path)
        {
            if (!File.Exists(path)) return false;
            var lines = File.ReadAllLines(path);
            if (lines.Length == 0) return false;
            n = int.Parse(lines[0].Trim());
            var tmp = new System.Collections.Generic.List<(int, int, double)>();
            for (int i = 1; i < lines.Length; i++)
            {
                var p = lines[i].Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                if (p.Length != 3) continue;
                int a = int.Parse(p[0]), b = int.Parse(p[1]);
                double km = double.Parse(p[2]);
                tmp.Add((a, b, km));
            }
            edges = tmp.ToArray();
            return true;
        }

        public static void GenerateSpeeds()
        {
            var rnd = new Random();
            speed = new double[edges.Length];
            for (int i = 0; i < edges.Length; i++)
                speed[i] = 30 + rnd.NextDouble() * 50; 
        }

        public static void BuildTimeMatrix()
        {
            adjMetres = new double[n, n];
            edgeId = new int[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                {
                    adjMetres[i, j] = double.MaxValue;
                    edgeId[i, j] = -1;
                }

            for (int e = 0; e < edges.Length; e++)
            {
                int u = edges[e].from - 1, v = edges[e].to - 1;
                double m = edges[e].km * 1000;          
                double sec = m / (speed[e] / 3.6);         
                double min = sec / 60;
                adjMetres[u, v] = min;
                edgeId[u, v] = e;
                adjMetres[v, u] = min;
                edgeId[v, u] = e;
            }
        }

        public static void PrintSpeeds()
        {
            Console.WriteLine("\nСкорости на участках (км/ч):");
            for (int e = 0; e < edges.Length; e++)
                Console.WriteLine("  {0}–{1}: {2:F1}", edges[e].from, edges[e].to, speed[e]);
        }

        public static double ComputeTimeAndPath(int start, int goal, out System.Collections.Generic.List<int> path)
        {
            double[] dist = Dijkstra(adjMetres, start);
            int[] prev = new int[n];
            for (int i = 0; i < n; i++) prev[i] = -1;
            bool[] vis = new bool[n];
            int unvis = n;

            for (int i = 0; i < n; i++) dist[i] = double.MaxValue;
            dist[start] = 0.0;

            while (unvis > 0)
            {
                int v = -1;
                for (int i = 0; i < n; i++)
                {
                    if (vis[i]) continue;
                    if (v == -1 || dist[v] > dist[i]) v = i;
                }
                vis[v] = true; unvis--;
                for (int i = 0; i < n; i++)
                {
                    if (dist[i] > dist[v] + adjMetres[v, i])
                    {
                        dist[i] = dist[v] + adjMetres[v, i];
                        prev[i] = v;
                    }
                }
            }

            path = new System.Collections.Generic.List<int>();
            if (dist[goal] == double.MaxValue) return -1;
            for (int at = goal; at != -1; at = prev[at])
                path.Add(at);
            path.Reverse();
            return dist[goal];
        }
        public static double[] Dijkstra(double[,] a, int v0)
        {
            double[] dist = new double[n];
            bool[] vis = new bool[n];
            int unvis = n;
            int v;

            for (int i = 0; i < n; i++)
                dist[i] = Double.MaxValue;
            dist[v0] = 0.0;

            while (unvis > 0)
            {
                v = -1;
                for (int i = 0; i < n; i++)
                {
                    if (vis[i])
                        continue;
                    if ((v == -1) || (dist[v] > dist[i]))
                        v = i;
                }
                vis[v] = true;
                unvis--;
                for (int i = 0; i < n; i++)
                {
                    if (dist[i] > dist[v] + a[v, i])
                        dist[i] = dist[v] + a[v, i];
                }
            }
            return dist;
        }
    }
}