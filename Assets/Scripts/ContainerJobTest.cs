using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class ContainerJobTest : MonoBehaviour
{
    struct VelocityJob : IJob
    {
        // В заданиях объявляются все данные, к которым будет осуществляться доступ в задании
        // Объявление данных только для чтения позволяет нескольким заданиям получать к ним доступ параллельно
        [ReadOnly]
        public NativeArray<Vector3> velocity;

        // По умолчанию предполагается, что контейнеры предназначены для чтения и записи
        public NativeArray<Vector3> position;

        // Time.deltaTime должно быть скопировано в задание, так как задания не имеют понятия кадра.
        // Главный поток ожидает выполнения задания на том же или следующем кадре, но задание должно
        // выполнять работу детерминированным и независимым образом при выполнении в рабочих потоках.
        public float deltaTime;

        // Код, фактически выполняемый в задании
        public void Execute()
        {
            // Перемещение позиций на основе дельты времени и скорости
            for (var i = 0; i < position.Length; i++)
                position[i] = position[i] + velocity[i] * deltaTime;
        }
    }

    private NativeArray<Vector3> position;
    private NativeArray<Vector3> velocity;
    
    private void Start()
    {
        position = new NativeArray<Vector3>(500, Allocator.Persistent);

        velocity = new NativeArray<Vector3>(500, Allocator.Persistent);
    }

    public void Update()
    {
        for (var i = 0; i < velocity.Length; i++)
            velocity[i] = new Vector3(0, 10, 0);


        // Инициализация данных задания
        var job = new VelocityJob()
        {
            deltaTime = Time.deltaTime,
            position = position,
            velocity = velocity
        };

        // Планирование выполнения задания, возвращает JobHandle, который можно использовать потом
        JobHandle jobHandle = job.Schedule();

        // Убедиться, что задание выполнено
        // Не рекомендуется завершать задание сразу,
        // так как это не дает реального параллелизма.
        // Оптимально планировать выполнение задания в начале кадра, а затем ожидать его в конце кадра.
        jobHandle.Complete();

        Debug.Log(job.position[0]);
    }

    private void OnDestroy()
    {
        // Нативные массивы должны утилизироваться вручную
        position.Dispose();
        velocity.Dispose();
    }
}