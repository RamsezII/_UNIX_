using System;
using System.Collections.Generic;

namespace _ARK_
{
    [Obsolete]
    public class SCHEDULER_old : BOSON
    {
        public static readonly SCHEDULER_old scheduler = new(new(null, SIGT.EXEC));
        readonly Queue<BOSON> queue = new();
        readonly List<BOSON> list = new();

        //----------------------------------------------------------------------------------------------------------

        SCHEDULER_old(in SIGNAL signal) : base(signal)
        {
        }

        //----------------------------------------------------------------------------------------------------------

        public void ADD_PARALLEL(in BOSON boson)
        {
            lock (list)
                if (list.Contains(boson))
                    throw new Exception($"{this}.{nameof(ADD_PARALLEL)} -> {boson} is already scheduled");
                else
                {
                    lock (boson.state)
                        boson.state._value = BOSON_STAT.SCHEDULED;
                    list.Add(boson);
                }
        }

        public void ADD_SEQUENTIAL(in BOSON boson)
        {
            lock (queue)
                if (queue.Contains(boson))
                    throw new Exception($"{this}.{nameof(ADD_SEQUENTIAL)} -> {boson} is already scheduled");
                else
                {
                    lock (boson.state)
                        boson.state._value = BOSON_STAT.SCHEDULED;
                    queue.Enqueue(boson);
                }
        }

        public void GIGATICK()
        {
            lock (list)
                for (int i = 0; i < list.Count; i++)
                {
                    BOSON boson = list[i];
                    lock (boson.state)
                        switch (boson.state._value)
                        {
                            case BOSON_STAT.SCHEDULED:
                                boson.OnSchedule();
                                break;

                            case BOSON_STAT.RUNNING:
                                boson.OnTick();
                                break;

                            case BOSON_STAT.DISPOSED:
                                list.RemoveAt(i--);
                                break;
                        }
                }

            lock (queue)
                if (queue.TryPeek(out BOSON boson))
                    lock (boson.state)
                        switch (boson.state._value)
                        {
                            case BOSON_STAT.SCHEDULED:
                                boson.OnSchedule();
                                break;

                            case BOSON_STAT.RUNNING:
                                boson.OnTick();
                                break;

                            case BOSON_STAT.DISPOSED:
                                queue.Dequeue();
                                break;
                        }
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDispose()
        {
            lock (list)
            {
                for (int i = 0; i < list.Count; i++)
                    list[i].Dispose();
                list.Clear();
            }
            lock (queue)
            {
                foreach (BOSON boson in queue)
                    boson.Dispose();
                queue.Clear();
            }
        }
    }
}