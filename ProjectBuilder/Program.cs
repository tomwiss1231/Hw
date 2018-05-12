using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectBuilder
{
    internal static class Program
    {
        private static void ProjectDelay()
        {
            Task.Delay(TimeSpan.FromSeconds(1));
        }
        
        public static void Main(string[] args)
        {
            var project1Task = Task.Run(() =>
            {
                Console.Out.WriteLine("project1Task");
                ProjectDelay();
            });
            
            var project2Task = Task.Run(() =>
            {
                Console.Out.WriteLine("project2Task");
                ProjectDelay();
            });
            
            var project3Task = Task.Run(() =>
            {
                Console.Out.WriteLine("project3Task");
                ProjectDelay();
            });
            
            var project4Task = project1Task.ContinueWith(_=>
            {
                Console.Out.WriteLine("project4Task");
                ProjectDelay();
            });
            
            var project5Task = Task.Run(() =>
            {
                Task.WaitAll(project1Task, project2Task, project3Task);
                Console.Out.WriteLine("project5Task");
                ProjectDelay();
            }); 
            
            var project6Task = Task.Run(() =>
            {
                Task.WaitAll(project3Task, project4Task);
                Console.Out.WriteLine("project6Task");
                ProjectDelay();
            });

            var project7Task = Task.Run(() =>
            {
                Task.WaitAll(project5Task, project6Task);
                Console.Out.WriteLine("project7Task");
                ProjectDelay();
            });

            var project8Task = Task.Run(() =>
            {
                Task.WaitAll(project5Task);
                Console.Out.WriteLine("project8Task");
                ProjectDelay();
            });

            Task.WaitAll(project1Task, project2Task, project3Task, project4Task, project5Task, project6Task,
                         project7Task, project8Task);

        }
    }
}