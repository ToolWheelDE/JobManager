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
            public bool Started { get; set; } = false;

            public void TestJob_Success()
            {
                Started = true;
            }
        }

        private IServiceProvider CreateServiceProvider(Action<IJobManagerConfigurationBuilder>? configure = null)
        {
            var services = new ServiceCollection();

            services.AddJobManager(configure);

            return services.BuildServiceProvider();
        }


        [Test]
        //[DebuggerHidden]
        public void Test_Execute_Job()
        {
            var provider = CreateServiceProvider(configure => configure
                .ConfigureJobs(jobs => jobs
                    .AddMethod<TestInstanz>(m => m.TestJob_Success, m => m.Id("1"))
                ));

            var jobService = provider.GetRequiredService<IJobService>();
            var jobTaskService = provider.GetRequiredService<IJobTaskService>();
            var job = jobService.Find("1");

            Assert.That(job, Is.Not.Null);

            var jobTask = jobTaskService.Execute(job);

            jobTaskService.WaitAllTasks();

            Assert.That(jobTask, Is.Not.Null);
            Assert.That(jobTask.Status, Is.EqualTo(JobTaskStatus.Success));
        }

        [Test]
        [DebuggerHidden]
        public void Test_Execute_Job_Inline()
        {
            var instance = new TestInstanz();

            var provider = CreateServiceProvider(configure => configure
                .ConfigureJobs(jobs => jobs
                    .AddMethod(() => instance.Started = true, m => m.Id("1"))
                ));

            var jobService = provider.GetRequiredService<IJobService>();
            var jobTaskService = provider.GetRequiredService<IJobTaskService>();
            var job = jobService.Find("1");

            Assert.That(job, Is.Not.Null);

            var jobTask = jobTaskService.Execute(job);

            jobTaskService.WaitAllTasks();

            Assert.That(instance.Started, Is.True);
            Assert.That(jobTask, Is.Not.Null);
            Assert.That(jobTask.Status, Is.EqualTo(JobTaskStatus.Success));
        }

        [Test]
        [DebuggerHidden]
        public void Test_Execute_Job_Inline_With_Exception()
        {
            var instance = default(TestInstanz);

            var provider = CreateServiceProvider(configure => configure
                .ConfigureJobs(jobs => jobs
                    .AddMethod(() => instance.Started = true, m => m.Id("1"))
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
