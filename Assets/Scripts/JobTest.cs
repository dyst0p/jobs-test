using Unity.Burst;
using Unity.Jobs;
using UnityEngine;

public class JobTest : MonoBehaviour
{
    [BurstCompile]
    private struct HelloJob : IJob
    {
        public void Execute()
        {
            Debug.Log("Hello. world!");
        }
    }

    private void Start()
    {
        var job = new HelloJob();
        JobHandle handle = job.Schedule();
        handle.Complete(); // выполнение задачи можно отложить
        
        // Можно сразу
        // jobA.Schedule().Complete();
        // Или так
        // job.Run();
        // Но в этом случае main thread будет ждать выполнения задачи тут же,
        // что в целом убивает всю идею параллельных вычислений,
        // но хорошо подходит для дебага и отладки
    }
}
