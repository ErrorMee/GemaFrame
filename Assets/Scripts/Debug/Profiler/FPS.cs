using System;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
//Editor -> ProjectSetting -> QualitySettings >Donot Sync 关闭垂直同步
public class FPS : MonoBehaviour {

    private StringBuilder stringBuilder = new StringBuilder();

    private Text fpsText;

    public float udateInterval = 0.3F;

    private float lastInterval;

    private int frames = 0;

    private int fps;

    private void Awake()
    {
        fpsText = gameObject.GetComponent<Text>();
    }

    private void Start()
    {
        lastInterval = Time.realtimeSinceStartup;

        frames = 0;
    }

    void Update()
    {
        ++frames;

        if (Time.realtimeSinceStartup > lastInterval + udateInterval)
        {
            fps = (int)(frames / (Time.realtimeSinceStartup - lastInterval));

            frames = 0;

            lastInterval = Time.realtimeSinceStartup;

            stringBuilder.Remove(0, stringBuilder.Length);

            stringBuilder.Append("FPS ");
            stringBuilder.Append(StringUtil.stringsFrom00To99[Mathf.Clamp(fps, 0, 99)]);
            stringBuilder.Append("/");
            stringBuilder.Append(Application.targetFrameRate);

            

            long reservedMonoHeap = Profiler.GetMonoHeapSizeLong() / 1024L;
            long monoHeap = Profiler.GetMonoUsedSizeLong() / 1024L;
            long unityStack = Profiler.GetTempAllocatorSize() / 1024L;
            long total = Profiler.GetTotalAllocatedMemoryLong() / 1024L;
            long reservedTotal = Profiler.GetTotalReservedMemoryLong() / 1024L;

            stringBuilder.Append("\nmono:");
            stringBuilder.Append(monoHeap);
            stringBuilder.Append("/");
            stringBuilder.Append(reservedMonoHeap);
            stringBuilder.Append(StringUtil.K);

            stringBuilder.Append("\ntotal:");
            stringBuilder.Append(total);
            stringBuilder.Append("/");
            stringBuilder.Append(reservedTotal);
            stringBuilder.Append(StringUtil.K);

            stringBuilder.Append("\nstack:");
            stringBuilder.Append(unityStack);
            stringBuilder.Append(StringUtil.K);


            fpsText.text = stringBuilder.ToString();
        }
    }
}


/*
    1 GetMonoHeapSizeLong
    预留的托管堆空间大小
    当实际使用的托管堆超出时预留堆大小增加
    预留堆大小影响GC的频率和单次回收的消耗 预留堆越大gc消耗越大，gc的频率越小

    2 GetMonoUsedSizeLong
    实际分配的托管堆大小（包括可以但还没有GC的内存）

    3 GetRuntimeMemorySizeLong
    获取具体的Unity对象在运行时占用的大小

    4 GetTempAllocatorSize
    （栈）unity使用的运行时栈内存

    5 GetTotalAllocatedMemoryLong
    总的使用的内存

    6 GetTotalReservedMemoryLong
    总的预留的内存

    7 GetTotalUnusedReservedMemoryLong
    总的未使用的内存
*/
