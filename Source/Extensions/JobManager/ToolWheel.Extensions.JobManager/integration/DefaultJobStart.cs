using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolWheel.Extensions.JobManager;
using ToolWheel.Extensions.JobManager.Configuration;
using ToolWheel.Extensions.JobManager.Service;
using System.Linq.Expressions;
using System.Diagnostics;

namespace ToolWheel
{
    [TestFixture]
    public class DefaultJobStart
    {
        public class TestInstanz
        {
            public bool Completed { get; set; } = false;
        }

        private IServiceProvider CreateServiceProvider(Action<IJobManagerConfigurationBuilder>? configure = null)
        {
            var services = new ServiceCollection();

            services.AddJobManager(configure);

            return services.BuildServiceProvider();
        }


        [Test]
        [DebuggerHidden]
        public void Test_Execute_Job()
        {
            var instance = new TestInstanz();

            var provider = CreateServiceProvider(configure => configure
                .ConfigureJobs(jobs => jobs
                    .AddMethod(() => instance.Completed = true, m => m.Id("1"))
                ));

            var jobService = provider.GetRequiredService<IJobService>();
            var jobTaskService = provider.GetRequiredService<IJobTaskService>();
            var job = jobService.Find("1");

            Assert.That(job, Is.Not.Null);

            var jobTask = jobTaskService.Execute(job);

            jobTaskService.WaitAllTasks();

            Assert.That(instance.Completed, Is.True);
            Assert.That(jobTask, Is.Not.Null);
            Assert.That(jobTask.Status, Is.EqualTo(JobTaskStatus.Success));
        }

        [Test]
        [DebuggerHidden]
        public void Test_Execute_Job_With_Exception()
        {
            var instance = default(TestInstanz);

            var provider = CreateServiceProvider(configure => configure
                .ConfigureJobs(jobs => jobs
                    .AddMethod(() => instance.Completed = true, m => m.Id("1"))
                ));

            var jobService = provider.GetRequiredService<IJobService>();
            var jobTaskService = provider.GetRequiredService<IJobTaskService>();
            var job = jobService.Find("1");

            Assert.That(job, Is.Not.Null);

            var jobTask = jobTaskService.Execute(job);

            jobTaskService.WaitAllTasks();

            Assert.That(jobTask, Is.Not.Null);
            Assert.That(jobTask.Status, Is.EqualTo(JobTaskStatus.Failed));
        }
    }
}
