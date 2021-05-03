using System;

namespace LearningHangFire
{
    public interface IJobService
    {
        void PrintJob();
    }

    //Servicio de ejemplo que trabajara junto con hangfire
    public class JobService : IJobService 
    {
        public void PrintJob()
        {
            Console.WriteLine($"Hang Fire Recurring Job");
        }
    }
}
