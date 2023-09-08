using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

public class MyDependentJob : MonoBehaviour
{
    // Создаем собственный массив из одного float для хранения результата. Тут будет результат завершения работы.
    NativeArray<float> result;
    // Создаем JobHandle для управления завершением задания
    JobHandle secondHandle;

    // Первое задание считает сумму
    public struct MyJob : IJob
    {
        public float a;
        public float b;
        public NativeArray<float> result;

        public void Execute()
        {
            result[0] = a + b;
        }
    }

    // Второе задание добавляет единицу
    public struct AddOneJob : IJob
    {
        public NativeArray<float> result;

        public void Execute()
        {
            result[0] = result[0] + 1;
        }
    }
    
    void Update()
    {
        result = new NativeArray<float>(1, Allocator.TempJob);

        // Настройка первого задания
        MyJob jobData = new MyJob
        {
            a = 10,
            b = 10,
            result = result
        };

        // Планирование первого задания
        JobHandle firstHandle = jobData.Schedule();

        // Настройка второго задания
        AddOneJob incJobData = new AddOneJob
        {
            result = result
        };

        // Планирование второго задания
        secondHandle = incJobData.Schedule(firstHandle);
    }

    private void LateUpdate()
    {
        // Немного ждём, чтобы задания успели отработать
        secondHandle.Complete();

        // Все копии NativeArray указывают на один и тот же участок нативной памяти
        // Можно получить доступ к результату в "своей" копии NativeArray
        // float aPlusBPlusOne = result[0];

        // Освободить память, выделенную под массив результатов
        result.Dispose();
    }

}