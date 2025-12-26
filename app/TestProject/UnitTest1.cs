using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace ConsoleApp
{
    public class UnitTest1
    {
        public UnitTest1()
        {
            using (var writer = new StreamWriter("Kolchugino.txt"))
            {
                writer.WriteLine("3"); 
                writer.WriteLine("1 2 10"); 
                writer.WriteLine("2 3 15"); 
            }
        }

        [Fact]
        public void TestRouteCalculation()
        {
            const int StartVertex = 1;
            const int GoalVertex = 3;

            Program.LoadGraph("Kolchugino.txt");
            Program.GenerateSpeeds();
            Program.BuildTimeMatrix(); 

            List<int> path;
            double result = Program.ComputeTimeAndPath(StartVertex - 1, GoalVertex - 1, out path);

            Assert.True(result >= 0 && result <= 100, $"Время должно быть положительным числом и меньше 100 минут, фактическое значение: {result}");
            Assert.Equal(new List<int>() { 1, 2, 3 }, path.ConvertAll(x => x + 1)); 
        }

        public void Dispose()
        {
            File.Delete("Kolchugino.txt");
        }
    }
}
