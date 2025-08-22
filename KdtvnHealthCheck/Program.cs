using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using Action = Microsoft.Win32.TaskScheduler.Action;


namespace KdtvnHealthCheck
{
    public class Program
    {
        static void Main(string[] args)
        {

            CreateTask();
            Console.WriteLine("Task Scheduler has been created successfully.");

        }

        private static void CreateTask()
        {
            TaskDefinition taskDefinition = TaskService.Instance.NewTask();
            taskDefinition.RegistrationInfo.Description = "Starts KdtvnHealthCheck on Windows startup.";

            LogonTrigger logonTrigger = new LogonTrigger
            {
                Delay = TimeSpan.FromMinutes(5) // Trì hoãn 5 minutes
            };
            taskDefinition.Triggers.Add(logonTrigger);

            BootTrigger bootTrigger = new BootTrigger
            {
                Delay = TimeSpan.FromMinutes(5) // Trì hoãn 5 minutes
            };
            taskDefinition.Triggers.Add(bootTrigger);

            taskDefinition.Settings.StartWhenAvailable = true;
            taskDefinition.Settings.DisallowStartIfOnBatteries = false;
            taskDefinition.Settings.StopIfGoingOnBatteries = false;
            taskDefinition.Settings.ExecutionTimeLimit = TimeSpan.Zero;
            taskDefinition.Settings.AllowHardTerminate = false;

            taskDefinition.Principal.RunLevel = TaskRunLevel.Highest;
            taskDefinition.Principal.LogonType = TaskLogonType.InteractiveToken;

            string directory = AppDomain.CurrentDomain.BaseDirectory;
            string filename = $"KdtvnHealthCheck.bat"; // Tên file bat thực thi
            string fullPath = Path.Combine(directory, filename);

            taskDefinition.Actions.Add(new ExecAction(fullPath, "", Path.GetDirectoryName(fullPath)));

            TaskService.Instance.RootFolder.RegisterTaskDefinition(nameof(KdtvnHealthCheck), taskDefinition);
        }

    }
}
