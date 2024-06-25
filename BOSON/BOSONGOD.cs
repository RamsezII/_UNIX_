using System.Collections.Generic;
using _UTIL_;
using UnityEngine;

namespace _UNIX_
{
    public class BOSONGOD : MonoBehaviour
    {
        public static BOSONGOD bgod;
        ushort _id;
        public readonly Dictionary<ushort, BOSON> bosons = new();
        public static readonly ThreadSafe<bool> collectDeadBosons = new();

        //----------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            if (bgod == null)
                bgod = this;
            else
                throw new BOSON_EXCP("BOSONGOD already exists");
        }

        //----------------------------------------------------------------------------------------------------------

        public bool TryGetBoson<T>(ushort id, out T boson) where T : BOSON
        {
            lock (bosons)
                if (bosons.TryGetValue(id, out BOSON _boson))
                {
                    boson = (T)_boson;
                    return true;
                }
            boson = null;
            return false;
        }

        public void PropagateIDs(in BOSON boson)
        {
            if (boson != null && boson.BID == 0)
            {
                lock (bosons)
                    for (ushort i = 0; i < ushort.MaxValue; i++)
                    {
                        ushort id = (ushort)(i + _id);
                        if (id > 0)
                            if (!bosons.TryGetValue(id, out BOSON old) || old.state.Value == BOSON_STAT.DISPOSED)
                            {
                                bosons[id] = boson;
                                boson.BID = id;
                                _id = (ushort)(id + 1);
                                PropagateIDs(boson.b_out);
                                PropagateIDs(boson.b_err);
                                return;
                            }
                    }
                throw new BOSON_EXCP($"{this}: No available ID");
            }
        }

        public void CollectDeadBosons()
        {
            List<ushort> deadIDs = new();

            lock (bosons)
                foreach (var pair in bosons)
                    if (pair.Value.state.Value == BOSON_STAT.DISPOSED)
                        deadIDs.Add(pair.Key);

            lock (bosons)
                for (int i = 0; i < deadIDs.Count; i++)
                    bosons.Remove(deadIDs[i]);
        }

        //----------------------------------------------------------------------------------------------------------

        private void LateUpdate()
        {
            if (collectDeadBosons.PullValue())
                CollectDeadBosons();
        }

        //----------------------------------------------------------------------------------------------------------

        private void OnDestroy()
        {
            if (bgod == this)
                bgod = null;

            lock (bosons)
            {
                foreach (BOSON boson in bosons.Values)
                    boson.Dispose();
                bosons.Clear();
            }
        }
    }
}