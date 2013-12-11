// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UIThreadScheduler.cs" company="sgmunn">
//   (c) sgmunn 2013  
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//   the Software.
//
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//   THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//   IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
//  Inspired by
//  https://gist.github.com/1431457
// --------------------------------------------------------------------------------------------------------------------

namespace Mobile.Utils.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Android.OS;

    public class UIThreadScheduler : TaskScheduler
    {
        private readonly object taskListLock = new object();

        private readonly Handler handler;

        private List<ScheduledTask> taskList = new List<ScheduledTask>();

        private class ScheduledTask
        {
            public Task TheTask;
            public bool ShouldRun = true;
            public bool IsRunning = false;
        }

        public UIThreadScheduler ()
        {
            this.handler = new Handler();
        }

        public override int MaximumConcurrencyLevel 
        {
            get 
            {
                return int.MaxValue;
            }
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            lock (this.taskListLock) 
            {
                return this.taskList.Select(x => x.TheTask).ToList();
            }
        }

        protected virtual void QueueAction(Action action)
        {
            this.handler.Post(action);
        }

        protected override void QueueTask(Task task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            var t = new ScheduledTask () { TheTask = task };

            lock (this.taskListLock) 
            {
                //
                // Cleanout the task list before adding this new task
                //
                this.taskList = this.taskList.Where (x => x.ShouldRun && !x.IsRunning).ToList();
                this.taskList.Add(t);
            }

            QueueAction (delegate {
                if (t.ShouldRun) 
                {
                    t.IsRunning = true;
                    base.TryExecuteTask(t.TheTask);
                }
            });
        }

        protected override bool TryDequeue (Task task)
        {
            var t = default(ScheduledTask);

            lock (this.taskListLock) 
            {
                t = this.taskList.FirstOrDefault(x => x.TheTask == task);
            }

            if (t != null && !t.IsRunning) {
                t.ShouldRun = false;
                return !t.IsRunning;
            } else {
                return false;
            }
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            if (task == null)
                throw new ArgumentNullException ("task");

            return base.TryExecuteTask(task);
        }
    }
}

