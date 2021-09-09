using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering
{
    /// <summary>
    /// Represents timing data captured from a single frame.
    /// </summary>
    internal struct FrameTimeSample
    {
        internal float FramesPerSecond;
        internal float FullFrameTime;
        internal float MainThreadCPUFrameTime;
        internal float MainThreadCPUPresentWaitTime;
        internal float RenderThreadCPUFrameTime;
        internal float GPUFrameTime;

        internal FrameTimeSample(float initValue)
        {
            FramesPerSecond = initValue;
            FullFrameTime = initValue;
            MainThreadCPUFrameTime = initValue;
            MainThreadCPUPresentWaitTime = initValue;
            RenderThreadCPUFrameTime = initValue;
            GPUFrameTime = initValue;
        }
    };

    /// <summary>
    /// Container class for sample history with helpers to calculate min, max and average in one pass.
    /// </summary>
    class FrameTimeSampleHistory
    {
        List<FrameTimeSample> m_Samples = new();

        internal FrameTimeSample SampleAverage;
        internal FrameTimeSample SampleMin;
        internal FrameTimeSample SampleMax;

        internal void Add(FrameTimeSample sample)
        {
            m_Samples.Add(sample);
        }

        internal void ComputeAggregateValues()
        {
            // Helper functions

            float SampleValueAdd(float value, float other)
            {
                return value + other;
            };

            float SampleValueMin(float value, float other)
            {
                return other > 0 ? Mathf.Min(value, other) : value;
            };

            float SampleValueMax(float value, float other)
            {
                return Mathf.Max(value, other);
            };

            float SampleValueCountValid(float value, float other)
            {
                return other > 0 ? value + 1 : value;
            };

            float SampleValueDivide(float value, float other)
            {
                return value / other;
            };

            void ForEachSampleMember(ref FrameTimeSample aggregate, FrameTimeSample sample, Func<float, float, float> func)
            {
                aggregate.FramesPerSecond = func(aggregate.FramesPerSecond, sample.FramesPerSecond);
                aggregate.FullFrameTime = func(aggregate.FullFrameTime, sample.FullFrameTime);
                aggregate.MainThreadCPUFrameTime = func(aggregate.MainThreadCPUFrameTime, sample.MainThreadCPUFrameTime);
                aggregate.MainThreadCPUPresentWaitTime = func(aggregate.MainThreadCPUPresentWaitTime, sample.MainThreadCPUPresentWaitTime);
                aggregate.RenderThreadCPUFrameTime = func(aggregate.RenderThreadCPUFrameTime, sample.RenderThreadCPUFrameTime);
                aggregate.GPUFrameTime = func(aggregate.GPUFrameTime, sample.GPUFrameTime);
            };

            FrameTimeSample average = new();
            FrameTimeSample min = new(float.MaxValue);
            FrameTimeSample max = new(float.MinValue);
            FrameTimeSample numValidSamples = new(); // Using the struct to record how many valid samples each field has

            for (int i = 0; i < m_Samples.Count; i++)
            {
                var s = m_Samples[i];

                ForEachSampleMember(ref min, s, SampleValueMin);
                ForEachSampleMember(ref max, s, SampleValueMax);
                ForEachSampleMember(ref average, s, SampleValueAdd);
                ForEachSampleMember(ref numValidSamples, s, SampleValueCountValid);
            }

            ForEachSampleMember(ref average, numValidSamples, SampleValueDivide);

            SampleAverage = average;
            SampleMin = min;
            SampleMax = max;
        }

        internal void DiscardOldSamples(int sampleHistorySize)
        {
            while (m_Samples.Count >= sampleHistorySize)
                m_Samples.RemoveAt(0);
        }

        internal void Clear()
        {
            m_Samples.Clear();
        }
    }
}
