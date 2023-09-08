using Unity.Burst;
using Unity.Jobs;
using UnityEngine;

public class Pow3JobTest : MonoBehaviour
{
    [BurstCompile]
    private struct Pow3Job : IJob
    {
        public int num;
        
        public void Execute()
        {
            int result = num * num * num;
            Debug.Log($"{num}^3 = {result}");
        }
    }

    private int count;
    private JobHandle handle;
    
    void Update()
    {
        count++;
        Pow3Job job = new Pow3Job()
        {
            num = count // вариант с инициализатором
        };
        // job.num = count; тоже приемлемый вариант
        handle = job.Schedule();
    }

    private void LateUpdate()
    {
        handle.Complete();
    }
}
