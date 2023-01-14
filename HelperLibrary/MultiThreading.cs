using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GetCookieFBTool.HelperLibrary
{
	public class MultiThreading<T> : IDisposable
	{

		// (get) Token: 0x06000029 RID: 41 RVA: 0x00002FAC File Offset: 0x000011AC
		// (set) Token: 0x0600002A RID: 42 RVA: 0x00002FB4 File Offset: 0x000011B4
		public ConcurrentQueue<T> queueAccs { get; set; }


		// (get) Token: 0x0600002B RID: 43 RVA: 0x00002FBD File Offset: 0x000011BD
		public int Count
		{
			get
			{
				return this.queueAccs.Count;
			}
		}


		// (get) Token: 0x0600002C RID: 44 RVA: 0x00002FCA File Offset: 0x000011CA
		// (set) Token: 0x0600002D RID: 45 RVA: 0x00002FD2 File Offset: 0x000011D2
		public bool IsWorking { get; private set; }


		// (get) Token: 0x0600002E RID: 46 RVA: 0x00002FDB File Offset: 0x000011DB
		// (set) Token: 0x0600002F RID: 47 RVA: 0x00002FE3 File Offset: 0x000011E3
		public bool IsPaused { get; private set; }


		// (get) Token: 0x06000030 RID: 48 RVA: 0x00002FEC File Offset: 0x000011EC
		// (set) Token: 0x06000031 RID: 49 RVA: 0x00002FF4 File Offset: 0x000011F4
		public bool IsStopping { get; private set; }


		// (get) Token: 0x06000032 RID: 50 RVA: 0x00002FFD File Offset: 0x000011FD
		// (set) Token: 0x06000033 RID: 51 RVA: 0x00003005 File Offset: 0x00001205
		public int ThreadCount { get; set; }
		public static bool isUseProxy { get; set; } = false;
		public int delayTime { get; set; }

		// (get) Token: 0x06000034 RID: 52 RVA: 0x0000300E File Offset: 0x0000120E
		public int Completed
		{
			get
			{
				return this.nCompleted;
			}
		}


		// (add) Token: 0x06000035 RID: 53 RVA: 0x00003016 File Offset: 0x00001216
		// (remove) Token: 0x06000036 RID: 54 RVA: 0x0000302F File Offset: 0x0000122F
		public event EventHandler<EventArgs> WorkCompleted
		{
			add
			{
				this._workCompletedAsyncEvent = (EventHandler<EventArgs>)Delegate.Combine(this._workCompletedAsyncEvent, value);
			}
			remove
			{
				this._workCompletedAsyncEvent = (EventHandler<EventArgs>)Delegate.Remove(this._workCompletedAsyncEvent, value);
			}
		}


		public MultiThreading()
		{
			this.queueAccs = new ConcurrentQueue<T>();
			this.ThreadCount = 1;
			this.delayTime = 0;
		}


		public MultiThreading(int ThrCount)
		{
			this.ThreadCount = ThrCount;
		}


		private void ThreadMethod(object param)
		{
			MultiThreading<T>.ThreadParams threadParams = (MultiThreading<T>.ThreadParams)param;
			try
			{
				do
				{
					this.evtPauseEvent.WaitOne(-1);
					if (this.queueAccs.Count <= 0)
					{
						break;
					}
					// get account in list
					T obj;
					if (this.queueAccs.TryDequeue(out obj))
					{
						this.evtPauseEvent.WaitOne(-1);
						threadParams.Action(obj);
						Interlocked.Increment(ref this.nCompleted);
					}
				}
				while (this.IsWorking);
			}
			finally
			{
				if (Interlocked.Decrement(ref this.nCurrentThreadCount) == 0 && !this.IsPaused)
				{
					this.IsWorking = false;
					EventHandler<EventArgs> workCompletedAsyncEvent = this._workCompletedAsyncEvent;
					if (workCompletedAsyncEvent != null)
					{
						workCompletedAsyncEvent(this, EventArgs.Empty);
					}
				}
			}
		}


		public void Run(ConcurrentQueue<T> queue, Action<T> action)
		{
			if (this.IsWorking)
			{
				throw new InvalidOperationException();
			}
			if (queue == null || queue.Count == 0)
			{
				throw new ArgumentNullException("Queue is empty!");
			}
			if (action == null)
			{
				throw new ArgumentNullException("Action in NULL!");
			}
			if (this.ThreadCount > queue.Count)
			{
				this.ThreadCount = queue.Count;
			}

			this.queueAccs = queue;
			this.LsThreads = new List<Thread>(this.ThreadCount);
			this.nCompleted = 0;
			this.nCurrentThreadCount = this.ThreadCount;
			this.evtPauseEvent = new ManualResetEvent(true);
			this.IsStopping = false;
			this.IsWorking = true;

			try
			{
				for (int i = 0; i < this.ThreadCount; i++)
				{
					Thread thread = new Thread(new ParameterizedThreadStart(this.ThreadMethod))
					{
						IsBackground = true,
						Name = string.Format("Thread #{0}", i)
					};
					this.LsThreads.Add(thread);
					thread.Start(new MultiThreading<T>.ThreadParams
					{
						Action = action
					});
					Thread.Sleep(delayTime);
				}
			}
			catch (Exception)
			{
				this.IsWorking = false;
				throw;
			}
		}


		public void Run(Action<T> action)
		{

			if (this.IsWorking)
			{
				throw new InvalidOperationException();
			}
			if (this.queueAccs == null || this.queueAccs.Count == 0)
			{
				throw new ArgumentNullException("Queue is empty!");
			}
			if (action == null)
			{
				throw new ArgumentNullException("Action in NULL!");
			}
			if (this.ThreadCount > this.queueAccs.Count)
			{
				this.ThreadCount = this.queueAccs.Count;
			}

			this.LsThreads = new List<Thread>(this.ThreadCount);
			this.nCompleted = 0;
			this.nCurrentThreadCount = this.ThreadCount;
			this.evtPauseEvent = new ManualResetEvent(true);
			this.IsStopping = false;
			this.IsWorking = true;

			try
			{
				for (int i = 0; i < this.ThreadCount; i++)
				{
					Thread thread = new Thread(new ParameterizedThreadStart(this.ThreadMethod))
					{
						IsBackground = true,
						Name = string.Format("Thread #{0}", i)
					};
					this.LsThreads.Add(thread);
					thread.Start(new MultiThreading<T>.ThreadParams
					{
						Action = action
					});
					Thread.Sleep(delayTime);
				}
			}
			catch (Exception)
			{
				this.IsWorking = false;
				throw;
			}
		}


		public void Pause()
		{
			if (!this.IsWorking)
			{
				return;
			}

			if (!this.IsPaused)
			{
				this.IsPaused = true;
				this.evtPauseEvent.Reset();
			}
		}


		public void Resume()
		{
			if (!this.IsWorking)
			{
				throw new InvalidOperationException();
			}
			if (this.IsPaused)
			{
				this.IsPaused = false;
				this.evtPauseEvent.Set();
			}
		}


		public void AddItems(Queue<T> queue)
		{
			foreach (T item in queue)
			{
				this.queueAccs.Enqueue(item);
			}
		}


		public void AddItem(T account)
		{
			ConcurrentQueue<T> queueAccs = this.queueAccs;
			lock (queueAccs)
			{
				this.queueAccs.Enqueue(account);
			}
		}


		public void Cancel()
		{
			if (!this.IsWorking)
			{
				//throw new InvalidOperationException();
				return;
			}
			this.IsStopping = true;
			foreach (Thread thread in this.LsThreads)
			{
				try
				{
					thread.Abort();
				}
				catch
				{
				}
			}
			this.LsThreads = new List<Thread>();
			this.IsStopping = false;
			this.IsWorking = false;
		}


		public void Dispose()
		{
			this.LsThreads = new List<Thread>();
			this.queueAccs = new ConcurrentQueue<T>();
			this.IsWorking = false;
			this.IsStopping = false;
		}


		private int nCurrentThreadCount;


		private int nCompleted;


		public ManualResetEvent evtPauseEvent;


		private List<Thread> LsThreads;


		private EventHandler<EventArgs> _workCompletedAsyncEvent;


		private class ThreadParams
		{

			// (get) Token: 0x06000196 RID: 406 RVA: 0x00013CEE File Offset: 0x00011EEE
			// (set) Token: 0x06000197 RID: 407 RVA: 0x00013CF6 File Offset: 0x00011EF6
			public Action<T> Action { get; internal set; }
		}
	}
}
