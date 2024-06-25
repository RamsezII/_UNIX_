using System.Collections.Generic;
using System.Text;
using _UNIX_;
using _UTIL_;
using UnityEngine;

namespace _UNIX_
{
    public partial class TERMINAL
    {
        readonly List<BOSON> temp_bosons = new();
        readonly ThreadSafe<bool> rebuildTemp = new(true);
        float lastTemp;
        byte tempI;

        //----------------------------------------------------------------------------------------------------------

        public void AddTask(in BOSON boson)
        {
            lock (temp_bosons)
                if (!temp_bosons.Contains(boson))
                    temp_bosons.Add(boson);
        }

        public void RemoveTemp(in BOSON boson)
        {
            lock (temp_bosons)
                temp_bosons.Remove(boson);
        }

        void AutoRebuildTemp()
        {
            if (Time.unscaledTime < lastTemp + .1f)
                return;

            lock (temp_bosons)
                if (temp_bosons.Count == 0)
                    return;

            lastTemp = Time.unscaledTime;
            rebuildTemp.Value = true;
        }

        void RebuildTemp()
        {
            rebuildTemp.Value = false;
            tempI = (byte)(++tempI % 4);

            char logo = tempI switch
            {
                0 => '|',
                1 => '/',
                2 => '-',
                3 => '\\',
                _ => '_',
            };

            StringBuilder sb1 = new(), sb2 = new();

            lock (temp_bosons)
                for (int i = 0; i < temp_bosons.Count; i++)
                {
                    BOSON boson = temp_bosons[i];
                    lock (boson.state)
                        if (boson.state._value == BOSON_STAT.DISPOSED)
                            temp_bosons.RemoveAt(i--);
                        else
                            lock (boson.TASK_LOG)
                                if (boson.background)
                                    sb1.AppendLine($"{logo} {boson.TASK_LOG}");
                                else
                                    sb2.AppendLine($"{logo} {boson.TASK_LOG}");
                }

            if (sb1.Length > 0)
                tasks1_text.text = sb1.ToString()[..^1];
            else
                tasks1_text.text = string.Empty;

            if (sb2.Length > 0)
                tasks2_text.text = sb2.ToString()[..^1];
            else
                tasks2_text.text = string.Empty;
        }
    }
}