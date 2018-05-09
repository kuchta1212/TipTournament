using Microsoft.Owin;
using Owin;
using Quartz;
using Quartz.Impl;
using TipTournament.Controllers;

[assembly: OwinStartupAttribute(typeof(TipTournament.Startup))]
namespace TipTournament
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            StdSchedulerFactory factory = new StdSchedulerFactory();
            var scheduler = factory.GetScheduler().Result;
            scheduler.Start();

            var jobDetail = JobBuilder.Create<ImportJob>().Build();
            var trigger =
                TriggerBuilder.Create().WithSimpleSchedule(x => x.WithIntervalInSeconds(15).RepeatForever()).Build();

            var scheduleJob = scheduler.ScheduleJob(jobDetail, trigger);

        }
    }
}
